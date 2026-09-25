using System.Collections;
using UnityEngine;

namespace BattleRoyaleX
{
    // Presentation only: damage and displacement never originate in this component.
    public sealed class CombatEventVfxPresenter : MonoBehaviour
    {
        Material glow, ribbon, blood, bloodRibbon;
        const int EffectBudget = 70;
        static readonly Color Violet = new Color(0.65f, 0.19f, 1f);
        static readonly Color Ice = new Color(0.15f, 0.85f, 1f);
        static readonly Color Gold = new Color(1f, 0.58f, 0.12f);
        static readonly Color Crimson = new Color(1f, 0.012f, 0.022f);
        static readonly Color DarkBlood = new Color(0.46f, 0.002f, 0.008f);

        void Awake()
        {
            var shader = Resources.Load<Shader>("ArcaneGlow");
            if (shader == null) { Debug.LogError("BRX: ArcaneGlow shader missing."); enabled = false; return; }
            glow = new Material(shader) { name = "BRX_SoftMagic" };
            ribbon = new Material(shader) { name = "BRX_MagicRibbon" };
            ribbon.SetFloat("_Shape", 1f);
            var bloodShader = Resources.Load<Shader>("BloodSoft");
            if (bloodShader == null) bloodShader = shader;
            blood = new Material(bloodShader) { name = "BRX_BloodDroplet" };
            bloodRibbon = new Material(bloodShader) { name = "BRX_BloodRibbon" };
            bloodRibbon.SetFloat("_Shape", 1f);
        }
        void OnEnable() => CombatEvents.Raised += Present;
        void OnDisable() => CombatEvents.Raised -= Present;
        void OnDestroy()
        {
            if (glow != null) Destroy(glow);
            if (ribbon != null) Destroy(ribbon);
            if (blood != null) Destroy(blood);
            if (bloodRibbon != null) Destroy(bloodRibbon);
        }

        static bool Assassin(CharacterRuntime c) => c != null && c.Definition != null && c.Definition.characterClass == CharacterClass.Assassin;
        static Color Tint(CharacterRuntime c) => Assassin(c) ? Violet : Gold;
        static Color SkillTint(AbilityDefinition ability, CharacterRuntime source)
        {
            if (ability == null) return Tint(source);
            switch (ability.abilityId)
            {
                case "Assassin_Defense_Base": return new Color(0.36f, 0.66f, 1f);
                case "Assassin_Defense_A": return new Color(0.65f, 0.40f, 1f);
                case "Assassin_Defense_B": return new Color(0.24f, 0.95f, 0.82f);
                case "Assassin_Move_A": return new Color(0.95f, 0.25f, 0.72f);
                case "Assassin_Move_B": return new Color(0.32f, 0.64f, 1f);
                case "Assassin_Ult_A": return new Color(1f, 0.20f, 0.44f);
                case "Assassin_Ult_B": return new Color(0.16f, 0.92f, 0.76f);
                case "Warrior_Defense_Base": return new Color(0.48f, 0.76f, 1f);
                case "Warrior_Defense_A": return new Color(0.92f, 0.96f, 1f);
                case "Warrior_Defense_B": return new Color(0.28f, 0.86f, 0.95f);
                case "Warrior_Move_A": return new Color(1f, 0.38f, 0.12f);
                case "Warrior_Move_B": return new Color(0.32f, 0.68f, 1f);
                case "Warrior_Ult_A": return new Color(0.26f, 0.76f, 1f);
                case "Warrior_Ult_B": return new Color(1f, 0.38f, 0.18f);
                default: return Tint(source);
            }
        }
        GameObject Effect(string name, Vector3 position)
        {
            if (glow == null || transform.childCount >= EffectBudget) return null;
            var go = new GameObject(name); go.transform.SetParent(transform); go.transform.position = position; return go;
        }
        void Present(CombatEventData e)
        {
            Color color = SkillTint(e.ability, e.source);
            Vector3 chest = e.source != null ? e.source.transform.position + Vector3.up * 0.35f : e.position;
            switch (e.kind)
            {
                case CombatEventKind.AbilityAttack:
                    if (e.phase != AbilityPhase.Active) break;
                    StartCoroutine(Slash(e.source, Mathf.Clamp(Mathf.RoundToInt(e.value), 1, 3))); break;
                case CombatEventKind.AbilityMove:
                    if (e.phase != AbilityPhase.Active) break;
                    if (e.ability != null && e.ability.slot == AbilitySlot.Defense && e.ability.redirectOnDefense)
                        StartCoroutine(ReactiveCue(e.source, color));
                    StartCoroutine(Dash(e.source, e.ability, color)); break;
                case CombatEventKind.AbilityGuard:
                    if (e.phase != AbilityPhase.Active) break;
                    if (e.ability != null && e.ability.behavior == AbilityBehavior.SmokeEscape)
                    { Ring(e.position, color, e.ability.smokeRadius, 0.45f); Burst(chest, color, 12, 1.5f, 0.3f); break; }
                    StartCoroutine(GuardAura(e.source, e.ability, color)); break;
                case CombatEventKind.AbilityUltimate:
                    bool charged = e.ability != null && e.ability.behavior == AbilityBehavior.ChargedDashSequence;
                    if (e.phase != (charged ? AbilityPhase.Startup : AbilityPhase.Active)) break;
                    Ring(e.position, color, e.ability != null ? e.ability.explosionRadius : 1.8f, 0.4f);
                    Burst(chest, color, 28, 2.5f, 0.42f);
                    StartCoroutine(Aura(e.source, Mathf.Max(0.5f, e.value), e.ability, color)); break;
                case CombatEventKind.Clash:
                    Burst(e.position, Tint(e.source), 18, 4f, 0.32f);
                    Burst(e.position, Tint(e.target), 18, 3.5f, 0.27f);
                    Ring(e.position, Color.white, 1.7f, 0.35f); break;
                case CombatEventKind.Parry:
                    Burst(e.position, Ice, 24, 4.5f, 0.35f);
                    Ring(e.position, Gold, 1.8f, 0.4f);
                    StartCoroutine(Shield(e.source, 0.25f)); break;
                case CombatEventKind.CounterReady:
                    StartCoroutine(CounterCue(e.source, e.value)); break;
                case CombatEventKind.CounterHit:
                    Burst(e.position, Gold, 22, 4f, 0.24f);
                    StartCoroutine(ImpactFan(e.position, e.direction, Gold)); break;
                case CombatEventKind.DefenseRedirect:
                    Ring(e.position, color, 0.8f, 0.20f);
                    Burst(e.position + Vector3.up * 0.3f, color, 10, 1.7f, 0.16f);
                    StartCoroutine(ImpactFan(e.position, e.direction, color));
                    if (e.ability != null)
                        StartCoroutine(Dash(e.source, e.ability, color));
                    break;
                case CombatEventKind.Block:
                    Burst(e.position, Ice, 15, 3f, 0.26f);
                    if (e.value > 0.01f) BloodImpact(e.position, e.direction, true);
                    StartCoroutine(Shield(e.target, 0.22f)); break;
                case CombatEventKind.Hit:
                    Burst(e.position, color, 8, 1.8f, 0.16f);
                    BloodImpact(e.position, e.direction, false); break;
                case CombatEventKind.Death:
                    StartCoroutine(DeathSequence(e.target, e.position, e.direction)); break;
                case CombatEventKind.Dodge:
                    Burst(e.position, Violet, 8, 1.6f, 0.3f); break;
                case CombatEventKind.Heal:
                    Burst(chest, new Color(0.2f,1f,0.65f), 15, 1.2f, 0.36f); break;
                case CombatEventKind.Energy:
                case CombatEventKind.Nullify:
                case CombatEventKind.Reflect:
                case CombatEventKind.VariationSwap:
                case CombatEventKind.TacticalUsed:
                    Ring(e.position, Ice, 1.3f, 0.45f); Burst(chest, Ice, 12, 2f, 0.28f); break;
                case CombatEventKind.ItemPickup:
                    Color pickupColor = WorldPickup.ColorFor(e.item);
                    Ring(e.position, pickupColor, 0.9f, 0.32f);
                    Burst(chest, pickupColor, 9, 1.4f, 0.22f); break;
            }
        }
        void Burst(Vector3 position, Color color, int count, float speed, float size)
        {
            var go = Effect("VFX_CombatBurst", position); if (go == null) return;
            var ps = go.AddComponent<ParticleSystem>(); ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main; main.playOnAwake = false; main.loop = false; main.duration = 0.1f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.22f,0.48f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.3f, speed);
            main.startSize = new ParticleSystem.MinMaxCurve(size * 0.6f, size * 1.5f);
            main.startColor = color; main.maxParticles = 40; main.simulationSpace = ParticleSystemSimulationSpace.World;
            var em = ps.emission; em.rateOverTime = 0; em.SetBursts(new[] { new ParticleSystem.Burst(0,(short)count) });
            var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = 0.08f;
            var fade = ps.colorOverLifetime; fade.enabled = true; fade.color = Fade(Color.white);
            var shrink = ps.sizeOverLifetime; shrink.enabled = true;
            shrink.size = new ParticleSystem.MinMaxCurve(1, AnimationCurve.EaseInOut(0,1,1,0));
            ps.GetComponent<ParticleSystemRenderer>().sharedMaterial = glow;
            ps.Play(); Destroy(go,0.7f);
        }

        void BloodImpact(Vector3 position, Vector3 direction, bool blocked)
        {
            BloodSpray("VFX_BloodImpact", position, direction, blocked ? 5 : 24,
                blocked ? 1.5f : 4.6f, blocked ? 0.055f : 0.14f, blocked ? 0.35f : 0.78f);
        }

        void BloodSpray(string name, Vector3 position, Vector3 direction, int count, float speed, float size, float gravity)
        {
            var go = Effect(name, position + Vector3.up * 0.18f); if (go == null) return;
            direction.y = Mathf.Max(0.12f, direction.y + 0.18f);
            go.transform.rotation = Quaternion.LookRotation(direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward);
            var ps = go.AddComponent<ParticleSystem>(); ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main; main.playOnAwake = false; main.loop = false; main.duration = 0.08f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.32f, 0.72f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.40f, speed);
            main.startSize = new ParticleSystem.MinMaxCurve(size * 0.45f, size * 1.35f);
            main.startColor = new ParticleSystem.MinMaxGradient(Crimson, DarkBlood);
            main.gravityModifier = gravity; main.maxParticles = 48; main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = ps.emission; emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });
            var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 34f; shape.radius = 0.045f; shape.length = 0.10f;
            var fade = ps.colorOverLifetime; fade.enabled = true; fade.color = Fade(Color.white);
            var shrink = ps.sizeOverLifetime; shrink.enabled = true;
            shrink.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0.18f));
            var renderer = ps.GetComponent<ParticleSystemRenderer>(); renderer.sharedMaterial = blood;
            renderer.renderMode = ParticleSystemRenderMode.Stretch; renderer.lengthScale = 0.55f; renderer.velocityScale = 0.10f;
            ps.Play(); Destroy(go, 1.05f);
        }

        IEnumerator DeathSequence(CharacterRuntime target, Vector3 position, Vector3 direction)
        {
            if (target != null) position = target.transform.position;
            BloodSpray("VFX_DeathBurst", position + Vector3.up * 0.35f, direction, 46, 6.0f, 0.18f, 0.95f);
            BloodSpray("VFX_DeathMist", position + Vector3.up * 0.75f, -direction + Vector3.up * 0.2f, 28, 2.5f, 0.25f, 0.28f);

            Vector3 ground = position; ground.y = 0.055f;
            var go = Effect("VFX_BloodPool", ground); if (go == null) yield break;
            var line = go.AddComponent<LineRenderer>(); line.sharedMaterial = bloodRibbon; line.useWorldSpace = true;
            line.positionCount = 65; line.widthMultiplier = 0.32f; line.numCornerVertices = 4; line.numCapVertices = 4;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; line.receiveShadows = false;
            Color pool = new Color(0.62f, 0.003f, 0.008f, 0.92f);
            for (float t = 0f; t < 1.8f && go != null; t += Time.deltaTime)
            {
                float f = Mathf.Clamp01(t / 1.8f);
                float radius = Mathf.Lerp(0.12f, 1.15f, 1f - Mathf.Pow(1f - f, 3f));
                for (int i = 0; i < 65; i++)
                {
                    float angle = i * Mathf.PI * 2f / 64f;
                    float irregular = 1f + Mathf.Sin(angle * 5f + position.x) * 0.08f + Mathf.Sin(angle * 9f + position.z) * 0.04f;
                    line.SetPosition(i, ground + new Vector3(Mathf.Cos(angle) * radius * irregular, 0f,
                        Mathf.Sin(angle) * radius * irregular));
                }
                pool.a = Mathf.Lerp(0.92f, 0f, Mathf.Clamp01((f - 0.55f) / 0.45f));
                line.startColor = line.endColor = pool;
                yield return null;
            }
            if (go != null) Destroy(go);
        }
        static Gradient Fade(Color c)
        {
            var g = new Gradient(); g.SetKeys(new[] { new GradientColorKey(c,0),new GradientColorKey(c,1) },
                new[] { new GradientAlphaKey(1,0),new GradientAlphaKey(0.8f,0.3f),new GradientAlphaKey(0,1) }); return g;
        }
        LineRenderer Line(GameObject go, Color c, float width, int count)
        {
            var line = go.AddComponent<LineRenderer>(); line.sharedMaterial = ribbon; line.useWorldSpace = true;
            line.positionCount = count; line.widthMultiplier = width; line.startColor = line.endColor = c;
            line.numCornerVertices = 3; line.numCapVertices = 3;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false; return line;
        }
        void Ring(Vector3 center, Color c, float radius, float duration) => StartCoroutine(RingRoutine(center,c,radius,duration));
        IEnumerator RingRoutine(Vector3 center, Color color, float radius, float duration)
        {
            center.y = 0.07f;
            var go = Effect("VFX_ArcaneRing", center); if (go == null) yield break;
            var line = Line(go,color,0.16f,65);
            for(float t=0; t<duration && go!=null; t+=Time.deltaTime)
            {
                float f=t/duration; float r=Mathf.Lerp(radius*0.3f,radius,1-Mathf.Pow(1-f,3));
                for(int i=0;i<65;i++) { float a=i*Mathf.PI*2/64; line.SetPosition(i,center+new Vector3(Mathf.Cos(a)*r,0,Mathf.Sin(a)*r)); }
                color.a=1-f; line.startColor=line.endColor=color; yield return null;
            }
            if(go!=null) Destroy(go);
        }
        IEnumerator Slash(CharacterRuntime source, int step)
        {
            if(source==null) yield break;
            // The Active event already marks contact. Never replay startup in presentation.
            if(source==null || source.Health.IsDead || source.State.IsStaggered) yield break;
            Color c=Tint(source); bool assassin=Assassin(source);
            var go=Effect("VFX_CrescentSlash",source.transform.position); if(go==null)yield break;
            var line=Line(go,c,step==3?0.4f:0.26f,25);
            line.widthCurve=new AnimationCurve(new Keyframe(0,0),new Keyframe(0.25f,1),new Keyframe(0.7f,0.7f),new Keyframe(1,0));
            Vector3 center=source.transform.position+Vector3.up*0.35f;
            Quaternion facing=Quaternion.LookRotation(source.Motor.Facing);
            float radius=assassin?1.25f:1.6f;
            for(float t=0;t<0.24f && go!=null;t+=Time.deltaTime)
            {
                float f=t/0.24f;
                for(int i=0;i<25;i++)
                {
                    float a=Mathf.Lerp(-1.3f,1.3f,i/24f)+(step%2==0?-1:1)*f*0.8f;
                    line.SetPosition(i,center+facing*new Vector3(Mathf.Sin(a)*radius,(i/24f-0.5f)*0.3f,Mathf.Cos(a)*radius));
                }
                c.a=1-f; line.startColor=line.endColor=c; yield return null;
            }
            if(go!=null)Destroy(go);
        }
        IEnumerator Dash(CharacterRuntime source, AbilityDefinition ability, Color color)
        {
            if(source==null)yield break;
            bool defensive = ability != null && ability.slot == AbilitySlot.Defense;
            if(!Assassin(source))
            {
                Burst(source.transform.position + source.Motor.Facing * 0.7f + Vector3.up * 0.25f, color, 12, 2.4f, 0.24f);
                Ring(source.transform.position, color, 0.65f, 0.18f);
                yield return null;
                while (source != null && source.Motor.IsDashing) yield return null;
                if (source != null && !source.Health.IsDead)
                    StartCoroutine(ImpactFan(source.transform.position, source.Motor.Facing, color));
                yield break;
            }
            var go=Effect("VFX_PhantomRibbon",source.transform.position+Vector3.up*0.3f); if(go==null)yield break;
            var trail=go.AddComponent<TrailRenderer>(); trail.sharedMaterial=ribbon;
            trail.time=defensive ? 0.10f : 0.16f; trail.minVertexDistance=0.04f; trail.widthMultiplier=defensive ? 0.22f : 0.46f;
            trail.widthCurve=AnimationCurve.EaseInOut(0f,1f,1f,0f);
            trail.colorGradient=Fade(color); trail.numCapVertices=4;
            Vector3 origin = source.transform.position;
            yield return null;
            while(source!=null && !source.Health.IsDead && source.Motor.IsDashing)
            { go.transform.position=source.transform.position+Vector3.up*0.3f; yield return null; }
            if (source != null && !source.Health.IsDead)
            {
                Ring(source.transform.position, color, defensive ? 0.5f : 0.8f, 0.18f);
                if (ability != null && ability.behavior == AbilityBehavior.DashReturn)
                    Ring(origin, color, 0.65f, ability.returnWindow);
            }
            trail.emitting=false; Destroy(go,0.3f);
        }
        IEnumerator ReactiveCue(CharacterRuntime source, Color color)
        {
            yield return null;
            if (source == null) yield break;
            var go = Effect("VFX_ReactiveWindow", source.transform.position); if (go == null) yield break;
            var line = Line(go, color, 0.075f, 33);
            while (source != null && !source.Health.IsDead && source.Abilities.ReactiveDefenseRemaining > 0f)
            {
                Vector3 center = source.transform.position; center.y = 0.1f;
                float span = Mathf.Clamp01(source.Abilities.ReactiveDefenseRemaining) * Mathf.PI * 2f;
                for (int i = 0; i < 33; i++)
                {
                    float angle = i * span / 32f;
                    line.SetPosition(i, center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 0.8f);
                }
                yield return null;
            }
            if (go != null) Destroy(go);
        }

        IEnumerator Shield(CharacterRuntime source, float duration)
        {
            if(source==null)yield break;
            var go=Effect("VFX_Aegis",source.transform.position);if(go==null)yield break;
            Color c=Ice;var line=Line(go,c,0.15f,33);
            for(float t=0;t<duration && source!=null;t+=Time.deltaTime)
            {
                Quaternion q=Quaternion.LookRotation(source.Motor.Facing);
                Vector3 center=source.transform.position+source.Motor.Facing*0.65f+Vector3.up*0.3f;
                for(int i=0;i<33;i++) {float a=i*Mathf.PI*2/32;line.SetPosition(i,center+q*new Vector3(Mathf.Cos(a)*0.62f,Mathf.Sin(a)*0.75f,0));}
                c.a=1-t/duration;line.startColor=line.endColor=c;yield return null;
            }
            if(go!=null)Destroy(go);
        }
        IEnumerator Aura(CharacterRuntime source,float duration,AbilityDefinition ability,Color color)
        {
            if(source==null)yield break;
            int version = source.Abilities.ResetVersion;
            var go=Effect("VFX_UltimateRune",source.transform.position);if(go==null)yield break;
            var line=Line(go,color,0.09f,73);
            int lobes = ability != null && ability.abilityId.EndsWith("_A") ? 3 : ability != null && ability.abilityId.EndsWith("_B") ? 8 : 6;
            for(float t=0;t<duration && source!=null && !source.Health.IsDead;t+=Time.deltaTime)
            {
                var active=source.Abilities!=null?source.Abilities.GetEquipped(AbilitySlot.Ultimate):null;
                if(active!=null && active.behavior==AbilityBehavior.ChargedDashSequence && !source.Abilities.IsChargedSequenceActive) break;
                Vector3 center=source.transform.position;center.y=0.1f;
                if (active != ability || source.Abilities.ResetVersion != version) break;
                for(int i=0;i<73;i++) {float angle=i*Mathf.PI*2/72;float a=angle+t;float r=0.96f+0.10f*Mathf.Cos(angle*lobes);line.SetPosition(i,center+new Vector3(Mathf.Cos(a)*r,0,Mathf.Sin(a)*r));}
                color.a=Mathf.Min(1,(duration-t)*3)*0.8f;line.startColor=line.endColor=color;yield return null;
            }
            if(go!=null)Destroy(go);
        }

        IEnumerator GuardAura(CharacterRuntime source, AbilityDefinition ability, Color color)
        {
            if (source == null || ability == null) yield break;
            var go = Effect("VFX_Defense_" + ability.abilityId, source.transform.position);
            if (go == null) yield break;
            var line = Line(go, color, ability.behavior == AbilityBehavior.Guard ? 0.14f : 0.085f, 49);
            var shieldObject = Effect("VFX_DefensePanels", source.transform.position);
            LineRenderer shield = shieldObject != null ? Line(shieldObject, color, 0.065f, 25) : null;
            yield return null;
            while (source != null && !source.Health.IsDead && source.Defense.IsActive && source.Defense.ActiveAbility == ability)
            {
                Vector3 center = source.transform.position; center.y = 0.09f;
                float radius = ability.behavior == AbilityBehavior.Guard ? 1.02f : 0.78f;
                for (int i = 0; i < 49; i++)
                {
                    float angle = i * Mathf.PI * 2f / 48;
                    line.SetPosition(i, center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
                }
                if (shield != null)
                {
                    Quaternion facing = Quaternion.LookRotation(source.Motor.Facing);
                    Vector3 chest = source.transform.position + Vector3.up * 0.1f + source.Motor.Facing * 0.65f;
                    for (int i = 0; i < 25; i++)
                    {
                        float angle = i * Mathf.PI * 2f / 24;
                        float r = 0.62f + (ability.behavior == AbilityBehavior.Guard ? 0.07f * Mathf.Cos(angle * 6f) : 0f);
                        shield.SetPosition(i, chest + facing * new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r * 1.25f, 0f));
                    }
                }
                color.a = Mathf.Min(0.65f, source.Defense.Remaining * 5f);
                line.startColor = line.endColor = color;
                if (shield != null) shield.startColor = shield.endColor = color;
                yield return null;
            }
            if (go != null) Destroy(go);
            if (shieldObject != null) Destroy(shieldObject);
        }

        IEnumerator CounterCue(CharacterRuntime source, float duration)
        {
            var go = Effect("VFX_CounterReady", source.transform.position);
            if (go == null) yield break;
            var line = Line(go, Gold, 0.10f, 4);
            yield return null;
            for (float t = 0; t < duration && source != null && source.Abilities.HasCounterOpportunity; t += Time.deltaTime)
            {
                Vector3 p = source.transform.position + Vector3.up * 1.55f;
                line.SetPosition(0,p + Vector3.left * 0.28f);
                line.SetPosition(1,p + Vector3.up * 0.30f);
                line.SetPosition(2,p + Vector3.right * 0.28f);
                line.SetPosition(3,p + Vector3.left * 0.28f);
                yield return null;
            }
            if (go != null) Destroy(go);
        }

        IEnumerator ImpactFan(Vector3 position, Vector3 direction, Color color)
        {
            var go = Effect("VFX_DefensiveResponse", position);
            if (go == null) yield break;
            var line = Line(go, color, 0.20f, 25);
            direction.y = 0f;
            Quaternion facing = Quaternion.LookRotation(direction.sqrMagnitude > 0.01f ? direction : Vector3.forward);
            position.y = 0.22f;
            for (float t = 0; t < 0.28f; t += Time.deltaTime)
            {
                float f = t / 0.28f;
                for (int i = 0; i < 25; i++)
                {
                    float angle = Mathf.Lerp(-1.05f, 1.05f, i / 24f);
                    line.SetPosition(i, position + facing * new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * (0.4f + f * 1.65f));
                }
                color.a = 1f - f; line.startColor = line.endColor = color;
                yield return null;
            }
            if (go != null) Destroy(go);
        }
    }
}
