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
            PrototypeVisualFactory.ConfigureVisualAssets();
            PrototypeDataFactory.CreateDefaultData();
            PrototypeVFXFactory.CreatePrototypeVFX();
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
            CharacterDefinition mage = Find<CharacterDefinition>("Mage");
            CharacterDefinition archer = Find<CharacterDefinition>("Archer");
            CharacterRuntime p1 = CreateCharacter("Opponent_Warrior_Slot", warrior, TeamId.PlayerOne, new Vector3(-7f,1f,0f), false);
            CharacterRuntime p2 = CreateCharacter("Player_Lab_Slot", assassin, TeamId.PlayerTwo, new Vector3(7f,1f,0f), true, true);
            p1.gameObject.AddComponent<PrototypeTrainingBot>();
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
            PrototypeCombatHUD hud = systems.AddComponent<PrototypeCombatHUD>(); hud.playerOne=p1; hud.playerTwo=p2;
            PrototypeCombatLabController lab = systems.AddComponent<PrototypeCombatLabController>();
            lab.fixedOpponent=p1; lab.playerSlot=p2; lab.warriorDefinition=warrior; lab.assassinDefinition=assassin;
            lab.mageDefinition=mage; lab.archerDefinition=archer;
            lab.visualProfiles=AssetDatabase.LoadAssetAtPath<VisualProfileRegistry>("Assets/BattleRoyaleX/Visual/Profiles/VisualProfileRegistry.asset");
            PrototypeDebugHUD debugHud = systems.AddComponent<PrototypeDebugHUD>(); debugHud.playerOne=p1; debugHud.playerTwo=p2; debugHud.lab=lab;
            systems.AddComponent<PrototypeMobileTouchControls>();
            systems.AddComponent<CombatEventVfxPresenter>();
            CombatVFXRouter vfxRouter=systems.AddComponent<CombatVFXRouter>();
            vfxRouter.library=AssetDatabase.LoadAssetAtPath<CombatVFXLibrary>("Assets/BattleRoyaleX/Visual/Profiles/CombatVFXLibrary.asset");
            vfxRouter.profiles=lab.visualProfiles;
            AirdropManager drop = systems.AddComponent<AirdropManager>();
            drop.firstDropDelay = 12f;
            drop.itemPool = AssetDatabase.FindAssets("t:ItemDefinition", new[]{"Assets/BattleRoyaleX/GeneratedData/Items"}).Select(g=>AssetDatabase.LoadAssetAtPath<ItemDefinition>(AssetDatabase.GUIDToAssetPath(g))).Where(x=>x!=null).ToList();

            CreateGroundPickups();
            CreateArcaneDemoObjects();

            string sceneFolder="Assets/BattleRoyaleX/GeneratedScenes";
            if(!AssetDatabase.IsValidFolder(sceneFolder)) AssetDatabase.CreateFolder("Assets/BattleRoyaleX", "GeneratedScenes");
            EditorSceneManager.SaveScene(scene, sceneFolder + "/Prototype01_Arena.unity");
            Selection.activeObject = p1.gameObject;
            Debug.Log("Battle Royale X: test scene generated. Press Play. P1 WASD/F/G/H/R, P2 arrows/numpad.");
        }

        static CharacterRuntime CreateCharacter(string name, CharacterDefinition def, TeamId team, Vector3 position, bool playerTwo, bool createLabVisuals=false)
        {
            GameObject go=new GameObject(name); go.transform.position=position;
            CharacterController cc=go.AddComponent<CharacterController>(); cc.height=2f; cc.radius=0.45f; cc.center=Vector3.up;
            GameObject hurtboxObject = new GameObject("Hurtbox");
            hurtboxObject.transform.SetParent(go.transform, false);
            hurtboxObject.transform.localPosition = Vector3.up;
            CapsuleCollider hurt = hurtboxObject.AddComponent<CapsuleCollider>();
            hurt.height = 2f; hurt.radius = 0.45f; hurt.center = Vector3.zero; hurt.isTrigger = true;
            hurtboxObject.AddComponent<Hurtbox>();
            CharacterRuntime runtime=go.AddComponent<CharacterRuntime>(); runtime.definition=def; runtime.teamId=team;
            GameObject primaryVisual = PrototypeVisualFactory.CreateCharacterVisual(go.transform, def.characterClass);
            if (primaryVisual != null && createLabVisuals)
            {
                foreach (CharacterClass other in new[] { CharacterClass.Warrior, CharacterClass.Assassin,
                    CharacterClass.Mage, CharacterClass.Archer })
                {
                    if (other == def.characterClass) continue;
                    GameObject alternate = PrototypeVisualFactory.CreateCharacterVisual(go.transform, other);
                    if (alternate != null) { alternate.name = "LabVisual_" + other; alternate.SetActive(false); }
                }
            }
            if (primaryVisual == null)
            {
                GameObject fallback=GameObject.CreatePrimitive(PrimitiveType.Capsule);
                fallback.name="FallbackCapsuleVisual";
                fallback.transform.SetParent(go.transform, false);
                Object.DestroyImmediate(fallback.GetComponent<Collider>());
            }
            PrototypeLocalInput input=go.AddComponent<PrototypeLocalInput>();
            if(playerTwo) input.ConfigurePlayerTwo(); else input.enabled=false;
            go.AddComponent<WorldHealthBar>();
            return runtime;
        }

        static void CreateObstacle(Vector3 pos, Vector3 scale){GameObject o=GameObject.CreatePrimitive(PrimitiveType.Cube);o.name="Arena_Obstacle";o.transform.position=pos;o.transform.localScale=scale;}

        static void CreateGroundPickups()
        {
            // Two readable class lanes plus neutral consumables. Every authored variation is available in the arena.
            string[] ids={
                "Var_Assassin_Counter","Var_Assassin_Double","Var_Assassin_Travel","Var_Assassin_Return","Var_Assassin_Exec","Var_Assassin_Hunt",
                "Var_Warrior_Parry","Var_Warrior_Fortress","Var_Warrior_Impact","Var_Warrior_Advance","Var_Warrior_Ret","Var_Warrior_Push",
                "Heal","Heal","Smoke","Smoke","Energy","Cooldown","Backpack4","Repulsion","Barrier","Null"
            };
            Vector3[] positions={
                new Vector3(-15,0.25f,-11),new Vector3(-11,0.25f,-11),new Vector3(-7,0.25f,-11),
                new Vector3(-15,0.25f,-7),new Vector3(-11,0.25f,-7),new Vector3(-7,0.25f,-7),
                new Vector3(7,0.25f,7),new Vector3(11,0.25f,7),new Vector3(15,0.25f,7),
                new Vector3(7,0.25f,11),new Vector3(11,0.25f,11),new Vector3(15,0.25f,11),
                new Vector3(-4,0.25f,14),new Vector3(4,0.25f,-14),new Vector3(0,0.25f,14),new Vector3(0,0.25f,-14),
                new Vector3(-4,0.25f,-14),new Vector3(4,0.25f,14),new Vector3(-15,0.25f,3),
                new Vector3(15,0.25f,-3),new Vector3(15,0.25f,3),new Vector3(-15,0.25f,-3)
            };
            for(int i=0;i<ids.Length;i++)
            {
                ItemDefinition item=AssetDatabase.LoadAssetAtPath<ItemDefinition>("Assets/BattleRoyaleX/GeneratedData/Items/"+ids[i]+".asset");
                if(item==null) continue;
                GameObject go=new GameObject("Pickup_"+item.itemId+"_"+i);go.transform.position=positions[i];
                SphereCollider trigger=go.AddComponent<SphereCollider>();trigger.isTrigger=true;trigger.radius=1.25f;trigger.center=Vector3.up*0.55f;
                WorldPickup wp=go.AddComponent<WorldPickup>();wp.item=item;
            }
        }

        static void CreateArcaneDemoObjects()
        {
            ArcaneDemoObject.Create("Arcane_PhaseWall", ArcaneDemoKind.PhaseWall,
                new Vector3(-12f, 1.5f, 0f), new Vector3(0.45f, 3f, 6f));
            ArcaneDemoObject.Create("Arcane_PrismaticWall", ArcaneDemoKind.PrismaticWall,
                new Vector3(12f, 1.5f, 0f), new Vector3(0.45f, 3f, 6f));
            ArcaneDemoObject.Create("Arcane_Amplifier", ArcaneDemoKind.AmplificationBarrier,
                new Vector3(0f, 1.5f, 10f), new Vector3(6f, 3f, 0.35f));
            ArcaneDemoObject.Create("Arcane_FragmentCrystal", ArcaneDemoKind.FragmentCrystal,
                new Vector3(0f, 1f, -10f), Vector3.one * 1.6f);
            ArcaneDemoObject.Create("Arcane_SpeedRune", ArcaneDemoKind.SpeedRune,
                new Vector3(-7f, 0.08f, 10f), new Vector3(3f, 0.12f, 3f));
            ArcaneDemoObject.Create("Arcane_ReactiveBush", ArcaneDemoKind.ReactiveBush,
                new Vector3(7f, 1f, -10f), new Vector3(3f, 2f, 3f));
        }

        static T Find<T>(string name) where T:Object
        {
            string guid=AssetDatabase.FindAssets($"{name} t:{typeof(T).Name}",new[]{"Assets/BattleRoyaleX/GeneratedData"}).FirstOrDefault();
            return string.IsNullOrEmpty(guid)?null:AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}
#endif
