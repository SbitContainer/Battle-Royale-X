#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace BattleRoyaleX.EditorTools
{
    public static class PrototypeMobileBuildSetup
    {
        const string ScenePath = "Assets/BattleRoyaleX/GeneratedScenes/Prototype01_Arena.unity";

        [MenuItem("Battle Royale X/Prototype 01/Configure Android Device Test")]
        public static void ConfigureAndroidDeviceTest()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null)
            {
                Debug.LogError("Build the Prototype 01 test scene before configuring Android.");
                return;
            }

            PlayerSettings.companyName = "SbitContainer";
            PlayerSettings.productName = "Battle Royale X";
            PlayerSettings.bundleVersion = "0.7.0-warrior-lab";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.sbitcontainer.battleroyalex.prototype");
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.bundleVersionCode = 7;
            // Support the common 32-bit and 64-bit Android devices used for local tests.
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("Battle Royale X: Android device-test configured. Switch platform to Android and build an APK.");
        }
    }
}
#endif
