using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    [CreateAssetMenu(menuName = "Battle Royale X/Visual/Visual Profile Registry", fileName = "VisualProfileRegistry")]
    public sealed class VisualProfileRegistry : ScriptableObject
    {
        public CharacterVisualProfile[] characters;
        public AbilityVisualProfile[] abilities;

        Dictionary<CharacterClass, CharacterVisualProfile> characterLookup;
        Dictionary<string, AbilityVisualProfile> abilityLookup;

        public CharacterVisualProfile GetCharacter(CharacterClass characterClass)
        {
            BuildIfNeeded();
            characterLookup.TryGetValue(characterClass, out var value);
            return value;
        }

        public AbilityVisualProfile GetAbility(string abilityId)
        {
            if (string.IsNullOrWhiteSpace(abilityId)) return null;
            BuildIfNeeded();
            abilityLookup.TryGetValue(abilityId, out var value);
            return value;
        }

        void BuildIfNeeded()
        {
            if (characterLookup != null && abilityLookup != null) return;

            characterLookup = new Dictionary<CharacterClass, CharacterVisualProfile>();
            abilityLookup = new Dictionary<string, AbilityVisualProfile>();

            if (characters != null)
            {
                foreach (var profile in characters)
                    if (profile != null) characterLookup[profile.characterClass] = profile;
            }

            if (abilities != null)
            {
                foreach (var profile in abilities)
                    if (profile != null && !string.IsNullOrWhiteSpace(profile.abilityId))
                        abilityLookup[profile.abilityId] = profile;
            }
        }

        void OnValidate()
        {
            characterLookup = null;
            abilityLookup = null;
        }
    }
}
