using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BattleRoyaleX
{
    /// <summary>Local presentation only; the external host owns following, visibility and combat lifecycle.</summary>
    public sealed class AssassinReferenceVfx : MonoBehaviour
    {
        // 0 startup, 1 dash/cut wake, 2 execution contact, 3 hunt curve, 4 five-cuts cross,
        // 5 return marker, 6 defensive contact, 7 basic crescent, 8 dodge wake.
        const int Steps=32;
        static readonly Color Violet=new Color(.55f,.12f,1.05f,.75f), Silver=new Color(1.05f,.84f,1.25f,.85f);
        sealed class Strip
        { public Mesh mesh; public Vector3[] vertices; public Color[] colors; public Color tint; public int mode; public float width,offset; }
        readonly List<Material> materials=new List<Material>(5);
        readonly List<Mesh> meshes=new List<Mesh>(10);
        readonly List<Strip> strips=new List<Strip>(6);
        Transform root;
        Material ribbonMaterial,solidMaterial,mistMaterial,sparkMaterial;
        Mesh shardsMesh;
        Vector3[] shardVertices;
        Mesh spectersMesh;
        Vector3[] specterVertices;
        static readonly Vector3[] SpecterShape={
            new Vector3(0,.65f,0),new Vector3(-.13f,.43f,0),new Vector3(.13f,.43f,0),
            new Vector3(-.25f,.22f,0),new Vector3(.22f,.15f,0),new Vector3(.12f,-.35f,0),new Vector3(-.14f,-.3f,0),
            new Vector3(-.4f,-.58f,0),new Vector3(.32f,-.49f,0),new Vector3(-.12f,-.79f,0),new Vector3(.19f,-.73f,0)};
        int style;
        float began,lifetime,radius;

        public void Begin(int visualStyle,float duration,float size)
        {
            if(root!=null)return;
            style=Mathf.Clamp(visualStyle,0,8);lifetime=Mathf.Max(.05f,duration);radius=Mathf.Clamp(size,.2f,3.5f);began=Time.time;
            root=new GameObject("Assassin Violet Geometry").transform;root.SetParent(transform,false);
            Shader glow=Resources.Load<Shader>("ArcaneGlow"), solid=Resources.Load<Shader>("BRXAimPreview"), mist=Resources.Load<Shader>("AssassinMist");
            if(glow==null||solid==null||mist==null){Destroy(root.gameObject);Destroy(this);return;}
            ribbonMaterial=NewMaterial(glow,"Assassin feathered ribbons");ribbonMaterial.SetFloat("_Shape",1);
            sparkMaterial=NewMaterial(glow,"Assassin silver-violet motes");sparkMaterial.SetFloat("_Shape",0);
            solidMaterial=NewMaterial(solid,"Assassin graphite facets");solidMaterial.color=Color.white;
            mistMaterial=NewMaterial(mist,"Assassin organic mist");mistMaterial.color=Color.white;
            if(style==0)
            {Layer(0,.16f,0);Layer(5,.045f,0);CreateParticles(false,0,8);CreateParticles(true,0,6);}
            else if(style==1||style==8)
            {Layer(1,style==8?.16f:.24f,0);CreateTrail(-.13f);CreateTrail(.13f);CreateSpecters();CreateParticles(false,10,0);CreateParticles(true,8,0);}
            else if(style==2)
            {Layer(2,.22f,0);CreateShards();CreateParticles(false,28,0);CreateParticles(true,18,0);}
            else if(style==3)
            {Layer(3,.21f,0);CreateShards();CreateParticles(false,22,0);CreateParticles(true,10,0);}
            else if(style==4)
            {Layer(4,.22f,0);Layer(4,.22f,1);CreateShards();CreateParticles(false,30,0);CreateParticles(true,12,0);}
            else if(style==5)
            {Layer(5,.045f,0);Layer(0,.095f,0);CreateParticles(false,0,5);}
            else if(style==6)
            {Layer(2,.13f,0);CreateParticles(false,20,0);CreateParticles(true,6,0);}
            else
            {Layer(7,.18f,0);CreateParticles(false,14,0);}
            UpdateGeometry(0);
        }

        Material NewMaterial(Shader shader,string label)
        {var value=new Material(shader){name=label,renderQueue=3000};materials.Add(value);return value;}
        void ConfigureRenderer(Renderer renderer,Material material)
        {
            renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
            renderer.lightProbeUsage=LightProbeUsage.Off;renderer.reflectionProbeUsage=ReflectionProbeUsage.Off;
        }
        void Layer(int mode,float width,float offset)
        {AddStrip(mode,width,offset,Violet);AddStrip(mode,width*.3f,offset,Silver);}
        void AddStrip(int mode,float width,float offset,Color tint)
        {
            int lanes=mode==0||mode==5?4:mode==1?3:mode==2?5:1;
            var vertices=new Vector3[(Steps+1)*2*lanes];var colors=new Color[vertices.Length];var uv=new Vector2[vertices.Length];
            var triangles=new int[Steps*6*lanes];
            for(int lane=0;lane<lanes;lane++)for(int point=0;point<=Steps;point++)
            {
                int at=lane*(Steps+1)*2+point*2;float u=(float)point/Steps;
                uv[at]=new Vector2(u,0);uv[at+1]=new Vector2(u,1);colors[at]=colors[at+1]=tint;
                if(point==Steps)continue;
                int tri=lane*Steps*6+point*6;triangles[tri]=at;triangles[tri+1]=at+2;triangles[tri+2]=at+1;
                triangles[tri+3]=at+1;triangles[tri+4]=at+2;triangles[tri+5]=at+3;
            }
            var mesh=new Mesh{name="Assassin dimensional strip",vertices=vertices,colors=colors,uv=uv,triangles=triangles};mesh.MarkDynamic();meshes.Add(mesh);
            var item=new GameObject("Violet strip "+mode,typeof(MeshFilter),typeof(MeshRenderer));item.transform.SetParent(root,false);
            item.GetComponent<MeshFilter>().sharedMesh=mesh;ConfigureRenderer(item.GetComponent<MeshRenderer>(),ribbonMaterial);
            strips.Add(new Strip{mesh=mesh,vertices=vertices,colors=colors,tint=tint,mode=mode,width=width,offset=offset});
        }
        void CreateTrail(float side)
        {
            var item=new GameObject("Actual movement wake",typeof(TrailRenderer));item.transform.SetParent(root,false);item.transform.localPosition=new Vector3(side,.15f,-.25f);
            var trail=item.GetComponent<TrailRenderer>();trail.time=.18f;trail.minVertexDistance=.065f;trail.widthMultiplier=.15f;
            trail.widthCurve=AnimationCurve.Linear(0,1,1,0);trail.colorGradient=Fade(Violet);ConfigureRenderer(trail,ribbonMaterial);
        }
        void CreateShards()
        {
            const int count=10;shardVertices=new Vector3[count*6];var colors=new Color[shardVertices.Length];var triangles=new int[count*24];
            int[] facets={0,2,3,0,3,4,0,4,5,0,5,2,1,3,2,1,4,3,1,5,4,1,2,5};
            for(int shard=0;shard<count;shard++)
            {
                for(int vertex=0;vertex<6;vertex++)colors[shard*6+vertex]=vertex==0?new Color(.65f,.28f,.95f,.8f):new Color(.065f,.04f,.095f,.95f);
                for(int triangle=0;triangle<24;triangle++)triangles[shard*24+triangle]=shard*6+facets[triangle];
            }
            shardsMesh=new Mesh{name="Assassin graphite fragments",vertices=shardVertices,colors=colors,triangles=triangles};shardsMesh.MarkDynamic();meshes.Add(shardsMesh);
            var item=new GameObject("Graphite fracture facets",typeof(MeshFilter),typeof(MeshRenderer));item.transform.SetParent(root,false);
            item.GetComponent<MeshFilter>().sharedMesh=shardsMesh;ConfigureRenderer(item.GetComponent<MeshRenderer>(),solidMaterial);
        }
        void CreateSpecters()
        {
            int[] template={0,1,2,1,3,4,1,4,2,3,6,5,3,5,4,6,7,5,5,7,8,6,9,5,5,10,4};
            specterVertices=new Vector3[SpecterShape.Length*3];var colors=new Color[specterVertices.Length];var triangles=new int[template.Length*3];
            for(int ghost=0;ghost<3;ghost++)
            {
                for(int vertex=0;vertex<SpecterShape.Length;vertex++)colors[ghost*SpecterShape.Length+vertex]=
                    vertex==0||vertex==7?new Color(.31f,.13f,.52f,.32f/(ghost+1)):new Color(.035f,.025f,.05f,.35f/(ghost+1));
                for(int triangle=0;triangle<template.Length;triangle++)triangles[ghost*template.Length+triangle]=ghost*SpecterShape.Length+template[triangle];
            }
            spectersMesh=new Mesh{name="Three stylized graphite afterimages",vertices=specterVertices,colors=colors,triangles=triangles};
            spectersMesh.MarkDynamic();meshes.Add(spectersMesh);
            var item=new GameObject("Procedural hood and cloak afterimages",typeof(MeshFilter),typeof(MeshRenderer));item.transform.SetParent(root,false);
            item.GetComponent<MeshFilter>().sharedMesh=spectersMesh;ConfigureRenderer(item.GetComponent<MeshRenderer>(),solidMaterial);
        }
        void CreateParticles(bool smoke,int burst,float rate)
        {
            var item=new GameObject(smoke?"Organic graphite smoke":"Violet silver sparks",typeof(ParticleSystem));item.transform.SetParent(root,false);
            var particles=item.GetComponent<ParticleSystem>();particles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            var main=particles.main;main.playOnAwake=false;main.loop=rate>0;main.duration=lifetime;
            main.startLifetime=new ParticleSystem.MinMaxCurve(smoke?.28f:.15f,smoke?.65f:.4f);
            main.startSpeed=new ParticleSystem.MinMaxCurve(smoke?.12f:.5f,smoke?.4f:2.6f);
            main.startSize=new ParticleSystem.MinMaxCurve(smoke?.45f:.035f,smoke?.9f:.10f);
            main.startColor=smoke?new Color(.15f,.055f,.23f,.7f):Silver;
            main.maxParticles=smoke?40:48;main.simulationSpace=ParticleSystemSimulationSpace.World;
            var emission=particles.emission;emission.rateOverTime=rate;if(burst>0)emission.SetBursts(new[]{new ParticleSystem.Burst(0,(short)burst)});
            var shape=particles.shape;shape.shapeType=ParticleSystemShapeType.Sphere;shape.radius=smoke?.22f:.12f;
            var fade=particles.colorOverLifetime;fade.enabled=true;fade.color=Fade(Color.white);
            var shrink=particles.sizeOverLifetime;shrink.enabled=true;shrink.size=new ParticleSystem.MinMaxCurve(1,AnimationCurve.Linear(0,smoke?.7f:1,1,smoke?1.4f:0));
            ConfigureRenderer(particles.GetComponent<ParticleSystemRenderer>(),smoke?mistMaterial:sparkMaterial);particles.Play();
        }
        static Gradient Fade(Color color)
        {
            var value=new Gradient();value.SetKeys(new[]{new GradientColorKey(color,0),new GradientColorKey(color,1)},
                new[]{new GradientAlphaKey(0,0),new GradientAlphaKey(color.a,.12f),new GradientAlphaKey(0,1)});return value;
        }
        void Update()
        {
            if(root==null)return;float progress=(Time.time-began)/lifetime;
            if(progress>=1){Destroy(root.gameObject);Destroy(this);return;}UpdateGeometry(Mathf.Clamp01(progress));
        }
        void UpdateGeometry(float progress)
        {
            float elapsed=Time.time-began,fade=Mathf.Clamp01((1-progress)*4);
            if(style==5)root.localPosition=new Vector3(0,.055f-transform.position.y,0);
            solidMaterial.color=new Color(1,1,1,fade);mistMaterial.SetFloat("_Opacity",fade);
            foreach(Strip strip in strips)
            {
                int lanes=strip.mode==0||strip.mode==5?4:strip.mode==1?3:strip.mode==2?5:1;
                Color color=strip.tint;color.a*=fade;
                for(int lane=0;lane<lanes;lane++)for(int point=0;point<=Steps;point++)
                {
                    float u=(float)point/Steps,width=strip.width;Vector3 center,across;
                    if(strip.mode==0)
                    {
                        float angle=lane*Mathf.PI/4;Vector3 direction=new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0);
                        center=new Vector3(0,.9f,.05f)+direction*(u-.5f)*radius*1.6f;across=new Vector3(-direction.y,direction.x,0);
                        width*=Mathf.Sin(u*Mathf.PI)*(.65f+.35f*Mathf.Sin(elapsed*6+lane));
                    }
                    else if(strip.mode==1)
                    {
                        center=new Vector3((lane-1)*.17f+Mathf.Sin(u*9+elapsed*6)*.04f,.1f+lane*.2f,-u*radius*1.6f);
                        across=new Vector3(1,.2f,0);width*=Mathf.Sin(u*Mathf.PI)*(1-u*.7f);
                    }
                    else if(strip.mode==2)
                    {
                        float angle=lane*2.399963f;Vector3 direction=new Vector3(Mathf.Sin(angle),.4f+(lane%2)*.65f,Mathf.Cos(angle));
                        center=direction*u*radius*Mathf.Lerp(.4f,1.1f,progress);across=Vector3.Cross(direction,Vector3.up).normalized;width*=Mathf.Sin(u*Mathf.PI);
                    }
                    else if(strip.mode==3)
                    {
                        float angle=u*Mathf.PI*2.15f+progress*.8f,r=radius*(.35f+u*.65f);
                        center=new Vector3(Mathf.Sin(angle)*r,.1f+u*.45f,Mathf.Cos(angle)*r);
                        across=new Vector3(Mathf.Sin(angle),.2f,Mathf.Cos(angle));width*=Mathf.Sin(u*Mathf.PI);
                    }
                    else if(strip.mode==4)
                    {
                        float x=(u-.5f)*radius*2;center=new Vector3(x,.3f+(strip.offset>.5f?-x:x),.1f);
                        across=strip.offset>.5f?new Vector3(.7f,.7f,0):new Vector3(-.7f,.7f,0);width*=Mathf.Sin(u*Mathf.PI);
                    }
                    else if(strip.mode==5)
                    {
                        float angle=lane*Mathf.PI/4;Vector3 direction=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));
                        center=direction*(u-.5f)*radius*1.5f+Vector3.up*.025f;across=Vector3.Cross(direction,Vector3.up);width*=Mathf.Sin(u*Mathf.PI);
                    }
                    else
                    {
                        float angle=Mathf.Lerp(-1.8f,1.8f,u)+progress*.65f;
                        center=new Vector3(Mathf.Sin(angle)*radius*.75f,.2f+Mathf.Cos(angle)*radius*.55f,.45f+Mathf.Sin(angle)*.16f);
                        across=new Vector3(Mathf.Sin(angle),Mathf.Cos(angle),0);width*=Mathf.Sin(u*Mathf.PI);
                    }
                    int at=lane*(Steps+1)*2+point*2;strip.vertices[at]=center-across*width;strip.vertices[at+1]=center+across*width;
                    strip.colors[at]=strip.colors[at+1]=color;
                }
                strip.mesh.vertices=strip.vertices;strip.mesh.colors=strip.colors;strip.mesh.RecalculateBounds();
            }
            if(shardsMesh!=null)
            {
                for(int shard=0;shard<10;shard++)
                {
                    float angle=shard*2.399963f+elapsed,r=radius*(.18f+progress*.65f);
                    Vector3 center=new Vector3(Mathf.Sin(angle)*r,Mathf.Sin(progress*Mathf.PI)*(.6f+(shard%3)*.2f),Mathf.Cos(angle)*r);
                    Quaternion rotation=Quaternion.Euler(shard*13+elapsed*70,shard*23,elapsed*100);float size=.07f+(shard%4)*.017f;
                    for(int vertex=0;vertex<6;vertex++)
                    {Vector3 point=vertex==0?Vector3.up:vertex==1?Vector3.down:vertex==2?Vector3.left:vertex==3?Vector3.forward:vertex==4?Vector3.right:Vector3.back;
                        shardVertices[shard*6+vertex]=center+rotation*(point*size);}
                }
                shardsMesh.vertices=shardVertices;shardsMesh.RecalculateBounds();
            }
            if(spectersMesh!=null)
            {
                Camera camera=Camera.main;
                Quaternion rotation=camera!=null?Quaternion.Inverse(root.rotation)*camera.transform.rotation:Quaternion.identity;
                for(int ghost=0;ghost<3;ghost++)
                {
                    Vector3 center=new Vector3((ghost%2==0?.06f:-.06f),.1f,-radius*(.4f+ghost*.48f));
                    for(int vertex=0;vertex<SpecterShape.Length;vertex++)specterVertices[ghost*SpecterShape.Length+vertex]=center+rotation*SpecterShape[vertex];
                }
                spectersMesh.vertices=specterVertices;spectersMesh.RecalculateBounds();
            }
        }
        void OnDestroy()
        {
            if(root!=null)Destroy(root.gameObject);
            foreach(Material material in materials)if(material!=null)Destroy(material);
            foreach(Mesh mesh in meshes)if(mesh!=null)Destroy(mesh);
        }
    }
}
