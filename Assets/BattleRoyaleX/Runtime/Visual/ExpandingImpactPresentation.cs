using UnityEngine;

namespace BattleRoyaleX
{
    // A bounded, presentation-only shockwave. Its final radius is supplied by the combat event.
    public sealed class ExpandingImpactPresentation : MonoBehaviour
    {
        public float radius = 1f;
        float elapsed;
        LineRenderer[] rings;
        void Awake() => rings = GetComponentsInChildren<LineRenderer>();
        void Update()
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 0.55f);
            transform.localScale = Vector3.one * Mathf.Lerp(0.15f, radius, 1f - (1f - t) * (1f - t));
            foreach (LineRenderer ring in rings)
            {
                Color color = ring.startColor;
                color.a = (1f - t) * 0.8f;
                ring.startColor = ring.endColor = color;
            }
            if (t >= 1f) Destroy(gameObject);
        }
    }
}
