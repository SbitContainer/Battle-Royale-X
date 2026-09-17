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
            Vector3 desired = center + rotation * (Vector3.back * distance);
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Pow(1f - followSharpness, Time.deltaTime * 60f));
            transform.rotation = rotation;
            cam.fieldOfView = fieldOfView;
        }
    }
}
