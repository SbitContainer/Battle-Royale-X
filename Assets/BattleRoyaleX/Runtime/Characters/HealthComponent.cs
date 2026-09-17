using System;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class HealthComponent : MonoBehaviour
    {
        public float MaxHealth { get; private set; } = 100f;
        public float CurrentHealth { get; private set; } = 100f;
        public bool IsDead => CurrentHealth <= 0f;
        public float LastDamageTime { get; private set; } = -999f;
        public event Action<float, float> Changed;
        public event Action Died;

        public void Initialize(float maxHealth)
        {
            MaxHealth = Mathf.Max(1f, maxHealth);
            CurrentHealth = MaxHealth;
            LastDamageTime = -999f;
            Changed?.Invoke(CurrentHealth, MaxHealth);
        }

        public void ApplyDamage(float amount)
        {
            if (IsDead || amount <= 0f) return;
            LastDamageTime = Time.time;
            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            Changed?.Invoke(CurrentHealth, MaxHealth);
            if (CurrentHealth <= 0f) Died?.Invoke();
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
            Changed?.Invoke(CurrentHealth, MaxHealth);
        }
    }
}
