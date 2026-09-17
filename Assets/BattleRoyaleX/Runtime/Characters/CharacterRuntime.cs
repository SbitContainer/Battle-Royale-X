using System.Collections;
using UnityEngine;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed class CharacterRuntime : MonoBehaviour
    {
        public CharacterDefinition definition;
        public TeamId teamId = TeamId.PlayerOne;

        public CharacterDefinition Definition => definition;
        public TeamId TeamId => teamId;
        public HealthComponent Health { get; private set; }
        public EnergyComponent Energy { get; private set; }
        public CharacterStateController State { get; private set; }
        public CharacterMotor25D Motor { get; private set; }
        public DefenseController Defense { get; private set; }
        public AbilityController Abilities { get; private set; }
        public InteractionCooldownController Interactions { get; private set; }
        public InventoryController Inventory { get; private set; }
        public RuntimeModifiers Modifiers { get; private set; } = RuntimeModifiers.Identity;

        Coroutine modifierRoutine;

        void Awake()
        {
            Health = GetComponent<HealthComponent>() ?? gameObject.AddComponent<HealthComponent>();
            Energy = GetComponent<EnergyComponent>() ?? gameObject.AddComponent<EnergyComponent>();
            State = GetComponent<CharacterStateController>() ?? gameObject.AddComponent<CharacterStateController>();
            Motor = GetComponent<CharacterMotor25D>() ?? gameObject.AddComponent<CharacterMotor25D>();
            Defense = GetComponent<DefenseController>() ?? gameObject.AddComponent<DefenseController>();
            Abilities = GetComponent<AbilityController>() ?? gameObject.AddComponent<AbilityController>();
            Interactions = GetComponent<InteractionCooldownController>() ?? gameObject.AddComponent<InteractionCooldownController>();
            Inventory = GetComponent<InventoryController>() ?? gameObject.AddComponent<InventoryController>();
        }

        void Start()
        {
            if (definition != null) Initialize(definition);
        }

        public void Initialize(CharacterDefinition source)
        {
            definition = source;
            Health.Initialize(source.maxHealth);
            Energy.Initialize(source.maxEnergy, source.energyRegenPerSecond);
            Modifiers = RuntimeModifiers.Identity;
            Abilities.Initialize(source);
            Inventory.Initialize(3);
        }

        public void ApplyTimedModifiers(RuntimeModifiers modifiers, float duration)
        {
            if (modifierRoutine != null) StopCoroutine(modifierRoutine);
            modifierRoutine = StartCoroutine(ModifierRoutine(modifiers, duration));
        }

        IEnumerator ModifierRoutine(RuntimeModifiers modifiers, float duration)
        {
            Modifiers = modifiers;
            yield return new WaitForSeconds(Mathf.Max(0.01f, duration));
            Modifiers = RuntimeModifiers.Identity;
        }
    }
}
