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

        void Start()
        {
            if (Ability == null || Ability.visualProfile == null) return;
            AbilityVisualProfile profile = Ability.visualProfile;
            bool projectile = GetComponent<ProjectileHitboxMover>() != null;
            bool field = GetComponent<SlowField>() != null || GetComponent<PullField>() != null;
            GameObject prefab = projectile ? profile.projectilePrefab : field ? profile.areaPrefab : null;
            if (prefab == null) return;
            GameObject visual = Instantiate(prefab, transform);
            visual.name = "AbilityPresentation";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            if (field)
            {
                visual.transform.position = new Vector3(transform.position.x, 0.06f, transform.position.z);
                float radius = Mathf.Max(0.5f, Ability.fieldRadius > 0f ? Ability.fieldRadius : Ability.explosionRadius);
                visual.transform.localScale = Vector3.one * radius;
            }
            else
            {
                visual.transform.localScale *= profile.visualScale;
                if (Ability.behavior == AbilityBehavior.ComboProjectileUltimate)
                {
                    bool slow = GetComponent<ProjectileHitboxMover>().speed < Ability.projectileSpeed;
                    visual.transform.localScale = slow ? Vector3.one * 2.4f : new Vector3(0.55f, 0.55f, 3.5f);
                }
            }
        }
    }
}
