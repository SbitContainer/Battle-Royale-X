using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BattleRoyaleX
{
    // Local prototype smoke: a soft volumetric-looking field and a deterministic line-of-sight rule for the training bot.
    public sealed class SmokeField : MonoBehaviour
    {
        static readonly List<SmokeField> active = new List<SmokeField>();

        public float Radius { get; private set; }
        public float ExpiresAt { get; private set; }
        public int ParticleCount { get; private set; }
        public bool SuppressesAttacks { get; private set; }

        Material smokeMaterial;
        Material ringMaterial;

        public static SmokeField Spawn(Vector3 center, float radius, float duration, bool suppressesAttacks = false)
        {
            GameObject go = new GameObject("Smoke_Field");
            go.transform.position = new Vector3(center.x, 0.12f, center.z);
            SmokeField field = go.AddComponent<SmokeField>();
            field.SuppressesAttacks = suppressesAttacks;
            field.Initialize(Mathf.Max(1f, radius), Mathf.Max(0.25f, duration));
            return field;
        }

        public static bool Contains(Vector3 position, bool escapeOnly = false)
        {
            foreach (var field in active)
            {
                if (field == null || Time.time >= field.ExpiresAt || (escapeOnly && !field.SuppressesAttacks)) continue;
                Vector3 delta = position - field.transform.position; delta.y = 0f;
                if (delta.sqrMagnitude <= field.Radius * field.Radius) return true;
            }
            return false;
        }

        public static bool PreventsAttack(CharacterRuntime actor) => actor != null && Contains(actor.transform.position, true);
        public static void ClearAll()
        {
            foreach (var field in active.ToArray()) if (field != null)
            { field.ExpiresAt = Time.time; Destroy(field.gameObject); }
            active.Clear();
        }

        void Update()
        {
            if (smokeMaterial == null) return;
            var local = SmokeVisibility.LocalPlayer;
            Vector3 delta = local != null ? local.transform.position - transform.position : Vector3.one * 999f;
            delta.y = 0f;
            // Dense from outside; a lighter local veil keeps the owner's frosted silhouette readable.
            smokeMaterial.SetFloat("_Opacity", local != null && delta.sqrMagnitude < Radius * Radius ? 0.30f : 1f);
        }

        void Initialize(float radius, float duration)
        {
            Radius = radius;
            ExpiresAt = Time.time + duration;
            active.Add(this);
            BuildParticles(duration);
            BuildBoundary();
            Destroy(gameObject, duration);
        }

        public static bool BlocksSight(Vector3 from, Vector3 to)
        {
            Vector2 start = new Vector2(from.x, from.z);
            Vector2 end = new Vector2(to.x, to.z);
            Vector2 segment = end - start;
            float lengthSqr = segment.sqrMagnitude;

            for (int i = active.Count - 1; i >= 0; i--)
            {
                SmokeField field = active[i];
                if (field == null || Time.time >= field.ExpiresAt)
                {
                    active.RemoveAt(i);
                    continue;
                }
                Vector2 center = new Vector2(field.transform.position.x, field.transform.position.z);
                float t = lengthSqr <= 0.0001f ? 0f : Mathf.Clamp01(Vector2.Dot(center - start, segment) / lengthSqr);
                Vector2 nearest = start + segment * t;
                if ((nearest - center).sqrMagnitude <= field.Radius * field.Radius) return true;
            }
            return false;
        }

        void BuildParticles(float duration)
        {
            ParticleSystem particles = gameObject.AddComponent<ParticleSystem>();
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = particles.main;
            main.playOnAwake = false;
            main.loop = true;
            main.duration = Mathf.Max(0.5f, duration);
            main.startLifetime = new ParticleSystem.MinMaxCurve(1.8f, 3.2f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.04f, 0.18f);
            main.startSize = new ParticleSystem.MinMaxCurve(Radius * 0.34f, Radius * 0.62f);
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(0.16f, 0.21f, 0.28f, 0.85f),
                new Color(0.42f, 0.49f, 0.56f, 0.75f));
            main.maxParticles = 96;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;

            var emission = particles.emission;
            emission.rateOverTime = 24f;
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = Radius * 0.76f;
            shape.position = Vector3.up * Mathf.Max(0.8f, Radius * 0.34f);

            var noise = particles.noise;
            noise.enabled = true;
            noise.strength = new ParticleSystem.MinMaxCurve(0.18f, 0.42f);
            noise.frequency = 0.28f;
            noise.scrollSpeed = 0.18f;

            var color = particles.colorOverLifetime;
            color.enabled = true;
            Gradient fade = new Gradient();
            fade.SetKeys(
                new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(0.68f, 0.75f, 0.80f), 1f) },
                new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.58f, 0.18f), new GradientAlphaKey(0.42f, 0.72f), new GradientAlphaKey(0f, 1f) });
            color.color = fade;

            Shader shader = Resources.Load<Shader>("SmokeCloud");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader != null) smokeMaterial = new Material(shader) { name = "BRX_SoftSmoke" };
            ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = smokeMaterial;
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.sortingFudge = 2f;

            ParticleCount = 64;
            particles.Emit(ParticleCount);
            particles.Play();
        }

        void BuildBoundary()
        {
            GameObject ring = new GameObject("Smoke_Radius");
            ring.transform.SetParent(transform, false);
            LineRenderer line = ring.AddComponent<LineRenderer>();
            Shader shader = Resources.Load<Shader>("ArcaneGlow");
            if (shader != null) ringMaterial = new Material(shader) { name = "BRX_SmokeBoundary" };
            line.sharedMaterial = ringMaterial;
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = 49;
            line.widthMultiplier = 0.08f;
            line.startColor = line.endColor = new Color(0.60f, 0.72f, 0.80f, 0.34f);
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
            for (int i = 0; i < line.positionCount; i++)
            {
                float angle = i * Mathf.PI * 2f / (line.positionCount - 1);
                line.SetPosition(i, new Vector3(Mathf.Cos(angle) * Radius, 0f, Mathf.Sin(angle) * Radius));
            }
        }

        void OnDestroy()
        {
            active.Remove(this);
            if (smokeMaterial != null) Destroy(smokeMaterial);
            if (ringMaterial != null) Destroy(ringMaterial);
        }
    }
}
