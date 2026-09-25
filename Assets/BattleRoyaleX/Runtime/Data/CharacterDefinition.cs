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
        [Tooltip("V1 Skill 1 variants in A/B/C order.")]
        public AbilityDefinition[] skill1Variants = new AbilityDefinition[3];
        [Tooltip("V1 Skill 2 variants in A/B/C order.")]
        public AbilityDefinition[] skill2Variants = new AbilityDefinition[3];
        [Tooltip("V1 Ultimate variants in A/B/C order.")]
        public AbilityDefinition[] ultimateVariants = new AbilityDefinition[3];
        public CharacterVisualProfile visualProfile;

        [Header("Legacy loadout compatibility")]
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

        public AbilityDefinition GetVariant(AbilitySlot slot, int index)
        {
            index = Mathf.Clamp(index, 0, 2);
            AbilityDefinition[] variants = slot == AbilitySlot.Skill1 ? skill1Variants :
                slot == AbilitySlot.Skill2 ? skill2Variants : slot == AbilitySlot.Ultimate ? ultimateVariants : null;
            if (variants != null && variants.Length == 3 && variants[index] != null) return variants[index];

            if (slot == AbilitySlot.Skill1)
                return index == 0 ? defenseBase : index == 1 ? defenseVariantA : defenseVariantB;
            if (slot == AbilitySlot.Skill2)
                return index == 0 ? movementBase : index == 1 ? movementVariantA : movementVariantB;
            if (slot == AbilitySlot.Ultimate)
                return index == 0 ? ultimateBase : index == 1 ? ultimateVariantA : ultimateVariantB;
            return slot == AbilitySlot.BasicAttack ? basicAttack : null;
        }

        public void SetV1Loadout(AbilityDefinition skill1A, AbilityDefinition skill1B, AbilityDefinition skill1C,
            AbilityDefinition skill2A, AbilityDefinition skill2B, AbilityDefinition skill2C,
            AbilityDefinition ultimateA, AbilityDefinition ultimateB, AbilityDefinition ultimateC)
        {
            skill1Variants = new[] { skill1A, skill1B, skill1C };
            skill2Variants = new[] { skill2A, skill2B, skill2C };
            ultimateVariants = new[] { ultimateA, ultimateB, ultimateC };
        }
    }
}
