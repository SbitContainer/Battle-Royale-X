using UnityEngine;

namespace BattleRoyaleX
{
    [CreateAssetMenu(menuName = "Battle Royale X/Item Definition", fileName = "Item_")]
    public sealed class ItemDefinition : ScriptableObject
    {
        public string itemId;
        public string displayName;
        public ItemKind kind;
        public Sprite icon;

        [Header("Consumable Values")]
        public float amount = 20f;
        public float cooldownReductionSeconds = 2f;
        [Min(0f)] public float useDuration = 0f;
        public bool interruptible = true;

        [Header("Variation")]
        public AbilityDefinition variationAbility;

        [Header("Backpack")]
        [Min(3)] public int backpackCapacity = 4;

        [Header("Tactical")]
        public TacticalKind tacticalKind;
        public float tacticalRadius = 3f;
        public float tacticalDuration = 4f;
        public float tacticalForce = 7f;
    }
}
