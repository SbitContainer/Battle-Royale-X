using UnityEngine;

namespace BattleRoyaleX
{
    [RequireComponent(typeof(Camera))]
    public sealed class IsometricCameraRig : MonoBehaviour
    {
        public Transform targetA;
        public Transform targetB;
        public float pitch = 52f;
        public float yaw = 45f;
        public float fieldOfView = 38f;
        public float minDistance = 12f;
        public float maxDistance = 18f;
        public float separationForMaxDistance = 20f;
        [Range(0.01f, 1f)] public float followSharpness = 0.12f;

        Camera cam;
        bool positioned;

        void Awake()
        {
            cam = GetComponent<Camera>();
            cam.orthographic = false;
            cam.fieldOfView = fieldOfView;
        }

        void LateUpdate()
        {
            if (targetA == null) return;
            Vector3 center = targetB != null ? (targetA.position + targetB.position) * 0.5f : targetA.position;
            float separation = targetB != null ? Vector3.Distance(targetA.position, targetB.position) : 0f;
            float distance = Mathf.Lerp(minDistance, maxDistance, Mathf.Clamp01(separation / separationForMaxDistance));
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            cam.fieldOfView = fieldOfView;
            // maxDistance is the preferred zoom, not a reason to crop a combatant.
            // Fit a body envelope including the feet below the logical root.
            distance = Mathf.Max(distance, RequiredDistance(targetA, center, rotation));
            if (targetB != null) distance = Mathf.Max(distance, RequiredDistance(targetB, center, rotation));
            Vector3 desired = center + rotation * (Vector3.back * distance);
            float blend = 1f - Mathf.Pow(1f - followSharpness, Time.deltaTime * 60f);
            transform.position = positioned ? Vector3.Lerp(transform.position, desired, blend) : desired;
            transform.rotation = rotation;
            positioned = true;
        }

        float RequiredDistance(Transform target, Vector3 center, Quaternion rotation)
        {
            Quaternion inverse = Quaternion.Inverse(rotation);
            float vertical = Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad) * 0.78f;
            float horizontal = vertical * Mathf.Max(0.1f, cam.aspect);
            float required = minDistance;
            for (int x = -1; x <= 1; x += 2)
            for (int y = -1; y <= 2; y += 3)
            for (int z = -1; z <= 1; z += 2)
            {
                Vector3 point = inverse * (target.position + new Vector3(x, y, z) - center);
                required = Mathf.Max(required, Mathf.Abs(point.x) / horizontal - point.z,
                    Mathf.Abs(point.y) / vertical - point.z);
            }
            return required;
        }
    }
}
