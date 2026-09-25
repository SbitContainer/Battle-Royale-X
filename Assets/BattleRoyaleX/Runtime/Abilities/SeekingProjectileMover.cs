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

        public void Configure(CharacterRuntime source, float projectileSpeed, float degreesPerSecond, float lifetime)
        {
            owner = source;
            speed = Mathf.Max(0.1f, projectileSpeed);
            turnRate = Mathf.Max(0f, degreesPerSecond);
            expiresAt = Time.time + Mathf.Max(0.1f, lifetime);
            target = FindTarget();
        }

        void Update()
        {
            if (Time.time >= expiresAt) { Destroy(gameObject); return; }
            if (target == null || target.Health == null || target.Health.IsDead) target = FindTarget();
            Vector3 forward = transform.forward;
            if (target != null)
            {
                Vector3 desired = target.transform.position + Vector3.up * 0.8f - transform.position;
                desired.y = 0f;
                if (desired.sqrMagnitude > 0.001f)
                    forward = Vector3.RotateTowards(forward, desired.normalized,
                        turnRate * Mathf.Deg2Rad * Time.deltaTime, 0f).normalized;
            }
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            transform.position += forward * speed * Time.deltaTime;
            ProjectileHitboxMover legacy = GetComponent<ProjectileHitboxMover>();
            if (legacy != null) legacy.enabled = false;
        }

        CharacterRuntime FindTarget()
        {
            CharacterRuntime best = null;
            float bestDistance = float.MaxValue;
            foreach (CharacterRuntime candidate in FindObjectsByType<CharacterRuntime>())
            {
                if (candidate == null || candidate == owner || candidate.Health == null || candidate.Health.IsDead ||
                    owner == null || candidate.TeamId == owner.TeamId || candidate.TeamId == TeamId.Neutral) continue;
                float distance = (candidate.transform.position - transform.position).sqrMagnitude;
                if (distance >= bestDistance) continue;
                bestDistance = distance;
                best = candidate;
            }
            return best;
        }
    }
}
