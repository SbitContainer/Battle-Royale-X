using UnityEngine;

namespace BattleRoyaleX
{
    public sealed partial class AbilityController
    {
        bool TryUseV1SecondActivation(AbilityDefinition ability)
        {
            CloneTeleportController clones = GetComponent<CloneTeleportController>();
            if (clones != null && clones.Ability == ability && clones.CanTeleport)
                return clones.TryTeleport(runtime.Motor.Facing);

            MageConvergenceController convergence = GetComponent<MageConvergenceController>();
            if (convergence != null && convergence.Ability == ability && convergence.CanFireSecond)
                return convergence.TryFireSecond(runtime.Motor.Facing);
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
                mover.Configure(runtime, ability.projectileSpeed, ability.turnRate, 4f);
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

        Hitbox SpawnV1Projectile(AbilityDefinition ability, Vector3 direction, float damageScale, float speedScale)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) direction = runtime.Motor.Facing;
            direction.Normalize();
            GameObject go = new GameObject($"Projectile_{ability.abilityId}");
            go.transform.position = transform.position + direction * 1.1f + Vector3.up * 0.8f;
            go.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            Hitbox hitbox = go.AddComponent<Hitbox>();
            DamagePacket packet = new DamagePacket(runtime, ability, direction);
            packet.damage *= Mathf.Max(0f, damageScale);
            hitbox.Configure(runtime, packet, new Vector3(ability.width, ability.height, Mathf.Max(0.5f, ability.width)), 4f);
            ProjectileHitboxMover mover = go.AddComponent<ProjectileHitboxMover>();
            mover.direction = direction;
            mover.speed = ability.projectileSpeed * Mathf.Max(0.01f, speedScale);
            mover.maxLifetime = 4f;
            go.AddComponent<ProjectileInteractionState>();
            OwnedAbilityEffect marker = go.AddComponent<OwnedAbilityEffect>();
            marker.Configure(runtime, ability);
            return hitbox;
        }

        void SpawnSlowField(AbilityDefinition ability)
        {
            GameObject go = new GameObject($"SlowField_{ability.abilityId}");
            go.transform.position = transform.position + runtime.Motor.Facing * Mathf.Max(0f, ability.range);
            go.AddComponent<OwnedAbilityEffect>().Configure(runtime, ability);
            go.AddComponent<SlowField>().Configure(runtime, ability);
        }

        void SpawnPullField(AbilityDefinition ability)
        {
            GameObject go = new GameObject($"PullField_{ability.abilityId}");
            go.transform.position = transform.position + runtime.Motor.Facing * Mathf.Max(0f, ability.range);
            go.AddComponent<OwnedAbilityEffect>().Configure(runtime, ability);
            go.AddComponent<PullField>().Configure(runtime, ability);
        }

        void ExecuteBlink(AbilityDefinition ability)
        {
            Vector3 direction = runtime.Motor.Facing;
            float distance = Mathf.Max(0f, ability.movementDistance);
            if (Physics.Raycast(transform.position + Vector3.up, direction, out RaycastHit hit, distance,
                ~0, QueryTriggerInteraction.Ignore))
                distance = Mathf.Max(0f, hit.distance - 0.55f);
            runtime.Motor.Teleport(transform.position + direction * distance);
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
