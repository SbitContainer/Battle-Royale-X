#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace BattleRoyaleX.EditorTools
{
    // Runs as part of the existing generator, so rebuilding the arena keeps the authored effects.
    public static class PrototypeSkillVfxPolish
    {
        const string Textures = "Assets/ThirdParty/Kenney/ParticlePack/";
        const string Materials = "Assets/BattleRoyaleX/Visual/VFX/Shared/Materials/";

        public static void Decorate(GameObject root, string path)
        {
            bool mage = path.Contains("/Mage/");
            bool archer = path.Contains("/Archer/");
            if (!mage && !archer) return;
            Color cyan = new Color(0.12f,0.72f,1f,0.85f);
            Color gold = new Color(1f,0.69f,0.18f,0.9f);
            Color tint = mage ? cyan : gold;
            bool field = path.Contains("_Field");
            bool shot = path.Contains("_Projectile");
            if (path.Contains("Mage_S2_C")) tint = new Color(1f,0.25f,0.04f,0.9f);
            if (path.Contains("Archer_S1_B")) tint = new Color(0.62f,0.9f,1f,1f);
            if (!field && !shot)
            {
                ParticleSystem cast = root.GetComponent<ParticleSystem>();
                if (cast != null) cast.GetComponent<ParticleSystemRenderer>().sharedMaterial =
                    Mat(root.name+"_Cast", mage ? "magic_01.png" : "spark_06.png", tint);
                return;
            }
            if (shot)
            {
                foreach (Transform child in root.transform) if (child.name.EndsWith("Core")) Object.DestroyImmediate(child.gameObject);
                TrailRenderer trail = root.GetComponent<TrailRenderer>();
                if (trail != null)
                {
                    trail.sharedMaterial = Mat(root.name+"_Trail","light_02.png",tint);
                    trail.time = mage ? 0.32f : 0.2f;
                    trail.startWidth = mage ? 0.27f : 0.095f;
                    trail.startColor = Color.white; trail.endColor = new Color(1,1,1,0);
                }
                if (archer) CreateArrow(root.transform, tint, path.Contains("Archer_S1_B"));
                else
                {
                    float size = path.Contains("Mage_S1_B") ? 1.65f : 0.48f;
                    var core = Primitive(root.transform,"ArcanePlasma",PrimitiveType.Sphere,
                        Vector3.zero,Vector3.one*size, Mat(root.name+"_Plasma",null,tint,1));
                    core.AddComponent<SkillVfxMotion>().spin = new Vector3(30,80,45);
                    for(int ring=0;ring<2;ring++)
                    {
                        GameObject orbit = new GameObject("Orbit"+ring); orbit.transform.SetParent(root.transform,false);
                        orbit.transform.localRotation=Quaternion.Euler(ring==0?50:110,0,25);
                        Ring(orbit.transform,"EnergyRibbon",size*0.65f,0.018f,
                            Mat(root.name+"_Orbit",null,new Color(0.35f,0.9f,1f,0.85f)));
                        orbit.AddComponent<SkillVfxMotion>().spin=new Vector3(25,ring==0?120:-100,40);
                    }
                    Particles(root.transform,"PlasmaWisps","magic_01.png",tint,size*0.6f,0.28f,0.55f,16,true);
                }
                return;
            }
            for(int childIndex=root.transform.childCount-1;childIndex>=0;childIndex--)
                if(root.transform.GetChild(childIndex).name.StartsWith("Rune_")) Object.DestroyImmediate(root.transform.GetChild(childIndex).gameObject);
            if(path.Contains("Mage_S1_C"))
            {
                Transform surface=root.transform.Find("TranslucentSwamp");
                if(surface!=null)
                {
                    surface.localScale=Vector3.one*2f;
                    surface.GetComponent<Renderer>().sharedMaterial=Mat("Swamp_AnimatedSurface",null,Color.white,2,false);
                }
                foreach(LineRenderer line in root.GetComponentsInChildren<LineRenderer>())
                { line.startColor=line.endColor=new Color(0.2f,0.6f,0.3f,0.42f); line.startWidth=line.endWidth=0.023f; }
                Particles(root.transform,"SwampMist","smoke_04.png",new Color(0.3f,0.56f,0.3f,0.24f),0.6f,0.28f,1.5f,8,true,false);
                Particles(root.transform,"SurfaceBubbles","circle_03.png",new Color(0.36f,0.92f,0.48f,0.65f),0.8f,0.065f,0.85f,14,true);
            }
            else if(path.Contains("Mage_S2_C"))
            {
                Material fire=Mat("RepulsionFlameCurtain","flame_04.png",new Color(1f,0.38f,0.055f,1f));
                fire.SetFloat("_Intensity",4f);
                for(int i=0;i<20;i++)
                {
                    float angle=i*Mathf.PI*2f/20f;
                    var flame=Primitive(root.transform,"FlameTongue_"+i,PrimitiveType.Quad,
                        new Vector3(Mathf.Cos(angle)*0.85f,0.23f,Mathf.Sin(angle)*0.85f),
                        new Vector3(0.38f,0.58f,1f),fire);
                    flame.transform.localRotation=Quaternion.Euler(0f,-i*18f,0f);
                    var flicker=flame.AddComponent<SkillVfxMotion>();
                    flicker.spin=Vector3.zero; flicker.pulse=0.18f;
                }
                foreach(LineRenderer line in root.GetComponentsInChildren<LineRenderer>())
                { line.startWidth=line.endWidth=0.012f; }
                Particles(root.transform,"ExpandingFlames","flame_04.png",new Color(1f,0.36f,0.03f,0.85f),0.87f,0.3f,0.48f,32,false);
                Particles(root.transform,"HotEmbers","spark_06.png",new Color(1f,0.83f,0.16f,1f),0.9f,0.07f,0.5f,24,false);
            }
            else if(mage)
            {
                GameObject swirl=Primitive(root.transform,"StormVortex",PrimitiveType.Quad,new Vector3(0,0.01f,0),Vector3.one*1.8f,
                    Mat(root.name+"_Vortex","twirl_01.png",new Color(0.25f,0.46f,1f,0.52f)));
                swirl.transform.localRotation=Quaternion.Euler(-90f,0,0);
                swirl.AddComponent<SkillVfxMotion>().spin=new Vector3(0,0,45);
                Particles(root.transform,"StormFilaments","magic_01.png",tint,0.8f,0.4f,0.8f,20,true);
            }
            else
            {
                Particles(root.transform,"DustAndSparks","spark_06.png",gold,0.85f,0.075f,0.5f,10,true);
                if(path.Contains("Archer_S1_C"))
                    for(int i=0;i<4;i++)
                    {
                        GameObject spike=Primitive(root.transform,"TrapJaw",PrimitiveType.Cube,new Vector3(0,0.04f,0),
                            new Vector3(0.025f,0.018f,0.36f),Mat("TrapMetal",null,new Color(0.6f,0.43f,0.13f,1),0,false));
                        spike.transform.localRotation=Quaternion.Euler(0,i*45,0);
                    }
            }
        }

        static GameObject Primitive(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Material material)
        {
            GameObject go=GameObject.CreatePrimitive(type); go.name=name;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(parent,false); go.transform.localPosition=position; go.transform.localScale=scale;
            Renderer renderer=go.GetComponent<Renderer>(); renderer.sharedMaterial=material;
            renderer.shadowCastingMode=ShadowCastingMode.Off; renderer.receiveShadows=false;
            return go;
        }

        static void CreateArrow(Transform root,Color tint,bool precise)
        {
            Material metal=Mat("ArrowShaft",null,new Color(0.72f,0.52f,0.24f,1),0,false);
            var shaft=Primitive(root,"ArrowShaft",PrimitiveType.Cylinder,Vector3.zero,
                new Vector3(0.045f,0.42f,0.045f),metal);
            shaft.transform.localRotation=Quaternion.Euler(90,0,0);
            var tip=Primitive(root,"Arrowhead",PrimitiveType.Cube,new Vector3(0,0,0.5f),new Vector3(0.12f,0.035f,0.22f),
                Mat(precise?"PrecisionArrowHead":"ArrowHead",null,tint));
            tip.transform.localRotation=Quaternion.Euler(0,0,45);
            for(int i=0;i<2;i++)
            {
                var fin=Primitive(root,"Fletching",PrimitiveType.Cube,new Vector3(0,0,-0.34f),new Vector3(0.18f,0.018f,0.24f),metal);
                fin.transform.localRotation=Quaternion.Euler(0,0,i*90);
            }
            Particles(root,"ArrowGlow","light_02.png",tint,0.04f,precise?0.35f:0.16f,0.18f,8,true);
        }

        static void Ring(Transform parent,string name,float radius,float width,Material material)
        {
            GameObject go=new GameObject(name); go.transform.SetParent(parent,false);
            LineRenderer line=go.AddComponent<LineRenderer>(); line.useWorldSpace=false; line.loop=true; line.positionCount=48;
            line.sharedMaterial=material; line.startWidth=line.endWidth=width;
            for(int i=0;i<48;i++) { float a=i*Mathf.PI*2/48; line.SetPosition(i,new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*radius); }
        }

        static void Particles(Transform parent,string name,string texture,Color tint,float radius,float size,float lifetime,int count,bool loop,bool additive=true)
        {
            GameObject go=new GameObject(name); go.transform.SetParent(parent,false);
            ParticleSystem ps=go.AddComponent<ParticleSystem>(); ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            var main=ps.main; main.loop=loop; main.duration=Mathf.Max(0.5f,lifetime); main.startLifetime=lifetime;
            main.startSpeed=0.1f; main.startSize=new ParticleSystem.MinMaxCurve(size*0.7f,size*1.3f);
            main.startColor=Color.white; main.maxParticles=32; main.scalingMode=ParticleSystemScalingMode.Hierarchy;
            main.startRotation=new ParticleSystem.MinMaxCurve(0,Mathf.PI*2);
            var shape=ps.shape; shape.shapeType=ParticleSystemShapeType.Circle; shape.radius=radius;
            shape.rotation=new Vector3(-90,0,0); shape.radiusThickness=loop?1f:0.15f;
            var emission=ps.emission; emission.rateOverTime=loop?count:0;
            if(!loop) emission.SetBursts(new[]{new ParticleSystem.Burst(0,(short)count)});
            var velocity=ps.velocityOverLifetime; velocity.enabled=true; velocity.space=ParticleSystemSimulationSpace.Local;
            velocity.y=0.22f;
            var fade=ps.colorOverLifetime; fade.enabled=true; Gradient gradient=new Gradient();
            gradient.SetKeys(new[]{new GradientColorKey(Color.white,0),new GradientColorKey(Color.white,1)},
                new[]{new GradientAlphaKey(0,0),new GradientAlphaKey(1,0.15f),new GradientAlphaKey(0,1)});
            fade.color=gradient;
            var sizeOver=ps.sizeOverLifetime; sizeOver.enabled=true;
            sizeOver.size=new ParticleSystem.MinMaxCurve(1,new AnimationCurve(new Keyframe(0,0.4f),new Keyframe(0.3f,1),new Keyframe(1,0.5f)));
            ParticleSystemRenderer renderer=ps.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial=Mat(parent.name+"_"+name,texture,tint,0,additive);
            renderer.shadowCastingMode=ShadowCastingMode.Off;
        }

        static Material Mat(string name,string texture,Color tint,float mode=0,bool additive=true)
        {
            string path=Materials+"Polished_"+name+".mat";
            Material mat=AssetDatabase.LoadAssetAtPath<Material>(path);
            Shader shader=Shader.Find("BattleRoyaleX/Combat FX");
            if(mat==null) { mat=new Material(shader); AssetDatabase.CreateAsset(mat,path); }
            mat.shader=shader; mat.SetColor("_Tint",tint); mat.SetFloat("_Mode",mode);
            mat.SetFloat("_Intensity",mode==2?1f:1.35f);
            mat.SetFloat("_DstBlend",additive?(float)BlendMode.One:(float)BlendMode.OneMinusSrcAlpha);
            if(texture!=null)
            {
                string assetPath=Textures+texture;
                TextureImporter importer=AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if(importer!=null && (importer.mipmapEnabled || importer.maxTextureSize!=512))
                { importer.mipmapEnabled=false; importer.alphaIsTransparency=true; importer.maxTextureSize=512; importer.wrapMode=TextureWrapMode.Clamp; importer.SaveAndReimport(); }
                mat.SetTexture("_MainTex",AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath));
            }
            else mat.SetTexture("_MainTex",Texture2D.whiteTexture);
            EditorUtility.SetDirty(mat); return mat;
        }
    }
}
#endif
