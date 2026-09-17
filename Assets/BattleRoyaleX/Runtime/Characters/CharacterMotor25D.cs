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
        Vector3 facing = Vector3.forward;
        bool dashing;

        public Vector3 Facing => facing;
        public bool IsDashing => dashing;

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

        void Update()
        {
            if (runtime == null || runtime.State == null || runtime.State.InputLocked || dashing) return;

            float speed = runtime.Definition != null ? runtime.Definition.moveSpeed : 5f;
            speed *= runtime.Modifiers.moveSpeedMultiplier;
            controller.Move(movementInput * speed * Time.deltaTime);

            if (movementInput.sqrMagnitude > 0.0025f)
            {
                facing = movementInput.normalized;
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

        public void Dash(float distance, float duration, bool passThroughCharacters, float invulnerableTime)
        {
            if (!dashing) StartCoroutine(DashRoutine(distance, duration, passThroughCharacters, invulnerableTime));
        }

        IEnumerator DashRoutine(float distance, float duration, bool passThroughCharacters, float invulnerableTime)
        {
            dashing = true;
            if (invulnerableTime > 0f) runtime.State.SetInvulnerable(invulnerableTime);

            Vector3 start = transform.position;
            Vector3 target = start + facing * distance;
            float elapsed = 0f;

            CharacterController[] ignoredControllers = null;
            if (passThroughCharacters)
            {
                ignoredControllers = FindObjectsOfType<CharacterController>();
                foreach (CharacterController other in ignoredControllers)
                {
                    if (other != null && other != controller) Physics.IgnoreCollision(controller, other, true);
                }
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 desired = Vector3.Lerp(start, target, t);
                Vector3 delta = desired - transform.position;
                controller.Move(delta);
                yield return null;
            }

            if (ignoredControllers != null)
            {
                foreach (CharacterController other in ignoredControllers)
                {
                    if (other != null && other != controller) Physics.IgnoreCollision(controller, other, false);
                }
            }
            dashing = false;
        }

        public void Teleport(Vector3 worldPosition)
        {
            controller.enabled = false;
            transform.position = worldPosition;
            controller.enabled = true;
        }

        public void ApplyImpulse(Vector3 direction, float distance, float duration = 0.12f)
        {
            if (direction.sqrMagnitude < 0.001f || distance <= 0f) return;
            StartCoroutine(ImpulseRoutine(direction.normalized, distance, duration));
        }

        IEnumerator ImpulseRoutine(Vector3 direction, float distance, float duration)
        {
            Vector3 start = transform.position;
            Vector3 end = start + direction * distance;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                Vector3 desired = Vector3.Lerp(start, end, Mathf.Clamp01(elapsed / duration));
                controller.Move(desired - transform.position);
                yield return null;
            }
        }
    }
}
