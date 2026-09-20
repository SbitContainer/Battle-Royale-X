#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BattleRoyaleX.EditorTools
{
    [InitializeOnLoad]
    public static class PrototypeLiveTestLauncher
    {
        const string Pending = "BRX.LiveTests.Pending";

        static PrototypeLiveTestLauncher()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(Pending, false)) return;
                SessionState.SetBool(Pending, false);
                new GameObject("Live_Test_Runner_Editor_Only").AddComponent<PrototypeLiveTests>();
            };
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
