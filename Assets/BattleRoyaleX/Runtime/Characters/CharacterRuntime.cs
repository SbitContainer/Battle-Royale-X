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
        public HealthRegenerationController HealthRegeneration { get; private set; }
        public RuntimeModifiers Modifiers { get; private set; } = RuntimeModifiers.Identity;

        Coroutine modifierRoutine;
        float speedBonusUntil;
        float speedBonusMultiplier = 1f;
        float slowUntil;
        float slowMultiplier = 1f;
        public float MovementSpeedBonus => Time.time < speedBonusUntil ? speedBonusMultiplier : 1f;
        public float MovementSlowMultiplier => Time.time < slowUntil ? slowMultiplier : 1f;
        public void ApplyMovementSpeedBonus(float multiplier, float duration)
        {
            speedBonusMultiplier = Mathf.Max(1f, multiplier);
            speedBonusUntil = Time.time + Mathf.Max(0f, duration);
        }

        public void ApplyMovementSlow(float multiplier, float duration)
        {
            float clamped = Mathf.Clamp(multiplier, 0.1f, 1f);
            if (Time.time >= slowUntil || clamped < slowMultiplier) slowMultiplier = clamped;
            slowUntil = Mathf.Max(slowUntil, Time.time + Mathf.Max(0f, duration));
        }

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
            HealthRegeneration = GetComponent<HealthRegenerationController>() ?? gameObject.AddComponent<HealthRegenerationController>();
            if (GetComponent<SmokeVisibility>() == null) gameObject.AddComponent<SmokeVisibility>();
        }

        void Start()
        {
            if (definition != null) Initialize(definition);
        }

        public void Initialize(CharacterDefinition source)
        {
            definition = source;
            HealthRegeneration.CancelRegeneration();
            HealthRegeneration.ResetNaturalRegenerationTimer();
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

        public void CancelTimedModifiers()
        {
            speedBonusUntil = 0f;
            slowUntil = 0f;
            slowMultiplier = 1f;
            if (modifierRoutine != null) StopCoroutine(modifierRoutine);
            modifierRoutine = null;
            Modifiers = RuntimeModifiers.Identity;
        }

        public void ResetTransientState()
        {
            if (Abilities != null) Abilities.ResetTransientState();
            if (Motor != null) Motor.StopMovementImmediately();
        }

        IEnumerator ModifierRoutine(RuntimeModifiers modifiers, float duration)
        {
            Modifiers = modifiers;
            yield return new WaitForSeconds(Mathf.Max(0.01f, duration));
            Modifiers = RuntimeModifiers.Identity;
            modifierRoutine = null;
        }
    }
}
