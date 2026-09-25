#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BattleRoyaleX.EditorTools
{
    [InitializeOnLoad]
    public static class PrototypeVisualRuntimeSmoke
    {
        const string ScenePath = "Assets/BattleRoyaleX/GeneratedScenes/Prototype01_Arena.unity";
        const string ActiveKey = "BRX.VisualSmoke.Active";
        const string BatchKey = "BRX.VisualSmoke.Batch";
        const string ResultReadyKey = "BRX.VisualSmoke.ResultReady";
        const string PassKey = "BRX.VisualSmoke.Pass";
        const string FailKey = "BRX.VisualSmoke.Fail";

        static readonly List<string> failures = new List<string>();
        static double phaseStartedAt;
        static int phase;
        static int passCount;
        static CharacterRuntime warrior;
        static Animator warriorAnimator;
        static Vector3 stationaryRootPosition;

        static PrototypeVisualRuntimeSmoke()
        {
            EditorApplication.update += ResumeOrFinish;
        }

        [MenuItem("Battle Royale X/Prototype 01/Run Visual Runtime Smoke")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Exit Play Mode before starting the visual smoke test.");

            SessionState.SetBool(ActiveKey, true);
            SessionState.SetBool(BatchKey, Application.isBatchMode);
            SessionState.SetBool(ResultReadyKey, false);
            SessionState.SetInt(PassKey, 0);
            SessionState.SetInt(FailKey, 0);
            EditorSceneManager.OpenScene(ScenePath);
            EditorApplication.EnterPlaymode();
        }

        public static void RunBatch() => Run();

        static void ResumeOrFinish()
        {
            if (!SessionState.GetBool(ActiveKey, false)) return;

            if (SessionState.GetBool(ResultReadyKey, false) && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                FinishAfterPlayMode();
                return;
            }

            if (!EditorApplication.isPlaying || phase != 0) return;

            failures.Clear();
            passCount = 0;
            phase = 1;
            phaseStartedAt = Time.time;
        }

        static void TickPlayMode()
        {
            if (!EditorApplication.isPlaying || phase == 0) return;
            double elapsed = Time.time - phaseStartedAt;

            if (phase == 1 && elapsed >= 0.35d)
            {
                CharacterRuntime[] characters = UnityEngine.Object.FindObjectsByType<CharacterRuntime>();
                Check(characters.Length == 2, "Runtime/exatamente dois personagens ativos");
                warrior = characters.FirstOrDefault(character => character.Definition != null && character.Definition.characterClass == CharacterClass.Warrior);
                CharacterRuntime assassin = characters.FirstOrDefault(character => character.Definition != null && character.Definition.characterClass == CharacterClass.Assassin);
                Check(warrior != null && assassin != null, "Runtime/Guerreiro e Assassino inicializados");

                foreach (CharacterRuntime character in characters)
                {
                    PrototypeLocalInput localInput = character.GetComponent<PrototypeLocalInput>();
                    if (localInput != null) localInput.enabled = false;
                    PrototypeTrainingBot trainingBot = character.GetComponent<PrototypeTrainingBot>();
                    if (trainingBot != null) trainingBot.enabled = false;
                }

                Animator[] animators = characters.Select(character => character.GetComponentInChildren<Animator>(true)).ToArray();
                Check(animators.All(animator => animator != null && animator.isActiveAndEnabled), "Runtime/Animators ativos");
                Check(animators.All(animator => animator.avatar != null && animator.avatar.isValid && animator.avatar.isHuman), "Runtime/avatares humanoides validos");
                Check(animators.All(animator => !animator.applyRootMotion), "Runtime/root motion desativado");

                if (warrior != null)
                {
                    warriorAnimator = warrior.GetComponentInChildren<Animator>(true);
                    warrior.Motor.SetMoveInput(Vector2.right);
                    Check(warrior.Abilities.TryUse(AbilitySlot.BasicAttack), "Runtime/ataque continua aceito pela logica");
                    CombatEvents.Raise(new CombatEventData(CombatEventKind.Hit, warrior.transform.position, warrior, assassin, 1f));
                }

                phase = 2;
                phaseStartedAt = Time.time;
                return;
            }

            if (phase == 2 && elapsed >= 0.18d)
            {
                if (warriorAnimator != null)
                {
                    Check(warriorAnimator.GetFloat("MoveAmount") > 0.05f, "Runtime/movimento alimenta o Animator");
                    AnimatorStateInfo current = warriorAnimator.GetCurrentAnimatorStateInfo(0);
                    AnimatorStateInfo next = warriorAnimator.GetNextAnimatorStateInfo(0);
                    Check(current.IsName("Attack1") || next.IsName("Attack1"), "Runtime/ataque dispara estado visual");
                }
                if (warrior != null)
                {
                    float groundY = VisualGroundY(warrior);
                    Check(Mathf.Abs(groundY) < 0.16f, $"Runtime/pes permanecem no chao durante golpe (y={groundY:0.000})");
                }

                Check(UnityEngine.Object.FindObjectsByType<ParticleSystem>()
                    .Any(particles => particles.name == "VFX_CombatBurst"), "Runtime/evento de combate gera VFX");
                Check(UnityEngine.Object.FindObjectsByType<ParticleSystem>()
                    .Any(particles => particles.name == "VFX_BloodImpact"), "Runtime/contato de dano gera sangue sem sprite quadrado");

                PrototypeCombatHUD hud = UnityEngine.Object.FindAnyObjectByType<PrototypeCombatHUD>();
                Check(hud != null && hud.GetComponentInChildren<Canvas>(true) != null,
                    "Runtime/HUD de combate cria Canvas responsivo");
                Check(hud != null && hud.GetComponentsInChildren<UnityEngine.UI.Text>(true).Length >= 10,
                    "Runtime/HUD apresenta vida, energia, habilidades e inventario");

                if (warrior != null)
                {
                    warrior.Motor.StopMovementImmediately();
                    // The check below is about animation root motion, not the intentional analogue easing in the motor.
                    warrior.Motor.enabled = false;
                    CharacterController controller = warrior.GetComponent<CharacterController>();
                    if (controller != null) controller.enabled = false;
                    stationaryRootPosition = warrior.transform.position;
                }

                phase = 3;
                phaseStartedAt = Time.time;
                return;
            }

            if (phase == 3 && elapsed >= 0.32d)
            {
                if (warrior != null)
                {
                    Vector3 delta = warrior.transform.position - stationaryRootPosition;
                    Check(new Vector2(delta.x, delta.z).magnitude < 0.05f, "Runtime/animacao nao desloca o root logico");
                    Check(Mathf.Abs(warrior.transform.position.y - 1f) < 0.02f, "Runtime/personagem permanece no plano XZ");
                    float groundY = VisualGroundY(warrior);
                    Check(Mathf.Abs(groundY) < 0.16f, $"Runtime/pes permanecem no chao na recuperacao (y={groundY:0.000})");
                }

                CharacterRuntime[] characters = UnityEngine.Object.FindObjectsByType<CharacterRuntime>();
                Check(characters.All(character => character.transform.Find("VisualModel") != null), "Runtime/modelos visuais permanecem anexados");
                Check(characters.All(character => character.transform.Find("FallbackCapsuleVisual") == null), "Runtime/nenhuma capsula de fallback ativa");
                CompletePlayMode();
            }
        }

        static void CompletePlayMode()
        {
            phase = 0;
            SessionState.SetInt(PassKey, passCount);
            SessionState.SetInt(FailKey, failures.Count);
            SessionState.SetBool(ResultReadyKey, true);
            Debug.Log($"[BRX RUNTIME SUMMARY] pass={passCount} fail={failures.Count}");
            System.IO.File.WriteAllText("Logs/polish-smoke.txt", $"pass={passCount} fail={failures.Count}\n"+string.Join("\n",failures));
            EditorApplication.ExitPlaymode();
        }

        static void FinishAfterPlayMode()
        {
            bool batch = SessionState.GetBool(BatchKey, false);
            int pass = SessionState.GetInt(PassKey, 0);
            int fail = SessionState.GetInt(FailKey, 0);
            SessionState.EraseBool(ActiveKey);
            SessionState.EraseBool(BatchKey);
            SessionState.EraseBool(ResultReadyKey);
            SessionState.EraseInt(PassKey);
            SessionState.EraseInt(FailKey);
            Debug.Log($"[BRX RUNTIME FINAL] pass={pass} fail={fail}");
            if (batch) EditorApplication.Exit(fail == 0 ? 0 : 1);
        }

        static void Check(bool condition, string name)
        {
            if (condition)
            {
                passCount++;
                Debug.Log("[BRX RUNTIME PASS] " + name);
            }
            else
            {
                failures.Add(name);
                Debug.LogError("[BRX RUNTIME FAIL] " + name);
            }
        }

        static float VisualGroundY(CharacterRuntime character)
        {
            Animator animator = character.GetComponentInChildren<Animator>(true);
            if (animator != null && animator.isHuman)
            {
                Transform left = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                Transform right = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                if (left != null && right != null) return Mathf.Min(left.position.y, right.position.y);
            }
            return float.PositiveInfinity;
        }

        [InitializeOnEnterPlayMode]
        static void RegisterPlayModeTick(EnterPlayModeOptions options)
        {
            EditorApplication.update -= TickPlayMode;
            EditorApplication.update += TickPlayMode;
        }
    }
}
#endif
