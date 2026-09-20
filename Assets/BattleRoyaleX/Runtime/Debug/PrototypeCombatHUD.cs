using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed class PrototypeCombatHUD : MonoBehaviour
    {
        [Serializable]
        sealed class PlayerPanel
        {
            public Text title;
            public Image healthFill;
            public Text healthLabel;
            public Image energyFill;
            public Text energyLabel;
            public Text primaryAbilities;
            public Text secondaryAbilities;
            public Text inventory;
        }

        public CharacterRuntime playerOne;
        public CharacterRuntime playerTwo;

        readonly StringBuilder inventoryBuilder = new StringBuilder(160);

        Font font;
        PlayerPanel leftPanel;
        PlayerPanel rightPanel;
        CanvasGroup eventGroup;
        Text eventText;
        GameObject resultPanel;
        Text resultText;
        float eventVisibleUntil;
        bool built;

        static readonly Color PanelColor = new Color(0.025f, 0.035f, 0.055f, 0.92f);
        static readonly Color PanelBorderColor = new Color(0.22f, 0.27f, 0.36f, 0.95f);
        static readonly Color WarriorColor = new Color(0.95f, 0.48f, 0.12f, 1f);
        static readonly Color AssassinColor = new Color(0.53f, 0.28f, 0.96f, 1f);
        static readonly Color HealthColor = new Color(0.88f, 0.16f, 0.12f, 1f);
        static readonly Color EnergyColor = new Color(0.12f, 0.58f, 0.95f, 1f);
        static readonly Color TextColor = new Color(0.94f, 0.96f, 1f, 1f);
        static readonly Color MutedTextColor = new Color(0.68f, 0.73f, 0.82f, 1f);

        void Awake()
        {
            Build();
        }

        void OnEnable()
        {
            CombatEvents.Raised += OnCombatEvent;
        }

        void OnDisable()
        {
            CombatEvents.Raised -= OnCombatEvent;
        }

        void Update()
        {
            if (!built) Build();
            UpdatePanel(leftPanel, playerOne, false);
            UpdatePanel(rightPanel, playerTwo, true);
            UpdateEventBanner();
            UpdateResult();
        }

        void Build()
        {
            if (built) return;

            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 18);

            GameObject canvasObject = new GameObject("CombatHUD_Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            canvasObject.layer = LayerMask.NameToLayer("UI");

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
            leftPanel = CreatePlayerPanel(canvasRect, "P1_Panel", false, WarriorColor);
            rightPanel = CreatePlayerPanel(canvasRect, "P2_Panel", true, AssassinColor);
            CreateEventBanner(canvasRect);
            CreateControlsFooter(canvasRect);
            CreateResultPanel(canvasRect);
            built = true;
        }

        PlayerPanel CreatePlayerPanel(RectTransform parent, string name, bool rightAligned, Color accentColor)
        {
            RectTransform panel = CreateRect(name, parent);
            panel.anchorMin = panel.anchorMax = rightAligned ? Vector2.one : new Vector2(0f, 1f);
            panel.pivot = rightAligned ? Vector2.one : new Vector2(0f, 1f);
            panel.anchoredPosition = rightAligned ? new Vector2(-28f, -28f) : new Vector2(28f, -28f);
            panel.sizeDelta = new Vector2(470f, 250f);
            if (Application.isMobilePlatform) panel.localScale = Vector3.one * 0.78f;

            Image background = panel.gameObject.AddComponent<Image>();
            background.color = PanelColor;
            Outline outline = panel.gameObject.AddComponent<Outline>();
            outline.effectColor = PanelBorderColor;
            outline.effectDistance = new Vector2(2f, -2f);

            RectTransform accent = CreateRect("ClassAccent", panel);
            accent.anchorMin = rightAligned ? new Vector2(1f, 0f) : Vector2.zero;
            accent.anchorMax = rightAligned ? Vector2.one : new Vector2(0f, 1f);
            accent.pivot = rightAligned ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f);
            accent.sizeDelta = new Vector2(7f, 0f);
            accent.gameObject.AddComponent<Image>().color = accentColor;

            TextAnchor alignment = rightAligned ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
            float horizontal = rightAligned ? -22f : 22f;

            PlayerPanel result = new PlayerPanel();
            result.title = CreateText(panel, "Title", 24, FontStyle.Bold, alignment, TextColor);
            SetTopRect(result.title.rectTransform, horizontal, -12f, 426f, 30f, rightAligned);

            CreateBar(panel, "Health", -52f, rightAligned, HealthColor, out result.healthFill, out result.healthLabel);
            CreateBar(panel, "Energy", -84f, rightAligned, EnergyColor, out result.energyFill, out result.energyLabel);

            result.primaryAbilities = CreateText(panel, "PrimaryAbilities", 17, FontStyle.Bold, alignment, TextColor);
            SetTopRect(result.primaryAbilities.rectTransform, horizontal, -118f, 426f, 24f, rightAligned);

            result.secondaryAbilities = CreateText(panel, "SecondaryAbilities", 17, FontStyle.Bold, alignment, TextColor);
            SetTopRect(result.secondaryAbilities.rectTransform, horizontal, -146f, 426f, 24f, rightAligned);

            Text inventoryTitle = CreateText(panel, "InventoryTitle", 14, FontStyle.Bold, alignment, MutedTextColor);
            inventoryTitle.text = "INVENTÁRIO";
            SetTopRect(inventoryTitle.rectTransform, horizontal, -181f, 426f, 20f, rightAligned);

            result.inventory = CreateText(panel, "Inventory", 15, FontStyle.Normal, alignment, TextColor);
            SetTopRect(result.inventory.rectTransform, horizontal, -205f, 426f, 28f, rightAligned);
            return result;
        }

        void CreateBar(RectTransform parent, string name, float y, bool rightAligned, Color fillColor,
            out Image fill, out Text label)
        {
            RectTransform backgroundRect = CreateRect(name + "Bar", parent);
            SetTopRect(backgroundRect, rightAligned ? -22f : 22f, y, 426f, 24f, rightAligned);
            Image background = backgroundRect.gameObject.AddComponent<Image>();
            background.color = new Color(0.08f, 0.10f, 0.14f, 0.96f);

            RectTransform fillRect = CreateRect("Fill", backgroundRect);
            Stretch(fillRect, 2f);
            fill = fillRect.gameObject.AddComponent<Image>();
            fill.color = fillColor;
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = rightAligned ? 1 : 0;
            fill.fillAmount = 1f;

            label = CreateText(backgroundRect, "Value", 15, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            Stretch(label.rectTransform, 0f);
            AddTextShadow(label);
        }

        void CreateEventBanner(RectTransform parent)
        {
            RectTransform banner = CreateRect("CombatEvent", parent);
            banner.anchorMin = banner.anchorMax = new Vector2(0.5f, 1f);
            banner.pivot = new Vector2(0.5f, 1f);
            banner.anchoredPosition = new Vector2(0f, -30f);
            banner.sizeDelta = new Vector2(620f, 66f);
            banner.gameObject.AddComponent<Image>().color = new Color(0.02f, 0.03f, 0.05f, 0.78f);
            eventGroup = banner.gameObject.AddComponent<CanvasGroup>();
            eventGroup.alpha = 0f;

            eventText = CreateText(banner, "Message", 26, FontStyle.Bold, TextAnchor.MiddleCenter, TextColor);
            Stretch(eventText.rectTransform, 10f);
            AddTextShadow(eventText);
        }

        void CreateControlsFooter(RectTransform parent)
        {
            Text controls = CreateText(parent, "Controls", 15, FontStyle.Normal, TextAnchor.MiddleCenter, MutedTextColor);
            controls.rectTransform.anchorMin = controls.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            controls.rectTransform.pivot = new Vector2(0.5f, 0f);
            controls.rectTransform.anchoredPosition = new Vector2(0f, 18f);
            controls.rectTransform.sizeDelta = new Vector2(1180f, 28f);
            controls.text = Application.isMobilePlatform
                ? ""
                : "P1  WASD · F ataque · G defesa · H movimento · R ultimate    |    P2  SETAS · NUM 1/2/3/0";
            if (Application.isMobilePlatform) controls.gameObject.SetActive(false);
        }

        void CreateResultPanel(RectTransform parent)
        {
            RectTransform panel = CreateRect("CombatResult", parent);
            panel.anchorMin = panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.anchoredPosition = new Vector2(0f, 90f);
            panel.sizeDelta = new Vector2(720f, 150f);
            panel.gameObject.AddComponent<Image>().color = new Color(0.015f, 0.02f, 0.035f, 0.94f);
            Outline outline = panel.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.85f, 0.68f, 0.25f, 0.9f);
            outline.effectDistance = new Vector2(3f, -3f);

            resultText = CreateText(panel, "ResultText", 38, FontStyle.Bold, TextAnchor.MiddleCenter, TextColor);
            Stretch(resultText.rectTransform, 12f);
            AddTextShadow(resultText);
            resultPanel = panel.gameObject;
            resultPanel.SetActive(false);
        }

        void UpdatePanel(PlayerPanel panel, CharacterRuntime character, bool playerTwoSide)
        {
            if (panel == null || character == null || character.Definition == null) return;

            string prefix = playerTwoSide ? "P2" : "P1";
            bool bot = character.GetComponent<PrototypeTrainingBot>() != null && character.GetComponent<PrototypeTrainingBot>().isActiveAndEnabled;
            panel.title.text = prefix + "  " + character.Definition.displayName.ToUpperInvariant() + (bot ? "  BOT" : "");

            float health = character.Health != null ? character.Health.CurrentHealth : 0f;
            float maxHealth = character.Health != null ? character.Health.MaxHealth : 1f;
            panel.healthFill.fillAmount = Mathf.Clamp01(health / Mathf.Max(1f, maxHealth));
            panel.healthLabel.text = $"VIDA   {health:0} / {maxHealth:0}";

            float energy = character.Energy != null ? character.Energy.CurrentEnergy : 0f;
            float maxEnergy = character.Energy != null ? character.Energy.MaxEnergy : 1f;
            panel.energyFill.fillAmount = Mathf.Clamp01(energy / Mathf.Max(1f, maxEnergy));
            panel.energyLabel.text = $"ENERGIA   {energy:0} / {maxEnergy:0}";

            string attackKey = playerTwoSide ? "NUM1" : "F";
            string defenseKey = playerTwoSide ? "NUM2" : "G";
            string movementKey = playerTwoSide ? "NUM3" : "H";
            string ultimateKey = playerTwoSide ? "NUM0" : "R";
            panel.primaryAbilities.text = Application.isMobilePlatform
                ? $"ATAQUE {Cooldown(character, AbilitySlot.BasicAttack)}   DEFESA {Cooldown(character, AbilitySlot.Defense)}"
                : $"[{attackKey}] ATAQUE  {Cooldown(character, AbilitySlot.BasicAttack)}    [{defenseKey}] DEFESA  {Cooldown(character, AbilitySlot.Defense)}";
            string ultimateStatus = character.Abilities != null && character.Abilities.IsChargedSequenceActive
                ? character.Abilities.ChargedSequenceRemaining + " CARGAS"
                : Cooldown(character, AbilitySlot.Ultimate);
            panel.secondaryAbilities.text = Application.isMobilePlatform
                ? $"MOVIMENTO {Cooldown(character, AbilitySlot.Movement)}   ULT {ultimateStatus}"
                : $"[{movementKey}] MOVIMENTO  {Cooldown(character, AbilitySlot.Movement)}    [{ultimateKey}] ULT  {ultimateStatus}";
            panel.inventory.text = InventoryText(character, playerTwoSide);
        }

        string InventoryText(CharacterRuntime character, bool playerTwoSide)
        {
            inventoryBuilder.Clear();
            int capacity = character.Inventory != null ? character.Inventory.Capacity : 0;
            for (int i = 0; i < 4; i++)
            {
                if (i > 0) inventoryBuilder.Append("   ");
                string key = playerTwoSide ? "NUM" + (i + 4) : (i + 1).ToString();
                inventoryBuilder.Append('[').Append(key).Append("] ");
                if (i >= capacity)
                {
                    inventoryBuilder.Append("BLOQ.");
                }
                else if (i < character.Inventory.Slots.Count && character.Inventory.Slots[i] != null)
                {
                    inventoryBuilder.Append(ShortName(character.Inventory.Slots[i].displayName));
                }
                else
                {
                    inventoryBuilder.Append('—');
                }
            }
            return inventoryBuilder.ToString();
        }

        static string ShortName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "ITEM";
            const int maxLength = 14;
            return value.Length <= maxLength ? value.ToUpperInvariant() : value.Substring(0, maxLength - 1).ToUpperInvariant() + "…";
        }

        static string Cooldown(CharacterRuntime character, AbilitySlot slot)
        {
            if (character.Abilities == null) return "—";
            float remaining = character.Abilities.GetCooldownRemaining(slot);
            return remaining <= 0.05f ? "PRONTO" : remaining.ToString("0.0") + "s";
        }

        void OnCombatEvent(CombatEventData data)
        {
            if (eventText == null) return;
            if ((data.kind == CombatEventKind.AbilityAttack || data.kind == CombatEventKind.AbilityGuard ||
                 data.kind == CombatEventKind.AbilityMove || data.kind == CombatEventKind.AbilityUltimate) &&
                data.phase != AbilityPhase.Active) return;
            eventText.text = EventLabel(data);
            eventText.color = EventColor(data.kind);
            eventVisibleUntil = Time.unscaledTime + 1.1f;
            eventGroup.alpha = 1f;
        }

        void UpdateEventBanner()
        {
            if (eventGroup == null) return;
            float remaining = eventVisibleUntil - Time.unscaledTime;
            eventGroup.alpha = Mathf.Clamp01(remaining * 2.5f);
        }

        void UpdateResult()
        {
            if (resultPanel == null || playerOne == null || playerTwo == null) return;
            bool p1Dead = playerOne.Health != null && playerOne.Health.IsDead;
            bool p2Dead = playerTwo.Health != null && playerTwo.Health.IsDead;
            bool finished = p1Dead || p2Dead;
            // The Android training loop restores both fighters automatically; do not interrupt it with a death screen.
            if (Application.isMobilePlatform)
            {
                resultPanel.SetActive(false);
                return;
            }
            resultPanel.SetActive(finished);
            if (!finished) return;

            if (p1Dead && p2Dead) resultText.text = "EMPATE";
            else if (p2Dead) resultText.text = "P1 · GUERREIRO VENCEU";
            else resultText.text = "P2 · ASSASSINO VENCEU";
        }

        static string EventLabel(CombatEventData data)
        {
            switch (data.kind)
            {
                case CombatEventKind.Hit: return "ACERTO";
                case CombatEventKind.Block: return "BLOQUEIO";
                case CombatEventKind.Parry: return "PARRY PERFEITO";
                case CombatEventKind.Dodge: return "ESQUIVA";
                case CombatEventKind.Clash: return "CLASH";
                case CombatEventKind.Nullify: return "ANULADO";
                case CombatEventKind.Reflect: return "REFLETIDO";
                case CombatEventKind.Heal: return "CURA";
                case CombatEventKind.Energy: return "ENERGIA";
                case CombatEventKind.VariationSwap:
                    return data.ability != null ? "EQUIPADA: " + data.ability.displayName.ToUpperInvariant() : "VARIAÇÃO EQUIPADA";
                case CombatEventKind.TacticalUsed:
                    return data.item != null ? data.item.displayName.ToUpperInvariant() : "ITEM TÁTICO";
                case CombatEventKind.ItemPickup:
                    return data.item != null ? "COLETADO: " + data.item.displayName.ToUpperInvariant() : "ITEM COLETADO";
                case CombatEventKind.CounterReady: return "CONTRA-ATAQUE PRONTO · ATAQUE!";
                case CombatEventKind.CounterHit: return "CONTRA-ATAQUE · REPULSÃO";
                case CombatEventKind.DefenseRedirect: return "ESQUIVA · CONTRA-TRAVESSIA";
                case CombatEventKind.AbilityAttack:
                case CombatEventKind.AbilityGuard:
                case CombatEventKind.AbilityMove:
                case CombatEventKind.AbilityUltimate:
                    return data.ability != null ? data.ability.displayName.ToUpperInvariant() : "HABILIDADE";
                default: return data.kind.ToString().ToUpperInvariant();
            }
        }

        static Color EventColor(CombatEventKind kind)
        {
            switch (kind)
            {
                case CombatEventKind.Parry: return new Color(1f, 0.84f, 0.28f);
                case CombatEventKind.Clash: return Color.white;
                case CombatEventKind.Block: return new Color(0.38f, 0.72f, 1f);
                case CombatEventKind.Dodge: return new Color(0.68f, 0.45f, 1f);
                case CombatEventKind.Nullify: return new Color(0.28f, 1f, 0.86f);
                case CombatEventKind.Heal: return new Color(0.3f, 1f, 0.42f);
                case CombatEventKind.ItemPickup: return new Color(0.35f, 1f, 0.72f);
                default: return new Color(1f, 0.48f, 0.22f);
            }
        }

        Text CreateText(Transform parent, string name, int size, FontStyle style, TextAnchor alignment, Color color)
        {
            RectTransform rect = CreateRect(name, parent);
            Text text = rect.gameObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        static RectTransform CreateRect(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.layer = LayerMask.NameToLayer("UI");
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        static void SetTopRect(RectTransform rect, float horizontal, float y, float width, float height, bool rightAligned)
        {
            rect.anchorMin = rect.anchorMax = rightAligned ? Vector2.one : new Vector2(0f, 1f);
            rect.pivot = rightAligned ? Vector2.one : new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(horizontal, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        static void Stretch(RectTransform rect, float inset)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }

        static void AddTextShadow(Text text)
        {
            Shadow shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.8f);
            shadow.effectDistance = new Vector2(1.5f, -1.5f);
        }
    }
}
