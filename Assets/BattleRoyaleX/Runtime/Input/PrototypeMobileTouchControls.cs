using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed partial class PrototypeMobileTouchControls : MonoBehaviour
    {
        public bool runInEditor;
        public CharacterRuntime Player => player;
        sealed class AbilityButtonView
        {
            public AbilitySlot slot;
            public RectTransform rect;
            public Image cooldown;
            public Text label;
            public Text charge;
        }

        sealed class ItemButtonView
        {
            public int slot;
            public RectTransform rect;
            public Image progress;
            public Text label;
        }

        const float JoystickRadius = 88f;
        const float JoystickDeadZone = 0.10f;
        const float AimThreshold = 18f;

        readonly Dictionary<AbilitySlot, AbilityButtonView> abilityButtons = new Dictionary<AbilitySlot, AbilityButtonView>();
        readonly List<ItemButtonView> itemButtons = new List<ItemButtonView>();
        CharacterRuntime player;
        Font font;
        RectTransform safeRoot;
        RectTransform joystickBase;
        RectTransform joystickKnob;
        RectTransform aimLine;
        RectTransform cancelZone;
        RectTransform pickupButton;
        Text pickupLabel;
        WorldPickup nearbyPickup;
        float nextPickupScanAt;
        Camera worldCamera;
        Sprite circleSprite;
        Rect lastSafeArea;

        Vector2 joystickOrigin;
        Vector2 joystickScreenDirection;
        Vector3 joystickWorldDirection;
        float joystickMagnitude;
        int joystickPointer = int.MinValue;

        int aimPointer = int.MinValue;
        AbilitySlot aimSlot;
        Vector2 aimStart;
        Vector2 aimCurrent;
        int attackPointer = int.MinValue;
        float nextAttackRepeatAt;

        void Start()
        {
            if (!Application.isMobilePlatform && !runInEditor)
            {
                enabled = false;
                return;
            }

            player = FindObjectsByType<CharacterRuntime>().FirstOrDefault(c => c.TeamId == TeamId.PlayerTwo);
            if (player == null) return;
            PrototypeLocalInput keyboard = player.GetComponent<PrototypeLocalInput>();
            if (keyboard != null) keyboard.enabled = false;
            worldCamera = Camera.main;
            EnsureEventSystem();
            Build();
            SelectPlayer(player);
        }

        void Update()
        {
            if (player == null || player.Motor == null) return;
            UpdateSafeArea();
            if (!menuOpen && !editingLayout)
                player.Motor.SetMoveInput(new Vector2(joystickWorldDirection.x, joystickWorldDirection.z) * joystickMagnitude);

            if (attackPointer != int.MinValue && Time.unscaledTime >= nextAttackRepeatAt)
            {
                TryAttack();
                nextAttackRepeatAt = Time.unscaledTime + 0.12f;
            }
            UpdateAbilityButtons();
            UpdateItemButtons();
            UpdatePickupButton();
            if (menuOpen) RefreshSkillInfo();
        }

        void OnDisable() { ClearTouches(); RestoreTimeScale(); }
        void OnApplicationFocus(bool focused) { if (!focused) ClearTouches(); }
        void OnApplicationPause(bool paused) { if (paused) ClearTouches(); }

        void ClearTouches()
        {
            joystickPointer = aimPointer = attackPointer = int.MinValue;
            joystickScreenDirection = Vector2.zero;
            joystickWorldDirection = Vector3.zero;
            joystickMagnitude = 0f;
            if (joystickKnob != null) joystickKnob.anchoredPosition = Vector2.zero;
            if (joystickBase != null) joystickBase.gameObject.SetActive(false);
            if (aimLine != null) aimLine.gameObject.SetActive(false);
            if (cancelZone != null) cancelZone.gameObject.SetActive(false);
            if (player != null && player.Motor != null) player.Motor.StopMovementImmediately();
        }

        void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null) return;
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(transform, false);
        }

        void Build()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 20);
            circleSprite = BuildCircleSprite(128);

            GameObject canvasObject = new GameObject("MobileTouchControls_Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;

            safeRoot = CreateRect(canvasObject.GetComponent<RectTransform>(), "SafeAreaRoot");
            safeRoot.pivot = Vector2.zero;
            lastSafeArea = new Rect(-1f, -1f, -1f, -1f);
            UpdateSafeArea();
            CreateMovementRegion();
            CreateJoystickVisuals();
            CreateAimVisuals();

            CreateAbilityButton("Attack", AbilitySlot.BasicAttack, new Vector2(-210f, 210f), 328f, "ATAQUE", new Color(0.90f, 0.18f, 0.09f, 0.92f));
            CreateAbilityButton("Defense", AbilitySlot.Defense, new Vector2(-555f, 205f), 232f, "ESQUIVA", new Color(0.10f, 0.48f, 0.95f, 0.92f));
            CreateAbilityButton("Movement", AbilitySlot.Movement, new Vector2(-515f, 485f), 232f, "DASH", new Color(0.62f, 0.18f, 0.98f, 0.92f));
            CreateAbilityButton("Ultimate", AbilitySlot.Ultimate, new Vector2(-235f, 615f), 260f, "ULT", new Color(1f, 0.62f, 0.04f, 0.96f));

            for (int slot = 0; slot < 4; slot++)
                CreateItemButton(slot, new Vector2(-350f + slot * 180f, 100f));

            CreatePickupButton();
            BuildSettings();
        }

        void CreateMovementRegion()
        {
            GameObject region = new GameObject("MovementTouchRegion", typeof(RectTransform), typeof(Image), typeof(EventTrigger));
            region.transform.SetParent(safeRoot, false);
            RectTransform rect = region.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(0.48f, 0.82f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            region.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.001f);
            EventTrigger trigger = region.GetComponent<EventTrigger>();
            trigger.triggers = new List<EventTrigger.Entry>();
            AddTrigger(trigger, EventTriggerType.PointerDown, BeginJoystick);
            AddTrigger(trigger, EventTriggerType.Drag, DragJoystick);
            AddTrigger(trigger, EventTriggerType.PointerUp, EndJoystick);
            AddTrigger(trigger, EventTriggerType.Cancel, EndJoystick);
        }

        void CreateJoystickVisuals()
        {
            joystickBase = CreateCircle(safeRoot, "Analogico", Vector2.zero, 224f, new Color(0.04f, 0.10f, 0.20f, 0.58f), false);
            joystickKnob = CreateCircle(joystickBase, "Analogico_Knob", Vector2.zero, 84f, new Color(0.22f, 0.75f, 1f, 0.88f), false);
            joystickKnob.anchorMin = joystickKnob.anchorMax = joystickKnob.pivot = new Vector2(0.5f, 0.5f);
            joystickBase.gameObject.SetActive(false);
        }

        void CreateAimVisuals()
        {
            GameObject lineObject = new GameObject("AimLine", typeof(RectTransform), typeof(Image));
            lineObject.transform.SetParent(safeRoot, false);
            aimLine = lineObject.GetComponent<RectTransform>();
            aimLine.anchorMin = aimLine.anchorMax = aimLine.pivot = Vector2.zero;
            aimLine.sizeDelta = new Vector2(160f, 12f);
            lineObject.GetComponent<Image>().color = new Color(0.62f, 0.38f, 1f, 0.72f);
            lineObject.SetActive(false);

            cancelZone = CreateCircle(safeRoot, "CancelAim", new Vector2(-550f, 765f), 184f, new Color(0.35f, 0.06f, 0.10f, 0.82f), false);
            cancelZone.anchorMin = cancelZone.anchorMax = new Vector2(1f, 0f);
            cancelZone.pivot = Vector2.one * 0.5f;
            AddLabel(cancelZone, "×", 48);
            cancelZone.gameObject.SetActive(false);
        }

        void CreateAbilityButton(string name, AbilitySlot slot, Vector2 position, float diameter, string label, Color color)
        {
            RectTransform rect = CreateCircle(safeRoot, name, position, diameter, color, true);
            rect.anchorMin = rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = Vector2.one * 0.5f;
            Text labelText = AddLabel(rect, label, slot == AbilitySlot.BasicAttack ? 38 : 28);

            RectTransform cooldownRect = CreateCircle(rect, "Cooldown", Vector2.zero, diameter - 8f, new Color(0.02f, 0.03f, 0.06f, 0.72f), false);
            cooldownRect.anchorMin = cooldownRect.anchorMax = cooldownRect.pivot = new Vector2(0.5f, 0.5f);
            Image cooldown = cooldownRect.GetComponent<Image>();
            cooldown.type = Image.Type.Filled;
            cooldown.fillMethod = Image.FillMethod.Radial360;
            cooldown.fillOrigin = 2;
            cooldown.fillClockwise = false;
            cooldown.raycastTarget = false;
            cooldownRect.SetAsFirstSibling();

            Text charge = AddLabel(rect, "", 28);
            charge.alignment = TextAnchor.UpperRight;
            charge.rectTransform.offsetMin = new Vector2(0f, diameter * 0.45f);
            charge.rectTransform.offsetMax = new Vector2(-8f, -5f);

            EventTrigger trigger = rect.gameObject.AddComponent<EventTrigger>();
            trigger.triggers = new List<EventTrigger.Entry>();
            if (slot == AbilitySlot.BasicAttack)
            {
                AddTrigger(trigger, EventTriggerType.PointerDown, BeginAttack);
                AddTrigger(trigger, EventTriggerType.PointerUp, EndAttack);
                AddTrigger(trigger, EventTriggerType.Cancel, EndAttack);
            }
            else if (slot == AbilitySlot.Defense)
            {
                AddTrigger(trigger, EventTriggerType.PointerDown, data => UseDefense(data as PointerEventData));
            }
            else
            {
                AddTrigger(trigger, EventTriggerType.PointerDown, data => BeginAim(slot, data as PointerEventData));
                AddTrigger(trigger, EventTriggerType.Drag, DragAim);
                AddTrigger(trigger, EventTriggerType.PointerUp, EndAim);
                AddTrigger(trigger, EventTriggerType.Cancel, CancelAim);
            }
            abilityButtons[slot] = new AbilityButtonView { slot = slot, rect = rect, cooldown = cooldown, label = labelText, charge = charge };
        }

        void CreateItemButton(int slot, Vector2 position)
        {
            RectTransform rect = CreateCircle(safeRoot, "Item_" + (slot + 1), position, 164f, new Color(0.12f, 0.27f, 0.20f, 0.86f), true);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = Vector2.one * 0.5f;
            Text label = AddLabel(rect, (slot + 1).ToString(), 25);
            RectTransform progressRect = CreateCircle(rect, "UseProgress", Vector2.zero, 152f, new Color(0.12f, 1f, 0.50f, 0.64f), false);
            progressRect.anchorMin = progressRect.anchorMax = progressRect.pivot = new Vector2(0.5f, 0.5f);
            Image progress = progressRect.GetComponent<Image>();
            progress.type = Image.Type.Filled;
            progress.fillMethod = Image.FillMethod.Radial360;
            progress.fillOrigin = 2;
            progress.fillAmount = 0f;
            progress.raycastTarget = false;
            progressRect.SetAsFirstSibling();
            EventTrigger trigger = rect.gameObject.AddComponent<EventTrigger>();
            trigger.triggers = new List<EventTrigger.Entry>();
            AddTrigger(trigger, EventTriggerType.PointerDown, data =>
            {
                if (player != null && player.Inventory != null) player.Inventory.UseSlot(slot);
            });
            itemButtons.Add(new ItemButtonView { slot = slot, rect = rect, progress = progress, label = label });
        }

        void CreatePickupButton()
        {
            pickupButton = CreateCircle(safeRoot, "Pickup", new Vector2(790f, 365f), 252f,
                new Color(0.10f, 0.72f, 0.42f, 0.94f), true);
            pickupButton.anchorMin = pickupButton.anchorMax = Vector2.zero;
            pickupButton.pivot = Vector2.one * 0.5f;
            pickupLabel = AddLabel(pickupButton, "PEGAR", 28);
            EventTrigger trigger = pickupButton.gameObject.AddComponent<EventTrigger>();
            trigger.triggers = new List<EventTrigger.Entry>();
            AddTrigger(trigger, EventTriggerType.PointerDown, data =>
            {
                if (nearbyPickup != null && player != null && nearbyPickup.TryCollect(player)) nearbyPickup = null;
            });
            pickupButton.gameObject.SetActive(false);
        }

        void UpdateSafeArea()
        {
            if (safeRoot == null || Screen.width <= 0 || Screen.height <= 0) return;
            Rect area = Screen.safeArea;
            if (area == lastSafeArea) return;
            lastSafeArea = area;
            safeRoot.anchorMin = new Vector2(area.xMin / Screen.width, area.yMin / Screen.height);
            safeRoot.anchorMax = new Vector2(area.xMax / Screen.width, area.yMax / Screen.height);
            safeRoot.offsetMin = safeRoot.offsetMax = Vector2.zero;
        }

        void BeginJoystick(BaseEventData data)
        {
            PointerEventData pointer = data as PointerEventData;
            if (pointer == null || joystickPointer != int.MinValue) return;
            joystickPointer = pointer.pointerId;
            if (!ScreenToSafe(pointer.position, out joystickOrigin)) return;
            joystickBase.gameObject.SetActive(true);
            joystickBase.anchorMin = joystickBase.anchorMax = joystickBase.pivot = Vector2.zero;
            joystickBase.anchoredPosition = joystickOrigin;
            UpdateJoystick(pointer.position);
        }

        void DragJoystick(BaseEventData data)
        {
            PointerEventData pointer = data as PointerEventData;
            if (pointer != null && pointer.pointerId == joystickPointer) UpdateJoystick(pointer.position);
        }

        void EndJoystick(BaseEventData data)
        {
            PointerEventData pointer = data as PointerEventData;
            if (pointer == null || pointer.pointerId != joystickPointer) return;
            joystickPointer = int.MinValue;
            joystickScreenDirection = Vector2.zero;
            joystickWorldDirection = Vector3.zero;
            joystickMagnitude = 0f;
            joystickKnob.anchoredPosition = Vector2.zero;
            joystickBase.gameObject.SetActive(false);
        }

        void UpdateJoystick(Vector2 screenPosition)
        {
            if (!ScreenToSafe(screenPosition, out Vector2 local)) return;
            Vector2 delta = local - joystickOrigin;
            if (delta.magnitude > JoystickRadius)
            {
                joystickOrigin = local - delta.normalized * JoystickRadius;
                joystickBase.anchoredPosition = joystickOrigin;
                delta = local - joystickOrigin;
            }
            float raw = Mathf.Clamp01(delta.magnitude / JoystickRadius);
            joystickMagnitude = raw <= JoystickDeadZone ? 0f : (raw - JoystickDeadZone) / (1f - JoystickDeadZone);
            joystickScreenDirection = delta.sqrMagnitude > 0.001f ? delta.normalized : Vector2.zero;
            joystickWorldDirection = ScreenVectorToWorld(joystickScreenDirection);
            joystickKnob.anchoredPosition = Vector2.ClampMagnitude(delta, JoystickRadius);
        }

        void BeginAttack(BaseEventData data)
        {
            PointerEventData pointer = data as PointerEventData;
            if (pointer == null || attackPointer != int.MinValue) return;
            attackPointer = pointer.pointerId;
            TryAttack();
            nextAttackRepeatAt = Time.unscaledTime + 0.12f;
        }

        void EndAttack(BaseEventData data)
        {
            PointerEventData pointer = data as PointerEventData;
            if (pointer != null && pointer.pointerId == attackPointer) attackPointer = int.MinValue;
        }

        void TryAttack()
        {
            if (player == null || player.Abilities == null) return;
            Vector3 direction = ResolveAimDirection(Vector2.zero, AbilitySlot.BasicAttack);
            player.Abilities.TryUse(AbilitySlot.BasicAttack, direction);
        }

        void UseDefense(PointerEventData pointer)
        {
            if (pointer == null || player == null || player.Abilities == null) return;
            Vector3 direction = joystickMagnitude > 0.15f ? joystickWorldDirection : player.Motor.Facing;
            player.Abilities.TryUse(AbilitySlot.Defense, direction);
        }

        void BeginAim(AbilitySlot slot, PointerEventData pointer)
        {
            if (pointer == null || aimPointer != int.MinValue) return;
            aimPointer = pointer.pointerId;
            aimSlot = slot;
            aimStart = aimCurrent = pointer.position;
            aimLine.gameObject.SetActive(true);
            cancelZone.gameObject.SetActive(true);
            UpdateAimVisual();
        }

        void DragAim(BaseEventData data)
        {
            PointerEventData pointer = data as PointerEventData;
            if (pointer == null || pointer.pointerId != aimPointer) return;
            aimCurrent = pointer.position;
            UpdateAimVisual();
        }

        void EndAim(BaseEventData data)
        {
            PointerEventData pointer = data as PointerEventData;
            if (pointer == null || pointer.pointerId != aimPointer) return;
            aimCurrent = pointer.position;
            bool cancelled = IsOverCancel(pointer.position);
            AbilitySlot slot = aimSlot;
            Vector2 drag = aimCurrent - aimStart;
            CancelAimVisuals();
            if (cancelled || player == null || player.Abilities == null) return;
            Vector3 direction = ResolveAimDirection(drag, slot);
            player.Abilities.TryUse(slot, direction);
        }

        void CancelAim(BaseEventData data)
        {
            PointerEventData pointer = data as PointerEventData;
            if (pointer != null && pointer.pointerId == aimPointer) CancelAimVisuals();
        }

        void CancelAimVisuals()
        {
            aimPointer = int.MinValue;
            aimLine.gameObject.SetActive(false);
            cancelZone.gameObject.SetActive(false);
        }

        void UpdateAimVisual()
        {
            if (!ScreenToSafe(aimStart, out Vector2 start) || !ScreenToSafe(aimCurrent, out Vector2 current)) return;
            Vector2 delta = current - start;
            float length = Mathf.Clamp(delta.magnitude, 40f, 360f);
            aimLine.anchoredPosition = start;
            aimLine.sizeDelta = new Vector2(length, aimSlot == AbilitySlot.Ultimate ? 16f : 11f);
            aimLine.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
            aimLine.GetComponent<Image>().color = IsOverCancel(aimCurrent)
                ? new Color(1f, 0.18f, 0.18f, 0.85f)
                : new Color(0.62f, 0.38f, 1f, 0.78f);
        }

        bool IsOverCancel(Vector2 screenPoint)
        {
            return cancelZone != null && RectTransformUtility.RectangleContainsScreenPoint(cancelZone, screenPoint, null);
        }

        Vector3 ResolveAimDirection(Vector2 screenDrag, AbilitySlot slot)
        {
            if (screenDrag.magnitude >= AimThreshold) return ScreenVectorToWorld(screenDrag.normalized);
            if (joystickMagnitude > 0.15f) return joystickWorldDirection;
            if (slot != AbilitySlot.Defense)
            {
                CharacterRuntime nearest = FindObjectsByType<CharacterRuntime>()
                    .Where(c => c != player && c.TeamId != player.TeamId && c.Health != null && !c.Health.IsDead &&
                        !SmokeField.BlocksSight(player.transform.position, c.transform.position))
                    .OrderBy(c => (c.transform.position - player.transform.position).sqrMagnitude)
                    .FirstOrDefault();
                if (nearest != null && Vector3.Distance(nearest.transform.position, player.transform.position) <= 6f)
                    return (nearest.transform.position - player.transform.position).normalized;
            }
            return player.Motor.Facing;
        }

        Vector3 ScreenVectorToWorld(Vector2 screenDirection)
        {
            if (worldCamera == null) worldCamera = Camera.main;
            if (worldCamera == null) return new Vector3(screenDirection.x, 0f, screenDirection.y).normalized;
            Vector3 forward = worldCamera.transform.forward;
            Vector3 right = worldCamera.transform.right;
            forward.y = right.y = 0f;
            forward.Normalize();
            right.Normalize();
            Vector3 result = right * screenDirection.x + forward * screenDirection.y;
            return result.sqrMagnitude > 0.001f ? result.normalized : Vector3.zero;
        }

        void UpdateAbilityButtons()
        {
            if (player == null || player.Abilities == null) return;
            foreach (AbilityButtonView view in abilityButtons.Values)
            {
                AbilityDefinition ability = player.Abilities.GetEquipped(view.slot);
                float remaining = player.Abilities.GetCooldownRemaining(view.slot);
                float total = ability != null ? Mathf.Max(0.01f, ability.cooldown) : 1f;
                bool hunt = view.slot == AbilitySlot.Ultimate && player.Abilities.IsHuntRecastReady;
                bool activeCharges = view.slot == AbilitySlot.Ultimate && (player.Abilities.IsChargedSequenceActive || hunt);
                view.cooldown.fillAmount = activeCharges ? 0f : Mathf.Clamp01(remaining / total);
                view.charge.text = hunt ? "2ª · " + player.Abilities.HuntTimeRemaining.ToString("0.0") + "s" :
                    activeCharges ? player.Abilities.ChargedSequenceRemaining.ToString() :
                    remaining > 0f ? remaining.ToString("0.0") + "s" : "";
                if (ability != null) view.label.text = AbilityLabel(ability, view.slot);
            }
        }

        void UpdateItemButtons()
        {
            if (player == null || player.Inventory == null) return;
            foreach (ItemButtonView view in itemButtons)
            {
                ItemDefinition item = view.slot < player.Inventory.Slots.Count ? player.Inventory.Slots[view.slot] : null;
                view.label.text = item == null ? (view.slot + 1).ToString() : ItemLabel(item);
                view.rect.GetComponent<Image>().color = item == null
                    ? new Color(0.08f, 0.10f, 0.12f, 0.58f)
                    : Color.Lerp(WorldPickup.ColorFor(item), Color.black, 0.35f);
                bool active = player.Inventory.IsUsingItem && player.Inventory.ActiveItem == item;
                view.progress.fillAmount = active ? player.Inventory.ActiveUseProgress : 0f;
            }
        }

        void UpdatePickupButton()
        {
            if (pickupButton == null || player == null) return;
            if (Time.unscaledTime >= nextPickupScanAt)
            {
                nextPickupScanAt = Time.unscaledTime + 0.10f;
                nearbyPickup = WorldPickup.FindNearestCollectible(player, 2.6f);
            }
            bool visible = nearbyPickup != null && nearbyPickup.CanCollect(player);
            pickupButton.gameObject.SetActive(visible || editingLayout);
            if (visible)
            {
                pickupLabel.text = "PEGAR\n" + ShortName(nearbyPickup.item != null ? nearbyPickup.item.displayName : "ITEM", 12);
                pickupButton.GetComponent<Image>().color = Color.Lerp(WorldPickup.ColorFor(nearbyPickup.item), Color.black, 0.18f);
            }
        }

        static string AbilityLabel(AbilityDefinition ability, AbilitySlot slot)
        {
            if (ability == null) return slot.ToString().ToUpperInvariant();
            if (slot == AbilitySlot.BasicAttack) return "ATAQUE";
            if (slot == AbilitySlot.Defense && ability.behavior == AbilityBehavior.Dodge) return "ESQUIVA";
            return ShortName(ability.displayName, 12);
        }

        static string ItemLabel(ItemDefinition item)
        {
            if (item == null) return "";
            if (item.kind == ItemKind.Heal) return "CURA";
            if (item.kind == ItemKind.Energy) return "ENERGIA";
            if (item.kind == ItemKind.CooldownRefresh) return "RECARGA";
            if (item.kind == ItemKind.Tactical && item.tacticalKind == TacticalKind.Smoke) return "FUMAÇA";
            if (item.kind == ItemKind.Variation) return "RUNA\n" + ShortName(item.displayName.Replace("Runa: ", ""), 8);
            return ShortName(item.displayName, 9);
        }

        static string ShortName(string value, int max)
        {
            if (string.IsNullOrWhiteSpace(value)) return "ITEM";
            string clean = value.Replace("Runa: ", "").Trim().ToUpperInvariant();
            if (clean.Length <= max) return clean;
            int space = clean.LastIndexOf(' ', Mathf.Min(max, clean.Length - 1));
            return clean.Substring(0, space > 3 ? space : max);
        }

        bool ScreenToSafe(Vector2 screen, out Vector2 local) => RectTransformUtility.ScreenPointToLocalPointInRectangle(safeRoot, screen, null, out local);

        RectTransform CreateCircle(RectTransform parent, string name, Vector2 position, float diameter, Color color, bool raycast)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = Vector2.one * diameter;
            Image image = go.GetComponent<Image>();
            image.sprite = circleSprite;
            image.color = color;
            image.raycastTarget = raycast;
            if (raycast) go.AddComponent<CircleRaycastFilter>();
            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(0.80f, 0.92f, 1f, 0.76f);
            outline.effectDistance = new Vector2(2f, -2f);
            return rect;
        }

        Text AddLabel(RectTransform parent, string value, int size)
        {
            GameObject go = new GameObject("Label", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            Text text = go.GetComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.raycastTarget = false;
            text.text = value;
            return text;
        }

        static RectTransform CreateRect(RectTransform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(data => { if (!menuOpen && !editingLayout) callback(data); });
            trigger.triggers.Add(entry);
        }

        static Sprite BuildCircleSprite(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { name = "BRX_RuntimeCircle" };
            Color32[] pixels = new Color32[size * size];
            float center = (size - 1) * 0.5f;
            float radius = center - 1f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                byte alpha = (byte)Mathf.RoundToInt(Mathf.Clamp01(radius - distance + 1f) * 255f);
                pixels[y * size + x] = new Color32(255, 255, 255, alpha);
            }
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            return Sprite.Create(texture, new Rect(0f, 0f, size, size), Vector2.one * 0.5f, 100f);
        }
    }

    public sealed class CircleRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
    {
        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            RectTransform rect = transform as RectTransform;
            if (rect == null || !RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, eventCamera, out Vector2 local)) return false;
            Vector2 centered = local - rect.rect.center;
            Vector2 normalized = new Vector2(centered.x / Mathf.Max(1f, rect.rect.width), centered.y / Mathf.Max(1f, rect.rect.height));
            return normalized.sqrMagnitude <= 0.25f;
        }
    }
}
