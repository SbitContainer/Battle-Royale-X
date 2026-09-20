using System.Linq;
using UnityEngine;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed class WorldPickup : MonoBehaviour
    {
        public ItemDefinition item;
        public ChoicePickupGroup choiceGroup;

        Transform visual;
        TextMesh label;
        bool collected;

        public bool IsCollected => collected;

        void Awake()
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null) collider.isTrigger = true;
        }

        void Start() => BuildPresentation();

        void Update()
        {
            if (visual != null)
            {
                visual.localPosition = Vector3.up * (0.42f + Mathf.Sin(Time.time * 2.6f + transform.position.x) * 0.10f);
                visual.Rotate(Vector3.up, 55f * Time.deltaTime, Space.World);
            }

            if (label != null && Camera.main != null)
            {
                Vector3 direction = label.transform.position - Camera.main.transform.position;
                label.transform.rotation = Quaternion.LookRotation(direction, Camera.main.transform.up);
            }
        }

        public bool CanCollect(CharacterRuntime character)
        {
            return !collected && item != null && character != null && character.Inventory != null &&
                character.Inventory.CanAdd(item);
        }

        public bool TryCollect(CharacterRuntime character)
        {
            if (!CanCollect(character) || !character.Inventory.TryAdd(item)) return false;
            collected = true;
            CombatEvents.Raise(new CombatEventData(CombatEventKind.ItemPickup, transform.position, character, character,
                item: item));
            if (choiceGroup != null) choiceGroup.Choose(this);
            else Destroy(gameObject);
            return true;
        }

        public static WorldPickup FindNearestCollectible(CharacterRuntime character, float radius)
        {
            if (character == null) return null;
            float maxSqr = radius * radius;
            return FindObjectsByType<WorldPickup>(FindObjectsSortMode.None)
                .Where(pickup => pickup != null && pickup.CanCollect(character) &&
                    (pickup.transform.position - character.transform.position).sqrMagnitude <= maxSqr)
                .OrderBy(pickup => (pickup.transform.position - character.transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        void OnTriggerEnter(Collider other)
        {
            // Mobile collection is intentional through the PEGAR button. Desktop retains walk-over pickup.
            if (Application.isMobilePlatform) return;
            TryCollect(other.GetComponentInParent<CharacterRuntime>());
        }

        void BuildPresentation()
        {
            foreach (Renderer renderer in GetComponents<Renderer>()) renderer.enabled = false;

            Color color = ColorFor(item);
            GameObject visualObject = new GameObject("Pickup_Visual");
            visualObject.transform.SetParent(transform, false);
            visual = visualObject.transform;

            CreatePedestal(color);
            if (item != null && item.kind == ItemKind.Heal) CreatePotion(color);
            else if (item != null && item.kind == ItemKind.Tactical) CreateGrenade(color);
            else if (item != null && item.kind == ItemKind.Variation) CreateRune(color);
            else if (item != null && item.kind == ItemKind.BackpackUpgrade) CreateBackpack(color);
            else CreateOrb(color);

            GameObject lightObject = new GameObject("Pickup_Light");
            lightObject.transform.SetParent(visual, false);
            lightObject.transform.localPosition = Vector3.up * 0.32f;
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.range = 2.7f;
            light.intensity = 1.25f;

            GameObject labelObject = new GameObject("Pickup_Label");
            labelObject.transform.SetParent(transform, false);
            labelObject.transform.localPosition = Vector3.up * 1.48f;
            label = labelObject.AddComponent<TextMesh>();
            label.text = LabelFor(item);
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = 0.055f;
            label.fontSize = 52;
            label.color = color;
        }

        void CreatePedestal(Color color)
        {
            Primitive("Pedestal", PrimitiveType.Cylinder, visual, new Vector3(0f, -0.20f, 0f),
                new Vector3(0.62f, 0.06f, 0.62f), color * 0.42f);
        }

        void CreatePotion(Color color)
        {
            Primitive("Potion_Bottle", PrimitiveType.Cylinder, visual, Vector3.zero,
                new Vector3(0.24f, 0.42f, 0.24f), color);
            Primitive("Potion_Shoulder", PrimitiveType.Sphere, visual, new Vector3(0f, 0.24f, 0f),
                new Vector3(0.34f, 0.24f, 0.34f), color);
            Primitive("Potion_Stopper", PrimitiveType.Cylinder, visual, new Vector3(0f, 0.47f, 0f),
                new Vector3(0.13f, 0.14f, 0.13f), new Color(0.32f, 0.18f, 0.08f));
        }

        void CreateGrenade(Color color)
        {
            Primitive("Grenade_Body", PrimitiveType.Sphere, visual, new Vector3(0f, 0.05f, 0f),
                new Vector3(0.48f, 0.58f, 0.48f), color * 0.72f);
            Primitive("Grenade_Cap", PrimitiveType.Cylinder, visual, new Vector3(0f, 0.40f, 0f),
                new Vector3(0.16f, 0.14f, 0.16f), color);
            GameObject pin = Primitive("Grenade_Pin", PrimitiveType.Cylinder, visual, new Vector3(0.20f, 0.50f, 0f),
                new Vector3(0.05f, 0.16f, 0.05f), color);
            pin.transform.localRotation = Quaternion.Euler(0f, 0f, 70f);
        }

        void CreateRune(Color color)
        {
            GameObject diamond = Primitive("Skill_Rune", PrimitiveType.Cube, visual, new Vector3(0f, 0.12f, 0f),
                new Vector3(0.42f, 0.42f, 0.16f), color);
            diamond.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            Primitive("Rune_Core", PrimitiveType.Sphere, visual, new Vector3(0f, 0.12f, -0.10f),
                Vector3.one * 0.17f, Color.white);
        }

        void CreateBackpack(Color color)
        {
            Primitive("Backpack_Body", PrimitiveType.Cube, visual, new Vector3(0f, 0.10f, 0f),
                new Vector3(0.46f, 0.55f, 0.28f), color);
            Primitive("Backpack_Pocket", PrimitiveType.Cube, visual, new Vector3(0f, 0.02f, -0.18f),
                new Vector3(0.32f, 0.24f, 0.12f), color * 0.7f);
        }

        void CreateOrb(Color color)
        {
            Primitive("Item_Orb", PrimitiveType.Sphere, visual, new Vector3(0f, 0.12f, 0f),
                Vector3.one * 0.42f, color);
        }

        static GameObject Primitive(string name, PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, Color color)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = scale;
            Collider collider = go.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            Renderer renderer = go.GetComponent<Renderer>();
            Material material = CreateMaterial(color);
            if (renderer != null && material != null) renderer.sharedMaterial = material;
            PickupRuntimeMaterial cleanup = go.AddComponent<PickupRuntimeMaterial>();
            cleanup.material = material;
            return go;
        }

        static string LabelFor(ItemDefinition definition)
        {
            if (definition == null) return "ITEM";
            if (definition.kind == ItemKind.Variation && definition.variationAbility != null)
                return definition.variationAbility.requiredClass == CharacterClass.Assassin
                    ? "ASSASSINO\n" + definition.displayName.Replace("Runa: ", "").ToUpperInvariant()
                    : "GUERREIRO\n" + definition.displayName.Replace("Runa: ", "").ToUpperInvariant();
            return definition.displayName.ToUpperInvariant();
        }

        static Material CreateMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return null;
            Material material = new Material(shader) { name = "BRX_Pickup_Runtime" };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color")) material.color = color;
            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 0.65f);
            }
            return material;
        }

        public static Color ColorFor(ItemDefinition definition)
        {
            if (definition == null) return Color.white;
            if (definition.kind == ItemKind.Variation && definition.variationAbility != null)
            {
                Color classColor = definition.variationAbility.requiredClass == CharacterClass.Assassin
                    ? new Color(0.72f, 0.22f, 1f)
                    : new Color(1f, 0.56f, 0.10f);
                if (definition.variationAbility.slot == AbilitySlot.Defense) return Color.Lerp(classColor, new Color(0.12f, 0.72f, 1f), 0.35f);
                if (definition.variationAbility.slot == AbilitySlot.Ultimate) return Color.Lerp(classColor, Color.white, 0.25f);
                return classColor;
            }
            switch (definition.kind)
            {
                case ItemKind.Heal: return new Color(0.16f, 1f, 0.38f);
                case ItemKind.Energy: return new Color(0.12f, 0.62f, 1f);
                case ItemKind.CooldownRefresh: return new Color(1f, 0.82f, 0.12f);
                case ItemKind.Tactical:
                    return definition.tacticalKind == TacticalKind.Smoke ? new Color(0.70f, 0.78f, 0.84f) : new Color(0.12f, 0.92f, 0.88f);
                default: return new Color(1f, 0.65f, 0.12f);
            }
        }
    }

    public sealed class PickupRuntimeMaterial : MonoBehaviour
    {
        public Material material;
        void OnDestroy() { if (material != null) Destroy(material); }
    }
}
