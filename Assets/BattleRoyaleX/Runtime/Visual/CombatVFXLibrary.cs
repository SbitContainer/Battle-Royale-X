using UnityEngine;

namespace BattleRoyaleX
{
    [CreateAssetMenu(menuName = "Battle Royale X/Visual/Combat VFX Library", fileName = "CombatVFXLibrary")]
    public sealed class CombatVFXLibrary : ScriptableObject
    {
        [Header("Combat")]
        public GameObject hitPrefab;
        public GameObject blockPrefab;
        public GameObject parryPrefab;
        public GameObject dodgePrefab;
        public GameObject clashPrefab;
        public GameObject nullifyPrefab;
        public GameObject reflectPrefab;

        [Header("Resources")]
        public GameObject healPrefab;
        public GameObject energyPrefab;
        public GameObject variationSwapPrefab;
        public GameObject tacticalUsedPrefab;

        [Header("Audio")]
        public AudioClip hitClip;
        public AudioClip blockClip;
        public AudioClip parryClip;
        public AudioClip dodgeClip;
        public AudioClip clashClip;
    }
}
