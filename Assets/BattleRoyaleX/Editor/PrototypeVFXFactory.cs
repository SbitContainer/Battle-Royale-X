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
            GameObject convergenceRing = CreateWorldPrefab(Root + "/Mage/VFX_Convergence_Shockwave.prefab",
                new Color(0.2f, 0.75f, 1f), true, false);

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
                profile.projectilePrefab = IsProjectile(ability) ? CreateWorldPrefab(
                    $"{Root}/{classFolder}/VFX_{ability.abilityId}_Projectile.prefab", color, false,
                    ability.requiredClass == CharacterClass.Archer) : null;
                profile.areaPrefab = IsArea(ability) ? CreateWorldPrefab(
                    $"{Root}/{classFolder}/VFX_{ability.abilityId}_Field.prefab", color, true, false) : null;
                if (ability.behavior == AbilityBehavior.ComboProjectileUltimate) profile.areaPrefab = convergenceRing;
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
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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

        static GameObject CreateWorldPrefab(string path, Color color, bool field, bool arrow)
        {
            GameObject root = new GameObject(Path.GetFileNameWithoutExtension(path));
            Material material = MaterialFor(root.name, field ? Color.white : color);
            if (field)
            {
                material.SetTexture("_BaseMap", Texture2D.whiteTexture);
                EditorUtility.SetDirty(material);
                for (int ringIndex = 0; ringIndex < 2; ringIndex++)
                {
                    GameObject ring = new GameObject("Radius_" + ringIndex);
                    ring.transform.SetParent(root.transform, false);
                    LineRenderer line = ring.AddComponent<LineRenderer>();
                    line.useWorldSpace = false; line.loop = true; line.positionCount = 64;
                    line.sharedMaterial = material; line.startWidth = line.endWidth = 0.06f;
                    line.startColor = line.endColor = color;
                    float radius = ringIndex == 0 ? 1f : 0.78f;
                    for (int i = 0; i < 64; i++)
                    {
                        float angle = i * Mathf.PI * 2f / 64f;
                        line.SetPosition(i, new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius);
                    }
                }
                // Small runic spokes keep the boundary readable without filling the whole disc.
                for (int i = 0; i < 12; i++)
                {
                    GameObject mark = new GameObject("Rune_" + i);
                    mark.transform.SetParent(root.transform, false);
                    LineRenderer line = mark.AddComponent<LineRenderer>();
                    line.useWorldSpace = false; line.positionCount = 3;
                    line.sharedMaterial = material; line.startWidth = line.endWidth = 0.04f;
                    line.startColor = line.endColor = color;
                    Quaternion rotation = Quaternion.Euler(0f, i * 30f, 0f);
                    line.SetPositions(new[] { rotation * new Vector3(-0.035f, 0f, 0.85f),
                        rotation * new Vector3(0f, 0f, 0.92f), rotation * new Vector3(0.035f, 0f, 0.85f) });
                }
                GameObject flow = new GameObject("InwardMotes"); flow.transform.SetParent(root.transform, false);
                ParticleSystem motes = flow.AddComponent<ParticleSystem>();
                motes.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var main = motes.main;
                main.loop = true; main.startLifetime = 1.6f; main.startSpeed = 0f;
                main.startSize = 0.035f; main.startColor = color; main.maxParticles = 32;
                main.simulationSpace = ParticleSystemSimulationSpace.Local;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                var emission = motes.emission; emission.rateOverTime = 12f;
                var shape = motes.shape; shape.shapeType = ParticleSystemShapeType.Circle;
                shape.radius = 0.88f; shape.radiusThickness = 0f; shape.rotation = new Vector3(90f, 0f, 0f);
                var fade = motes.colorOverLifetime; fade.enabled = true;
                Gradient gradient = new Gradient();
                gradient.SetKeys(new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
                    new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.8f, 0.2f), new GradientAlphaKey(0f, 1f) });
                fade.color = gradient;
                motes.GetComponent<ParticleSystemRenderer>().sharedMaterial = MaterialFor(root.name + "_Motes", Color.white);
                root.AddComponent<ArcaneFieldPresentation>();
            }
            else
            {
                GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                core.name = arrow ? "ArrowCore" : "ArcaneCore";
                Object.DestroyImmediate(core.GetComponent<Collider>());
                core.transform.SetParent(root.transform, false);
                core.transform.localScale = arrow ? new Vector3(0.08f, 0.08f, 0.65f) : Vector3.one * 0.26f;
                core.GetComponent<Renderer>().sharedMaterial = material;
                TrailRenderer trail = root.AddComponent<TrailRenderer>();
                trail.sharedMaterial = material; trail.time = arrow ? 0.12f : 0.24f;
                trail.startWidth = arrow ? 0.09f : 0.22f; trail.endWidth = 0f;
                trail.minVertexDistance = 0.06f; trail.startColor = color;
                trail.endColor = new Color(color.r, color.g, color.b, 0f);
            }
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }
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
            // Particle quads need a soft alpha mask and transparent blending in URP.
            // An opaque untextured material exposes the square billboard geometry.
            material.SetTexture("_BaseMap", SoftParticleTexture());
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite", 0f);
            material.SetOverrideTag("RenderType", "Transparent");
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            EditorUtility.SetDirty(material);
            return material;
        }

        static Texture2D SoftParticleTexture()
        {
            const string path = Root + "/Shared/Materials/SoftParticle.asset";
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture != null) return texture;
            const int size = 64;
            texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
                { name = "SoftParticle", wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float radius = new Vector2((x + 0.5f) / size * 2f - 1f,
                        (y + 0.5f) / size * 2f - 1f).magnitude;
                    float alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(1f - radius));
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha * alpha);
                }
            texture.SetPixels(pixels);
            texture.Apply();
            AssetDatabase.CreateAsset(texture, path);
            return texture;
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
