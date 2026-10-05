using UnityEngine;
using UnityEngine.Rendering;

namespace BattleRoyaleX
{
    // Visual blades have no colliders; combat pulses are resolved separately.
    public sealed class AssassinDaggersPresentation : MonoBehaviour
    {
        Transform[] blades;
        float radius;
        Material steel;
        Material trailMaterial;
        Mesh bladeMesh;
        Renderer[] renderers;
        TrailRenderer[] trails;
        bool previousShow=true;
        public void Configure(int count, float orbitRadius)
        {
            if(blades!=null||count<=0)return;
            radius = orbitRadius; blades = new Transform[count];
            Shader shader=Resources.Load<Shader>("BRXAimPreview"), glow=Resources.Load<Shader>("ArcaneGlow");
            if(shader==null||glow==null)return;
            steel=new Material(shader){name="Assassin faceted silver",color=Color.white};
            trailMaterial=new Material(glow){name="Assassin violet blade wake"};trailMaterial.SetFloat("_Shape",1);
            bladeMesh=new Mesh{name="Pointed silver dagger with guard"};
            bladeMesh.vertices=new[]{new Vector3(0,0,-.20f),new Vector3(-.105f,0,.02f),new Vector3(0,0,.62f),new Vector3(.105f,0,.02f),
                new Vector3(0,.05f,.14f),new Vector3(0,-.035f,.14f),
                new Vector3(-.17f,.02f,-.18f),new Vector3(.17f,.02f,-.18f),new Vector3(.14f,.02f,-.24f),new Vector3(-.14f,.02f,-.24f),
                new Vector3(-.025f,.025f,-.23f),new Vector3(.025f,.025f,-.23f),new Vector3(.025f,.025f,-.4f),new Vector3(-.025f,.025f,-.4f),
                new Vector3(-.065f,.02f,-.40f),new Vector3(0,.03f,-.46f),new Vector3(.065f,.02f,-.40f),new Vector3(0,.03f,-.35f)};
            bladeMesh.triangles=new[]{0,1,4,1,2,4,2,3,4,3,0,4,1,0,5,2,1,5,3,2,5,0,3,5,
                6,7,8,6,8,9,10,11,12,10,12,13,14,15,16,14,16,17};
            var colors=new Color[18];
            for(int i=0;i<colors.Length;i++)colors[i]=i<6?new Color(.64f,.68f,.85f,1f):new Color(.2f,.14f,.3f,1f);
            colors[2]=new Color(1.05f,.95f,1.2f,1f);colors[4]=new Color(.94f,.95f,1.05f,1f);colors[5]=new Color(.35f,.3f,.48f,1f);
            colors[6]=colors[7]=colors[14]=colors[16]=new Color(.57f,.28f,.88f,1f);bladeMesh.colors=colors;
            bladeMesh.RecalculateNormals(); bladeMesh.RecalculateBounds();
            renderers=new Renderer[count*2];trails=new TrailRenderer[count];
            var gradient=new Gradient();gradient.SetKeys(new[]{new GradientColorKey(new Color(.94f,.8f,1.1f),0),new GradientColorKey(new Color(.47f,.12f,.85f),1)},
                new[]{new GradientAlphaKey(.8f,0),new GradientAlphaKey(0,1)});
            for (int i = 0; i < count; i++)
            {
                GameObject blade = new GameObject();
                blade.name = "Dagger_" + i; blade.transform.SetParent(transform, false);
                blade.AddComponent<MeshFilter>().sharedMesh = bladeMesh;
                var meshRenderer=blade.AddComponent<MeshRenderer>();meshRenderer.sharedMaterial=steel;
                meshRenderer.shadowCastingMode=ShadowCastingMode.Off;meshRenderer.receiveShadows=false;renderers[i*2]=meshRenderer;
                blades[i] = blade.transform;
                var trail = blade.AddComponent<TrailRenderer>();
                trail.sharedMaterial = trailMaterial; trail.time = 0.12f; trail.startWidth = 0.06f; trail.endWidth = 0f;
                trail.minVertexDistance = 0.05f;
                trail.colorGradient=gradient;trail.shadowCastingMode=ShadowCastingMode.Off;trail.receiveShadows=false;
                renderers[i*2+1]=trail;trails[i]=trail;
            }
        }
        void LateUpdate()
        {
            if (blades == null||renderers==null) return;
            var marker = GetComponent<OwnedAbilityEffect>();
            CharacterRuntime owner=marker!=null?marker.Owner:null;
            if(owner==null){var hit=GetComponent<Hitbox>();if(hit!=null)owner=hit.Owner;}
            bool hidden=owner!=null&&((owner.Abilities!=null&&owner.Abilities.IsExecutionHidden)||SmokeField.Contains(owner.transform.position));
            bool show=!hidden||SmokeVisibility.LocalPlayer==null||SmokeVisibility.LocalPlayer==owner;
            if(show!=previousShow)
            {
                foreach(var renderer in renderers)if(renderer!=null)renderer.enabled=show;
                foreach(var trail in trails)if(trail!=null){trail.Clear();trail.emitting=show;}
                previousShow=show;
            }
            if(steel!=null)steel.color=hidden&&show?new Color(.55f,.5f,.65f,.75f):Color.white;
            if (radius <= 0f) return;
            for (int i = 0; i < blades.Length; i++)
            {
                float angle = Time.time * 9f + i * Mathf.PI * 2f / blades.Length;
                blades[i].localPosition = new Vector3(Mathf.Cos(angle) * radius, 1f, Mathf.Sin(angle) * radius);
                blades[i].localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f);
            }
        }
        void OnDestroy() { if (steel != null) Destroy(steel);if(trailMaterial!=null)Destroy(trailMaterial); if (bladeMesh != null) Destroy(bladeMesh); }
    }
}
