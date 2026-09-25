using UnityEngine;

namespace BattleRoyaleX
{
    /// <summary>
    /// Presentation-only bridge for generic CombatEvents.
    /// Gameplay must never depend on this component being present.
    /// </summary>
    public sealed class CombatVFXRouter : MonoBehaviour
    {
        public CombatVFXLibrary library;
        [Min(0.01f)] public float defaultLifetime = 2f;

        void OnEnable() => CombatEvents.Raised += OnCombatEvent;
        void OnDisable() => CombatEvents.Raised -= OnCombatEvent;

        void OnCombatEvent(CombatEventData data)
        {
            if (library == null) return;

            GameObject prefab = null;
            AudioClip clip = null;

            switch (data.kind)
            {
                case CombatEventKind.Hit:
                    prefab = library.hitPrefab;
                    clip = library.hitClip;
                    break;
                case CombatEventKind.Block:
                    prefab = library.blockPrefab;
                    clip = library.blockClip;
                    break;
                case CombatEventKind.Parry:
                    prefab = library.parryPrefab;
                    clip = library.parryClip;
                    break;
                case CombatEventKind.Dodge:
                    prefab = library.dodgePrefab;
                    clip = library.dodgeClip;
                    break;
                case CombatEventKind.Clash:
                    prefab = library.clashPrefab;
                    clip = library.clashClip;
                    break;
                case CombatEventKind.Nullify:
                    prefab = library.nullifyPrefab;
                    break;
                case CombatEventKind.Reflect:
                    prefab = library.reflectPrefab;
                    break;
                case CombatEventKind.Heal:
                    prefab = library.healPrefab;
                    break;
                case CombatEventKind.Energy:
                    prefab = library.energyPrefab;
                    break;
                case CombatEventKind.VariationSwap:
                    prefab = library.variationSwapPrefab;
                    break;
                case CombatEventKind.TacticalUsed:
                    prefab = library.tacticalUsedPrefab;
                    break;
            }

            Spawn(prefab, data.position);
            if (clip != null) AudioSource.PlayClipAtPoint(clip, data.position);
        }

        void Spawn(GameObject prefab, Vector3 position)
        {
            if (prefab == null) return;
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            Destroy(instance, Mathf.Max(0.05f, defaultLifetime));
        }
    }
}
