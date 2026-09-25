using UnityEngine;

namespace BattleRoyaleX
{
    // Local-space flow only; neither particles nor rings participate in combat physics.
    public sealed class ArcaneFieldPresentation : MonoBehaviour
    {
        public bool inward = true;
        ParticleSystem motes;
        readonly ParticleSystem.Particle[] particles = new ParticleSystem.Particle[32];
        Transform innerRing;

        void Awake()
        {
            motes = GetComponentInChildren<ParticleSystem>();
            innerRing = transform.Find("Radius_1");
        }

        void LateUpdate()
        {
            if (innerRing != null) innerRing.localRotation = Quaternion.Euler(0f, Time.time * 16f, 0f);
            if (motes == null) return;
            int count = motes.GetParticles(particles);
            for (int i = 0; i < count; i++)
            {
                Vector3 radial = particles[i].position;
                radial.y = 0f;
                particles[i].velocity = radial.normalized * (inward ? -0.42f : 0.42f) +
                    Vector3.Cross(Vector3.up, radial) * 0.7f;
            }
            motes.SetParticles(particles, count);
        }
    }
}
