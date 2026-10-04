using UnityEngine;

namespace BattleRoyaleX
{
    // Eligibility is independent of reduction, perfect windows and probabilistic defense.
    [CreateAssetMenu(menuName = "Battle Royale X/Defense Interaction Rule")]
    public sealed class DefenseInteractionRule : ScriptableObject
    {
        public enum RuleCode { All = 1, AllExcept = 2 }
        public RuleCode code = RuleCode.All;
        public string[] excludedAbilityIds = System.Array.Empty<string>();
        public bool Allows(AbilityDefinition attack)
        {
            if (code == RuleCode.All) return true;
            if (code != RuleCode.AllExcept || attack == null || string.IsNullOrEmpty(attack.abilityId)) return false;
            return excludedAbilityIds == null || System.Array.IndexOf(excludedAbilityIds, attack.abilityId) < 0;
        }
    }
}
