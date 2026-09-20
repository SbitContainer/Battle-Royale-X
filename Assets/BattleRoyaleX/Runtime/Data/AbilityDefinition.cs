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
        [Tooltip("Temporarily prevents the victim from starting skills after this attack hits. Movement remains available.")]
        [Min(0f)] public float skillLockOnHit;

        [Header("Basic Combo")]
        [Range(1, 3)] public int comboSteps = 1;
        [Range(0.05f, 0.35f)] public float comboInputBuffer = 0.18f;
        [Min(0.1f)] public float comboSecondDamageMultiplier = 1f;
        [Min(0.1f)] public float comboThirdDamageMultiplier = 1.25f;

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

        [Header("Post-dash movement bonus (independent of ultimate)")]
        [Min(0f)] public float speedBonusDuration;
        [Min(1f)] public float speedBonusMultiplier = 1f;

        [Header("Two-stage hunt")]
        [Min(1f)] public float huntAcquireRange = 12f;
        [Min(1f)] public float huntSpeed = 22f;
        [Min(0.1f)] public float huntMaxDuration = 1.5f;
        [Min(0f)] public float huntFirstDamage = 6f;
        [Min(0f)] public float huntFirstPush = 2f;
        [Min(0f)] public float huntOvershoot = 3.5f;

        [Header("Escape smoke")]
        [Min(1f)] public float smokeRadius = 6f;
        [Min(0.1f)] public float smokeDuration = 4f;

        [Header("Charged dash sequence")]
        [Range(1, 8)] public int chargeCount = 5;
        [Min(0.1f)] public float chargeWindow = 6f;

        [Header("Manual counter opportunity")]
        [Min(0f)] public float counterBonusDamage = 0f;
        [Min(0f)] public float counterWindow = 0f;
        [Min(0f)] public float counterKnockback = 0f;
        public bool counterOnBlock;

        [Header("Defensive redirection")]
        public bool redirectOnDefense;
        [Min(0f)] public float redirectDistance = 3f;
        [Min(0.05f)] public float redirectDuration = 0.18f;

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
