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
            transform.position += direction.normalized * speed * Time.deltaTime;
            if (Time.time - bornAt >= maxLifetime) Destroy(gameObject);
        }
    }
}
