using UnityEngine;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed class MageConvergenceController : MonoBehaviour
    {
        CharacterRuntime runtime;
        Hitbox slowProjectile;
        Hitbox fastProjectile;
        float opensAt;
        float expiresAt;
        bool secondFired;

        public AbilityDefinition Ability { get; private set; }
        public bool CanFireSecond => Ability != null && !secondFired && Time.time >= opensAt && Time.time <= expiresAt;

        public void Begin(CharacterRuntime source, AbilityDefinition ability)
        {
            Cancel();
            runtime = source;
            Ability = ability;
            opensAt = Time.time + Mathf.Max(0f, ability.secondActivationDelay);
            expiresAt = opensAt + Mathf.Max(0.25f, ability.secondActivationWindow);
            slowProjectile = runtime.Abilities.SpawnConvergenceProjectile(ability, runtime.Motor.Facing, 0.22f, 0.42f);
        }

        public bool TryFireSecond(Vector3 direction)
        {
            if (!CanFireSecond || runtime == null) return false;
            secondFired = true;
            fastProjectile = runtime.Abilities.SpawnConvergenceProjectile(Ability, direction, 0.22f, 1.65f);
            if (fastProjectile == null) { Fail(); return false; }
            ConvergenceProjectileLink link = fastProjectile.gameObject.AddComponent<ConvergenceProjectileLink>();
            link.Configure(this, slowProjectile);
            expiresAt = Time.time + Mathf.Max(0.5f, Ability.secondActivationWindow);
            return true;
        }

        void Update()
        {
            if (Ability == null) return;
            if (secondFired && (fastProjectile == null || slowProjectile == null)) { Fail(); return; }
            if (Time.time > expiresAt) Fail();
        }

        internal void CompleteCombo(Vector3 position)
        {
            if (Ability == null || runtime == null) return;
            AbilityDefinition completed = Ability;
            CombatResolver.ResolveAreaEffect(position, Mathf.Max(1f, completed.explosionRadius),
                completed.damage, runtime, completed);
            runtime.Abilities.SetCooldownOverride(AbilitySlot.Ultimate,
                Mathf.Max(completed.cooldown, completed.comboSuccessCooldown));
            CombatEvents.Raise(new CombatEventData(CombatEventKind.Clash, position, runtime, null,
                completed.damage, completed, AbilityPhase.Completed, direction: runtime.Motor.Facing));
            Cancel();
        }

        void Fail()
        {
            if (Ability != null && runtime != null)
                runtime.Abilities.SetCooldownOverride(AbilitySlot.Ultimate,
                    Mathf.Max(0f, Ability.comboFailureCooldown));
            Cancel();
        }

        public void Cancel()
        {
            if (slowProjectile != null) Destroy(slowProjectile.gameObject);
            if (fastProjectile != null) Destroy(fastProjectile.gameObject);
            slowProjectile = fastProjectile = null;
            Ability = null;
            secondFired = false;
            opensAt = expiresAt = 0f;
        }
    }

    public sealed class ConvergenceProjectileLink : MonoBehaviour
    {
        MageConvergenceController controller;
        Hitbox slow;

        public void Configure(MageConvergenceController source, Hitbox slowProjectile)
        {
            controller = source;
            slow = slowProjectile;
        }

        void Update()
        {
            if (controller == null || slow == null) return;
            if ((slow.transform.position - transform.position).sqrMagnitude > 0.75f * 0.75f) return;
            controller.CompleteCombo((slow.transform.position + transform.position) * 0.5f);
        }
    }
}
