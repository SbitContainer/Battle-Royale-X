#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleRoyaleX.EditorTools
{
    public static class PrototypeSceneBuilder
    {
        [MenuItem("Battle Royale X/Prototype 01/Build Test Scene")]
        public static void BuildScene()
        {
            PrototypeDataFactory.CreateDefaultData();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Arena_Ground_45x45";
            ground.transform.localScale = new Vector3(4.5f,1f,4.5f);

            CreateObstacle(new Vector3(9f,1f,9f), new Vector3(4f,2f,2f));
            CreateObstacle(new Vector3(-9f,1f,9f), new Vector3(2f,2f,4f));
            CreateObstacle(new Vector3(9f,1f,-9f), new Vector3(2f,2f,4f));
            CreateObstacle(new Vector3(-9f,1f,-9f), new Vector3(4f,2f,2f));
            CreateObstacle(new Vector3(18f,0.75f,0f), new Vector3(2f,1.5f,5f));
            CreateObstacle(new Vector3(-18f,0.75f,0f), new Vector3(2f,1.5f,5f));
            CreateObstacle(new Vector3(0f,0.75f,18f), new Vector3(5f,1.5f,2f));
            CreateObstacle(new Vector3(0f,0.75f,-18f), new Vector3(5f,1.5f,2f));

            CharacterDefinition warrior = Find<CharacterDefinition>("Warrior");
            CharacterDefinition assassin = Find<CharacterDefinition>("Assassin");
            CharacterRuntime p1 = CreateCharacter("P1_Warrior", warrior, TeamId.PlayerOne, new Vector3(-7f,1f,0f), false);
            CharacterRuntime p2 = CreateCharacter("P2_Assassin", assassin, TeamId.PlayerTwo, new Vector3(7f,1f,0f), true);
            p1.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
            p2.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);

            Camera camera = new GameObject("Main Camera").AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0f,14f,-11f);
            var rig = camera.gameObject.AddComponent<IsometricCameraRig>();
            rig.targetA = p1.transform; rig.targetB = p2.transform;

            Light light = new GameObject("Directional Light").AddComponent<Light>();
            light.type = LightType.Directional; light.intensity = 1.2f; light.transform.rotation = Quaternion.Euler(50f,-35f,0f);

            GameObject systems = new GameObject("Prototype_Systems");
            PrototypeDebugHUD hud = systems.AddComponent<PrototypeDebugHUD>(); hud.playerOne=p1; hud.playerTwo=p2;
            AirdropManager drop = systems.AddComponent<AirdropManager>();
            drop.itemPool = AssetDatabase.FindAssets("t:ItemDefinition", new[]{"Assets/BattleRoyaleX/GeneratedData/Items"}).Select(g=>AssetDatabase.LoadAssetAtPath<ItemDefinition>(AssetDatabase.GUIDToAssetPath(g))).Where(x=>x!=null).ToList();

            CreateGroundPickups();

            string sceneFolder="Assets/BattleRoyaleX/GeneratedScenes";
            if(!AssetDatabase.IsValidFolder(sceneFolder)) AssetDatabase.CreateFolder("Assets/BattleRoyaleX", "GeneratedScenes");
            EditorSceneManager.SaveScene(scene, sceneFolder + "/Prototype01_Arena.unity");
            Selection.activeObject = p1.gameObject;
            Debug.Log("Battle Royale X: test scene generated. Press Play. P1 WASD/F/G/H/R, P2 arrows/numpad.");
        }

        static CharacterRuntime CreateCharacter(string name, CharacterDefinition def, TeamId team, Vector3 position, bool playerTwo)
        {
            GameObject go=GameObject.CreatePrimitive(PrimitiveType.Capsule); go.name=name; go.transform.position=position;
            Object.DestroyImmediate(go.GetComponent<CapsuleCollider>());
            CharacterController cc=go.AddComponent<CharacterController>(); cc.height=2f; cc.radius=0.45f; cc.center=Vector3.up;
            GameObject hurtboxObject = new GameObject("Hurtbox");
            hurtboxObject.transform.SetParent(go.transform, false);
            hurtboxObject.transform.localPosition = Vector3.up;
            CapsuleCollider hurt = hurtboxObject.AddComponent<CapsuleCollider>();
            hurt.height = 2f; hurt.radius = 0.45f; hurt.center = Vector3.zero; hurt.isTrigger = true;
            hurtboxObject.AddComponent<Hurtbox>();
            CharacterRuntime runtime=go.AddComponent<CharacterRuntime>(); runtime.definition=def; runtime.teamId=team;
            PrototypeLocalInput input=go.AddComponent<PrototypeLocalInput>(); if(playerTwo) input.ConfigurePlayerTwo();
            return runtime;
        }

        static void CreateObstacle(Vector3 pos, Vector3 scale){GameObject o=GameObject.CreatePrimitive(PrimitiveType.Cube);o.name="Arena_Obstacle";o.transform.position=pos;o.transform.localScale=scale;}

        static void CreateGroundPickups()
        {
            string[] ids={"Heal","Energy","Cooldown","Smoke","Repulsion","Barrier","Var_Assassin_Counter","Var_Assassin_Return","Var_Warrior_Parry","Var_Warrior_Impact"};
            Vector3[] positions={new Vector3(-14,0.6f,-14),new Vector3(14,0.6f,-14),new Vector3(-14,0.6f,14),new Vector3(14,0.6f,14),new Vector3(0,0.6f,-16),new Vector3(0,0.6f,16),new Vector3(-16,0.6f,0),new Vector3(16,0.6f,0),new Vector3(-7,0.6f,8),new Vector3(7,0.6f,-8)};
            for(int i=0;i<ids.Length;i++)
            {
                ItemDefinition item=Find<ItemDefinition>(ids[i]); if(item==null) continue;
                GameObject go=GameObject.CreatePrimitive(PrimitiveType.Sphere);go.name="Pickup_"+item.displayName;go.transform.position=positions[i];go.transform.localScale=Vector3.one*0.65f;go.GetComponent<Collider>().isTrigger=true;WorldPickup wp=go.AddComponent<WorldPickup>();wp.item=item;
            }
        }

        static T Find<T>(string name) where T:Object
        {
            string guid=AssetDatabase.FindAssets($"{name} t:{typeof(T).Name}",new[]{"Assets/BattleRoyaleX/GeneratedData"}).FirstOrDefault();
            return string.IsNullOrEmpty(guid)?null:AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}
#endif
