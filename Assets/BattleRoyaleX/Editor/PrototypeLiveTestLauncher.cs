#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BattleRoyaleX.EditorTools
{
    [InitializeOnLoad]
    public static class PrototypeLiveTestLauncher
    {
        const string Pending = "BRX.LiveTests.Pending";
        const string Batch = "BRX.LiveTests.Batch";

        static PrototypeLiveTestLauncher()
        {
            Application.logMessageReceived += (message, stack, type) =>
            {
                if (!SessionState.GetBool(Batch, false) || !message.StartsWith("[BRX LIVE FINAL]")) return;
                SessionState.SetBool(Batch, false);
                int exitCode = message.Contains(" fail=0 ") ? 0 : 1;
                EditorApplication.delayCall += () => EditorApplication.Exit(exitCode);
            };
            EditorApplication.playModeStateChanged += state =>
            {
                if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(Pending, false)) return;
                SessionState.SetBool(Pending, false);
                new GameObject("Live_Test_Runner_Editor_Only").AddComponent<PrototypeLiveTests>();
            };
        }

        // Do not use -quit: the editor must remain alive until the Play Mode coroutine completes.
        public static void BuildAndRunBatch()
        {
            PrototypeSceneBuilder.BuildScene();
            RunBatch();
        }

        public static void RunBatch()
        {
            PrototypeVisualValidation.ValidateVisualScene();
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
                "Assets/BattleRoyaleX/GeneratedScenes/Prototype01_Arena.unity");
            SessionState.SetBool(Batch, true);
            Run();
        }

        [MenuItem("Battle Royale X/Prototype 01/Run Live Test Matrix")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("Stop Play Mode before running the live matrix.");
                return;
            }
            SessionState.SetBool(Pending, true);
            EditorApplication.EnterPlaymode();
        }
    }
}
#endif
