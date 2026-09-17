using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class AirdropManager : MonoBehaviour
    {
        public float firstDropDelay = 35f;
        public float arenaHalfSize = 18f;
        public List<ItemDefinition> itemPool = new List<ItemDefinition>();
        bool spawned;

        void Update()
        {
            if (!spawned && Time.timeSinceLevelLoad >= firstDropDelay)
            {
                spawned = true;
                SpawnDrop();
            }
        }

        public void SpawnDrop()
        {
            Vector3 center = new Vector3(Random.Range(-arenaHalfSize, arenaHalfSize), 0.5f, Random.Range(-arenaHalfSize, arenaHalfSize));
            GameObject group = new GameObject("Airdrop_Choice");
            group.transform.position = center;
            ChoicePickupGroup choice = group.AddComponent<ChoicePickupGroup>();

            int count = Mathf.Min(3, itemPool.Count);
            for (int i = 0; i < count; i++)
            {
                ItemDefinition item = itemPool[Random.Range(0, itemPool.Count)];
                GameObject pickup = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pickup.name = "Drop_" + item.displayName;
                pickup.transform.SetParent(group.transform);
                float angle = i * Mathf.PI * 2f / Mathf.Max(1, count);
                pickup.transform.localPosition = new Vector3(Mathf.Cos(angle) * 1.4f, 0f, Mathf.Sin(angle) * 1.4f);
                Collider c = pickup.GetComponent<Collider>();
                c.isTrigger = true;
                WorldPickup wp = pickup.AddComponent<WorldPickup>();
                wp.item = item;
                wp.choiceGroup = choice;
            }
        }
    }
}
