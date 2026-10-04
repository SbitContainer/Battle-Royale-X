using UnityEngine;

namespace BattleRoyaleX
{
    // Presentation follows the damaging Blink dash with a short delay.
    public sealed class MageSoulEcho : MonoBehaviour
    {
        CharacterRuntime owner;
        float expiresAt;

        public static void Create(CharacterRuntime source)
        {
            if (source == null) return;
            GameObject echo = CloneTeleportController.CreateProjection(source);
            echo.name = "BlinkSoulEcho";
            echo.transform.SetPositionAndRotation(source.transform.position, source.transform.rotation);
            Collider collider = echo.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            echo.AddComponent<MageSoulEcho>().Initialize(source);
        }

        void Initialize(CharacterRuntime source)
        {
            owner = source;
            expiresAt = Time.time + 0.48f;
        }

        void LateUpdate()
        {
            if (owner == null || Time.time >= expiresAt) { Destroy(gameObject); return; }
            transform.position = Vector3.Lerp(transform.position, owner.transform.position,
                1f - Mathf.Exp(-10f * Time.deltaTime));
            transform.rotation = owner.transform.rotation;
        }
    }
}
