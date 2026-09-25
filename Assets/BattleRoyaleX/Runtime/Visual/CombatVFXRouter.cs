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
        public VisualProfileRegistry profiles;
        [Min(0.01f)] public float defaultLifetime = 2f;

        void OnEnable() => CombatEvents.Raised += OnCombatEvent;
        void OnDisable() => CombatEvents.Raised -= OnCombatEvent;

        void OnCombatEvent(CombatEventData data)
        {
            AbilityVisualProfile abilityProfile = profiles != null && data.ability != null
                ? profiles.GetAbility(data.ability.abilityId) : data.ability != null ? data.ability.visualProfile : null;
            if (abilityProfile != null && data.kind == CombatEventKind.Clash &&
                data.phase == AbilityPhase.Completed && data.ability.behavior == AbilityBehavior.ComboProjectileUltimate)
            {
                if (abilityProfile.areaPrefab != null)
                {
                    GameObject wave = Instantiate(abilityProfile.areaPrefab,
                        new Vector3(data.position.x, 0.07f, data.position.z), Quaternion.identity);
                    foreach (ParticleSystem particles in wave.GetComponentsInChildren<ParticleSystem>())
                        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    wave.AddComponent<ExpandingImpactPresentation>().radius = Mathf.Max(1f, data.ability.explosionRadius);
                }
                Spawn(abilityProfile.impactPrefab, data.position, 2.2f, 0.8f);
                return;
            }
            if (abilityProfile != null && (data.kind == CombatEventKind.AbilityAttack ||
                data.kind == CombatEventKind.AbilityGuard || data.kind == CombatEventKind.AbilityMove ||
                data.kind == CombatEventKind.AbilityUltimate))
            {
                if (data.phase != AbilityPhase.Startup && data.phase != AbilityPhase.Active) return;
                bool worldField = data.ability.behavior == AbilityBehavior.SlowField ||
                    data.ability.behavior == AbilityBehavior.PullTrap ||
                    data.ability.behavior == AbilityBehavior.ComboProjectileUltimate;
                if (data.phase == AbilityPhase.Active && worldField) return;
                GameObject cast = data.phase == AbilityPhase.Active && abilityProfile.areaPrefab != null
                    ? abilityProfile.areaPrefab : abilityProfile.castPrefab;
                if (data.phase == AbilityPhase.Active && abilityProfile.areaPrefab == null) return;
                float titanScale = data.source != null && data.source.Abilities != null && data.source.Abilities.IsTitanEvolved &&
                    data.ability.slot == AbilitySlot.Ultimate ? 1.35f : 1f;
                bool groundArea = data.phase == AbilityPhase.Active;
                Vector3 position = groundArea ? new Vector3(data.position.x, 0.06f, data.position.z) :
                    data.position + abilityProfile.castOffset;
                float scale = groundArea ? Mathf.Max(0.5f, data.ability.explosionRadius) : abilityProfile.visualScale * titanScale;
                Spawn(cast, position, scale, abilityProfile.fallbackLifetime);
                if (abilityProfile.castClip != null) AudioSource.PlayClipAtPoint(abilityProfile.castClip, data.position);
                return;
            }

            if (library == null && abilityProfile == null) return;

            GameObject prefab = null;
            AudioClip clip = null;

            switch (data.kind)
            {
                case CombatEventKind.Hit:
                    prefab = abilityProfile != null && abilityProfile.impactPrefab != null ?
                        abilityProfile.impactPrefab : library != null ? library.hitPrefab : null;
                    clip = abilityProfile != null && abilityProfile.impactClip != null ?
                        abilityProfile.impactClip : library != null ? library.hitClip : null;
                    break;
                case CombatEventKind.Block:
                    prefab = library != null ? library.blockPrefab : null;
                    clip = library != null ? library.blockClip : null;
                    break;
                case CombatEventKind.Parry:
                    prefab = library != null ? library.parryPrefab : null;
                    clip = library != null ? library.parryClip : null;
                    break;
                case CombatEventKind.Dodge:
                    prefab = library != null ? library.dodgePrefab : null;
                    clip = library != null ? library.dodgeClip : null;
                    break;
                case CombatEventKind.Clash:
                    prefab = library != null ? library.clashPrefab : null;
                    clip = library != null ? library.clashClip : null;
                    break;
                case CombatEventKind.Nullify:
                    prefab = library != null ? library.nullifyPrefab : null;
                    break;
                case CombatEventKind.Reflect:
                    prefab = library != null ? library.reflectPrefab : null;
                    break;
                case CombatEventKind.Heal:
                    prefab = library != null ? library.healPrefab : null;
                    break;
                case CombatEventKind.Energy:
                    prefab = library != null ? library.energyPrefab : null;
                    break;
                case CombatEventKind.VariationSwap:
                    prefab = library != null ? library.variationSwapPrefab : null;
                    break;
                case CombatEventKind.TacticalUsed:
                    prefab = library != null ? library.tacticalUsedPrefab : null;
                    break;
            }

            Spawn(prefab, data.position, 1f, defaultLifetime);
            if (clip != null) AudioSource.PlayClipAtPoint(clip, data.position);
        }

        void Spawn(GameObject prefab, Vector3 position, float scale, float lifetime)
        {
            if (prefab == null) return;
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            instance.transform.localScale *= Mathf.Max(0.01f, scale);
            Destroy(instance, Mathf.Max(0.05f, lifetime));
        }
    }
}
