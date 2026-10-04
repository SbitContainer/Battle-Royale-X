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
        float bowPoseUntil, bowReleaseAt;
        float warriorChargeStarted, warriorChargeUntil;
        int warriorChargeReset;
        void Awake()
        {
            runtime = GetComponentInParent<CharacterRuntime>();
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>(true);
            if (animator != null) animator.applyRootMotion = false;
        }

        void OnEnable() => CombatEvents.Raised += OnCombatEvent;
        void OnDisable() => CombatEvents.Raised -= OnCombatEvent;

        void Update()
        {
            if (animator == null || runtime == null) return;
            float moveAmount = runtime.Motor != null ? runtime.Motor.MovementAmount : 0f;
            animator.SetFloat(MoveAmountHash, moveAmount, 0.08f, Time.deltaTime);
            animator.SetBool(DeadHash, runtime.Health != null && runtime.Health.IsDead);
        }

        public void PlayBasicStep(int step)
        {
            warriorChargeUntil=0f;
            if (animator == null) return;
            if (IsArcher) { bowReleaseAt=Time.time+0.12f; bowPoseUntil=Time.time+0.4f; return; }
            animator.SetTrigger(step <= 1 ? Attack1Hash : step == 2 ? Attack2Hash : Attack3Hash);
        }

        bool IsArcher => runtime != null && runtime.Definition != null && runtime.Definition.characterClass == CharacterClass.Archer;

        void LateUpdate()
        {
            if (animator != null && animator.isHuman && runtime != null && !runtime.Health.IsDead &&
                Time.time < warriorChargeUntil && runtime.Abilities.IsActionBusy && runtime.Abilities.ResetVersion == warriorChargeReset)
            {
                Transform chestBone=animator.GetBoneTransform(HumanBodyBones.Chest);
                if(chestBone!=null)
                {
                    float charge=Mathf.Clamp01((Time.time-warriorChargeStarted)/Mathf.Max(.05f,warriorChargeUntil-warriorChargeStarted));
                    Vector3 facing=runtime.transform.forward, chargeSide=runtime.transform.right;
                    Vector3 focus=chestBone.position+facing*.4f+Vector3.up*Mathf.Lerp(.03f,.3f,charge);
                    PoseArm(HumanBodyBones.LeftUpperArm,HumanBodyBones.LeftLowerArm,HumanBodyBones.LeftHand,focus-chargeSide*.2f,-chargeSide);
                    PoseArm(HumanBodyBones.RightUpperArm,HumanBodyBones.RightLowerArm,HumanBodyBones.RightHand,focus+chargeSide*.2f,chargeSide);
                }
                return;
            }
            if (!IsArcher || animator == null || !animator.isHuman || Time.time >= bowPoseUntil) return;
            Transform left = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            Transform right = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            if (left == null || right == null) return;
            Vector3 forward = runtime.transform.forward, side = runtime.transform.right;
            Vector3 chest = (left.position+right.position)*0.5f;
            float release=Mathf.Clamp01((Time.time-bowReleaseAt)/0.12f);
            PoseArm(HumanBodyBones.LeftUpperArm,HumanBodyBones.LeftLowerArm,HumanBodyBones.LeftHand,
                chest+forward*0.59f-side*0.15f-Vector3.up*0.05f,-side);
            PoseArm(HumanBodyBones.RightUpperArm,HumanBodyBones.RightLowerArm,HumanBodyBones.RightHand,
                chest+forward*Mathf.Lerp(0.06f,0.38f,release)+side*0.13f,side);
        }

        void PoseArm(HumanBodyBones upperId,HumanBodyBones lowerId,HumanBodyBones handId,Vector3 target,Vector3 bend)
        {
            Transform upper=animator.GetBoneTransform(upperId), lower=animator.GetBoneTransform(lowerId), hand=animator.GetBoneTransform(handId);
            if(upper==null || lower==null || hand==null) return;
            float a=Vector3.Distance(upper.position,lower.position), b=Vector3.Distance(lower.position,hand.position);
            Vector3 delta=target-upper.position; float length=Mathf.Clamp(delta.magnitude,0.01f,a+b-0.005f);
            Vector3 direction=delta.normalized;
            float along=(a*a-b*b+length*length)/(2*length);
            Vector3 perpendicular=Vector3.ProjectOnPlane(bend,direction).normalized;
            Vector3 elbow=upper.position+direction*along+perpendicular*Mathf.Sqrt(Mathf.Max(0,a*a-along*along));
            upper.rotation=Quaternion.FromToRotation(lower.position-upper.position,elbow-upper.position)*upper.rotation;
            lower.rotation=Quaternion.FromToRotation(hand.position-lower.position,target-lower.position)*lower.rotation;
        }

        public void PlayChargedDash()
        {
            if (animator != null) animator.SetTrigger(UltimateDashHash);
        }

        public void PlayAbility(AbilityDefinition ability)
        {
            if (animator == null || ability == null) return;
            warriorChargeUntil=0f;
            if (ability.behavior == AbilityBehavior.WarriorGroundBlast || ability.behavior == AbilityBehavior.WarriorGroundField ||
                ability.behavior == AbilityBehavior.WarriorGroundWaves)
            {
                warriorChargeStarted=Time.time;warriorChargeUntil=Time.time+ability.startup;
                warriorChargeReset=runtime != null ? runtime.Abilities.ResetVersion : 0;
            }
            if (IsArcher && (ability.attackKind == AttackKind.Projectile || ability.behavior == AbilityBehavior.ArrowRain))
            { bowReleaseAt=Time.time+ability.startup; bowPoseUntil=bowReleaseAt+0.3f; return; }
            bool assassin = runtime != null && runtime.Definition != null && runtime.Definition.characterClass == CharacterClass.Assassin;
            switch (ability.behavior)
            {
                case AbilityBehavior.MeleeAttack:
                case AbilityBehavior.ProjectileAttack:
                case AbilityBehavior.AreaAttack:
                case AbilityBehavior.SeekingProjectile:
                case AbilityBehavior.MultiShot:
                case AbilityBehavior.ComboProjectileUltimate:
                case AbilityBehavior.SlowField:
                case AbilityBehavior.PullTrap:
                case AbilityBehavior.Repulsion:
                case AbilityBehavior.ArrowRain:
                case AbilityBehavior.DaggerTeleport:
                case AbilityBehavior.WarriorShieldCharge:
                    PlayBasicStep(1);
                    break;
                case AbilityBehavior.Guard:
                case AbilityBehavior.Parry:
                case AbilityBehavior.SmokeEscape:
                case AbilityBehavior.OrbitingDaggers:
                case AbilityBehavior.WarriorFortress:
                case AbilityBehavior.WarriorSkillCapture:
                    animator.SetTrigger(DefenseHash);
                    break;
                case AbilityBehavior.Dodge:
                case AbilityBehavior.Dash:
                case AbilityBehavior.DashThrough:
                case AbilityBehavior.DashReturn:
                case AbilityBehavior.Blink:
                case AbilityBehavior.CloneTeleport:
                case AbilityBehavior.MultiDash:
                case AbilityBehavior.Grapple:
                case AbilityBehavior.ExecutionStrike:
                case AbilityBehavior.WarriorPursuitStrike:
                case AbilityBehavior.WarriorPursuitLong:
                    animator.SetTrigger(assassin ? MoveAssassinHash : MoveWarriorHash);
                    break;
                case AbilityBehavior.ChargedDashSequence:
                    animator.SetTrigger(UltimateDashHash);
                    break;
                case AbilityBehavior.UltimateBuff:
                case AbilityBehavior.TimedBuff:
                case AbilityBehavior.WarriorGroundBlast:
                case AbilityBehavior.WarriorGroundField:
                case AbilityBehavior.WarriorGroundWaves:
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
