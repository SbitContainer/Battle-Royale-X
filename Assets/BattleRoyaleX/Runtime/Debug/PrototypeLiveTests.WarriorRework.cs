#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed partial class PrototypeLiveTests
    {
        // Framework parry is still covered explicitly; this is not the redesigned Usurpador asset.
        AbilityDefinition GenericParryFixture()
        {
            var fixture = Instantiate(Ability("Warrior_Defense_Base")); temporary.Add(fixture);
            fixture.abilityId="Fixture_Generic_Parry"; fixture.behavior=AbilityBehavior.Parry;
            fixture.defenseKind=DefenseKind.Parry; fixture.defenseDuration=.7f;
            fixture.perfectWindow=.12f; fixture.damageReduction=.5f;
            fixture.counterBonusDamage=6f; fixture.counterKnockback=5f; fixture.counterWindow=1f;
            fixture.counterOnBlock=false; return fixture;
        }

        void ProbeIncoming(CharacterRuntime source, CharacterRuntime target, AbilityDefinition ability)
        {
            Hitbox probe=SpawnHit(source,ability,target.transform.position+Vector3.up);
            probe.GetComponent<Collider>().enabled=false;
            probe.TryResolveHurtbox(target.GetComponentInChildren<Hurtbox>());
            probe.Cancel(); Destroy(probe.gameObject);
        }

        void CaptureWarriorFrame(string label)
        {
            Camera camera=Camera.main;
            if(camera==null){Debug.LogWarning("[BRX VISUAL006] capture unavailable: no camera");return;}
            Vector3 position=camera.transform.position;Quaternion rotation=camera.transform.rotation;
            float fov=camera.fieldOfView;RenderTexture old=camera.targetTexture, previous=RenderTexture.active;
            RenderTexture target=new RenderTexture(1280,720,24);
            Texture2D image=new Texture2D(1280,720,TextureFormat.RGB24,false);
            try
            {
                camera.transform.position=w.transform.position+new Vector3(5,7,-7);
                camera.transform.LookAt(w.transform.position+Vector3.up*.5f);camera.fieldOfView=40;
                camera.targetTexture=target;camera.Render();RenderTexture.active=target;
                image.ReadPixels(new Rect(0,0,1280,720),0,0);image.Apply();
                System.IO.Directory.CreateDirectory("Logs/Warrior007");
                System.IO.File.WriteAllBytes("Logs/Warrior007/"+label+".png",image.EncodeToPNG());
                Debug.Log("[BRX VISUAL007] captured "+label);
            }
            finally
            {
                camera.targetTexture=old;RenderTexture.active=previous;
                camera.transform.SetPositionAndRotation(position,rotation);camera.fieldOfView=fov;
                Destroy(image);target.Release();Destroy(target);
            }
        }

        IEnumerator TestWarriorRework()
        {
            stage="COMBAT-004: básicos, proteção, captura e ultimates";
            ResetPair(1.4f); events.Clear();
            Vector3 enemyStart=a.transform.position;
            Check(w.Abilities.TryUse(AbilitySlot.BasicAttack) && !w.Abilities.TryUse(AbilitySlot.BasicAttack),
                "Básico inicia e rejeita repetição no mesmo instante");
            yield return new WaitForSeconds(.7f);
            Check(Near(w.Abilities.GetCooldownRemaining(AbilitySlot.BasicAttack),0f) &&
                Vector3.Distance(enemyStart,a.transform.position)<.03f,
                "Básico do Guerreiro não exibe cooldown nem empurra alvo");

            foreach(string id in new[]{"Warrior_Basic","Assassin_Basic","Mage_Basic","Archer_Basic"})
            {
                AbilityDefinition basic=Ability(id);
                Check(basic!=null && Near(basic.cooldown,0f) && basic.startup+basic.activeTime+basic.recovery>0f,
                    id+": cadência positiva por fases, sem cooldown de dados");
            }

            ResetPair(20f); events.Clear();
            bool started=w.Abilities.TryUse(AbilitySlot.BasicAttack);
            float deadline=Time.time+1.25f;
            while(Time.time<deadline){w.Abilities.TryUse(AbilitySlot.BasicAttack);yield return null;}
            w.Abilities.ReleaseBasicAttackInput();
            int steps=events.Count(e=>e.source==w && e.kind==CombatEventKind.AbilityAttack && e.phase==AbilityPhase.Active);
            yield return new WaitForSeconds(.9f);
            Check(started && steps>=2 && !w.Abilities.IsComboInProgress && !w.Abilities.IsActionBusy,
                "Segurar básico encadeia golpes; soltar termina sem ação presa");

            ResetPair(20f); events.Clear(); w.Abilities.TryUse(AbilitySlot.Skill1);
            yield return new WaitForSeconds(.12f);
            ProbeIncoming(a,w,a.Definition.basicAttack);
            CombatResolver.ResolveAreaEffect(w.transform.position,1f,23f,a,Ability("Mage_S1_A"));
            w.Abilities.TryUse(AbilitySlot.BasicAttack);
            yield return new WaitForSeconds(.7f);
            ProbeIncoming(a,w,a.Definition.basicAttack);
            Check(Near(w.Health.CurrentHealth,w.Health.MaxHealth) && !w.Abilities.HasCounterOpportunity,
                "Guarda anula básico/área e permanece durante ataque próprio, sem counter");
            yield return new WaitForSeconds(1.4f);
            ProbeIncoming(a,w,a.Definition.basicAttack);
            Check(Near(w.Health.MaxHealth-w.Health.CurrentHealth,a.Definition.basicAttack.damage),
                "Guarda termina após dois segundos e volta a receber dano");

            ResetPair(20f); events.Clear();
            // Generated phase wall at x=-12 is outside the skill test, but would cancel a shield launched from x=-10.
            w.Motor.Teleport(new Vector3(0f,1f,0f));a.Motor.Teleport(new Vector3(20f,1f,0f));Physics.SyncTransforms();
            w.Abilities.EquipLabVariation(Ability("Warrior_Defense_B"));
            int fortressVersion=w.Abilities.ResetVersion;
            float fortressStarted=Time.time;
            bool fortressAccepted=w.Abilities.TryUse(AbilitySlot.Skill1);
            yield return new WaitForSeconds(.12f);
            AbilityDefinition unavoidable=Instantiate(Ability("Mage_Ult_A")); temporary.Add(unavoidable);
            CaptureWarriorFrame("fortress");
            WarriorSkillPresentation fortressVisual=FindObjectsByType<WarriorSkillPresentation>().FirstOrDefault(v=>v.name=="Warrior_Shields");
            MeshFilter[] shieldFaces=fortressVisual!=null?fortressVisual.GetComponentsInChildren<MeshFilter>():new MeshFilter[0];
            Check(shieldFaces.Length==3 && shieldFaces.All(f=>f.sharedMesh!=null && f.sharedMesh.vertexCount==7 &&
                f.sharedMesh.colors.All(c=>c.a<=.25f)) && fortressVisual.GetComponentsInChildren<Collider>().Length==0,
                "Fortaleza: três faces chanfradas translúcidas sem colisão visual");
            unavoidable.blockable=unavoidable.parryable=unavoidable.reflectable=unavoidable.nullifiable=false;
            ProbeIncoming(a,w,unavoidable);
            CombatResolver.ResolveAreaEffect(w.transform.position,1f,34f,a,unavoidable);
            w.Health.ApplyDamage(7f);
            Check(Near(w.Health.CurrentHealth,w.Health.MaxHealth),"Fortaleza anula dano direto, projétil e área mesmo sem flags bloqueáveis");
            yield return new WaitForSeconds(1.5f);
            Hitbox[] shields=FindObjectsByType<Hitbox>().Where(h=>h.Owner==w && h.Packet.ability==Ability("Warrior_Defense_B") && !h.Cancelled).ToArray();
            Debug.Log($"[BRX FORT004 PROBE] accepted={fortressAccepted} elapsed={Time.time-fortressStarted} reset={fortressVersion}/{w.Abilities.ResetVersion} gen={typeof(AbilityController).GetField("warriorGeneration",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(w.Abilities)} live={shields.Length} hp={w.Health.CurrentHealth} center={w.transform.position} all="+
                string.Join(";",FindObjectsByType<Hitbox>().Select(h=>$"{h.name}:owner={h.Owner?.name}:ability={h.Packet.ability?.abilityId}:cancel={h.Cancelled}:pos={h.transform.position}")));
            Check(shields.Length==3,"Fortaleza dispara exatamente três escudos ao fim da proteção");
            if(shields.Length==3)
            {
                float angle=Vector3.Angle(shields[0].Packet.direction,shields[1].Packet.direction);
                Check(Near(angle,120f,.15f),"Escudos da Fortaleza saem separados por 120 graus");
            }
            else Check(false,"Escudos da Fortaleza saem separados por 120 graus");
            ProbeIncoming(a,w,a.Definition.basicAttack);
            Check(w.Health.CurrentHealth<w.Health.MaxHealth,"Fortaleza não mantém imunidade depois de 1,5 segundos");

            ResetPair(20f);w.Abilities.EquipLabVariation(Ability("Warrior_Defense_B"));w.Abilities.TryUse(AbilitySlot.Skill1);
            yield return new WaitForSeconds(.12f);
            bool fortressBeforeSwap=w.Abilities.HasFortressDamageImmunity;
            w.Abilities.EquipVariation(Ability("Warrior_Defense_Base"));yield return null;
            Check(fortressBeforeSwap && !w.Abilities.HasFortressDamageImmunity,"Trocar variante encerra imunidade da Fortaleza anterior");

            ResetPair(20f); events.Clear();
            w.Abilities.EquipLabVariation(Ability("Warrior_Defense_A"));w.Abilities.TryUse(AbilitySlot.Skill1);
            yield return new WaitForSeconds(.12f);ProbeIncoming(a,w,a.Definition.basicAttack);
            Check(!w.Abilities.HasCapturedSkill && w.Health.CurrentHealth<w.Health.MaxHealth,
                "Usurpador não absorve nem bloqueia ataque básico");

            ResetPair(4f); events.Clear();
            AbilityDefinition original=Ability("Mage_S1_C");
            w.Abilities.EquipLabVariation(Ability("Warrior_Defense_A"));w.Abilities.TryUse(AbilitySlot.Skill1);
            yield return new WaitForSeconds(.12f);
            CombatResolver.ResolveAreaEffect(w.transform.position,1f,17f,a,original);
            Check(w.Abilities.HasCapturedSkill && w.Abilities.CapturedSkillId==original.abilityId && Near(w.Health.CurrentHealth,w.Health.MaxHealth),
                "Usurpador absorve skill de área e identifica a origem");
            yield return new WaitForSeconds(.2f);
            float energyBefore=w.Energy.CurrentEnergy;
            AbilityDefinition snapshot=w.Abilities.GetAimDefinition(AbilitySlot.Skill1);
            bool basicBeforeReplay=w.Abilities.TryUse(AbilitySlot.BasicAttack);
            bool recast=w.Abilities.TryUseAimed(AbilitySlot.Skill1,Vector3.right,a.transform.position);
            Check(basicBeforeReplay && recast && !w.Abilities.IsComboInProgress && Near(energyBefore,w.Energy.CurrentEnergy) && snapshot!=original && snapshot.behavior==original.behavior,
                "Recast interrompe básico, usa snapshot original e não cobra segunda energia");
            yield return new WaitForSeconds(original.startup+.15f);
            Check(FindObjectsByType<SlowField>().Any(f=>f.GetComponent<OwnedAbilityEffect>()!=null &&
                f.GetComponent<OwnedAbilityEffect>().Owner==w && f.GetComponent<OwnedAbilityEffect>().Ability==snapshot),
                "Skill capturada de campo continua sendo campo, não projétil genérico");
            w.Abilities.ResetTransientState();yield return null;
            Check(!w.Abilities.HasCapturedSkill && !FindObjectsByType<SlowField>().Any(f=>
                f.GetComponent<OwnedAbilityEffect>()!=null && f.GetComponent<OwnedAbilityEffect>().Owner==w),
                "Reset elimina snapshot e efeitos capturados do proprietário");

            ResetPair(20f);w.Abilities.EquipLabVariation(Ability("Warrior_Defense_A"));w.Abilities.TryUse(AbilitySlot.Skill1);
            yield return new WaitForSeconds(.12f);ProbeIncoming(a,w,Ability("Mage_S1_A"));
            yield return new WaitForSeconds(2.12f);
            Check(!w.Abilities.HasCapturedSkill && !w.Abilities.TryUse(AbilitySlot.Skill1),
                "Captura expira em dois segundos e respeita cooldown original");

            ResetPair(3f);events.Clear();w.Abilities.EquipLabVariation(Ability("Warrior_Move_A"));
            bool stationaryPursuit=w.Abilities.TryUse(AbilitySlot.Skill2,Vector3.right);
            yield return new WaitForSeconds(1.1f);
            Check(stationaryPursuit && Near(a.Health.MaxHealth-a.Health.CurrentHealth,
                Ability("Warrior_Move_A").damage+Ability("Warrior_Move_A").secondStrikeDamage,.08f) && Count(CombatEventKind.Hit,a)==2,
                "Perseguição curta contra alvo parado aplica contato de 18 e golpe extra de 6 uma vez cada");

            ResetPair(3f);w.Abilities.EquipLabVariation(Ability("Warrior_Move_A"));
            bool pursuitStarted=w.Abilities.TryUse(AbilitySlot.Skill2,Vector3.right);
            float movementDeadline=Time.time+1.4f;
            while(!w.Motor.IsDashing && Time.time<movementDeadline)yield return null;
            while(w.Motor.IsDashing && Time.time<movementDeadline)yield return null;
            a.Motor.Teleport(a.transform.position+Vector3.forward*6f);Physics.SyncTransforms();
            yield return new WaitForSeconds(.4f);
            Check(pursuitStarted && Near(a.Health.MaxHealth-a.Health.CurrentHealth,Ability("Warrior_Move_A").damage,.08f),
                "Fugir no intervalo do segundo golpe impede dano remoto da perseguição curta");

            foreach(string id in new[]{"Warrior_Ult_Base","Warrior_Ult_A","Warrior_Ult_B"})
            {
                ResetPair(.7f);events.Clear();AbilityDefinition ultimate=Ability(id);
                w.Abilities.EquipLabVariation(ultimate);
                bool cast=w.Abilities.TryUseAimed(AbilitySlot.Ultimate,Vector3.right,a.transform.position);
                yield return new WaitForSeconds(ultimate.startup*.5f);
                Check(FindObjectsByType<WarriorSkillPresentation>().Any(v=>v.name=="Warrior_Conjuration"),
                    id+": conjuração visível durante startup");
                CaptureWarriorFrame(id+"-charging");
                yield return new WaitForSeconds(ultimate.startup*.5f+.02f);
                CaptureWarriorFrame(id+"-release");
                float hpBefore=w.Health.CurrentHealth;ProbeIncoming(a,w,a.Definition.basicAttack);
                Check(cast && Near(hpBefore-w.Health.CurrentHealth,a.Definition.basicAttack.damage*(1f-ultimate.damageReduction),.08f),
                    id+": ultimate protege enquanto executa a ofensiva");
                yield return new WaitForSeconds(.2f);
                CaptureWarriorFrame(id+"-peak");
                float finish=Time.time+ultimate.fieldDuration+.4f;
                // Keep the target in the local strike to measure pulse damage separately from knockback.
                while(Time.time<finish){a.Motor.Teleport(w.transform.position+Vector3.right*.7f);yield return null;}
                int expectedTicks=ultimate.behavior==AbilityBehavior.WarriorGroundBlast ? 1 :
                    ultimate.behavior==AbilityBehavior.WarriorGroundWaves ? 3 : Mathf.CeilToInt(ultimate.fieldDuration/ultimate.fieldTickInterval);
                Check(Near(a.Health.MaxHealth-a.Health.CurrentHealth,ultimate.damage*expectedTicks,.15f),
                    id+": dano de explosão/campo/ondas corresponde a todos os pulsos");
                Check(events.Any(e=>e.source==w && e.ability==ultimate && e.kind==CombatEventKind.AbilityUltimate && e.phase==AbilityPhase.Active),
                    id+": evento de ultimate e execução de área preservados");
                Check(AbilityAimSolution.IsSelfCentered(ultimate) && !AbilityAimSolution.IsGroundTarget(ultimate) &&
                    ultimate.range==0f && ultimate.fieldRadius<=3f && ultimate.damage*expectedTicks<=18f,
                    id+": alcance local e dano máximo reduzido");
                float frozenDamage=id=="Warrior_Ult_Base" ? 18f : id=="Warrior_Ult_A" ? 3f : 6f;
                float frozenRadius=id=="Warrior_Ult_Base" ? 2.2f : id=="Warrior_Ult_A" ? 2.5f : 3f;
                Check(Near(ultimate.damage,frozenDamage) && Near(ultimate.fieldRadius,frozenRadius),
                    id+": dados correspondem aos valores congelados de VISUAL-006");
                ResetPair(8f);w.Abilities.EquipLabVariation(ultimate);
                w.Abilities.TryUseAimed(AbilitySlot.Ultimate,Vector3.right,a.transform.position);
                yield return new WaitForSeconds(ultimate.startup+ultimate.fieldDuration+.4f);
                Check(Near(a.Health.CurrentHealth,a.Health.MaxHealth),id+": mirar longe não desloca área nem acerta fora do raio");
            }

            ResetPair(2.8f);AbilityDefinition localWaves=Ability("Warrior_Ult_B");
            w.Abilities.EquipLabVariation(localWaves);w.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(localWaves.startup+.65f);
            Check(Near(a.Health.CurrentHealth,a.Health.MaxHealth),"Ruptura: alvo a 2,8 m não recebe as duas ondas interiores");
            yield return new WaitForSeconds(.5f);
            Check(Near(a.Health.MaxHealth-a.Health.CurrentHealth,6f),"Ruptura: alvo a 2,8 m recebe apenas terceira onda de 6");

            ResetPair(.7f);w.Abilities.EquipLabVariation(Ability("Warrior_Ult_Base"));
            w.Abilities.TryUse(AbilitySlot.Ultimate);yield return new WaitForSeconds(.2f);
            w.Abilities.InterruptOffensiveAction();yield return new WaitForSeconds(1f);
            Check(Near(a.Health.CurrentHealth,a.Health.MaxHealth) &&
                !FindObjectsByType<WarriorSkillPresentation>().Any(v=>v.name=="Warrior_Conjuration"),
                "Interromper conjuração cancela dano e limpa carga visual");

            AbilityDefinition ground=Ability("Mage_S1_C");
            Vector3 origin=new Vector3(2f,1f,3f);
            Vector3 near=AbilityAimSolution.ResolvePoint(origin,Vector3.right,ground,.25f);
            Vector3 far=AbilityAimSolution.ResolvePoint(origin,Vector3.right,ground,1f);
            Check(Near(Vector3.Distance(near,origin),ground.range*.25f) && Near(Vector3.Distance(far,origin),ground.range),
                "Mira de área regula distância curta e longa no mesmo alcance");
            Check(Vector3.Distance(AbilityAimSolution.ClampGroundPoint(origin,origin+Vector3.right*999f,ground),origin)<=ground.range+.01f,
                "Centro selecionado não ultrapassa alcance autorizado");
            ResetPair(5f);
            yield return TestWarriorReferencePresentation();
        }

        IEnumerator TestWarriorReferencePresentation()
        {
            foreach(string id in new[]{"Warrior_Defense_Base","Warrior_Defense_A"})
            {
                ResetPair(8f);w.Abilities.EquipLabVariation(Ability(id));w.Abilities.TryUse(AbilitySlot.Skill1);
                yield return new WaitForSeconds(.15f);CaptureWarriorFrame(id+"-reference");
                Check(FindObjectsByType<WarriorReferenceVfx>().Length>0,id+": defesa usa composição tridimensional de referência");
                w.Abilities.ResetTransientState();yield return null;
                Check(FindObjectsByType<WarriorReferenceVfx>().Length==0,id+": reset remove superfície e recursos da apresentação");
            }
            // Exercise the presentation directly: these events must never cause damage or spend resources.
            ResetPair(8f);
            float hp=a.Health.CurrentHealth,energy=w.Energy.CurrentEnergy;
            foreach(int step in new[]{1,2,3})
            {
                CombatEvents.Raise(new CombatEventData(CombatEventKind.AbilityAttack,w.transform.position,w,w,
                    step,w.Definition.basicAttack,AbilityPhase.Active,direction:w.Motor.Facing));
                yield return new WaitForSeconds(.08f);CaptureWarriorFrame("basic-step-"+step);
                Check(FindObjectsByType<WarriorReferenceVfx>().Any(),"Básico passo "+step+": corte de referência presente");
                yield return new WaitForSeconds(.3f);
            }
            Check(Near(a.Health.CurrentHealth,hp)&&Near(w.Energy.CurrentEnergy,energy),
                "Eventos de apresentação não aplicam dano nem consomem energia");
            foreach(var kind in new[]{WarriorSkillPresentation.Kind.Explosion,WarriorSkillPresentation.Kind.GroundField,WarriorSkillPresentation.Kind.Shockwave})
            {
                var host=new GameObject("Warrior_TestVisual");host.transform.position=w.transform.position;
                host.AddComponent<OwnedAbilityEffect>().Configure(w,null);
                host.AddComponent<WarriorSkillPresentation>().Begin(w,kind,.55f,2f,0,false);
                yield return new WaitForSeconds(.2f);
                WarriorReferenceVfx helper=host.GetComponentInChildren<WarriorReferenceVfx>();
                Check(helper!=null&&helper.GetComponentsInChildren<Collider>().Length==0&&
                    helper.GetComponentsInChildren<Rigidbody>().Length==0,kind+": volume animado sem colisão ou física de gameplay");
                Renderer[] renderers=helper!=null?helper.GetComponentsInChildren<Renderer>():new Renderer[0];
                Check(renderers.Length>0&&renderers.Length<=14,kind+": composição tem volume e respeita limite de 14 renderers auxiliares");
                Check(renderers.Length>0&&renderers.All(r=>r.sharedMaterials.All(m=>m!=null&&m.shader!=null&&m.shader.isSupported)),
                    kind+": shaders da composição estão disponíveis e suportados");
                yield return new WaitForSeconds(.5f);
                Check(host==null&&FindObjectsByType<WarriorReferenceVfx>().Length==0,kind+": término visual limpa host e helper");
            }
            ResetPair(5f);
        }
    }
}
#endif
