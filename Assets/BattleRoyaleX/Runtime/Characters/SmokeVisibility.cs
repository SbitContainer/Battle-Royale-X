using UnityEngine;

namespace BattleRoyaleX
{
    // Local training perspective only. Never deactivates gameplay, colliders, animations or incoming damage.
    public sealed class SmokeVisibility : MonoBehaviour
    {
        public static CharacterRuntime LocalPlayer { get; set; }
        CharacterRuntime actor;
        Renderer[] meshes;
        MaterialPropertyBlock tint;
        int mode = -1;
        bool[] originalEnabled;
        MaterialPropertyBlock[] originalBlocks;

        void Start()
        {
            actor = GetComponent<CharacterRuntime>();
            var all = GetComponentsInChildren<Renderer>();
            meshes = System.Array.FindAll(all, r => r is SkinnedMeshRenderer || r is MeshRenderer);
            tint = new MaterialPropertyBlock();
            originalEnabled = new bool[meshes.Length];
            originalBlocks = new MaterialPropertyBlock[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                originalEnabled[i] = meshes[i].enabled;
                originalBlocks[i] = new MaterialPropertyBlock(); meshes[i].GetPropertyBlock(originalBlocks[i]);
            }
        }

        void LateUpdate()
        {
            if (meshes == null) return;
            bool inside = SmokeField.Contains(transform.position);
            int next = !inside || LocalPlayer == null ? 0 : actor == LocalPlayer ? 1 : 2;
            if (next == mode) return;
            mode = next;
            for (int i = 0; i < meshes.Length; i++)
            {
                if (meshes[i] == null) continue;
                meshes[i].enabled = originalEnabled[i] && mode != 2;
                if (mode == 1)
                {
                    // Frosted/dimmed owner retains the original texture and readable silhouette.
                    meshes[i].GetPropertyBlock(tint);
                    tint.SetColor("_BaseColor", new Color(0.48f, 0.55f, 0.63f, 1f));
                    tint.SetFloat("_Smoothness", 0f);
                    meshes[i].SetPropertyBlock(tint);
                }
                else if (originalBlocks != null && i < originalBlocks.Length && originalBlocks[i] != null)
                    meshes[i].SetPropertyBlock(originalBlocks[i]);
            }
        }

        void OnDisable()
        {
            if (meshes == null || originalEnabled == null || originalBlocks == null) return;
            for (int i = 0; i < meshes.Length; i++) if (meshes[i] != null)
            {
                if (i < originalEnabled.Length) meshes[i].enabled = originalEnabled[i];
                if (i < originalBlocks.Length && originalBlocks[i] != null) meshes[i].SetPropertyBlock(originalBlocks[i]);
            }
            mode = -1;
        }
        void OnDestroy() { if (LocalPlayer == actor) LocalPlayer = null; }
    }
}
