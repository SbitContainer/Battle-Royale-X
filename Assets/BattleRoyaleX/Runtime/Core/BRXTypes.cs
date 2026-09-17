using System;
using UnityEngine;

namespace BattleRoyaleX
{
    public enum CharacterClass { Warrior, Assassin, Mage, Marksman }
    public enum TeamId { Neutral = 0, PlayerOne = 1, PlayerTwo = 2 }
    public enum AbilitySlot { BasicAttack, Defense, Movement, Ultimate }
    public enum AbilityBehavior
    {
        MeleeAttack,
        ProjectileAttack,
        AreaAttack,
        Guard,
        Parry,
        Dodge,
        Dash,
        DashThrough,
        DashReturn,
        UltimateBuff
    }

    public enum AttackKind { Physical, Magical, Projectile, Area }
    public enum DefenseKind { None, Guard, Parry, Nullify, Reflect }
    public enum CombatOutcome { Hit, Blocked, Parried, Dodged, Clash, Nullified, Reflected, Ignored }
    public enum ItemKind { Heal, Energy, CooldownRefresh, Variation, BackpackUpgrade, Tactical }
    public enum TacticalKind { None, Smoke, Repulsion, Barrier, NullField }
    public enum CombatEventKind { Hit, Block, Parry, Dodge, Clash, Nullify, Reflect, Heal, Energy, VariationSwap, TacticalUsed }

    [Serializable]
    public struct RuntimeModifiers
    {
        public float damageMultiplier;
        public float moveSpeedMultiplier;
        public float staggerResistanceMultiplier;
        public float defenseWindowMultiplier;
        public float movementCooldownMultiplier;

        public static RuntimeModifiers Identity => new RuntimeModifiers
        {
            damageMultiplier = 1f,
            moveSpeedMultiplier = 1f,
            staggerResistanceMultiplier = 1f,
            defenseWindowMultiplier = 1f,
            movementCooldownMultiplier = 1f
        };
    }

    public readonly struct CombatEventData
    {
        public readonly CombatEventKind kind;
        public readonly Vector3 position;
        public readonly CharacterRuntime source;
        public readonly CharacterRuntime target;
        public readonly float value;

        public CombatEventData(CombatEventKind kind, Vector3 position, CharacterRuntime source, CharacterRuntime target, float value = 0f)
        {
            this.kind = kind;
            this.position = position;
            this.source = source;
            this.target = target;
            this.value = value;
        }
    }

    public static class CombatEvents
    {
        public static event Action<CombatEventData> Raised;
        public static void Raise(CombatEventData data) => Raised?.Invoke(data);
    }
}
