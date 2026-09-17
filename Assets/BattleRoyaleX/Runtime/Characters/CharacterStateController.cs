using System.Collections;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class CharacterStateController : MonoBehaviour
    {
        public bool IsInvulnerable { get; private set; }
        public bool IsStaggered { get; private set; }
        public bool InputLocked => IsStaggered || manualLockCount > 0;

        int manualLockCount;
        Coroutine invulnerableRoutine;
        Coroutine staggerRoutine;

        public void SetInvulnerable(float duration)
        {
            if (duration <= 0f) return;
            if (invulnerableRoutine != null) StopCoroutine(invulnerableRoutine);
            invulnerableRoutine = StartCoroutine(Invulnerability(duration));
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
