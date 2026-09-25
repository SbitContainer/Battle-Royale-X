using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public enum ArcaneDemoKind
    {
        PhaseWall,
        PrismaticWall,
        AmplificationBarrier,
        FragmentCrystal,
        SpeedRune,
        ReactiveBush
    }

    public sealed class ProjectileInteractionState : MonoBehaviour
    {
        public bool reflected;
        public bool amplified;
        public bool fragmented;
    }

    [RequireComponent(typeof(Collider))]
    public sealed class ArcaneDemoObject : MonoBehaviour
    {
        public ArcaneDemoKind kind;
        readonly Dictionary<CharacterRuntime, float> runeReadyAt = new Dictionary<CharacterRuntime, float>();
        Renderer cachedRenderer;
        bool bushHidden;

        public static ArcaneDemoObject Create(string objectName, ArcaneDemoKind demoKind, Vector3 position, Vector3 scale)
        {
            PrimitiveType primitive = demoKind == ArcaneDemoKind.SpeedRune ? PrimitiveType.Cylinder :
                demoKind == ArcaneDemoKind.FragmentCrystal ? PrimitiveType.Sphere : PrimitiveType.Cube;
            GameObject go = GameObject.CreatePrimitive(primitive);
            go.name = objectName;
            go.transform.position = position;
            go.transform.localScale = scale;
            Collider collider = go.GetComponent<Collider>();
            collider.isTrigger = true;
            ArcaneDemoObject result = go.AddComponent<ArcaneDemoObject>();
            result.kind = demoKind;
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                renderer.sharedMaterial = new Material(shader) { color = ColorFor(demoKind) };
            }
            return result;
        }

        static Color ColorFor(ArcaneDemoKind value)
        {
            if (value == ArcaneDemoKind.PhaseWall) return new Color(0.1f, 0.8f, 1f, 0.55f);
            if (value == ArcaneDemoKind.PrismaticWall) return new Color(0.8f, 0.35f, 1f, 0.65f);
            if (value == ArcaneDemoKind.AmplificationBarrier) return new Color(1f, 0.35f, 0.75f, 0.55f);
            if (value == ArcaneDemoKind.FragmentCrystal) return new Color(0.25f, 1f, 0.9f, 1f);
            if (value == ArcaneDemoKind.SpeedRune) return new Color(0.2f, 1f, 0.35f, 0.8f);
            return new Color(0.15f, 0.55f, 0.2f, 1f);
        }

        void Awake() => cachedRenderer = GetComponent<Renderer>();

        void OnTriggerEnter(Collider other)
        {
            CharacterRuntime character = other.GetComponentInParent<CharacterRuntime>();
            if (kind == ArcaneDemoKind.SpeedRune && character != null)
            {
                float ready = runeReadyAt.TryGetValue(character, out float value) ? value : 0f;
                if (Time.time >= ready)
                {
                    runeReadyAt[character] = Time.time + 6f;
                    character.ApplyMovementSpeedBonus(1.15f, 2.5f);
                }
                return;
            }

            Hitbox hitbox = other.GetComponent<Hitbox>();
            if (hitbox == null || hitbox.Packet.ability == null) return;
            bool projectile = other.GetComponent<ProjectileHitboxMover>() != null ||
                other.GetComponent<SeekingProjectileMover>() != null;
            ProjectileInteractionState state = other.GetComponent<ProjectileInteractionState>() ??
                other.gameObject.AddComponent<ProjectileInteractionState>();

            if (kind == ArcaneDemoKind.PhaseWall && projectile)
            {
                hitbox.Cancel();
                Destroy(other.gameObject);
            }
            else if (kind == ArcaneDemoKind.PrismaticWall && projectile && hitbox.Packet.reflectable && !state.reflected)
            {
                state.reflected = true;
                ProjectileHitboxMover mover = other.GetComponent<ProjectileHitboxMover>();
                if (mover != null) mover.direction = Vector3.Reflect(mover.direction, transform.forward).normalized;
                other.transform.forward = Vector3.Reflect(other.transform.forward, transform.forward).normalized;
            }
            else if (kind == ArcaneDemoKind.AmplificationBarrier && projectile &&
                (hitbox.Packet.ability.amplifiable || (hitbox.Packet.ability.tags & AbilityTags.Amplifiable) != 0) && !state.amplified)
            {
                state.amplified = true;
                DamagePacket packet = hitbox.Packet;
                packet.damage *= 1.12f;
                hitbox.UpdatePacket(packet);
            }
            else if (kind == ArcaneDemoKind.FragmentCrystal && !state.fragmented &&
                (hitbox.Packet.ability.fragmentTrigger || (hitbox.Packet.ability.tags & AbilityTags.FragmentTrigger) != 0))
            {
                state.fragmented = true;
                for (int i = 0; i < 8; i++)
                {
                    Vector3 direction = Quaternion.Euler(0f, i * 45f, 0f) * Vector3.forward;
                    hitbox.Owner.Abilities.SpawnConvergenceProjectile(hitbox.Packet.ability, direction, 0.22f, 0.75f);
                }
                hitbox.Cancel();
                Destroy(other.gameObject);
            }
            else if (kind == ArcaneDemoKind.ReactiveBush && !bushHidden &&
                (hitbox.Packet.ability.heavy || (hitbox.Packet.ability.tags & (AbilityTags.Heavy | AbilityTags.Area)) != 0))
            {
                StartCoroutine(HideBush());
            }
        }

        IEnumerator HideBush()
        {
            bushHidden = true;
            if (cachedRenderer != null) cachedRenderer.enabled = false;
            yield return new WaitForSeconds(18f);
            if (cachedRenderer != null) cachedRenderer.enabled = true;
            bushHidden = false;
        }
    }
}
