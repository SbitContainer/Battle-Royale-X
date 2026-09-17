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
        readonly Dictionary<DefenseKind, float> specialReadyAt = new Dictionary<DefenseKind, float>();

        void Awake() => runtime = GetComponent<CharacterRuntime>();

        public void Activate(DefenseKind kind, float duration, float perfectWindow, float reduction, float specialInteractionCooldown)
        {
            activeKind = kind;
            float multiplier = runtime != null ? runtime.Modifiers.defenseWindowMultiplier : 1f;
            activeUntil = Time.time + duration * multiplier;
            perfectUntil = Time.time + perfectWindow * multiplier;
            damageReduction = Mathf.Clamp01(reduction);
            specialCooldown = Mathf.Max(0f, specialInteractionCooldown);
        }

        public CombatOutcome ResolveIncoming(DamagePacket packet, out float damageAfterDefense)
        {
            damageAfterDefense = packet.damage;
            if (runtime != null && runtime.State.IsInvulnerable)
            {
                damageAfterDefense = 0f;
                return CombatOutcome.Dodged;
            }

            if (Time.time > activeUntil || activeKind == DefenseKind.None)
                return CombatOutcome.Hit;

            switch (activeKind)
            {
                case DefenseKind.Guard:
                    if (!packet.blockable) return CombatOutcome.Hit;
                    damageAfterDefense = packet.damage * (1f - damageReduction);
                    return CombatOutcome.Blocked;

                case DefenseKind.Parry:
                    if (!packet.parryable) return CombatOutcome.Hit;
                    if (Time.time <= perfectUntil && IsSpecialReady(DefenseKind.Parry))
                    {
                        ConsumeSpecial(DefenseKind.Parry);
                        damageAfterDefense = 0f;
                        return CombatOutcome.Parried;
                    }
                    damageAfterDefense = packet.damage * 0.5f;
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
