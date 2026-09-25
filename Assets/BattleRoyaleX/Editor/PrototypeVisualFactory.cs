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
            CreateMageController();
            AssetDatabase.SaveAssets();
            Debug.Log("Battle Royale X: Quaternius visual asset import settings configured.");
        }

        public static GameObject CreateCharacterVisual(Transform parent, CharacterClass characterClass)
        {
            string modelPath = characterClass == CharacterClass.Assassin ? FemaleModelPath : MaleModelPath;
            string materialPath = characterClass == CharacterClass.Mage ? MaterialsRoot + "/Mage_Body.mat" :
                characterClass == CharacterClass.Archer ? MaterialsRoot + "/Archer_Body.mat" :
                characterClass == CharacterClass.Assassin ? AssassinMaterialPath : WarriorMaterialPath;
            GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ControllerPath);
            if (characterClass == CharacterClass.Mage)
                controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(GeneratedRoot + "/Mage.overrideController") ?? controller;
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
            if (characterClass == CharacterClass.Warrior && rightHand != null && leftHand != null)
            {
                AttachProp("Sword_Bronze", rightHand, 1.05f, false);
                AttachProp("Shield_Wooden", leftHand, 0.72f, true);
                return;
            }
            if (characterClass == CharacterClass.Assassin && rightHand != null && leftHand != null)
            {
                AttachProp("Sword_Bronze", rightHand, 0.52f, false).name = "Assassin_Dagger";
                AttachProp("Sword_Bronze", leftHand, 0.52f, false).name = "Assassin_OffhandDagger";
                return;
            }
            if (characterClass == CharacterClass.Mage && rightHand != null)
            {
                GameObject staff = Primitive("Mage_Staff", PrimitiveType.Cylinder, rightHand, bronze);
                staff.transform.localPosition = new Vector3(0f, 0.35f, 0f);
                staff.transform.localScale = new Vector3(0.045f, 0.7f, 0.045f);
                GameObject orb = Primitive("Mage_ArcaneOrb", PrimitiveType.Sphere, rightHand,
                    AssetDatabase.LoadAssetAtPath<Material>(MaterialsRoot + "/Mage_Orb.mat"));
                orb.transform.localPosition = new Vector3(0f, 1.1f, 0f);
                orb.transform.localScale = Vector3.one * 0.22f;
                return;
            }
            if (characterClass == CharacterClass.Archer && leftHand != null)
            {
                GameObject bow = new GameObject("Archer_Bow");
                bow.transform.SetParent(leftHand, false);
                LineRenderer arc = bow.AddComponent<LineRenderer>();
                arc.useWorldSpace = false; arc.sharedMaterial = bronze;
                arc.startWidth = arc.endWidth = 0.045f; arc.positionCount = 17;
                for (int i = 0; i < 17; i++)
                {
                    float angle = Mathf.Lerp(-Mathf.PI * 0.5f, Mathf.PI * 0.5f, i / 16f);
                    arc.SetPosition(i, new Vector3(0f, Mathf.Sin(angle) * 0.55f, Mathf.Cos(angle) * 0.23f));
                }
                GameObject cord = new GameObject("BowString"); cord.transform.SetParent(bow.transform, false);
                LineRenderer line = cord.AddComponent<LineRenderer>();
                line.useWorldSpace = false; line.sharedMaterial = metal;
                line.startWidth = line.endWidth = 0.009f; line.positionCount = 2;
                line.SetPositions(new[] { Vector3.down * 0.55f, Vector3.up * 0.55f });
                return;
            }
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
            if (characterClass == CharacterClass.Assassin && leftHand != null)
            {
                GameObject dagger = Primitive("Assassin_OffhandDagger", PrimitiveType.Cube, leftHand, metal);
                dagger.transform.localPosition = new Vector3(0f, 0.36f, 0f);
                dagger.transform.localScale = new Vector3(0.055f, 0.48f, 0.025f);
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

        static GameObject AttachProp(string name, Transform hand, float height, bool shield)
        {
            const string root = "Assets/ThirdParty/Quaternius/FantasyProps/";
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(root + name + ".fbx");
            if (asset == null) throw new InvalidOperationException("Missing licensed prop: " + name);
            GameObject socket = new GameObject(shield ? "Warrior_Shield" : "Warrior_Sword");
            socket.transform.SetParent(hand, false);
            GameObject prop = (GameObject)PrefabUtility.InstantiatePrefab(asset, socket.transform);
            prop.transform.localPosition = Vector3.zero;
            prop.transform.localRotation = Quaternion.identity;
            MeshFilter[] filters = prop.GetComponentsInChildren<MeshFilter>();
            Bounds bounds = new Bounds(); bool first = true;
            foreach (MeshFilter filter in filters)
            {
                if (filter.sharedMesh == null) continue;
                Bounds mesh = filter.sharedMesh.bounds;
                for (int i = 0; i < 8; i++)
                {
                    Vector3 p = mesh.center + Vector3.Scale(mesh.extents,
                        new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                    p = socket.transform.InverseTransformPoint(filter.transform.TransformPoint(p));
                    if (first) { bounds = new Bounds(p, Vector3.zero); first = false; } else bounds.Encapsulate(p);
                }
            }
            Vector3 dimensions = bounds.size;
            int longest = dimensions.x > dimensions.y ? 0 : 1;
            if (dimensions.z > dimensions[longest]) longest = 2;
            int shortest = dimensions.x < dimensions.y ? 0 : 1;
            if (dimensions.z < dimensions[shortest]) shortest = 2;
            Vector3 upAxis = longest == 0 ? Vector3.right : longest == 1 ? Vector3.up : Vector3.forward;
            Vector3 normalAxis = shortest == 0 ? Vector3.right : shortest == 1 ? Vector3.up : Vector3.forward;
            Quaternion alignment = shield && shortest != longest ?
                Quaternion.Inverse(Quaternion.LookRotation(normalAxis, upAxis)) : Quaternion.FromToRotation(upAxis, Vector3.up);
            float scale = height / Mathf.Max(0.01f, dimensions[longest]);
            prop.transform.localRotation = alignment;
            prop.transform.localScale *= scale;
            prop.transform.localPosition = -(alignment * bounds.center) * scale + Vector3.up * (shield ? 0.12f : height * 0.36f);
            socket.transform.localRotation = shield ? Quaternion.Euler(0f, 90f, 0f) : Quaternion.identity;
            string materialPath = MaterialsRoot + "/" + name + ".mat";
            CreateOrUpdateLitMaterial(materialPath, root + (shield ? "T_Trim_Props_BaseColor.png" : "T_Trim_Metal_BaseColor.png"),
                null, Color.white, shield ? 0.3f : 0.6f);
            foreach (Renderer renderer in prop.GetComponentsInChildren<Renderer>())
                renderer.sharedMaterials = Enumerable.Repeat(AssetDatabase.LoadAssetAtPath<Material>(materialPath),
                    renderer.sharedMaterials.Length).ToArray();
            foreach (Collider collider in prop.GetComponentsInChildren<Collider>()) UnityEngine.Object.DestroyImmediate(collider);
            return socket;
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
            string orbPath = MaterialsRoot + "/Mage_Orb.mat";
            CreateOrUpdateLitMaterial(orbPath, null, null, new Color(0.12f, 0.75f, 1f), 0.75f);
            Material orbMaterial = AssetDatabase.LoadAssetAtPath<Material>(orbPath);
            orbMaterial.EnableKeyword("_EMISSION");
            orbMaterial.SetColor("_EmissionColor", new Color(0.1f, 0.8f, 1.2f));
            EditorUtility.SetDirty(orbMaterial);
            CreateOrUpdateLitMaterial(MaterialsRoot + "/Mage_Body.mat", OutfitRoot + "T_Ranger_3_BaseColor.png",
                OutfitRoot + "T_Ranger_Normal.png", new Color(0.3f, 0.8f, 1f), 0.3f);
            CreateOrUpdateLitMaterial(MaterialsRoot + "/Archer_Body.mat", OutfitRoot + "T_Ranger_BaseColor.png",
                OutfitRoot + "T_Ranger_Normal.png", new Color(0.45f, 0.7f, 0.42f), 0.2f);
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

        static void CreateMageController()
        {
            const string path = GeneratedRoot + "/Mage.overrideController";
            AnimatorOverrideController controller = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(path);
            if (controller == null)
            {
                controller = new AnimatorOverrideController();
                AssetDatabase.CreateAsset(controller, path);
            }
            controller.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ControllerPath);
            AnimationClip cast = FindClip(LocomotionPath, "Armature|Spell_Simple_Shoot");
            foreach (string clip in new[] { "Sword_Regular_A", "Sword_Regular_B", "Sword_Regular_C", "Shield_OneShot" })
                controller["Armature|" + clip] = cast;
            EditorUtility.SetDirty(controller);
        }

        static void CreateAnimatorController()
        {
            AnimatorController existing = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (existing != null)
            {
                foreach (AnimatorControllerLayer layer in existing.layers)
                {
                    foreach (ChildAnimatorState child in layer.stateMachine.states)
                        child.state.iKOnFeet = true;
                    AnimatorState existingDeath = layer.stateMachine.states.FirstOrDefault(s => s.state.name == "Death").state;
                    AnimatorState locomotion = layer.stateMachine.states.FirstOrDefault(s => s.state.name == "Locomotion").state;
                    if (existingDeath != null && locomotion != null && !existingDeath.transitions.Any(t => t.destinationState == locomotion))
                        AddRespawnTransition(existingDeath, locomotion);
                }
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
            AddRespawnTransition(deathState, locomotionState);

            EditorUtility.SetDirty(controller);
        }

        static void AddRespawnTransition(AnimatorState death, AnimatorState locomotion)
        {
            AnimatorStateTransition transition = death.AddTransition(locomotion);
            transition.hasExitTime = false; transition.duration = 0.08f;
            transition.AddCondition(AnimatorConditionMode.IfNot, 0f, "Dead");
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
