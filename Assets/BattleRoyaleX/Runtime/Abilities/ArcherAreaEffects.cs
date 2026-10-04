using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class ArcherProximityTrap : MonoBehaviour
    {
        CharacterRuntime owner;
        AbilityDefinition ability;
        float armedAt;
        float expiresAt;
        bool triggered;

        public void Configure(CharacterRuntime source, AbilityDefinition definition)
        {
            owner = source;
            ability = definition;
            armedAt = Time.time + 0.2f;
            expiresAt = Time.time + Mathf.Max(0.2f, definition.fieldDuration);
        }

        void Update()
        {
            if (triggered || owner == null || owner.Health.IsDead || Time.time >= expiresAt)
            { Destroy(gameObject); return; }
            if (Time.time < armedAt) return;
            foreach (CharacterRuntime target in FindObjectsByType<CharacterRuntime>())
            {
                if (target == null || target == owner || target.TeamId == owner.TeamId ||
                    target.Health == null || target.Health.IsDead) continue;
                Vector3 offset = target.transform.position - transform.position;
                offset.y = 0f;
                if (offset.sqrMagnitude > 1.5f * 1.5f) continue;
                triggered = true;
                gameObject.AddComponent<PullField>().Configure(owner, ability);
                GameObject hitObject = new GameObject("ProximityTrap_Blast");
                hitObject.transform.position = transform.position;
                Hitbox hit = hitObject.AddComponent<Hitbox>();
                hit.Configure(owner, new DamagePacket(owner, ability, offset.normalized),
                    Vector3.one * Mathf.Max(0.5f, ability.fieldRadius * 2f), 0.15f);
                foreach (Collider collider in Physics.OverlapSphere(transform.position, ability.fieldRadius,
                    ~0, QueryTriggerInteraction.Collide)) hit.TryResolveHurtbox(collider.GetComponent<Hurtbox>());
                // Keep the pulling field alive after the one-time detonation.
                enabled = false;
                return;
            }
        }
    }

    public sealed class ArcherArrowRain : MonoBehaviour
    {
        CharacterRuntime owner;
        AbilityDefinition ability;
        float expiresAt;
        float nextPulse;
        int pulse;
        static Material arrowMaterial;

        public void Configure(CharacterRuntime source, AbilityDefinition definition)
        {
            owner = source;
            ability = definition;
            expiresAt = Time.time + Mathf.Max(0.2f, definition.fieldDuration);
            nextPulse = Time.time;
        }

        void Update()
        {
            if (owner == null || owner.Health.IsDead || Time.time >= expiresAt)
            { Destroy(gameObject); return; }
            if (Time.time < nextPulse) return;
            nextPulse = Time.time + 0.35f;
            pulse++;
            float radius = Mathf.Max(0.5f, ability.fieldRadius);
            Vector3 center = transform.position;
            GameObject hitObject = new GameObject("ArrowRain_Pulse");
            hitObject.transform.position = center;
            Hitbox hit = hitObject.AddComponent<Hitbox>();
            hit.Configure(owner, new DamagePacket(owner, ability, owner.Motor.Facing),
                Vector3.one * 0.1f, 0.08f);
            hit.GetComponent<Collider>().enabled = false;
            HashSet<CharacterRuntime> struck = new HashSet<CharacterRuntime>();
            foreach (Collider collider in Physics.OverlapSphere(center, radius, ~0, QueryTriggerInteraction.Collide))
            {
                Hurtbox hurt = collider.GetComponent<Hurtbox>();
                if (hurt != null && hurt.Owner != null && struck.Add(hurt.Owner)) hit.TryResolveHurtbox(hurt);
            }
            for (int i = 0; i < 4; i++)
            {
                float angle = (pulse * 93f + i * 137.5f) * Mathf.Deg2Rad;
                float distance = radius * (i == 0 ? 0.15f : 0.72f);
                GameObject prefab = owner.Definition.basicAttack.visualProfile != null ?
                    owner.Definition.basicAttack.visualProfile.projectilePrefab : null;
                GameObject arrow = prefab != null ? Instantiate(prefab) : GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                arrow.name = "FallingArrow";
                arrow.transform.SetParent(transform, true);
                arrow.transform.position = center + new Vector3(Mathf.Cos(angle) * distance, 5.5f,
                    Mathf.Sin(angle) * distance);
                if (prefab != null) arrow.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
                else arrow.transform.localScale = new Vector3(0.035f, 0.9f, 0.035f);
                Collider c = arrow.GetComponent<Collider>(); if (c != null) Destroy(c);
                if (arrowMaterial == null)
                {
                    Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
                    if (shader != null)
                    {
                        arrowMaterial = new Material(shader);
                        arrowMaterial.SetColor("_BaseColor", new Color(0.85f, 0.94f, 1f, 1f));
                    }
                }
                if (prefab == null && arrowMaterial != null) arrow.GetComponent<Renderer>().sharedMaterial = arrowMaterial;
                arrow.AddComponent<ArcherFallingArrow>();
            }
        }
    }

    public sealed class ArcherFallingArrow : MonoBehaviour
    {
        float bornAt;
        void Awake() => bornAt = Time.time;
        void Update()
        {
            transform.position += Vector3.down * (18f * Time.deltaTime);
            if (Time.time - bornAt > 0.36f) Destroy(gameObject);
        }
    }
}
