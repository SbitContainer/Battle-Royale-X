using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class WorldPickup : MonoBehaviour
    {
        public ItemDefinition item;
        public ChoicePickupGroup choiceGroup;

        void Awake()
        {
            Collider c = GetComponent<Collider>();
            if (c != null) c.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            CharacterRuntime character = other.GetComponentInParent<CharacterRuntime>();
            if (character == null || item == null) return;
            if (!character.Inventory.TryAdd(item)) return;
            if (choiceGroup != null) choiceGroup.Choose(this);
            else Destroy(gameObject);
        }
    }
}
