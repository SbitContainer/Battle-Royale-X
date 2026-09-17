using UnityEngine;

namespace BattleRoyaleX
{
    public struct DamagePacket
    {
        public CharacterRuntime source;
        public AttackKind attackKind;
        public float damage;
        public float knockback;
        public bool clashable;
        public bool blockable;
        public bool parryable;
        public bool reflectable;
        public bool nullifiable;
        public float clashDamageFactor;
        public bool canDestroyMagicalProjectiles;
        public float attackInteractionCooldown;
        public float explosionRadius;
        public Vector3 direction;

        public DamagePacket(CharacterRuntime source, AbilityDefinition ability, Vector3 direction)
        {
            this.source = source;
            attackKind = ability.attackKind;
            damage = ability.damage * (source != null ? source.Modifiers.damageMultiplier : 1f);
            knockback = ability.knockback;
            clashable = ability.clashable;
            blockable = ability.blockable;
            parryable = ability.parryable;
            reflectable = ability.reflectable;
            nullifiable = ability.nullifiable;
            clashDamageFactor = ability.clashDamageFactor;
            canDestroyMagicalProjectiles = ability.canDestroyMagicalProjectiles;
            attackInteractionCooldown = ability.attackInteractionCooldown;
            explosionRadius = ability.explosionRadius;
            this.direction = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward;
        }
    }
}
