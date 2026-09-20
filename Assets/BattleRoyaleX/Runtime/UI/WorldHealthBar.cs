using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyaleX
{
    [DisallowMultipleComponent]
    public sealed class WorldHealthBar : MonoBehaviour
    {
        CharacterRuntime runtime;
        RectTransform fill;
        Camera worldCamera;

        void Start()
        {
            runtime = GetComponent<CharacterRuntime>();
            if (runtime == null || runtime.Health == null) { enabled = false; return; }
            Build();
            runtime.Health.Changed += OnHealthChanged;
            OnHealthChanged(runtime.Health.CurrentHealth, runtime.Health.MaxHealth);
        }

        void OnDestroy()
        {
            if (runtime != null && runtime.Health != null) runtime.Health.Changed -= OnHealthChanged;
        }

        void LateUpdate()
        {
            if (worldCamera == null) worldCamera = Camera.main;
            if (worldCamera != null) transform.Find("WorldHealthBar").rotation = worldCamera.transform.rotation;
        }

        void Build()
        {
            GameObject root = new GameObject("WorldHealthBar", typeof(RectTransform), typeof(Canvas));
            root.transform.SetParent(transform, false);
            RectTransform rect = (RectTransform)root.transform;
            rect.localPosition = new Vector3(0f, 2.35f, 0f);
            rect.sizeDelta = new Vector2(180f, 18f);
            rect.localScale = Vector3.one * 0.01f;
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 20;

            Image back = CreateImage(root.transform, "Back", new Color(0.035f, 0.04f, 0.05f, 0.88f));
            back.rectTransform.anchorMin = Vector2.zero;
            back.rectTransform.anchorMax = Vector2.one;
            back.rectTransform.offsetMin = Vector2.zero;
            back.rectTransform.offsetMax = Vector2.zero;

            Image front = CreateImage(root.transform, "Fill", new Color(0.18f, 0.92f, 0.32f, 1f));
            fill = front.rectTransform;
            fill.anchorMin = Vector2.zero;
            fill.anchorMax = Vector2.one;
            fill.pivot = new Vector2(0f, 0.5f);
            fill.offsetMin = new Vector2(2f, 2f);
            fill.offsetMax = new Vector2(-2f, -2f);
        }

        static Image CreateImage(Transform parent, string name, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        void OnHealthChanged(float current, float maximum)
        {
            if (fill == null) return;
            Vector2 max = fill.anchorMax;
            max.x = Mathf.Clamp01(current / Mathf.Max(1f, maximum));
            fill.anchorMax = max;
        }
    }
}
