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
                GameObject clone = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                clone.name = $"ArcaneClone_{i + 1}";
                clone.transform.position = transform.position + direction * distance;
                clone.transform.localScale = new Vector3(0.7f, 0.95f, 0.7f);
                Collider collider = clone.GetComponent<Collider>();
                if (collider != null) Destroy(collider);
                clone.AddComponent<OwnedAbilityEffect>().Configure(runtime, ability);
                clones.Add(clone);
            }
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
}
