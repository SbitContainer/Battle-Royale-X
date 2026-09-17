using UnityEngine;

namespace BattleRoyaleX
{
    [CreateAssetMenu(menuName = "Battle Royale X/Ability Definition", fileName = "Ability_")]
    public sealed class AbilityDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string abilityId;
        public string displayName;
        public AbilitySlot slot;
        public bool classRestricted = false;
        public CharacterClass requiredClass;
        public AbilityBehavior behavior;

        [Header("Costs / Timing")]
        [Min(0f)] public float cooldown = 3f;
        [Min(0f)] public float energyCost = 0f;
        [Min(0f)] public float startup = 0.08f;
        [Min(0.01f)] public float activeTime = 0.12f;
        [Min(0f)] public float recovery = 0.16f;

        [Header("Attack")]
        public AttackKind attackKind = AttackKind.Physical;
        [Min(0f)] public float damage = 10f;
        [Min(0f)] public float range = 1.8f;
        [Min(0.1f)] public float width = 1f;
        [Min(0.1f)] public float height = 1.5f;
        [Min(0f)] public float knockback = 1f;
        public bool clashable = true;
        public bool blockable = true;
        public bool parryable = true;
        public bool reflectable = false;
        public bool nullifiable = false;
        public bool canDestroyMagicalProjectiles = false;
        [Min(0f)] public float attackInteractionCooldown = 30f;
        [Min(0f)] public float projectileSpeed = 14f;
        [Min(0f)] public float explosionRadius = 2.5f;
        [Range(0f, 1f)] public float clashDamageFactor = 0.25f;

        [Header("Defense")]
        public DefenseKind defenseKind = DefenseKind.None;
        [Min(0.01f)] public float defenseDuration = 0.45f;
        [Min(0f)] public float perfectWindow = 0.12f;
        [Range(0f, 1f)] public float damageReduction = 0.7f;
        [Min(0f)] public float specialInteractionCooldown = 0f;

        [Header("Movement")]
        [Min(0f)] public float movementDistance = 4f;
        [Min(0.01f)] public float movementDuration = 0.16f;
        [Min(0f)] public float invulnerabilityDuration = 0f;
        [Min(0f)] public float returnWindow = 1.8f;
        public bool passThroughCharacters = false;

        [Header("Ultimate / Temporary Modifier")]
        [Min(0f)] public float buffDuration = 5f;
        [Min(0.01f)] public float damageMultiplier = 1f;
        [Min(0.01f)] public float moveSpeedMultiplier = 1f;
        [Min(0.01f)] public float staggerResistanceMultiplier = 1f;
        [Min(0.01f)] public float defenseWindowMultiplier = 1f;
        [Min(0.01f)] public float movementCooldownMultiplier = 1f;

        public RuntimeModifiers ToRuntimeModifiers() => new RuntimeModifiers
        {
            damageMultiplier = damageMultiplier,
            moveSpeedMultiplier = moveSpeedMultiplier,
            staggerResistanceMultiplier = staggerResistanceMultiplier,
            defenseWindowMultiplier = defenseWindowMultiplier,
            movementCooldownMultiplier = movementCooldownMultiplier
        };
    }
}
