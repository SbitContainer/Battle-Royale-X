using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class ChoicePickupGroup : MonoBehaviour
    {
        bool chosen;

        public void Choose(WorldPickup selected)
        {
            if (chosen) return;
            chosen = true;
            WorldPickup[] pickups = GetComponentsInChildren<WorldPickup>(true);
            foreach (WorldPickup pickup in pickups)
            {
                if (pickup != selected) Destroy(pickup.gameObject);
            }
            Destroy(selected.gameObject);
            Destroy(gameObject, 0.05f);
        }
    }
}
