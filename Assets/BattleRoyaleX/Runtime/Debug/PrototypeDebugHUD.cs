using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class PrototypeDebugHUD : MonoBehaviour
    {
        public CharacterRuntime playerOne;
        public CharacterRuntime playerTwo;
        public PrototypeLabController lab;
        public Texture2D buttonTexture;
        GUIStyle labButton;

        void OnGUI()
        {
            if (labButton == null)
            {
                labButton = new GUIStyle(GUI.skin.button);
                if (buttonTexture != null) labButton.normal.background = buttonTexture;
            }
            GUIStyle previousButton = GUI.skin.button;
            Matrix4x4 previousMatrix = GUI.matrix;
            // Match the combat Canvas scaling, keeping the lab below its health panel.
            float hudScale = Mathf.Sqrt((Screen.width / 1920f) * (Screen.height / 1080f));
            float top = 285f * hudScale;
            float scale = Mathf.Clamp(Mathf.Min(Screen.width / 550f, (Screen.height - top - 90f) / 450f), 0.2f, 1f);
            GUI.matrix = Matrix4x4.TRS(new Vector3(0f, top, 0f), Quaternion.identity, Vector3.one * scale);
            GUI.skin.button = labButton;
            try { DrawLab(); }
            finally { GUI.skin.button = previousButton; GUI.matrix = previousMatrix; }
        }

        void DrawLab()
        {
            GUI.Box(new Rect(10, 10, 520, 430), "Battle Royale X — Laboratório V1");
            DrawCharacter(new Rect(20, 35, 370, 45), "P1", playerOne);
            DrawCharacter(new Rect(20, 85, 370, 45), "P2", playerTwo);
            GUI.Label(new Rect(20, 130, 370, 20), "P1: WASD F/G/H/R 1-4 | P2: Arrows Numpad1/2/3/0 4-7");
            if (lab == null) return;
            GUI.Label(new Rect(20, 157, 440, 20), "Slot do oponente: GUERREIRO (fixo) | Slot do jogador:");
            if (GUI.Button(new Rect(20, 180, 100, 30), "Guerreiro")) lab.SwitchPlayerClass(CharacterClass.Warrior);
            if (GUI.Button(new Rect(125, 180, 100, 30), "Assassino")) lab.SwitchPlayerClass(CharacterClass.Assassin);
            if (GUI.Button(new Rect(230, 180, 100, 30), "Mago")) lab.SwitchPlayerClass(CharacterClass.Mage);
            if (GUI.Button(new Rect(335, 180, 100, 30), "Arqueiro")) lab.SwitchPlayerClass(CharacterClass.Archer);
            DrawVariations(AbilitySlot.Skill1, "Skill 1", 218f);
            DrawVariations(AbilitySlot.Skill2, "Skill 2", 250f);
            DrawVariations(AbilitySlot.Ultimate, "Ultimate", 282f);
            bool evolved = lab.IsTitanEvolved;
            bool toggled = GUI.Toggle(new Rect(20, 316, 175, 24), evolved, "Evolução Titânica");
            if (toggled != evolved) lab.SetTitanEvolved(toggled);
            if (GUI.Button(new Rect(200, 312, 92, 28), "Reset HP")) lab.ResetHealth();
            if (GUI.Button(new Rect(297, 312, 92, 28), "Reset EN")) lab.ResetEnergy();
            if (GUI.Button(new Rect(394, 312, 116, 28), "Reset CDs")) lab.ResetCooldowns();
            if (GUI.Button(new Rect(20, 345, 100, 28), "Respawn")) lab.ResetLab();
            GUI.Label(new Rect(130, 350, 85, 24), "Modo bot");
            if (GUI.Button(new Rect(210, 345, 90, 28), "Normal")) lab.SetBotMode(PrototypeTrainingBot.TrainingMode.Normal);
            if (GUI.Button(new Rect(305, 345, 90, 28), "Parado")) lab.SetBotMode(PrototypeTrainingBot.TrainingMode.Stationary);
            if (GUI.Button(new Rect(400, 345, 110, 28), "Bate parado")) lab.SetBotMode(PrototypeTrainingBot.TrainingMode.StationaryAttack);
            GUI.Label(new Rect(20, 382, 480, 38), "Teclas: F Basic | G Skill1 | H Skill2 | R Ultimate\nToda troca é instantânea e exclusiva do laboratório.");
        }

        void DrawVariations(AbilitySlot slot, string label, float y)
        {
            GUI.Label(new Rect(20, y + 5f, 80, 24), label);
            if (GUI.Button(new Rect(100, y, 70, 28), "A")) lab.EquipVariation(slot, 0);
            if (GUI.Button(new Rect(175, y, 70, 28), "B")) lab.EquipVariation(slot, 1);
            if (GUI.Button(new Rect(250, y, 70, 28), "C")) lab.EquipVariation(slot, 2);
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
