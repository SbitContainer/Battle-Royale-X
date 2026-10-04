using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class SlowField : MonoBehaviour
    {
        CharacterRuntime owner;
        AbilityDefinition ability;
        float radius;
        float multiplier;
        float expiresAt;
        float nextDamageAt;

        public void Configure(CharacterRuntime source, AbilityDefinition ability)
        {
            owner = source;
            this.ability = ability;
            radius = Mathf.Max(0.5f, ability.fieldRadius > 0f ? ability.fieldRadius : ability.explosionRadius);
            multiplier = 1f - Mathf.Clamp01(ability.slowPercent);
            expiresAt = Time.time + Mathf.Max(0.1f, ability.fieldDuration);
            nextDamageAt = Time.time;
        }

        void Update()
        {
            if (Time.time >= expiresAt) { Destroy(gameObject); return; }
            bool damageTick = Time.time >= nextDamageAt;
            Hitbox pulse = null;
            if (damageTick && owner != null && ability != null && ability.damage > 0f)
            {
                nextDamageAt = Time.time + 0.5f;
                GameObject go = new GameObject("SlowFieldDamagePulse");
                go.transform.position = transform.position;
                pulse = go.AddComponent<Hitbox>();
                pulse.Configure(owner, new DamagePacket(owner, ability, owner.Motor.Facing), Vector3.one, 0.08f);
                go.GetComponent<Collider>().enabled = false;
            }
            foreach (Collider hit in Physics.OverlapSphere(transform.position, radius, ~0, QueryTriggerInteraction.Collide))
            {
                CharacterRuntime target = hit.GetComponentInParent<CharacterRuntime>();
                if (target == null || target == owner || target.Health == null || target.Health.IsDead ||
                    owner == null || target.TeamId == owner.TeamId) continue;
                target.ApplyMovementSlow(multiplier, 0.15f);
                if (pulse != null) pulse.TryResolveHurtbox(hit.GetComponent<Hurtbox>());
            }
        }
    }
}
