using UnityEngine;

namespace BattleRoyaleX
{
    // One geometry contract for input, ground casts and their presentation. No damage authority here.
    public static class AbilityAimSolution
    {
        public static bool IsGroundTarget(AbilityDefinition a)
        {
            if (a == null) return false;
            string name = a.behavior.ToString();
            return a.behavior == AbilityBehavior.SlowField || a.behavior == AbilityBehavior.PullTrap ||
                a.behavior == AbilityBehavior.ArrowRain || a.behavior == AbilityBehavior.AreaAttack;
        }

        public static bool IsSelfCentered(AbilityDefinition a)
        {
            if (a == null) return true;
            string name = a.behavior.ToString();
            return a.behavior == AbilityBehavior.Guard || a.behavior == AbilityBehavior.Parry ||
                a.behavior == AbilityBehavior.Repulsion || a.behavior == AbilityBehavior.OrbitingDaggers ||
                a.behavior == AbilityBehavior.SmokeEscape || a.behavior == AbilityBehavior.UltimateBuff ||
                a.behavior == AbilityBehavior.TimedBuff || a.behavior == AbilityBehavior.CloneTeleport ||
                name == "WarriorFortress" || name == "WarriorSkillCapture" || name == "WarriorGuardAura" ||
                name == "WarriorGroundBlast" || name == "WarriorGroundField" || name == "WarriorGroundWaves";
        }

        public static bool IsMovement(AbilityDefinition a)
        {
            if (a == null) return false;
            return a.behavior == AbilityBehavior.Dash || a.behavior == AbilityBehavior.DashThrough ||
                a.behavior == AbilityBehavior.DashReturn || a.behavior == AbilityBehavior.Dodge ||
                a.behavior == AbilityBehavior.Blink || a.behavior == AbilityBehavior.MultiDash ||
                a.behavior == AbilityBehavior.ChargedDashSequence || a.behavior == AbilityBehavior.Grapple;
        }

        public static bool IsTracking(AbilityDefinition a) => a != null && (a.pursueTarget ||
            a.behavior == AbilityBehavior.HuntSequence || a.behavior == AbilityBehavior.ExecutionStrike ||
            a.behavior.ToString() == "WarriorPursuitStrike" || a.behavior.ToString() == "WarriorPursuitLong");

        public static bool IsProjectile(AbilityDefinition a) => a != null && (a.behavior == AbilityBehavior.ProjectileAttack ||
            a.behavior == AbilityBehavior.SeekingProjectile || a.behavior == AbilityBehavior.MultiShot ||
            a.behavior == AbilityBehavior.ComboProjectileUltimate || a.behavior.ToString() == "WarriorShieldCharge");

        public static float MaxDistance(AbilityDefinition a)
        {
            if (a == null) return 0f;
            if (IsTracking(a)) return a.behavior == AbilityBehavior.HuntSequence || a.behavior == AbilityBehavior.ExecutionStrike ? a.huntAcquireRange :
                Mathf.Max(a.pursuitAcquireRange, a.movementDistance);
            if (IsMovement(a)) return Mathf.Max(0f, a.movementDistance);
            if (IsProjectile(a)) return Mathf.Max(0f, a.range) + 1.1f;
            return Mathf.Max(0f, a.range);
        }

        public static Vector3 ResolvePoint(Vector3 origin, Vector3 direction, AbilityDefinition ability, float fraction = 1f)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) direction = Vector3.forward;
            if (IsSelfCentered(ability)) return origin;
            float distance = MaxDistance(ability);
            if (IsGroundTarget(ability)) distance *= Mathf.Clamp(fraction, 0.15f, 1f);
            if (ability != null && ability.reverseMovement && IsMovement(ability)) direction = -direction;
            return origin + direction.normalized * distance;
        }

        public static Vector3 ClampGroundPoint(Vector3 origin, Vector3 point, AbilityDefinition ability)
        {
            if (IsSelfCentered(ability)) return origin;
            Vector3 offset = point - origin; offset.y = 0f;
            return origin + Vector3.ClampMagnitude(offset, MaxDistance(ability));
        }

        public static float AreaRadius(AbilityDefinition a)
        {
            if (a == null) return 0f;
            if (a.behavior == AbilityBehavior.SmokeEscape) return a.smokeRadius;
            if (a.behavior.ToString() == "WarriorFortress") return 1.25f;
            if (a.behavior == AbilityBehavior.Repulsion || a.behavior == AbilityBehavior.AreaAttack)
                return Mathf.Max(0.1f, a.explosionRadius);
            if (a.behavior == AbilityBehavior.Guard || a.behavior == AbilityBehavior.Parry ||
                a.behavior == AbilityBehavior.UltimateBuff || a.behavior == AbilityBehavior.TimedBuff ||
                a.behavior.ToString() == "WarriorSkillCapture" || a.behavior.ToString() == "WarriorGuardAura") return 0.85f;
            return Mathf.Max(0.1f, a.fieldRadius);
        }

        public static Vector3 ClonePoint(CharacterRuntime owner, AbilityDefinition ability, Vector3 direction)
        {
            Vector3 best = owner.transform.position;
            float score = float.NegativeInfinity;
            foreach (ArcaneCloneMotion clone in Object.FindObjectsByType<ArcaneCloneMotion>())
            {
                OwnedAbilityEffect marker = clone.GetComponent<OwnedAbilityEffect>();
                if (marker == null || marker.Owner != owner || marker.Ability != ability) continue;
                Vector3 offset = clone.transform.position - owner.transform.position; offset.y = 0f;
                float dot = Vector3.Dot(direction.normalized, offset.normalized);
                if (dot > score) { score = dot; best = clone.transform.position; }
            }
            return best;
        }

        public static float GrappleDistance(CharacterRuntime owner, AbilityDefinition ability, Vector3 direction,
            out CharacterRuntime target)
        {
            target = null;
            float distance = ability.movementDistance;
            foreach (RaycastHit hit in Physics.RaycastAll(owner.transform.position + Vector3.up,
                direction.normalized, distance, ~0, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.transform.IsChildOf(owner.transform) || hit.distance >= distance) continue;
                CharacterRuntime found = hit.collider.GetComponentInParent<CharacterRuntime>();
                if (found != null && (found.TeamId == owner.TeamId || found.Health.IsDead)) continue;
                distance = hit.distance; target = found;
            }
            return distance;
        }
    }
}
