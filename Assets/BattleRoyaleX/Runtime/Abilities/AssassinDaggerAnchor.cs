using System;
using UnityEngine;

namespace BattleRoyaleX
{
    // Swept collision bounds the anchor by walls and first enemy contact even at low FPS.
    public sealed class AssassinDaggerAnchor : MonoBehaviour
    {
        CharacterRuntime owner;
        Hitbox hit;
        ProjectileHitboxMover projectileIdentity;
        float expires, opened, travelled, rootY;
        bool stopped, cancelled;
        public AbilityDefinition Ability { get; private set; }
        public Vector3 Direction { get; private set; }
        public bool CanRecast => !cancelled && hit != null && !hit.Cancelled && owner != null && !owner.Health.IsDead && Time.time >= opened && Time.time < expires;
        public Vector3 TeleportPosition => new Vector3(transform.position.x, rootY, transform.position.z);

        public void Configure(CharacterRuntime source, AbilityDefinition ability, Vector3 direction)
        {
            owner = source; Ability = ability; Direction = direction.normalized; rootY = source.transform.position.y;
            opened = Time.time + ability.secondActivationDelay; expires = Time.time + ability.secondActivationWindow;
            hit = gameObject.AddComponent<Hitbox>();
            hit.Configure(owner, new DamagePacket(owner, ability, Direction), Vector3.one * 0.1f, ability.secondActivationWindow);
            // Reuse existing trigger-based projectile interactions; this component owns the swept motion.
            projectileIdentity = gameObject.AddComponent<ProjectileHitboxMover>(); projectileIdentity.enabled = false;
            projectileIdentity.direction = Direction; projectileIdentity.speed = ability.projectileSpeed;
            gameObject.AddComponent<ProjectileInteractionState>();
            var visuals = gameObject.AddComponent<AssassinDaggersPresentation>(); visuals.Configure(1, 0f);
            transform.rotation = Quaternion.LookRotation(Direction);
        }

        void Update()
        {
            if (cancelled) return;
            if (owner == null || owner.Health.IsDead || Time.time >= expires) { Cancel(); return; }
            if (hit == null || hit.Cancelled) { Cancel(); return; }
            if (stopped) return;
            Direction = projectileIdentity.direction.normalized;
            var packet = hit.Packet; packet.direction = Direction; hit.UpdatePacket(packet);
            float step = Mathf.Min(Ability.projectileSpeed * Time.deltaTime, Mathf.Max(0f, Ability.range - travelled));
            Vector3 origin = transform.position;
            var contacts = Physics.SphereCastAll(origin, Ability.width * 0.5f, Direction, step, ~0, QueryTriggerInteraction.Collide);
            Array.Sort(contacts, (a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit contact in contacts)
            {
                var actor = contact.collider.GetComponentInParent<CharacterRuntime>();
                if (actor == owner || actor != null && actor.TeamId == owner.TeamId) continue;
                Hurtbox hurt = contact.collider.GetComponent<Hurtbox>();
                if (hurt != null && hurt.Owner != null)
                {
                    hit.TryResolveHurtbox(hurt);
                    transform.position = origin + Direction * (contact.distance - 0.45f);
                    stopped = true; return;
                }
                if (contact.collider.isTrigger || actor != null) continue;
                // Back off from the contact even if it lies within this frame's step:
                // the full character capsule is wider than the projectile.
                transform.position = origin + Direction * (contact.distance - 0.6f);
                stopped = true; return;
            }
            transform.position += Direction * step; travelled += step;
            if (travelled >= Ability.range) stopped = true;
        }

        public void Cancel() { cancelled = true; if (hit != null) hit.Cancel(); Destroy(gameObject); }
    }
}
