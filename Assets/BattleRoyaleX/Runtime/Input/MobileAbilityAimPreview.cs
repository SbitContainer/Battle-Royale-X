using UnityEngine;

namespace BattleRoyaleX
{
    // Presentation only: shows the same endpoint that is submitted to AbilityController.
    public sealed class MobileAbilityAimPreview : MonoBehaviour
    {
        LineRenderer path, boundary, rangeBoundary, corridor, endpointMarker;
        Material material;
        readonly System.Collections.Generic.List<LineRenderer> extras = new System.Collections.Generic.List<LineRenderer>();
        void Awake()
        {
            Shader shader = Resources.Load<Shader>("BRXAimPreview");
            if (shader == null) shader = Shader.Find("BattleRoyaleX/AimPreview");
            if (shader == null) { Debug.LogError("BRX aim preview shader unavailable; combat remains enabled."); return; }
            material = new Material(shader);
            material.SetColor("_Color", Color.white);
            path = MakeLine("CastDirection", 0.055f);
            boundary = MakeLine("CastArea", 0.06f);
            rangeBoundary = MakeLine("MaximumCastRange", 0.035f);
            corridor = MakeLine("HitWidth", 0.055f);
            endpointMarker = MakeLine("CastEndpoint", 0.08f);
            rangeBoundary.startColor = rangeBoundary.endColor = new Color(0.35f, 0.8f, 1f, 0.28f);
            path.startColor = path.endColor = new Color(0.9f, 0.95f, 1f, 0.9f);
            corridor.startColor = corridor.endColor = new Color(0.2f, 0.85f, 1f, 0.9f);
            boundary.startColor = boundary.endColor = new Color(0.25f, 1f, 0.68f, 0.95f);
            for (int i = 0; i < 12; i++) extras.Add(MakeLine("AdditionalPath_" + i, 0.04f));
        }
        LineRenderer MakeLine(string name, float width)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(transform, false);
            LineRenderer line = child.AddComponent<LineRenderer>();
            line.sharedMaterial = material; line.useWorldSpace = true;
            line.startWidth = line.endWidth = width;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return line;
        }
        public void Show(Vector3 origin, Vector3 endpoint, AbilityDefinition ability, CharacterRuntime owner = null, Vector3 aimDirection = default)
        {
            if (material == null || ability == null) return;
            gameObject.SetActive(true);
            origin.y = endpoint.y = 0.12f;
            path.positionCount = 2; path.SetPosition(0, origin); path.SetPosition(1, endpoint);
            bool area = AbilityAimSolution.IsGroundTarget(ability) || AbilityAimSolution.IsSelfCentered(ability);
            CloneTeleportController clones = owner != null ? owner.GetComponent<CloneTeleportController>() : null;
            bool cloneRecast = ability.behavior == AbilityBehavior.CloneTeleport && clones != null && clones.Ability == ability && clones.CanTeleport;
            if (cloneRecast) area = false;
            Circle(rangeBoundary, origin, AbilityAimSolution.MaxDistance(ability));
            Circle(endpointMarker, endpoint, 0.15f);
            path.enabled = (endpoint - origin).sqrMagnitude > 0.01f;
            rangeBoundary.enabled = !AbilityAimSolution.IsSelfCentered(ability);
            boundary.enabled = area;
            if (area) Circle(boundary, endpoint, AbilityAimSolution.AreaRadius(ability));
            corridor.enabled = !area;
            if (!area)
            {
                Vector3 direction = endpoint - origin;
                if (direction.sqrMagnitude < 0.001f) direction = Vector3.forward;
                float halfWidth = ability.width * 0.5f;
                if (AbilityAimSolution.IsMovement(ability))
                {
                    CharacterController capsule = owner != null ? owner.GetComponent<CharacterController>() : null;
                    halfWidth = Mathf.Max(0.35f, halfWidth) + (capsule != null ? capsule.skinWidth : 0.08f) * 2f;
                }
                Vector3 side = Vector3.Cross(Vector3.up, direction.normalized) * halfWidth;
                Vector3 corridorStart = AbilityAimSolution.IsProjectile(ability) ? origin + direction.normalized * 1.1f : origin;
                Vector3 corridorEnd = endpoint;
                if (AbilityAimSolution.IsProjectile(ability))
                {
                    float halfDepth = Mathf.Max(0.5f, ability.width) * 0.5f;
                    corridorStart -= direction.normalized * halfDepth;
                    corridorEnd += direction.normalized * halfDepth;
                }
                // A corridor, not a thin direction line: the actual full hitbox width remains visible.
                corridor.positionCount = 5;
                corridor.SetPositions(new[] { corridorStart - side, corridorEnd - side, corridorEnd + side, corridorStart + side, corridorStart - side });
            }
            foreach (LineRenderer extra in extras) extra.enabled = false;
            if (ability.behavior == AbilityBehavior.CloneTeleport)
            {
                corridor.enabled = false;
                boundary.enabled = false;
                int index = 0;
                if (cloneRecast)
                {
                    foreach (ArcaneCloneMotion clone in FindObjectsByType<ArcaneCloneMotion>())
                    {
                        OwnedAbilityEffect marker = clone.GetComponent<OwnedAbilityEffect>();
                        if (marker == null || marker.Owner != owner || marker.Ability != ability || index >= extras.Count) continue;
                        Vector3 center = clone.transform.position; center.y = origin.y;
                        extras[index].enabled = true; Circle(extras[index++], center, 0.45f);
                    }
                }
                else
                {
                    Vector3 facing = aimDirection.sqrMagnitude > 0.001f ? aimDirection.normalized : owner != null ? owner.Motor.Facing : Vector3.forward;
                    for (int i = 0; i < 3; i++)
                    {
                        Vector3 center = origin + Quaternion.Euler(0, 120f * i, 0) * facing * Mathf.Min(2.2f, Mathf.Max(2f, ability.movementDistance) * 0.5f);
                        extras[i].enabled = true; Circle(extras[i], center, 0.45f);
                    }
                }
            }
            if (ability.behavior == AbilityBehavior.MultiShot || ability.behavior == AbilityBehavior.SeekingProjectile)
            {
                Vector3 direction = endpoint - origin; direction.Normalize();
                int count = Mathf.Clamp(ability.projectileCount, 1, extras.Count);
                for (int i = 0; i < count; i++)
                {
                    float angle = ability.behavior == AbilityBehavior.MultiShot
                        ? (count == 1 ? 0f : i / (float)(count - 1) - 0.5f) * ability.spreadAngle
                        : (i - (count - 1) * 0.5f) * Mathf.Max(5f, ability.spreadAngle);
                    LineRenderer extra = extras[i]; extra.enabled = true; extra.positionCount = 2;
                    extra.SetPosition(0, origin); extra.SetPosition(1, origin + Quaternion.Euler(0, angle, 0) * direction * (ability.range + 1.1f));
                }
            }
            // Seeking/pursuit routes are indicative: enemies can move after release.
            if (AbilityAimSolution.IsTracking(ability) || ability.seeking)
                path.startColor = path.endColor = new Color(1f, 0.75f, 0.15f, 0.9f);
            else path.startColor = path.endColor = Color.white;
        }
        void Circle(LineRenderer line, Vector3 center, float radius)
        {
            line.positionCount = 65;
            for (int i = 0; i <= 64; i++)
            {
                float angle = i * Mathf.PI * 2f / 64f;
                line.SetPosition(i, center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius);
            }
        }
        void OnDestroy() { if (material != null) Destroy(material); }
    }
}
