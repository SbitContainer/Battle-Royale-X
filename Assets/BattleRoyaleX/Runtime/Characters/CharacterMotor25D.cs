using System.Collections;
using UnityEngine;

namespace BattleRoyaleX
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class CharacterMotor25D : MonoBehaviour
    {
        CharacterController controller;
        CharacterRuntime runtime;
        Vector3 movementInput;
        Vector3 smoothedMovement;
        Vector3 facing = Vector3.forward;
        bool dashing;
        Coroutine dashRoutine;
        CharacterController[] ignoredDuringDash;
        Vector3 dashDirection;
        float dashRemaining;
        float dashSpeed;
        Coroutine impulseRoutine;

        [Header("Responsive movement")]
        [Tooltip("How quickly the character reaches a new analogue-stick direction. Lower values feel heavier.")]
        [Range(6f, 32f)] public float acceleration = 19f;
        [Range(6f, 32f)] public float deceleration = 24f;

        public Vector3 Facing => facing;
        public bool IsDashing => dashing;
        public float MovementAmount => smoothedMovement.magnitude;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            runtime = GetComponent<CharacterRuntime>();
            facing = transform.forward;
            facing.y = 0f;
            if (facing.sqrMagnitude < 0.001f) facing = Vector3.forward;
            facing.Normalize();
        }

        public void SetMoveInput(Vector2 input)
        {
            movementInput = new Vector3(input.x, 0f, input.y);
            if (movementInput.sqrMagnitude > 1f) movementInput.Normalize();
        }

        // Used when an external state takes full control (death, scripted reset or a root-motion check).
        // Normal thumb-stick release intentionally keeps the short deceleration in Update.
        public void StopMovementImmediately()
        {
            movementInput = Vector3.zero;
            smoothedMovement = Vector3.zero;
        }

        void Update()
        {
            if (runtime == null || runtime.State == null || runtime.State.MovementLocked || dashing) return;

            float speed = runtime.Definition != null ? runtime.Definition.moveSpeed : 5f;
            speed *= runtime.Modifiers.moveSpeedMultiplier * runtime.MovementSpeedBonus * runtime.MovementSlowMultiplier;
            float response = movementInput.sqrMagnitude > 0.001f ? acceleration : deceleration;
            float blend = 1f - Mathf.Exp(-response * Time.deltaTime);
            smoothedMovement = Vector3.Lerp(smoothedMovement, movementInput, blend);
            if (smoothedMovement.sqrMagnitude < 0.0001f) smoothedMovement = Vector3.zero;
            controller.Move(smoothedMovement * speed * Time.deltaTime);

            if (smoothedMovement.sqrMagnitude > 0.0025f && !runtime.Abilities.IsActionBusy &&
                !runtime.Abilities.IsComboInProgress && !runtime.Defense.IsActive)
            {
                facing = smoothedMovement.normalized;
                float rotSpeed = runtime.Definition != null ? runtime.Definition.rotationSpeed : 900f;
                Quaternion target = Quaternion.LookRotation(facing, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotSpeed * Time.deltaTime);
            }
        }

        public void FaceDirection(Vector3 direction)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) return;
            facing = direction.normalized;
            transform.rotation = Quaternion.LookRotation(facing, Vector3.up);
        }

        public void Dash(float distance, float duration, bool passThroughCharacters, float invulnerableTime,
            System.Action<Vector3, Vector3> onTravel = null, System.Func<Vector3> destination = null)
        {
            if (!dashing) dashRoutine = StartCoroutine(DashRoutine(distance, duration, passThroughCharacters, invulnerableTime, onTravel, destination));
        }

        IEnumerator DashRoutine(float distance, float duration, bool passThroughCharacters, float invulnerableTime,
            System.Action<Vector3, Vector3> onTravel, System.Func<Vector3> destination)
        {
            dashing = true;
            smoothedMovement = Vector3.zero;
            if (invulnerableTime > 0f) runtime.State.SetInvulnerable(invulnerableTime);

            dashDirection = facing;
            dashRemaining = distance;
            dashSpeed = distance / Mathf.Max(0.01f, duration);
            float pursuitDeadline = Time.time + duration;

            if (passThroughCharacters)
            {
                ignoredDuringDash = FindObjectsByType<CharacterController>();
                foreach (CharacterController other in ignoredDuringDash)
                {
                    if (other != null && other != controller) Physics.IgnoreCollision(controller, other, true);
                }
            }

            try
            {
                while (dashRemaining > 0.0001f && !runtime.Health.IsDead && controller.enabled &&
                    (destination == null || Time.time < pursuitDeadline))
                {
                    float step = Mathf.Min(dashRemaining, dashSpeed * Time.deltaTime);
                    if (destination != null)
                    {
                        Vector3 delta = destination() - transform.position;
                        delta.y = 0f;
                        if (delta.magnitude < 0.08f) break;
                        FaceDirection(delta);
                        dashDirection = facing;
                        step = Mathf.Min(step, delta.magnitude);
                    }
                    dashRemaining -= step;
                    Vector3 previous = transform.position;
                    controller.Move(dashDirection * step);
                    onTravel?.Invoke(previous, transform.position);
                    yield return null;
                }
            }
            finally
            {
                RestoreIgnoredCollisions();
                dashing = false;
                dashRoutine = null;
            }
        }

        // Redirect the SAME journey and hit registry. No second dash or extended invulnerability.
        public bool RedirectDash(Vector3 direction, float distance, float duration)
        {
            direction.y = 0f;
            if (!dashing || direction.sqrMagnitude < 0.001f) return false;
            FaceDirection(direction);
            dashDirection = facing;
            dashRemaining = Mathf.Max(0f, distance);
            dashSpeed = dashRemaining / Mathf.Max(0.05f, duration);
            return true;
        }

        public void CancelDash()
        {
            if (dashRoutine != null) StopCoroutine(dashRoutine);
            dashRoutine = null;
            RestoreIgnoredCollisions();
            dashing = false;
            smoothedMovement = Vector3.zero;
        }

        void RestoreIgnoredCollisions()
        {
            if (ignoredDuringDash == null) return;
            foreach (CharacterController other in ignoredDuringDash)
                if (other != null && other != controller && controller != null)
                    Physics.IgnoreCollision(controller, other, false);
            ignoredDuringDash = null;
        }

        public void Teleport(Vector3 worldPosition)
        {
            controller.enabled = false;
            transform.position = worldPosition;
            controller.enabled = true;
            smoothedMovement = Vector3.zero;
        }

        public void ApplyImpulse(Vector3 direction, float distance, float duration = 0.12f)
        {
            if (direction.sqrMagnitude < 0.001f || distance <= 0f) return;
            direction.y = 0f;
            if (impulseRoutine != null) StopCoroutine(impulseRoutine);
            impulseRoutine = StartCoroutine(ImpulseRoutine(direction.normalized, distance, Mathf.Max(0.02f, duration)));
        }

        IEnumerator ImpulseRoutine(Vector3 direction, float distance, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration && controller.enabled && !runtime.Health.IsDead)
            {
                float dt = Mathf.Min(Time.deltaTime, duration - elapsed);
                elapsed += dt;
                // Additive displacement lets the player steer or defend during recoil, including against walls.
                controller.Move(direction * (distance * dt / duration));
                yield return null;
            }
            impulseRoutine = null;
        }

        public void ResetTransientState()
        {
            CancelDash();
            if (impulseRoutine != null) StopCoroutine(impulseRoutine);
            impulseRoutine = null;
            StopMovementImmediately();
        }
    }
}
