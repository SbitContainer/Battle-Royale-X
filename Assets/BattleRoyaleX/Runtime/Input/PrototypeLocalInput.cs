using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class PrototypeLocalInput : MonoBehaviour
    {
        public KeyCode up = KeyCode.W;
        public KeyCode down = KeyCode.S;
        public KeyCode left = KeyCode.A;
        public KeyCode right = KeyCode.D;
        public KeyCode basicAttack = KeyCode.F;
        public KeyCode defense = KeyCode.G;
        public KeyCode movement = KeyCode.H;
        public KeyCode ultimate = KeyCode.R;
        public KeyCode inventory1 = KeyCode.Alpha1;
        public KeyCode inventory2 = KeyCode.Alpha2;
        public KeyCode inventory3 = KeyCode.Alpha3;
        public KeyCode inventory4 = KeyCode.Alpha4;

        CharacterRuntime runtime;
        bool basicBlockedUntilRelease;

        void Awake() => runtime = GetComponent<CharacterRuntime>();

        void Update()
        {
            if (runtime == null || runtime.Health.IsDead) return;
            if (!Input.GetKey(basicAttack)) basicBlockedUntilRelease = false;
            if (Time.timeScale <= 0f)
            { basicBlockedUntilRelease = true; runtime.Abilities.ReleaseBasicAttackInput(); return; }
            if (Input.GetKeyUp(basicAttack)) runtime.Abilities.ReleaseBasicAttackInput();
            Vector2 move = Vector2.zero;
            if (Input.GetKey(left)) move.x -= 1f;
            if (Input.GetKey(right)) move.x += 1f;
            if (Input.GetKey(down)) move.y -= 1f;
            if (Input.GetKey(up)) move.y += 1f;
            runtime.Motor.SetMoveInput(move.normalized);

            if (Input.GetKeyDown(defense)) UseAimed(AbilitySlot.Skill1);
            if (Input.GetKeyDown(movement)) UseAimed(AbilitySlot.Skill2);
            if (Input.GetKeyDown(ultimate)) UseAimed(AbilitySlot.Ultimate);
            if (Time.timeScale > 0f && !basicBlockedUntilRelease && Input.GetKey(basicAttack) && !Input.GetKeyDown(defense) &&
                !Input.GetKeyDown(movement) && !Input.GetKeyDown(ultimate)) UseAimed(AbilitySlot.BasicAttack);
            if (Input.GetKeyDown(inventory1)) runtime.Inventory.UseSlot(0);
            if (Input.GetKeyDown(inventory2)) runtime.Inventory.UseSlot(1);
            if (Input.GetKeyDown(inventory3)) runtime.Inventory.UseSlot(2);
            if (Input.GetKeyDown(inventory4)) runtime.Inventory.UseSlot(3);
        }

        void UseAimed(AbilitySlot slot)
        {
            if (slot != AbilitySlot.BasicAttack)
            { basicBlockedUntilRelease = true; runtime.Abilities.ReleaseBasicAttackInput(); }
            if (runtime.Definition == null || (runtime.Definition.characterClass != CharacterClass.Archer &&
                runtime.Definition.characterClass != CharacterClass.Mage) || Camera.main == null)
            { runtime.Abilities.TryUse(slot); return; }
            Plane ground = new Plane(Vector3.up, runtime.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!ground.Raycast(ray, out float distance)) { runtime.Abilities.TryUse(slot); return; }
            Vector3 direction = ray.GetPoint(distance) - runtime.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f) runtime.Abilities.TryUse(slot, direction);
            else runtime.Abilities.TryUse(slot);
        }

        void OnDisable()
        { basicBlockedUntilRelease = true; if (runtime != null && runtime.Abilities != null) runtime.Abilities.ReleaseBasicAttackInput(); }
        void OnApplicationFocus(bool focused) { if (!focused) OnDisable(); }

        public void ConfigurePlayerTwo()
        {
            up = KeyCode.UpArrow;
            down = KeyCode.DownArrow;
            left = KeyCode.LeftArrow;
            right = KeyCode.RightArrow;
            basicAttack = KeyCode.Keypad1;
            defense = KeyCode.Keypad2;
            movement = KeyCode.Keypad3;
            ultimate = KeyCode.Keypad0;
            inventory1 = KeyCode.Keypad4;
            inventory2 = KeyCode.Keypad5;
            inventory3 = KeyCode.Keypad6;
            inventory4 = KeyCode.Keypad7;
        }
    }
}
