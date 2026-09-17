using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public static class CombatResolver
    {
        const string AttackNullifyMagicKey = "AttackNullifyMagic";

        public static void ResolveAttack(Hitbox hitbox, Hurtbox hurtbox)
        {
            if (hitbox == null || hurtbox == null || hitbox.Owner == null || hurtbox.Owner == null) return;
            CharacterRuntime attacker = hitbox.Owner;
            CharacterRuntime target = hurtbox.Owner;
            if (attacker.TeamId == target.TeamId) return;

            CombatOutcome outcome = target.Defense.ResolveIncoming(hitbox.Packet, out float finalDamage);
            Vector3 eventPos = hurtbox.transform.position;

            switch (outcome)
            {
                case CombatOutcome.Dodged:
                    CombatEvents.Raise(new CombatEventData(CombatEventKind.Dodge, eventPos, attacker, target));
                    return;

                case CombatOutcome.Parried:
                    attacker.State.ApplyStagger(0.32f / Mathf.Max(0.1f, attacker.Modifiers.staggerResistanceMultiplier));
                    CombatEvents.Raise(new CombatEventData(CombatEventKind.Parry, eventPos, target, attacker));
                    hitbox.Cancel();
                    return;

                case CombatOutcome.Nullified:
                    CombatEvents.Raise(new CombatEventData(CombatEventKind.Nullify, eventPos, target, attacker));
                    hitbox.Cancel();
                    return;

                case CombatOutcome.Reflected:
                    CombatEvents.Raise(new CombatEventData(CombatEventKind.Reflect, eventPos, target, attacker));
                    attacker.Health.ApplyDamage(hitbox.Packet.damage);
                    hitbox.Cancel();
                    return;

                case CombatOutcome.Blocked:
                    target.Health.ApplyDamage(finalDamage);
                    CombatEvents.Raise(new CombatEventData(CombatEventKind.Block, eventPos, attacker, target, finalDamage));
                    break;

                default:
                    target.Health.ApplyDamage(finalDamage);
                    CombatEvents.Raise(new CombatEventData(CombatEventKind.Hit, eventPos, attacker, target, finalDamage));
                    break;
            }

            if (hitbox.Packet.knockback > 0f && target.Motor != null)
                target.Motor.ApplyImpulse(hitbox.Packet.direction, hitbox.Packet.knockback);
        }

        public static void ResolveHitboxInteraction(Hitbox a, Hitbox b)
        {
            if (a == null || b == null || a.Cancelled || b.Cancelled || a.Owner == null || b.Owner == null) return;
            if (a.Owner.TeamId == b.Owner.TeamId) return;

            bool aPhysical = a.Packet.attackKind == AttackKind.Physical;
            bool bPhysical = b.Packet.attackKind == AttackKind.Physical;
            bool aMagic = IsMagicLike(a.Packet.attackKind);
            bool bMagic = IsMagicLike(b.Packet.attackKind);

            if (aPhysical && bPhysical && a.Packet.clashable && b.Packet.clashable)
            {
                ResolvePhysicalClash(a, b);
                return;
            }

            if (aMagic && bMagic)
            {
                ResolveMagicCollision(a, b);
                return;
            }

            if (aPhysical && bMagic && TryPhysicalNullify(a, b)) return;
            if (bPhysical && aMagic && TryPhysicalNullify(b, a)) return;
        }

        static bool IsMagicLike(AttackKind kind) => kind == AttackKind.Magical || kind == AttackKind.Projectile;

        static void ResolvePhysicalClash(Hitbox a, Hitbox b)
        {
            float damageToA = b.Packet.damage * Mathf.Clamp01(b.Packet.clashDamageFactor);
            float damageToB = a.Packet.damage * Mathf.Clamp01(a.Packet.clashDamageFactor);
            a.Owner.Health.ApplyDamage(damageToA);
            b.Owner.Health.ApplyDamage(damageToB);
            a.Owner.State.ApplyStagger(0.14f);
            b.Owner.State.ApplyStagger(0.14f);

            Vector3 pos = (a.transform.position + b.transform.position) * 0.5f;
            CombatEvents.Raise(new CombatEventData(CombatEventKind.Clash, pos, a.Owner, b.Owner, damageToB));
            a.Cancel();
            b.Cancel();
        }

        static void ResolveMagicCollision(Hitbox a, Hitbox b)
        {
            Vector3 pos = (a.transform.position + b.transform.position) * 0.5f;
            float radius = Mathf.Max(1f, Mathf.Max(a.Packet.explosionRadius, b.Packet.explosionRadius));
            float aoeDamage = (a.Packet.damage + b.Packet.damage) * 0.35f;
            ApplyAreaDamage(pos, radius, aoeDamage, a.Owner, b.Owner);
            CombatEvents.Raise(new CombatEventData(CombatEventKind.Clash, pos, a.Owner, b.Owner, aoeDamage));
            a.Cancel();
            b.Cancel();
        }

        static bool TryPhysicalNullify(Hitbox physical, Hitbox magic)
        {
            if (!physical.Packet.canDestroyMagicalProjectiles || physical.Owner.Interactions == null) return false;
            if (!physical.Owner.Interactions.TryConsume(AttackNullifyMagicKey, physical.Packet.attackInteractionCooldown)) return false;

            Vector3 pos = (physical.transform.position + magic.transform.position) * 0.5f;
            physical.Cancel();
            magic.Cancel();
            CombatEvents.Raise(new CombatEventData(CombatEventKind.Nullify, pos, physical.Owner, magic.Owner));
            return true;
        }

        static void ApplyAreaDamage(Vector3 center, float radius, float damage, CharacterRuntime sourceA, CharacterRuntime sourceB)
        {
            Collider[] hits = Physics.OverlapSphere(center, radius);
            HashSet<CharacterRuntime> damaged = new HashSet<CharacterRuntime>();
            foreach (Collider hit in hits)
            {
                CharacterRuntime target = hit.GetComponentInParent<CharacterRuntime>();
                if (target == null || damaged.Contains(target)) continue;
                damaged.Add(target);
                target.Health.ApplyDamage(damage);
                CombatEvents.Raise(new CombatEventData(CombatEventKind.Hit, center, sourceA, target, damage));
            }
        }
    }
}
