using UnityEngine;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public class PrototypeLabController : MonoBehaviour
    {
        public CharacterRuntime fixedOpponent;
        public CharacterRuntime playerSlot;
        public CharacterDefinition warriorDefinition;
        public CharacterDefinition assassinDefinition;
        public CharacterDefinition mageDefinition;
        public CharacterDefinition archerDefinition;
        public VisualProfileRegistry visualProfiles;

        public CharacterClass PlayerClass => playerSlot != null && playerSlot.Definition != null
            ? playerSlot.Definition.characterClass : CharacterClass.Assassin;
        public PrototypeTrainingBot OpponentBot => fixedOpponent != null ? fixedOpponent.GetComponent<PrototypeTrainingBot>() : null;
        public PrototypeTrainingBot.TrainingMode BotMode => OpponentBot != null
            ? OpponentBot.Mode : PrototypeTrainingBot.TrainingMode.Normal;
        public bool IsTitanEvolved => playerSlot != null && playerSlot.Abilities != null && playerSlot.Abilities.IsTitanEvolved;

        public bool SwitchPlayerClass(CharacterClass characterClass)
        {
            CharacterDefinition chosen = DefinitionFor(characterClass);
            if (playerSlot == null || chosen == null) return false;
            bool titan = IsTitanEvolved;
            playerSlot.Initialize(chosen);
            playerSlot.Abilities.SetTitanEvolved(titan);
            SetPlayerVisual(characterClass);
            EnsureOpponentIsWarrior();
            return true;
        }

        public CharacterDefinition DefinitionFor(CharacterClass characterClass)
        {
            if (characterClass == CharacterClass.Warrior) return warriorDefinition;
            if (characterClass == CharacterClass.Assassin) return assassinDefinition;
            if (characterClass == CharacterClass.Mage) return mageDefinition;
            if (characterClass == CharacterClass.Archer || characterClass == CharacterClass.Marksman) return archerDefinition;
            return null;
        }

        public bool EquipVariation(AbilitySlot slot, int variationIndex)
        {
            return playerSlot != null && playerSlot.Abilities != null && slot != AbilitySlot.BasicAttack &&
                variationIndex >= 0 && variationIndex <= 2 && playerSlot.Abilities.EquipLabVariation(slot, variationIndex);
        }

        public int GetVariationIndex(AbilitySlot slot)
        {
            if (playerSlot == null || playerSlot.Definition == null || playerSlot.Abilities == null) return 0;
            AbilityDefinition current = playerSlot.Abilities.GetEquipped(slot);
            for (int i = 0; i < 3; i++) if (playerSlot.Definition.GetVariant(slot, i) == current) return i;
            return 0;
        }

        public void SetTitanEvolved(bool active)
        {
            if (playerSlot != null && playerSlot.Abilities != null) playerSlot.Abilities.SetTitanEvolved(active);
        }

        public void ResetHealth()
        {
            if (playerSlot != null) playerSlot.Health.RestoreFull();
            if (fixedOpponent != null) fixedOpponent.Health.RestoreFull();
        }

        public void ResetEnergy()
        {
            if (playerSlot != null) playerSlot.Energy.Restore(playerSlot.Energy.MaxEnergy);
            if (fixedOpponent != null) fixedOpponent.Energy.Restore(fixedOpponent.Energy.MaxEnergy);
        }

        public void ResetCooldowns()
        {
            if (playerSlot != null) playerSlot.Abilities.ResetCooldowns();
            if (fixedOpponent != null) fixedOpponent.Abilities.ResetCooldowns();
        }

        public bool SetBotMode(PrototypeTrainingBot.TrainingMode mode)
        {
            EnsureOpponentIsWarrior();
            PrototypeTrainingBot bot = OpponentBot;
            if (bot == null) return false;
            bot.SetMode(mode);
            bot.enabled = true;
            return true;
        }

        public PrototypeTrainingBot.TrainingMode CycleBotMode()
        {
            PrototypeTrainingBot.TrainingMode next = (PrototypeTrainingBot.TrainingMode)(((int)BotMode + 1) % 3);
            SetBotMode(next);
            return next;
        }

        public void ResetLab()
        {
            if (fixedOpponent != null && warriorDefinition != null) fixedOpponent.Initialize(warriorDefinition);
            if (playerSlot != null && playerSlot.Definition != null) playerSlot.Initialize(playerSlot.Definition);
            EnsureOpponentIsWarrior();
        }

        public void EnsureOpponentIsWarrior()
        {
            if (fixedOpponent == null || warriorDefinition == null) return;
            if (fixedOpponent.Definition != warriorDefinition) fixedOpponent.Initialize(warriorDefinition);
            PrototypeTrainingBot bot = fixedOpponent.GetComponent<PrototypeTrainingBot>() ??
                fixedOpponent.gameObject.AddComponent<PrototypeTrainingBot>();
            bot.enabled = true;
        }

        void SetPlayerVisual(CharacterClass activeClass)
        {
            foreach (Transform child in playerSlot.GetComponentsInChildren<Transform>(true))
            {
                if (child.parent != playerSlot.transform) continue;
                if (child.name == "VisualModel") child.gameObject.SetActive(activeClass == CharacterClass.Assassin);
                else if (child.name.StartsWith("LabVisual_"))
                    child.gameObject.SetActive(child.name == "LabVisual_" + activeClass);
            }
        }
    }
}
