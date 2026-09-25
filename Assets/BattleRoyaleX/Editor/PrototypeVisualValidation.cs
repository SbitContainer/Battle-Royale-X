#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleRoyaleX.EditorTools
{
    public static class PrototypeVisualValidation
    {
        const string ScenePath = "Assets/BattleRoyaleX/GeneratedScenes/Prototype01_Arena.unity";
        const string ControllerPath = "Assets/BattleRoyaleX/GeneratedVisuals/BRX_CombatV2.controller";

        static readonly List<string> failures = new List<string>();
        static int passCount;

        [MenuItem("Battle Royale X/Prototype 01/Validate Visual Scene")]
        public static void ValidateVisualScene()
        {
            failures.Clear();
            passCount = 0;

            ValidateModelAsset(PrototypeVisualFactory.MaleModelPath, "Guerreiro");
            ValidateModelAsset(PrototypeVisualFactory.FemaleModelPath, "Assassino");
            ValidateAnimationAssets();
            ValidateController();
            ValidateGeneratedScene();

            Debug.Log($"[BRX VISUAL SUMMARY] pass={passCount} fail={failures.Count}");
            if (failures.Count > 0)
                throw new InvalidOperationException("Battle Royale X visual validation failed: " + string.Join(" | ", failures));
        }

        static void ValidateModelAsset(string path, string label)
        {
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Check(model != null, $"Assets/{label}: modelo existe");
            if (model == null) return;

            Animator animator = model.GetComponentInChildren<Animator>(true);
            Check(animator != null && animator.avatar != null && animator.avatar.isValid && animator.avatar.isHuman,
                $"Assets/{label}: avatar humanoide valido");
            Check(model.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length > 0,
                $"Assets/{label}: malha skinned existe");
        }

        static void ValidateAnimationAssets()
        {
            string[] requiredLocomotion = { "Armature|Idle_Loop", "Armature|Jog_Fwd_Loop", "Armature|Roll", "Armature|Hit_Chest", "Armature|Death01" };
            string[] requiredCombat = { "Armature|Sword_Regular_A", "Armature|Sword_Regular_B", "Armature|Sword_Regular_C",
                "Armature|Sword_Block", "Armature|Sword_Dash", "Armature|Shield_Dash", "Armature|Shield_OneShot" };
            ValidateClips(PrototypeVisualFactory.LocomotionPath, requiredLocomotion, "locomocao");
            ValidateClips(PrototypeVisualFactory.CombatPath, requiredCombat, "combate");
        }

        static void ValidateClips(string path, IEnumerable<string> required, string label)
        {
            HashSet<string> names = new HashSet<string>(AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<AnimationClip>().Select(clip => clip.name));
            Check(required.All(names.Contains), $"Animacoes/{label}: clips obrigatorios existem");
        }

        static void ValidateController()
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            Check(controller != null, "Animator/controller gerado existe");
            if (controller == null) return;

            string[] requiredParameters = { "MoveAmount", "Attack1", "Attack2", "Attack3", "Defense", "MoveAssassin",
                "MoveWarrior", "UltimateDash", "UltimateAssassin", "UltimateWarrior", "Hit", "Dead" };
            HashSet<string> parameters = new HashSet<string>(controller.parameters.Select(parameter => parameter.name));
            Check(requiredParameters.All(parameters.Contains), "Animator/parametros de apresentacao completos");

            string[] requiredStates = { "Locomotion", "Attack1", "Attack2", "Attack3", "Defense", "MoveAssassin",
                "MoveWarrior", "UltimateDash", "UltimateAssassin", "UltimateWarrior", "Hit", "Death" };
            HashSet<string> states = new HashSet<string>(controller.layers[0].stateMachine.states.Select(state => state.state.name));
            Check(requiredStates.All(states.Contains), "Animator/estados de apresentacao completos");
        }

        static void ValidateGeneratedScene()
        {
            Check(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null, "Cena/asset gerado existe");
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            CharacterRuntime[] characters = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<CharacterRuntime>(true)).ToArray();
            Check(characters.Length == 2, "Cena/exatamente dois personagens logicos");

            foreach (CharacterRuntime character in characters)
                ValidateCharacter(character);

            CombatEventVfxPresenter vfx = UnityEngine.Object.FindAnyObjectByType<CombatEventVfxPresenter>();
            Check(vfx != null, "Cena/apresentador de VFX de combate existe");

            PrototypeCombatHUD hud = UnityEngine.Object.FindAnyObjectByType<PrototypeCombatHUD>();
            Check(hud != null && hud.playerOne != null && hud.playerTwo != null,
                "Cena/HUD de combate referencia os dois jogadores");

            Check(UnityEngine.Object.FindAnyObjectByType<PrototypeMobileTouchControls>() != null,
                "Cena/controles touch para teste Android existem");
            Check(UnityEngine.Object.FindAnyObjectByType<PrototypeTrainingBot>() != null,
                "Cena/bot de treino do Guerreiro existe");
            PrototypeLabController lab = UnityEngine.Object.FindAnyObjectByType<PrototypeLabController>();
            Check(lab != null && lab.warriorDefinition != null && lab.assassinDefinition != null &&
                lab.mageDefinition != null && lab.archerDefinition != null,
                "Cena/laboratório possui as quatro classes e adversário Guerreiro");
            Check(UnityEngine.Object.FindAnyObjectByType<CombatVFXRouter>() != null,
                "Cena/router de VFX desacoplado existe");
            Check(UnityEngine.Object.FindObjectsByType<ArcaneDemoObject>().Select(x => x.kind).Distinct().Count() == 6,
                "Cena/seis objetos arcanos de demonstração existem");

            CharacterDefinition[] definitions = { lab?.warriorDefinition, lab?.assassinDefinition,
                lab?.mageDefinition, lab?.archerDefinition };
            Check(definitions.All(d => d != null && d.basicAttack != null &&
                Enumerable.Range(0,3).All(i => d.GetVariant(AbilitySlot.Skill1,i) != null &&
                    d.GetVariant(AbilitySlot.Skill2,i) != null && d.GetVariant(AbilitySlot.Ultimate,i) != null)),
                "Dados/4 Basics e 36 habilidades A-B-C estão completos");

            WorldPickup[] pickups = UnityEngine.Object.FindObjectsByType<WorldPickup>(FindObjectsSortMode.None);
            string[] expectedVariations = { "Var_Assassin_Counter","Var_Assassin_Double","Var_Assassin_Travel","Var_Assassin_Return","Var_Assassin_Exec","Var_Assassin_Hunt",
                "Var_Warrior_Parry","Var_Warrior_Fortress","Var_Warrior_Impact","Var_Warrior_Advance","Var_Warrior_Ret","Var_Warrior_Push" };
            Check(expectedVariations.All(id => pickups.Any(p => p.item != null && p.item.itemId == id)),
                "Cena/todas as 12 variações de habilidade estão no chão");
            Check(pickups.Any(p => p.item != null && p.item.kind == ItemKind.Heal),
                "Cena/poção de cura está disponível no chão");
            Check(pickups.Any(p => p.item != null && p.item.tacticalKind == TacticalKind.Smoke),
                "Cena/granada de fumaça está disponível no chão");
            Check(pickups.All(p => p.GetComponent<Collider>() != null && p.GetComponent<Collider>().isTrigger),
                "Cena/pickups possuem área de coleta sem bloquear movimento");

            IsometricCameraRig cameraRig = UnityEngine.Object.FindAnyObjectByType<IsometricCameraRig>();
            Check(cameraRig != null && cameraRig.targetA != null && cameraRig.targetB != null,
                "Cena/camera preserva os dois alvos");
        }

        static void ValidateCharacter(CharacterRuntime character)
        {
            string label = character != null && character.Definition != null ? character.Definition.displayName : character.name;
            Check(character.GetComponent<CharacterController>() != null, $"Cena/{label}: CharacterController no root logico");
            Check(character.GetComponentInChildren<Hurtbox>(true) != null, $"Cena/{label}: Hurtbox preservada");
            Check(character.GetComponent<Renderer>() == null, $"Cena/{label}: root logico sem renderer");

            Transform visual = character.transform.Find("VisualModel");
            Check(visual != null, $"Cena/{label}: VisualModel filho existe");
            if (visual == null) return;

            Animator animator = visual.GetComponent<Animator>();
            Check(animator != null && animator.runtimeAnimatorController != null,
                $"Cena/{label}: Animator configurado");
            Check(animator != null && !animator.applyRootMotion,
                $"Cena/{label}: root motion desativado");
            Check(visual.GetComponent<CharacterVisualAnimator>() != null,
                $"Cena/{label}: ponte visual configurada");
            Check(visual.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length > 0,
                $"Cena/{label}: modelo renderizavel");
            Check(character.transform.Find("FallbackCapsuleVisual") == null,
                $"Cena/{label}: sem fallback de capsula");
        }

        static void Check(bool condition, string name)
        {
            if (condition)
            {
                passCount++;
                Debug.Log("[BRX VISUAL PASS] " + name);
                return;
            }

            failures.Add(name);
            Debug.LogError("[BRX VISUAL FAIL] " + name);
        }
    }
}
#endif
