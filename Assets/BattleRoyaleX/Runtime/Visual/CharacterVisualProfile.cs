using UnityEngine;

namespace BattleRoyaleX
{
    [CreateAssetMenu(menuName = "Battle Royale X/Visual/Character Visual Profile", fileName = "Visual_Character_")]
    public sealed class CharacterVisualProfile : ScriptableObject
    {
        public CharacterClass characterClass;

        [Header("Character")]
        public GameObject modelPrefab;
        public RuntimeAnimatorController animatorController;

        [Header("Equipment")]
        public GameObject primaryWeaponPrefab;
        public GameObject secondaryWeaponPrefab;
        public string primaryWeaponSocket = "RightHand";
        public string secondaryWeaponSocket = "LeftHand";

        [Header("Shared Feedback")]
        public GameObject lightHitPrefab;
        public GameObject heavyHitPrefab;
        public GameObject bloodLightPrefab;
        public GameObject bloodHeavyPrefab;
        public GameObject dodgePrefab;
        public GameObject classAuraPrefab;

        [Header("UI")]
        public Sprite portrait;
        public Sprite classIcon;

        [Header("Notes")]
        [TextArea] public string visualNotes;
    }
}
