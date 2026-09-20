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
        ItemDefinition activeItem;
        float activeUseStartedAt;
        float activeUseDuration;

        public int Capacity { get; private set; } = 3;
        public IReadOnlyList<ItemDefinition> Slots => slots;
        public bool IsUsingItem => usingItem;
        public ItemDefinition ActiveItem => activeItem;
        public float ActiveUseProgress => !usingItem || activeUseDuration <= 0f
            ? 0f
            : Mathf.Clamp01((Time.time - activeUseStartedAt) / activeUseDuration);
        public event Action Changed;

        void Awake() => runtime = GetComponent<CharacterRuntime>();

        public void Initialize(int capacity)
        {
            StopAllCoroutines();
            if (usingItem) runtime.State.LockInput(false);
            Capacity = Mathf.Max(1, capacity);
            slots.Clear();
            usingItem = false;
            activeItem = null;
            activeUseStartedAt = 0f;
            activeUseDuration = 0f;
            Changed?.Invoke();
        }

        public bool CanAdd(ItemDefinition item)
        {
            if (item == null || slots.Count >= Capacity) return false;
            return item.kind != ItemKind.Variation || item.variationAbility == null || !item.variationAbility.classRestricted ||
                runtime.Definition == null || runtime.Definition.characterClass == item.variationAbility.requiredClass;
        }

        public bool TryAdd(ItemDefinition item)
        {
            if (!CanAdd(item)) return false;
            slots.Add(item);
            Changed?.Invoke();
            return true;
        }

        public bool UseSlot(int index)
        {
            if (usingItem || index < 0 || index >= slots.Count || runtime.Health.IsDead) return false;
            ItemDefinition item = slots[index];
            if (item.kind == ItemKind.Heal)
            {
                if (!runtime.HealthRegeneration.StartRegeneration(item.amount, item.healDuration)) return false;
                slots.RemoveAt(index);
                CombatEvents.Raise(new CombatEventData(CombatEventKind.Heal, transform.position, runtime, runtime, item.amount));
                Changed?.Invoke();
                return true;
            }
            StartCoroutine(UseRoutine(item));
            return true;
        }

        IEnumerator UseRoutine(ItemDefinition item)
        {
            usingItem = true;
            activeItem = item;
            activeUseStartedAt = Time.time;
            activeUseDuration = Mathf.Max(0f, item.useDuration);
            float damageAtStart = runtime.Health.LastDamageTime;

            if (activeUseDuration > 0f)
            {
                runtime.State.LockInput(true);
                while (Time.time - activeUseStartedAt < activeUseDuration)
                {
                    if (item.interruptible && runtime.Health.LastDamageTime > damageAtStart)
                    {
                        runtime.State.LockInput(false);
                        ClearActiveUse();
                        Changed?.Invoke();
                        yield break;
                    }
                    yield return null;
                }
                runtime.State.LockInput(false);
            }

            bool succeeded = Apply(item);
            if (succeeded) slots.Remove(item);
            ClearActiveUse();
            Changed?.Invoke();
        }

        void ClearActiveUse()
        {
            usingItem = false;
            activeItem = null;
            activeUseStartedAt = 0f;
            activeUseDuration = 0f;
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
                    return false;
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
                    CombatEvents.Raise(new CombatEventData(CombatEventKind.TacticalUsed, transform.position, runtime, runtime, item: item));
                    return true;
            }
            return false;
        }
    }
}
