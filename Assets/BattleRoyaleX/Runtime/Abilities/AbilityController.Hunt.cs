using System.Collections;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed partial class AbilityController
    {
        AbilityDefinition huntAbility;
        CharacterRuntime huntTarget;
        float huntExpiresAt;
        bool huntSecondReady;
        public bool IsHuntRecastReady => huntSecondReady && huntAbility != null && huntTarget != null &&
            !huntTarget.Health.IsDead && Time.time <= huntExpiresAt;
        public float HuntTimeRemaining => IsHuntRecastReady ? Mathf.Max(0f, huntExpiresAt - Time.time) : 0f;
        public float ReactiveDefenseRemaining => defenseJourney != null && !defenseRedirected ? Mathf.Max(0f, defenseJourneyUntil - Time.time) : 0f;

        CharacterRuntime FindHuntTarget(AbilityDefinition ability, Vector3 aim)
        {
            CharacterRuntime selected = null;
            float score = float.MaxValue;
            foreach (var candidate in FindObjectsByType<CharacterRuntime>())
            {
                if (candidate == runtime || candidate.TeamId == runtime.TeamId || candidate.Health.IsDead ||
                    candidate.Abilities.IsExecutionHidden || SmokeField.BlocksSight(transform.position, candidate.transform.position)) continue;
                Vector3 delta = candidate.transform.position - transform.position; delta.y = 0f;
                if (delta.magnitude > ability.huntAcquireRange || !HuntPathClear(delta)) continue;
                float candidateScore = delta.magnitude + (aim.sqrMagnitude > 0.01f ? (1f - Vector3.Dot(aim.normalized, delta.normalized)) * 4f : 0f);
                if (candidateScore < score) { selected = candidate; score = candidateScore; }
            }
            return selected;
        }

        bool HuntPathClear(Vector3 delta)
        {
            foreach (RaycastHit hit in Physics.SphereCastAll(transform.position + Vector3.up * 0.2f, 0.3f,
                delta.normalized, delta.magnitude, ~0, QueryTriggerInteraction.Ignore))
                if (hit.collider.GetComponentInParent<CharacterRuntime>() == null) return false;
            return true;
        }

        bool TryHuntRecast()
        {
            if (!IsHuntRecastReady || runtime.State.SkillsLocked || actionBusy || comboInProgress || runtime.Motor.IsDashing ||
                huntTarget.Abilities.IsExecutionHidden || SmokeField.BlocksSight(transform.position, huntTarget.transform.position)) return false;
            return StartHuntStage(true);
        }

        bool StartHuntStage(bool second)
        {
            huntSecondReady = false;
            actionBusy = true;
            currentAction = huntAbility;
            int id = ++actionSerial;
            runtime.Defense.Deactivate();
            runtime.Motor.FaceDirection(huntTarget.transform.position - transform.position);
            var visual = GetComponentInChildren<CharacterVisualAnimator>();
            if (visual != null) visual.PlayChargedDash();
            CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityMove, transform.position, runtime, huntTarget,
                second ? 2 : 1, huntAbility, AbilityPhase.Startup, id, runtime.Motor.Facing));
            StartCoroutine(ExecuteHunt(huntAbility, huntTarget, second, id));
            return true;
        }

        IEnumerator ExecuteHunt(AbilityDefinition ability, CharacterRuntime target, bool second, int id)
        {
            yield return new WaitForSeconds(ability.startup);
            if (!ActionStillValid(id) || target == null || target.Health.IsDead) { FinishAction(id); ClearHunt(); yield break; }
            Vector3 origin = transform.position;
            Vector3 finish = origin;
            bool contacted = false;
            GameObject go = new GameObject("HuntHit_" + (second ? 2 : 1));
            Hitbox sweep = go.AddComponent<Hitbox>();
            DamagePacket packet = new DamagePacket(runtime, ability, runtime.Motor.Facing)
            {
                damage = (second ? ability.damage : ability.huntFirstDamage) * runtime.Modifiers.damageMultiplier,
                knockback = second ? 0f : ability.huntFirstPush,
                clashable = false
            };
            sweep.Configure(runtime, packet, Vector3.one, ability.huntMaxDuration + 0.5f);
            go.GetComponent<Collider>().enabled = false;
            CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityMove, origin, runtime, target,
                second ? 2 : 1, ability, AbilityPhase.Active, id, runtime.Motor.Facing));
            runtime.Motor.Dash(ability.huntSpeed * ability.huntMaxDuration, ability.huntMaxDuration, true, 0f,
                (from, to) =>
                {
                    if (target == null || contacted || sweep == null) return;
                    foreach (var collider in Physics.OverlapCapsule(from + Vector3.up, to + Vector3.up,
                        Mathf.Max(0.4f, ability.width * 0.5f), ~0, QueryTriggerInteraction.Collide))
                    {
                        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
                        if (hurtbox == null || hurtbox.Owner != target) continue;
                        contacted = true;
                        Vector3 through = target.transform.position - origin; through.y = 0f;
                        if (through.sqrMagnitude < 0.01f) through = runtime.Motor.Facing;
                        finish = second ? target.transform.position + through.normalized * ability.huntOvershoot : to;
                        // Direction is sampled at contact, after tracking a possible enemy dash.
                        packet.direction = through.normalized;
                        sweep.UpdatePacket(packet);
                        sweep.TryResolveHurtbox(hurtbox);
                        break;
                    }
                },
                () =>
                {
                    if (!ActionStillValid(id) || target == null || target.Health.IsDead) return transform.position;
                    if (contacted) return finish;
                    // A dash never breaks the lock; smoke does. Budget and world collisions bound pursuit.
                    if (target.Abilities.IsExecutionHidden || SmokeField.BlocksSight(transform.position, target.transform.position)) return transform.position;
                    return target.transform.position;
                });
            while (runtime.Motor.IsDashing && ActionStillValid(id)) yield return null;
            if (!ActionStillValid(id)) runtime.Motor.CancelDash();
            if (sweep != null) { sweep.Cancel(); Destroy(go); }
            if (ability.recovery > 0f)
            {
                runtime.State.SetActionRecovery(true);
                yield return new WaitForSeconds(ability.recovery);
                runtime.State.SetActionRecovery(false);
            }
            bool valid = ActionStillValid(id);
            FinishAction(id);
            if (valid && contacted && !second && huntAbility == ability)
            { huntSecondReady = true; huntExpiresAt = Time.time + ability.chargeWindow; }
            else ClearHunt();
        }

        void ClearHunt()
        {
            huntAbility = null; huntTarget = null; huntSecondReady = false; huntExpiresAt = 0f;
        }
    }
}
