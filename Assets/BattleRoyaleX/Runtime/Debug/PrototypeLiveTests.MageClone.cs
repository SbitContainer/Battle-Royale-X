#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleRoyaleX
{
    public sealed partial class PrototypeLiveTests
    {
        IEnumerator TestMageCloneTouch011(PrototypeLabController lab)
        {
            stage = "BUG011: Ecos Arcanos via botão, render e segunda interação";
            ResetPair(12f); lab.EquipVariation(AbilitySlot.Skill2, 1);
            var ui = new GameObject("BUG011_TouchFixture");
            var controls = ui.AddComponent<PrototypeMobileTouchControls>(); controls.runInEditor = true;
            yield return null; yield return null;
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            ResetPair(12f); lab.EquipVariation(AbilitySlot.Skill2, 1);
            var movement = controls.GetComponentsInChildren<RectTransform>(true).First(r => r.name == "Movement");
            var attack = controls.GetComponentsInChildren<RectTransform>(true).First(r => r.name == "Attack");
            var pointer = new PointerEventData(EventSystem.current) { pointerId = 181,
                position = RectTransformUtility.WorldToScreenPoint(null, movement.position) };
            ExecuteEvents.Execute(movement.gameObject, pointer, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(movement.gameObject, pointer, ExecuteEvents.pointerUpHandler);
            yield return new WaitForSeconds(.28f);
            var clones = FindObjectsByType<ArcaneCloneMotion>();
            var projections = FindObjectsByType<ArcaneProjectionResources>();
            Check(clones.Length == 3 && projections.Length == 3 && !a.Abilities.IsActionBusy &&
                a.Abilities.CanRecast(AbilitySlot.Skill2), "BUG011 toque cria três clones e libera a ação sem perder recast");
            Check(projections.Length == 3 && projections.All(p => p.material != null &&
                p.material.shader == Resources.Load<Shader>("BRXCombatFx") && p.material.shader.isSupported &&
                p.material.GetFloat("_Mode") == 1f && p.meshes.All(m => m.vertexCount > 0)),
                "BUG011 projeções usam shader Resources incluído no Android e malhas reais");
            Check(CaptureMageClones011(projections) > 120,
                "BUG011 clones produzem pixels visíveis na câmera, não apenas objetos existentes");

            var basic = new PointerEventData(EventSystem.current) { pointerId = 182,
                position = RectTransformUtility.WorldToScreenPoint(null, attack.position) };
            events.Clear();
            ExecuteEvents.Execute(attack.gameObject, basic, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(attack.gameObject, basic, ExecuteEvents.pointerUpHandler);
            yield return new WaitForSeconds(.65f);
            Check(events.Any(e => e.source == a && e.ability == a.Definition.basicAttack && e.phase == AbilityPhase.Active) &&
                !a.Abilities.IsActionBusy && a.Abilities.CanRecast(AbilitySlot.Skill2),
                "BUG011 ataque básico não fica bloqueado pela escolha de clone");
            var skill = controls.GetComponentsInChildren<RectTransform>(true).First(r => r.name == "Defense");
            var skillPointer = new PointerEventData(EventSystem.current) { pointerId = 183,
                position = RectTransformUtility.WorldToScreenPoint(null, skill.position) };
            events.Clear();
            ExecuteEvents.Execute(skill.gameObject, skillPointer, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(skill.gameObject, skillPointer, ExecuteEvents.pointerUpHandler);
            yield return new WaitForSeconds(.55f);
            Check(events.Any(e => e.source == a && e.ability == a.Abilities.GetEquipped(AbilitySlot.Skill1) &&
                e.phase == AbilityPhase.Active) && !a.Abilities.IsActionBusy && a.Abilities.CanRecast(AbilitySlot.Skill2),
                "BUG011 outra skill executa pelo botão enquanto a escolha de clone continua disponível");
            if (clones.Length > 0 && clones[0] != null)
            {
                Vector3 destination = clones[0].transform.position;
                Vector3 screenDirection = Camera.main.WorldToScreenPoint(destination) - Camera.main.WorldToScreenPoint(a.transform.position);
                ExecuteEvents.Execute(movement.gameObject, pointer, ExecuteEvents.pointerDownHandler);
                pointer.position += new Vector2(screenDirection.x, screenDirection.y).normalized * 160f;
                ExecuteEvents.Execute(movement.gameObject, pointer, ExecuteEvents.dragHandler);
                yield return new WaitForSeconds(.05f);
                ExecuteEvents.Execute(movement.gameObject, pointer, ExecuteEvents.pointerUpHandler);
                yield return null; yield return null;
                Check(Vector3.Distance(a.transform.position, destination) < .6f &&
                    FindObjectsByType<ArcaneCloneMotion>().Length == 0 && !a.Abilities.IsActionBusy,
                    "BUG011 segundo toque arrastado escolhe o clone e encerra a janela sem travar");
            }
            else Check(false, "BUG011 clone disponível para escolha por arraste");

            ResetPair(12f); lab.EquipVariation(AbilitySlot.Skill2, 1);
            a.Abilities.TryUse(AbilitySlot.Skill2); yield return new WaitForSeconds(3.1f);
            Check(FindObjectsByType<ArcaneCloneMotion>().Length == 0 && !a.Abilities.IsActionBusy &&
                !a.Abilities.CanRecast(AbilitySlot.Skill2) && a.Abilities.TryUse(AbilitySlot.BasicAttack),
                "BUG011 expirar sem escolher clone limpa ecos e permite outro ataque");
            ResetPair(12f); lab.EquipVariation(AbilitySlot.Skill2, 1);
            a.Abilities.TryUse(AbilitySlot.Skill2); yield return new WaitForSeconds(.28f);
            lab.SwitchPlayerClass(CharacterClass.Archer);
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            yield return null; yield return null;
            Check(FindObjectsByType<ArcaneCloneMotion>().Length == 0 && !a.Abilities.IsActionBusy,
                "BUG011 trocar classe cancela escolha de clone e não deixa ação presa");
            Destroy(ui); lab.SwitchPlayerClass(CharacterClass.Mage);
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            yield return null;
        }

        int CaptureMageClones011(ArcaneProjectionResources[] projections)
        {
            Camera camera = Camera.main;
            if (camera == null) return 0;
            Vector3 position = camera.transform.position; Quaternion rotation = camera.transform.rotation;
            float fov = camera.fieldOfView;
            var target = new RenderTexture(1280, 720, 24);
            var shown = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            var hidden = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            RenderTexture old = camera.targetTexture, active = RenderTexture.active;
            Renderer[] renderers = projections.SelectMany(p => p.GetComponentsInChildren<Renderer>()).ToArray();
            bool[] enabled = renderers.Select(r => r.enabled).ToArray();
            try
            {
                camera.transform.position = a.transform.position + new Vector3(7, 10, -9);
                camera.transform.LookAt(a.transform.position + Vector3.up * .7f); camera.fieldOfView = 45;
                camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
                shown.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); shown.Apply();
                foreach (Renderer renderer in renderers) renderer.enabled = false;
                camera.Render(); RenderTexture.active = target;
                hidden.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); hidden.Apply();
                System.IO.Directory.CreateDirectory("Logs/Bug011");
                System.IO.File.WriteAllBytes("Logs/Bug011/mage-clones-visible.png", shown.EncodeToPNG());
                System.IO.File.WriteAllBytes("Logs/Bug011/mage-clones-hidden-control.png", hidden.EncodeToPNG());
                Color32[] withClones = shown.GetPixels32(), withoutClones = hidden.GetPixels32();
                int pixels = 0;
                for (int i = 0; i < withClones.Length; i++)
                    if (Mathf.Abs(withClones[i].r - withoutClones[i].r) + Mathf.Abs(withClones[i].g - withoutClones[i].g) +
                        Mathf.Abs(withClones[i].b - withoutClones[i].b) > 30) pixels++;
                Debug.Log("[BRX BUG011 RENDER] changedPixels=" + pixels);
                return pixels;
            }
            finally
            {
                for (int i = 0; i < renderers.Length; i++) renderers[i].enabled = enabled[i];
                camera.targetTexture = old; RenderTexture.active = active;
                camera.transform.SetPositionAndRotation(position, rotation); camera.fieldOfView = fov;
                Destroy(shown); Destroy(hidden); target.Release(); Destroy(target);
            }
        }
    }
}
#endif
