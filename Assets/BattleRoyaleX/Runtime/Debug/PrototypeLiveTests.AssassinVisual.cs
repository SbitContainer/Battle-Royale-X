#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace BattleRoyaleX
{
    public sealed partial class PrototypeLiveTests
    {
        void CaptureAssassinFrame(string label)
        {
            Camera camera = Camera.main;
            if (camera == null) { Check(false, "VISUAL009 câmera disponível"); return; }
            Vector3 position = camera.transform.position; Quaternion rotation = camera.transform.rotation;
            float fov = camera.fieldOfView;
            RenderTexture old = camera.targetTexture, previous = RenderTexture.active;
            var target = new RenderTexture(1280,720,24);
            var image = new Texture2D(1280,720,TextureFormat.RGB24,false);
            try
            {
                camera.transform.position = a.transform.position + new Vector3(5,5,-7);
                camera.transform.LookAt(a.transform.position + Vector3.up*.7f); camera.fieldOfView = 36;
                camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
                image.ReadPixels(new Rect(0,0,1280,720),0,0); image.Apply();
                System.IO.Directory.CreateDirectory("Logs/Assassin009");
                System.IO.File.WriteAllBytes("Logs/Assassin009/"+label+".png",image.EncodeToPNG());
                Debug.Log("[BRX VISUAL009] captured " + label);
            }
            finally
            {
                camera.targetTexture = old; RenderTexture.active = previous;
                camera.transform.SetPositionAndRotation(position,rotation); camera.fieldOfView = fov;
                Destroy(image); target.Release(); Destroy(target);
            }
        }

        IEnumerator TestAssassinVisual009()
        {
            stage = "VISUAL009: referências Assassino, fumaça e furtividade local";
            // Resources assets retain the two runtime Lit keyword combinations in Android builds.
            foreach (bool normal in new[] { false, true })
            {
                Material retained = Resources.Load<Material>(normal ? "StealthLitTransparentNormal" : "StealthLitTransparent");
                Check(retained != null && retained.shader.name == "Universal Render Pipeline/Lit" &&
                    retained.IsKeywordEnabled("_SURFACE_TYPE_TRANSPARENT") &&
                    retained.IsKeywordEnabled("_NORMALMAP") == normal && retained.GetFloat("_Surface") == 1f &&
                    retained.GetFloat("_ZWrite") == 0f && retained.renderQueue == 3000,
                    "RELEASE010 Resources retém Lit transparente" + (normal ? " com normal map" : " sem normal map"));
            }
            ResetPair(3f); SmokeVisibility.LocalPlayer = a;
            var mesh = a.GetComponentInChildren<SkinnedMeshRenderer>();
            Material[] originals = mesh.sharedMaterials;
            ShadowCastingMode originalShadows = mesh.shadowCastingMode;
            var originalBlock = new MaterialPropertyBlock(); mesh.GetPropertyBlock(originalBlock);
            a.Abilities.EquipVariation(Ability("Assassin_Ult_A")); a.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(.6f);
            var ghostBlock = new MaterialPropertyBlock(); mesh.GetPropertyBlock(ghostBlock);
            Check(a.Abilities.IsExecutionHidden && mesh.enabled && ghostBlock.GetColor("_BaseColor").a > .1f &&
                ghostBlock.GetColor("_BaseColor").a < .5f && mesh.sharedMaterials.All(m=>m.GetFloat("_Surface")==1f),
                "VISUAL009 Execução mostra modelo realmente transparente ao proprietário, com shader transparente");
            Check(mesh.sharedMaterials.Zip(originals,(ghost,original)=>ghost!=original &&
                ghost.GetTexture("_BaseMap")==original.GetTexture("_BaseMap")).All(x=>x) &&
                mesh.shadowCastingMode==ShadowCastingMode.Off,
                "VISUAL009 furtividade preserva texturas, não altera material compartilhado e não revela sombra sólida");
            CaptureAssassinFrame("execution-owner-stealth");
            SmokeVisibility.LocalPlayer=w; yield return null; yield return null;
            Check(!mesh.enabled,"VISUAL009 adversário não enxerga o modelo furtivo");
            CaptureAssassinFrame("execution-enemy-hidden");
            yield return new WaitForSeconds(2.2f);
            var restored = new MaterialPropertyBlock(); mesh.GetPropertyBlock(restored);
            Check(mesh.enabled && mesh.sharedMaterials.SequenceEqual(originals) &&
                mesh.shadowCastingMode==originalShadows && restored.GetColor("_BaseColor")==originalBlock.GetColor("_BaseColor"),
                "VISUAL009 fim da Execução restaura materiais, propriedades, sombras e visibilidade exatamente");
            ResetPair(6f); SmokeVisibility.LocalPlayer=a;
            float hp=a.Health.CurrentHealth, energy=a.Energy.CurrentEnergy;
            var smoke=SmokeField.Spawn(a.transform.position,3f,1.5f,true);
            yield return new WaitForSeconds(.2f);
            Check(smoke.GetComponent<ParticleSystemRenderer>().sharedMaterial.shader==Resources.Load<Shader>("AssassinMist") &&
                smoke.GetComponent<ParticleSystem>().main.maxParticles==96 && SmokeField.PreventsAttack(a),
                "VISUAL009 fumaça sombria usa shader orgânico com partículas limitadas e mantém supressão original");
            mesh.GetPropertyBlock(ghostBlock);
            Check(mesh.enabled && ghostBlock.GetColor("_BaseColor").a<.5f,"VISUAL009 próprio modelo segue legível e translúcido na fumaça");
            CaptureAssassinFrame("smoke-owner");
            SmokeVisibility.LocalPlayer=w; yield return null; yield return null;
            Check(!mesh.enabled,"VISUAL009 fumaça continua ocultando modelo para adversário");
            CaptureAssassinFrame("smoke-outside");
            SmokeField.ClearAll(); yield return null; yield return null;
            Check(mesh.enabled && mesh.sharedMaterials.SequenceEqual(originals) && Near(hp,a.Health.CurrentHealth) &&
                a.Energy.CurrentEnergy>=energy,"VISUAL009 limpar fumaça restaura modelo sem dano/custo introduzido pela apresentação");
            SmokeVisibility.LocalPlayer=a;
            for(int style=0;style<=8;style++)
            {
                var root = new GameObject("Visual009_Fixture_"+style); root.transform.position=a.transform.position;
                root.AddComponent<AssassinReferenceVfx>().Begin(style,.35f,1.2f);
                Destroy(root,.35f);
                yield return new WaitForSeconds(.1f);
                Check(root.GetComponentsInChildren<Renderer>().Length<=12 && root.GetComponentsInChildren<Collider>().Length==0 &&
                    root.GetComponentsInChildren<Hitbox>().Length==0,"VISUAL009 estilo "+style+" limitado e sem física/autoridade de dano");
                CaptureAssassinFrame("reference-style-"+style);
                yield return new WaitForSeconds(.4f);
                Check(root==null,"VISUAL009 estilo "+style+" remove efeito ao terminar");
            }
            ResetPair(6f); a.Abilities.EquipVariation(Ability("Assassin_Defense_B"));
            a.Abilities.TryUse(AbilitySlot.Defense); yield return new WaitForSeconds(.25f);
            CaptureAssassinFrame("five-daggers");
            ResetPair(6f); SmokeVisibility.LocalPlayer=null;
        }
    }
}
#endif
