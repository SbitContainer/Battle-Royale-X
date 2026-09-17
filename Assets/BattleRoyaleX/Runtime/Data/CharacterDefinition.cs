using UnityEngine;

namespace BattleRoyaleX
{
    [CreateAssetMenu(menuName = "Battle Royale X/Character Definition", fileName = "Character_")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string characterId;
        public string displayName;
        public CharacterClass characterClass;

        [Header("Base Stats")]
        [Min(1f)] public float maxHealth = 100f;
        [Min(0f)] public float maxEnergy = 100f;
        [Min(0f)] public float energyRegenPerSecond = 8f;
        [Min(0.1f)] public float moveSpeed = 5f;
        [Min(0.1f)] public float rotationSpeed = 900f;

        [Header("Base Loadout")]
        public AbilityDefinition basicAttack;
        public AbilityDefinition defenseBase;
        public AbilityDefinition movementBase;
        public AbilityDefinition ultimateBase;

        [Header("Defense Variants")]
        public AbilityDefinition defenseVariantA;
        public AbilityDefinition defenseVariantB;

        [Header("Movement Variants")]
        public AbilityDefinition movementVariantA;
        public AbilityDefinition movementVariantB;

        [Header("Ultimate Variants")]
        public AbilityDefinition ultimateVariantA;
        public AbilityDefinition ultimateVariantB;
    }
}
