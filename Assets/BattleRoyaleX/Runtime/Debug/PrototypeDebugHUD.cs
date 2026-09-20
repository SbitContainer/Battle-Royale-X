using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class PrototypeDebugHUD : MonoBehaviour
    {
        public CharacterRuntime playerOne;
        public CharacterRuntime playerTwo;
        public PrototypeCombatLabController lab;

        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 460, 355), "Battle Royale X — Laboratório de Combate");
            DrawCharacter(new Rect(20, 35, 370, 45), "P1", playerOne);
            DrawCharacter(new Rect(20, 85, 370, 45), "P2", playerTwo);
            GUI.Label(new Rect(20, 130, 370, 20), "P1: WASD F/G/H/R 1-4 | P2: Arrows Numpad1/2/3/0 4-7");
            if (lab == null) return;
            GUI.Label(new Rect(20, 157, 440, 20), "Slot do oponente: GUERREIRO (fixo) | Slot do jogador:");
            if (GUI.Button(new Rect(20, 180, 100, 30), "Guerreiro")) lab.SwitchPlayerClass(CharacterClass.Warrior);
            if (GUI.Button(new Rect(125, 180, 100, 30), "Assassino")) lab.SwitchPlayerClass(CharacterClass.Assassin);
            if (GUI.Button(new Rect(230, 180, 100, 30), "Reiniciar")) lab.ResetLab();
            DrawVariations(AbilitySlot.Defense, "Defesa", 218f);
            DrawVariations(AbilitySlot.Movement, "Movimento", 250f);
            DrawVariations(AbilitySlot.Ultimate, "Ultimate", 282f);
            GUI.Label(new Rect(20, 319, 95, 24), "Modo do bot");
            if (GUI.Button(new Rect(115, 314, 105, 28), "Normal")) lab.SetBotMode(PrototypeTrainingBot.TrainingMode.Normal);
            if (GUI.Button(new Rect(225, 314, 105, 28), "Parado")) lab.SetBotMode(PrototypeTrainingBot.TrainingMode.Stationary);
            if (GUI.Button(new Rect(335, 314, 115, 28), "Bate parado")) lab.SetBotMode(PrototypeTrainingBot.TrainingMode.StationaryAttack);
        }

        void DrawVariations(AbilitySlot slot, string label, float y)
        {
            GUI.Label(new Rect(20, y + 5f, 80, 24), label);
            if (GUI.Button(new Rect(100, y, 70, 28), "Base")) lab.EquipVariation(slot, 0);
            if (GUI.Button(new Rect(175, y, 70, 28), "A")) lab.EquipVariation(slot, 1);
            if (GUI.Button(new Rect(250, y, 70, 28), "B")) lab.EquipVariation(slot, 2);
            AbilityDefinition current = lab.playerSlot != null ? lab.playerSlot.Abilities.GetEquipped(slot) : null;
            GUI.Label(new Rect(328, y + 5f, 132, 24), current != null ? current.displayName : "—");
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
