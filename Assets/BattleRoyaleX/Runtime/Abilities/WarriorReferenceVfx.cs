using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BattleRoyaleX
{
    /// <summary>Procedural reference-inspired presentation. The host remains the lifetime/combat authority.</summary>
    public sealed class WarriorReferenceVfx : MonoBehaviour
    {
        // 0 blast, 1 sword field, 2 helix, 3 guard, 4 absorption dome, 5 fortress accent,
        // 6 impact fan, 7 crescent, 8 cross, 9 swirl, 10/11/12 ultimate charging, 13 blue-gold defensive contact.
        const int Steps=48;
        static readonly Color Gold=new Color(1.2f,.63f,.12f,.8f), Core=new Color(1.35f,1.14f,.7f,.88f), Red=new Color(1f,.16f,.025f,.66f);
        sealed class Ribbon
        {
            public Mesh mesh; public Vector3[] vertices; public int mode; public float width, offset; public Transform transform;
        }
        readonly List<Material> materials=new List<Material>(5);
        readonly List<Mesh> meshes=new List<Mesh>(12);
        readonly List<Ribbon> ribbons=new List<Ribbon>(10);
        readonly List<Transform> groundObjects=new List<Transform>(2);
        Transform root;
        Material energyMaterial, solidMaterial, particleMaterial, shieldMaterial;
        int style;
        float started,lifetime,radius;
        Mesh swordsMesh,debrisMesh;
        Vector3[] swordsVertices,debrisVertices;
        Vector3[] swordTemplate;
        readonly float[] previousSwordPhase=new float[8];
        ParticleSystem sparks;

        public void Begin(int visualStyle,float duration,float size)
        {
            if(root!=null)return;
            style=Mathf.Clamp(visualStyle,0,13);lifetime=Mathf.Max(.05f,duration);radius=Mathf.Clamp(size,.25f,4f);started=Time.time;
            root=new GameObject("Reference Energy Geometry").transform;root.SetParent(transform,false);
            Shader energy=Resources.Load<Shader>("WarriorEnergy"), solid=Resources.Load<Shader>("BRXAimPreview"), soft=Resources.Load<Shader>("ArcaneGlow");
            if(energy==null||solid==null||soft==null){Destroy(root.gameObject);Destroy(this);return;}
            energyMaterial=NewMaterial(energy,"Warm layered energy");solidMaterial=NewMaterial(solid,"Spectral blades and fragments");solidMaterial.color=Color.white;
            particleMaterial=NewMaterial(soft,"Soft reference sparks");particleMaterial.SetFloat("_Shape",0);
            shieldMaterial=NewMaterial(energy,"Blue-gold hex shield");shieldMaterial.SetFloat("_Mode",1);shieldMaterial.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);
            bool ultimate=style<=2||(style>=10&&style<=12);
            if(ultimate||style==4||style==5)
            {groundObjects.Add(root);root.localPosition=new Vector3(0,.055f-transform.position.y,0);}
            if(style==0||style==10)
            {
                AddRibbon(0,.5f,0,Red);AddRibbon(0,.15f,0,Core);
                AddRibbon(1,.15f,0,Gold);if(style==0)CreateDebris();
                CreateSparks(style==0?46:0,style==10?12:0);
            }
            else if(style==1||style==11)
            {
                CreateSwords();AddRibbon(1,.085f,0,Gold);AddRibbon(1,.04f,.1f,Core);
                CreateSparks(0,style==11?8:5);
            }
            else if(style==2||style==12)
            {
                AddRibbon(0,.25f,0,Gold);AddRibbon(0,.065f,0,Core);
                AddRibbon(2,.2f,0,Red);AddRibbon(2,.075f,0,Core);
                AddRibbon(2,.2f,Mathf.PI,Gold);AddRibbon(2,.065f,Mathf.PI,Core);
                AddRibbon(1,.11f,0,Gold);if(style==2)CreateDebris();CreateSparks(style==2?22:0,12);
            }
            else if(style==3||style==4)
            {
                CreateShield(style==4);CreateSparks(0,5);
            }
            else if(style==5)
            {
                // Accent only. Existing Fortress is the sole owner of its three shield faces.
                AddRibbon(1,.055f,0,new Color(.2f,.65f,1f,.6f));CreateSparks(0,7);
            }
            else
            {
                int mode=style==7?3:style==8?4:style==9?5:6;
                AddRibbon(mode,style==6||style==13?.19f:.26f,0,style==13?new Color(.14f,.56f,1f,.72f):Red);
                AddRibbon(mode,.075f,0,style==13?new Color(1.2f,.91f,.46f,.85f):Core);
                if(style==8){AddRibbon(4,.26f,1,Gold);AddRibbon(4,.075f,1,Core);}
                if(style==9){AddRibbon(5,.13f,Mathf.PI,Gold);}
                CreateSparks(style==6?34:style==13?18:12,0);
            }
            UpdateGeometry(0);
        }

        Material NewMaterial(Shader shader,string name)
        {var value=new Material(shader){name=name,renderQueue=3000};materials.Add(value);return value;}
        GameObject MeshObject(string name,Mesh mesh,Material material)
        {
            var item=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));item.transform.SetParent(root,false);
            item.GetComponent<MeshFilter>().sharedMesh=mesh;var renderer=item.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;
            renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
            renderer.lightProbeUsage=LightProbeUsage.Off;renderer.reflectionProbeUsage=ReflectionProbeUsage.Off;return item;
        }
        void AddRibbon(int mode,float width,float offset,Color color)
        {
            int planes=mode==0?3:mode==6?5:1;
            var vertices=new Vector3[(Steps+1)*2*planes];var colors=new Color[vertices.Length];var uv=new Vector2[vertices.Length];
            var triangles=new int[Steps*6*planes];
            for(int p=0;p<planes;p++)for(int i=0;i<=Steps;i++)
            {
                int at=p*(Steps+1)*2+i*2;float progress=(float)i/Steps;
                colors[at]=colors[at+1]=color;uv[at]=new Vector2(progress,0);uv[at+1]=new Vector2(progress,1);
                if(i==Steps)continue;
                int tri=p*Steps*6+i*6;triangles[tri]=at;triangles[tri+1]=at+2;triangles[tri+2]=at+1;
                triangles[tri+3]=at+1;triangles[tri+4]=at+2;triangles[tri+5]=at+3;
            }
            var mesh=new Mesh{name="Reference energy ribbon",vertices=vertices,colors=colors,uv=uv,triangles=triangles};mesh.MarkDynamic();meshes.Add(mesh);
            var item=MeshObject("Layered energy "+mode,mesh,energyMaterial);
            ribbons.Add(new Ribbon{mesh=mesh,vertices=vertices,mode=mode,width=width,offset=offset,transform=item.transform});
        }

        void CreateSwords()
        {
            // Flat double-sided blade, central ridge, guard and handle. Eight visible swords, one renderer.
            swordTemplate=new[]{new Vector3(0,-.78f,0),new Vector3(-.105f,.28f,0),new Vector3(0,.48f,.025f),new Vector3(.105f,.28f,0),
                new Vector3(-.24f,.48f,0),new Vector3(.24f,.48f,0),new Vector3(.22f,.55f,0),new Vector3(-.22f,.55f,0),
                new Vector3(-.035f,.53f,0),new Vector3(.035f,.53f,0),new Vector3(.035f,.78f,0),new Vector3(-.035f,.78f,0)};
            int[] templateTriangles={0,1,2,0,2,3,4,5,6,4,6,7,8,9,10,8,10,11};
            swordsVertices=new Vector3[swordTemplate.Length*8];var colors=new Color[swordsVertices.Length];var triangles=new int[templateTriangles.Length*8];
            for(int sword=0;sword<8;sword++)
            {
                for(int i=0;i<swordTemplate.Length;i++)colors[sword*swordTemplate.Length+i]=i==0||i==2?Core:Gold;
                for(int i=0;i<templateTriangles.Length;i++)triangles[sword*templateTriangles.Length+i]=sword*swordTemplate.Length+templateTriangles[i];
            }
            swordsMesh=new Mesh{name="Eight spectral swords",vertices=swordsVertices,colors=colors,triangles=triangles};swordsMesh.MarkDynamic();meshes.Add(swordsMesh);
            MeshObject("Eight falling spectral swords",swordsMesh,solidMaterial);
        }

        void CreateDebris()
        {
            const int count=12;debrisVertices=new Vector3[count*6];var colors=new Color[debrisVertices.Length];var triangles=new int[count*24];
            int[] octahedron={0,2,3,0,3,4,0,4,5,0,5,2,1,3,2,1,4,3,1,5,4,1,2,5};
            for(int i=0;i<count;i++)
            {
                for(int j=0;j<6;j++)colors[i*6+j]=j==0?new Color(.75f,.36f,.075f,.8f):new Color(.17f,.12f,.085f,.95f);
                for(int j=0;j<24;j++)triangles[i*24+j]=i*6+octahedron[j];
            }
            debrisMesh=new Mesh{name="Floating fractured stone",vertices=debrisVertices,colors=colors,triangles=triangles};debrisMesh.MarkDynamic();meshes.Add(debrisMesh);
            MeshObject("Twelve energized fragments",debrisMesh,solidMaterial);
        }

        void CreateShield(bool dome)
        {
            const int columns=24,rows=12;var vertices=new Vector3[(columns+1)*(rows+1)];var uv=new Vector2[vertices.Length];
            var colors=new Color[vertices.Length];var triangles=new int[columns*rows*6];
            for(int row=0;row<=rows;row++)for(int column=0;column<=columns;column++)
            {
                float u=(float)column/columns,v=(float)row/rows;int at=row*(columns+1)+column;
                if(dome)
                {
                    float a=u*Mathf.PI*2,b=v*Mathf.PI*.5f;
                    vertices[at]=new Vector3(Mathf.Sin(a)*Mathf.Cos(b)*1.3f,Mathf.Sin(b)*1.6f,Mathf.Cos(a)*Mathf.Cos(b)*1.3f);
                }
                else
                {float a=(u-.5f)*Mathf.PI*.72f;vertices[at]=new Vector3(Mathf.Sin(a)*.9f,v*1.65f-.73f,.8f+(1-Mathf.Cos(a))*.25f);}
                colors[at]=Color.white;uv[at]=new Vector2(u,v);
                if(row==rows||column==columns)continue;
                int next=at+columns+1,tri=(row*columns+column)*6;
                triangles[tri]=at;triangles[tri+1]=next;triangles[tri+2]=at+1;triangles[tri+3]=at+1;triangles[tri+4]=next;triangles[tri+5]=next+1;
            }
            var mesh=new Mesh{name=dome?"Blue hex hemisphere":"Curved blue gold shield",vertices=vertices,colors=colors,uv=uv,triangles=triangles};
            mesh.RecalculateNormals();mesh.RecalculateBounds();meshes.Add(mesh);MeshObject(mesh.name,mesh,shieldMaterial);
            if(dome)AddRibbon(1,.065f,0,new Color(1f,.78f,.31f,.75f));
        }

        void CreateSparks(int burst,float rate)
        {
            var item=new GameObject("Soft sparks and embers",typeof(ParticleSystem));item.transform.SetParent(root,false);
            sparks=item.GetComponent<ParticleSystem>();sparks.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            var main=sparks.main;main.playOnAwake=false;main.loop=rate>0;main.duration=lifetime;
            main.startLifetime=new ParticleSystem.MinMaxCurve(.18f,.6f);main.startSpeed=new ParticleSystem.MinMaxCurve(.5f,style==0?4f:1.5f);
            main.startSize=new ParticleSystem.MinMaxCurve(.035f,.1f);main.startColor=style==3||style==4||style==5||style==13?new Color(.35f,.8f,1f,.75f):Gold;
            main.maxParticles=96;main.simulationSpace=ParticleSystemSimulationSpace.World;
            var emission=sparks.emission;emission.rateOverTime=rate;if(burst>0)emission.SetBursts(new[]{new ParticleSystem.Burst(0,(short)burst)});
            var shape=sparks.shape;shape.shapeType=rate>0?ParticleSystemShapeType.Cone:ParticleSystemShapeType.Sphere;
            shape.radius=rate>0?radius*.75f:.12f;shape.angle=12f;
            if(rate>0)item.transform.localRotation=Quaternion.Euler(-90,0,0);
            var fade=sparks.colorOverLifetime;fade.enabled=true;var gradient=new Gradient();
            gradient.SetKeys(new[]{new GradientColorKey(Color.white,0),new GradientColorKey(Color.white,1)},new[]{new GradientAlphaKey(0,0),new GradientAlphaKey(1,.1f),new GradientAlphaKey(0,1)});fade.color=gradient;
            var shrink=sparks.sizeOverLifetime;shrink.enabled=true;shrink.size=new ParticleSystem.MinMaxCurve(1,AnimationCurve.Linear(0,1,1,0));
            var renderer=sparks.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=particleMaterial;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
            sparks.Play();
        }

        void Update()
        {
            if(root==null)return;float progress=(Time.time-started)/lifetime;
            if(progress>=1){Destroy(root.gameObject);Destroy(this);return;}UpdateGeometry(Mathf.Clamp01(progress));
        }
        void UpdateGeometry(float progress)
        {
            float elapsed=Time.time-started,fade=Mathf.Clamp01((1-progress)*5),charge=style>=10&&style<=12?Mathf.Lerp(.25f,1,progress):1;
            foreach(Transform anchor in groundObjects)anchor.localPosition=new Vector3(0,.055f-transform.position.y,0);
            energyMaterial.SetFloat("_Opacity",fade*charge);shieldMaterial.SetFloat("_Opacity",fade);solidMaterial.color=new Color(1,1,1,fade);
            foreach(Ribbon ribbon in ribbons)
            {
                int planes=ribbon.mode==0?3:ribbon.mode==6?5:1;
                for(int plane=0;plane<planes;plane++)for(int i=0;i<=Steps;i++)
                {
                    float u=(float)i/Steps;Vector3 center,across;float width=ribbon.width;
                    if(ribbon.mode==0)
                    {
                        float a=plane*Mathf.PI/3;center=Vector3.up*(u*(style>=10?2.8f*charge:4f));
                        across=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));width*=.75f+.25f*Mathf.Sin(u*19+elapsed*8);
                    }
                    else if(ribbon.mode==1)
                    {
                        float a=u*Mathf.PI*2;float r=radius*(style==0||style==2?Mathf.Lerp(.3f,1.05f,progress):1);
                        center=new Vector3(Mathf.Sin(a)*r,.035f+ribbon.offset,Mathf.Cos(a)*r);across=new Vector3(Mathf.Sin(a),0,Mathf.Cos(a));
                    }
                    else if(ribbon.mode==2)
                    {
                        float a=u*Mathf.PI*5+elapsed*4+ribbon.offset,r=radius*.3f*(.7f+.3f*u);
                        center=new Vector3(Mathf.Sin(a)*r,u*3.4f*charge,Mathf.Cos(a)*r);across=new Vector3(Mathf.Sin(a),0,Mathf.Cos(a));
                    }
                    else if(ribbon.mode==3)
                    {
                        float a=Mathf.Lerp(-1.9f,1.9f,u)+progress*.9f,r=radius*.8f;
                        center=new Vector3(Mathf.Sin(a)*r,.45f+Mathf.Cos(a)*r,.8f+.25f*Mathf.Sin(a));
                        across=new Vector3(Mathf.Sin(a),Mathf.Cos(a),0);width*=Mathf.Sin(u*Mathf.PI);
                    }
                    else if(ribbon.mode==4)
                    {
                        float x=(u-.5f)*radius*2.2f;
                        center=new Vector3(x,.35f+(ribbon.offset>.5f?-x:x),.75f);across=new Vector3(-.7f,.7f,0);
                        if(ribbon.offset>.5f)across.x=.7f;width*=Mathf.Sin(u*Mathf.PI);
                    }
                    else if(ribbon.mode==5)
                    {
                        float a=u*Mathf.PI*4+elapsed*8+ribbon.offset,r=radius*.8f;
                        center=new Vector3(Mathf.Sin(a)*r,.15f+u*.9f,Mathf.Cos(a)*r);across=new Vector3(Mathf.Sin(a),.2f,Mathf.Cos(a));
                    }
                    else
                    {
                        float a=plane*.55f-1.1f;Vector3 direction=new Vector3(Mathf.Sin(a),.35f+plane*.06f,Mathf.Cos(a));
                        center=direction*u*radius*Mathf.Lerp(.5f,1.2f,progress);across=Vector3.Cross(direction,Vector3.up).normalized;
                        width*=Mathf.Sin(u*Mathf.PI)*(1-progress);
                    }
                    int at=plane*(Steps+1)*2+i*2;ribbon.vertices[at]=center-across*width;ribbon.vertices[at+1]=center+across*width;
                }
                ribbon.mesh.vertices=ribbon.vertices;ribbon.mesh.RecalculateBounds();
            }
            if(swordsMesh!=null)UpdateSwords(elapsed,progress);
            if(debrisMesh!=null)UpdateDebris(elapsed,progress);
        }

        void UpdateSwords(float elapsed,float progress)
        {
            for(int sword=0;sword<8;sword++)
            {
                float angle=sword*Mathf.PI/4,cycle=Mathf.Repeat(elapsed*.8f+sword*.13f,1);
                float height=style==11?Mathf.Lerp(1.3f,2.4f,progress):Mathf.Lerp(2.7f,.85f,Mathf.SmoothStep(0,1,Mathf.Clamp01((cycle-.3f)/.6f)));
                Vector3 center=new Vector3(Mathf.Sin(angle)*radius*.87f,height,Mathf.Cos(angle)*radius*.87f);
                Quaternion rotation=Quaternion.Euler(0,sword*45f,0);
                for(int i=0;i<swordTemplate.Length;i++)swordsVertices[sword*swordTemplate.Length+i]=center+rotation*swordTemplate[i];
                if(style==1&&cycle>.9f&&previousSwordPhase[sword]<=.9f&&sparks!=null)
                {
                    var emit=new ParticleSystem.EmitParams{position=root.TransformPoint(new Vector3(center.x,.1f,center.z)),startSize=.14f,startLifetime=.3f,startColor=Core};
                    sparks.Emit(emit,3);
                }
                previousSwordPhase[sword]=cycle;
            }
            swordsMesh.vertices=swordsVertices;swordsMesh.RecalculateBounds();
        }
        void UpdateDebris(float elapsed,float progress)
        {
            for(int stone=0;stone<12;stone++)
            {
                float angle=stone*2.399963f+(style==2?elapsed:0),r=radius*(style==2?.6f:.25f+progress*.65f);
                Vector3 center=new Vector3(Mathf.Sin(angle)*r,.2f+Mathf.Sin(progress*Mathf.PI)*(1.5f+(stone%3)*.25f),Mathf.Cos(angle)*r);
                float size=.11f+(stone%4)*.025f;
                Quaternion rotation=Quaternion.Euler(stone*21+elapsed*100,stone*39,elapsed*80);
                for(int vertex=0;vertex<6;vertex++)
                {
                    Vector3 point=vertex==0?Vector3.up:vertex==1?Vector3.down:vertex==2?Vector3.left:vertex==3?Vector3.forward:vertex==4?Vector3.right:Vector3.back;
                    debrisVertices[stone*6+vertex]=center+rotation*(point*size);
                }
            }
            debrisMesh.vertices=debrisVertices;debrisMesh.RecalculateBounds();
        }
        void OnDestroy()
        {
            if(root!=null)Destroy(root.gameObject);
            foreach(Material material in materials)if(material!=null)Destroy(material);
            foreach(Mesh mesh in meshes)if(mesh!=null)Destroy(mesh);
        }
    }
}
