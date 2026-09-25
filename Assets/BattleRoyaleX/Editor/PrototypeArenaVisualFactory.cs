#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BattleRoyaleX.EditorTools
{
    // Scene presentation only. Existing ground and obstacle colliders retain their geometry.
    public static class PrototypeArenaVisualFactory
    {
        const string Sources = "Assets/ThirdParty/Quaternius/";
        const string Materials = "Assets/BattleRoyaleX/GeneratedVisuals/Materials/";

        public static void Decorate()
        {
            Material stone = MakeMaterial("ArenaStone", "MedievalVillage/T_UnevenBrick_BaseColor.png", new Color(0.6f, 0.68f, 0.72f));
            Material leaves = MakeMaterial("ArenaLeaves", "Nature/Leaves_NormalTree.png", new Color(0.52f, 0.75f, 0.6f), true);
            Material bark = MakeMaterial("ArenaBark", "Nature/Bark_NormalTree.png", Color.white);
            Material rock = MakeMaterial("ArenaRock", "Nature/Rocks_Diffuse.png", new Color(0.7f, 0.75f, 0.8f));
            GameObject ground = GameObject.Find("Arena_Ground_45x45");
            if (ground != null)
            {
                Material floor = MakeMaterial("ArenaFloor", "MedievalVillage/T_UnevenBrick_BaseColor.png", new Color(0.35f, 0.42f, 0.43f));
                floor.mainTextureScale = Vector2.one * 14f;
                EditorUtility.SetDirty(floor);
                ground.GetComponent<Renderer>().sharedMaterial = floor;
            }
            GameObject decoration = new GameObject("Arena_Visuals");
            foreach (BoxCollider obstacle in Object.FindObjectsByType<BoxCollider>())
            {
                if (obstacle.name != "Arena_Obstacle") continue;
                obstacle.GetComponent<Renderer>().sharedMaterial = stone;
            }
            for (int i = 0; i < 4; i++)
            {
                Vector3 corner = new Vector3(i % 2 == 0 ? -19f : 19f, 0f, i < 2 ? -19f : 19f);
                Place("Nature/CommonTree_1.fbx", decoration.transform, corner, 4.5f, bark, leaves);
                Place("Nature/Pebble_Round_1.fbx", decoration.transform, corner * 0.91f, 1.2f, rock);
                Place("Nature/Bush_Common.fbx", decoration.transform, corner * 0.8f, 1.25f, leaves);
            }
            Place("MedievalVillage/Wall_Arch.fbx", decoration.transform, new Vector3(0f, 0f, 21f), 4f, stone);
            Place("MedievalVillage/Wall_UnevenBrick_Straight.fbx", decoration.transform, new Vector3(-6f, 0f, 21f), 3f, stone);
            Place("MedievalVillage/Wall_UnevenBrick_Straight.fbx", decoration.transform, new Vector3(6f, 0f, 21f), 3f, stone);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.55f, 0.65f, 0.76f);
            RenderSettings.ambientEquatorColor = new Color(0.35f, 0.4f, 0.43f);
            RenderSettings.ambientGroundColor = new Color(0.16f, 0.2f, 0.19f);
            if (Camera.main != null) Camera.main.backgroundColor = new Color(0.1f, 0.16f, 0.22f);
            AssetDatabase.SaveAssets();
        }

        static Material MakeMaterial(string name, string texture, Color tint, bool cutout = false)
        {
            string path = Materials + name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }
            material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(Sources + texture));
            material.SetColor("_BaseColor", tint); material.SetFloat("_Smoothness", 0.12f);
            if (cutout)
            {
                material.SetFloat("_AlphaClip", 1f); material.SetFloat("_Cutoff", 0.35f);
                material.SetFloat("_Cull", 0f); material.EnableKeyword("_ALPHATEST_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            }
            EditorUtility.SetDirty(material);
            return material;
        }

        static void Place(string asset, Transform parent, Vector3 position, float height, params Material[] materials)
        {
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(Sources + asset);
            if (model == null) { Debug.LogWarning("Arena asset unavailable: " + asset); return; }
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(model, parent);
            instance.transform.position = position;
            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;
            Bounds bounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
                Material[] assigned = renderer.sharedMaterials;
                for (int j = 0; j < assigned.Length; j++)
                {
                    string label = assigned[j] != null ? assigned[j].name.ToLowerInvariant() : renderer.name.ToLowerInvariant();
                    assigned[j] = materials.Length > 1 && (label.Contains("lea") || label.Contains("foliage")) ? materials[1] : materials[0];
                }
                renderer.sharedMaterials = assigned;
            }
            instance.transform.localScale *= height / Mathf.Max(0.01f, bounds.size.y);
            bounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers) bounds.Encapsulate(renderer.bounds);
            instance.transform.position += Vector3.up * (position.y - bounds.min.y);
        }
    }
}
#endif
