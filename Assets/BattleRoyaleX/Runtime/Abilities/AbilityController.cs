using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class AbilityController : MonoBehaviour
    {
        CharacterRuntime runtime;
        readonly Dictionary<AbilitySlot, AbilityDefinition> equipped = new Dictionary<AbilitySlot, AbilityDefinition>();
        readonly Dictionary<AbilitySlot, float> readyAt = new Dictionary<AbilitySlot, float>();

        bool returnArmed;
        Vector3 returnPosition;
        float returnExpiresAt;

        void Awake() => runtime = GetComponent<CharacterRuntime>();

        public void Initialize(CharacterDefinition definition)
        {
            equipped[AbilitySlot.BasicAttack] = definition.basicAttack;
            equipped[AbilitySlot.Defense] = definition.defenseBase;
            equipped[AbilitySlot.Movement] = definition.movementBase;
            equipped[AbilitySlot.Ultimate] = definition.ultimateBase;
            readyAt.Clear();
        }

        public AbilityDefinition GetEquipped(AbilitySlot slot) => equipped.TryGetValue(slot, out var value) ? value : null;

        public bool TryUse(AbilitySlot slot)
        {
            if (runtime == null || runtime.State.InputLocked || runtime.Health.IsDead) return false;
            AbilityDefinition ability = GetEquipped(slot);
            if (ability == null) return false;

            if (ability.behavior == AbilityBehavior.DashReturn && returnArmed && Time.time <= returnExpiresAt)
            {
                runtime.Motor.Teleport(returnPosition);
                returnArmed = false;
                return true;
            }

            float currentReadyAt = readyAt.TryGetValue(slot, out float t) ? t : 0f;
            if (Time.time < currentReadyAt) return false;
            if (!runtime.Energy.TrySpend(ability.energyCost)) return false;

            float cooldown = ability.cooldown;
            if (slot == AbilitySlot.Movement) cooldown *= runtime.Modifiers.movementCooldownMultiplier;
            readyAt[slot] = Time.time + Mathf.Max(0f, cooldown);
            StartCoroutine(Execute(ability));
            return true;
        }

        IEnumerator Execute(AbilityDefinition ability)
        {
            if (ability.startup > 0f) yield return new WaitForSeconds(ability.startup);

            switch (ability.behavior)
            {
                case AbilityBehavior.MeleeAttack:
                    SpawnMeleeHitbox(ability);
                    break;

                case AbilityBehavior.ProjectileAttack:
                    SpawnProjectileHitbox(ability);
                    break;

                case AbilityBehavior.AreaAttack:
                    SpawnAreaHitbox(ability);
                    break;

                case AbilityBehavior.Guard:
                case AbilityBehavior.Parry:
                    runtime.Defense.Activate(ability.defenseKind, ability.defenseDuration, ability.perfectWindow,
                        ability.damageReduction, ability.specialInteractionCooldown);
                    break;

                case AbilityBehavior.Dodge:
                    runtime.State.SetInvulnerable(Mathf.Max(ability.invulnerabilityDuration, ability.movementDuration));
                    runtime.Motor.Dash(ability.movementDistance, ability.movementDuration, ability.passThroughCharacters,
                        ability.invulnerabilityDuration);
                    break;

                case AbilityBehavior.Dash:
                case AbilityBehavior.DashThrough:
                    runtime.Motor.Dash(ability.movementDistance, ability.movementDuration, ability.passThroughCharacters,
                        ability.invulnerabilityDuration);
                    break;

                case AbilityBehavior.DashReturn:
                    returnPosition = transform.position;
                    returnExpiresAt = Time.time + ability.returnWindow;
                    returnArmed = true;
                    runtime.Motor.Dash(ability.movementDistance, ability.movementDuration, ability.passThroughCharacters,
                        ability.invulnerabilityDuration);
                    break;

                case AbilityBehavior.UltimateBuff:
                    runtime.ApplyTimedModifiers(ability.ToRuntimeModifiers(), ability.buffDuration);
                    break;
            }

            if (ability.recovery > 0f)
            {
                runtime.State.LockInput(true);
                yield return new WaitForSeconds(ability.recovery);
                runtime.State.LockInput(false);
            }
        }

        void SpawnMeleeHitbox(AbilityDefinition ability)
        {
            GameObject go = new GameObject($"Hitbox_{ability.displayName}");
            go.transform.position = transform.position + runtime.Motor.Facing * Mathf.Max(0.2f, ability.range * 0.5f);
            go.transform.rotation = Quaternion.LookRotation(runtime.Motor.Facing, Vector3.up);
            go.layer = gameObject.layer;
            Hitbox hitbox = go.AddComponent<Hitbox>();
            DamagePacket packet = new DamagePacket(runtime, ability, runtime.Motor.Facing);
            hitbox.Configure(runtime, packet, new Vector3(ability.width, ability.height, ability.range), ability.activeTime);
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
            GameObject go = new GameObject($"Area_{ability.displayName}");
            go.transform.position = transform.position + runtime.Motor.Facing * Mathf.Max(0.5f, ability.range);
            Hitbox hitbox = go.AddComponent<Hitbox>();
            DamagePacket packet = new DamagePacket(runtime, ability, runtime.Motor.Facing);
            float diameter = Mathf.Max(0.5f, ability.explosionRadius * 2f);
            hitbox.Configure(runtime, packet, new Vector3(diameter, ability.height, diameter), ability.activeTime);
        }

        public bool EquipVariation(AbilityDefinition variation)
        {
            if (variation == null || variation.slot == AbilitySlot.BasicAttack) return false;
            if (variation.classRestricted && runtime.Definition != null && runtime.Definition.characterClass != variation.requiredClass) return false;
            equipped[variation.slot] = variation;
            CombatEvents.Raise(new CombatEventData(CombatEventKind.VariationSwap, transform.position, runtime, runtime));
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
