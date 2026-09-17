using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class PrototypeDebugHUD : MonoBehaviour
    {
        public CharacterRuntime playerOne;
        public CharacterRuntime playerTwo;

        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 390, 145), "Battle Royale X — Prototype 01");
            DrawCharacter(new Rect(20, 35, 370, 45), "P1", playerOne);
            DrawCharacter(new Rect(20, 85, 370, 45), "P2", playerTwo);
            GUI.Label(new Rect(20, 130, 370, 20), "P1: WASD F/G/H/R 1-4 | P2: Arrows Numpad1/2/3/0 4-7");
        }

        void DrawCharacter(Rect rect, string prefix, CharacterRuntime c)
        {
            if (c == null) { GUI.Label(rect, prefix + ": missing"); return; }
            string inv = "";
            for (int i = 0; i < c.Inventory.Slots.Count; i++) inv += (i > 0 ? ", " : "") + c.Inventory.Slots[i].displayName;
            GUI.Label(rect, $"{prefix} {c.Definition.displayName} | HP {c.Health.CurrentHealth:0}/{c.Health.MaxHealth:0} | EN {c.Energy.CurrentEnergy:0}/{c.Energy.MaxEnergy:0}\nInv[{c.Inventory.Slots.Count}/{c.Inventory.Capacity}]: {inv}");
        }
    }
}
