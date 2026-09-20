using System.Collections;
using UnityEngine;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed class HealthRegenerationController : MonoBehaviour
    {
        HealthComponent health;
        Coroutine routine;

        public bool IsRegenerating => routine != null;
        public float RemainingAmount { get; private set; }
        public float RemainingDuration { get; private set; }

        void Awake() => health = GetComponent<HealthComponent>();

        public bool StartRegeneration(float amount, float duration)
        {
            if (health == null || health.IsDead || health.CurrentHealth >= health.MaxHealth || amount <= 0f || duration <= 0f)
                return false;
            CancelRegeneration();
            RemainingAmount = amount;
            RemainingDuration = duration;
            routine = StartCoroutine(Regenerate(amount, duration));
            return true;
        }

        public void CancelRegeneration()
        {
            if (routine != null) StopCoroutine(routine);
            routine = null;
            RemainingAmount = RemainingDuration = 0f;
        }

        IEnumerator Regenerate(float total, float duration)
        {
            float elapsed = 0f;
            float delivered = 0f;
            while (elapsed < duration && health != null && !health.IsDead)
            {
                float dt = Mathf.Min(Time.deltaTime, duration - elapsed);
                elapsed += dt;
                float targetDelivered = total * (elapsed / duration);
                float tick = targetDelivered - delivered;
                delivered = targetDelivered;
                if (tick > 0f) health.Heal(tick);
                RemainingAmount = Mathf.Max(0f, total - delivered);
                RemainingDuration = Mathf.Max(0f, duration - elapsed);
                yield return null;
            }
            routine = null;
            RemainingAmount = RemainingDuration = 0f;
        }
    }
}
