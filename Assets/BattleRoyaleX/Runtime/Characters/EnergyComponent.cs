using System;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class EnergyComponent : MonoBehaviour
    {
        public float MaxEnergy { get; private set; } = 100f;
        public float CurrentEnergy { get; private set; } = 100f;
        public float RegenPerSecond { get; private set; } = 8f;
        public event Action<float, float> Changed;

        public void Initialize(float maxEnergy, float regenPerSecond)
        {
            MaxEnergy = Mathf.Max(0f, maxEnergy);
            RegenPerSecond = Mathf.Max(0f, regenPerSecond);
            CurrentEnergy = MaxEnergy;
            Changed?.Invoke(CurrentEnergy, MaxEnergy);
        }

        private void Update()
        {
            if (CurrentEnergy >= MaxEnergy || RegenPerSecond <= 0f) return;
            CurrentEnergy = Mathf.Min(MaxEnergy, CurrentEnergy + RegenPerSecond * Time.deltaTime);
            Changed?.Invoke(CurrentEnergy, MaxEnergy);
        }

        public bool TrySpend(float amount)
        {
            amount = Mathf.Max(0f, amount);
            if (CurrentEnergy + 0.001f < amount) return false;
            CurrentEnergy -= amount;
            Changed?.Invoke(CurrentEnergy, MaxEnergy);
            return true;
        }

        public void Restore(float amount)
        {
            if (amount <= 0f) return;
            CurrentEnergy = Mathf.Min(MaxEnergy, CurrentEnergy + amount);
            Changed?.Invoke(CurrentEnergy, MaxEnergy);
        }
    }
}
