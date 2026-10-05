using UnityEngine;

namespace BattleRoyaleX
{
    // Follows the authoritative actor, without moving it or resolving hits.
    public sealed class AssassinVisualAttachment : MonoBehaviour
    {
        CharacterRuntime owner;
        Renderer[] renderers;
        bool[] original;
        public void Initialize(CharacterRuntime actor)
        {
            owner = actor; renderers = GetComponentsInChildren<Renderer>();
            original = new bool[renderers.Length];
            for (int i = 0; i < renderers.Length; i++) original[i] = renderers[i].enabled;
        }
        void LateUpdate()
        {
            if (owner == null || owner.Health.IsDead) { Destroy(gameObject); return; }
            transform.position = owner.transform.position;
            transform.rotation = Quaternion.LookRotation(owner.Motor.Facing);
            bool show = SmokeVisibility.LocalPlayer == null || SmokeVisibility.LocalPlayer == owner ||
                (!owner.Abilities.IsExecutionHidden && !SmokeField.Contains(owner.transform.position));
            for (int i = 0; i < renderers.Length; i++) if (renderers[i] != null)
                renderers[i].enabled = show && original[i];
        }
    }
}
