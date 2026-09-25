using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class SlowField : MonoBehaviour
    {
        CharacterRuntime owner;
        float radius;
        float multiplier;
        float expiresAt;

        public void Configure(CharacterRuntime source, AbilityDefinition ability)
        {
            owner = source;
            radius = Mathf.Max(0.5f, ability.fieldRadius > 0f ? ability.fieldRadius : ability.explosionRadius);
            multiplier = 1f - Mathf.Clamp01(ability.slowPercent);
            expiresAt = Time.time + Mathf.Max(0.1f, ability.fieldDuration);
        }

        void Update()
        {
            if (Time.time >= expiresAt) { Destroy(gameObject); return; }
            foreach (Collider hit in Physics.OverlapSphere(transform.position, radius, ~0, QueryTriggerInteraction.Collide))
            {
                CharacterRuntime target = hit.GetComponentInParent<CharacterRuntime>();
                if (target == null || target == owner || target.Health == null || target.Health.IsDead ||
                    owner == null || target.TeamId == owner.TeamId) continue;
                target.ApplyMovementSlow(multiplier, 0.15f);
            }
        }
    }
}
