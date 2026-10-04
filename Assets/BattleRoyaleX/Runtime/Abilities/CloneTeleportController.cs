using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed class CloneTeleportController : MonoBehaviour
    {
        readonly List<GameObject> clones = new List<GameObject>();
        CharacterRuntime runtime;
        float opensAt;
        float expiresAt;

        public AbilityDefinition Ability { get; private set; }
        public bool CanTeleport => Ability != null && Time.time >= opensAt && Time.time <= expiresAt && clones.Count > 0;

        public void Begin(CharacterRuntime source, AbilityDefinition ability)
        {
            Cancel();
            runtime = source;
            Ability = ability;
            opensAt = Time.time + Mathf.Max(0f, ability.secondActivationDelay);
            expiresAt = opensAt + Mathf.Max(0.2f, ability.secondActivationWindow);
            float distance = Mathf.Max(2f, ability.movementDistance);
            for (int i = 0; i < 3; i++)
            {
                Vector3 direction = Quaternion.Euler(0f, i * 120f, 0f) * runtime.Motor.Facing;
                GameObject clone = CreateProjection(runtime);
                clone.name = $"ArcaneClone_{i + 1}";
                clone.transform.position = transform.position + direction * Mathf.Min(2.2f, distance * 0.5f);
                clone.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                Collider collider = clone.GetComponent<Collider>();
                if (collider != null) Destroy(collider);
                clone.AddComponent<OwnedAbilityEffect>().Configure(runtime, ability);
                clone.AddComponent<ArcaneCloneMotion>().Configure(runtime, ability, direction);
                clones.Add(clone);
            }
        }

        internal static GameObject CreateProjection(CharacterRuntime source)
        {
            SkinnedMeshRenderer[] skins = source.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skins.Length == 0) return GameObject.CreatePrimitive(PrimitiveType.Capsule);
            GameObject projection = new GameObject("ArcaneProjection");
            projection.transform.SetPositionAndRotation(source.transform.position, source.transform.rotation);
            ArcaneProjectionResources resources = projection.AddComponent<ArcaneProjectionResources>();
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
            Material material = new Material(shader);
            material.color = new Color(0.15f, 0.85f, 1f, 0.38f);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", material.color);
            material.SetFloat("_Surface", 1f); material.SetFloat("_ZWrite", 0f);
            material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            resources.material = material;
            foreach (SkinnedMeshRenderer skin in skins)
            {
                Mesh mesh = new Mesh(); skin.BakeMesh(mesh); resources.meshes.Add(mesh);
                GameObject part = new GameObject("Projected_" + skin.name, typeof(MeshFilter), typeof(MeshRenderer));
                part.transform.SetPositionAndRotation(skin.transform.position, skin.transform.rotation);
                part.transform.localScale = skin.transform.lossyScale;
                part.transform.SetParent(projection.transform, true);
                part.GetComponent<MeshFilter>().sharedMesh = mesh;
                Material[] materials = new Material[Mathf.Max(1, mesh.subMeshCount)];
                for (int m = 0; m < materials.Length; m++) materials[m] = material;
                part.GetComponent<MeshRenderer>().sharedMaterials = materials;
                part.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            // Outfit heads are static attachments, not part of the skinned clothing mesh.
            foreach (MeshFilter face in source.GetComponentsInChildren<MeshFilter>())
            {
                if (!face.name.StartsWith("Face_") || face.sharedMesh == null) continue;
                GameObject part = new GameObject("Projected_" + face.name, typeof(MeshFilter), typeof(MeshRenderer));
                part.transform.SetPositionAndRotation(face.transform.position, face.transform.rotation);
                part.transform.localScale = face.transform.lossyScale;
                part.transform.SetParent(projection.transform, true);
                part.GetComponent<MeshFilter>().sharedMesh = face.sharedMesh;
                Material[] materials = new Material[Mathf.Max(1, face.sharedMesh.subMeshCount)];
                for (int m = 0; m < materials.Length; m++) materials[m] = material;
                part.GetComponent<MeshRenderer>().sharedMaterials = materials;
                part.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            return projection;
        }

        public bool TryTeleport(Vector3 desiredDirection)
        {
            if (!CanTeleport || runtime == null || runtime.Health == null || runtime.Health.IsDead) return false;
            desiredDirection.y = 0f;
            if (desiredDirection.sqrMagnitude < 0.001f) desiredDirection = runtime.Motor.Facing;
            GameObject best = null;
            float bestDot = float.NegativeInfinity;
            foreach (GameObject clone in clones)
            {
                if (clone == null) continue;
                Vector3 direction = clone.transform.position - transform.position;
                direction.y = 0f;
                float dot = Vector3.Dot(desiredDirection.normalized, direction.normalized);
                if (dot <= bestDot) continue;
                bestDot = dot;
                best = clone;
            }
            if (best == null) return false;
            runtime.Motor.Teleport(best.transform.position);
            Cancel();
            return true;
        }

        void Update()
        {
            if (Ability != null && Time.time > expiresAt) Cancel();
        }

        public void Cancel()
        {
            foreach (GameObject clone in clones) if (clone != null) Destroy(clone);
            clones.Clear();
            Ability = null;
            opensAt = expiresAt = 0f;
        }
    }

    // A visual decoy that advances and fires once. The projectile uses the normal combat resolver.
    public sealed class ArcaneCloneMotion : MonoBehaviour
    {
        CharacterRuntime owner;
        AbilityDefinition ability;
        Vector3 direction;
        float bornAt;
        bool fired;

        public void Configure(CharacterRuntime source, AbilityDefinition definition, Vector3 forward)
        {
            owner = source; ability = definition; direction = forward.normalized; bornAt = Time.time;
        }

        void Update()
        {
            if (owner == null || owner.Health == null || owner.Health.IsDead) { Destroy(gameObject); return; }
            float age = Time.time - bornAt;
            if (age < 0.8f)
            {
                float step = 2.6f * Time.deltaTime;
                if (!Physics.Raycast(transform.position + Vector3.up, direction, step + 0.4f,
                    ~0, QueryTriggerInteraction.Ignore)) transform.position += direction * step;
            }
            if (!fired && age >= 0.28f)
            {
                fired = true;
                owner.Abilities.SpawnCloneProjectile(ability, transform.position, direction);
            }
        }
    }

    // Projection has only rendered pose snapshots: no animator, scripts, hurtbox or hitbox copied from the owner.
    public sealed class ArcaneProjectionResources : MonoBehaviour
    {
        public Material material;
        public readonly List<Mesh> meshes = new List<Mesh>();
        void OnDestroy()
        {
            if (material != null) Destroy(material);
            foreach (Mesh mesh in meshes) if (mesh != null) Destroy(mesh);
        }
    }
}
