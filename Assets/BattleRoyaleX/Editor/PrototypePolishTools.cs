#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace BattleRoyaleX.EditorTools
{
    // Local editor work queue; inert unless an explicit one-shot request exists in Unity's ignored Temp folder.
    [InitializeOnLoad]
    public static class PrototypePolishTools
    {
        const string Request = "Temp/brx-polish-request.txt";
        static double nextCheck;
        static PrototypePolishTools() { EditorApplication.update += Tick; }
        static void Tick()
        {
            if (EditorApplication.timeSinceStartup < nextCheck || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            nextCheck = EditorApplication.timeSinceStartup + 1;
            if (!File.Exists(Request)) return;
            string request = File.ReadAllText(Request).Trim();
            if (request == SessionState.GetString("BRX.PolishRequest", "")) return;
            SessionState.SetString("BRX.PolishRequest", request);
            Directory.CreateDirectory("Logs");
            try
            {
                string action=request.Split(':')[0];
                if(action=="prepare") Prepare();
                else if(action=="data") PrototypeDataFactory.CreateDefaultData();
                else if(action=="scene") PrototypeSceneBuilder.BuildScene();
                else if(action=="refresh") AssetDatabase.Refresh();
                else if(action=="test") PrototypeLiveTestLauncher.Run();
                else if(action=="smoke") PrototypeVisualRuntimeSmoke.Run();
                else if(action=="validate") PrototypeVisualValidation.ValidateVisualScene();
                else if(action=="stop") EditorApplication.isPlaying=false;
                else if(action=="capture") Capture();
                else if(action=="preview") Preview();
                else if(action=="labpreview") PreviewClass(request.Split(':')[1]);
                else if(action=="inspect") InspectModels();
                else if(action=="build") Build();
                else throw new InvalidOperationException("Unknown polish action");
                File.WriteAllText("Logs/polish-command.txt",request+" OK "+DateTime.Now);
            }
            catch(Exception e) { File.WriteAllText("Logs/polish-command.txt",request+" FAILED "+e); Debug.LogException(e); }
        }

        [MenuItem("Battle Royale X/Prototype 01/Prepare Combat Polish")]
        public static void Prepare()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Stop Play Mode first.");
            PrototypeSceneBuilder.BuildScene();
            const string root="Assets/BattleRoyaleX/GeneratedVisuals/Materials/";
            var floor=AssetDatabase.LoadAssetAtPath<Material>(root+"Arena_Stone.mat");
            if(floor==null) { floor=new Material(Resources.Load<Shader>("ArenaStone")); AssetDatabase.CreateAsset(floor,root+"Arena_Stone.mat"); }
            GameObject.Find("Arena_Ground_45x45").GetComponent<Renderer>().sharedMaterial=floor;
            var stone=AssetDatabase.LoadAssetAtPath<Material>(root+"Arena_Pillars.mat");
            if(stone==null) { stone=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(stone,root+"Arena_Pillars.mat"); }
            stone.SetColor("_BaseColor",new Color(0.18f,0.23f,0.29f));stone.SetFloat("_Smoothness",0.32f);
            foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>()) if(r.name=="Arena_Obstacle")r.sharedMaterial=stone;
            foreach(var light in UnityEngine.Object.FindObjectsByType<Light>()) if(light.type==LightType.Directional)
            { light.shadows=LightShadows.Soft; light.intensity=1.6f; light.color=new Color(1f,0.91f,0.8f); }
            RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(0.48f,0.55f,0.68f);
            var cam=Camera.main; cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(0.035f,0.055f,0.085f);
            EditorUtility.SetDirty(stone);AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            var report=new System.Text.StringBuilder();
            foreach(var c in UnityEngine.Object.FindObjectsByType<CharacterRuntime>())
            {
                var animator=c.GetComponentInChildren<Animator>();
                report.AppendLine(c.name+" avatar="+(animator!=null && animator.avatar!=null && animator.avatar.isValid));
                foreach(var r in c.GetComponentsInChildren<SkinnedMeshRenderer>())report.AppendLine(r.name+" -> "+string.Join(",",r.sharedMaterials.Select(m=>m==null?"NULL":m.name)));
            }
            File.WriteAllText("Logs/polish-models.txt",report.ToString());
        }
        static void Capture()
        {
            if(!EditorApplication.isPlaying) { EditorApplication.EnterPlaymode(); return; }
            ScreenCapture.CaptureScreenshot(Path.GetFullPath("Logs/combat-polish.png"));
        }
        static void InspectModels()
        {
            var report=new System.Text.StringBuilder();
            foreach(string path in new[]{PrototypeVisualFactory.MaleModelPath,PrototypeVisualFactory.FemaleModelPath,
                "Assets/ThirdParty/Quaternius/UniversalBaseCharacters/Models/Superhero_Male_FullBody.fbx",
                "Assets/ThirdParty/Quaternius/UniversalBaseCharacters/Models/Superhero_Female_FullBody.fbx"})
            {
                report.AppendLine(path);
                foreach(var r in AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    report.AppendLine(r.name+" | "+r.sharedMesh.vertexCount+" | "+string.Join(",",r.sharedMaterials.Select(m=>m.name)));
            }
            File.WriteAllText("Logs/polish-source-meshes.txt",report.ToString());
        }
        static void PreviewClass(string className)
        {
            if (!EditorApplication.isPlaying) throw new InvalidOperationException("Enter Play Mode first.");
            if (!Enum.TryParse(className, true, out CharacterClass selected)) throw new ArgumentException("Unknown class");
            PrototypeLabController lab = UnityEngine.Object.FindAnyObjectByType<PrototypeLabController>();
            if (lab == null || !lab.SwitchPlayerClass(selected)) throw new InvalidOperationException("Class unavailable");
            foreach (var bot in UnityEngine.Object.FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            foreach (var input in UnityEngine.Object.FindObjectsByType<PrototypeLocalInput>()) input.enabled = false;
            foreach (var hud in UnityEngine.Object.FindObjectsByType<PrototypeDebugHUD>()) hud.enabled = false;
            lab.fixedOpponent.Initialize(lab.warriorDefinition);
            lab.fixedOpponent.Motor.Teleport(new Vector3(-3f, 1f, 0f));
            lab.playerSlot.Motor.Teleport(new Vector3(3f, 1f, 0f));
            lab.fixedOpponent.Motor.FaceDirection(Vector3.right);
            lab.playerSlot.Motor.FaceDirection(Vector3.left);
            double start = EditorApplication.timeSinceStartup;
            bool cast = false;
            void Frame()
            {
                if (!EditorApplication.isPlaying) { EditorApplication.update -= Frame; return; }
                double elapsed = EditorApplication.timeSinceStartup - start;
                if (!cast && elapsed > 1f)
                {
                    cast = true;
                    lab.playerSlot.Abilities.TryUse(AbilitySlot.Skill1, Vector3.left);
                }
                if (elapsed > 1.45f)
                {
                    ScreenCapture.CaptureScreenshot(Path.GetFullPath("Logs/astra-" + className.ToLowerInvariant() + ".png"));
                    EditorApplication.update -= Frame;
                }
            }
            EditorApplication.update += Frame;
        }
        static void Preview()
        {
            if(!EditorApplication.isPlaying)throw new InvalidOperationException("Enter Play Mode first.");
            foreach(var runner in UnityEngine.Object.FindObjectsByType<PrototypeLiveTests>())UnityEngine.Object.Destroy(runner.gameObject);
            foreach(var bot in UnityEngine.Object.FindObjectsByType<PrototypeTrainingBot>())bot.enabled=false;
            var characters=UnityEngine.Object.FindObjectsByType<CharacterRuntime>();
            foreach(var c in characters)
            {
                c.GetComponent<PrototypeLocalInput>().enabled=false;c.Initialize(c.Definition);c.Motor.StopMovementImmediately();
                float side=c.TeamId==TeamId.PlayerOne?-1f:1f;
                c.Motor.Teleport(new Vector3(side*1.1f,1f,0));c.Motor.FaceDirection(Vector3.left*side);
                c.Abilities.TryUse(AbilitySlot.Ultimate);
            }
            double start=EditorApplication.timeSinceStartup;bool attacked=false;
            void TickPreview()
            {
                if(!EditorApplication.isPlaying){EditorApplication.update-=TickPreview;return;}
                double elapsed=EditorApplication.timeSinceStartup-start;
                if(!attacked && elapsed>2){attacked=true;foreach(var c in characters)c.Abilities.TryUse(AbilitySlot.BasicAttack);}
                if(elapsed>2.3){Capture();EditorApplication.update-=TickPreview;}
            }
            EditorApplication.update+=TickPreview;
        }
        static void Build()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Stop Play Mode first.");
            PlayerSettings.bundleVersion="0.8.0-blood-death";
            PlayerSettings.Android.bundleVersionCode=8;
            // The prototype uses Input + StandaloneInputModule. Android does not support Both.
            var settings=new SerializedObject(Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings"));
            var input=settings.FindProperty("activeInputHandler");
            if(input==null)throw new InvalidOperationException("Active Input Handling setting not found");
            input.intValue=0;settings.ApplyModifiedPropertiesWithoutUndo();AssetDatabase.SaveAssets();
            var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes=new[]{"Assets/BattleRoyaleX/GeneratedScenes/Prototype01_Arena.unity"},
                locationPathName="Builds/BattleRoyaleX-blood-death.apk",target=BuildTarget.Android,options=BuildOptions.None });
            File.WriteAllText("Logs/polish-build.txt",result.summary.result+" errors="+result.summary.totalErrors+" bytes="+result.summary.totalSize);
            if(result.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new InvalidOperationException("Android build failed");
        }
    }
}
#endif
