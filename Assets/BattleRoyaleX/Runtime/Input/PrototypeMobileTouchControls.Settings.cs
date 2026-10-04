using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleRoyaleX
{
    public sealed partial class PrototypeMobileTouchControls
    {
        const string LayoutKey = "BRX.MobileLayout.v4";
        [Serializable] public sealed class LayoutEntry { public string id; public float x, y, scale = 1f; }
        [Serializable] public sealed class LayoutData { public List<LayoutEntry> entries = new List<LayoutEntry>(); }
        readonly List<RectTransform> editableButtons = new List<RectTransform>();
        readonly Dictionary<string, Vector2> defaultPositions = new Dictionary<string, Vector2>();
        readonly Dictionary<AbilitySlot, Text> labVariationLabels = new Dictionary<AbilitySlot, Text>();
        RectTransform settingsPanel, editToolbar;
        RectTransform selectedButton;
        Text infoText, editStatus, playerLabel, botModeLabel;
        PrototypeCombatLabController combatLab;
        bool menuOpen, editingLayout, ownsPause;
        float previousTimeScale;
        public bool IsEditingLayout => editingLayout;

        void BuildSettings()
        {
            Canvas.ForceUpdateCanvases();
            foreach (var view in abilityButtons.Values) RegisterEditable(view.rect);
            foreach (var view in itemButtons) RegisterEditable(view.rect);
            RegisterEditable(pickupButton);
            LoadLayout();
            var menu = MenuButton(safeRoot, "MENU / SKILLS", new Vector2(0f, -60f), new Vector2(290f, 80f), OpenSettings);
            menu.anchorMin = menu.anchorMax = new Vector2(0.5f, 1f);
            playerLabel = AddLabel(menu, "", 20);
            playerLabel.rectTransform.anchoredPosition = new Vector2(0f, -68f);
            var reset = MenuButton(safeRoot, "RESET COOLDOWNS", new Vector2(340f, -60f), new Vector2(330f, 80f), ResetLabCooldowns);
            reset.anchorMin = reset.anchorMax = new Vector2(0.5f, 1f);

            settingsPanel = CreateRect(safeRoot, "CombatSettings");
            settingsPanel.anchorMin = settingsPanel.anchorMax = settingsPanel.pivot = Vector2.one * 0.5f;
            settingsPanel.sizeDelta = new Vector2(Mathf.Min(1420f, safeRoot.rect.width - 40f), 880f);
            settingsPanel.gameObject.AddComponent<Image>().color = new Color(0.025f, 0.04f, 0.08f, 0.98f);
            MenuButton(settingsPanel, "VOLTAR", new Vector2(495f, 380f), new Vector2(235f, 75f), CloseSettings);
            MenuButton(settingsPanel, "EDITAR BOTÕES", new Vector2(-445f, 380f), new Vector2(320f, 75f), BeginLayoutEdit);
            MenuButton(settingsPanel, "TROCAR CLASSE", new Vector2(-30f, 380f), new Vector2(390f, 75f), SwitchPlayer);
            MenuButton(settingsPanel, "GUERREIRO", new Vector2(-515f, 290f), new Vector2(245f, 65f),
                () => SelectLabClass(CharacterClass.Warrior));
            MenuButton(settingsPanel, "ASSASSINO", new Vector2(-180f, 290f), new Vector2(245f, 65f),
                () => SelectLabClass(CharacterClass.Assassin));
            MenuButton(settingsPanel, "MAGO", new Vector2(150f, 290f), new Vector2(245f, 65f),
                () => SelectLabClass(CharacterClass.Mage));
            MenuButton(settingsPanel, "ARQUEIRO", new Vector2(485f, 290f), new Vector2(245f, 65f),
                () => SelectLabClass(CharacterClass.Archer));
            BuildLabTestControls();
            infoText = AddLabel(settingsPanel, "", 27);
            infoText.alignment = TextAnchor.UpperLeft; infoText.fontStyle = FontStyle.Normal;
            infoText.rectTransform.offsetMin = new Vector2(460f, 30f);
            infoText.rectTransform.offsetMax = new Vector2(-35f, -210f);

            editToolbar = CreateRect(safeRoot, "LayoutToolbar");
            editToolbar.anchorMin = editToolbar.anchorMax = new Vector2(0.5f, 1f);
            editToolbar.pivot = new Vector2(0.5f, 1f); editToolbar.sizeDelta = new Vector2(1450f, 175f);
            editToolbar.gameObject.AddComponent<Image>().color = new Color(0.03f, 0.06f, 0.10f, 0.98f);
            MenuButton(editToolbar, "SALVAR", new Vector2(590f, -56f), new Vector2(220f, 76f), SaveLayoutAndClose);
            MenuButton(editToolbar, "PADRÃO 2×", new Vector2(270f, -56f), new Vector2(280f, 76f), ResetLayout);
            MenuButton(editToolbar, "MENOR −", new Vector2(-365f, -56f), new Vector2(240f, 76f), () => ResizeSelected(-0.1f));
            MenuButton(editToolbar, "MAIOR +", new Vector2(-65f, -56f), new Vector2(240f, 76f), () => ResizeSelected(0.1f));
            foreach (Transform child in editToolbar) ((RectTransform)child).anchorMin = ((RectTransform)child).anchorMax = new Vector2(0.5f, 1f);
            editStatus = AddLabel(editToolbar, "Arraste os botões. Toque para selecionar; use MENOR / MAIOR.", 25);
            editStatus.rectTransform.offsetMax = new Vector2(0f, -100f);
            settingsPanel.gameObject.SetActive(false); editToolbar.gameObject.SetActive(false);
        }

        void BuildLabTestControls()
        {
            combatLab = FindAnyObjectByType<PrototypeCombatLabController>();
            CreateLabVariationButton(AbilitySlot.Defense, 1, new Vector2(100f, 100f), "SKILL 1");
            CreateLabVariationButton(AbilitySlot.Movement, 2, new Vector2(260f, 100f), "SKILL 2");
            CreateLabVariationButton(AbilitySlot.Ultimate, 3, new Vector2(420f, 100f), "ULT");
            RectTransform bot = MenuButton(settingsPanel, "BOT", new Vector2(-500f, -275f),
                new Vector2(350f, 72f), () => CycleBotMode());
            botModeLabel = bot.GetComponentInChildren<Text>();
            RefreshLabControls();
        }

        void CreateLabVariationButton(AbilitySlot slot, int number, Vector2 position, string shortName)
        {
            RectTransform rect = CreateCircle(safeRoot, "LabSkill_" + number, position, 144f,
                new Color(0.10f, 0.22f, 0.34f, 0.94f), true);
            rect.anchorMin = rect.anchorMax = Vector2.zero;
            rect.pivot = Vector2.one * 0.5f;
            Text label = AddLabel(rect, number + "\n" + shortName + " A", 19);
            EventTrigger trigger = rect.gameObject.AddComponent<EventTrigger>();
            trigger.triggers = new List<EventTrigger.Entry>();
            AddTrigger(trigger, EventTriggerType.PointerDown, data => CycleLabVariation(slot));
            labVariationLabels[slot] = label;
        }

        public bool CycleLabVariation(AbilitySlot slot)
        {
            if (combatLab == null) combatLab = FindAnyObjectByType<PrototypeCombatLabController>();
            if (combatLab == null || slot == AbilitySlot.BasicAttack) return false;
            int next = (combatLab.GetVariationIndex(slot) + 1) % 3;
            bool changed = combatLab.EquipVariation(slot, next);
            if (changed) RefreshLabControls();
            return changed;
        }

        public PrototypeTrainingBot.TrainingMode CycleBotMode()
        {
            if (combatLab == null) combatLab = FindAnyObjectByType<PrototypeCombatLabController>();
            PrototypeTrainingBot.TrainingMode result = combatLab != null
                ? combatLab.CycleBotMode() : PrototypeTrainingBot.TrainingMode.Normal;
            RefreshLabControls();
            return result;
        }

        public PrototypeTrainingBot.TrainingMode CurrentBotMode => combatLab != null
            ? combatLab.BotMode : PrototypeTrainingBot.TrainingMode.Normal;

        public void ResetLabCooldowns()
        {
            if (combatLab == null) combatLab = FindAnyObjectByType<PrototypeCombatLabController>();
            if (combatLab != null) combatLab.ResetCooldowns();
        }

        void UpdateLabControls()
        {
            if (combatLab == null) combatLab = FindAnyObjectByType<PrototypeCombatLabController>();
            RefreshLabControls();
        }

        void RefreshLabControls()
        {
            if (combatLab == null) return;
            SetLabVariationLabel(AbilitySlot.Defense, 1, "SKILL 1");
            SetLabVariationLabel(AbilitySlot.Movement, 2, "SKILL 2");
            SetLabVariationLabel(AbilitySlot.Ultimate, 3, "ULT");
            if (botModeLabel != null) botModeLabel.text = "BOT: " + BotModeName(combatLab.BotMode);
        }

        void SetLabVariationLabel(AbilitySlot slot, int number, string shortName)
        {
            if (!labVariationLabels.TryGetValue(slot, out Text label) || label == null) return;
            int index = combatLab.GetVariationIndex(slot);
            label.text = number + "\n" + shortName + " " + (index == 0 ? "A" : index == 1 ? "B" : "C");
        }

        static string BotModeName(PrototypeTrainingBot.TrainingMode mode)
        {
            if (mode == PrototypeTrainingBot.TrainingMode.Stationary) return "PARADO";
            if (mode == PrototypeTrainingBot.TrainingMode.StationaryAttack) return "PARADO + ATAQUE";
            return "NORMAL";
        }

        RectTransform MenuButton(RectTransform parent, string title, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction action)
        {
            var rect = CreateRect(parent, title);
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * 0.5f;
            rect.anchoredPosition = position; rect.sizeDelta = size;
            rect.gameObject.AddComponent<Image>().color = new Color(0.12f, 0.24f, 0.36f, 1f);
            rect.gameObject.AddComponent<Button>().onClick.AddListener(action);
            AddLabel(rect, title, 27);
            return rect;
        }

        void RegisterEditable(RectTransform rect)
        {
            Vector2 center = safeRoot.InverseTransformPoint(rect.TransformPoint(rect.rect.center));
            rect.anchorMin = rect.anchorMax = Vector2.zero; rect.pivot = Vector2.one * 0.5f;
            rect.anchoredPosition = center;
            editableButtons.Add(rect);
            defaultPositions[rect.name] = new Vector2(center.x / safeRoot.rect.width, center.y / safeRoot.rect.height);
            var handle = rect.gameObject.AddComponent<MobileLayoutHandle>();
            handle.controls = this;
        }

        void PauseForMenu()
        {
            ClearTouches();
            if (ownsPause) return;
            previousTimeScale = Time.timeScale; ownsPause = true; Time.timeScale = 0f;
        }
        void RestoreTimeScale() { if (!ownsPause) return; Time.timeScale = previousTimeScale; ownsPause = false; }
        public void OpenSettings()
        {
            if (editingLayout) return;
            PauseForMenu(); menuOpen = true;
            settingsPanel.gameObject.SetActive(true); settingsPanel.SetAsLastSibling(); RefreshSkillInfo();
        }
        public void CloseSettings() { menuOpen = false; settingsPanel.gameObject.SetActive(false); RestoreTimeScale(); }
        public void BeginLayoutEdit()
        {
            PauseForMenu(); menuOpen = false; editingLayout = true;
            settingsPanel.gameObject.SetActive(false); editToolbar.gameObject.SetActive(true); editToolbar.SetAsLastSibling();
            selectedButton = abilityButtons[AbilitySlot.BasicAttack].rect;
            pickupButton.gameObject.SetActive(true);
        }
        public void EditButton(RectTransform rect, Vector2? screenPoint)
        {
            if (!editingLayout || !editableButtons.Contains(rect)) return;
            selectedButton = rect;
            if (screenPoint.HasValue && ScreenToSafe(screenPoint.Value, out Vector2 local)) rect.anchoredPosition = local;
            ClampButton(rect);
            editStatus.text = "Selecionado: " + rect.name + " · " + Mathf.RoundToInt(rect.localScale.x * 200f) + "% do tamanho antigo · arraste para mover";
        }
        public void ResizeSelected(float delta)
        {
            if (!editingLayout || selectedButton == null) return;
            selectedButton.localScale = Vector3.one * Mathf.Clamp(selectedButton.localScale.x + delta, 0.5f, 1.5f);
            EditButton(selectedButton, null);
        }
        void ClampButton(RectTransform rect)
        {
            float radius = rect.rect.width * rect.localScale.x * 0.5f + 8f;
            rect.anchoredPosition = new Vector2(Mathf.Clamp(rect.anchoredPosition.x, radius, safeRoot.rect.width - radius),
                Mathf.Clamp(rect.anchoredPosition.y, radius, safeRoot.rect.height - 180f - radius));
        }
        public string SerializeLayout()
        {
            var data = new LayoutData();
            foreach (var rect in editableButtons) data.entries.Add(new LayoutEntry { id = rect.name,
                x = rect.anchoredPosition.x / safeRoot.rect.width, y = rect.anchoredPosition.y / safeRoot.rect.height, scale = rect.localScale.x });
            return JsonUtility.ToJson(data);
        }
        public void ApplyLayout(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            try
            {
                LayoutData data = JsonUtility.FromJson<LayoutData>(json);
                if (data == null || data.entries == null) return;
                foreach (var entry in data.entries)
                {
                    if (entry == null || float.IsNaN(entry.x) || float.IsNaN(entry.y) || float.IsNaN(entry.scale) ||
                        float.IsInfinity(entry.x) || float.IsInfinity(entry.y) || float.IsInfinity(entry.scale)) continue;
                    var rect = editableButtons.Find(r => r.name == entry.id); if (rect == null) continue;
                    rect.localScale = Vector3.one * Mathf.Clamp(entry.scale, 0.5f, 1.5f);
                    rect.anchoredPosition = new Vector2(Mathf.Clamp01(entry.x) * safeRoot.rect.width, Mathf.Clamp01(entry.y) * safeRoot.rect.height);
                    ClampButton(rect);
                }
            }
            catch (ArgumentException) { Debug.LogWarning("BRX: layout inválido; mantendo padrão."); }
        }
        void LoadLayout() => ApplyLayout(PlayerPrefs.GetString(LayoutKey, ""));
        public void SaveLayoutAndClose()
        {
            if (!editingLayout) return;
            PlayerPrefs.SetString(LayoutKey, SerializeLayout()); PlayerPrefs.Save();
            editingLayout = false; editToolbar.gameObject.SetActive(false); ClearTouches(); RestoreTimeScale();
        }
        public void ResetLayout()
        {
            foreach (var rect in editableButtons)
            {
                rect.localScale = Vector3.one;
                rect.anchoredPosition = Vector2.Scale(defaultPositions[rect.name], safeRoot.rect.size); ClampButton(rect);
            }
            if (editStatus != null) editStatus.text = "Padrão restaurado: diâmetros duplicados. SALVAR para manter.";
        }

        public void SwitchPlayer()
        {
            PrototypeCombatLabController lab = FindAnyObjectByType<PrototypeCombatLabController>();
            if (lab != null && lab.playerSlot != null)
            {
                CharacterClass next = lab.PlayerClass == CharacterClass.Assassin ? CharacterClass.Warrior :
                    lab.PlayerClass == CharacterClass.Warrior ? CharacterClass.Mage :
                    lab.PlayerClass == CharacterClass.Mage ? CharacterClass.Archer : CharacterClass.Assassin;
                SelectLabClass(next);
                return;
            }
            foreach (var candidate in FindObjectsByType<CharacterRuntime>())
                if (candidate != player && candidate.TeamId != player.TeamId) { SelectPlayer(candidate); RefreshSkillInfo(); return; }
        }
        public bool SelectLabClass(CharacterClass characterClass)
        {
            PrototypeCombatLabController lab = FindAnyObjectByType<PrototypeCombatLabController>();
            if (lab == null || lab.playerSlot == null || !lab.SwitchPlayerClass(characterClass)) return false;
            SelectPlayer(lab.playerSlot);
            RefreshSkillInfo();
            return true;
        }
        public void SelectPlayer(CharacterRuntime chosen)
        {
            if (chosen == null) return;
            ClearTouches(); player = chosen; nearbyPickup = null;
            foreach (var actor in FindObjectsByType<CharacterRuntime>())
            {
                actor.ResetTransientState();
                var keyboard = actor.GetComponent<PrototypeLocalInput>(); if (keyboard != null) keyboard.enabled = false;
                var bot = actor.GetComponent<PrototypeTrainingBot>() ?? actor.gameObject.AddComponent<PrototypeTrainingBot>();
                bot.runInEditor = runInEditor;
                bot.enabled = actor != player;
                bot.ResetAwareness();
            }
            SmokeVisibility.LocalPlayer = chosen;
            if (playerLabel != null) playerLabel.text = "VOCÊ: " + chosen.Definition.displayName.ToUpperInvariant();
            RefreshLabControls();
        }

        void RefreshSkillInfo()
        {
            if (infoText == null || player == null) return;
            var builder = new StringBuilder("<b>" + player.Definition.displayName.ToUpperInvariant() + " · HABILIDADES EQUIPADAS</b>\n");
            builder.AppendLine("Pegue runas no chão e toque no item para trocar a variação. O combate pausa neste menu.\n");
            foreach (AbilitySlot slot in new[] { AbilitySlot.BasicAttack, AbilitySlot.Skill1,
                AbilitySlot.Skill2, AbilitySlot.Ultimate })
            {
                var ability = player.Abilities.GetEquipped(slot); if (ability == null) continue;
                builder.AppendLine(AbilityTechnicalInfo.Describe(ability)); builder.AppendLine();
            }
            builder.Append("Fumaça: sem mira no inimigo oculto. Quem está na área de fuga não ataca; golpes de fora podem acertar.");
            infoText.text = builder.ToString();
        }
    }

    public sealed class MobileLayoutHandle : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        public PrototypeMobileTouchControls controls;
        public void OnPointerDown(PointerEventData e) { if (controls != null) controls.EditButton((RectTransform)transform, null); }
        public void OnDrag(PointerEventData e) { if (controls != null) controls.EditButton((RectTransform)transform, e.position); }
    }
}
