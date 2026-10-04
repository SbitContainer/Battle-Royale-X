using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BattleRoyaleX
{
    // Presentation only. No colliders, rigidbodies, damage or combat-state changes.
    public sealed class WarriorSkillPresentation : MonoBehaviour
    {
        // Append only; existing callers/serialized values retain their meanings.
        public enum Kind { Aura, Absorb, Shields, FlyingShield, Charge, GroundField, Explosion, Conjuration, Impact, Shockwave, Slash, DefenseImpact }
        const int Segments = 48;
        static readonly Vector3[] ShieldOutline = {
            new Vector3(-.46f,-.68f,0), new Vector3(.46f,-.68f,0), new Vector3(.46f,.44f,0),
            new Vector3(.27f,.68f,0), new Vector3(-.27f,.68f,0), new Vector3(-.46f,.44f,0)
        };
        readonly List<Material> materials = new List<Material>(4);
        readonly List<Mesh> meshes = new List<Mesh>(4);
        readonly List<LineRenderer> animatedLines = new List<LineRenderer>(4);
        struct GroundAnchor { public Transform transform; public float height; }
        readonly List<GroundAnchor> groundAnchors = new List<GroundAnchor>(6);
        readonly Transform[] shields = new Transform[3];
        CharacterRuntime owner;
        CharacterDefinition definition;
        Kind kind;
        int resetVersion, actionVersion;
        float started, duration, radius, phase;
        bool follow, ownsHost, finished;
        Transform visualRoot, groundDisc;
        Transform reference;
        Material faceMaterial, shieldFaceMaterial, edgeMaterial, glowMaterial, streakMaterial;
        Color energy;

        public void Begin(CharacterRuntime source, Kind effect, float lifetime, float size, float initialPhase, bool followOwner)
        {
            owner=source; kind=effect; duration=Mathf.Max(.05f,lifetime); radius=Mathf.Max(.12f,size);
            phase=initialPhase; follow=followOwner; started=Time.time;
            definition=source!=null?source.Definition:null;
            resetVersion=source!=null&&source.Abilities!=null?source.Abilities.ResetVersion:-1;
            actionVersion=source!=null&&source.Abilities!=null?source.Abilities.PresentationActionId:-1;
            // Never delete a gameplay projectile just because its presentation ends.
            ownsHost=GetComponent<Hitbox>()==null&&GetComponent<Collider>()==null&&
                (gameObject.name.StartsWith("Warrior_")||GetComponents<Component>().Length==2);
            visualRoot=new GameObject("Warrior Skill Visuals").transform; visualRoot.SetParent(transform,false);
            Shader geometry=Resources.Load<Shader>("BRXAimPreview");
            Shader soft=Resources.Load<Shader>("ArcaneGlow");
            if(owner==null||geometry==null||soft==null){Finish();return;}
            energy=effect==Kind.Absorb?new Color(.24f,.72f,1f,.85f):
                effect==Kind.GroundField?new Color(1f,.38f,.08f,.82f):new Color(1f,.76f,.25f,.88f);
            if(effect==Kind.Conjuration) energy=phase<.5f?new Color(1f,.66f,.18f,.85f):
                phase<1.5f?new Color(.5f,.8f,1f,.85f):new Color(1f,.38f,.08f,.85f);
            if(effect==Kind.Shockwave)energy=new Color(.58f,.82f,1f,.85f);
            faceMaterial=Material(geometry,"Warrior translucent surface");faceMaterial.color=Color.white;
            Shader shieldShader=Resources.Load<Shader>("WarriorEnergy");
            shieldFaceMaterial=Material(shieldShader!=null?shieldShader:geometry,"Warrior blue gold shield face");
            if(shieldShader!=null)
            {
                shieldFaceMaterial.SetFloat("_Mode",1);shieldFaceMaterial.SetFloat("_VertexAlphaGain",3);
                shieldFaceMaterial.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);
            }
            edgeMaterial=Material(geometry,"Warrior energy outlines");edgeMaterial.color=Color.white;
            glowMaterial=Material(soft,"Warrior soft radial glow");glowMaterial.SetFloat("_Shape",0);
            streakMaterial=Material(soft,"Warrior soft energy streaks");streakMaterial.SetFloat("_Shape",1);
            switch(kind)
            {
                case Kind.Shields:
                    for(int i=0;i<3;i++)shields[i]=Shield("Fortress Shield "+(i+1),1f);
                    Ring("Fortress foot glow",.6f,.045f,.035f,.4f);break;
                case Kind.FlyingShield:
                    shields[0]=Shield("Thrown Shield",Mathf.Clamp(radius*1.8f,.7f,1.15f));
                    shields[0].rotation=FacingRotation();Trail(shields[0],.34f,.34f);break;
                case Kind.Charge:
                    shields[0]=Shield("Charge Shield",1.05f);Trail(shields[0],.21f,.42f);
                    Particles("Charge wind",Vector3.up*.4f,0,18,1.1f,.1f,.2f,true,true);break;
                case Kind.Aura:
                    Reference(3);Ring("Guard energy footing",radius,.06f,.045f,.35f);
                    Particles("Guard embers",Vector3.up*.2f,0,5,.25f,.06f,.4f,true,false);break;
                case Kind.Absorb:
                    Reference(4);
                    Particles("Absorption motes",Vector3.up*.65f,0,10,.15f,.075f,.5f,true,false);break;
                case Kind.GroundField:
                    Reference(1);
                    groundDisc=Annulus("Burning ground edge",.84f,.065f,.18f);
                    Ring("Ground outer energy",radius,.07f,.065f,.8f);Ring("Ground inner heat",radius*.65f,.075f,.045f,.3f);
                    Particles("Ground heat plumes",Vector3.up*.08f,0,9,.35f,.38f,.65f,true,false);
                    Particles("Ground rising sparks",Vector3.up*.12f,0,6,.85f,.08f,.6f,true,true);break;
                case Kind.Explosion:
                    Reference(0);
                    groundDisc=Annulus("Expanding shock front",.68f,.075f,.35f);Ring("Blast ground wave",radius,.09f,.13f,.95f);
                    Particles("Explosion core",Vector3.up*.4f,14,0,1.3f,.75f,.55f,false,false);
                    Particles("Explosion energy shards",Vector3.up*.25f,30,0,Mathf.Min(7f,radius*1.8f),.12f,.7f,false,true);
                    Particles("Explosion soft dust",Vector3.up*.1f,12,0,1.25f,.8f,.75f,false,false,true);break;
                case Kind.Shockwave:
                    Reference(2);
                    groundDisc=Annulus("Horizontal energy wave",.86f,.065f,.36f);Ring("Wide shockwave lip",radius,.1f,.085f,.85f);
                    Particles("Shockwave low sparks",Vector3.up*.08f,20,0,Mathf.Min(5f,radius*1.2f),.08f,.4f,false,true);break;
                case Kind.Conjuration:
                    Reference(10+Mathf.Clamp(Mathf.RoundToInt(phase),0,2));
                    groundDisc=Annulus("Gathering ground energy",.88f,.08f,.14f);
                    Ring("Charging spiral A",radius*.7f,.12f,.045f,.6f);Ring("Charging spiral B",radius*.4f,.25f,.035f,.4f);
                    Particles("Conjuration rising sparks",Vector3.up*.15f,0,18,.8f,.09f,.55f,true,true);break;
                case Kind.Impact:
                    Reference(6);
                    Particles("Contact flash",Vector3.zero,7,0,.5f,.3f,.22f,false,false);
                    Particles("Contact splinters",Vector3.zero,14,0,2.7f,.07f,.36f,false,true);break;
                case Kind.Slash:
                    Reference(7+Mathf.Clamp(Mathf.RoundToInt(phase),0,2));break;
                case Kind.DefenseImpact:
                    Reference(13);break;
            }
            if(kind==Kind.Charge)
            {
                AbilityDefinition move=owner.Abilities.GetEquipped(AbilitySlot.Skill2);
                Reference(move!=null&&move.variantIndex==2?9:8);
            }
            UpdateVisuals(0);
        }

        Material Material(Shader shader,string label)
        {
            var material=new Material(shader){name=label,renderQueue=3000};materials.Add(material);return material;
        }

        void Reference(int style)
        {
            reference=new GameObject("Warrior Reference Composition").transform;
            reference.SetParent(visualRoot,false);
            if(kind==Kind.Slash||kind==Kind.Aura||kind==Kind.Charge)reference.rotation=FacingRotation();
            reference.gameObject.AddComponent<WarriorReferenceVfx>().Begin(style,duration,radius);
        }

        Transform Shield(string name,float scale)
        {
            Transform shield=new GameObject(name).transform;shield.SetParent(visualRoot,false);shield.localScale=Vector3.one*scale;
            var surface=new GameObject("Upper-chamfer translucent face",typeof(MeshFilter),typeof(MeshRenderer));
            surface.transform.SetParent(shield,false);
            var vertices=new Vector3[ShieldOutline.Length+1];var colors=new Color[vertices.Length];var uv=new Vector2[vertices.Length];
            Color shieldBlue=new Color(.12f,.65f,1f,1f);
            var triangles=new int[ShieldOutline.Length*3];colors[0]=Alpha(shieldBlue,.12f);uv[0]=Vector2.one*.5f;
            for(int i=0;i<ShieldOutline.Length;i++)
            {
                vertices[i+1]=ShieldOutline[i];colors[i+1]=Alpha(shieldBlue,.24f);
                uv[i+1]=new Vector2(vertices[i+1].x+.5f,vertices[i+1].y/1.4f+.5f);
                triangles[i*3]=0;triangles[i*3+1]=i+1;triangles[i*3+2]=(i+1)%ShieldOutline.Length+1;
            }
            var mesh=new Mesh{name="BRX Warrior chamfered shield",vertices=vertices,colors=colors,uv=uv,triangles=triangles};
            mesh.RecalculateNormals();mesh.RecalculateBounds();meshes.Add(mesh);surface.GetComponent<MeshFilter>().sharedMesh=mesh;
            Renderer(surface.GetComponent<MeshRenderer>(),shieldFaceMaterial);
            var outline=Line("Solid luminous border",shield,.045f,Alpha(energy,.94f));outline.loop=true;outline.positionCount=ShieldOutline.Length;
            var inset=Line("Inner chamfer border",shield,.013f,new Color(1f,.94f,.69f,.7f));inset.loop=true;inset.positionCount=ShieldOutline.Length;
            for(int i=0;i<ShieldOutline.Length;i++)
            {outline.SetPosition(i,ShieldOutline[i]+Vector3.forward*.008f);inset.SetPosition(i,ShieldOutline[i]*.88f+Vector3.forward*.012f);}
            Stroke("Shield spine",shield,new Vector3(0,-.47f,.017f),new Vector3(0,.44f,.017f),.023f,.5f);
            Stroke("Shield cross brace",shield,new Vector3(-.32f,0,.017f),new Vector3(.32f,0,.017f),.018f,.5f);
            var crest=Line("Diamond crest",shield,.035f,Alpha(energy,.85f));crest.loop=true;crest.positionCount=4;
            crest.SetPosition(0,new Vector3(0,.16f,.025f));crest.SetPosition(1,new Vector3(.12f,0,.025f));
            crest.SetPosition(2,new Vector3(0,-.16f,.025f));crest.SetPosition(3,new Vector3(-.12f,0,.025f));return shield;
        }

        Transform Annulus(string name,float innerRadius,float height,float alpha)
        {
            var item=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));item.transform.SetParent(visualRoot,false);
            item.transform.localPosition=Vector3.up*height;
            var vertices=new Vector3[(Segments+1)*3];var colors=new Color[vertices.Length];var triangles=new int[Segments*12];
            for(int i=0;i<=Segments;i++)
            {
                float angle=i*Mathf.PI*2/Segments;Vector3 dir=new Vector3(Mathf.Sin(angle),0,Mathf.Cos(angle));
                vertices[i*3]=dir*innerRadius;vertices[i*3+1]=dir*Mathf.Lerp(innerRadius,1,.55f);vertices[i*3+2]=dir;
                colors[i*3]=Alpha(energy,0);colors[i*3+1]=Alpha(energy,alpha);colors[i*3+2]=Alpha(energy,0);
                if(i==Segments)continue;
                for(int band=0;band<2;band++)
                {
                    int at=i*12+band*6,a=i*3+band,b=(i+1)*3+band;
                    triangles[at]=a;triangles[at+1]=b;triangles[at+2]=a+1;
                    triangles[at+3]=a+1;triangles[at+4]=b;triangles[at+5]=b+1;
                }
            }
            var mesh=new Mesh{name=name,vertices=vertices,colors=colors,triangles=triangles};
            mesh.RecalculateNormals();mesh.RecalculateBounds();meshes.Add(mesh);
            item.GetComponent<MeshFilter>().sharedMesh=mesh;Renderer(item.GetComponent<MeshRenderer>(),faceMaterial);
            AnchorGround(item.transform);return item.transform;
        }

        LineRenderer Line(string name,Transform parent,float width,Color color)
        {
            var item=new GameObject(name,typeof(LineRenderer));item.transform.SetParent(parent,false);var line=item.GetComponent<LineRenderer>();
            line.useWorldSpace=false;line.widthMultiplier=width;line.numCornerVertices=2;line.numCapVertices=2;
            line.startColor=line.endColor=color;line.alignment=LineAlignment.View;Renderer(line,edgeMaterial);return line;
        }
        void Stroke(string name,Transform parent,Vector3 from,Vector3 to,float width,float alpha)
        {var line=Line(name,parent,width,Alpha(energy,alpha));line.positionCount=2;line.SetPosition(0,from);line.SetPosition(1,to);}
        void Ring(string name,float size,float height,float width,float alpha)
        {
            var line=Line(name,visualRoot,width,Alpha(energy,alpha));line.loop=true;line.positionCount=Segments;
            for(int i=0;i<Segments;i++){float a=i*Mathf.PI*2/Segments;line.SetPosition(i,new Vector3(Mathf.Sin(a)*size,height,Mathf.Cos(a)*size));}
            if(kind!=Kind.Absorb)AnchorGround(line.transform);
            animatedLines.Add(line);
        }
        void Trail(Transform target,float lifetime,float width)
        {
            var trail=target.gameObject.AddComponent<TrailRenderer>();trail.time=lifetime;trail.minVertexDistance=.12f;trail.widthMultiplier=width;
            trail.widthCurve=AnimationCurve.Linear(0,1,1,0);trail.colorGradient=Fade(Alpha(energy,.6f));Renderer(trail,streakMaterial);
        }

        void Particles(string name,Vector3 point,int burst,float rate,float speed,float size,float life,bool continuous,bool streak,bool dust=false)
        {
            var item=new GameObject(name,typeof(ParticleSystem));item.transform.SetParent(visualRoot,false);item.transform.localPosition=point;
            var particles=item.GetComponent<ParticleSystem>();particles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            var main=particles.main;main.playOnAwake=false;main.loop=continuous;main.duration=Mathf.Max(.1f,duration);
            main.startLifetime=new ParticleSystem.MinMaxCurve(life*.6f,life);main.startSpeed=new ParticleSystem.MinMaxCurve(speed*.5f,speed);
            main.startSize=new ParticleSystem.MinMaxCurve(size*.65f,size);main.startColor=dust?new Color(.53f,.36f,.2f,.17f):Alpha(energy,.55f);
            main.maxParticles=Mathf.Max(burst,24);main.simulationSpace=ParticleSystemSimulationSpace.World;main.gravityModifier=streak&&!continuous?.35f:0;
            var emission=particles.emission;emission.rateOverTime=rate;if(burst>0)emission.SetBursts(new[]{new ParticleSystem.Burst(0,(short)burst)});
            var shape=particles.shape;shape.shapeType=continuous?ParticleSystemShapeType.Cone:ParticleSystemShapeType.Sphere;
            shape.radius=continuous?Mathf.Min(radius*.8f,3f):.15f;shape.angle=continuous?8f:25f;
            if(continuous)item.transform.localRotation=Quaternion.Euler(-90f,0,0);
            if(kind==Kind.Shockwave)
            {shape.shapeType=ParticleSystemShapeType.Circle;shape.radius=.12f;item.transform.localRotation=Quaternion.Euler(-90f,0,0);}
            if(kind==Kind.GroundField||kind==Kind.Conjuration||kind==Kind.Shockwave||dust)AnchorGround(item.transform);
            var fade=particles.colorOverLifetime;fade.enabled=true;fade.color=Fade(Color.white);
            var shrink=particles.sizeOverLifetime;shrink.enabled=true;shrink.size=new ParticleSystem.MinMaxCurve(1,AnimationCurve.EaseInOut(0,1,1,dust?1.6f:0));
            var renderer=particles.GetComponent<ParticleSystemRenderer>();renderer.renderMode=streak?ParticleSystemRenderMode.Stretch:ParticleSystemRenderMode.Billboard;
            renderer.lengthScale=streak?2f:1f;renderer.velocityScale=streak?.18f:0;Renderer(renderer,streak?streakMaterial:glowMaterial);particles.Play();
        }
        static Gradient Fade(Color color)
        {
            var value=new Gradient();value.SetKeys(new[]{new GradientColorKey(color,0),new GradientColorKey(color,1)},
                new[]{new GradientAlphaKey(0,0),new GradientAlphaKey(color.a,.12f),new GradientAlphaKey(0,1)});return value;
        }
        static Color Alpha(Color color,float alpha){color.a=alpha;return color;}
        static void Renderer(Renderer renderer,Material material)
        {
            renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
            renderer.lightProbeUsage=LightProbeUsage.Off;renderer.reflectionProbeUsage=ReflectionProbeUsage.Off;
        }
        Quaternion FacingRotation()
        {
            Vector3 dir=owner!=null&&owner.Motor!=null?owner.Motor.Facing:transform.forward;
            return Quaternion.LookRotation(dir.sqrMagnitude>.001f?dir:Vector3.forward,Vector3.up);
        }
        void Update()
        {
            if(finished)return;
            if(owner==null||owner.Health==null||owner.Health.IsDead||owner.Definition!=definition||
                (owner.Abilities!=null&&owner.Abilities.ResetVersion!=resetVersion)||Time.time>=started+duration||
                (kind==Kind.Conjuration&&owner.Abilities!=null&&(owner.Abilities.PresentationActionId!=actionVersion||
                    (Time.time>started+.04f&&!owner.Abilities.IsActionBusy))))
            {Finish();return;}
            if(follow)transform.position=owner.transform.position;UpdateVisuals(Mathf.Clamp01((Time.time-started)/duration));
        }
        void UpdateVisuals(float t)
        {
            // The existing arena is flat at world y=0, while actor roots sit at y=1.
            // Preserve actor-height shields/core VFX; floor anchors alone compensate for the host height.
            foreach(var anchor in groundAnchors)
                if(anchor.transform!=null)
                {
                    Vector3 local=anchor.transform.localPosition;
                    local.y=anchor.height-visualRoot.position.y;anchor.transform.localPosition=local;
                }
            float fade=Mathf.Clamp01((1-t)*5),entry=Mathf.SmoothStep(0,1,Mathf.Clamp01(t*8));
            faceMaterial.color=new Color(1,1,1,fade);edgeMaterial.color=new Color(1,1,1,fade);
            shieldFaceMaterial.color=new Color(1,1,1,fade);
            if(reference!=null&&(kind==Kind.Slash||kind==Kind.Aura||kind==Kind.Charge))reference.rotation=FacingRotation();
            if(kind==Kind.Shields)
            {
                for(int i=0;i<3;i++)
                {
                    float angle=(phase+i*120+t*360f)*Mathf.Deg2Rad;
                    Vector3 outwards=new Vector3(Mathf.Sin(angle),0,Mathf.Cos(angle));
                    shields[i].localPosition=outwards*radius+Vector3.up*(1+.045f*Mathf.Sin(angle*3));
                    shields[i].rotation=Quaternion.LookRotation(outwards,Vector3.up);shields[i].localScale=Vector3.one*Mathf.Lerp(.7f,1,entry);
                }
            }
            else if(kind==Kind.FlyingShield)shields[0].localRotation=Quaternion.Inverse(transform.rotation)*FacingRotation()*Quaternion.Euler(0,0,(Time.time-started)*640);
            else if(shields[0]!=null)
            {shields[0].position=owner.transform.position+owner.Motor.Facing*(kind==Kind.Charge?.82f:.73f)+Vector3.up;shields[0].rotation=FacingRotation();}
            if(groundDisc!=null)
            {
                float scale=kind==Kind.Explosion||kind==Kind.Shockwave?Mathf.Lerp(.12f,1.06f,Mathf.Sqrt(t)):
                    kind==Kind.Conjuration?Mathf.Lerp(1,.3f,t):1+.025f*Mathf.Sin((Time.time-started)*5);
                groundDisc.localScale=Vector3.one*radius*scale;
            }
            for(int j=0;j<animatedLines.Count;j++)
            {
                var line=animatedLines[j];
                if(kind==Kind.Explosion||kind==Kind.Shockwave)line.transform.localScale=Vector3.one*Mathf.Lerp(.12f,1.06f,Mathf.Sqrt(t));
                else if(kind==Kind.Absorb||kind==Kind.Conjuration)
                {
                    for(int i=0;i<Segments;i++)
                    {
                        float a=i*Mathf.PI*2/Segments+(Time.time-started)*(j==0?3:-4);
                        float r=radius*(j==0?.8f:.55f)*(kind==Kind.Conjuration?Mathf.Lerp(1,.35f,t):1);
                        float y=kind==Kind.Absorb?.7f+Mathf.Sin(a*2+(Time.time-started)*4)*.26f:.08f+Mathf.Sin(a*3)*.04f;
                        line.SetPosition(i,new Vector3(Mathf.Sin(a)*r,y,Mathf.Cos(a)*r));
                    }
                }
            }
        }
        void AnchorGround(Transform item)
        {
            groundAnchors.Add(new GroundAnchor{transform=item,height=item.localPosition.y});
        }
        void Finish()
        {
            if(finished)return;finished=true;
            if(ownsHost)Destroy(gameObject);else{if(visualRoot!=null)Destroy(visualRoot.gameObject);Destroy(this);}
        }
        void OnDestroy()
        {
            if(visualRoot!=null)Destroy(visualRoot.gameObject);
            foreach(var material in materials)if(material!=null)Destroy(material);
            foreach(var mesh in meshes)if(mesh!=null)Destroy(mesh);
        }
    }
}
