using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class OwnedAbilityEffect : MonoBehaviour
    {
        public CharacterRuntime Owner { get; private set; }
        public AbilityDefinition Ability { get; private set; }

        public void Configure(CharacterRuntime owner, AbilityDefinition ability)
        {
            Owner = owner;
            Ability = ability;
        }
    }
}
