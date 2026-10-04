using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class SkillVfxMotion : MonoBehaviour
    {
        public Vector3 spin = new Vector3(0f, 70f, 0f);
        public float pulse = 0.08f;
        Vector3 baseScale;
        float bornAt;
        void Awake() { baseScale = transform.localScale; bornAt = Time.time; }
        void Update()
        {
            transform.Rotate(spin * Time.deltaTime, Space.Self);
            transform.localScale = baseScale * (1f + Mathf.Sin((Time.time-bornAt)*6f)*pulse);
        }
    }

    public sealed class AreaCastPresentation : MonoBehaviour { }
}
