using UnityEngine;

namespace BattleRoyaleX
{
    public static class TacticalEffectSpawner
    {
        public static void Use(CharacterRuntime user, ItemDefinition item)
        {
            if (user == null || item == null) return;
            Vector3 center = user.transform.position + user.Motor.Facing * 2f;

            switch (item.tacticalKind)
            {
                case TacticalKind.Smoke:
                    SpawnTimedPrimitive("Smoke_Field", PrimitiveType.Cylinder, center, new Vector3(item.tacticalRadius * 2f, 0.25f, item.tacticalRadius * 2f), item.tacticalDuration, false);
                    break;
                case TacticalKind.Repulsion:
                    Repulse(user, center, item.tacticalRadius, item.tacticalForce);
                    SpawnTimedPrimitive("Repulsion_Pulse", PrimitiveType.Sphere, center, Vector3.one * item.tacticalRadius, 0.25f, false);
                    break;
                case TacticalKind.Barrier:
                    GameObject barrier = SpawnTimedPrimitive("Barrier", PrimitiveType.Cube, center, new Vector3(3f, 2f, 0.35f), item.tacticalDuration, true);
                    barrier.transform.rotation = Quaternion.LookRotation(user.Motor.Facing, Vector3.up);
                    break;
                case TacticalKind.NullField:
                    GameObject field = SpawnTimedPrimitive("Null_Field", PrimitiveType.Cylinder, center, new Vector3(item.tacticalRadius * 2f, 0.15f, item.tacticalRadius * 2f), item.tacticalDuration, false);
                    field.AddComponent<NullField>();
                    break;
            }
        }

        static void Repulse(CharacterRuntime user, Vector3 center, float radius, float force)
        {
            Collider[] hits = Physics.OverlapSphere(center, radius);
            foreach (Collider hit in hits)
            {
                CharacterRuntime target = hit.GetComponentInParent<CharacterRuntime>();
                if (target == null || target == user) continue;
                Vector3 dir = target.transform.position - center;
                dir.y = 0f;
                target.Motor.ApplyImpulse(dir.normalized, force * 0.35f);
            }
        }

        static GameObject SpawnTimedPrimitive(string name, PrimitiveType type, Vector3 position, Vector3 scale, float duration, bool keepCollider)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
            Collider c = go.GetComponent<Collider>();
            if (c != null) c.enabled = keepCollider;
            Object.Destroy(go, Mathf.Max(0.05f, duration));
            return go;
        }
    }
}
