#if UNITY_EDITOR
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleRoyaleX
{
    public sealed partial class PrototypeLiveTests
    {
        IEnumerator TestMobileV4()
        {
            stage = "V4: Caçada, Travessia, fumaça, esquiva e controles";
            var hunt = Ability("Assassin_Ult_B");
            ResetPair(6f); a.Abilities.EquipVariation(hunt); events.Clear();
            float energyBefore = a.Energy.CurrentEnergy;
            bool started = a.Abilities.TryUse(AbilitySlot.Ultimate);
            float paid = energyBefore - a.Energy.CurrentEnergy;
            yield return new WaitForSeconds(0.13f);
            w.Motor.FaceDirection(Vector3.forward); w.Motor.Dash(4f, 0.22f, false, 0f);
            yield return new WaitForSeconds(1.8f);
            Check(started && Near(paid, hunt.energyCost) && a.Abilities.IsHuntRecastReady &&
                Near(w.Health.MaxHealth - w.Health.CurrentHealth, hunt.huntFirstDamage),
                "Caçada 1 segue dash lateral, causa dano leve e libera segundo acionamento");
            Check(Count(CombatEventKind.Hit,w) == 1 && w.transform.position.z > 4.5f,
                "Caçada 1 empurra e não repete dano durante perseguição");
            Check(a.Abilities.IsHuntRecastReady && !a.Motor.IsDashing && Identity(a.Modifiers),
                "Caçada aguarda segundo toque sem buff antigo nem avanço automático");
            Vector3 origin = a.transform.position;
            energyBefore = a.Energy.CurrentEnergy;
            bool recast = a.Abilities.TryUse(AbilitySlot.Ultimate);
            float extraPaid = energyBefore - a.Energy.CurrentEnergy;
            yield return new WaitForSeconds(1.8f);
            Vector3 approach = w.transform.position - origin; approach.y = 0;
            Check(recast && Near(extraPaid,0f) && Near(w.Health.MaxHealth-w.Health.CurrentHealth,hunt.huntFirstDamage+hunt.damage),
                "Caçada 2 cobra energia só no primeiro uso e aplica dano de travessia uma vez");
            Check(Vector3.Dot(a.transform.position-w.transform.position,approach.normalized)>2.5f && !a.Abilities.IsHuntRecastReady &&
                !a.Abilities.TryUse(AbilitySlot.Ultimate), "Caçada 2 termina no lado oposto além do alvo; terceiro toque recusado");

            ResetPair(4f); a.Abilities.EquipVariation(hunt); a.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(6.8f);
            Check(!a.Abilities.IsHuntRecastReady && !a.Abilities.TryUse(AbilitySlot.Ultimate), "Caçada: segunda etapa expira e mantém recarga");
            ResetPair(30f); a.Abilities.EquipVariation(hunt); energyBefore=a.Energy.CurrentEnergy;
            Check(!a.Abilities.TryUse(AbilitySlot.Ultimate) && Near(a.Energy.CurrentEnergy,energyBefore), "Caçada sem alvo em alcance não consome energia");
            ResetPair(8f); a.Abilities.EquipVariation(hunt); a.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(0.12f);
            var wall=GameObject.CreatePrimitive(PrimitiveType.Cube); wall.transform.position=new Vector3(0f,1.5f,0f);
            wall.transform.localScale=new Vector3(0.5f,3f,10f); Physics.SyncTransforms();
            yield return new WaitForSeconds(2f);
            Check(a.transform.position.x>0 && Near(w.Health.CurrentHealth,w.Health.MaxHealth) && !a.Abilities.IsActionBusy,
                "Caçada encontra parede: termina por limite sem atravessar cenário nem causar dano remoto");
            Destroy(wall); yield return null;

            ResetPair(5f); events.Clear(); a.Abilities.EquipVariation(hunt); a.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(0.9f);
            var assassinBot=a.GetComponent<PrototypeTrainingBot>() ?? a.gameObject.AddComponent<PrototypeTrainingBot>();
            assassinBot.runInEditor=true; assassinBot.enabled=true; assassinBot.ResetAwareness();
            yield return new WaitForSeconds(1.5f); assassinBot.enabled=false;
            Check(events.Count(e=>e.source==a && e.ability==hunt && e.phase==AbilityPhase.Active)==2 && !a.Abilities.IsHuntRecastReady,
                "Assassino bot utiliza a segunda etapa da Caçada pela mesma API do jogador");

            ResetPair(10f); a.Abilities.EquipVariation(Ability("Assassin_Move_A"));
            RuntimeModifiers mods=RuntimeModifiers.Identity; mods.damageMultiplier=1.35f;
            a.ApplyTimedModifiers(mods,5f); a.Abilities.TryUse(AbilitySlot.Movement);
            yield return new WaitForSeconds(0.4f);
            Check(a.Abilities.CanRecast(AbilitySlot.Movement) && Near(a.MovementSpeedBonus,1f) && Near(a.Modifiers.damageMultiplier,1.35f),
                "Travessia prepara teleporte sem bônus antigo nem substituir buff externo");
            yield return new WaitForSeconds(2.1f);
            Check(Near(a.MovementSpeedBonus,1f) && Near(a.Modifiers.damageMultiplier,1.35f),
                "Travessia não modifica bônus independentes da ultimate");

            foreach(string id in new[]{"Assassin_Defense_Base"})
            {
                ResetPair(2f); events.Clear(); a.Abilities.EquipVariation(Ability(id)); a.Abilities.TryUse(AbilitySlot.Defense,Vector3.forward);
                yield return new WaitForSeconds(0.72f);
                bool armed=a.Abilities.ReactiveDefenseRemaining>0 && !a.Motor.IsDashing;
                var hit=SpawnHit(w,w.Definition.basicAttack,a.transform.position+Vector3.up); hit.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>());
                hit.Cancel(); Destroy(hit.gameObject);
                bool avoided=Near(a.Health.CurrentHealth,a.Health.MaxHealth);
                yield return new WaitForSeconds(0.5f);
                Check(armed && avoided && a.transform.position.x<w.transform.position.x-2.5f && Count(CombatEventKind.Hit,w)==1,
                    id+": golpe aos 0,72 s dispara travessia longa com dano único baixo");
            }
            ResetPair(2f); a.Abilities.TryUse(AbilitySlot.Defense,Vector3.forward);
            yield return new WaitForSeconds(1.2f);
            var expiredHit=SpawnHit(w,w.Definition.basicAttack,a.transform.position+Vector3.up); expiredHit.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>());
            Check(a.Health.CurrentHealth<a.Health.MaxHealth && a.Abilities.ReactiveDefenseRemaining==0f,
                "Esquiva expirada não evita golpe nem dispara travessia");
            expiredHit.Cancel(); Destroy(expiredHit.gameObject);

            ResetPair(12f); a.Abilities.TryUse(AbilitySlot.Defense,Vector3.forward);
            a.Health.ApplyDamage(500f);
            Check(Near(a.Health.CurrentHealth,a.Health.MaxHealth),"Esquiva já protege no acionamento, inclusive antes do primeiro frame ativo");
            yield return new WaitForSeconds(0.7f);
            foreach(var kind in new[]{AttackKind.Physical,AttackKind.Projectile,AttackKind.Area,AttackKind.Magical})
            {
                var h=SpawnHit(w,w.Definition.basicAttack,a.transform.position+Vector3.up);
                var packet=h.Packet; packet.attackKind=kind; packet.blockable=false; packet.damage=500f; h.UpdatePacket(packet);
                h.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>()); h.Cancel(); Destroy(h.gameObject);
            }
            a.Health.ApplyDamage(500f);
            Check(Near(a.Health.CurrentHealth,a.Health.MaxHealth),"Esquiva ignora golpes físicos, projéteis, área, magia e dano direto repetidos durante 1 segundo");
            yield return new WaitForSeconds(0.5f); a.Health.ApplyDamage(1f);
            Check(Near(a.Health.CurrentHealth,a.Health.MaxHealth-1f),"Imunidade da esquiva expira e dano volta a funcionar");

            ResetPair(2f); a.Abilities.TryUse(AbilitySlot.Defense,Vector3.forward);
            yield return new WaitForSeconds(0.94f);
            var late=SpawnHit(w,w.Definition.basicAttack,a.transform.position+Vector3.up); late.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>());
            late.Cancel(); Destroy(late.gameObject);
            yield return new WaitForSeconds(0.12f); a.Health.ApplyDamage(500f);
            Check(a.Motor.IsDashing && Near(a.Health.CurrentHealth,a.Health.MaxHealth),"Travessia disparada no fim da janela mantém imunidade até a chegada");
            yield return new WaitForSeconds(0.5f);

            ResetPair(16f); a.Abilities.EquipVariation(Ability("Assassin_Defense_A"));
            SmokeVisibility.LocalPlayer=a; a.Abilities.TryUse(AbilitySlot.Defense);
            yield return new WaitForSeconds(0.3f);
            var field=FindAnyObjectByType<SmokeField>();
            Check(field!=null && field.Radius==6f && field.SuppressesAttacks && field.ParticleCount>=64,
                "Contra-Sombra cria fumaça densa de 12 m de diâmetro, sem ataque ou parry automático");
            Check(!a.Abilities.TryUse(AbilitySlot.BasicAttack) && !a.Abilities.TryUse(AbilitySlot.Ultimate) && !a.State.IsInvulnerable,
                "Dentro da fumaça: ataque/ultimate bloqueados, sem invulnerabilidade");
            var outsideHit=SpawnHit(w,w.Definition.basicAttack,a.transform.position+Vector3.up); outsideHit.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>());
            Check(Near(a.Health.MaxHealth-a.Health.CurrentHealth,w.Definition.basicAttack.damage),
                "Ataque lançado de fora acerta personagem oculto dentro da fumaça");
            outsideHit.Cancel(); Destroy(outsideHit.gameObject);
            w.Motor.Teleport(a.transform.position+Vector3.left*2f); Physics.SyncTransforms();
            yield return null; yield return null;
            Check(!w.Abilities.TryUse(AbilitySlot.BasicAttack) && !w.GetComponentInChildren<SkinnedMeshRenderer>().enabled &&
                a.GetComponentInChildren<SkinnedMeshRenderer>().enabled,
                "Fumaça impede ambos de atacar; inimigo oculto e personagem local visível");
            var ownColor=new MaterialPropertyBlock(); a.GetComponentInChildren<SkinnedMeshRenderer>().GetPropertyBlock(ownColor);
            Check(ownColor.GetColor("_BaseColor").r<0.6f,"Personagem local recebe acabamento fosco enquanto oculto");
            var suppressed=SpawnHit(w,w.Definition.basicAttack,a.transform.position+Vector3.up); float hp=a.Health.CurrentHealth;
            suppressed.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>());
            Check(Near(a.Health.CurrentHealth,hp),"Hitbox existente de atacante dentro da fumaça não causa dano");
            suppressed.Cancel(); Destroy(suppressed.gameObject);
            SmokeField.ClearAll(); yield return null; yield return null;
            Check(w.GetComponentInChildren<SkinnedMeshRenderer>().enabled && !SmokeField.PreventsAttack(a),
                "Fim da fumaça restaura visibilidade e possibilidade de ataque");
            SmokeVisibility.LocalPlayer=null;

            ResetPair(20f);
            var uiGo=new GameObject("Test_MobileV4_Controls");
            var controls=uiGo.AddComponent<PrototypeMobileTouchControls>(); controls.runInEditor=true;
            yield return null; yield return null;
            var labSelectors=controls.GetComponentsInChildren<RectTransform>(true).Where(r=>r.name.StartsWith("LabSkill_")).OrderBy(r=>r.name).ToArray();
            Check(labSelectors.Length==3 && labSelectors.All(r=>r.parent.name=="SafeAreaRoot" && r.gameObject.activeInHierarchy),
                "Os três seletores A/B/C estão acessíveis na tela de combate");
            bool actualSelectionWorked=true;
            foreach(CharacterClass cls in new[]{CharacterClass.Mage,CharacterClass.Archer})
            {
                controls.SelectLabClass(cls);
                for(int slotIndex=0;slotIndex<3;slotIndex++)
                {
                    AbilitySlot slot=(AbilitySlot)(slotIndex+1);
                    for(int click=1;click<=3;click++)
                    {
                        var selectionPointer=new PointerEventData(EventSystem.current) { pointerId=31,
                            position=RectTransformUtility.WorldToScreenPoint(null,labSelectors[slotIndex].position) };
                        ExecuteEvents.Execute(labSelectors[slotIndex].gameObject,selectionPointer,ExecuteEvents.pointerDownHandler);
                        actualSelectionWorked &= a.Abilities.GetEquipped(slot)==a.Definition.GetVariant(slot,click%3);
                    }
                }
            }
            Check(actualSelectionWorked,"Toques reais nos três seletores alternam A/B/C de Mago e Arqueiro");
            controls.SelectLabClass(CharacterClass.Assassin);
            bool defenseA=controls.CycleLabVariation(AbilitySlot.Defense) && a.Abilities.GetEquipped(AbilitySlot.Defense)==a.Definition.defenseVariantA;
            bool defenseB=controls.CycleLabVariation(AbilitySlot.Defense) && a.Abilities.GetEquipped(AbilitySlot.Defense)==a.Definition.defenseVariantB;
            bool defenseBase=controls.CycleLabVariation(AbilitySlot.Defense) && a.Abilities.GetEquipped(AbilitySlot.Defense)==a.Definition.defenseBase;
            Check(defenseA && defenseB && defenseBase,"Botão 1 alterna Defesa Base/A/B sem consumir runa");
            Check(controls.CycleLabVariation(AbilitySlot.Movement) && controls.CycleLabVariation(AbilitySlot.Movement) &&
                a.Abilities.GetEquipped(AbilitySlot.Movement)==a.Definition.movementVariantB,"Botão 2 alterna Movimento de A para B");
            controls.CycleLabVariation(AbilitySlot.Movement);
            Check(controls.CycleLabVariation(AbilitySlot.Ultimate) && controls.CycleLabVariation(AbilitySlot.Ultimate) &&
                a.Abilities.GetEquipped(AbilitySlot.Ultimate)==a.Definition.ultimateVariantB,"Botão 3 alterna Ultimate de A para B");
            controls.CycleLabVariation(AbilitySlot.Ultimate);
            var stationary=controls.CycleBotMode(); var stationaryAttack=controls.CycleBotMode(); var normal=controls.CycleBotMode();
            Check(stationary==PrototypeTrainingBot.TrainingMode.Stationary && stationaryAttack==PrototypeTrainingBot.TrainingMode.StationaryAttack &&
                normal==PrototypeTrainingBot.TrainingMode.Normal && controls.CurrentBotMode==PrototypeTrainingBot.TrainingMode.Normal,
                "Botão do laboratório alterna bot Normal/Parado/Parado + ataque");
            controls.OpenSettings();
            Check(controls.Player==a && Time.timeScale==0f,"Menu mobile pausa combate e identifica jogador Assassino");
            Check(labSelectors.All(r=>r.parent.name=="SafeAreaRoot"), "Menu de classe mantém os seletores como controles da arena");
            controls.SwitchPlayer();
            Check(controls.Player==a && a.Definition.characterClass==CharacterClass.Warrior && w.Definition.characterClass==CharacterClass.Warrior &&
                w.GetComponent<PrototypeTrainingBot>().enabled && !a.GetComponent<PrototypeTrainingBot>().enabled &&
                !a.GetComponent<PrototypeLocalInput>().enabled && !w.GetComponent<PrototypeLocalInput>().enabled,
                "Seletor mobile troca a classe do slot jogador para Guerreiro e mantém oponente Guerreiro bot");
            controls.SwitchPlayer();
            Check(controls.Player==a && a.Definition.characterClass==CharacterClass.Mage &&
                w.GetComponent<PrototypeTrainingBot>().enabled,
                "Seletor mobile alcança a classe Mago");
            controls.SwitchPlayer();
            Check(controls.Player==a && a.Definition.characterClass==CharacterClass.Archer,
                "Seletor mobile alcança a classe Arqueiro");
            controls.SwitchPlayer();
            Check(controls.Player==a && a.Definition.characterClass==CharacterClass.Assassin &&
                w.GetComponent<PrototypeTrainingBot>().enabled && !a.GetComponent<PrototypeTrainingBot>().enabled,
                "Ciclo completo restaura Assassino no slot jogador e Guerreiro no slot bot");
            bool selectedMage = controls.SelectLabClass(CharacterClass.Mage) &&
                a.Definition.characterClass == CharacterClass.Mage;
            bool selectedAssassin = controls.SelectLabClass(CharacterClass.Assassin) &&
                a.Definition.characterClass == CharacterClass.Assassin;
            Check(selectedMage && selectedAssassin,
                "Menu permite selecionar diretamente a classe, sem percorrer todas as opções");
            Check(AbilityTechnicalInfo.Describe(hunt).Contains("22") && AbilityTechnicalInfo.Describe(Ability("Assassin_Move_A")).Contains("teleporta"),
                "Informações técnicas são geradas dos valores reais das habilidades");
            controls.BeginLayoutEdit();
            var attackRect=controls.GetComponentsInChildren<RectTransform>().First(r=>r.name=="Attack");
            var defenseRect=controls.GetComponentsInChildren<RectTransform>().First(r=>r.name=="Defense");
            Check(Near(attackRect.rect.width,328f) && Near(defenseRect.rect.width,232f),"Botões padrão têm diâmetro duplicado: ataque 328 e defesa 232");
            string oldPrefs=PlayerPrefs.GetString("BRX.MobileLayout.v4","");
            bool hadPrefs=PlayerPrefs.HasKey("BRX.MobileLayout.v4");
            controls.EditButton(attackRect,null); controls.ResizeSelected(0.2f);
            var pointer=new PointerEventData(EventSystem.current) { position=RectTransformUtility.WorldToScreenPoint(null,attackRect.position)+new Vector2(-50f,60f) };
            float attackBefore=a.Abilities.GetCooldownRemaining(AbilitySlot.BasicAttack);
            ExecuteEvents.Execute(attackRect.gameObject,pointer,ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(attackRect.gameObject,pointer,ExecuteEvents.dragHandler);
            Vector2 moved=attackRect.anchoredPosition; float resized=attackRect.localScale.x;
            string saved=controls.SerializeLayout(); controls.ResetLayout(); controls.ApplyLayout(saved);
            Check(Vector2.Distance(attackRect.anchoredPosition,moved)<0.1f && Near(attackRect.localScale.x,resized) &&
                Near(attackBefore,a.Abilities.GetCooldownRemaining(AbilitySlot.BasicAttack)),
                "Arrastar/redimensionar e serializar/restaurar layout mantém posição/tamanho sem lançar habilidade");
            controls.SaveLayoutAndClose();
            Check(PlayerPrefs.GetString("BRX.MobileLayout.v4")==saved && Time.timeScale>0f && !controls.IsEditingLayout,
                "Salvar layout persiste preferências locais e retoma combate");
            if(hadPrefs)PlayerPrefs.SetString("BRX.MobileLayout.v4",oldPrefs);else PlayerPrefs.DeleteKey("BRX.MobileLayout.v4"); PlayerPrefs.Save();
            controls.ResetLayout();
            foreach(var bot in FindObjectsByType<PrototypeTrainingBot>())bot.enabled=false;
            ScreenCapture.CaptureScreenshot(Path.GetFullPath("Logs/mobile-v4-controls.png"));
            yield return new WaitForSecondsRealtime(0.2f);
            controls.OpenSettings();
            ScreenCapture.CaptureScreenshot(Path.GetFullPath("Logs/mobile-v4-info.png"));
            yield return new WaitForSecondsRealtime(0.2f);
            controls.CloseSettings(); Destroy(uiGo); yield return null;
            SmokeVisibility.LocalPlayer=null;
            ResetPair(5f);
        }
    }
}
#endif
