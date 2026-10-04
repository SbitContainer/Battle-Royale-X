#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleRoyaleX
{
    public sealed partial class PrototypeLiveTests
    {
        IEnumerator TestIndependentMobileMotion()
        {
            stage = "Input: movimento e mira independentes";
            ResetPair(20f);
            var go = new GameObject("Test_IndependentMobileMotion");
            var controls = go.AddComponent<PrototypeMobileTouchControls>();
            controls.runInEditor = true;
            yield return null; yield return null;
            controls.SelectPlayer(a);
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            var transforms = controls.GetComponentsInChildren<RectTransform>(true);
            var region = transforms.First(t => t.name == "MovementTouchRegion");
            var skill = transforms.First(t => t.name == "Movement");
            var line = transforms.First(t => t.name == "AimLine");
            var start = RectTransformUtility.WorldToScreenPoint(null, region.position);
            var skillPosition = RectTransformUtility.WorldToScreenPoint(null, skill.position);
            var move = new PointerEventData(EventSystem.current) { pointerId = 71, position = start };
            var aim = new PointerEventData(EventSystem.current) { pointerId = 92, position = skillPosition };
            ExecuteEvents.Execute(skill.gameObject, aim, ExecuteEvents.pointerDownHandler);
            Check(line.gameObject.activeSelf && !line.GetComponent<Image>().raycastTarget,
                "Linha de mira não intercepta toques do analógico");
            ExecuteEvents.Execute(region.gameObject, move, ExecuteEvents.pointerDownHandler);
            var method = typeof(PrototypeMobileTouchControls).GetMethod("ApplyJoystickTouch", BindingFlags.Instance | BindingFlags.NonPublic);
            var pointerField = typeof(PrototypeMobileTouchControls).GetField("joystickPointer", BindingFlags.Instance | BindingFlags.NonPublic);
            var directionField = typeof(PrototypeMobileTouchControls).GetField("joystickWorldDirection", BindingFlags.Instance | BindingFlags.NonPublic);
            var aimField = typeof(PrototypeMobileTouchControls).GetField("aimPointer", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(controls, new object[] { 71, start + Vector2.right * 80f, TouchPhase.Moved });
            var before = a.transform.position;
            yield return new WaitForSeconds(0.18f);
            var first = (Vector3)directionField.GetValue(controls);
            Check((a.transform.position - before).magnitude > 0.1f && (int)aimField.GetValue(controls) == 92,
                "Segurar skill primeiro permite iniciar e mover analógico com outro dedo");
            method.Invoke(controls, new object[] { 92, start - Vector2.right * 80f, TouchPhase.Moved });
            Check((Vector3)directionField.GetValue(controls) == first,
                "Dedo da skill não altera direção do analógico");
            method.Invoke(controls, new object[] { 71, start - Vector2.right * 80f, TouchPhase.Moved });
            Check(Vector3.Dot(first, (Vector3)directionField.GetValue(controls)) < -0.9f,
                "Analógico muda direção enquanto mantém mira da skill");
            method.Invoke(controls, new object[] { 71, start, TouchPhase.Ended });
            yield return new WaitForSeconds(0.3f);
            var stopped = a.transform.position;
            yield return new WaitForSeconds(0.15f);
            Check((int)pointerField.GetValue(controls) == int.MinValue && (int)aimField.GetValue(controls) == 92 &&
                (a.transform.position - stopped).magnitude < 0.03f,
                "Soltar analógico para movimento sem soltar ou lançar skill");
            ExecuteEvents.Execute(region.gameObject, move, ExecuteEvents.pointerDownHandler);
            method.Invoke(controls, new object[] { 71, start + Vector2.right * 80f, TouchPhase.Stationary });
            ExecuteEvents.Execute(skill.gameObject, aim, ExecuteEvents.cancelHandler);
            Check((int)pointerField.GetValue(controls) == 71 && (int)aimField.GetValue(controls) == int.MinValue,
                "Cancelar mira não cancela dedo de movimento");
            method.Invoke(controls, new object[] { 71, start, TouchPhase.Canceled });
            Check((int)pointerField.GetValue(controls) == int.MinValue && (Vector3)directionField.GetValue(controls) == Vector3.zero,
                "Cancelamento físico limpa direção antiga do analógico");
            ExecuteEvents.Execute(region.gameObject, move, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(skill.gameObject, aim, ExecuteEvents.pointerDownHandler);
            controls.OpenSettings();
            Check((int)pointerField.GetValue(controls) == int.MinValue && (int)aimField.GetValue(controls) == int.MinValue,
                "Menu limpa os dois dedos sem movimento preso");
            controls.CloseSettings();
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            Destroy(go); yield return null;
            ResetPair(5f);
        }
    }
}
#endif
