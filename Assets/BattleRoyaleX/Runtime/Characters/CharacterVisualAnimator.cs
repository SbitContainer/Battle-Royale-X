using UnityEngine;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed class CharacterVisualAnimator : MonoBehaviour
    {
        static readonly int MoveAmountHash = Animator.StringToHash("MoveAmount");
        static readonly int Attack1Hash = Animator.StringToHash("Attack1");
        static readonly int Attack2Hash = Animator.StringToHash("Attack2");
        static readonly int Attack3Hash = Animator.StringToHash("Attack3");
        static readonly int DefenseHash = Animator.StringToHash("Defense");
        static readonly int MoveAssassinHash = Animator.StringToHash("MoveAssassin");
        static readonly int MoveWarriorHash = Animator.StringToHash("MoveWarrior");
        static readonly int UltimateDashHash = Animator.StringToHash("UltimateDash");
        static readonly int UltimateAssassinHash = Animator.StringToHash("UltimateAssassin");
        static readonly int UltimateWarriorHash = Animator.StringToHash("UltimateWarrior");
        static readonly int HitHash = Animator.StringToHash("Hit");
        static readonly int DeadHash = Animator.StringToHash("Dead");

        Animator animator;
        CharacterRuntime runtime;
        bool suppressTrainingDeath;

        void Awake()
        {
            runtime = GetComponentInParent<CharacterRuntime>();
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>(true);
            if (animator != null) animator.applyRootMotion = false;
        }

        void Start() => suppressTrainingDeath = Application.isMobilePlatform && FindAnyObjectByType<PrototypeTrainingBot>() != null;
        void OnEnable() => CombatEvents.Raised += OnCombatEvent;
        void OnDisable() => CombatEvents.Raised -= OnCombatEvent;

        void Update()
        {
            if (animator == null || runtime == null) return;
            float moveAmount = runtime.Motor != null ? runtime.Motor.MovementAmount : 0f;
            animator.SetFloat(MoveAmountHash, moveAmount, 0.08f, Time.deltaTime);
            animator.SetBool(DeadHash, !suppressTrainingDeath && runtime.Health != null && runtime.Health.IsDead);
        }

        public void PlayBasicStep(int step)
        {
            if (animator == null) return;
            animator.SetTrigger(step <= 1 ? Attack1Hash : step == 2 ? Attack2Hash : Attack3Hash);
        }

        public void PlayChargedDash()
        {
            if (animator != null) animator.SetTrigger(UltimateDashHash);
        }

        public void PlayAbility(AbilityDefinition ability)
        {
            if (animator == null || ability == null) return;
            bool assassin = runtime != null && runtime.Definition != null && runtime.Definition.characterClass == CharacterClass.Assassin;
            switch (ability.behavior)
            {
                case AbilityBehavior.MeleeAttack:
                case AbilityBehavior.ProjectileAttack:
                case AbilityBehavior.AreaAttack:
                    PlayBasicStep(1);
                    break;
                case AbilityBehavior.Guard:
                case AbilityBehavior.Parry:
                case AbilityBehavior.SmokeEscape:
                    animator.SetTrigger(DefenseHash);
                    break;
                case AbilityBehavior.Dodge:
                case AbilityBehavior.Dash:
                case AbilityBehavior.DashThrough:
                case AbilityBehavior.DashReturn:
                    animator.SetTrigger(assassin ? MoveAssassinHash : MoveWarriorHash);
                    break;
                case AbilityBehavior.ChargedDashSequence:
                    animator.SetTrigger(UltimateDashHash);
                    break;
                case AbilityBehavior.UltimateBuff:
                    animator.SetTrigger(assassin ? UltimateAssassinHash : UltimateWarriorHash);
                    break;
            }
        }

        void OnCombatEvent(CombatEventData data)
        {
            if (animator == null || runtime == null) return;
            if (data.kind == CombatEventKind.DefenseRedirect && data.source == runtime)
            {
                animator.SetTrigger(MoveAssassinHash);
                return;
            }
            if (data.kind == CombatEventKind.Hit && data.target == runtime)
            {
                if (runtime.Motor == null || !runtime.Motor.IsDashing) animator.SetTrigger(HitHash);
                return;
            }
            if ((data.kind == CombatEventKind.Block && data.target == runtime) ||
                (data.kind == CombatEventKind.Parry && data.source == runtime))
                animator.SetTrigger(DefenseHash);
        }
    }
}
