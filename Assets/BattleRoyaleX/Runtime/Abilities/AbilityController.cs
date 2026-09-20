using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed partial class AbilityController : MonoBehaviour
    {
        CharacterRuntime runtime;
        readonly Dictionary<AbilitySlot, AbilityDefinition> equipped = new Dictionary<AbilitySlot, AbilityDefinition>();
        readonly Dictionary<AbilitySlot, float> readyAt = new Dictionary<AbilitySlot, float>();

        bool returnArmed;
        Vector3 returnPosition;
        float returnExpiresAt;
        bool comboInProgress;
        bool comboQueued;
        float comboQueueOpensAt;
        float comboQueueClosesAt;
        float comboCounterBonus;
        float comboCounterKnockback;
        Coroutine comboRoutine;
        bool actionBusy;
        int actionSerial;

        bool chargedSequenceActive;
        int chargedSequenceRemaining;
        float chargedSequenceExpiresAt;
        AbilityDefinition chargedSequenceAbility;
        float counterExpiresAt;
        float counterBonusDamage;
        float counterKnockback;
        AbilityDefinition defenseJourney;
        float defenseJourneyUntil;
        bool defenseRedirected;
        AbilityDefinition currentAction;

        public bool IsComboInProgress => comboInProgress;
        public bool IsActionBusy => actionBusy;
        public bool IsChargedSequenceActive => chargedSequenceActive && Time.time <= chargedSequenceExpiresAt;
        public int ChargedSequenceRemaining => IsChargedSequenceActive ? chargedSequenceRemaining : 0;
        public float ChargedSequenceTimeRemaining => IsChargedSequenceActive ? Mathf.Max(0f, chargedSequenceExpiresAt - Time.time) : 0f;
        public bool HasCounterOpportunity => counterBonusDamage > 0f && Time.time <= counterExpiresAt;
        public int ResetVersion { get; private set; }

        void Awake() => runtime = GetComponent<CharacterRuntime>();

        void Update()
        {
            if (Time.time >= defenseJourneyUntil) defenseJourney = null;
            if (huntAbility != null && (Time.time > huntExpiresAt || huntTarget == null || huntTarget.Health.IsDead)) ClearHunt();
            if (runtime != null && runtime.Health.IsDead && (actionBusy || comboInProgress || chargedSequenceActive || HasCounterOpportunity))
                ResetTransientState();
            if (chargedSequenceActive && (Time.time > chargedSequenceExpiresAt || runtime == null || runtime.Health.IsDead))
                EndChargedSequence(AbilityPhase.Completed);
            if (counterBonusDamage > 0f && Time.time > counterExpiresAt)
                ClearCounterOpportunity();
        }

        public void Initialize(CharacterDefinition definition)
        {
            ResetTransientState();
            equipped[AbilitySlot.BasicAttack] = definition.basicAttack;
            equipped[AbilitySlot.Defense] = definition.defenseBase;
            equipped[AbilitySlot.Movement] = definition.movementBase;
            equipped[AbilitySlot.Ultimate] = definition.ultimateBase;
            readyAt.Clear();
            ClearRuntimeFlags();
        }

        public AbilityDefinition GetEquipped(AbilitySlot slot) => equipped.TryGetValue(slot, out var value) ? value : null;
        public bool TryUse(AbilitySlot slot) => TryUseInternal(slot, Vector3.zero, false);
        public bool TryUse(AbilitySlot slot, Vector3 worldDirection) => TryUseInternal(slot, worldDirection, worldDirection.sqrMagnitude > 0.001f);

        bool TryUseInternal(AbilitySlot slot, Vector3 worldDirection, bool hasDirection)
        {
            if (runtime == null || runtime.Health == null || runtime.Health.IsDead) return false;
            AbilityDefinition ability = GetEquipped(slot);
            if (ability == null) return false;
            // Movement/defense remain available to escape, but damage is suppressed centrally while inside smoke.
            if (SmokeField.PreventsAttack(runtime) && (slot == AbilitySlot.BasicAttack || slot == AbilitySlot.Ultimate)) return false;

            if (ability.behavior == AbilityBehavior.HuntSequence && IsHuntRecastReady)
                return TryHuntRecast();

            if (slot == AbilitySlot.Ultimate && chargedSequenceActive && ability == chargedSequenceAbility)
                return TryChargedDashRecast(ability, worldDirection, hasDirection);

            if (slot == AbilitySlot.BasicAttack && ability.behavior == AbilityBehavior.MeleeAttack && ability.comboSteps > 1)
                return TryUseBasicCombo(ability, worldDirection, hasDirection);

            if (runtime.State.SkillsLocked || actionBusy || runtime.Motor.IsDashing) return false;
            if (ability.behavior == AbilityBehavior.DashReturn && returnArmed && Time.time <= returnExpiresAt)
            {
                Vector3 delta = returnPosition - transform.position;
                if (delta.sqrMagnitude < 0.01f) return false;
                runtime.Defense.Deactivate();
                runtime.Motor.FaceDirection(delta);
                actionBusy = true;
                int returnId = ++actionSerial;
                StartCoroutine(ExecuteReturn(ability, delta.magnitude, returnId));
                returnArmed = false;
                return true;
            }

            float currentReadyAt = readyAt.TryGetValue(slot, out float t) ? t : 0f;
            CharacterRuntime acquired = ability.behavior == AbilityBehavior.HuntSequence ? FindHuntTarget(ability, worldDirection) : null;
            if (ability.behavior == AbilityBehavior.HuntSequence && acquired == null) return false;
            if (Time.time < currentReadyAt || !runtime.Energy.TrySpend(ability.energyCost)) return false;
            if (comboInProgress) CancelBasicCombo();
            if (hasDirection) runtime.Motor.FaceDirection(worldDirection);
            if (slot != AbilitySlot.Defense) runtime.Defense.Deactivate();

            float cooldown = ability.cooldown;
            if (slot == AbilitySlot.Movement) cooldown *= runtime.Modifiers.movementCooldownMultiplier;
            readyAt[slot] = Time.time + Mathf.Max(0f, cooldown);

            if (ability.behavior == AbilityBehavior.ChargedDashSequence)
                return BeginChargedSequence(ability);
            if (ability.behavior == AbilityBehavior.HuntSequence)
            {
                huntAbility = ability; huntTarget = acquired; huntExpiresAt = Time.time + ability.chargeWindow;
                return StartHuntStage(false);
            }

            actionBusy = true;
            int id = ++actionSerial;
            currentAction = ability;
            defenseRedirected = false;
            if (ability.slot == AbilitySlot.Defense && ability.behavior == AbilityBehavior.Dodge)
                runtime.State.SetInvulnerable(ability.startup + ability.invulnerabilityDuration);
            CharacterVisualAnimator visuals = GetComponentInChildren<CharacterVisualAnimator>(true);
            if (visuals != null) visuals.PlayAbility(ability);
            CombatEvents.Raise(new CombatEventData(AbilityEventFor(ability), transform.position, runtime, runtime,
                ability.behavior == AbilityBehavior.UltimateBuff ? ability.buffDuration : 0f,
                ability, AbilityPhase.Startup, id, runtime.Motor.Facing));
            StartCoroutine(Execute(ability, id));
            return true;
        }

        bool TryUseBasicCombo(AbilityDefinition ability, Vector3 worldDirection, bool hasDirection)
        {
            if (runtime.State.BasicAttackLocked || actionBusy || runtime.Motor.IsDashing) return false;
            if (comboInProgress)
            {
                if (Time.time < comboQueueOpensAt || Time.time > comboQueueClosesAt || comboQueued) return false;
                comboQueued = true;
                return true;
            }
            float ready = readyAt.TryGetValue(AbilitySlot.BasicAttack, out float value) ? value : 0f;
            if (Time.time < ready || !runtime.Energy.TrySpend(ability.energyCost)) return false;
            if (hasDirection) runtime.Motor.FaceDirection(worldDirection);
            runtime.Defense.Deactivate();
            comboCounterBonus = ConsumeCounterBonus();
            readyAt[AbilitySlot.BasicAttack] = Time.time + Mathf.Max(0.05f, ability.cooldown);
            comboRoutine = StartCoroutine(ExecuteBasicCombo(ability));
            return true;
        }

        IEnumerator ExecuteBasicCombo(AbilityDefinition ability)
        {
            comboInProgress = true;
            int comboId = ++actionSerial;
            for (int step = 1; step <= ability.comboSteps; step++)
            {
                comboQueued = false;
                CharacterVisualAnimator visuals = GetComponentInChildren<CharacterVisualAnimator>();
                if (visuals != null) visuals.PlayBasicStep(step);
                CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityAttack, transform.position, runtime, runtime,
                    step, ability, AbilityPhase.Startup, comboId, runtime.Motor.Facing));
                float startup = ComboStartup(ability, step);
                if (startup > 0f) yield return new WaitForSeconds(startup);
                if (runtime.Health.IsDead || runtime.State.IsStaggered || comboId != actionSerial) break;
                CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityAttack, transform.position, runtime, runtime,
                    step, ability, AbilityPhase.Active, comboId, runtime.Motor.Facing));
                SpawnMeleeHitbox(ability, ComboDamageMultiplier(ability, step), step == ability.comboSteps ? 1.08f : 1f,
                    step == 1 ? comboCounterBonus : 0f, step == 1 ? comboCounterKnockback : 0f);
                float recovery = ComboRecovery(ability, step);
                comboQueueOpensAt = Time.time + Mathf.Max(0f, recovery - ability.comboInputBuffer);
                comboQueueClosesAt = Time.time + recovery;
                CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityAttack, transform.position, runtime, runtime,
                    step, ability, AbilityPhase.Recovery, comboId, runtime.Motor.Facing));
                runtime.State.SetActionRecovery(true);
                yield return new WaitForSeconds(recovery);
                runtime.State.SetActionRecovery(false);
                if (!comboQueued || step == ability.comboSteps) break;
            }
            comboCounterBonus = 0f;
            comboCounterKnockback = 0f;
            comboQueued = false;
            comboInProgress = false;
            comboRoutine = null;
            runtime.State.SetActionRecovery(false);
        }

        void CancelBasicCombo()
        {
            if (comboRoutine != null) StopCoroutine(comboRoutine);
            comboRoutine = null;
            comboInProgress = comboQueued = false;
            comboCounterBonus = comboCounterKnockback = 0f;
            runtime.State.SetActionRecovery(false);
            foreach (Hitbox hit in FindObjectsByType<Hitbox>())
                if (hit.Owner == runtime && hit.Packet.ability != null && hit.Packet.ability.slot == AbilitySlot.BasicAttack)
                    hit.Cancel();
        }

        bool BeginChargedSequence(AbilityDefinition ability)
        {
            chargedSequenceActive = true;
            chargedSequenceAbility = ability;
            chargedSequenceRemaining = Mathf.Max(1, ability.chargeCount);
            chargedSequenceExpiresAt = Time.time + Mathf.Max(0.1f, ability.chargeWindow);
            CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityUltimate, transform.position, runtime, runtime,
                ability.chargeWindow, ability, AbilityPhase.Startup, ++actionSerial, runtime.Motor.Facing));
            return StartChargedDash(ability);
        }

        bool TryChargedDashRecast(AbilityDefinition ability, Vector3 worldDirection, bool hasDirection)
        {
            if (!IsChargedSequenceActive || chargedSequenceRemaining <= 0 || runtime.State.SkillsLocked || actionBusy || runtime.Motor.IsDashing)
                return false;
            if (hasDirection) runtime.Motor.FaceDirection(worldDirection);
            runtime.Defense.Deactivate();
            return StartChargedDash(ability);
        }

        bool StartChargedDash(AbilityDefinition ability)
        {
            if (chargedSequenceRemaining <= 0) return false;
            chargedSequenceRemaining--;
            actionBusy = true;
            int id = ++actionSerial;
            currentAction = ability;
            int castNumber = Mathf.Max(1, ability.chargeCount) - chargedSequenceRemaining;
            CharacterVisualAnimator visuals = GetComponentInChildren<CharacterVisualAnimator>();
            if (visuals != null) visuals.PlayChargedDash();
            CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityMove, transform.position, runtime, runtime,
                castNumber, ability, AbilityPhase.Startup, id, runtime.Motor.Facing));
            StartCoroutine(ExecuteChargedDash(ability, id, castNumber));
            return true;
        }

        IEnumerator ExecuteChargedDash(AbilityDefinition ability, int id, int castNumber)
        {
            if (ability.startup > 0f) yield return new WaitForSeconds(ability.startup);
            if (!ActionStillValid(id)) { FinishAction(id); yield break; }
            CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityMove, transform.position, runtime, runtime,
                castNumber, ability, AbilityPhase.Active, id, runtime.Motor.Facing));
            ExecuteMovement(ability, ability.movementDistance);
            while (runtime.Motor.IsDashing && !runtime.Health.IsDead) yield return null;
            if (ActionStillValid(id) && ability.recovery > 0f)
            {
                CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityMove, transform.position, runtime, runtime,
                    castNumber, ability, AbilityPhase.Recovery, id, runtime.Motor.Facing));
                runtime.State.SetActionRecovery(true);
                yield return new WaitForSeconds(ability.recovery);
                runtime.State.SetActionRecovery(false);
            }
            FinishAction(id);
            if (chargedSequenceRemaining <= 0) EndChargedSequence(AbilityPhase.Completed);
        }

        IEnumerator ExecuteReturn(AbilityDefinition ability, float distance, int id)
        {
            currentAction = ability;
            CharacterVisualAnimator visuals = GetComponentInChildren<CharacterVisualAnimator>();
            if (visuals != null) visuals.PlayAbility(ability);
            CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityMove, transform.position, runtime, runtime,
                0f, ability, AbilityPhase.Active, id, runtime.Motor.Facing));
            ExecuteMovement(ability, distance);
            while (runtime.Motor.IsDashing && !runtime.Health.IsDead) yield return null;
            if (ability.recovery > 0f)
            {
                runtime.State.SetActionRecovery(true);
                yield return new WaitForSeconds(ability.recovery);
                runtime.State.SetActionRecovery(false);
            }
            FinishAction(id);
        }

        static float ComboStartup(AbilityDefinition ability, int step) => step == 1 ? ability.startup : step == 2 ? ability.startup * 1.15f : ability.startup * 1.45f;
        static float ComboRecovery(AbilityDefinition ability, int step) => step == 1 ? ability.recovery : step == 2 ? ability.recovery * 1.12f : ability.recovery * 1.75f;
        static float ComboDamageMultiplier(AbilityDefinition ability, int step) => step == 2 ? ability.comboSecondDamageMultiplier : step == 3 ? ability.comboThirdDamageMultiplier : 1f;

        static CombatEventKind AbilityEventFor(AbilityDefinition ability)
        {
            switch (ability.behavior)
            {
                case AbilityBehavior.Guard:
                case AbilityBehavior.Parry:
                case AbilityBehavior.SmokeEscape: return CombatEventKind.AbilityGuard;
                case AbilityBehavior.Dodge:
                case AbilityBehavior.Dash:
                case AbilityBehavior.DashThrough:
                case AbilityBehavior.DashReturn:
                case AbilityBehavior.ChargedDashSequence:
                case AbilityBehavior.HuntSequence: return CombatEventKind.AbilityMove;
                case AbilityBehavior.UltimateBuff: return CombatEventKind.AbilityUltimate;
                default: return CombatEventKind.AbilityAttack;
            }
        }

        IEnumerator Execute(AbilityDefinition ability, int id)
        {
            if (ability.startup > 0f) yield return new WaitForSeconds(ability.startup);
            if (!ActionStillValid(id)) { FinishAction(id); yield break; }
            CombatEvents.Raise(new CombatEventData(AbilityEventFor(ability), transform.position, runtime, runtime,
                ability.behavior == AbilityBehavior.UltimateBuff ? ability.buffDuration : 0f,
                ability, AbilityPhase.Active, id, runtime.Motor.Facing));
            switch (ability.behavior)
            {
                case AbilityBehavior.MeleeAttack: SpawnMeleeHitbox(ability, 1f, 1f, ConsumeCounterBonus()); break;
                case AbilityBehavior.ProjectileAttack: SpawnProjectileHitbox(ability); break;
                case AbilityBehavior.AreaAttack: SpawnAreaHitbox(ability); break;
                case AbilityBehavior.Guard:
                case AbilityBehavior.Parry:
                    runtime.Defense.Activate(ability.defenseKind, ability.defenseDuration, ability.perfectWindow,
                        ability.damageReduction, ability.specialInteractionCooldown, ability);
                    FinishAction(id);
                    yield break;
                case AbilityBehavior.Dodge:
                    defenseJourney = ability.slot == AbilitySlot.Defense ? ability : null;
                    defenseJourneyUntil = Time.time + ability.defenseDuration;
                    ExecuteMovement(ability, ability.movementDistance);
                    while (runtime.Motor.IsDashing && !runtime.Health.IsDead) yield return null;
                    break;
                case AbilityBehavior.SmokeEscape:
                    SmokeField.Spawn(transform.position, ability.smokeRadius, ability.smokeDuration, true);
                    break;
                case AbilityBehavior.Dash:
                case AbilityBehavior.DashThrough:
                    ExecuteMovement(ability, ability.movementDistance);
                    while (runtime.Motor.IsDashing && !runtime.Health.IsDead) yield return null;
                    if (ActionStillValid(id) && ability.speedBonusDuration > 0f)
                        runtime.ApplyMovementSpeedBonus(ability.speedBonusMultiplier, ability.speedBonusDuration);
                    break;
                case AbilityBehavior.DashReturn:
                    returnPosition = transform.position;
                    returnExpiresAt = Time.time + ability.returnWindow;
                    returnArmed = true;
                    ExecuteMovement(ability, ability.movementDistance);
                    while (runtime.Motor.IsDashing && !runtime.Health.IsDead) yield return null;
                    break;
                case AbilityBehavior.UltimateBuff:
                    if (ability.damage > 0f) SpawnAreaHitbox(ability);
                    runtime.ApplyTimedModifiers(ability.ToRuntimeModifiers(), ability.buffDuration);
                    break;
            }
            if (ActionStillValid(id) && ability.recovery > 0f)
            {
                runtime.State.SetActionRecovery(true);
                yield return new WaitForSeconds(ability.recovery);
                runtime.State.SetActionRecovery(false);
            }
            FinishAction(id);
        }

        bool ActionStillValid(int id) => id == actionSerial && runtime != null && runtime.Health != null && !runtime.Health.IsDead;
        void FinishAction(int id)
        {
            if (id != actionSerial) return;
            actionBusy = false;
            runtime.State.SetActionRecovery(false);
            if (currentAction != null)
                CombatEvents.Raise(new CombatEventData(AbilityEventFor(currentAction), transform.position, runtime, runtime,
                    ability: currentAction, phase: AbilityPhase.Completed, actionId: id));
            currentAction = null;
        }

        public void InterruptOffensiveAction()
        {
            CancelBasicCombo();
            if (!actionBusy) return;
            actionSerial++;
            runtime.Motor.CancelDash();
            foreach (Hitbox hit in FindObjectsByType<Hitbox>())
                if (hit != null && hit.Owner == runtime) hit.Cancel();
            actionBusy = false;
            currentAction = null;
            runtime.State.SetActionRecovery(false);
        }

        public bool TryDefensiveRedirect(CharacterRuntime attacker, bool parried)
        {
            AbilityDefinition defense = parried ? runtime.Defense.ActiveAbility : defenseJourney;
            if (defense == null || !defense.redirectOnDefense || defenseRedirected || attacker == null ||
                attacker.TeamId == runtime.TeamId || runtime.Health.IsDead || attacker.Health.IsDead) return false;
            Vector3 delta = attacker.transform.position - transform.position;
            delta.y = 0f;
            // Only a nearby physical exchange redirects the character; distant projectiles cannot teleport him.
            if (delta.magnitude > defense.redirectDistance - 3f) return false;
            if (!parried && Time.time >= defenseJourneyUntil) return false;
            defenseRedirected = true;
            defenseJourney = null;
            Vector3 direction = delta.sqrMagnitude > 0.01f ? delta.normalized : -runtime.Motor.Facing;
            float distance = Mathf.Min(defense.redirectDistance, delta.magnitude + 4f);
            {
                runtime.Motor.CancelDash();
                CancelBasicCombo();
                runtime.Motor.FaceDirection(direction);
                runtime.Defense.Deactivate();
                actionBusy = true;
                currentAction = defense;
                StartCoroutine(ExecuteDefensiveRedirect(defense, distance, ++actionSerial));
            }
            CombatEvents.Raise(new CombatEventData(CombatEventKind.DefenseRedirect, transform.position, runtime, attacker,
                ability: defense, phase: AbilityPhase.Active, actionId: actionSerial, direction: direction));
            return true;
        }

        IEnumerator ExecuteDefensiveRedirect(AbilityDefinition defense, float distance, int id)
        {
            // Defer past ResolveAttack: never recurse into physics resolution inside the incoming contact.
            yield return null;
            if (!ActionStillValid(id)) { FinishAction(id); yield break; }
            ExecuteMovement(defense, distance, defense.redirectDuration, true);
            while (runtime.Motor.IsDashing && !runtime.Health.IsDead) yield return null;
            yield return new WaitForSeconds(defense.recovery);
            FinishAction(id);
        }

        void ExecuteMovement(AbilityDefinition ability, float distance, float durationOverride = 0f, bool reactive = false)
        {
            if (runtime.Motor.IsDashing) return;
            float duration = durationOverride > 0f ? durationOverride : ability.movementDuration;
            Hitbox sweep = null;
            if (ability.damage > 0f && (ability.slot != AbilitySlot.Defense || reactive))
            {
                GameObject go = new GameObject("MovementHit_" + ability.abilityId);
                go.transform.position = transform.position;
                sweep = go.AddComponent<Hitbox>();
                DamagePacket packet = new DamagePacket(runtime, ability, runtime.Motor.Facing) { knockback = 0f, clashable = false };
                sweep.Configure(runtime, packet, Vector3.one, duration + ability.redirectDuration + 0.2f);
                go.GetComponent<Collider>().enabled = false;
            }
            runtime.Motor.Dash(distance, duration, ability.passThroughCharacters,
                reactive ? duration : ability.invulnerabilityDuration, (from, to) =>
                {
                    if (sweep == null || sweep.Cancelled) return;
                    sweep.transform.position = to;
                    CharacterController ownController = GetComponent<CharacterController>();
                    float skin = ownController != null ? ownController.skinWidth : 0.08f;
                    foreach (Collider collider in Physics.OverlapCapsule(from + Vector3.up, to + Vector3.up,
                        Mathf.Max(0.35f, ability.width * 0.5f) + skin * 2f, ~0, QueryTriggerInteraction.Collide))
                        sweep.TryResolveHurtbox(collider.GetComponent<Hurtbox>());
                });
        }

        void SpawnMeleeHitbox(AbilityDefinition ability, float damageMultiplier = 1f, float rangeMultiplier = 1f, float bonusDamage = 0f, float counterPush = 0f)
        {
            GameObject go = new GameObject($"Hitbox_{ability.displayName}");
            float range = ability.range * rangeMultiplier;
            go.transform.position = transform.position + runtime.Motor.Facing * Mathf.Max(0.2f, range * 0.5f);
            go.transform.rotation = Quaternion.LookRotation(runtime.Motor.Facing, Vector3.up);
            go.layer = gameObject.layer;
            Hitbox hitbox = go.AddComponent<Hitbox>();
            DamagePacket packet = new DamagePacket(runtime, ability, runtime.Motor.Facing);
            packet.damage = packet.damage * damageMultiplier + bonusDamage * runtime.Modifiers.damageMultiplier;
            packet.knockback *= damageMultiplier;
            packet.isCounter = bonusDamage > 0f || counterPush > 0f;
            if (packet.isCounter) packet.knockback = Mathf.Max(packet.knockback, counterPush);
            hitbox.Configure(runtime, packet, new Vector3(ability.width, ability.height, range), ability.activeTime);
        }

        void SpawnProjectileHitbox(AbilityDefinition ability)
        {
            GameObject go = new GameObject($"Projectile_{ability.displayName}");
            go.transform.position = transform.position + runtime.Motor.Facing * 1.1f + Vector3.up * 0.8f;
            go.transform.rotation = Quaternion.LookRotation(runtime.Motor.Facing, Vector3.up);
            Hitbox hitbox = go.AddComponent<Hitbox>();
            DamagePacket packet = new DamagePacket(runtime, ability, runtime.Motor.Facing);
            hitbox.Configure(runtime, packet, new Vector3(ability.width, ability.height, Mathf.Max(0.5f, ability.width)), 4f);
            ProjectileHitboxMover mover = go.AddComponent<ProjectileHitboxMover>();
            mover.direction = runtime.Motor.Facing;
            mover.speed = ability.projectileSpeed;
            mover.maxLifetime = 4f;
        }

        void SpawnAreaHitbox(AbilityDefinition ability)
        {
            Vector3 center = transform.position + runtime.Motor.Facing * Mathf.Max(0f, ability.range);
            GameObject go = new GameObject($"Area_{ability.displayName}");
            go.transform.position = center;
            Hitbox hitbox = go.AddComponent<Hitbox>();
            DamagePacket packet = new DamagePacket(runtime, ability, runtime.Motor.Facing);
            float radius = Mathf.Max(0.25f, ability.explosionRadius);
            hitbox.Configure(runtime, packet, Vector3.one * 0.1f, ability.activeTime);
            go.GetComponent<Collider>().enabled = false;
            foreach (Collider collider in Physics.OverlapSphere(center, radius, ~0, QueryTriggerInteraction.Collide))
                hitbox.TryResolveHurtbox(collider.GetComponent<Hurtbox>());
        }

        public void GrantCounterOpportunity(AbilityDefinition defendedWith = null)
        {
            AbilityDefinition defense = defendedWith != null ? defendedWith : GetEquipped(AbilitySlot.Defense);
            if (defense == null || defense.counterBonusDamage <= 0f || defense.counterWindow <= 0f) return;
            counterBonusDamage = defense.counterBonusDamage;
            counterKnockback = defense.counterKnockback;
            counterExpiresAt = Time.time + defense.counterWindow;
            CombatEvents.Raise(new CombatEventData(CombatEventKind.CounterReady, transform.position, runtime, runtime,
                defense.counterWindow, defense));
        }

        float ConsumeCounterBonus()
        {
            if (!HasCounterOpportunity) { ClearCounterOpportunity(); return 0f; }
            float result = counterBonusDamage;
            comboCounterKnockback = counterKnockback;
            ClearCounterOpportunity();
            return result;
        }

        void ClearCounterOpportunity() { counterBonusDamage = counterKnockback = 0f; counterExpiresAt = 0f; }

        void EndChargedSequence(AbilityPhase phase)
        {
            if (!chargedSequenceActive) return;
            AbilityDefinition ended = chargedSequenceAbility;
            chargedSequenceActive = false;
            chargedSequenceRemaining = 0;
            chargedSequenceExpiresAt = 0f;
            chargedSequenceAbility = null;
            CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityUltimate, transform.position, runtime, runtime,
                0f, ended, phase, actionSerial, runtime != null && runtime.Motor != null ? runtime.Motor.Facing : Vector3.forward));
        }

        public void ResetTransientState()
        {
            ResetVersion++;
            StopAllCoroutines();
            actionSerial++;
            foreach (Hitbox hitbox in FindObjectsByType<Hitbox>())
            {
                if (hitbox != null && hitbox.Owner == runtime)
                {
                    hitbox.Cancel();
                    Destroy(hitbox.gameObject);
                }
            }
            if (runtime != null)
            {
                if (runtime.Motor != null) runtime.Motor.ResetTransientState();
                if (runtime.Defense != null) runtime.Defense.ResetTransientState();
                if (runtime.State != null) runtime.State.ResetTransientState();
                runtime.CancelTimedModifiers();
            }
            ClearRuntimeFlags();
        }

        void ClearRuntimeFlags()
        {
            returnArmed = false;
            comboInProgress = comboQueued = false;
            comboCounterBonus = 0f;
            comboCounterKnockback = 0f;
            comboRoutine = null;
            currentAction = defenseJourney = null;
            defenseJourneyUntil = 0f;
            ClearHunt();
            defenseRedirected = false;
            actionBusy = false;
            if (runtime != null && runtime.State != null) runtime.State.SetActionRecovery(false);
            chargedSequenceActive = false;
            chargedSequenceRemaining = 0;
            chargedSequenceExpiresAt = 0f;
            chargedSequenceAbility = null;
            ClearCounterOpportunity();
        }

        public bool EquipVariation(AbilityDefinition variation)
        {
            if (variation == null || variation.slot == AbilitySlot.BasicAttack) return false;
            if (variation.classRestricted && runtime.Definition != null && runtime.Definition.characterClass != variation.requiredClass) return false;
            if (variation.slot == AbilitySlot.Ultimate) { EndChargedSequence(AbilityPhase.Cancelled); ClearHunt(); }
            if (variation.slot == AbilitySlot.Defense) { runtime.Defense.Deactivate(); defenseJourney = null; ClearCounterOpportunity(); }
            equipped[variation.slot] = variation;
            CombatEvents.Raise(new CombatEventData(CombatEventKind.VariationSwap, transform.position, runtime, runtime,
                ability: variation));
            return true;
        }

        public bool EquipLabVariation(AbilityDefinition variation)
        {
            if (!EquipVariation(variation)) return false;
            readyAt[variation.slot] = 0f;
            return true;
        }

        public float GetCooldownRemaining(AbilitySlot slot)
        {
            float t = readyAt.TryGetValue(slot, out float value) ? value : 0f;
            return Mathf.Max(0f, t - Time.time);
        }

        public void ReduceAllCooldowns(float seconds)
        {
            if (seconds <= 0f) return;
            var keys = new List<AbilitySlot>(readyAt.Keys);
            foreach (var key in keys) readyAt[key] = Mathf.Max(Time.time, readyAt[key] - seconds);
        }
    }
}
