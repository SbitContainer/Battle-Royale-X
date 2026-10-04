#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BattleRoyaleX
{
    // Editor-only integration tests. Spawned by the menu, never saved into the arena.
    public sealed partial class PrototypeLiveTests : MonoBehaviour
    {
        CharacterRuntime w, a;
        readonly List<string> results = new List<string>();
        readonly List<CombatEventData> events = new List<CombatEventData>();
        readonly List<UnityEngine.Object> temporary = new List<UnityEngine.Object>();
        string stage = "Preparando testes", last = "", reportPath;
        int passed, failed;
        bool finished;
        int runtimeErrors;
        float automaticDropDelay;
        float previousCaptureDelta;
        bool ownsCaptureStep;

        IEnumerator Start()
        {
            previousCaptureDelta = Time.captureDeltaTime;
            ownsCaptureStep = true;
            // Repeatable simulation time, not an FPS benchmark. Restore the editor setting at completion or interruption.
            Time.captureDeltaTime = 1f / 60f;
            reportPath = Path.GetFullPath("Docs/TEST_RESULTS_LAB_001.md");
            Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
            File.WriteAllText(reportPath, "# Matriz ao vivo — " + DateTime.Now + "\n\nTestes no Play Mode, com comandos às APIs reais e colisões da Unity. Não substituem avaliação humana de diversão.\n\n");
            CombatEvents.Raised += Record;
            Application.logMessageReceived += OnLog;
            // Freeze the timer before the first frame: shader warmup can make that frame longer than the drop delay.
            var drop = FindAnyObjectByType<AirdropManager>();
            automaticDropDelay = drop.firstDropDelay;
            drop.enabled = false;
            yield return new WaitForSeconds(1f);
            var characters = FindObjectsByType<CharacterRuntime>();
            w = characters.FirstOrDefault(c => c.TeamId == TeamId.PlayerOne);
            a = characters.FirstOrDefault(c => c.TeamId == TeamId.PlayerTwo);
            if (w == null || a == null) { Check(false, "Cena contém os dois jogadores"); yield break; }
            foreach (var c in characters) c.GetComponent<PrototypeLocalInput>().enabled = false;
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            stage = "Movimento e enquadramento inicial";
            yield return new WaitForSeconds(2f);
            Check(FullBodyVisible(w) && FullBodyVisible(a), "Câmera enquadra o corpo inteiro dos dois no spawn");
            ResetPair(25f); yield return new WaitForSeconds(2f);
            Check(FullBodyVisible(w) && FullBodyVisible(a), "Câmera mantém corpos inteiros com maior separação");
            var i1 = w.GetComponent<PrototypeLocalInput>(); var i2 = a.GetComponent<PrototypeLocalInput>();
            Check(i1.up == KeyCode.W && i1.left == KeyCode.A && i1.down == KeyCode.S && i1.right == KeyCode.D,
                "P1: mapeamento WASD (entrada física verificada separadamente)");
            Check(i2.up == KeyCode.UpArrow && i2.left == KeyCode.LeftArrow && i2.down == KeyCode.DownArrow && i2.right == KeyCode.RightArrow,
                "P2: mapeamento setas (entrada física verificada separadamente)");
            ResetPair(6f);
            Vector3 wp = w.transform.position, ap = a.transform.position;
            w.Motor.SetMoveInput(Vector2.up); a.Motor.SetMoveInput(Vector2.up);
            yield return new WaitForSeconds(0.6f);
            w.Motor.SetMoveInput(Vector2.zero); a.Motor.SetMoveInput(Vector2.zero);
            Check(w.transform.position.z > wp.z + 1f, "Motor do Guerreiro responde ao movimento");
            Check(a.transform.position.z > ap.z + 1f, "Motor do Assassino responde ao movimento");
            Check(Near(w.transform.position.y, wp.y) && Near(a.transform.position.y, ap.y), "Personagens permanecem no plano XZ");
            yield return new WaitForSeconds(1f);

            stage = "Ataques reais, dano único e cooldown";
            ResetPair(1.4f); events.Clear();
            bool accepted = w.Abilities.TryUse(AbilitySlot.BasicAttack);
            bool spamRejected = !w.Abilities.TryUse(AbilitySlot.BasicAttack);
            yield return new WaitForSeconds(0.7f);
            Check(accepted && Near(a.Health.MaxHealth-a.Health.CurrentHealth,w.Definition.basicAttack.damage) && Count(CombatEventKind.Hit,a)==1,
                "Ataque do Guerreiro causa dano uma vez por ativação via física");
            Check(FindObjectsByType<ParticleSystem>().Any(p=>p.name=="VFX_BloodImpact"),
                "Contato com dano gera partículas de sangue direcionais");
            Check(spamRejected && Near(w.Abilities.GetCooldownRemaining(AbilitySlot.BasicAttack),0f), "Cadência das fases impede spam sem cooldown de básico");
            ResetPair(1.4f); events.Clear(); a.Abilities.TryUse(AbilitySlot.BasicAttack);
            yield return new WaitForSeconds(0.7f);
            Check(Near(w.Health.MaxHealth-w.Health.CurrentHealth,a.Definition.basicAttack.damage) && Count(CombatEventKind.Hit,w)==1,
                "Ataque do Assassino causa dano uma vez por ativação via física");
            ResetPair(1.4f); a.teamId = TeamId.PlayerOne; w.Abilities.TryUse(AbilitySlot.BasicAttack);
            yield return new WaitForSeconds(0.6f);
            Check(Near(a.Health.CurrentHealth,a.Health.MaxHealth), "Sem friendly fire na mesma equipe");
            a.teamId = TeamId.PlayerTwo;

            stage = "Clash: colisão, dano reduzido e micro-stagger";
            ResetPair(2.7f); events.Clear();
            // Synchronize active windows of the two existing attacks without changing the data.
            w.Abilities.TryUse(AbilitySlot.BasicAttack);
            yield return new WaitForSeconds(Mathf.Max(0f,w.Definition.basicAttack.startup-a.Definition.basicAttack.startup));
            a.Abilities.TryUse(AbilitySlot.BasicAttack);
            bool sawStagger=false, sawCancelled=false;
            float until=Time.time+0.8f;
            while(Time.time<until)
            {
                if(events.Any(e=>e.kind==CombatEventKind.Clash))
                {
                    sawStagger |= w.State.IsStaggered && a.State.IsStaggered;
                    sawCancelled |= FindObjectsByType<Hitbox>().Count(h=>h.Cancelled)>=2;
                }
                yield return null;
            }
            Check(events.Any(e=>e.kind==CombatEventKind.Clash), "Dois ataques físicos colidem em Clash");
            Check(Near(w.Health.MaxHealth-w.Health.CurrentHealth,a.Definition.basicAttack.damage*a.Definition.basicAttack.clashDamageFactor)
                && Near(a.Health.MaxHealth-a.Health.CurrentHealth,w.Definition.basicAttack.damage*w.Definition.basicAttack.clashDamageFactor), "Clash causa apenas dano reduzido nos dois");
            Check(sawStagger && !w.State.IsStaggered && !a.State.IsStaggered, "Micro-stagger do Clash inicia e expira");
            Check(sawCancelled, "Clash cancela ambas as hitboxes");
            yield return new WaitForSeconds(1f);

            stage = "Defesa: guarda, parry perfeito e parcial";
            ResetPair(1.4f); events.Clear(); w.Abilities.TryUse(AbilitySlot.Defense);
            yield return new WaitForSeconds(0.12f); a.Abilities.TryUse(AbilitySlot.BasicAttack);
            yield return new WaitForSeconds(0.6f);
            Check(Count(CombatEventKind.Block,w)==1 && Near(w.Health.CurrentHealth,w.Health.MaxHealth), "Guarda anula ataque básico totalmente");
            yield return new WaitForSeconds(0.4f);
            ResetPair(1.4f); events.Clear();
            w.Abilities.EquipVariation(GenericParryFixture());
            w.Abilities.TryUse(AbilitySlot.Defense); a.Abilities.TryUse(AbilitySlot.BasicAttack);
            bool parryStagger=false; until=Time.time+0.65f;
            while(Time.time<until) { parryStagger |= a.State.IsStaggered; yield return null; }
            Check(Count(CombatEventKind.Parry,a)==1 && Near(w.Health.CurrentHealth,w.Health.MaxHealth) && parryStagger && !a.State.IsStaggered,
                "Parry perfeito zera dano e aplica micro-stagger no atacante");
            ResetPair(1.4f); events.Clear();
            var partialDefense = GenericParryFixture();
            w.Abilities.EquipVariation(partialDefense); w.Abilities.TryUse(AbilitySlot.Defense);
            // Synchronize contact to the actual defense window, not an extra attack startup plus variable frame time.
            float partialDeadline = Time.time + 1f;
            while (!w.Defense.IsActive && Time.time < partialDeadline) yield return null;
            while (w.Defense.Remaining > partialDefense.defenseDuration - partialDefense.perfectWindow - 0.02f) yield return null;
            bool withinPartialWindow = w.Defense.IsActive;
            var partialHit = SpawnHit(a, a.Definition.basicAttack, w.transform.position + Vector3.up);
            partialHit.TryResolveHurtbox(w.GetComponentInChildren<Hurtbox>());
            partialHit.Cancel(); Destroy(partialHit.gameObject);
            Check(withinPartialWindow && Count(CombatEventKind.Block,w)==1 && Near(w.Health.MaxHealth-w.Health.CurrentHealth,a.Definition.basicAttack.damage*0.5f), "Parry fora da janela perfeita defende parcialmente");
            yield return new WaitForSeconds(0.6f);

            stage = "Esquiva e movimentos especiais";
            ResetPair(6f); a.Abilities.TryUse(AbilitySlot.Defense);
            yield return new WaitForSeconds(a.Definition.defenseBase.startup+0.03f);
            var probe = SpawnHit(w,w.Definition.basicAttack,a.transform.position+Vector3.up);
            events.Clear();
            yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            Check(Near(a.Health.CurrentHealth,a.Health.MaxHealth) && Count(CombatEventKind.Dodge,a)>0, "Esquiva reativa evita o golpe recebido na janela");
            yield return new WaitForSeconds(0.6f);
            ResetPair(8f); wp=w.transform.position; w.Abilities.TryUse(AbilitySlot.Movement);
            yield return new WaitForSeconds(0.7f);
            Check(Vector3.Distance(wp,w.transform.position)<0.03f && w.Definition.movementBase.behavior==AbilityBehavior.WarriorShieldCharge, "Arremesso de escudo mantém o Guerreiro parado");
            ResetPair(1.4f); a.Abilities.EquipVariation(Ability("Assassin_Move_A")); a.Abilities.TryUse(AbilitySlot.Movement);
            yield return new WaitForSeconds(0.6f);
            Check(a.Abilities.CanRecast(AbilitySlot.Movement) && Near(w.Health.MaxHealth-w.Health.CurrentHealth,4f), "Travessia lança adaga, causa dano leve e arma teleporte");
            ResetPair(6f); ap=a.transform.position; a.Abilities.EquipVariation(Ability("Assassin_Move_B")); a.Abilities.TryUse(AbilitySlot.Movement);
            yield return new WaitForSeconds(0.65f);
            bool returned=a.Abilities.TryUse(AbilitySlot.Movement);
            yield return new WaitForSeconds(0.5f);
            Check(returned && Vector3.Distance(ap,a.transform.position)<0.1f, "Retorno volta dentro da janela");
            yield return new WaitForSeconds(0.5f);
            ResetPair(6f); a.Abilities.EquipVariation(Ability("Assassin_Move_B")); a.Abilities.TryUse(AbilitySlot.Movement);
            yield return new WaitForSeconds(2.2f); ap=a.transform.position;
            Check(!a.Abilities.TryUse(AbilitySlot.Movement) && Vector3.Distance(ap,a.transform.position)<0.1f, "Retorno expira e respeita cooldown");

            stage = "Dano de travessia e resposta defensiva";
            ResetPair(2f); events.Clear(); a.Abilities.TryUse(AbilitySlot.Movement);
            yield return new WaitForSeconds(0.7f);
            Check(Near(w.Health.MaxHealth-w.Health.CurrentHealth,24f) && Count(CombatEventKind.Hit,w)==1,
                "Travessia do Assassino causa 24 de dano uma única vez");
            ResetPair(2f); events.Clear(); w.Defense.Activate(DefenseKind.Guard,1f,0f,0.75f,0f);
            a.Abilities.TryUse(AbilitySlot.Movement); yield return new WaitForSeconds(1.1f);
            Check(Near(w.Health.MaxHealth-w.Health.CurrentHealth,6f) && Count(CombatEventKind.Block,w)==1,
                "Guarda reduz travessia de 24 para 6 de dano");
            ResetPair(2f); events.Clear(); w.Abilities.EquipVariation(GenericParryFixture()); w.Defense.Activate(DefenseKind.Parry,0.7f,0.6f,0.5f,0f);
            a.Abilities.TryUse(AbilitySlot.Movement); yield return new WaitForSeconds(0.9f);
            Check(Near(w.Health.CurrentHealth,w.Health.MaxHealth) && Count(CombatEventKind.Parry,a)==1 && Near(a.Health.CurrentHealth,a.Health.MaxHealth)
                && w.Abilities.HasCounterOpportunity, "Parry nega travessia e abre contra-ataque manual");
            w.Motor.Teleport(new Vector3(-0.7f,1,0)); a.Motor.Teleport(new Vector3(0.7f,1,0));
            w.Motor.FaceDirection(Vector3.right); a.Motor.FaceDirection(Vector3.left); Physics.SyncTransforms();
            w.Abilities.TryUse(AbilitySlot.BasicAttack); yield return new WaitForSeconds(0.7f);
            Check(Near(a.Health.MaxHealth-a.Health.CurrentHealth,w.Definition.basicAttack.damage+6f) && !w.Abilities.HasCounterOpportunity,
                "Contra-ataque do Guerreiro exige básico e adiciona 6 de dano uma vez");
            ResetPair(2f); events.Clear(); w.Abilities.TryUse(AbilitySlot.Movement);
            yield return new WaitForSeconds(0.7f);
            Debug.Log($"[BRX DASH PROBE] hp={a.Health.CurrentHealth}/{a.Health.MaxHealth} hits={Count(CombatEventKind.Hit,a)} gap={Vector3.Distance(w.transform.position,a.transform.position)}");
            Check(Near(a.Health.MaxHealth-a.Health.CurrentHealth,w.Definition.movementBase.damage) && Count(CombatEventKind.Hit,a)==1,
                "Arremesso de escudo causa o dano configurado uma vez");
            ResetPair(2f); a.teamId=TeamId.PlayerOne; a.Abilities.TryUse(AbilitySlot.Movement);
            yield return new WaitForSeconds(0.7f);
            Check(Near(w.Health.CurrentHealth,w.Health.MaxHealth), "Travessia não causa friendly fire");
            a.teamId=TeamId.PlayerTwo;
            ResetPair(2f); w.Motor.Teleport(new Vector3(-1f,1f,3f)); Physics.SyncTransforms();
            a.Abilities.TryUse(AbilitySlot.Movement); yield return new WaitForSeconds(0.7f);
            Check(Near(w.Health.CurrentHealth,w.Health.MaxHealth), "Travessia fora do trajeto não acerta");
            ResetPair(4f);
            var wall=GameObject.CreatePrimitive(PrimitiveType.Cube); wall.name="Test_DashWall";
            wall.transform.position=new Vector3(0,1.5f,0);wall.transform.localScale=new Vector3(0.5f,3f,4f);Physics.SyncTransforms();
            a.Abilities.TryUse(AbilitySlot.Movement); yield return new WaitForSeconds(0.7f);
            Check(a.transform.position.x>0 && Near(w.Health.CurrentHealth,w.Health.MaxHealth), "Parede bloqueia dash e dano além dela");
            Destroy(wall); yield return null;

            stage = "Ultimates: início, término e variantes";
            ResetPair(5f); w.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(0.4f);
            yield return new WaitForSeconds(w.Definition.ultimateBase.startup);
            Check(Identity(w.Modifiers) && events.Any(e=>e.ability==w.Definition.ultimateBase && e.phase==AbilityPhase.Active), "Ultimate de explosão ativa sem buff legado de dano");
            yield return new WaitForSeconds(w.Definition.ultimateBase.buffDuration+0.2f);
            Check(Identity(w.Modifiers), "Buff expira e todos os modificadores voltam a 1.0");
            ResetPair(1.4f); events.Clear();
            float assassinEnergyBefore=a.Energy.CurrentEnergy;
            int acceptedDashes=a.Abilities.TryUse(AbilitySlot.Ultimate,Vector3.left)?1:0;
            float firstCastEnergy=a.Energy.CurrentEnergy;
            yield return new WaitForSeconds(0.42f);
            for(int dash=1;dash<5;dash++)
            {
                w.Motor.Teleport(new Vector3(-0.7f,1,0));a.Motor.Teleport(new Vector3(0.7f,1,0));
                a.Motor.FaceDirection(Vector3.left);Physics.SyncTransforms();
                if(a.Abilities.TryUse(AbilitySlot.Ultimate,Vector3.left))acceptedDashes++;
                yield return new WaitForSeconds(0.42f);
            }
            bool sixthRejected=!a.Abilities.TryUse(AbilitySlot.Ultimate,Vector3.left);
            Check(acceptedDashes==5 && sixthRejected && Near(assassinEnergyBefore-firstCastEnergy,45f,0.1f),
                "Cinco Cortes cobra energia uma vez e aceita exatamente cinco dashes manuais");
            Check(Near(w.Health.MaxHealth-w.Health.CurrentHealth,60f,0.1f) && Count(CombatEventKind.Hit,w)==5,
                "Cinco Cortes causa 12 por travessia e 60 no total sem defesa");
            Check(!a.Abilities.IsChargedSequenceActive && a.Abilities.ChargedSequenceRemaining==0 && a.Abilities.GetCooldownRemaining(AbilitySlot.Ultimate)>24f,
                "Cinco Cortes encerra cargas e mantém cooldown iniciado no primeiro cast");
            // Caçada is now a manual two-stage pursuit, covered in TestMobileV4 (no legacy buff expectation).
            ResetPair(5f); a.Abilities.EquipVariation(Ability("Assassin_Ult_A")); a.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(0.5f); wp=w.transform.position; w.Motor.SetMoveInput(Vector2.up);
            yield return new WaitForSeconds(0.4f); w.Motor.SetMoveInput(Vector2.zero);
            Check(a.Abilities.IsExecutionHidden && Identity(a.Modifiers) && !w.State.InputLocked && w.transform.position.z>wp.z+0.5f, "Execução golpeia e oculta sem buff antigo nem travar controle do oponente");
            yield return new WaitForSeconds(4f);

            stage = "Inventário: capacidade e cura contínua";
            ResetPair(5f);
            Check(w.Inventory.Capacity==3, "Mochila inicia com três slots");
            w.Inventory.TryAdd(Item("Heal")); w.Inventory.TryAdd(Item("Energy")); w.Inventory.TryAdd(Item("Smoke"));
            Check(!w.Inventory.TryAdd(Item("Barrier")) && w.Inventory.Slots.Count==3, "Quarto item recusado sem upgrade");
            w.Inventory.Initialize(3); w.Inventory.TryAdd(Item("Backpack4")); w.Inventory.UseSlot(0);
            yield return new WaitForSeconds(0.5f);
            Check(w.Inventory.Capacity==4, "Usar mochila de drop aumenta capacidade para quatro");
            Check(w.Inventory.Slots.Count==0, "Item consumido desaparece");
            w.Health.ApplyDamage(50f); w.Inventory.TryAdd(Item("Heal")); float hp=w.Health.CurrentHealth; w.Inventory.UseSlot(0);
            yield return new WaitForSeconds(0.5f);
            bool healStarted=w.Health.CurrentHealth>hp && w.Health.CurrentHealth<hp+Item("Heal").amount && w.HealthRegeneration.IsRegenerating;
            w.Health.ApplyDamage(1f); float damagedDuringHeal=w.Health.CurrentHealth; yield return new WaitForSeconds(0.3f);
            Check(healStarted && w.Inventory.Slots.Count==0 && w.Health.CurrentHealth>damagedDuringHeal && w.HealthRegeneration.IsRegenerating,
                "Cura começa progressivamente, é consumida e dano não interrompe o HoT");
            yield return new WaitForSeconds(4.4f);
            Check(Near(w.Health.CurrentHealth,hp-1f+Item("Heal").amount,0.25f) && !w.HealthRegeneration.IsRegenerating,
                "Cura contínua restaura 30 HP totais em 5 s");
            w.Energy.TrySpend(70f); float energy=w.Energy.CurrentEnergy;
            w.Inventory.TryAdd(Item("Energy")); w.Inventory.UseSlot(0);
            yield return new WaitForSeconds(0.95f);
            Check(!w.Inventory.IsUsingItem && w.Inventory.Slots.Count==0 && w.Energy.CurrentEnergy>energy+25f, "Essência mantém seu uso temporizado e restaura energia");

            stage = "Troca de variação e coleta no chão";
            string[] expectedGroundVariations={"Var_Assassin_Counter","Var_Assassin_Double","Var_Assassin_Travel","Var_Assassin_Return","Var_Assassin_Exec","Var_Assassin_Hunt",
                "Var_Warrior_Parry","Var_Warrior_Fortress","Var_Warrior_Impact","Var_Warrior_Advance","Var_Warrior_Ret","Var_Warrior_Push"};
            var groundPickups=FindObjectsByType<WorldPickup>();
            Check(expectedGroundVariations.All(id=>groundPickups.Any(p=>p.item!=null&&p.item.itemId==id)), "Todas as 12 variações existem como pickups no chão");
            var assassinRune=groundPickups.FirstOrDefault(p=>p.item!=null&&p.item.itemId=="Var_Assassin_Travel");
            Check(assassinRune!=null && !assassinRune.CanCollect(w) && !assassinRune.TryCollect(w) && assassinRune!=null,
                "Runa de outra classe é recusada e permanece no chão");
            events.Clear();
            Check(assassinRune!=null && assassinRune.TryCollect(a) && a.Inventory.Slots.Any(i=>i.itemId=="Var_Assassin_Travel")
                && events.Any(e=>e.kind==CombatEventKind.ItemPickup&&e.item!=null&&e.item.itemId=="Var_Assassin_Travel"),
                "Botão de coleta adiciona uma runa compatível e emite identificação do item");
            a.Inventory.UseSlot(0); yield return new WaitForSeconds(1.1f);
            Check(a.Abilities.GetEquipped(AbilitySlot.Movement)==Ability("Assassin_Move_A") && a.Inventory.Slots.Count==0,
                "Usar runa coletada muda a habilidade de movimento exibida e consome o item");
            w.Inventory.TryAdd(Item("Var_Warrior_Parry")); var original=w.Abilities.GetEquipped(AbilitySlot.Defense);
            w.Inventory.UseSlot(0); yield return new WaitForSeconds(0.4f);
            bool waiting=w.Abilities.GetEquipped(AbilitySlot.Defense)==original && w.Inventory.Slots.Count==1;
            w.Health.ApplyDamage(1f); yield return new WaitForSeconds(0.2f);
            Check(waiting && !w.Inventory.IsUsingItem && w.Inventory.Slots.Count==1 && w.Abilities.GetEquipped(AbilitySlot.Defense)==original, "Troca de runa aguarda e pode ser interrompida por dano");
            w.Inventory.UseSlot(0); yield return new WaitForSeconds(0.5f);
            bool kept=w.Inventory.Slots.Count==1; yield return new WaitForSeconds(0.55f);
            Check(kept && w.Inventory.Slots.Count==0 && w.Abilities.GetEquipped(AbilitySlot.Defense)==Ability("Warrior_Defense_A"), "Runa só é consumida ao concluir troca de cerca de 0.8 s");
            var pickup=FindObjectsByType<WorldPickup>().FirstOrDefault(p=>p.choiceGroup==null && p.item.kind==ItemKind.Heal);
            if(pickup!=null)
            {
                w.Motor.Teleport(pickup.transform.position+Vector3.up*0.4f+Vector3.back*1.2f);
                w.Motor.SetMoveInput(Vector2.up); yield return new WaitForSeconds(0.3f); w.Motor.SetMoveInput(Vector2.zero);
                bool collectedHeal=w.Inventory.Slots.Any(i=>i.kind==ItemKind.Heal);
                Check(collectedHeal, "Coleta real de poção por colisão entra no inventário");
                w.Health.ApplyDamage(30f); float beforeGroundHeal=w.Health.CurrentHealth;
                if(collectedHeal) w.Inventory.UseSlot(0);
                yield return new WaitForSeconds(1f);
                Check(collectedHeal && w.Health.CurrentHealth>beforeGroundHeal && w.Health.CurrentHealth<beforeGroundHeal+Item("Heal").amount &&
                    !w.Inventory.Slots.Any(i=>i.kind==ItemKind.Heal), "Poção do chão inicia cura progressiva e é consumida");
            }
            else { Check(false,"Coleta real de poção por colisão entra no inventário"); Check(false,"Poção coletada do chão conclui a cura e é consumida"); }

            stage = "Itens táticos: repulsão, barreira, fumaça e campo nulo";
            ResetPair(3f); ap=a.transform.position;
            w.Inventory.TryAdd(Item("Repulsion")); w.Inventory.UseSlot(0);
            yield return new WaitForSeconds(0.6f);
            Check(Vector3.Distance(ap,a.transform.position)>0.5f, "Repulsão desloca o adversário");
            ResetPair(6f); w.Inventory.TryAdd(Item("Barrier")); w.Inventory.UseSlot(0);
            yield return new WaitForSeconds(0.4f); var barrier=GameObject.Find("Barrier");
            bool barrierCollider=barrier!=null && barrier.GetComponent<Collider>().enabled && !barrier.GetComponent<Collider>().isTrigger;
            wp=w.transform.position; w.Motor.SetMoveInput(Vector2.right);
            yield return new WaitForSeconds(0.7f); w.Motor.SetMoveInput(Vector2.zero);
            bool blocked=w.transform.position.x < wp.x+2f;
            yield return new WaitForSeconds(3.6f);
            Check(barrierCollider && blocked && barrier==null, "Barreira bloqueia movimento e expira");
            w.Inventory.TryAdd(Item("Smoke")); w.Inventory.UseSlot(0);
            yield return new WaitForSeconds(0.4f);
            var smoke=FindAnyObjectByType<SmokeField>();
            Check(smoke!=null && smoke.GetComponent<ParticleSystem>()!=null && smoke.ParticleCount>=30,
                "Granada cria nuvem de fumaça suave com volume legível");
            Check(smoke!=null && SmokeField.BlocksSight(w.transform.position+Vector3.up,a.transform.position+Vector3.up),
                "Fumaça bloqueia a linha de visão usada pelo bot");
            ResetPair(6f); w.Inventory.TryAdd(Item("Null")); w.Inventory.UseSlot(0);
            yield return new WaitForSeconds(0.4f);
            var magic=CloneMagic(a.Definition.basicAttack);
            var field=FindAnyObjectByType<NullField>();
            var nullHit=SpawnHit(a,magic,field!=null?field.transform.position:Vector3.up*10f);
            yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            Check(nullHit!=null && nullHit.Cancelled, "Campo nulo cancela ataque nullifiable via física");

            stage = "Airdrop: relógio, três escolhas e coleta real";
            ResetPair(6f);
            foreach(var g in FindObjectsByType<ChoicePickupGroup>()) Destroy(g.gameObject);
            yield return null;
            drop.firstDropDelay=Time.timeSinceLevelLoad+1.5f; drop.enabled=true;
            yield return new WaitForSeconds(0.5f); bool absent=FindObjectsByType<ChoicePickupGroup>().Length==0;
            yield return new WaitForSeconds(1.3f);
            var group=FindAnyObjectByType<ChoicePickupGroup>();
            Check(absent && group!=null && Near(automaticDropDelay,12f), "Airdrop aguarda relógio configurado (12 s padrão; limiar de teste reduzido)");
            var choices=group!=null?group.GetComponentsInChildren<WorldPickup>():Array.Empty<WorldPickup>();
            Check(choices.Length==3, "Airdrop apresenta três opções");
            if(choices.Length>0)
            {
                var choice=choices[0]; choice.item=Item("Heal");
                // Move the test group to clear ground; exercise pickup physics, not Choose directly.
                group.transform.position=Vector3.up*0.5f;
                Vector3 chosenPosition=choice.transform.position;
                w.Motor.Teleport(choice.transform.position+Vector3.up*0.5f+Vector3.back*1.3f);
                a.Motor.Teleport(new Vector3(6,1,0)); w.Motor.SetMoveInput(Vector2.up);
                yield return new WaitForSeconds(0.4f); w.Motor.SetMoveInput(Vector2.zero);
                Check(w.Inventory.Slots.Count==1 && group==null && choices.All(c=>c==null), "Pegar uma escolha elimina as outras duas");
                // Re-enter the selected pickup: a consumed reward must not be collectible twice.
                w.Motor.Teleport(new Vector3(-4,1,-4)); yield return new WaitForFixedUpdate();
                w.Motor.Teleport(chosenPosition+Vector3.up*0.5f);
                yield return new WaitForSeconds(0.2f);
                Check(w.Inventory.Slots.Count==1, "Airdrop não concede a mesma recompensa duas vezes");
            }
            else { Check(false,"Pegar uma escolha elimina as outras duas"); Check(false,"Airdrop não concede a mesma recompensa duas vezes"); }

            stage = "Framework de magia: colisões e cooldown de interação";
            ResetPair(4f); events.Clear(); Physics.SyncTransforms();
            var m1=SpawnHit(w,magic,new Vector3(0,2,0)); var m2=SpawnHit(a,magic,new Vector3(0,2,0));
            yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            Check(m1.Cancelled && m2.Cancelled && events.Any(e=>e.kind==CombatEventKind.Clash)
                && Near(w.Health.MaxHealth-w.Health.CurrentHealth,magic.damage*0.7f)
                && Near(a.Health.MaxHealth-a.Health.CurrentHealth,magic.damage*0.7f), "Magia versus magia explode em área sem dano duplicado");
            yield return new WaitForSeconds(1.1f);
            ResetPair(6f); events.Clear();
            var physical=SpawnHit(w,w.Definition.basicAttack,new Vector3(0,3,0)); var incoming=SpawnHit(a,magic,new Vector3(0,3,0));
            yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            Check(physical.Cancelled && incoming.Cancelled && events.Any(e=>e.kind==CombatEventKind.Nullify), "Ataque físico marcado anula magia via física");
            Check(w.Interactions.Remaining("AttackNullifyMagic")>29f && w.Interactions.Remaining("AttackNullifyMagic")<=30f, "Anulação entra em cooldown separado de 30 s");
            yield return new WaitForSeconds(1.1f);
            physical=SpawnHit(w,w.Definition.basicAttack,new Vector3(0,3,0)); incoming=SpawnHit(a,magic,new Vector3(0,3,0));
            yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            Check(!physical.Cancelled && !incoming.Cancelled, "Durante cooldown novo ataque não anula magia");
            yield return new WaitForSeconds(1.2f);

            yield return TestDefenseAndAI();
            yield return TestWarriorPursuitAndBotModes();
            yield return TestMobileV4();
            yield return TestWarriorRework();
            yield return TestIndependentMobileMotion();
            yield return TestAssassinRework();
            yield return TestCombatLab001();
            yield return TestProductionV1();
            yield return TestVariationPresentation();

            stage = "Resultado da partida";
            ResetPair(1.4f); a.Health.ApplyDamage(a.Health.MaxHealth-1f); w.Abilities.TryUse(AbilitySlot.BasicAttack);
            yield return new WaitForSeconds(0.7f);
            Check(a.Health.IsDead && FindObjectsByType<UnityEngine.UI.Text>().Any(t=>t.isActiveAndEnabled && t.text.Contains("GUERREIRO VENCEU")), "Golpe final gera vitória do Guerreiro na HUD");
            Check(Count(CombatEventKind.Death,a)==1 &&
                FindObjectsByType<ParticleSystem>().Any(p=>p.name=="VFX_DeathBurst") &&
                FindObjectsByType<LineRenderer>().Any(l=>l.name=="VFX_BloodPool"),
                "Golpe fatal emite morte uma vez, explosão de sangue e marca no chão");
            Animator defeated = a.GetComponentInChildren<Animator>(true);
            AnimatorStateInfo defeatedCurrent = defeated.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo defeatedNext = defeated.GetNextAnimatorStateInfo(0);
            Check(defeated.GetBool("Dead") && (defeatedCurrent.IsName("Death") || defeatedNext.IsName("Death")),
                "Morte visual permanece ativa no mobile até o reinício da rodada");
            Check(runtimeErrors==0,"Nenhum erro ou exceção durante a execução");
            stage="CONCLUÍDO — "+passed+" passaram / "+failed+" falharam"; finished=true;
            File.AppendAllText(reportPath,"\nResultado: "+passed+" passaram; "+failed+" falharam.\n");
            Debug.Log("[BRX LIVE FINAL] pass="+passed+" fail="+failed+" report="+reportPath);
            Time.captureDeltaTime = previousCaptureDelta; ownsCaptureStep = false;
            foreach(var c in new[]{w,a}) c.GetComponent<PrototypeLocalInput>().enabled=true;
        }

        IEnumerator TestDefenseAndAI()
        {
            stage = "V3: respostas defensivas e inteligência do bot";
            foreach (string id in new[] { "Warrior_Defense_Base", "Warrior_Defense_A", "Warrior_Defense_B" })
            {
                ResetPair(1.4f); events.Clear();
                var defense = Ability(id);
                w.Abilities.EquipVariation(defense);
                w.Abilities.TryUse(AbilitySlot.Defense);
                yield return new WaitForSeconds(defense.startup+0.04f);
                bool noFreeDamage = Near(a.Health.CurrentHealth, a.Health.MaxHealth);
                var attack = SpawnHit(a, a.Definition.basicAttack, w.transform.position + Vector3.up);
                attack.TryResolveHurtbox(w.GetComponentInChildren<Hurtbox>());
                bool capture = defense.behavior==AbilityBehavior.WarriorSkillCapture;
                Check(noFreeDamage && !w.Abilities.HasCounterOpportunity && Near(a.Health.CurrentHealth, a.Health.MaxHealth) &&
                    (capture ? w.Health.CurrentHealth<w.Health.MaxHealth && !w.Abilities.HasCapturedSkill : Near(w.Health.CurrentHealth,w.Health.MaxHealth)),
                    id + ": defesa não causa dano gratuito; captura exclui básico e outras anulam");
                attack.Cancel(); Destroy(attack.gameObject);
                yield return new WaitForSeconds(0.2f);
                Vector3 before = a.transform.position;
                bool used = w.Abilities.TryUse(AbilitySlot.BasicAttack, Vector3.right);
                yield return new WaitForSeconds(0.55f);
                Check(used && Vector3.Distance(a.transform.position,before)<0.04f &&
                    Near(a.Health.MaxHealth - a.Health.CurrentHealth, w.Definition.basicAttack.damage) &&
                    !a.State.InputLocked && Count(CombatEventKind.CounterHit, a) == 0,
                    id + ": básico após defesa dá impacto sem empurrar nem counter legado");
            }

            ResetPair(1.4f); events.Clear(); w.Abilities.EquipVariation(GenericParryFixture()); w.Abilities.GrantCounterOpportunity();
            a.Defense.Activate(DefenseKind.Guard, 1f, 0f, 0.75f, 0f);
            Vector3 blockedStart = a.transform.position;
            w.Abilities.TryUse(AbilitySlot.BasicAttack, Vector3.right);
            yield return new WaitForSeconds(0.6f);
            Check(Near(a.Health.MaxHealth - a.Health.CurrentHealth, 19f * 0.25f) &&
                a.transform.position.x - blockedStart.x < 1.3f && Count(CombatEventKind.CounterHit,a) == 0,
                "Counter também pode ser bloqueado: dano e empurrão reduzidos");

            ResetPair(4f); w.Abilities.EquipVariation(GenericParryFixture()); w.Abilities.GrantCounterOpportunity();
            yield return new WaitForSeconds(1.3f);
            Check(!w.Abilities.HasCounterOpportunity, "Oportunidade de counter expira sem ataque automático");

            foreach (string id in new[] { "Assassin_Defense_Base" })
            {
                ResetPair(1.4f); events.Clear();
                var defense = Ability(id); a.Abilities.EquipVariation(defense);
                a.Abilities.TryUse(AbilitySlot.Defense, Vector3.forward);
                float limit = Time.time + 0.5f;
                while (Time.time < limit && !a.Motor.IsDashing && !a.Defense.IsActive) yield return null;
                var attack = SpawnHit(w, w.Definition.basicAttack, a.transform.position + Vector3.up);
                attack.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>());
                bool negated = Near(a.Health.CurrentHealth, a.Health.MaxHealth);
                if (defense.behavior == AbilityBehavior.Dodge)
                {
                    var repeated = SpawnHit(w,w.Definition.basicAttack,a.transform.position+Vector3.up);
                    repeated.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>());
                    repeated.Cancel(); Destroy(repeated.gameObject);
                }
                attack.Cancel(); Destroy(attack.gameObject);
                yield return new WaitForSeconds(1.15f);
                Check(negated && events.Count(e => e.kind == CombatEventKind.DefenseRedirect && e.source == a) == 1 &&
                    a.transform.position.x < w.transform.position.x - 0.45f,
                    id + ": defesa no contato muda direção e atravessa o atacante");
                Check(Near(w.Health.MaxHealth - w.Health.CurrentHealth, defense.damage) && defense.damage <= 3f,
                    id + ": travessia defensiva causa só 2–3 de dano uma vez");
                Check(!a.State.IsInvulnerable && !a.Motor.IsDashing && !a.Abilities.IsActionBusy,
                    id + ": viagem encerra sem invulnerabilidade ou ação presa");
            }

            ResetPair(2f); events.Clear();
            var barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrier.name = "Test_DefenseWall"; barrier.transform.position = new Vector3(0f,1.5f,0f);
            barrier.transform.localScale = new Vector3(0.4f,3f,6f); Physics.SyncTransforms();
            a.Abilities.TryUse(AbilitySlot.Defense, Vector3.left);
            yield return new WaitForSeconds(0.5f);
            Check(a.transform.position.x > 0f && Near(w.Health.CurrentHealth,w.Health.MaxHealth),
                "Esquiva defensiva respeita paredes e não causa dano atrás delas");
            Destroy(barrier); yield return null;

            // Observe actual cast events with a real bot; do not assert a perfect parry or make random outcomes mandatory.
            ResetPair(7f);
            var bot = w.GetComponent<PrototypeTrainingBot>();
            bot.runInEditor = true; bot.enabled = true;
            Vector3 pursuitStart = w.transform.position;
            yield return new WaitForSeconds(1.2f);
            Check(Vector3.Distance(pursuitStart,w.transform.position)>1.5f,
                "Bot se aproxima por movimento e investida usando APIs normais");
            bot.enabled = false;
            ResetPair(2.4f); bot.enabled = true;
            int observedBefore = bot.ObservedSkills, responsesBefore = bot.DefensiveResponses;
            // A 0.16-second live dash can already be over before the honest 0.22-second reaction.
            // Use an actual cast with a deliberately readable telegraph to test observation/reaction,
            // without requiring the bot to defend a threat that has already passed behind it.
            var telegraphed = Instantiate(a.Definition.ultimateBase); temporary.Add(telegraphed);
            telegraphed.startup = 0.65f; a.Abilities.EquipVariation(telegraphed);
            a.Abilities.TryUse(AbilitySlot.Ultimate,Vector3.left);
            yield return new WaitForSeconds(0.10f);
            Check(bot.ObservedSkills > observedBefore && bot.DefensiveResponses == responsesBefore,
                "Bot observa skill real, mas não reage antes do atraso humano");
            yield return new WaitForSeconds(0.38f);
            string probeFields = "";
            foreach (var name in new[] { "reactAt", "threatUntil", "responseMade", "threatOrigin", "threatDirection", "observed", "nextDecisionAt", "enemyGuardUntil" })
                probeFields += " " + name + "=" + typeof(PrototypeTrainingBot).GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(bot);
            Debug.Log($"[BRX BOT REACTION PROBE] now={Time.time} before={responsesBefore} responses={bot.DefensiveResponses} observedAt={bot.LastObservedAt} reactedAt={bot.LastReactionAt} intent={bot.CurrentIntent} wPos={w.transform.position} aPos={a.transform.position} busy={w.Abilities.IsActionBusy} dash={w.Motor.IsDashing} locked={w.State.InputLocked}" + probeFields);
            Check(bot.DefensiveResponses > responsesBefore && bot.LastReactionAt - bot.LastObservedAt >= 0.19f,
                "Bot identifica cast com preparação de 0,65 s e reage após pelo menos 0,20 s");
            bot.enabled = false;

            ResetPair(2.2f); bot.enabled = true;
            var smoke = SmokeField.Spawn(Vector3.zero,3.5f,2f);
            int hiddenBefore = bot.ObservedSkills;
            a.Abilities.TryUse(AbilitySlot.BasicAttack,Vector3.left);
            yield return new WaitForSeconds(0.5f);
            Check(bot.CurrentIntent == PrototypeTrainingBot.Intent.Search && bot.ObservedSkills == hiddenBefore,
                "Bot não lê habilidades inimigas através da fumaça");
            bot.enabled = false; Destroy(smoke.gameObject); yield return null;

            ResetPair(4f);
            var sightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sightWall.name = "Test_BotSightWall";
            sightWall.transform.position = new Vector3(0f,1.5f,0f);
            sightWall.transform.localScale = new Vector3(0.5f,3f,3f); Physics.SyncTransforms();
            bot.enabled = true;
            int behindWall = bot.ObservedSkills;
            Vector3 searchStart = w.transform.position;
            a.Abilities.TryUse(AbilitySlot.BasicAttack,Vector3.left);
            yield return new WaitForSeconds(0.6f);
            Check(bot.ObservedSkills == behindWall && Vector3.Distance(w.transform.position,searchStart)>0.2f,
                "Bot busca nova posição sem ler skill através da parede");
            bot.enabled = false; Destroy(sightWall); yield return null;

            ResetPair(2.2f); bot.enabled = true;
            a.Abilities.EquipVariation(Ability("Assassin_Defense_A"));
            a.Abilities.TryUse(AbilitySlot.Defense);
            yield return new WaitForSeconds(0.25f);
            Check(bot.ObservedSkills > hiddenBefore, "Bot reconhece uso público de variação defensiva");
            bot.enabled = false;
            ResetPair(3f); a.Abilities.TryUse(AbilitySlot.Defense,Vector3.forward);
            yield return new WaitForSeconds(0.07f);
            a.ResetTransientState();
            Check(!a.Motor.IsDashing && !a.State.IsInvulnerable && !a.Abilities.IsActionBusy,
                "Reset cancela viagem, iframe e efeitos defensivos transitórios");
        }

        IEnumerator TestWarriorPursuitAndBotModes()
        {
            stage = "Laboratório: perseguição do Guerreiro e modos determinísticos do bot";
            var bot = w.GetComponent<PrototypeTrainingBot>();
            bot.runInEditor = true;

            ResetPair(1.4f); events.Clear();
            bot.SetMode(PrototypeTrainingBot.TrainingMode.Stationary);
            bot.enabled = true;
            Vector3 stationaryStart = w.transform.position;
            float healthBefore = a.Health.CurrentHealth;
            yield return new WaitForSeconds(1.2f);
            Check(Vector3.Distance(stationaryStart,w.transform.position)<0.03f && Near(healthBefore,a.Health.CurrentHealth),
                "Bot Parado não anda nem ataca");

            bot.SetMode(PrototypeTrainingBot.TrainingMode.StationaryAttack);
            stationaryStart = w.transform.position;
            yield return new WaitForSeconds(2.2f);
            Check(Vector3.Distance(stationaryStart,w.transform.position)<0.03f && a.Health.CurrentHealth<healthBefore,
                "Bot Parado + ataque golpeia em alcance sem deslizar");
            bot.enabled = false;
            bot.SetMode(PrototypeTrainingBot.TrainingMode.Normal);

            foreach (string id in new[] { "Warrior_Move_Base", "Warrior_Move_A", "Warrior_Move_B" })
            {
                ResetPair(id=="Warrior_Move_A" ? 4f : 6f); events.Clear();
                AbilityDefinition pursuit = Ability(id);
                w.Abilities.EquipLabVariation(pursuit);
                a.Motor.Teleport(new Vector3(id=="Warrior_Move_A" ? 2f : 3f,1f,id=="Warrior_Move_Base" ? 0f : 1.4f));
                a.Motor.FaceDirection(Vector3.forward);
                Physics.SyncTransforms();
                Vector3 warriorStart = w.transform.position;
                if(id!="Warrior_Move_Base") a.Motor.SetMoveInput(Vector2.up);
                bool used = w.Abilities.TryUse(AbilitySlot.Movement,Vector3.right);
                yield return new WaitForSeconds(1.1f);
                a.Motor.StopMovementImmediately();
                // Moving target can escape the short pursuit's delayed extra strike; a stationary fixture below proves its bonus.
                float expectedDamage=pursuit.damage;
                Debug.Log($"[BRX PURSUIT004 PROBE] id={id} used={used} damage={a.Health.MaxHealth-a.Health.CurrentHealth} expected={expectedDamage} hits={Count(CombatEventKind.Hit,a)} w={w.transform.position} a={a.transform.position} gap={Vector3.Distance(w.transform.position,a.transform.position)} targetMove={a.MovementSlowMultiplier} busy={w.Abilities.IsActionBusy}");
                Check(used && Near(a.Health.MaxHealth-a.Health.CurrentHealth,expectedDamage,0.08f) && Count(CombatEventKind.Hit,a)==1,
                    id+": arremesso ou perseguição acerta uma vez; alvo em fuga pode escapar do golpe extra");
                Check(id=="Warrior_Move_Base" ? Vector3.Distance(w.transform.position,warriorStart)<0.03f : w.transform.position.z>warriorStart.z+0.25f,
                    id+": arremesso não move; perseguições corrigem direção");
            }

            ResetPair(4f); events.Clear();
            AbilityDefinition impact = Ability("Warrior_Move_Base");
            w.Abilities.EquipLabVariation(impact);
            Vector3 targetBefore = a.transform.position;
            w.Abilities.TryUse(AbilitySlot.Movement,Vector3.right);
            yield return new WaitForSeconds(0.9f);
            Check(a.transform.position.x-targetBefore.x>impact.knockback-0.45f,
                "Impacto do Guerreiro joga o inimigo para trás pela distância configurada");

            ResetPair(6f); events.Clear();
            w.Abilities.EquipLabVariation(Ability("Warrior_Move_A"));
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name="Test_WarriorPursuitWall";
            wall.transform.position=new Vector3(0f,1.5f,0f);
            wall.transform.localScale=new Vector3(0.5f,3f,5f);
            Physics.SyncTransforms();
            w.Abilities.TryUse(AbilitySlot.Movement,Vector3.right);
            yield return new WaitForSeconds(0.8f);
            Check(w.transform.position.x<0f && Near(a.Health.CurrentHealth,a.Health.MaxHealth),
                "Perseguição do Guerreiro respeita parede e não causa dano remoto");
            Destroy(wall); yield return null;
            bot.SetMode(PrototypeTrainingBot.TrainingMode.Normal);
            bot.enabled = false;
            ResetPair(5f);
        }

        IEnumerator TestVariationPresentation()
        {
            stage = "V3: efeitos das 18 habilidades e variações";
            var presenter = FindAnyObjectByType<CombatEventVfxPresenter>();
            string[] ids = { "Warrior_Defense_Base", "Warrior_Defense_A", "Warrior_Defense_B", "Warrior_Move_Base", "Warrior_Move_A", "Warrior_Move_B",
                "Warrior_Ult_Base", "Warrior_Ult_A", "Warrior_Ult_B", "Assassin_Defense_Base", "Assassin_Defense_A", "Assassin_Defense_B",
                "Assassin_Move_Base", "Assassin_Move_A", "Assassin_Move_B", "Assassin_Ult_Base", "Assassin_Ult_A", "Assassin_Ult_B" };
            foreach (string id in ids)
            {
                ResetPair(5f); events.Clear();
                yield return null;
                var ability = Ability(id);
                var actor = ability.requiredClass == CharacterClass.Warrior ? w : a;
                bool equipped = actor.Abilities.EquipVariation(ability);
                Check(equipped && actor.Abilities.GetEquipped(ability.slot) == ability,
                    id + ": variação está disponível e equipa no slot correto");
                actor.Abilities.TryUse(ability.slot, Vector3.forward);
                yield return new WaitForSeconds(0.65f);
            }
            Check(presenter.transform.childCount <= 70, "Efeitos das variações respeitam o limite compartilhado de 70 objetos");

            ResetPair(2.6f);
            var camera = Camera.main;
            var rig = camera.GetComponent<IsometricCameraRig>(); rig.enabled = false;
            camera.transform.position = new Vector3(5f,6.8f,-7f);
            camera.transform.LookAt(new Vector3(0f,0.6f,0f));
            w.Abilities.EquipVariation(Ability("Warrior_Defense_B"));
            a.Abilities.EquipVariation(Ability("Assassin_Defense_A"));
            w.Abilities.TryUse(AbilitySlot.Defense); a.Abilities.TryUse(AbilitySlot.Defense);
            yield return new WaitForSeconds(0.085f);
            ScreenCapture.CaptureScreenshot(Path.GetFullPath("Logs/defense-ai-guards.png"));
            yield return new WaitForSeconds(0.3f);
            rig.enabled = true;
        }

        void Record(CombatEventData data) => events.Add(data);
        void OnLog(string message,string stack,LogType type)
        {
            if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert) runtimeErrors++;
        }
        int Count(CombatEventKind kind,CharacterRuntime target)=>events.Count(e=>e.kind==kind&&e.target==target);
        static bool Near(float x,float y,float epsilon=0.03f)=>Mathf.Abs(x-y)<epsilon;
        static bool Identity(RuntimeModifiers m)=>Near(m.damageMultiplier,1)&&Near(m.moveSpeedMultiplier,1)&&Near(m.staggerResistanceMultiplier,1)&&Near(m.defenseWindowMultiplier,1)&&Near(m.movementCooldownMultiplier,1);
        static AbilityDefinition Ability(string id)=>AssetDatabase.LoadAssetAtPath<AbilityDefinition>("Assets/BattleRoyaleX/GeneratedData/Abilities/"+id+".asset");
        static ItemDefinition Item(string id)=>AssetDatabase.LoadAssetAtPath<ItemDefinition>("Assets/BattleRoyaleX/GeneratedData/Items/"+id+".asset");
        AbilityDefinition CloneMagic(AbilityDefinition source)
        {
            var copy=Instantiate(source); copy.attackKind=AttackKind.Projectile; copy.nullifiable=true; copy.damage=10; copy.explosionRadius=3f;
            temporary.Add(copy); return copy;
        }
        Hitbox SpawnHit(CharacterRuntime owner,AbilityDefinition definition,Vector3 position)
        {
            var go=new GameObject("Live_Test_Hitbox"); go.transform.position=position;
            var h=go.AddComponent<Hitbox>(); h.Configure(owner,new DamagePacket(owner,definition,Vector3.forward),Vector3.one,1f); return h;
        }
        void ResetPair(float separation)
        {
            SmokeField.ClearAll();
            foreach(var hit in FindObjectsByType<Hitbox>()) { hit.Cancel(); Destroy(hit.gameObject); }
            foreach(var c in new[]{w,a}) { c.Initialize(c.Definition); c.Motor.StopMovementImmediately(); }
            w.Motor.Teleport(new Vector3(-separation/2f,1,0)); a.Motor.Teleport(new Vector3(separation/2f,1,0));
            w.Motor.FaceDirection(Vector3.right); a.Motor.FaceDirection(Vector3.left); Physics.SyncTransforms();
        }
        static bool FullBodyVisible(CharacterRuntime c)
        {
            var camera=Camera.main;
            foreach(var renderer in c.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var mesh=new Mesh(); renderer.BakeMesh(mesh);
                foreach(var vertex in mesh.vertices)
                {
                    var p=camera.WorldToViewportPoint(renderer.transform.TransformPoint(vertex));
                    if(p.z<=0||p.x<0.02f||p.x>0.98f||p.y<0.02f||p.y>0.98f) { Destroy(mesh); return false; }
                }
                Destroy(mesh);
            }
            return true;
        }
        void Check(bool ok,string name)
        {
            if(ok) passed++; else failed++;
            last=(ok?"PASSOU: ":"FALHOU: ")+name;
            results.Add(last); File.AppendAllText(reportPath,"- "+last+"\n");
            Debug.Log("[BRX LIVE "+(ok?"PASS":"FAIL")+"] "+name);
        }
        void OnGUI()
        {
            var style=new GUIStyle(GUI.skin.box) { fontSize=18, alignment=TextAnchor.MiddleCenter, wordWrap=true };
            GUI.Box(new Rect(10,Screen.height-112,Screen.width-20,84),stage+"\n"+passed+" passaram | "+failed+" falharam\n"+last,style);
        }
        void OnDestroy()
        {
            if (ownsCaptureStep) { Time.captureDeltaTime = previousCaptureDelta; ownsCaptureStep = false; }
            CombatEvents.Raised-=Record;
            Application.logMessageReceived-=OnLog;
            foreach(var obj in temporary) if(obj!=null) Destroy(obj);
            if(!finished&&!string.IsNullOrEmpty(reportPath)) File.AppendAllText(reportPath,"\nExecução interrompida; casos restantes não executados.\n");
        }
    }
}
#endif
