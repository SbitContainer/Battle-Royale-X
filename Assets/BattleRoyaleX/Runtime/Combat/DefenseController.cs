using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class DefenseController : MonoBehaviour
    {
        CharacterRuntime runtime;
        DefenseKind activeKind = DefenseKind.None;
        float activeUntil;
        float perfectUntil;
        float damageReduction;
        float specialCooldown;
        bool perfectConsumed;
        public AbilityDefinition ActiveAbility { get; private set; }
        public bool IsActive => activeKind != DefenseKind.None && Time.time < activeUntil;
        public float Remaining => IsActive ? Mathf.Max(0f, activeUntil - Time.time) : 0f;
        readonly Dictionary<DefenseKind, float> specialReadyAt = new Dictionary<DefenseKind, float>();

        void Awake() => runtime = GetComponent<CharacterRuntime>();

        public void Activate(DefenseKind kind, float duration, float perfectWindow, float reduction, float specialInteractionCooldown,
            AbilityDefinition ability = null)
        {
            activeKind = kind;
            ActiveAbility = ability;
            float multiplier = runtime != null ? runtime.Modifiers.defenseWindowMultiplier : 1f;
            activeUntil = Time.time + duration * multiplier;
            perfectUntil = Time.time + perfectWindow * multiplier;
            damageReduction = Mathf.Clamp01(reduction);
            specialCooldown = Mathf.Max(0f, specialInteractionCooldown);
            perfectConsumed = false;
        }

        public void Deactivate()
        {
            activeKind = DefenseKind.None;
            ActiveAbility = null;
            activeUntil = perfectUntil = 0f;
            perfectConsumed = false;
        }

        public void ResetTransientState()
        {
            Deactivate();
            specialReadyAt.Clear();
        }

        public CombatOutcome ResolveIncoming(DamagePacket packet, out float damageAfterDefense)
        {
            damageAfterDefense = packet.damage;
            if (runtime.Abilities.TryWarriorProtection(packet, out damageAfterDefense, out CombatOutcome warriorOutcome)) return warriorOutcome;
            if (runtime.Abilities.TryRepelWithDaggers(packet))
            {
                damageAfterDefense = 0f;
                return CombatOutcome.Dodged;
            }
            if (packet.source != null && runtime.Abilities.TryDefensiveRedirect(packet.source, false))
            {
                damageAfterDefense = 0f;
                return CombatOutcome.Dodged;
            }
            if (runtime != null && runtime.State.IsInvulnerable)
            {
                damageAfterDefense = 0f;
                return CombatOutcome.Dodged;
            }

            if (Time.time > activeUntil || activeKind == DefenseKind.None)
                return CombatOutcome.Hit;

            if (ActiveAbility != null && ActiveAbility.behavior == AbilityBehavior.WarriorSkillCapture)
            {
                if (!runtime.Abilities.TryCaptureSkill(packet)) return CombatOutcome.Hit;
                damageAfterDefense = 0f;
                Deactivate();
                return CombatOutcome.Nullified;
            }

            if (ActiveAbility != null && ActiveAbility.defenseInteractionRule != null &&
                !ActiveAbility.defenseInteractionRule.Allows(packet.ability)) return CombatOutcome.Hit;
            switch (activeKind)
            {
                case DefenseKind.Guard:
                    if (!packet.blockable) return CombatOutcome.Hit;
                    damageAfterDefense = packet.damage * (1f - damageReduction);
                    return CombatOutcome.Blocked;

                case DefenseKind.Parry:
                    if (!packet.parryable) return CombatOutcome.Hit;
                    if (!perfectConsumed && Time.time <= perfectUntil && IsSpecialReady(DefenseKind.Parry))
                    {
                        perfectConsumed = true;
                        ConsumeSpecial(DefenseKind.Parry);
                        damageAfterDefense = 0f;
                        return CombatOutcome.Parried;
                    }
                    damageAfterDefense = packet.damage * (1f - damageReduction);
                    return CombatOutcome.Blocked;

                case DefenseKind.Nullify:
                    if (!packet.nullifiable || !IsSpecialReady(DefenseKind.Nullify)) return CombatOutcome.Hit;
                    ConsumeSpecial(DefenseKind.Nullify);
                    damageAfterDefense = 0f;
                    return CombatOutcome.Nullified;

                case DefenseKind.Reflect:
                    if (!packet.reflectable || !IsSpecialReady(DefenseKind.Reflect)) return CombatOutcome.Hit;
                    ConsumeSpecial(DefenseKind.Reflect);
                    damageAfterDefense = 0f;
                    return CombatOutcome.Reflected;
            }

            return CombatOutcome.Hit;
        }

        bool IsSpecialReady(DefenseKind kind) => !specialReadyAt.TryGetValue(kind, out float readyAt) || Time.time >= readyAt;
        void ConsumeSpecial(DefenseKind kind) => specialReadyAt[kind] = Time.time + specialCooldown;
    }
}
