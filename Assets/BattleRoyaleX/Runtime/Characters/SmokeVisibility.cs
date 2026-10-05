using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BattleRoyaleX
{
    // Perspective only: never changes colliders, targeting or damage immunity.
    public sealed class SmokeVisibility : MonoBehaviour
    {
        public static CharacterRuntime LocalPlayer { get; set; }
        CharacterRuntime actor;
        CharacterDefinition definition;
        int mode;
        readonly List<Snapshot> snapshots = new List<Snapshot>();
        sealed class Snapshot
        {
            public Renderer renderer;
            public bool enabled;
            public ShadowCastingMode shadows;
            public Material[] materials, ghosts;
            public MaterialPropertyBlock block;
        }
        void Start() { actor = GetComponent<CharacterRuntime>(); }
        void LateUpdate()
        {
            if (actor == null) return;
            if (definition != actor.Definition) { Restore(); definition = actor.Definition; }
            bool concealed = !actor.Health.IsDead &&
                (SmokeField.Contains(transform.position) || actor.Abilities.IsExecutionHidden);
            int next = !concealed || LocalPlayer == null ? 0 : actor == LocalPlayer ? 1 : 2;
            if (next == mode) return;
            Restore(); mode = next;
            if (mode == 0) return;
            // Active model/weapon geometry only; not transient owned effects.
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                if (!(renderer is MeshRenderer) && !(renderer is SkinnedMeshRenderer)) continue;
                if (renderer.GetComponentInParent<OwnedAbilityEffect>() != null) continue;
                var state = new Snapshot { renderer = renderer, enabled = renderer.enabled,
                    shadows = renderer.shadowCastingMode, materials = renderer.sharedMaterials,
                    block = new MaterialPropertyBlock() };
                renderer.GetPropertyBlock(state.block); snapshots.Add(state);
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                if (mode == 2) { renderer.enabled = false; continue; }
                state.ghosts = new Material[state.materials.Length];
                for (int i = 0; i < state.materials.Length; i++)
                {
                    Material original = state.materials[i];
                    if (original == null) continue;
                    var ghost = new Material(original) { name = original.name + "_LocalStealth" };
                    ghost.SetFloat("_Surface", 1f);
                    ghost.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                    ghost.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                    ghost.SetFloat("_ZWrite", 0f);
                    ghost.SetFloat("_AlphaClip", 0f);
                    ghost.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    ghost.DisableKeyword("_ALPHATEST_ON");
                    ghost.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    ghost.SetOverrideTag("RenderType", "Transparent");
                    ghost.renderQueue = (int)RenderQueue.Transparent;
                    ghost.SetShaderPassEnabled("ShadowCaster", false);
                    state.ghosts[i] = ghost;
                }
                renderer.sharedMaterials = state.ghosts;
                var tint = new MaterialPropertyBlock(); renderer.GetPropertyBlock(tint);
                tint.SetColor("_BaseColor", new Color(.48f, .38f, .68f, .34f));
                tint.SetFloat("_Smoothness", 0f); renderer.SetPropertyBlock(tint);
            }
        }
        void Restore()
        {
            foreach (Snapshot state in snapshots)
            {
                if (state.renderer != null)
                {
                    state.renderer.sharedMaterials = state.materials;
                    state.renderer.SetPropertyBlock(state.block);
                    state.renderer.enabled = state.enabled;
                    state.renderer.shadowCastingMode = state.shadows;
                }
                if (state.ghosts != null) foreach (Material ghost in state.ghosts)
                    if (ghost != null) Destroy(ghost);
            }
            snapshots.Clear(); mode = 0;
        }
        void OnDisable() => Restore();
        void OnDestroy() { Restore(); if (LocalPlayer == actor) LocalPlayer = null; }
    }
}
