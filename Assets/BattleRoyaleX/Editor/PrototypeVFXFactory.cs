#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BattleRoyaleX.EditorTools
{
    public static class PrototypeVFXFactory
    {
        const string Root = "Assets/BattleRoyaleX/Visual/VFX";
        const string ProfileRoot = "Assets/BattleRoyaleX/Visual/Profiles";

        [MenuItem("Battle Royale X/Visual/Create Prototype VFX")]
        public static void CreatePrototypeVFX()
        {
            foreach (string folder in new[] { Root, Root + "/Shared", Root + "/Shared/Materials", Root + "/Warrior", Root + "/Assassin",
                Root + "/Mage", Root + "/Archer", Root + "/World", Root + "/Titan", ProfileRoot })
                EnsureFolder(folder);

            Dictionary<string, GameObject> shared = new Dictionary<string, GameObject>();
            foreach (string id in new[] { "Hit", "BloodLight", "BloodHeavy", "Block", "Parry", "Clash", "Dodge", "Heal" })
                shared[id] = CreateParticlePrefab(Root + "/Shared/VFX_" + id + ".prefab", SharedColor(id), id.Contains("Heavy") ? 1.5f : 1f);

            string[] abilityGuids = AssetDatabase.FindAssets("t:AbilityDefinition", new[] { "Assets/BattleRoyaleX/GeneratedData/Abilities" });
            List<AbilityVisualProfile> profiles = new List<AbilityVisualProfile>();
            foreach (string guid in abilityGuids)
            {
                AbilityDefinition ability = AssetDatabase.LoadAssetAtPath<AbilityDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (ability == null) continue;
                string classFolder = ClassFolder(ability.requiredClass);
                Color color = ClassColor(ability.requiredClass);
                GameObject prefab = CreateParticlePrefab($"{Root}/{classFolder}/VFX_{ability.abilityId}.prefab",
                    color, ability.slot == AbilitySlot.Ultimate ? 1.7f : ability.slot == AbilitySlot.BasicAttack ? 0.8f : 1.15f);
                string profilePath = $"{ProfileRoot}/Visual_{ability.abilityId}.asset";
                AbilityVisualProfile profile = AssetDatabase.LoadAssetAtPath<AbilityVisualProfile>(profilePath) ??
                    ScriptableObject.CreateInstance<AbilityVisualProfile>();
                profile.abilityId = ability.abilityId;
                profile.characterClass = ability.requiredClass;
                profile.castPrefab = prefab;
                profile.projectilePrefab = IsProjectile(ability) ? prefab : null;
                profile.areaPrefab = IsArea(ability) ? prefab : null;
                profile.impactPrefab = prefab;
                profile.primaryTint = color;
                profile.visualScale = ability.slot == AbilitySlot.Ultimate ? 1.5f : 1f;
                if (!AssetDatabase.Contains(profile)) AssetDatabase.CreateAsset(profile, profilePath);
                EditorUtility.SetDirty(profile);
                ability.visualProfile = profile;
                EditorUtility.SetDirty(ability);
                profiles.Add(profile);
            }

            List<CharacterVisualProfile> characterProfiles = new List<CharacterVisualProfile>();
            foreach (string guid in AssetDatabase.FindAssets("t:CharacterDefinition", new[] { "Assets/BattleRoyaleX/GeneratedData/Characters" }))
            {
                CharacterDefinition definition = AssetDatabase.LoadAssetAtPath<CharacterDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (definition == null) continue;
                string profilePath = $"{ProfileRoot}/Visual_Character_{definition.characterClass}.asset";
                CharacterVisualProfile profile = AssetDatabase.LoadAssetAtPath<CharacterVisualProfile>(profilePath) ??
                    ScriptableObject.CreateInstance<CharacterVisualProfile>();
                profile.characterClass = definition.characterClass;
                profile.lightHitPrefab = shared["Hit"]; profile.heavyHitPrefab = shared["BloodHeavy"];
                profile.bloodLightPrefab = shared["BloodLight"]; profile.bloodHeavyPrefab = shared["BloodHeavy"];
                profile.dodgePrefab = shared["Dodge"];
                if (!AssetDatabase.Contains(profile)) AssetDatabase.CreateAsset(profile, profilePath);
                definition.visualProfile = profile;
                EditorUtility.SetDirty(profile); EditorUtility.SetDirty(definition);
                characterProfiles.Add(profile);
            }

            CombatVFXLibrary library = AssetDatabase.LoadAssetAtPath<CombatVFXLibrary>(ProfileRoot + "/CombatVFXLibrary.asset") ??
                ScriptableObject.CreateInstance<CombatVFXLibrary>();
            library.hitPrefab = shared["Hit"]; library.blockPrefab = shared["Block"]; library.parryPrefab = shared["Parry"];
            library.clashPrefab = shared["Clash"]; library.dodgePrefab = shared["Dodge"]; library.healPrefab = shared["Heal"];
            if (!AssetDatabase.Contains(library)) AssetDatabase.CreateAsset(library, ProfileRoot + "/CombatVFXLibrary.asset");
            EditorUtility.SetDirty(library);

            VisualProfileRegistry registry = AssetDatabase.LoadAssetAtPath<VisualProfileRegistry>(ProfileRoot + "/VisualProfileRegistry.asset") ??
                ScriptableObject.CreateInstance<VisualProfileRegistry>();
            registry.abilities = profiles.OrderBy(p => p.abilityId).ToArray();
            registry.characters = characterProfiles.OrderBy(p => (int)p.characterClass).ToArray();
            if (!AssetDatabase.Contains(registry)) AssetDatabase.CreateAsset(registry, ProfileRoot + "/VisualProfileRegistry.asset");
            EditorUtility.SetDirty(registry);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Battle Royale X: prototype VFX generated ({profiles.Count} ability profiles + 8 shared effects).");
        }

        static GameObject CreateParticlePrefab(string path, Color color, float scale)
        {
            GameObject root = new GameObject(Path.GetFileNameWithoutExtension(path));
            ParticleSystem particles = root.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.duration = 0.45f; main.loop = false; main.startLifetime = 0.35f;
            main.startSpeed = 3.2f * scale; main.startSize = 0.18f * scale; main.startColor = color;
            main.maxParticles = 36;
            var emission = particles.emission; emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)Mathf.RoundToInt(16f * scale)) });
            var shape = particles.shape; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = 24f; shape.radius = 0.08f;
            var colorOverLifetime = particles.colorOverLifetime; colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(new[] { new GradientColorKey(color, 0f), new GradientColorKey(color * 0.45f, 1f) },
                new[] { new GradientAlphaKey(color.a, 0f), new GradientAlphaKey(0f, 1f) });
            colorOverLifetime.color = gradient;
            ParticleSystemRenderer renderer = root.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = MaterialFor(Path.GetFileNameWithoutExtension(path), color);
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        static bool IsProjectile(AbilityDefinition ability) => ability.attackKind == AttackKind.Projectile ||
            ability.behavior == AbilityBehavior.ProjectileAttack || ability.behavior == AbilityBehavior.SeekingProjectile ||
            ability.behavior == AbilityBehavior.MultiShot || ability.behavior == AbilityBehavior.ComboProjectileUltimate;
        static bool IsArea(AbilityDefinition ability) => ability.attackKind == AttackKind.Area ||
            ability.behavior == AbilityBehavior.AreaAttack || ability.behavior == AbilityBehavior.SlowField ||
            ability.behavior == AbilityBehavior.PullTrap || ability.behavior == AbilityBehavior.Repulsion;
        static string ClassFolder(CharacterClass value) => value == CharacterClass.Warrior ? "Warrior" :
            value == CharacterClass.Assassin ? "Assassin" : value == CharacterClass.Mage ? "Mage" : "Archer";
        static Color ClassColor(CharacterClass value) => value == CharacterClass.Warrior ? new Color(1f, 0.45f, 0.12f) :
            value == CharacterClass.Assassin ? new Color(0.65f, 0.22f, 1f) : value == CharacterClass.Mage ?
            new Color(0.1f, 0.85f, 1f) : new Color(0.95f, 0.72f, 0.18f);
        static Color SharedColor(string id) => id.StartsWith("Blood") ? new Color(0.75f, 0.01f, 0.02f) :
            id == "Heal" ? new Color(0.1f, 1f, 0.35f) : id == "Parry" ? new Color(1f, 0.85f, 0.25f) : Color.white;

        static Material MaterialFor(string name, Color color)
        {
            string path = $"{Root}/Shared/Materials/{name}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Particles/Standard Unlit");
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }
            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}
#endif
