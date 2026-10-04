using System.Collections;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed partial class AbilityController
    {
        Vector3 GroundCastPosition(AbilityDefinition ability) => currentAction == ability ? actionAimPoint :
            transform.position + runtime.Motor.Facing * Mathf.Max(0f, ability.range);

        public bool CanRecast(AbilitySlot slot)
        {
            AbilityDefinition ability = GetEquipped(slot);
            if (ability == null) return false;
            if (ability.behavior == AbilityBehavior.WarriorSkillCapture && CanBorrowedRecast()) return true;
            if (ability.behavior == AbilityBehavior.WarriorSkillCapture && HasCapturedSkill) return true;
            if (CanDaggerRecast(ability)) return true;
            if (IsChargedSequenceActive && chargedSequenceAbility == ability) return true;
            if (ability.behavior == AbilityBehavior.HuntSequence && IsHuntRecastReady) return true;
            if (ability.behavior == AbilityBehavior.DashReturn && returnArmed && Time.time <= returnExpiresAt) return true;
            CloneTeleportController clones = GetComponent<CloneTeleportController>();
            if (clones != null && clones.Ability == ability && clones.CanTeleport) return true;
            MageConvergenceController convergence = GetComponent<MageConvergenceController>();
            return convergence != null && convergence.Ability == ability && convergence.CanFireSecond;
        }

        public Vector3 PreviewAimPoint(AbilitySlot slot, Vector3 direction, bool manual, float fraction)
        {
            AbilityDefinition ability = GetAimDefinition(slot);
            if (ability == null) return transform.position;
            if (ability.behavior == AbilityBehavior.WarriorSkillCapture && HasCapturedSkill) ability = capturedSkill;
            if (ability.behavior == AbilityBehavior.WarriorSkillCapture && CanBorrowedRecast()) ability = activeBorrowedSkill;
            if (CanDaggerRecast(ability)) return daggerAnchor.TeleportPosition;
            if (ability.behavior == AbilityBehavior.DashReturn && returnArmed && Time.time <= returnExpiresAt) return returnPosition;
            CloneTeleportController clones = GetComponent<CloneTeleportController>();
            if (clones != null && clones.Ability == ability && clones.CanTeleport)
                return AbilityAimSolution.ClonePoint(runtime, ability, direction);
            if (AbilityAimSolution.IsSelfCentered(ability)) return transform.position;
            if (AbilityAimSolution.IsTracking(ability))
            {
                CharacterRuntime tracked = ability.behavior == AbilityBehavior.HuntSequence && IsHuntRecastReady ? huntTarget :
                    ability.behavior == AbilityBehavior.HuntSequence || ability.behavior == AbilityBehavior.ExecutionStrike ?
                    FindHuntTarget(ability, direction) : FindPursuitTarget(ability, direction, manual);
                if (tracked != null)
                {
                    Vector3 offset = tracked.transform.position - transform.position; offset.y = 0f;
                    if (ability.behavior == AbilityBehavior.HuntSequence && IsHuntRecastReady)
                        return tracked.transform.position + offset.normalized * ability.huntOvershoot;
                    return tracked.transform.position - offset.normalized *
                        (ability.pursueTarget ? ability.pursuitStopDistance : 0f);
                }
            }
            if (!manual && ability.abilityId != "Archer_S1_B")
            {
                CharacterRuntime target = FindNearestVisibleTarget(ability.range);
                if (target != null)
                {
                    if (AbilityAimSolution.IsGroundTarget(ability)) return AbilityAimSolution.ClampGroundPoint(transform.position, target.transform.position, ability);
                    direction = target.transform.position - transform.position;
                }
            }
            if (ability.behavior == AbilityBehavior.Grapple)
            {
                direction.y = 0f;
                if (direction.sqrMagnitude < 0.001f) direction = runtime.Motor.Facing;
                float distance = AbilityAimSolution.GrappleDistance(runtime, ability, direction, out CharacterRuntime grappleTarget);
                return transform.position + direction.normalized * (grappleTarget != null ? Mathf.Max(0f, distance - 1.1f) : distance);
            }
            return AbilityAimSolution.ResolvePoint(transform.position, direction, ability, fraction);
        }
        bool TryUseV1SecondActivation(AbilityDefinition ability, Vector3 worldDirection, bool hasDirection)
        {
            CloneTeleportController clones = GetComponent<CloneTeleportController>();
            if (clones != null && clones.Ability == ability && clones.CanTeleport)
                return clones.TryTeleport(hasDirection ? worldDirection : runtime.Motor.Facing);

            MageConvergenceController convergence = GetComponent<MageConvergenceController>();
            if (convergence != null && convergence.Ability == ability && convergence.CanFireSecond)
                return convergence.TryFireSecond(hasDirection ? worldDirection : runtime.Motor.Facing);
            return false;
        }

        void SpawnSeekingProjectile(AbilityDefinition ability)
        {
            int count = Mathf.Clamp(ability.projectileCount, 1, 8);
            for (int i = 0; i < count; i++)
            {
                float angle = (i - (count - 1) * 0.5f) * Mathf.Max(5f, ability.spreadAngle);
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * runtime.Motor.Facing;
                Hitbox hitbox = SpawnV1Projectile(ability, direction, 1f, 1f);
                if (hitbox == null) continue;
                SeekingProjectileMover mover = hitbox.gameObject.AddComponent<SeekingProjectileMover>();
                mover.Configure(runtime, ability.projectileSpeed, ability.turnRate,
                    Mathf.Max(0.15f, ability.range / Mathf.Max(0.1f, ability.projectileSpeed)), 180f, ability.range);
            }
        }

        void SpawnMultiShot(AbilityDefinition ability)
        {
            int count = Mathf.Clamp(ability.projectileCount, 1, 12);
            float spread = Mathf.Max(0f, ability.spreadAngle);
            for (int i = 0; i < count; i++)
            {
                float t = count == 1 ? 0f : i / (float)(count - 1) - 0.5f;
                Vector3 direction = Quaternion.Euler(0f, t * spread, 0f) * runtime.Motor.Facing;
                SpawnV1Projectile(ability, direction, 1f, 1f);
            }
        }

        Hitbox SpawnV1Projectile(AbilityDefinition ability, Vector3 direction, float damageScale, float speedScale,
            Vector3? origin = null)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) direction = runtime.Motor.Facing;
            direction.Normalize();
            GameObject go = new GameObject($"Projectile_{ability.abilityId}");
            go.transform.position = origin ?? transform.position + direction * 1.1f + Vector3.up * 0.8f;
            go.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            Hitbox hitbox = go.AddComponent<Hitbox>();
            DamagePacket packet = new DamagePacket(runtime, ability, direction);
            packet.damage *= Mathf.Max(0f, damageScale);
            float lifetime = ability.range > 0f ? Mathf.Max(0.15f, ability.range /
                Mathf.Max(0.1f, ability.projectileSpeed * speedScale)) : 4f;
            hitbox.Configure(runtime, packet, new Vector3(ability.width, ability.height, Mathf.Max(0.5f, ability.width)), lifetime);
            ProjectileHitboxMover mover = go.AddComponent<ProjectileHitboxMover>();
            mover.direction = direction;
            mover.speed = ability.projectileSpeed * Mathf.Max(0.01f, speedScale);
            mover.maxLifetime = lifetime;
            go.AddComponent<ProjectileInteractionState>();
            OwnedAbilityEffect marker = go.AddComponent<OwnedAbilityEffect>();
            marker.Configure(runtime, ability);
            return hitbox;
        }

        internal Hitbox SpawnCloneProjectile(AbilityDefinition ability, Vector3 origin, Vector3 direction)
        {
            return SpawnV1Projectile(ability, direction, 1f, 1f, origin + Vector3.up * 0.8f);
        }

        void SpawnSlowField(AbilityDefinition ability)
        {
            GameObject go = new GameObject($"SlowField_{ability.abilityId}");
            go.transform.position = GroundCastPosition(ability);
            go.AddComponent<OwnedAbilityEffect>().Configure(runtime, ability);
            go.AddComponent<SlowField>().Configure(runtime, ability);
        }

        void SpawnPullField(AbilityDefinition ability)
        {
            GameObject go = new GameObject($"PullField_{ability.abilityId}");
            go.transform.position = GroundCastPosition(ability);
            go.AddComponent<OwnedAbilityEffect>().Configure(runtime, ability);
            if (ability.abilityId == "Archer_S1_C")
                go.AddComponent<ArcherProximityTrap>().Configure(runtime, ability);
            else go.AddComponent<PullField>().Configure(runtime, ability);
        }

        void SpawnArrowRain(AbilityDefinition ability)
        {
            GameObject go = new GameObject($"ArrowRain_{ability.abilityId}");
            go.transform.position = GroundCastPosition(ability);
            go.AddComponent<OwnedAbilityEffect>().Configure(runtime, ability);
            go.AddComponent<ArcherArrowRain>().Configure(runtime, ability);
        }

        CharacterRuntime FindNearestVisibleTarget(float range)
        {
            CharacterRuntime best = null;
            float bestDistance = Mathf.Max(0.5f, range) * Mathf.Max(0.5f, range);
            foreach (CharacterRuntime candidate in FindObjectsByType<CharacterRuntime>())
            {
                if (candidate == null || candidate == runtime || candidate.Health == null || candidate.Health.IsDead ||
                    candidate.TeamId == runtime.TeamId || candidate.TeamId == TeamId.Neutral ||
                    candidate.Abilities.IsExecutionHidden || SmokeField.BlocksSight(transform.position, candidate.transform.position)) continue;
                Vector3 offset = candidate.transform.position - transform.position;
                offset.y = 0f;
                float distance = offset.sqrMagnitude;
                if (distance >= bestDistance) continue;
                bestDistance = distance;
                best = candidate;
            }
            return best;
        }

        IEnumerator ExecuteGrapple(AbilityDefinition ability)
        {
            Vector3 direction = runtime.Motor.Facing;
            Vector3 origin = transform.position + Vector3.up;
            float distance = AbilityAimSolution.GrappleDistance(runtime, ability, direction, out CharacterRuntime target);
            Vector3 end = origin + direction * distance;
            GameObject rope = new GameObject("Archer_Grapple_Rope");
            rope.AddComponent<OwnedAbilityEffect>().Configure(runtime, null);
            rope.AddComponent<ArcherGrappleRope>().Begin(runtime, end, ability.movementDuration + 0.18f);
            float travel = target != null ? Mathf.Max(0f, distance - 1.1f) : distance;
            ExecuteMovement(ability, travel);
            while (runtime.Motor.IsDashing && !runtime.Health.IsDead) yield return null;
            if (target == null || target.Health == null || target.Health.IsDead ||
                Vector3.Distance(transform.position, target.transform.position) > 2.4f) yield break;
            CharacterVisualAnimator visual = GetComponentInChildren<CharacterVisualAnimator>();
            if (visual != null) visual.PlayBasicStep(1);
            GameObject kick = new GameObject("Archer_Grapple_Kick");
            kick.transform.position = target.transform.position + Vector3.up;
            DamagePacket packet = new DamagePacket(runtime, ability, direction);
            packet.knockback = Mathf.Max(packet.knockback, ability.knockback);
            Hitbox hitbox = kick.AddComponent<Hitbox>();
            hitbox.Configure(runtime, packet, new Vector3(1f, 1.5f, 1f), 0.12f);
            foreach (Hurtbox hurt in target.GetComponentsInChildren<Hurtbox>()) hitbox.TryResolveHurtbox(hurt);
        }

        void BeginCloneTeleport(AbilityDefinition ability)
        {
            CloneTeleportController controller = GetComponent<CloneTeleportController>() ??
                gameObject.AddComponent<CloneTeleportController>();
            controller.Begin(runtime, ability);
        }

        void BeginMageConvergence(AbilityDefinition ability)
        {
            MageConvergenceController controller = GetComponent<MageConvergenceController>() ??
                gameObject.AddComponent<MageConvergenceController>();
            controller.Begin(runtime, ability);
        }

        internal Hitbox SpawnConvergenceProjectile(AbilityDefinition ability, Vector3 direction,
            float damageScale, float speedScale)
        {
            return SpawnV1Projectile(ability, direction, damageScale, speedScale);
        }

        internal void SetCooldownOverride(AbilitySlot slot, float seconds)
        {
            readyAt[slot] = Time.time + Mathf.Max(0f, seconds);
        }
    }
}
