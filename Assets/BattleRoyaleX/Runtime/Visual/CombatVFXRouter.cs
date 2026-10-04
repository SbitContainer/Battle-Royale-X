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
            CharacterRuntime defender=data.kind==CombatEventKind.Block?data.target:
                data.kind==CombatEventKind.Reflect||data.kind==CombatEventKind.Parry?data.source:null;
            if(defender!=null&&defender.Definition!=null&&defender.Definition.characterClass==CharacterClass.Warrior)
            {
                GameObject contact=new GameObject("Warrior_DefenseImpact");
                contact.transform.position=defender.transform.position+Vector3.up;
                contact.AddComponent<OwnedAbilityEffect>().Configure(defender,null);
                contact.AddComponent<WarriorSkillPresentation>().Begin(defender,WarriorSkillPresentation.Kind.DefenseImpact,.35f,.6f,0,false);
            }
            // Warrior effects are authored here/per execution, not duplicated by generic rune profiles.
            if (data.ability != null && data.source != null &&
                data.ability.requiredClass == CharacterClass.Warrior && data.ability.classRestricted)
            {
                if(data.ability.slot==AbilitySlot.BasicAttack && data.kind==CombatEventKind.AbilityAttack)
                {
                    if(data.phase==AbilityPhase.Active)
                        SpawnWarrior(data,WarriorSkillPresentation.Kind.Slash,Mathf.Max(.16f,data.ability.activeTime+.1f),
                            Mathf.Min(data.ability.range,1.6f),true);
                    return;
                }
                bool ultimate = data.ability.behavior == AbilityBehavior.WarriorGroundBlast ||
                    data.ability.behavior == AbilityBehavior.WarriorGroundField || data.ability.behavior == AbilityBehavior.WarriorGroundWaves;
                bool dedicated = ultimate || data.ability.behavior == AbilityBehavior.WarriorFortress ||
                    data.ability.behavior == AbilityBehavior.WarriorSkillCapture || data.ability.behavior == AbilityBehavior.Guard ||
                    data.ability.behavior == AbilityBehavior.WarriorShieldCharge || data.ability.behavior == AbilityBehavior.WarriorPursuitStrike ||
                    data.ability.behavior == AbilityBehavior.WarriorPursuitLong;
                bool abilityEvent = data.kind == CombatEventKind.AbilityUltimate || data.kind == CombatEventKind.AbilityGuard ||
                    data.kind == CombatEventKind.AbilityMove;
                if (dedicated && abilityEvent)
                {
                    if (ultimate && data.phase == AbilityPhase.Startup)
                        SpawnWarrior(data, WarriorSkillPresentation.Kind.Conjuration, data.ability.startup,
                            data.ability.fieldRadius, true);
                    return;
                }
                if (data.kind == CombatEventKind.Hit)
                {
                    if (data.value > 0f)
                        SpawnWarrior(data, WarriorSkillPresentation.Kind.Impact, ultimate ? .55f : .3f,
                            ultimate ? .85f : .4f, false);
                    // Preserve profile impact audio while replacing only the visual.
                    if (data.ability.visualProfile != null && data.ability.visualProfile.impactClip != null)
                        AudioSource.PlayClipAtPoint(data.ability.visualProfile.impactClip, data.position);
                    return;
                }
            }
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
                if (data.phase == AbilityPhase.Active && data.ability.behavior == AbilityBehavior.Repulsion &&
                    data.source != null && data.source.Definition != null &&
                    data.source.Definition.characterClass == CharacterClass.Mage && abilityProfile.areaPrefab != null)
                {
                    GameObject flames = Instantiate(abilityProfile.areaPrefab,
                        new Vector3(data.position.x, 0.07f, data.position.z), Quaternion.identity);
                    flames.AddComponent<ExpandingImpactPresentation>().radius =
                        Mathf.Max(0.5f, data.ability.explosionRadius);
                    return;
                }
                bool worldField = data.ability.behavior == AbilityBehavior.SlowField ||
                    data.ability.behavior == AbilityBehavior.PullTrap ||
                    data.ability.behavior == AbilityBehavior.ArrowRain ||
                    (data.ability.behavior == AbilityBehavior.AreaAttack && data.ability.activeTime > 0.5f) ||
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

        void SpawnWarrior(CombatEventData data, WarriorSkillPresentation.Kind kind, float lifetime, float radius, bool follow)
        {
            GameObject instance = new GameObject("Warrior_" + kind);
            instance.transform.position = kind == WarriorSkillPresentation.Kind.Impact && data.target != null ?
                data.target.transform.position + Vector3.up * .8f : data.position;
            instance.AddComponent<OwnedAbilityEffect>().Configure(data.source, null);
            float phase = data.ability.behavior == AbilityBehavior.WarriorGroundField ? 1f :
                data.ability.behavior == AbilityBehavior.WarriorGroundWaves ? 2f : 0f;
            if(kind==WarriorSkillPresentation.Kind.Slash)phase=Mathf.Clamp(Mathf.RoundToInt(data.value)-1,0,2);
            instance.AddComponent<WarriorSkillPresentation>().Begin(data.source, kind, lifetime, radius, phase, follow);
        }
    }
}
