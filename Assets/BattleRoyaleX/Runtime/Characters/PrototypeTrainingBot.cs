using UnityEngine;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterRuntime))]
    public sealed class PrototypeTrainingBot : MonoBehaviour
    {
        public enum Intent { Approach, Orbit, Evade, Defend, Counter, Attack, Recover, Search }
        public enum TrainingMode { Normal, Stationary, StationaryAttack }
        public bool runInEditor;
        [Min(0.04f)] public float decisionInterval = 0.10f;
        [Min(0.20f)] public float reactionDelay = 0.22f;
        [Min(0.1f)] public float preferredDistance = 1.25f;
        [Min(0.1f)] public float retreatDistance = 0.70f;
        [Min(0.25f)] public float roundResetDelay = 2.5f;

        public Intent CurrentIntent { get; private set; }
        public int ObservedSkills { get; private set; }
        public int DefensiveResponses { get; private set; }
        public float LastObservedAt { get; private set; } = -999f;
        public float LastReactionAt { get; private set; } = -999f;
        [SerializeField] TrainingMode trainingMode;
        public TrainingMode Mode => trainingMode;

        CharacterRuntime runtime, target;
        float nextDecisionAt, nextActionAt, strafeSign = 1f, nextOrbitSwitch;
        Vector3 startPosition, targetStartPosition, lastSeenPosition;
        Quaternion startRotation, targetStartRotation;
        bool capturedTarget, resetPending;
        float resetAt, lastSeenAt = -999f;
        AbilityDefinition observed;
        Vector3 threatOrigin, threatDirection;
        float reactAt, threatUntil, enemyGuardUntil, punishUntil;
        bool responseMade;
        int attacksStarted;
        readonly RaycastHit[] probes = new RaycastHit[24];

        void Awake()
        {
            runtime = GetComponent<CharacterRuntime>();
            if (!Application.isMobilePlatform && !runInEditor) enabled = false;
        }
        void Start() { startPosition = transform.position; startRotation = transform.rotation; }
        void OnEnable() => CombatEvents.Raised += Observe;
        public void ResetAwareness()
        {
            ClearObservation(); target = null; capturedTarget = resetPending = false;
            nextDecisionAt = nextActionAt = Time.time + 0.3f;
        }
        public void SetMode(TrainingMode mode)
        {
            trainingMode = mode;
            if (runtime != null)
            {
                runtime.ResetTransientState();
                runtime.Motor.StopMovementImmediately();
            }
            ResetAwareness();
        }
        void OnDisable()
        {
            CombatEvents.Raised -= Observe;
            if (runtime != null && runtime.Motor != null) runtime.Motor.StopMovementImmediately();
            ClearObservation();
        }

        void Observe(CombatEventData e)
        {
            if (runtime == null || e.source == null || e.source == runtime || e.source.TeamId == runtime.TeamId || e.ability == null) return;
            if (e.kind != CombatEventKind.AbilityAttack && e.kind != CombatEventKind.AbilityMove &&
                e.kind != CombatEventKind.AbilityGuard && e.kind != CombatEventKind.AbilityUltimate) return;
            if (e.phase != AbilityPhase.Startup || !CanSee(e.source.transform.position)) return;
            target = e.source;
            // Only public cast events. No reading future input, enemy energy, cooldowns or attack queues.
            observed = e.ability;
            threatOrigin = e.position;
            threatDirection = e.direction.sqrMagnitude > 0.01f ? e.direction.normalized : e.source.Motor.Facing;
            LastObservedAt = Time.time;
            reactAt = Time.time + Mathf.Max(0.20f, reactionDelay);
            responseMade = false;
            ObservedSkills++;
            float travel = IsTravel(observed) ? observed.movementDuration : observed.activeTime;
            threatUntil = Time.time + observed.startup + travel + observed.recovery + 0.30f;
            if (observed.behavior == AbilityBehavior.ChargedDashSequence)
                threatUntil = Time.time + observed.chargeWindow;
            if (observed.slot == AbilitySlot.Defense)
                enemyGuardUntil = Time.time + observed.startup + Mathf.Max(travel, observed.defenseDuration);
            punishUntil = threatUntil + 0.40f;
        }

        static bool IsTravel(AbilityDefinition ability) => ability.behavior == AbilityBehavior.Dash ||
            ability.behavior == AbilityBehavior.DashThrough || ability.behavior == AbilityBehavior.DashReturn ||
            ability.behavior == AbilityBehavior.ChargedDashSequence || ability.behavior == AbilityBehavior.HuntSequence || ability.behavior == AbilityBehavior.Dodge;

        void Update()
        {
            if (runtime == null || runtime.Health == null || runtime.Abilities == null) return;
            if (target == null) target = FindOpponent();
            if (target == null) return;
            if (!capturedTarget)
            {
                targetStartPosition = target.transform.position;
                targetStartRotation = target.transform.rotation;
                capturedTarget = true;
            }
            if (runtime.Health.IsDead || target.Health.IsDead) { HandleRoundReset(); return; }
            if (Time.time < nextDecisionAt) return;
            nextDecisionAt = Time.time + Mathf.Max(0.04f, decisionInterval);

            if (trainingMode != TrainingMode.Normal)
            {
                UpdateStationaryMode();
                return;
            }

            if (!CanSee(target.transform.position))
            {
                CurrentIntent = Intent.Search;
                // Search the last seen location briefly, without tracking a moving target inside smoke.
                Vector3 search = lastSeenPosition - transform.position; search.y = 0f;
                Vector3 patrol = Quaternion.Euler(0f, Time.time * 24f, 0f) * Vector3.forward * 0.42f;
                Move(Time.time - lastSeenAt < 2f && search.magnitude > 1.5f ? search.normalized * 0.45f : patrol);
                observed = null; responseMade = false; enemyGuardUntil = 0f;
                return;
            }
            lastSeenPosition = target.transform.position;
            lastSeenAt = Time.time;
            Vector3 delta = lastSeenPosition - transform.position; delta.y = 0f;
            float distance = delta.magnitude;
            Vector3 toward = distance > 0.01f ? delta / distance : runtime.Motor.Facing;
            Vector3 side = Vector3.Cross(Vector3.up, toward) * strafeSign;
            if (Time.time >= nextOrbitSwitch) { strafeSign = -strafeSign; nextOrbitSwitch = Time.time + 1.65f; }

            if (runtime.Motor.IsDashing || runtime.Abilities.IsActionBusy || runtime.State.InputLocked) return;
            runtime.Motor.FaceDirection(toward);

            if (observed != null && Time.time >= reactAt && Time.time < threatUntil && !responseMade)
            {
                bool aimedAtMe = ThreatIntersectsSelf();
                if (aimedAtMe && observed.slot != AbilitySlot.Defense)
                {
                    LastReactionAt = Time.time;
                    responseMade = true;
                    DefensiveResponses++;
                    if (runtime.Abilities.TryUse(AbilitySlot.Defense, IsAssassin() ? side : toward))
                    {
                        CurrentIntent = Intent.Defend;
                        Move(side * 0.42f);
                        nextActionAt = Time.time + 0.22f;
                        return;
                    }
                    CurrentIntent = Intent.Evade;
                    Move(side * 0.85f - toward * 0.3f);
                    nextActionAt = Time.time + 0.32f;
                    return;
                }
            }

            if (Time.time < enemyGuardUntil && Time.time >= reactAt)
            {
                CurrentIntent = Intent.Orbit;
                Move(side * 0.65f + toward * (distance > 2.3f ? 0.5f : -0.32f));
                return;
            }

            bool counter = runtime.Abilities.HasCounterOpportunity;
            if (counter) CurrentIntent = Intent.Counter;
            else if (distance > preferredDistance + 0.35f) CurrentIntent = Intent.Approach;
            else if (runtime.Abilities.GetCooldownRemaining(AbilitySlot.BasicAttack) > 0.2f && !runtime.Abilities.IsComboInProgress)
                CurrentIntent = Intent.Recover;
            else CurrentIntent = Intent.Orbit;

            Vector3 movement = distance > preferredDistance + 0.35f
                ? toward * 0.94f + side * (counter ? 0f : 0.28f)
                : side * 0.55f + toward * (distance < retreatDistance ? -0.6f : 0.06f);
            if (CurrentIntent == Intent.Recover && distance < 1.9f) movement = side * 0.65f - toward * 0.38f;
            Move(movement);

            if (Time.time < nextActionAt) return;
            if (runtime.Abilities.IsHuntRecastReady && !runtime.Abilities.IsComboInProgress &&
                runtime.Abilities.TryUse(AbilitySlot.Ultimate, toward))
            { nextActionAt = Time.time + 0.42f; return; }
            AbilityDefinition basic = runtime.Abilities.GetEquipped(AbilitySlot.BasicAttack);
            if (basic == null) return;
            float reach = basic.range;
            if (distance <= reach - 0.1f)
            {
                if (runtime.Abilities.TryUse(AbilitySlot.BasicAttack, toward))
                {
                    CurrentIntent = counter ? Intent.Counter : Intent.Attack;
                    attacksStarted++;
                    nextActionAt = Time.time + 0.12f;
                    // Retain some lateral footwork, but do not slide out of our own attack reach.
                    Move(side * 0.20f);
                    return;
                }
            }
            // Chase after a visible escape; never dash into a wall or while our own combo is committed.
            if (distance > 3.5f && distance < 9f && !runtime.Abilities.IsComboInProgress &&
                PathClear(toward, Mathf.Min(distance, runtime.Abilities.GetEquipped(AbilitySlot.Movement).movementDistance)) &&
                runtime.Abilities.TryUse(AbilitySlot.Movement, toward))
            { nextActionAt = Time.time + 0.45f; return; }

            // An incoming startup is NOT an opening. Do not commit to our own ultimate and
            // become action-busy before the observation's human reaction delay has elapsed.
            bool pendingThreat = observed != null && observed.slot != AbilitySlot.Defense && Time.time < threatUntil;
            bool opening = !pendingThreat && (Time.time < punishUntil || attacksStarted >= 3 ||
                runtime.Health.CurrentHealth < runtime.Health.MaxHealth * 0.65f);
            if (opening && distance < 2.6f && !runtime.Abilities.IsComboInProgress &&
                runtime.Abilities.TryUse(AbilitySlot.Ultimate, toward))
            { nextActionAt = Time.time + 0.42f; attacksStarted = 0; }
        }

        bool IsAssassin() => runtime.Definition != null && runtime.Definition.characterClass == CharacterClass.Assassin;

        void UpdateStationaryMode()
        {
            runtime.Motor.StopMovementImmediately();
            CurrentIntent = trainingMode == TrainingMode.StationaryAttack ? Intent.Attack : Intent.Recover;
            if (target == null || !CanSee(target.transform.position)) return;
            Vector3 delta = target.transform.position - transform.position;
            delta.y = 0f;
            float distance = delta.magnitude;
            Vector3 toward = distance > 0.01f ? delta / distance : runtime.Motor.Facing;
            if (!runtime.Motor.IsDashing && !runtime.Abilities.IsActionBusy) runtime.Motor.FaceDirection(toward);
            if (trainingMode != TrainingMode.StationaryAttack || Time.time < nextActionAt || runtime.Motor.IsDashing) return;
            AbilityDefinition basic = runtime.Abilities.GetEquipped(AbilitySlot.BasicAttack);
            if (basic == null || distance > basic.range - 0.05f) return;
            if (runtime.Abilities.TryUse(AbilitySlot.BasicAttack, toward)) nextActionAt = Time.time + 0.08f;
        }

        bool ThreatIntersectsSelf()
        {
            Vector3 delta = transform.position - threatOrigin; delta.y = 0f;
            if (observed.behavior == AbilityBehavior.UltimateBuff || observed.behavior == AbilityBehavior.AreaAttack)
                return delta.magnitude <= observed.explosionRadius + 0.9f;
            float range = IsTravel(observed) ? observed.movementDistance : observed.range;
            float along = Vector3.Dot(delta, threatDirection);
            float lateral = (delta - threatDirection * along).magnitude;
            return along >= -0.7f && along <= range + 0.9f && lateral <= observed.width * 0.5f + 0.9f;
        }

        bool CanSee(Vector3 position)
        {
            if (SmokeField.BlocksSight(transform.position, position)) return false;
            Vector3 delta = position - transform.position; delta.y = 0f;
            return delta.magnitude < 28f && PathClear(delta.normalized, delta.magnitude);
        }

        bool PathClear(Vector3 direction, float length)
        {
            int count = Physics.SphereCastNonAlloc(transform.position + Vector3.up * 0.2f, 0.30f,
                direction, probes, Mathf.Max(0.01f, length), ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
                if (probes[i].collider != null && probes[i].collider.GetComponentInParent<CharacterRuntime>() == null)
                    return false;
            return count < probes.Length;
        }

        void Move(Vector3 movement)
        {
            movement = Vector3.ClampMagnitude(movement, 1f);
            if (movement.sqrMagnitude > 0.01f && !PathClear(movement.normalized, 1.1f))
            {
                // Stable side choice avoids oscillating into rectangular arena obstacles.
                Vector3 side = Quaternion.Euler(0f, 70f * strafeSign, 0f) * movement;
                if (!PathClear(side.normalized, 1.2f)) side = Quaternion.Euler(0f, -100f * strafeSign, 0f) * movement;
                movement = PathClear(side.normalized, 1.1f) ? side : Vector3.zero;
            }
            runtime.Motor.SetMoveInput(new Vector2(movement.x, movement.z));
        }

        void ClearObservation()
        {
            observed = null;
            responseMade = false;
            enemyGuardUntil = punishUntil = threatUntil = 0f;
            lastSeenAt = -999f;
        }

        void HandleRoundReset()
        {
            runtime.Motor.StopMovementImmediately();
            if (!resetPending) { resetPending = true; resetAt = Time.time + roundResetDelay; return; }
            if (Time.time < resetAt) return;
            ResetCharacter(runtime, startPosition, startRotation);
            ResetCharacter(target, targetStartPosition, targetStartRotation);
            ClearObservation();
            nextDecisionAt = nextActionAt = Time.time + 0.45f;
            resetPending = false;
        }

        static void ResetCharacter(CharacterRuntime character, Vector3 position, Quaternion rotation)
        {
            character.ResetTransientState();
            character.Motor.Teleport(position);
            character.Motor.FaceDirection(rotation * Vector3.forward);
            character.Health.RestoreFull();
            character.Energy.Restore(character.Energy.MaxEnergy);
        }

        CharacterRuntime FindOpponent()
        {
            foreach (CharacterRuntime c in FindObjectsByType<CharacterRuntime>())
                if (c != runtime && c.TeamId != runtime.TeamId && c.TeamId != TeamId.Neutral) return c;
            return null;
        }
    }
}
