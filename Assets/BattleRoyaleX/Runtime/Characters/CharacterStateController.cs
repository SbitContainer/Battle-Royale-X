using System.Collections;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class CharacterStateController : MonoBehaviour
    {
        public bool IsInvulnerable { get; private set; }
        public bool IsStaggered { get; private set; }
        public bool InputLocked => manualLockCount > 0;
        public bool MovementLocked => manualLockCount > 0 || Time.time < movementLockedUntil;
        public bool SkillsLocked => manualLockCount > 0 || Time.time < skillsLockedUntil;
        public bool BasicAttackLocked => manualLockCount > 0 || Time.time < basicAttackLockedUntil;
        public bool IsInActionRecovery { get; private set; }

        int manualLockCount;
        Coroutine invulnerableRoutine;
        Coroutine staggerRoutine;
        float invulnerableUntil;
        float movementLockedUntil;
        float skillsLockedUntil;
        float basicAttackLockedUntil;

        public void ResetTransientState()
        {
            StopAllCoroutines();
            invulnerableRoutine = staggerRoutine = null;
            IsInvulnerable = IsStaggered = false;
            manualLockCount = 0;
            invulnerableUntil = 0f;
            movementLockedUntil = skillsLockedUntil = basicAttackLockedUntil = 0f;
            IsInActionRecovery = false;
        }

        public void SetInvulnerable(float duration)
        {
            if (duration <= 0f) return;
            invulnerableUntil = Mathf.Max(invulnerableUntil, Time.time + duration);
            if (invulnerableRoutine != null) StopCoroutine(invulnerableRoutine);
            invulnerableRoutine = StartCoroutine(Invulnerability(invulnerableUntil - Time.time));
        }

        IEnumerator Invulnerability(float duration)
        {
            IsInvulnerable = true;
            yield return new WaitForSeconds(duration);
            IsInvulnerable = false;
            invulnerableRoutine = null;
        }

        public void ApplyStagger(float duration)
        {
            if (duration <= 0f) return;
            if (staggerRoutine != null) StopCoroutine(staggerRoutine);
            staggerRoutine = StartCoroutine(Stagger(duration));
        }

        public void ApplyMovementLock(float duration) => movementLockedUntil = Mathf.Max(movementLockedUntil, Time.time + Mathf.Max(0f, duration));
        public void ApplySkillLock(float duration) => skillsLockedUntil = Mathf.Max(skillsLockedUntil, Time.time + Mathf.Max(0f, duration));
        public void ApplyBasicAttackLock(float duration) => basicAttackLockedUntil = Mathf.Max(basicAttackLockedUntil, Time.time + Mathf.Max(0f, duration));
        public void SetActionRecovery(bool active) => IsInActionRecovery = active;

        IEnumerator Stagger(float duration)
        {
            IsStaggered = true;
            yield return new WaitForSeconds(duration);
            IsStaggered = false;
            staggerRoutine = null;
        }

        public void LockInput(bool locked)
        {
            if (locked) manualLockCount++;
            else manualLockCount = Mathf.Max(0, manualLockCount - 1);
        }
    }
}
