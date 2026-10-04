using UnityEngine;

namespace BattleRoyaleX
{
    // Visual blades have no colliders; combat pulses are resolved separately.
    public sealed class AssassinDaggersPresentation : MonoBehaviour
    {
        Transform[] blades;
        float radius;
        Material steel;
        Mesh bladeMesh;
        public void Configure(int count, float orbitRadius)
        {
            radius = orbitRadius; blades = new Transform[count];
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            steel = new Material(shader); steel.color = new Color(0.55f, 0.8f, 0.95f);
            steel.SetFloat("_Metallic", 0.85f); steel.SetFloat("_Smoothness", 0.85f);
            bladeMesh = new Mesh { name = "TaperedDaggerBlade" };
            bladeMesh.vertices = new[] { new Vector3(0,0,-0.32f), new Vector3(-0.10f,0,0.08f),
                new Vector3(0,0,0.45f), new Vector3(0.10f,0,0.08f), new Vector3(0,0.045f,0.04f), new Vector3(0,-0.045f,0.04f) };
            bladeMesh.triangles = new[] {0,1,4,1,2,4,2,3,4,3,0,4,1,0,5,2,1,5,3,2,5,0,3,5};
            bladeMesh.RecalculateNormals(); bladeMesh.RecalculateBounds();
            for (int i = 0; i < count; i++)
            {
                GameObject blade = new GameObject();
                blade.name = "Dagger_" + i; blade.transform.SetParent(transform, false);
                blade.AddComponent<MeshFilter>().sharedMesh = bladeMesh;
                blade.AddComponent<MeshRenderer>().sharedMaterial = steel;
                blades[i] = blade.transform;
                var trail = blade.AddComponent<TrailRenderer>();
                trail.sharedMaterial = steel; trail.time = 0.12f; trail.startWidth = 0.06f; trail.endWidth = 0f;
                trail.minVertexDistance = 0.05f;
            }
        }
        void LateUpdate()
        {
            if (blades == null) return;
            var marker = GetComponent<OwnedAbilityEffect>();
            bool show = marker == null || marker.Owner == null || !marker.Owner.Abilities.IsExecutionHidden ||
                SmokeVisibility.LocalPlayer == null || SmokeVisibility.LocalPlayer == marker.Owner;
            foreach (var blade in blades) foreach (var renderer in blade.GetComponents<Renderer>()) renderer.enabled = show;
            if (radius <= 0f) return;
            for (int i = 0; i < blades.Length; i++)
            {
                float angle = Time.time * 9f + i * Mathf.PI * 2f / blades.Length;
                blades[i].localPosition = new Vector3(Mathf.Cos(angle) * radius, 1f, Mathf.Sin(angle) * radius);
                blades[i].localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f);
            }
        }
        void OnDestroy() { if (steel != null) Destroy(steel); if (bladeMesh != null) Destroy(bladeMesh); }
    }
}
