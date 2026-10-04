using System.Collections;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed partial class AbilityController
    {
        float executionHiddenUntil, orbitUntil, nextOrbitPulse;
        AbilityDefinition orbitAbility;
        AssassinDaggerAnchor daggerAnchor;
        GameObject orbitVisual;
        public bool IsExecutionHidden => runtime != null && !runtime.Health.IsDead && Time.time < executionHiddenUntil;
        public bool HasOrbitingDaggers => orbitAbility != null && Time.time < orbitUntil && !runtime.Health.IsDead;
        public float OrbitTimeRemaining => HasOrbitingDaggers ? orbitUntil - Time.time : 0f;

        IEnumerator ExecuteExecution(AbilityDefinition ability, CharacterRuntime target, int id)
        {
            if (target == null || target.Health.IsDead) yield break;
            bool contacted = false;
            Hitbox strike = CreateAssassinHit(ability, ability.damage, ability.huntMaxDuration + 0.1f);
            runtime.Motor.Dash(ability.huntSpeed * ability.huntMaxDuration, ability.huntMaxDuration, true, 0f,
                (from, to) =>
                {
                    if (contacted || !ActionStillValid(id) || target == null || strike == null) return;
                    foreach (Collider c in Physics.OverlapCapsule(from + Vector3.up, to + Vector3.up,
                        Mathf.Max(0.4f, ability.width * 0.5f), ~0, QueryTriggerInteraction.Collide))
                    {
                        Hurtbox hurt = c.GetComponent<Hurtbox>();
                        if (hurt == null || hurt.Owner != target) continue;
                        contacted = true;
                        strike.TryResolveHurtbox(hurt);
                        executionHiddenUntil = Time.time + ability.stealthDuration;
                        var visual = GetComponentInChildren<CharacterVisualAnimator>();
                        if (visual != null) visual.PlayBasicStep(1);
                        break;
                    }
                }, () => !ActionStillValid(id) || contacted || target == null || target.Health.IsDead ||
                    target.Abilities.IsExecutionHidden || SmokeField.BlocksSight(transform.position, target.transform.position)
                    ? transform.position : target.transform.position);
            while (runtime.Motor.IsDashing && ActionStillValid(id)) yield return null;
            if (!ActionStillValid(id)) runtime.Motor.CancelDash();
            if (strike != null) { strike.Cancel(); Destroy(strike.gameObject); }
        }

        Hitbox CreateAssassinHit(AbilityDefinition ability, float damage, float lifetime)
        {
            GameObject go = new GameObject("AssassinHit_" + ability.abilityId);
            var hit = go.AddComponent<Hitbox>();
            DamagePacket packet = new DamagePacket(runtime, ability, runtime.Motor.Facing)
            { damage = damage * runtime.Modifiers.damageMultiplier, clashable = false };
            hit.Configure(runtime, packet, Vector3.one * 0.1f, lifetime);
            go.GetComponent<Collider>().enabled = false;
            return hit;
        }

        void ThrowTeleportDagger(AbilityDefinition ability)
        {
            if (daggerAnchor != null) daggerAnchor.Cancel();
            GameObject go = new GameObject("TeleportDagger");
            go.transform.position = transform.position + Vector3.up * 0.8f;
            go.AddComponent<OwnedAbilityEffect>().Configure(runtime, ability);
            daggerAnchor = go.AddComponent<AssassinDaggerAnchor>();
            daggerAnchor.Configure(runtime, ability, runtime.Motor.Facing);
        }

        bool CanDaggerRecast(AbilityDefinition ability) => daggerAnchor != null && daggerAnchor.Ability == ability && daggerAnchor.CanRecast;

        bool TryDaggerRecast(AbilityDefinition ability)
        {
            if (!CanDaggerRecast(ability) || actionBusy || comboInProgress || runtime.State.SkillsLocked || runtime.Motor.IsDashing) return false;
            Vector3 destination = daggerAnchor.TeleportPosition;
            // Do not teleport into a solid wall. Character overlap is avoided by the dagger's contact offset.
            var controller = GetComponent<CharacterController>();
            Vector3 center = destination + controller.center;
            float extent = Mathf.Max(0f, controller.height * 0.5f - controller.radius);
            foreach (Collider c in Physics.OverlapCapsule(center + Vector3.up * extent,
                center - Vector3.up * extent, controller.radius * 0.9f, ~0, QueryTriggerInteraction.Ignore))
                if (c.GetComponentInParent<CharacterRuntime>() == null) return false;
            Vector3 direction = daggerAnchor.Direction;
            daggerAnchor.Cancel(); daggerAnchor = null;
            actionBusy = true; currentAction = ability;
            int id = ++actionSerial;
            runtime.Defense.Deactivate();
            runtime.Motor.Teleport(destination); runtime.Motor.FaceDirection(direction);
            Hitbox hit = CreateAssassinHit(ability, ability.secondStrikeDamage, 0.1f);
            foreach (Collider c in Physics.OverlapSphere(transform.position + Vector3.up, ability.explosionRadius,
                ~0, QueryTriggerInteraction.Collide)) hit.TryResolveHurtbox(c.GetComponent<Hurtbox>());
            CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityMove, destination, runtime, runtime,
                2f, ability, AbilityPhase.Active, id, direction));
            var visual = GetComponentInChildren<CharacterVisualAnimator>();
            if (visual != null) visual.PlayBasicStep(1);
            StartCoroutine(FinishDaggerRecast(ability, id));
            return true;
        }

        IEnumerator FinishDaggerRecast(AbilityDefinition ability, int id)
        {
            runtime.State.SetActionRecovery(true);
            yield return new WaitForSeconds(Mathf.Max(0.02f, ability.recovery));
            runtime.State.SetActionRecovery(false); FinishAction(id);
        }

        void BeginOrbitingDaggers(AbilityDefinition ability)
        {
            CancelAssassinSlot(AbilitySlot.Defense);
            orbitAbility = ability; orbitUntil = Time.time + ability.fieldDuration; nextOrbitPulse = Time.time;
            orbitVisual = new GameObject("FiveOrbitingDaggers");
            orbitVisual.transform.SetParent(transform, false);
            orbitVisual.AddComponent<OwnedAbilityEffect>().Configure(runtime, ability);
            var visual = orbitVisual.AddComponent<AssassinDaggersPresentation>();
            visual.Configure(5, ability.fieldRadius);
        }

        public bool TryRepelWithDaggers(DamagePacket attack)
        {
            if (!HasOrbitingDaggers || attack.damage <= 0f || attack.source == runtime ||
                attack.source != null && attack.source.TeamId == runtime.TeamId) return false;
            if (orbitAbility.defenseInteractionRule != null && !orbitAbility.defenseInteractionRule.Allows(attack.ability)) return false;
            return Random.value < orbitAbility.repelChance;
        }

        void UpdateAssassinEffects()
        {
            if (runtime == null || runtime.Health == null) return;
            if (runtime.Health.IsDead) { ClearAssassinEffects(); return; }
            if (orbitAbility == null) return;
            if (!HasOrbitingDaggers) { CancelAssassinSlot(AbilitySlot.Defense); return; }
            if (Time.time < nextOrbitPulse || SmokeField.PreventsAttack(runtime)) return;
            nextOrbitPulse = Time.time + 0.5f;
            Hitbox hit = CreateAssassinHit(orbitAbility, orbitAbility.damage, 0.05f);
            foreach (Collider c in Physics.OverlapSphere(transform.position + Vector3.up, orbitAbility.fieldRadius,
                ~0, QueryTriggerInteraction.Collide)) hit.TryResolveHurtbox(c.GetComponent<Hurtbox>());
        }

        void CancelAssassinSlot(AbilitySlot slot)
        {
            if (slot == AbilitySlot.Defense)
            {
                orbitAbility = null; orbitUntil = 0f;
                if (orbitVisual != null) Destroy(orbitVisual); orbitVisual = null;
            }
            if (slot == AbilitySlot.Movement)
            { if (daggerAnchor != null) daggerAnchor.Cancel(); daggerAnchor = null; }
            if (slot == AbilitySlot.Ultimate) executionHiddenUntil = 0f;
        }

        void ClearAssassinEffects()
        {
            CancelAssassinSlot(AbilitySlot.Defense);
            CancelAssassinSlot(AbilitySlot.Movement);
            CancelAssassinSlot(AbilitySlot.Ultimate);
        }
    }
}
