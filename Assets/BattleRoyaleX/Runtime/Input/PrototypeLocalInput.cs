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

        void Awake() => runtime = GetComponent<CharacterRuntime>();

        void Update()
        {
            if (runtime == null || runtime.Health.IsDead) return;
            Vector2 move = Vector2.zero;
            if (Input.GetKey(left)) move.x -= 1f;
            if (Input.GetKey(right)) move.x += 1f;
            if (Input.GetKey(down)) move.y -= 1f;
            if (Input.GetKey(up)) move.y += 1f;
            runtime.Motor.SetMoveInput(move.normalized);

            if (Input.GetKeyDown(basicAttack)) runtime.Abilities.TryUse(AbilitySlot.BasicAttack);
            if (Input.GetKeyDown(defense)) runtime.Abilities.TryUse(AbilitySlot.Defense);
            if (Input.GetKeyDown(movement)) runtime.Abilities.TryUse(AbilitySlot.Movement);
            if (Input.GetKeyDown(ultimate)) runtime.Abilities.TryUse(AbilitySlot.Ultimate);
            if (Input.GetKeyDown(inventory1)) runtime.Inventory.UseSlot(0);
            if (Input.GetKeyDown(inventory2)) runtime.Inventory.UseSlot(1);
            if (Input.GetKeyDown(inventory3)) runtime.Inventory.UseSlot(2);
            if (Input.GetKeyDown(inventory4)) runtime.Inventory.UseSlot(3);
        }

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
