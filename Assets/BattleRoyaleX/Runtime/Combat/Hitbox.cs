using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public sealed class Hitbox : MonoBehaviour
    {
        public CharacterRuntime Owner { get; private set; }
        public DamagePacket Packet { get; private set; }
        public bool Cancelled { get; private set; }

        readonly HashSet<int> resolvedIds = new HashSet<int>();

        public void Configure(CharacterRuntime owner, DamagePacket packet, Vector3 size, float lifetime)
        {
            Owner = owner;
            Packet = packet;
            BoxCollider box = GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = size;

            Rigidbody body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            Destroy(gameObject, Mathf.Max(0.02f, lifetime));
        }

        void OnTriggerEnter(Collider other)
        {
            if (Cancelled || Owner == null) return;

            Hitbox otherHitbox = other.GetComponent<Hitbox>();
            if (otherHitbox != null)
            {
                if (otherHitbox.Owner != null && otherHitbox.Owner != Owner && MarkResolved(otherHitbox.GetInstanceID()))
                    CombatResolver.ResolveHitboxInteraction(this, otherHitbox);
                return;
            }

            Hurtbox hurtbox = other.GetComponent<Hurtbox>();
            if (hurtbox == null || hurtbox.Owner == null || hurtbox.Owner == Owner) return;
            if (!MarkResolved(hurtbox.Owner.GetInstanceID())) return;
            CombatResolver.ResolveAttack(this, hurtbox);
        }

        bool MarkResolved(int id)
        {
            if (resolvedIds.Contains(id)) return false;
            resolvedIds.Add(id);
            return true;
        }

        public void Cancel()
        {
            Cancelled = true;
            Collider c = GetComponent<Collider>();
            if (c != null) c.enabled = false;
        }
    }
}
