using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class ProjectileHitboxMover : MonoBehaviour
    {
        public Vector3 direction = Vector3.forward;
        public float speed = 14f;
        public float maxLifetime = 4f;
        float bornAt;

        void Awake() => bornAt = Time.time;

        void Update()
        {
            Vector3 previous = transform.position;
            transform.position += direction.normalized * speed * Time.deltaTime;
            if (GetComponent<ArcherDistanceDamage>() == null) ResolveTravel(GetComponent<Hitbox>(), previous, transform.position);
            if (Time.time - bornAt >= maxLifetime) Destroy(gameObject);
        }

        public static void ResolveTravel(Hitbox hit, Vector3 from, Vector3 to)
        {
            if (hit == null || hit.Cancelled || (to-from).sqrMagnitude < 0.00001f) return;
            Vector3 direction = (to-from).normalized;
            DamagePacket packet = hit.Packet; packet.direction = direction; hit.UpdatePacket(packet);
            BoxCollider box = hit.GetComponent<BoxCollider>();
            Vector3 size = box != null ? box.size : Vector3.one * 0.3f;
            size.z += Vector3.Distance(from,to);
            foreach (Collider collider in Physics.OverlapBox((from+to)*0.5f,size*0.5f,
                Quaternion.LookRotation(direction),~0,QueryTriggerInteraction.Collide))
                hit.TryResolveHurtbox(collider.GetComponent<Hurtbox>());
        }
    }
}
