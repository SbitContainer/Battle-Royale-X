using UnityEngine;

namespace BattleRoyaleX
{
    // Recomputes the packet before trigger contact; the heavy arrow rewards a long, exact hit.
    public sealed class ArcherDistanceDamage : MonoBehaviour
    {
        Hitbox hit;
        Vector3 origin;
        Vector3 previousPosition;
        float baseDamage;
        float range;
        public void Configure(float maximumRange)
        {
            hit = GetComponent<Hitbox>();
            origin = transform.position;
            previousPosition = origin;
            range = Mathf.Max(1f, maximumRange);
            baseDamage = hit.Packet.damage;
        }
        void LateUpdate()
        {
            if (hit == null || hit.Cancelled) return;
            DamagePacket packet = hit.Packet;
            packet.damage = baseDamage * Mathf.Lerp(1f, 1.8f,
                Mathf.Clamp01(Vector3.Distance(origin, transform.position) / range));
            hit.UpdatePacket(packet);
            // The narrow, fast arrow can cross an entire Hurtbox between physics frames.
            foreach (Collider collider in Physics.OverlapCapsule(previousPosition, transform.position,
                0.14f, ~0, QueryTriggerInteraction.Collide))
            {
                Hurtbox hurt = collider.GetComponent<Hurtbox>();
                if (hurt == null || hurt.Owner == null || hit.Owner == null ||
                    hurt.Owner == hit.Owner || hurt.Owner.TeamId == hit.Owner.TeamId) continue;
                hit.TryResolveHurtbox(hurt);
                hit.Cancel();
                Destroy(gameObject);
                break;
            }
            previousPosition = transform.position;
        }
    }

    public sealed class ArcherBackflipVisual : MonoBehaviour
    {
        Transform model;
        Quaternion original;
        float startedAt;
        float duration;
        public void Begin(Transform visual, float seconds)
        {
            model = visual;
            if (model == null) { Destroy(this); return; }
            original = model.localRotation;
            duration = Mathf.Max(0.1f, seconds);
            startedAt = Time.time;
        }
        void LateUpdate()
        {
            if (model == null) { Destroy(this); return; }
            float progress = Mathf.Clamp01((Time.time - startedAt) / duration);
            model.localRotation = original * Quaternion.Euler(-360f * progress, 0f, 0f);
            if (progress >= 1f) { model.localRotation = original; Destroy(this); }
        }
        void OnDestroy() { if (model != null) model.localRotation = original; }
    }

    public sealed class ArcherGrappleRope : MonoBehaviour
    {
        CharacterRuntime owner;
        LineRenderer line;
        Vector3 endpoint;
        float expiresAt;
        public void Begin(CharacterRuntime source, Vector3 target, float seconds)
        {
            owner = source;
            endpoint = target + Vector3.up;
            expiresAt = Time.time + Mathf.Max(0.1f, seconds);
            line = gameObject.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.startWidth = 0.07f;
            line.endWidth = 0.035f;
            line.useWorldSpace = true;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = new Color(0.95f, 0.76f, 0.32f);
            line.endColor = new Color(1f, 0.94f, 0.70f);
        }
        void LateUpdate()
        {
            if (owner == null || Time.time >= expiresAt) { Destroy(gameObject); return; }
            line.SetPosition(0, owner.transform.position + Vector3.up * 1.2f);
            line.SetPosition(1, endpoint);
        }
        void OnDestroy() { if (line != null && line.material != null) Destroy(line.material); }
    }
}
