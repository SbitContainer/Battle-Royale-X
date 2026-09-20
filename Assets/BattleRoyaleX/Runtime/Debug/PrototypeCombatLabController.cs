using UnityEngine;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed class PrototypeCombatLabController : MonoBehaviour
    {
        public CharacterRuntime fixedOpponent;
        public CharacterRuntime playerSlot;
        public CharacterDefinition warriorDefinition;
        public CharacterDefinition assassinDefinition;

        public CharacterClass PlayerClass => playerSlot != null && playerSlot.Definition != null
            ? playerSlot.Definition.characterClass : CharacterClass.Assassin;
        public PrototypeTrainingBot OpponentBot => fixedOpponent != null ? fixedOpponent.GetComponent<PrototypeTrainingBot>() : null;
        public PrototypeTrainingBot.TrainingMode BotMode => OpponentBot != null
            ? OpponentBot.Mode : PrototypeTrainingBot.TrainingMode.Normal;

        public bool SwitchPlayerClass(CharacterClass characterClass)
        {
            if (characterClass != CharacterClass.Warrior && characterClass != CharacterClass.Assassin) return false;
            CharacterDefinition chosen = characterClass == CharacterClass.Warrior ? warriorDefinition : assassinDefinition;
            if (playerSlot == null || chosen == null) return false;
            playerSlot.Initialize(chosen);
            SetPlayerVisual(characterClass);
            EnsureOpponentIsWarrior();
            return true;
        }

        public bool EquipVariation(AbilitySlot slot, int variationIndex)
        {
            if (playerSlot == null || playerSlot.Definition == null || slot == AbilitySlot.BasicAttack ||
                variationIndex < 0 || variationIndex > 2) return false;
            CharacterDefinition definition = playerSlot.Definition;
            AbilityDefinition selected = null;
            if (slot == AbilitySlot.Defense)
                selected = variationIndex == 0 ? definition.defenseBase : variationIndex == 1 ? definition.defenseVariantA : definition.defenseVariantB;
            else if (slot == AbilitySlot.Movement)
                selected = variationIndex == 0 ? definition.movementBase : variationIndex == 1 ? definition.movementVariantA : definition.movementVariantB;
            else if (slot == AbilitySlot.Ultimate)
                selected = variationIndex == 0 ? definition.ultimateBase : variationIndex == 1 ? definition.ultimateVariantA : definition.ultimateVariantB;
            return selected != null && playerSlot.Abilities.EquipLabVariation(selected);
        }

        public int GetVariationIndex(AbilitySlot slot)
        {
            if (playerSlot == null || playerSlot.Definition == null || playerSlot.Abilities == null) return 0;
            CharacterDefinition definition = playerSlot.Definition;
            AbilityDefinition current = playerSlot.Abilities.GetEquipped(slot);
            if (slot == AbilitySlot.Defense)
                return current == definition.defenseVariantA ? 1 : current == definition.defenseVariantB ? 2 : 0;
            if (slot == AbilitySlot.Movement)
                return current == definition.movementVariantA ? 1 : current == definition.movementVariantB ? 2 : 0;
            if (slot == AbilitySlot.Ultimate)
                return current == definition.ultimateVariantA ? 1 : current == definition.ultimateVariantB ? 2 : 0;
            return 0;
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
            PrototypeTrainingBot bot = fixedOpponent.GetComponent<PrototypeTrainingBot>() ?? fixedOpponent.gameObject.AddComponent<PrototypeTrainingBot>();
            bot.enabled = true;
        }

        void SetPlayerVisual(CharacterClass activeClass)
        {
            foreach (Transform child in playerSlot.GetComponentsInChildren<Transform>(true))
            {
                if (child.parent != playerSlot.transform) continue;
                if (child.name == "VisualModel") child.gameObject.SetActive(activeClass == CharacterClass.Assassin);
                else if (child.name.StartsWith("LabVisual_")) child.gameObject.SetActive(child.name == "LabVisual_" + activeClass);
            }
        }
    }
}
