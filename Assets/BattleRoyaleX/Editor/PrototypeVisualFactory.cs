#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace BattleRoyaleX.EditorTools
{
    public static class PrototypeVisualFactory
    {
        public const string OutfitRoot = "Assets/ThirdParty/Quaternius/FantasyOutfits/";
        public const string MaleModelPath = OutfitRoot + "Male_Ranger.fbx";
        public const string FemaleModelPath = OutfitRoot + "Female_Ranger.fbx";
        public const string LocomotionPath = "Assets/ThirdParty/Quaternius/UniversalAnimationLibrary/UAL1_Standard.fbx";
        public const string CombatPath = "Assets/ThirdParty/Quaternius/UniversalAnimationLibrary2/UAL2_Standard.fbx";

        const string GeneratedRoot = "Assets/BattleRoyaleX/GeneratedVisuals";
        const string MaterialsRoot = GeneratedRoot + "/Materials";
        const string ControllerPath = GeneratedRoot + "/BRX_CombatV2.controller";
        const string WarriorMaterialPath = MaterialsRoot + "/Warrior_Body.mat";
        const string AssassinMaterialPath = MaterialsRoot + "/Assassin_Body.mat";
        const string EyeMaterialPath = MaterialsRoot + "/Character_Eyes.mat";
        const string EyebrowMaterialPath = MaterialsRoot + "/Character_Eyebrows.mat";
        const string WarriorSkinPath = MaterialsRoot + "/Warrior_Skin.mat";
        const string AssassinSkinPath = MaterialsRoot + "/Assassin_Skin.mat";
        const string WeaponMetalPath = MaterialsRoot + "/Weapon_Metal.mat";
        const string ShieldBronzePath = MaterialsRoot + "/Shield_Bronze.mat";

        static readonly string[] NormalTexturePaths =
        {
            "Assets/ThirdParty/Quaternius/UniversalBaseCharacters/Textures/T_Eye_Normal.png",
            "Assets/ThirdParty/Quaternius/UniversalBaseCharacters/Textures/T_Superhero_Male_Normal.png",
            "Assets/ThirdParty/Quaternius/UniversalBaseCharacters/Textures/T_Superhero_Female_Normal.png"
        };

        [MenuItem("Battle Royale X/Prototype 01/Configure Visual Assets")]
        public static void ConfigureVisualAssets()
        {
            ConfigureModelImporter(MaleModelPath, false);
            ConfigureModelImporter(FemaleModelPath, false);
            ConfigureModelImporter(LocomotionPath, true);
            ConfigureModelImporter(CombatPath, true);

            foreach (string path in NormalTexturePaths)
                ConfigureNormalTexture(path);
            foreach (string path in new[] { "T_Ranger_Normal.png", "T_Regular_Male_Normal.png", "T_Regular_Female_Normal.png" })
                ConfigureNormalTexture(OutfitRoot + path);

            EnsureFolder(GeneratedRoot);
            EnsureFolder(MaterialsRoot);
            CreateOrUpdateMaterials();
            CreateAnimatorController();
            AssetDatabase.SaveAssets();
            Debug.Log("Battle Royale X: Quaternius visual asset import settings configured.");
        }

        public static GameObject CreateCharacterVisual(Transform parent, CharacterClass characterClass)
        {
            string modelPath = characterClass == CharacterClass.Assassin ? FemaleModelPath : MaleModelPath;
            string materialPath = characterClass == CharacterClass.Assassin ? AssassinMaterialPath : WarriorMaterialPath;
            GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ControllerPath);
            Material bodyMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            Material eyeMaterial = AssetDatabase.LoadAssetAtPath<Material>(EyeMaterialPath);
            Material eyebrowMaterial = AssetDatabase.LoadAssetAtPath<Material>(EyebrowMaterialPath);
            Material skinMaterial = AssetDatabase.LoadAssetAtPath<Material>(characterClass == CharacterClass.Assassin ? AssassinSkinPath : WarriorSkinPath);
            if (modelAsset == null || controller == null || bodyMaterial == null)
                return null;

            GameObject instance = PrefabUtility.InstantiatePrefab(modelAsset, parent) as GameObject;
            if (instance == null) return null;

            instance.name = "VisualModel";
            instance.transform.localPosition = Vector3.down;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;

            foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true))
            {
                Material[] assigned = renderer.sharedMaterials;
                for (int i = 0; i < assigned.Length; i++)
                {
                    string name = assigned[i] != null ? assigned[i].name : renderer.name;
                    assigned[i] = name.IndexOf("Ranger", StringComparison.OrdinalIgnoreCase) >= 0 ? bodyMaterial : skinMaterial;
                    if (name.IndexOf("Eye", StringComparison.OrdinalIgnoreCase) >= 0) assigned[i] = eyeMaterial;
                    if (name.IndexOf("Eyebrow", StringComparison.OrdinalIgnoreCase) >= 0) assigned[i] = eyebrowMaterial;
                }
                renderer.sharedMaterials = assigned;
            }

            Animator animator = instance.GetComponent<Animator>();
            if (animator == null) animator = instance.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            AttachFace(instance, animator, characterClass);
            AttachEquipment(instance, animator, characterClass);

            if (instance.GetComponent<CharacterVisualAnimator>() == null)
                instance.AddComponent<CharacterVisualAnimator>();

            return instance;
        }

        // Outfit files contain clothing, not the underlying head. Reuse only the head geometry
        // of the already licensed base mesh; never expose its unclothed torso under the outfit.
        static void AttachFace(GameObject outfit, Animator outfitAnimator, CharacterClass characterClass)
        {
            bool female=characterClass==CharacterClass.Assassin;
            string sex=female?"Female":"Male";
            string baseRoot="Assets/ThirdParty/Quaternius/UniversalBaseCharacters/";
            var source=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(baseRoot+"Models/Superhero_"+sex+"_FullBody.fbx"));
            try
            {
                var sourceAnimator=source.GetComponent<Animator>();
                Transform sourceHead=sourceAnimator.GetBoneTransform(HumanBodyBones.Head);
                Transform targetHead=outfitAnimator.GetBoneTransform(HumanBodyBones.Head);
                string materialPath=MaterialsRoot+"/"+sex+"_Face.mat";
                var skin=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if(skin==null){skin=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(skin,materialPath);}
                skin.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(baseRoot+"Textures/"+(female?"T_Superhero_Female_Dark_BaseColor.png":"T_Superhero_Male_Dark.png")));
                skin.SetColor("_BaseColor",Color.white);skin.SetFloat("_Smoothness",0.25f);EditorUtility.SetDirty(skin);
                foreach(var renderer in source.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    var mesh=new Mesh();renderer.BakeMesh(mesh);var vertices=mesh.vertices;
                    bool facePart=renderer.name.IndexOf("Eye",StringComparison.OrdinalIgnoreCase)>=0;
                    var weights=renderer.sharedMesh.boneWeights;var bones=renderer.bones;
                    bool HeadWeight(int i)
                    {
                        if(facePart)return true;
                        var b=weights[i];float sum=0;
                        int[] ids={b.boneIndex0,b.boneIndex1,b.boneIndex2,b.boneIndex3};float[] values={b.weight0,b.weight1,b.weight2,b.weight3};
                        for(int j=0;j<4;j++)if(bones[ids[j]].name.IndexOf("Head",StringComparison.OrdinalIgnoreCase)>=0||bones[ids[j]].name.IndexOf("Neck",StringComparison.OrdinalIgnoreCase)>=0)sum+=values[j];
                        return sum>0.5f;
                    }
                    var triangles=mesh.triangles;var kept=new System.Collections.Generic.List<int>();
                    for(int i=0;i<triangles.Length;i+=3)if(HeadWeight(triangles[i])&&HeadWeight(triangles[i+1])&&HeadWeight(triangles[i+2]))
                    {kept.Add(triangles[i]);kept.Add(triangles[i+1]);kept.Add(triangles[i+2]);}
                    for(int i=0;i<vertices.Length;i++)vertices[i]=sourceHead.InverseTransformPoint(renderer.transform.TransformPoint(vertices[i]));
                    mesh.vertices=vertices;mesh.triangles=kept.ToArray();mesh.RecalculateNormals();mesh.RecalculateBounds();
                    string path=GeneratedRoot+"/"+sex+"_Face_"+renderer.name+".asset";
                    var saved=AssetDatabase.LoadAssetAtPath<Mesh>(path);
                    if(saved==null){AssetDatabase.CreateAsset(mesh,path);saved=mesh;}else{EditorUtility.CopySerialized(mesh,saved);UnityEngine.Object.DestroyImmediate(mesh);EditorUtility.SetDirty(saved);}
                    var part=new GameObject("Face_"+renderer.name,typeof(MeshFilter),typeof(MeshRenderer));part.transform.SetParent(targetHead,false);
                    part.GetComponent<MeshFilter>().sharedMesh=saved;
                    part.GetComponent<MeshRenderer>().sharedMaterial=renderer.name.IndexOf("Eyebrow",StringComparison.OrdinalIgnoreCase)>=0?AssetDatabase.LoadAssetAtPath<Material>(EyebrowMaterialPath):facePart?AssetDatabase.LoadAssetAtPath<Material>(EyeMaterialPath):skin;
                }
            }
            finally{UnityEngine.Object.DestroyImmediate(source);}
        }

        static void AttachEquipment(GameObject outfit, Animator animator, CharacterClass characterClass)
        {
            Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            Transform leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            Material metal = AssetDatabase.LoadAssetAtPath<Material>(WeaponMetalPath);
            Material bronze = AssetDatabase.LoadAssetAtPath<Material>(ShieldBronzePath);
            if (rightHand != null)
            {
                float bladeLength = characterClass == CharacterClass.Assassin ? 0.48f : 0.78f;
                GameObject grip = Primitive("Weapon_Grip", PrimitiveType.Cylinder, rightHand, metal);
                grip.transform.localPosition = new Vector3(0f, 0.10f, 0f);
                grip.transform.localRotation = Quaternion.identity;
                grip.transform.localScale = new Vector3(0.035f, 0.10f, 0.035f);
                GameObject blade = Primitive(characterClass == CharacterClass.Assassin ? "Assassin_Dagger" : "Warrior_Sword", PrimitiveType.Cube, rightHand, metal);
                blade.transform.localPosition = new Vector3(0f, 0.18f + bladeLength * 0.5f, 0f);
                blade.transform.localRotation = Quaternion.identity;
                blade.transform.localScale = new Vector3(characterClass == CharacterClass.Assassin ? 0.055f : 0.075f, bladeLength, 0.025f);
            }
            if (characterClass == CharacterClass.Warrior && leftHand != null)
            {
                GameObject shield = Primitive("Warrior_Shield", PrimitiveType.Cylinder, leftHand, bronze);
                shield.transform.localPosition = new Vector3(0f, 0.18f, 0.05f);
                shield.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                shield.transform.localScale = new Vector3(0.42f, 0.055f, 0.52f);
            }
        }

        static GameObject Primitive(string name, PrimitiveType type, Transform parent, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            Collider collider = go.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
            Renderer renderer = go.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            return go;
        }

        static void ConfigureModelImporter(string path, bool importAnimation)
        {
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null)
                throw new InvalidOperationException($"Battle Royale X: model asset not found at {path}");

            bool dirty = false;
            if (importer.importAnimation != importAnimation)
            {
                importer.importAnimation = importAnimation;
                dirty = true;
            }
            if (importer.importCameras)
            {
                importer.importCameras = false;
                dirty = true;
            }
            if (importer.importLights)
            {
                importer.importLights = false;
                dirty = true;
            }

            if (importer.animationType != ModelImporterAnimationType.Human)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                dirty = true;
            }

            if (importer.avatarSetup != ModelImporterAvatarSetup.CreateFromThisModel)
            {
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                dirty = true;
            }

            var materialMode = importAnimation ? ModelImporterMaterialImportMode.None : ModelImporterMaterialImportMode.ImportStandard;
            if (importer.materialImportMode != materialMode)
            {
                importer.materialImportMode = materialMode;
                dirty = true;
            }

            if (importAnimation)
            {
                ModelImporterClipAnimation[] clips = importer.clipAnimations;
                if (clips == null || clips.Length == 0) clips = importer.defaultClipAnimations;
                bool clipsChanged = false;
                foreach (ModelImporterClipAnimation clip in clips)
                {
                    bool shouldLoop = clip.name.IndexOf("_Loop", StringComparison.OrdinalIgnoreCase) >= 0;
                    if (clip.loopTime != shouldLoop)
                    {
                        clip.loopTime = shouldLoop;
                        clipsChanged = true;
                    }
                    if (clip.loopPose != shouldLoop)
                    {
                        clip.loopPose = shouldLoop;
                        clipsChanged = true;
                    }
                    // Gameplay owns displacement; keep the animated skeleton planted at the feet.
                    // Bake every authored root delta into the pose, otherwise combat clips can
                    // hover during their transition back to locomotion.
                    if (!clip.lockRootHeightY) { clip.lockRootHeightY = true; clipsChanged = true; }
                    if (!clip.lockRootPositionXZ) { clip.lockRootPositionXZ = true; clipsChanged = true; }
                    if (!clip.lockRootRotation) { clip.lockRootRotation = true; clipsChanged = true; }
                    if (clip.keepOriginalPositionY) { clip.keepOriginalPositionY = false; clipsChanged = true; }
                    if (clip.keepOriginalPositionXZ) { clip.keepOriginalPositionXZ = false; clipsChanged = true; }
                    if (!clip.heightFromFeet) { clip.heightFromFeet = true; clipsChanged = true; }
                }
                if (clipsChanged)
                {
                    importer.clipAnimations = clips;
                    dirty = true;
                }
            }

            if (dirty)
                importer.SaveAndReimport();
        }

        static void ConfigureNormalTexture(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                throw new InvalidOperationException($"Battle Royale X: normal texture not found at {path}");

            if (importer.textureType == TextureImporterType.NormalMap)
                return;

            importer.textureType = TextureImporterType.NormalMap;
            importer.SaveAndReimport();
        }

        static void CreateOrUpdateMaterials()
        {
            CreateOrUpdateLitMaterial(
                WarriorMaterialPath,
                OutfitRoot + "T_Ranger_BaseColor.png",
                OutfitRoot + "T_Ranger_Normal.png",
                new Color(0.95f, 0.72f, 0.5f),
                0.35f);
            CreateOrUpdateLitMaterial(
                AssassinMaterialPath,
                OutfitRoot + "T_Ranger_3_BaseColor.png",
                OutfitRoot + "T_Ranger_Normal.png",
                new Color(0.63f, 0.64f, 0.95f),
                0.28f);
            CreateOrUpdateLitMaterial(
                EyeMaterialPath,
                "Assets/ThirdParty/Quaternius/UniversalBaseCharacters/Textures/T_Eye_Brown.png",
                "Assets/ThirdParty/Quaternius/UniversalBaseCharacters/Textures/T_Eye_Normal.png",
                Color.white,
                0.55f);
            CreateOrUpdateLitMaterial(EyebrowMaterialPath, null, null, new Color(0.035f, 0.025f, 0.02f), 0.2f);
            CreateOrUpdateLitMaterial(WarriorSkinPath, OutfitRoot + "T_Regular_Male_Dark_BaseColor.png", OutfitRoot + "T_Regular_Male_Normal.png", Color.white, 0.25f);
            CreateOrUpdateLitMaterial(AssassinSkinPath, OutfitRoot + "T_Regular_Female_Dark_BaseColor.png", OutfitRoot + "T_Regular_Female_Normal.png", Color.white, 0.25f);
            CreateOrUpdateLitMaterial(WeaponMetalPath, null, null, new Color(0.32f, 0.42f, 0.55f), 0.72f);
            CreateOrUpdateLitMaterial(ShieldBronzePath, null, null, new Color(0.48f, 0.24f, 0.08f), 0.48f);
        }

        static void CreateOrUpdateLitMaterial(string path, string baseTexturePath, string normalTexturePath, Color color, float smoothness)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("Battle Royale X: URP Lit shader is unavailable.");

            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }
            else if (material.shader != shader)
            {
                material.shader = shader;
            }

            Texture2D baseTexture = string.IsNullOrEmpty(baseTexturePath) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(baseTexturePath);
            Texture2D normalTexture = string.IsNullOrEmpty(normalTexturePath) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(normalTexturePath);
            material.SetColor("_BaseColor", color);
            material.SetTexture("_BaseMap", baseTexture);
            material.SetFloat("_Smoothness", smoothness);
            material.SetTexture("_BumpMap", normalTexture);
            if (normalTexture != null) material.EnableKeyword("_NORMALMAP");
            else material.DisableKeyword("_NORMALMAP");
            EditorUtility.SetDirty(material);
        }

        static void CreateAnimatorController()
        {
            AnimatorController existing = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (existing != null)
            {
                foreach (AnimatorControllerLayer layer in existing.layers)
                    foreach (ChildAnimatorState child in layer.stateMachine.states)
                        child.state.iKOnFeet = true;
                EditorUtility.SetDirty(existing);
                return;
            }

            AnimationClip idle = FindClip(LocomotionPath, "Armature|Idle_Loop");
            AnimationClip jog = FindClip(LocomotionPath, "Armature|Jog_Fwd_Loop");
            AnimationClip attackA = FindClip(CombatPath, "Armature|Sword_Regular_A");
            AnimationClip attackB = FindClip(CombatPath, "Armature|Sword_Regular_B");
            AnimationClip attackC = FindClip(CombatPath, "Armature|Sword_Regular_C");
            AnimationClip defense = FindClip(CombatPath, "Armature|Sword_Block");
            AnimationClip moveAssassin = FindClip(CombatPath, "Armature|Sword_Dash");
            AnimationClip moveWarrior = FindClip(CombatPath, "Armature|Shield_Dash");
            AnimationClip ultimateAssassin = FindClip(CombatPath, "Armature|Sword_Regular_Combo");
            AnimationClip ultimateWarrior = FindClip(CombatPath, "Armature|Shield_OneShot");
            AnimationClip hit = FindClip(LocomotionPath, "Armature|Hit_Chest");
            AnimationClip death = FindClip(LocomotionPath, "Armature|Death01");

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            controller.AddParameter("MoveAmount", AnimatorControllerParameterType.Float);
            controller.AddParameter("Attack1", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Attack2", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Attack3", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Defense", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("MoveAssassin", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("MoveWarrior", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("UltimateDash", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("UltimateAssassin", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("UltimateWarrior", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Dead", AnimatorControllerParameterType.Bool);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            BlendTree locomotionTree = new BlendTree
            {
                name = "Locomotion",
                blendType = BlendTreeType.Simple1D,
                blendParameter = "MoveAmount",
                useAutomaticThresholds = false
            };
            AssetDatabase.AddObjectToAsset(locomotionTree, controller);
            locomotionTree.AddChild(idle, 0f);
            locomotionTree.AddChild(jog, 1f);

            AnimatorState locomotionState = stateMachine.AddState("Locomotion");
            locomotionState.motion = locomotionTree;
            locomotionState.iKOnFeet = true;
            stateMachine.defaultState = locomotionState;

            AddPresentationState(stateMachine, locomotionState, "Attack1", attackA, "Attack1");
            AddPresentationState(stateMachine, locomotionState, "Attack2", attackB, "Attack2");
            AddPresentationState(stateMachine, locomotionState, "Attack3", attackC, "Attack3");
            AddPresentationState(stateMachine, locomotionState, "Defense", defense, "Defense");
            AddPresentationState(stateMachine, locomotionState, "MoveAssassin", moveAssassin, "MoveAssassin");
            AddPresentationState(stateMachine, locomotionState, "MoveWarrior", moveWarrior, "MoveWarrior");
            AddPresentationState(stateMachine, locomotionState, "UltimateDash", moveAssassin, "UltimateDash");
            AddPresentationState(stateMachine, locomotionState, "UltimateAssassin", ultimateAssassin, "UltimateAssassin");
            AddPresentationState(stateMachine, locomotionState, "UltimateWarrior", ultimateWarrior, "UltimateWarrior");
            AddPresentationState(stateMachine, locomotionState, "Hit", hit, "Hit");

            AnimatorState deathState = stateMachine.AddState("Death");
            deathState.motion = death;
            deathState.iKOnFeet = true;
            AnimatorStateTransition deathTransition = stateMachine.AddAnyStateTransition(deathState);
            deathTransition.hasExitTime = false;
            deathTransition.duration = 0.05f;
            deathTransition.canTransitionToSelf = false;
            deathTransition.AddCondition(AnimatorConditionMode.If, 0f, "Dead");

            EditorUtility.SetDirty(controller);
        }

        static void AddPresentationState(AnimatorStateMachine stateMachine, AnimatorState locomotionState,
            string stateName, Motion motion, string trigger)
        {
            AnimatorState state = stateMachine.AddState(stateName);
            state.motion = motion;
            state.iKOnFeet = true;

            AnimatorStateTransition enter = stateMachine.AddAnyStateTransition(state);
            enter.hasExitTime = false;
            enter.duration = 0.04f;
            enter.canTransitionToSelf = false;
            enter.AddCondition(AnimatorConditionMode.If, 0f, trigger);

            AnimatorStateTransition exit = state.AddTransition(locomotionState);
            exit.hasExitTime = true;
            exit.exitTime = 0.92f;
            exit.duration = 0.08f;
        }

        static AnimationClip FindClip(string path, string clipName)
        {
            AnimationClip clip = AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<AnimationClip>()
                .FirstOrDefault(candidate => candidate.name == clipName);
            if (clip == null) throw new InvalidOperationException($"Battle Royale X: animation clip '{clipName}' not found in {path}");
            return clip;
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = path.Substring(0, path.LastIndexOf('/'));
            string name = path.Substring(path.LastIndexOf('/') + 1);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

    }
}
#endif
