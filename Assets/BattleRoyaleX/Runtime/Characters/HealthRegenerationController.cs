using System.Collections;
using UnityEngine;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed class HealthRegenerationController : MonoBehaviour
    {
        HealthComponent health;
        CharacterRuntime runtime;
        Coroutine routine;

        [Header("Natural regeneration")]
        [Min(0f)] public float outOfCombatDelay = 8f;
        [Min(0f)] public float naturalPercentPerSecond = 0.025f;
        public float LastCombatTime { get; private set; } = -999f;

        public bool IsRegenerating => routine != null;
        public float RemainingAmount { get; private set; }
        public float RemainingDuration { get; private set; }

        void Awake()
        {
            health = GetComponent<HealthComponent>();
            runtime = GetComponent<CharacterRuntime>();
        }

        void OnEnable() => CombatEvents.Raised += OnCombatEvent;
        void OnDisable() => CombatEvents.Raised -= OnCombatEvent;

        void Update()
        {
            if (routine != null || health == null || health.IsDead || health.CurrentHealth >= health.MaxHealth) return;
            if (Time.time - Mathf.Max(LastCombatTime, health.LastDamageTime) < outOfCombatDelay) return;
            health.Heal(health.MaxHealth * naturalPercentPerSecond * Time.deltaTime);
        }

        void OnCombatEvent(CombatEventData data)
        {
            if (runtime == null) return;
            bool damaging = data.kind == CombatEventKind.Hit ||
                data.kind == CombatEventKind.Block && data.value > 0f || data.kind == CombatEventKind.Clash;
            if (damaging && (data.source == runtime || data.target == runtime)) LastCombatTime = Time.time;
        }

        public void ResetNaturalRegenerationTimer() => LastCombatTime = Time.time;

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
