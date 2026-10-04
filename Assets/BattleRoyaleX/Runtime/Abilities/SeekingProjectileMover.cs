using UnityEngine;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed class SeekingProjectileMover : MonoBehaviour
    {
        CharacterRuntime owner;
        CharacterRuntime target;
        float speed;
        float turnRate;
        float expiresAt;
        float maxAngle = 180f;
        float maxTargetRange = float.MaxValue;
        Vector3 initialDirection;
        public float MaximumAngle => maxAngle;
        public Vector3 InitialDirection => initialDirection;

        public void Configure(CharacterRuntime source, float projectileSpeed, float degreesPerSecond, float lifetime,
            float maximumAngle = 180f, float targetRange = float.MaxValue)
        {
            owner = source;
            speed = Mathf.Max(0.1f, projectileSpeed);
            turnRate = Mathf.Max(0f, degreesPerSecond);
            maxAngle = Mathf.Clamp(maximumAngle, 0f, 180f);
            maxTargetRange = Mathf.Max(0.1f, targetRange);
            initialDirection = transform.forward;
            expiresAt = Time.time + Mathf.Max(0.1f, lifetime);
            ProjectileHitboxMover legacy = GetComponent<ProjectileHitboxMover>();
            if (legacy != null) legacy.enabled = false;
            target = FindTarget();
        }

        void Update()
        {
            if (Time.time >= expiresAt) { Destroy(gameObject); return; }
            if (target == null || target.Health == null || target.Health.IsDead || target.Abilities.IsExecutionHidden) target = FindTarget();
            Vector3 forward = transform.forward;
            if (target != null)
            {
                Vector3 desired = target.transform.position + Vector3.up * 0.8f - transform.position;
                desired.y = 0f;
                if (desired.sqrMagnitude > 0.001f)
                {
                    Vector3 curved = Vector3.RotateTowards(forward, desired.normalized,
                        turnRate * Mathf.Deg2Rad * Time.deltaTime, 0f).normalized;
                    forward = Vector3.RotateTowards(initialDirection, curved, maxAngle * Mathf.Deg2Rad, 0f).normalized;
                }
            }
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            Vector3 previous = transform.position;
            transform.position += forward * speed * Time.deltaTime;
            ProjectileHitboxMover.ResolveTravel(GetComponent<Hitbox>(), previous, transform.position);
        }

        CharacterRuntime FindTarget()
        {
            CharacterRuntime best = null;
            float bestDistance = float.MaxValue;
            foreach (CharacterRuntime candidate in FindObjectsByType<CharacterRuntime>())
            {
                if (candidate == null || candidate == owner || candidate.Health == null || candidate.Health.IsDead ||
                    owner == null || candidate.TeamId == owner.TeamId || candidate.TeamId == TeamId.Neutral || candidate.Abilities.IsExecutionHidden) continue;
                float distance = (candidate.transform.position - transform.position).sqrMagnitude;
                Vector3 direction = candidate.transform.position - transform.position; direction.y = 0f;
                if (distance >= bestDistance || distance > maxTargetRange * maxTargetRange) continue;
                bestDistance = distance;
                best = candidate;
            }
            return best;
        }
    }
}
