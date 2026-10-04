#if UNITY_EDITOR
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BattleRoyaleX.EditorTools
{
    [InitializeOnLoad]
    public static class PrototypePresentationReview
    {
        static PrototypePresentationReview()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if(state==PlayModeStateChange.EnteredEditMode && SessionState.GetBool("BRX.ReviewTests",false))
                {
                    SessionState.SetBool("BRX.ReviewTests",false);
                    if(SessionState.GetBool("BRX.ReviewRelease",false))
                    {
                        SessionState.SetBool("BRX.ReviewRelease",false);
                        EditorApplication.delayCall += PrototypePolishTools.BuildArcherAndroid;
                    }
                    else EditorApplication.delayCall += PrototypeLiveTestLauncher.RunBatch;
                }
                if(state!=PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool("BRX.Review",false)) return;
                SessionState.SetBool("BRX.Review",false);
                new GameObject("PresentationReview").AddComponent<PresentationReviewRunner>();
            };
        }
        public static void Run()
        {
            PrototypePolishTools.Prepare();
            SessionState.SetBool("BRX.Review",true);
            EditorApplication.EnterPlaymode();
        }
        public static void RunRelease()
        {
            SessionState.SetBool("BRX.ReviewRelease",true);
            Run();
        }
    }
    public sealed class PresentationReviewRunner : MonoBehaviour
    {
        IEnumerator Start()
        {
            // Keep short-lived effects capturable even when this low-memory editor renders slowly.
            Time.captureDeltaTime = 1f / 30f;
            yield return null;
            PrototypeLabController lab=FindAnyObjectByType<PrototypeLabController>();
            Camera camera=Camera.main;
            foreach(var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled=false;
            foreach(var input in FindObjectsByType<PrototypeLocalInput>()) input.enabled=false;
            foreach(var rig in camera.GetComponents<IsometricCameraRig>()) rig.enabled=false;
            camera.transform.position=new Vector3(8f,12f,-10f);
            camera.transform.LookAt(new Vector3(0,0.6f,0)); camera.fieldOfView=38f;
            lab.SwitchPlayerClass(CharacterClass.Mage); Place(lab);
            lab.EquipVariation(AbilitySlot.Skill1,2);
            lab.playerSlot.Abilities.TryUse(AbilitySlot.Skill1);
            yield return new WaitForSeconds(1f); Capture(camera,"mage-swamp");
            lab.playerSlot.Abilities.ResetTransientState(); lab.ResetCooldowns(); Place(lab);
            lab.EquipVariation(AbilitySlot.Skill2,2);
            lab.playerSlot.Abilities.TryUse(AbilitySlot.Skill2);
            yield return new WaitForSeconds(0.48f); Capture(camera,"mage-fire");
            lab.playerSlot.Abilities.ResetTransientState(); lab.ResetCooldowns(); Place(lab);
            lab.EquipVariation(AbilitySlot.Skill1,1);
            lab.playerSlot.Abilities.TryUse(AbilitySlot.Skill1,Vector3.forward);
            yield return new WaitForSeconds(0.32f); Capture(camera,"mage-orb");
            lab.SwitchPlayerClass(CharacterClass.Archer); Place(lab);
            lab.EquipVariation(AbilitySlot.Ultimate,2);
            lab.playerSlot.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(0.94f); Capture(camera,"archer-rain");
            Debug.Log("[BRX PRESENTATION REVIEW] captures complete");
            Time.captureDeltaTime = 0f;
            SessionState.SetBool("BRX.ReviewTests",true);
            Destroy(gameObject);
            EditorApplication.ExitPlaymode();
        }
        static void Place(PrototypeLabController lab)
        {
            lab.playerSlot.Motor.Teleport(new Vector3(-1.7f,1,0));
            lab.fixedOpponent.Motor.Teleport(new Vector3(1.7f,1,0));
            lab.playerSlot.Motor.FaceDirection(Vector3.right);
            foreach(var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled=false;
            Physics.SyncTransforms();
        }
        static void Capture(Camera camera,string name)
        {
            RenderTexture previous=RenderTexture.active;
            RenderTexture target=new RenderTexture(1280,720,24);
            RenderTexture old=camera.targetTexture; camera.targetTexture=target;
            camera.Render(); RenderTexture.active=target;
            Texture2D image=new Texture2D(1280,720,TextureFormat.RGB24,false);
            image.ReadPixels(new Rect(0,0,1280,720),0,0); image.Apply();
            Directory.CreateDirectory("Logs/Presentation");
            File.WriteAllBytes("Logs/Presentation/"+name+".png",image.EncodeToPNG());
            camera.targetTexture=old; RenderTexture.active=previous;
            Destroy(image); target.Release(); Destroy(target);
        }
    }
}
#endif
