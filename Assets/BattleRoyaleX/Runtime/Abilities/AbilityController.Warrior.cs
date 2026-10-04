using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed partial class AbilityController
    {
        AbilityDefinition capturedSkill;
        AbilityDefinition activeBorrowedSkill;
        int warriorGeneration;
        readonly List<GameObject> warriorVisuals = new List<GameObject>();
        readonly List<AbilityDefinition> borrowedSkills = new List<AbilityDefinition>();
        float capturedUntil, warriorGuardUntil, warriorFortressUntil, warriorReductionUntil, warriorReduction;
        AbilityDefinition warriorGuardAbility;
        public bool IsWarrior => runtime != null && runtime.Definition != null && runtime.Definition.characterClass == CharacterClass.Warrior;
        public bool HasCapturedSkill => capturedSkill != null && Time.time <= capturedUntil && runtime != null && !runtime.Health.IsDead;
        public string CapturedSkillName => HasCapturedSkill ? capturedSkill.displayName : "";
        public string CapturedSkillId => HasCapturedSkill ? capturedSkill.abilityId : "";
        public float CaptureTimeRemaining => HasCapturedSkill ? Mathf.Max(0f, capturedUntil - Time.time) : 0f;
        public bool HasFortressDamageImmunity => Time.time < warriorFortressUntil;
        public int PresentationActionId => actionSerial;
        public AbilityDefinition GetAimDefinition(AbilitySlot slot)
        {
            AbilityDefinition equippedAbility = GetEquipped(slot);
            if (equippedAbility == null || equippedAbility.behavior != AbilityBehavior.WarriorSkillCapture) return equippedAbility;
            return HasCapturedSkill ? capturedSkill : CanBorrowedRecast() ? activeBorrowedSkill : equippedAbility;
        }
        public float BasicAttackSpeed => Mathf.Max(0.1f, runtime != null && runtime.Modifiers.attackSpeedMultiplier > 0f ? runtime.Modifiers.attackSpeedMultiplier : 1f) *
            (Time.time < archerOverloadUntil && (runtime == null || runtime.Modifiers.attackSpeedMultiplier <= 1f) ? 1.5f : 1f);

        void UpdateWarriorEffects()
        {
            if (capturedSkill != null && (Time.time > capturedUntil || runtime == null || runtime.Health.IsDead)) ClearCapturedSkill();
            if (runtime != null && runtime.Health.IsDead && (warriorGuardUntil > Time.time || warriorFortressUntil > Time.time || borrowedSkills.Count > 0))
                ResetTransientState();
        }

        public bool TryWarriorProtection(DamagePacket packet, out float damage, out CombatOutcome outcome)
        {
            damage = packet.damage; outcome = CombatOutcome.Hit;
            if (Time.time < warriorGuardUntil || Time.time < warriorFortressUntil)
            { damage = 0f; outcome = CombatOutcome.Blocked; return true; }
            if (Time.time < warriorReductionUntil)
            { damage *= 1f - Mathf.Clamp01(warriorReduction); outcome = CombatOutcome.Blocked; return true; }
            return false;
        }

        public bool TryCaptureSkill(DamagePacket packet)
        {
            if (!IsWarrior || packet.ability == null || packet.ability.slot == AbilitySlot.BasicAttack ||
                HasCapturedSkill || packet.source == null || packet.source.TeamId == runtime.TeamId) return false;
            // Captured copies may be absorbed but never copied again: no infinite replay chain.
            if (!borrowedSkills.Contains(packet.ability) && packet.ability.name.IndexOf("BRX_Captured_", System.StringComparison.Ordinal) < 0)
            {
                capturedSkill = Instantiate(packet.ability);
                capturedSkill.name = "BRX_Captured_" + packet.ability.abilityId;
                capturedSkill.hideFlags = HideFlags.DontSave;
                AbilityDefinition capture = GetEquipped(AbilitySlot.Skill1);
                capturedUntil = Time.time + Mathf.Max(0.1f, capture != null ? capture.captureRecastWindow : 2f);
            }
            return true;
        }

        void BeginWarriorCapture(AbilityDefinition ability)
        {
            ClearCapturedSkill();
            runtime.Defense.Activate(DefenseKind.Reflect, ability.defenseDuration, 0f, 0f, 0f, ability);
            SpawnWarriorPresentation(ability, WarriorSkillPresentation.Kind.Absorb, ability.defenseDuration, transform.position, 1.1f);
        }

        bool TryCastCapturedSkill(Vector3 direction, bool manual, Vector3? point)
        {
            if (!HasCapturedSkill || runtime.State.SkillsLocked || runtime.Motor.IsDashing) return false;
            AbilityDefinition snapshot = capturedSkill;
            if (SmokeField.PreventsAttack(runtime) && IsOffensiveBehavior(snapshot.behavior)) return false;
            if (actionBusy && currentAction != null && currentAction.slot == AbilitySlot.BasicAttack) InterruptOffensiveAction();
            if (actionBusy) return false;
            if (comboInProgress) CancelBasicCombo();
            CharacterRuntime target = manual ? FindPursuitTargetForSnapshot(snapshot, direction) : FindNearestVisibleTarget(Mathf.Max(snapshot.range, snapshot.huntAcquireRange));
            if (!manual && target != null) direction = target.transform.position - transform.position;
            if (direction.sqrMagnitude < 0.001f) direction = runtime.Motor.Facing;
            if ((snapshot.behavior == AbilityBehavior.ExecutionStrike || snapshot.behavior == AbilityBehavior.HuntSequence) && target == null) return false;
            capturedSkill = null; capturedUntil = 0f; borrowedSkills.Add(snapshot); activeBorrowedSkill = snapshot;
            runtime.Defense.Deactivate(); runtime.Motor.FaceDirection(direction);
            actionBusy = true; currentAction = snapshot;
            actionAimPoint = AbilityAimSolution.ClampGroundPoint(transform.position,
                point ?? (target != null ? target.transform.position : transform.position + runtime.Motor.Facing * snapshot.range), snapshot);
            int id = ++actionSerial;
            CombatEvents.Raise(new CombatEventData(AbilityEventFor(snapshot), transform.position, runtime, target,
                ability: snapshot, phase: AbilityPhase.Startup, actionId: id, direction: runtime.Motor.Facing));
            // Execute the original behavior, not an invented generic projectile.
            if (snapshot.behavior == AbilityBehavior.HuntSequence)
            { huntAbility = snapshot; huntTarget = target; huntExpiresAt = Time.time + snapshot.chargeWindow; actionBusy = false; return StartHuntStage(false); }
            if (snapshot.behavior == AbilityBehavior.ChargedDashSequence || snapshot.behavior == AbilityBehavior.MultiDash)
            { actionBusy = false; return BeginChargedSequence(snapshot); }
            StartCoroutine(Execute(snapshot, id, target));
            return true;
        }

        bool CanBorrowedRecast()
        {
            if (activeBorrowedSkill == null) return false;
            if (CanDaggerRecast(activeBorrowedSkill) || IsHuntRecastReady || IsChargedSequenceActive ||
                activeBorrowedSkill.behavior == AbilityBehavior.DashReturn && returnArmed && Time.time <= returnExpiresAt) return true;
            var clones = GetComponent<CloneTeleportController>();
            if (clones != null && clones.Ability == activeBorrowedSkill && clones.CanTeleport) return true;
            var convergence = GetComponent<MageConvergenceController>();
            return convergence != null && convergence.Ability == activeBorrowedSkill && convergence.CanFireSecond;
        }

        bool TryBorrowedRecast(Vector3 direction, bool manual)
        {
            AbilityDefinition a = activeBorrowedSkill;
            if (a == null || runtime.State.SkillsLocked || runtime.Motor.IsDashing) return false;
            if (actionBusy && currentAction != null && currentAction.slot == AbilitySlot.BasicAttack) InterruptOffensiveAction();
            if (actionBusy) return false;
            if (comboInProgress) CancelBasicCombo();
            if (CanDaggerRecast(a)) return TryDaggerRecast(a);
            if (IsHuntRecastReady) return TryHuntRecast();
            if (IsChargedSequenceActive) return TryChargedDashRecast(a, direction, manual);
            if (TryUseV1SecondActivation(a, direction, manual)) return true;
            if (a.behavior == AbilityBehavior.DashReturn && returnArmed && Time.time <= returnExpiresAt)
            {
                Vector3 delta = returnPosition - transform.position;
                if (delta.sqrMagnitude < .01f) return false;
                returnArmed = false; runtime.Motor.FaceDirection(delta);
                actionBusy = true; currentAction = a;
                StartCoroutine(ExecuteReturn(a, delta.magnitude, ++actionSerial)); return true;
            }
            return false;
        }

        CharacterRuntime FindPursuitTargetForSnapshot(AbilityDefinition a, Vector3 direction)
        {
            if (a.behavior == AbilityBehavior.ExecutionStrike || a.behavior == AbilityBehavior.HuntSequence) return FindHuntTarget(a, direction);
            return a.pursueTarget ? FindPursuitTarget(a, direction, true) : FindNearestVisibleTarget(Mathf.Max(a.range, a.movementDistance));
        }

        void BeginWarriorGuardPresentation(AbilityDefinition ability)
        {
            warriorGuardUntil = Time.time + ability.defenseDuration;
            warriorGuardAbility = ability;
            SpawnWarriorPresentation(ability, WarriorSkillPresentation.Kind.Aura, ability.defenseDuration, transform.position, 1.2f);
        }

        void BeginWarriorFortress(AbilityDefinition ability)
        {
            warriorFortressUntil = Time.time + ability.defenseDuration;
            float phase = new[] { 15f, 30f, 45f }[Random.Range(0, 3)];
            SpawnWarriorPresentation(ability, WarriorSkillPresentation.Kind.Shields, ability.defenseDuration, transform.position, 1.25f, phase);
            StartCoroutine(LaunchWarriorShields(ability, phase, warriorGeneration));
        }

        IEnumerator LaunchWarriorShields(AbilityDefinition ability, float phase, int reset)
        {
            yield return new WaitForSeconds(ability.defenseDuration);
            if (reset != warriorGeneration || runtime == null || runtime.Health.IsDead) yield break;
            for (int i = 0; i < 3; i++)
            {
                Vector3 dir = Quaternion.Euler(0f, phase + i * 120f, 0f) * Vector3.forward;
                Hitbox shield = SpawnV1Projectile(ability, dir, 1f, 1f, transform.position + Vector3.up * 0.8f + dir * 1.25f);
                if (shield != null) shield.gameObject.AddComponent<WarriorSkillPresentation>().Begin(runtime, WarriorSkillPresentation.Kind.FlyingShield,
                    ability.range / Mathf.Max(0.1f, ability.projectileSpeed), 0.45f, 0f, false);
            }
        }

        IEnumerator ExecuteWarriorMovement(AbilityDefinition ability, CharacterRuntime target, int id)
        {
            if (ability.behavior == AbilityBehavior.WarriorShieldCharge)
            {
                Hitbox shield = SpawnV1Projectile(ability, runtime.Motor.Facing, 1f, 1f);
                if (shield != null) shield.gameObject.AddComponent<WarriorSkillPresentation>().Begin(runtime,
                    WarriorSkillPresentation.Kind.FlyingShield, ability.range / Mathf.Max(0.1f, ability.projectileSpeed), 0.55f, 0f, false);
                yield break;
            }
            SpawnWarriorPresentation(ability, WarriorSkillPresentation.Kind.Charge, ability.movementDuration + 0.1f, transform.position, 0.8f);
            ExecuteMovement(ability, ability.movementDistance, pursuitTarget: target);
            while (runtime.Motor.IsDashing && ActionStillValid(id)) yield return null;
            if (!ActionStillValid(id)) yield break;
            if (ability.behavior == AbilityBehavior.WarriorPursuitStrike && target != null && !target.Health.IsDead &&
                Vector3.Distance(transform.position, target.transform.position) <= 1.6f)
            {
                yield return new WaitForSeconds(0.12f);
                if (!ActionStillValid(id) || target == null || target.Health.IsDead ||
                    Vector3.Distance(transform.position,target.transform.position)>1.6f) yield break;
                Hitbox followup = CreateAssassinHit(ability, ability.secondStrikeDamage, 0.12f);
                foreach (Hurtbox hurt in target.GetComponentsInChildren<Hurtbox>()) followup.TryResolveHurtbox(hurt);
            }
        }

        void BeginWarriorGroundEffect(AbilityDefinition ability)
        {
            warriorReductionUntil = Time.time + ability.buffDuration;
            warriorReduction = ability.damageReduction;
            // Local ground strike: aiming cannot move its center away from the caster.
            Vector3 center = transform.position;
            var kind = ability.behavior == AbilityBehavior.WarriorGroundField ? WarriorSkillPresentation.Kind.GroundField : WarriorSkillPresentation.Kind.Explosion;
            if (ability.behavior != AbilityBehavior.WarriorGroundWaves)
                SpawnWarriorPresentation(ability, kind, ability.behavior == AbilityBehavior.WarriorGroundField ? ability.fieldDuration : 0.8f,
                    center, ability.fieldRadius, follow: false);
            StartCoroutine(WarriorGroundPulses(ability, center, warriorGeneration));
        }

        IEnumerator WarriorGroundPulses(AbilityDefinition ability, Vector3 center, int reset)
        {
            int count = ability.behavior == AbilityBehavior.WarriorGroundWaves ? 3 :
                ability.behavior == AbilityBehavior.WarriorGroundField ? Mathf.Max(1, Mathf.CeilToInt(ability.fieldDuration / Mathf.Max(0.1f, ability.fieldTickInterval))) : 1;
            for (int i = 0; i < count; i++)
            {
                if (reset != warriorGeneration || runtime == null || runtime.Health.IsDead) yield break;
                float radius = ability.behavior == AbilityBehavior.WarriorGroundWaves ? ability.fieldRadius * (i + 1f) / 3f : ability.fieldRadius;
                CombatResolver.ResolveAreaEffect(center, radius, ability.damage * runtime.Modifiers.damageMultiplier, runtime, ability);
                if (ability.behavior == AbilityBehavior.WarriorGroundWaves)
                    SpawnWarriorPresentation(ability, WarriorSkillPresentation.Kind.Shockwave, 0.4f, center, radius, follow: false);
                if (i + 1 < count) yield return new WaitForSeconds(ability.fieldTickInterval);
            }
        }

        void SpawnWarriorPresentation(AbilityDefinition a, WarriorSkillPresentation.Kind kind, float duration, Vector3 point, float radius,
            float phase = 0f, bool follow = true)
        {
            GameObject visual = new GameObject("Warrior_" + kind); visual.transform.position = point;
            warriorVisuals.Add(visual);
            visual.AddComponent<OwnedAbilityEffect>().Configure(runtime, null);
            // Dedicated presentation replaces the old shared rune prefab; it has no damage authority.
            visual.AddComponent<WarriorSkillPresentation>().Begin(runtime, kind, duration, radius, phase, follow);
        }

        void ClearCapturedSkill()
        { if (capturedSkill != null) Destroy(capturedSkill); capturedSkill = null; capturedUntil = 0f; }
        void ClearWarriorEffects()
        {
            warriorGeneration++;
            foreach (GameObject visual in warriorVisuals) if (visual != null) Destroy(visual);
            warriorVisuals.Clear();
            foreach (Hitbox hit in FindObjectsByType<Hitbox>())
                if (hit != null && hit.Owner == runtime && borrowedSkills.Contains(hit.Packet.ability))
                { hit.Cancel(); Destroy(hit.gameObject); }
            foreach (OwnedAbilityEffect effect in FindObjectsByType<OwnedAbilityEffect>())
                if (effect != null && effect.Owner == runtime && borrowedSkills.Contains(effect.Ability)) Destroy(effect.gameObject);
            activeBorrowedSkill = null;
            ClearCapturedSkill();
            foreach (var skill in borrowedSkills) if (skill != null) Destroy(skill);
            borrowedSkills.Clear();
            warriorGuardUntil = warriorFortressUntil = warriorReductionUntil = warriorReduction = 0f;
            warriorGuardAbility = null;
        }
        void OnDestroy() { ClearCapturedSkill(); foreach (AbilityDefinition skill in borrowedSkills) if (skill != null) Destroy(skill); }
    }
}
