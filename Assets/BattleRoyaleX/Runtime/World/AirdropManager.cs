using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class AirdropManager : MonoBehaviour
    {
        public float firstDropDelay = 12f;
        public float arenaHalfSize = 18f;
        public List<ItemDefinition> itemPool = new List<ItemDefinition>();
        bool spawned;

        void Awake()
        {
            // Existing generated scenes may carry the old 35-second serialized value.
            // Keep prototype drops visible quickly without making custom shorter values slower.
            firstDropDelay = Mathf.Min(firstDropDelay, 12f);
        }

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
            CreateDropMarker(group.transform);

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

        static void CreateDropMarker(Transform parent)
        {
            GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            beam.name = "Airdrop_Beacon";
            beam.transform.SetParent(parent, false);
            beam.transform.localPosition = Vector3.up * 0.9f;
            beam.transform.localScale = new Vector3(0.08f, 0.9f, 0.08f);
            Collider collider = beam.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            Renderer renderer = beam.GetComponent<Renderer>();
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader != null)
            {
                Material material = new Material(shader) { name = "BRX_Airdrop_Beacon" };
                Color color = new Color(1f, 0.46f, 0.06f);
                if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
                else material.color = color;
                if (material.HasProperty("_EmissionColor"))
                {
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", color * 1.1f);
                }
                renderer.sharedMaterial = material;
                PickupRuntimeMaterial cleanup = beam.AddComponent<PickupRuntimeMaterial>();
                cleanup.material = material;
            }

            GameObject lightObject = new GameObject("Airdrop_BeaconLight");
            lightObject.transform.SetParent(parent, false);
            lightObject.transform.localPosition = Vector3.up * 1.1f;
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.4f, 0.08f);
            light.range = 9f;
            light.intensity = 3f;

            GameObject labelObject = new GameObject("Airdrop_Label");
            labelObject.transform.SetParent(parent, false);
            labelObject.transform.localPosition = Vector3.up * 2f;
            labelObject.transform.rotation = Quaternion.Euler(68f, 45f, 0f);
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = "AIRDROP";
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = 0.035f;
            label.fontSize = 48;
            label.color = new Color(1f, 0.55f, 0.1f);
        }
    }
}
