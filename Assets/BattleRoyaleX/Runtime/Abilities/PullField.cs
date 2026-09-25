using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class PullField : MonoBehaviour
    {
        CharacterRuntime owner;
        float radius;
        float strength;
        float expiresAt;
        float nextPulse;
        readonly HashSet<CharacterRuntime> seen = new HashSet<CharacterRuntime>();

        public void Configure(CharacterRuntime source, AbilityDefinition ability)
        {
            owner = source;
            radius = Mathf.Max(0.5f, ability.fieldRadius > 0f ? ability.fieldRadius : ability.explosionRadius);
            strength = Mathf.Max(0f, ability.pullStrength);
            expiresAt = Time.time + Mathf.Max(0.1f, ability.fieldDuration);
        }

        void Update()
        {
            if (Time.time >= expiresAt) { Destroy(gameObject); return; }
            if (Time.time < nextPulse) return;
            nextPulse = Time.time + 0.10f;
            seen.Clear();
            foreach (Collider hit in Physics.OverlapSphere(transform.position, radius, ~0, QueryTriggerInteraction.Collide))
            {
                CharacterRuntime target = hit.GetComponentInParent<CharacterRuntime>();
                if (target == null || target == owner || seen.Contains(target) || target.Health == null || target.Health.IsDead ||
                    owner == null || target.TeamId == owner.TeamId) continue;
                seen.Add(target);
                Vector3 toward = transform.position - target.transform.position;
                toward.y = 0f;
                float distance = toward.magnitude;
                if (distance > 0.1f)
                    target.Motor.ApplyImpulse(toward.normalized, Mathf.Min(distance, strength * 0.10f), 0.10f);
            }
        }
    }
}
