using UnityEngine;

namespace BattleRoyaleX
{
    [CreateAssetMenu(menuName = "Battle Royale X/Visual/Ability Visual Profile", fileName = "Visual_Ability_")]
    public sealed class AbilityVisualProfile : ScriptableObject
    {
        [Header("Identity")]
        public string abilityId;
        public CharacterClass characterClass;

        [Header("Prefabs")]
        public GameObject castPrefab;
        public GameObject projectilePrefab;
        public GameObject impactPrefab;
        public GameObject areaPrefab;
        public GameObject trailPrefab;

        [Header("Audio")]
        public AudioClip castClip;
        public AudioClip impactClip;

        [Header("Placement")]
        public Vector3 castOffset = Vector3.zero;
        public Vector3 projectileOffset = new Vector3(0f, 0.8f, 0.8f);
        public Vector3 areaOffset = Vector3.zero;
        [Min(0.01f)] public float visualScale = 1f;
        [Min(0f)] public float fallbackLifetime = 2f;

        [Header("Optional Tint")]
        public Color primaryTint = Color.white;
        public Color secondaryTint = Color.white;

        [Header("Notes")]
        [TextArea] public string visualNotes;
    }
}
