using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class InventoryController : MonoBehaviour
    {
        CharacterRuntime runtime;
        readonly List<ItemDefinition> slots = new List<ItemDefinition>();
        bool usingItem;

        public int Capacity { get; private set; } = 3;
        public IReadOnlyList<ItemDefinition> Slots => slots;
        public bool IsUsingItem => usingItem;
        public event Action Changed;

        void Awake() => runtime = GetComponent<CharacterRuntime>();

        public void Initialize(int capacity)
        {
            Capacity = Mathf.Max(1, capacity);
            slots.Clear();
            usingItem = false;
            Changed?.Invoke();
        }

        public bool TryAdd(ItemDefinition item)
        {
            if (item == null || slots.Count >= Capacity) return false;
            if (item.kind == ItemKind.Variation && item.variationAbility != null && item.variationAbility.classRestricted && runtime.Definition != null && runtime.Definition.characterClass != item.variationAbility.requiredClass) return false;
            slots.Add(item);
            Changed?.Invoke();
            return true;
        }

        public bool UseSlot(int index)
        {
            if (usingItem || index < 0 || index >= slots.Count || runtime.Health.IsDead) return false;
            ItemDefinition item = slots[index];
            StartCoroutine(UseRoutine(item));
            return true;
        }

        IEnumerator UseRoutine(ItemDefinition item)
        {
            usingItem = true;
            float startedAt = Time.time;
            float damageAtStart = runtime.Health.LastDamageTime;
            float duration = Mathf.Max(0f, item.useDuration);

            if (duration > 0f)
            {
                runtime.State.LockInput(true);
                while (Time.time - startedAt < duration)
                {
                    if (item.interruptible && runtime.Health.LastDamageTime > damageAtStart)
                    {
                        runtime.State.LockInput(false);
                        usingItem = false;
                        yield break;
                    }
                    yield return null;
                }
                runtime.State.LockInput(false);
            }

            bool succeeded = Apply(item);
            if (succeeded) slots.Remove(item);
            usingItem = false;
            Changed?.Invoke();
        }

        public void DropSlot(int index)
        {
            if (usingItem || index < 0 || index >= slots.Count) return;
            slots.RemoveAt(index);
            Changed?.Invoke();
        }

        bool Apply(ItemDefinition item)
        {
            switch (item.kind)
            {
                case ItemKind.Heal:
                    runtime.Health.Heal(item.amount);
                    CombatEvents.Raise(new CombatEventData(CombatEventKind.Heal, transform.position, runtime, runtime, item.amount));
                    return true;
                case ItemKind.Energy:
                    runtime.Energy.Restore(item.amount);
                    CombatEvents.Raise(new CombatEventData(CombatEventKind.Energy, transform.position, runtime, runtime, item.amount));
                    return true;
                case ItemKind.CooldownRefresh:
                    runtime.Abilities.ReduceAllCooldowns(item.cooldownReductionSeconds);
                    return true;
                case ItemKind.Variation:
                    return runtime.Abilities.EquipVariation(item.variationAbility);
                case ItemKind.BackpackUpgrade:
                    Capacity = Mathf.Max(Capacity, item.backpackCapacity);
                    return true;
                case ItemKind.Tactical:
                    TacticalEffectSpawner.Use(runtime, item);
                    CombatEvents.Raise(new CombatEventData(CombatEventKind.TacticalUsed, transform.position, runtime, runtime));
                    return true;
            }
            return false;
        }
    }
}
