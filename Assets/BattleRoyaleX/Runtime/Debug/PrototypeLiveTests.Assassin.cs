#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed partial class PrototypeLiveTests
    {
        IEnumerator TestAssassinRework()
        {
            stage = "Assassino: execução, adaga, órbita e códigos de interação";
            var execution = Ability("Assassin_Ult_A");
            ResetPair(6f); events.Clear(); a.Abilities.EquipVariation(execution);
            bool started = a.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(0.12f);
            w.Motor.FaceDirection(Vector3.forward); w.Motor.Dash(3f, 0.22f, false, 0f);
            yield return new WaitForSeconds(0.75f);
            Check(started && Count(CombatEventKind.Hit,w)==1 && Near(w.Health.MaxHealth-w.Health.CurrentHealth,execution.damage),
                "Execução segue alvo que avança lateralmente e golpeia uma única vez");
            Check(a.Abilities.IsExecutionHidden && !a.State.IsInvulnerable && Identity(a.Modifiers),
                "Execução oculta após golpe por 2 s, sem imunidade nem buff legado");
            SmokeVisibility.LocalPlayer=a; yield return null; yield return null;
            Check(a.GetComponentInChildren<SkinnedMeshRenderer>().enabled,"Invisibilidade mantém silhueta legível ao próprio jogador");
            SmokeVisibility.LocalPlayer=w; yield return null; yield return null;
            Check(!a.GetComponentInChildren<SkinnedMeshRenderer>().enabled,"Execução esconde modelo da perspectiva adversária");
            var attack = SpawnHit(w,w.Definition.basicAttack,a.transform.position+Vector3.up);
            attack.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>()); attack.Cancel(); Destroy(attack.gameObject);
            Check(a.Health.CurrentHealth<a.Health.MaxHealth,"Golpe direcionado à posição acerta Assassino invisível");
            yield return new WaitForSeconds(2.1f);
            Check(!a.Abilities.IsExecutionHidden && a.GetComponentInChildren<SkinnedMeshRenderer>().enabled,"Execução restaura visibilidade após 2 segundos");
            SmokeVisibility.LocalPlayer=null;
            ResetPair(30f); a.Abilities.EquipVariation(execution); float energy=a.Energy.CurrentEnergy;
            Check(!a.Abilities.TryUse(AbilitySlot.Ultimate) && Near(energy,a.Energy.CurrentEnergy),"Execução sem alvo em alcance não gasta energia");
            ResetPair(8f); a.Abilities.EquipVariation(execution); a.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(0.12f);
            var wall=GameObject.CreatePrimitive(PrimitiveType.Cube); wall.transform.position=new Vector3(0,1.5f,0);
            wall.transform.localScale=new Vector3(0.5f,3f,12f); Physics.SyncTransforms();
            yield return new WaitForSeconds(1.7f);
            Check(Near(w.Health.CurrentHealth,w.Health.MaxHealth) && !a.Abilities.IsExecutionHidden && !a.Abilities.IsActionBusy,
                "Parede limita Execução: sem dano remoto nem invisibilidade gratuita");
            Destroy(wall); yield return null;

            var travel=Ability("Assassin_Move_A");
            ResetPair(5f); events.Clear(); a.Abilities.EquipVariation(travel); Vector3 original=a.transform.position;
            bool thrown=a.Abilities.TryUse(AbilitySlot.Movement,Vector3.left);
            yield return new WaitForSeconds(0.5f);
            Check(thrown && Near(w.Health.MaxHealth-w.Health.CurrentHealth,travel.damage) && Vector3.Distance(original,a.transform.position)<0.05f,
                "Travessia lança adaga, causa 4 de dano único e não avança no primeiro toque");
            energy=a.Energy.CurrentEnergy; float cooldown=a.Abilities.GetCooldownRemaining(AbilitySlot.Movement);
            bool teleport=a.Abilities.TryUse(AbilitySlot.Movement);
            float extraCost=energy-a.Energy.CurrentEnergy;
            yield return new WaitForSeconds(0.2f);
            Check(teleport && Near(extraCost,0f) && Near(w.Health.MaxHealth-w.Health.CurrentHealth,travel.damage+travel.secondStrikeDamage) &&
                Count(CombatEventKind.Hit,w)==2,"Segundo toque teleporta e golpeia por 12, sem repetir dano nem custo de energia");
            Check(!a.Abilities.CanRecast(AbilitySlot.Movement) && !a.Abilities.TryUse(AbilitySlot.Movement) &&
                a.Abilities.GetCooldownRemaining(AbilitySlot.Movement)<cooldown,"Adaga permite um só teleporte e não reinicia recarga");
            ResetPair(20f); a.Abilities.EquipVariation(travel); a.Abilities.TryUse(AbilitySlot.Movement,Vector3.forward);
            yield return new WaitForSeconds(3.3f);
            Check(!a.Abilities.CanRecast(AbilitySlot.Movement) && FindObjectsByType<AssassinDaggerAnchor>().Length==0,
                "Janela de 3 s expira e remove adaga sem teleporte automático");
            ResetPair(5f); a.Abilities.EquipVariation(travel); a.Abilities.TryUse(AbilitySlot.Movement);
            yield return new WaitForSeconds(0.3f); a.Abilities.EquipVariation(Ability("Assassin_Move_Base")); yield return null;
            Check(FindObjectsByType<AssassinDaggerAnchor>().Length==0 && !a.Abilities.CanRecast(AbilitySlot.Movement),
                "Troca de variante cancela a adaga antiga e sua reativação");
            ResetPair(6f); a.Abilities.EquipVariation(travel);
            wall=GameObject.CreatePrimitive(PrimitiveType.Cube); wall.transform.position=new Vector3(0,1.5f,0);
            wall.transform.localScale=new Vector3(0.5f,3f,12f); Physics.SyncTransforms();
            a.Abilities.TryUse(AbilitySlot.Movement,Vector3.left); yield return new WaitForSeconds(0.5f);
            bool stoppedTeleport=a.Abilities.TryUse(AbilitySlot.Movement); yield return new WaitForSeconds(0.2f);
            Check(stoppedTeleport && a.transform.position.x>0.5f && Near(w.Health.CurrentHealth,w.Health.MaxHealth),
                "Adaga para na parede e teleporta somente ao lado livre, sem dano atrás do cenário");
            Destroy(wall); yield return null;
            ResetPair(6f); a.Abilities.EquipVariation(travel);
            var phase=ArcaneDemoObject.Create("AssassinTestPhaseWall",ArcaneDemoKind.PhaseWall,
                new Vector3(0,1.8f,0),new Vector3(0.8f,2f,4f)); Physics.SyncTransforms();
            a.Abilities.TryUse(AbilitySlot.Movement,Vector3.left); yield return new WaitForSeconds(0.8f);
            Check(!a.Abilities.CanRecast(AbilitySlot.Movement) && FindObjectsByType<AssassinDaggerAnchor>().Length==0,
                "Parede de fase I14 cancela adaga e teleporte usando a interação existente");
            Destroy(phase.gameObject); yield return null;

            var orbit=Ability("Assassin_Defense_B");
            ResetPair(1.4f); events.Clear(); a.Abilities.EquipVariation(orbit); original=a.transform.position;
            a.Abilities.TryUse(AbilitySlot.Defense); yield return new WaitForSeconds(0.25f);
            var orbitVisual=a.GetComponentsInChildren<AssassinDaggersPresentation>().FirstOrDefault();
            Check(a.Abilities.HasOrbitingDaggers && !a.State.IsInvulnerable && Vector3.Distance(original,a.transform.position)<0.05f &&
                orbitVisual!=null && orbitVisual.GetComponentsInChildren<Renderer>().Length==10,
                "Círculo possui cinco adagas com rastros, sem dash nem invulnerabilidade garantida");
            yield return new WaitForSeconds(3f);
            Check(Count(CombatEventKind.Hit,w)==6 && Near(w.Health.MaxHealth-w.Health.CurrentHealth,orbit.damage*6f),
                "Órbita aplica seis pulsos de 2 por 3 s sem duplicar por collider");
            Check(!a.Abilities.HasOrbitingDaggers && a.GetComponentsInChildren<AssassinDaggersPresentation>().Length==0,
                "Órbita expira e remove as cinco adagas");
            ResetPair(15f); a.Abilities.EquipVariation(orbit); a.Abilities.TryUse(AbilitySlot.Defense);
            yield return new WaitForSeconds(0.15f);
            var randomState=Random.state; Random.InitState(4273); int expectedRepels=0;
            for(int i=0;i<40;i++) if(Random.value<0.5f) expectedRepels++;
            Random.InitState(4273); events.Clear();
            for(int i=0;i<40;i++)
            {
                var hit=SpawnHit(w,w.Definition.basicAttack,a.transform.position+Vector3.up);
                var packet=hit.Packet; packet.damage=1f; packet.knockback=0f; packet.attackKind=(AttackKind)(i%4); hit.UpdatePacket(packet);
                hit.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>());
                hit.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>()); hit.Cancel(); Destroy(hit.gameObject);
            }
            Random.state=randomState;
            Check(expectedRepels>0 && expectedRepels<40 && Count(CombatEventKind.Dodge,a)==expectedRepels &&
                Near(a.Health.MaxHealth-a.Health.CurrentHealth,40-expectedRepels),
                "Defesa orbital usa exatamente 50% por golpe físico/projétil/área/magia e não sorteia collider duplicado");
            a.Abilities.ResetTransientState(); yield return null;
            Check(!a.Abilities.HasOrbitingDaggers && !a.Abilities.IsExecutionHidden && !a.Abilities.CanRecast(AbilitySlot.Movement),
                "Reset limpa órbita, invisibilidade e janela de adaga");

            var rule=ScriptableObject.CreateInstance<DefenseInteractionRule>(); temporary.Add(rule);
            rule.code=DefenseInteractionRule.RuleCode.All;
            rule.excludedAbilityIds=new[]{w.Definition.basicAttack.abilityId};
            Check(rule.Allows(w.Definition.basicAttack),"Regra código 1 aceita todos, independentemente da lista de exceções");
            rule.code=DefenseInteractionRule.RuleCode.AllExcept;
            Check(!rule.Allows(w.Definition.basicAttack) && rule.Allows(travel),"Regra código 2 exclui somente IDs definidos, não a categoria ultimate");
            var testOrbit=Instantiate(orbit); temporary.Add(testOrbit); testOrbit.defenseInteractionRule=rule; testOrbit.repelChance=1f;
            ResetPair(15f); a.Abilities.EquipVariation(testOrbit); a.Abilities.TryUse(AbilitySlot.Defense); yield return new WaitForSeconds(0.15f);
            var excluded=SpawnHit(w,w.Definition.basicAttack,a.transform.position+Vector3.up);
            excluded.TryResolveHurtbox(a.GetComponentInChildren<Hurtbox>()); excluded.Cancel(); Destroy(excluded.gameObject);
            Check(Near(a.Health.MaxHealth-a.Health.CurrentHealth,w.Definition.basicAttack.damage),
                "Exceção código 2 atravessa defesa orbital mesmo com chance de repelir em 100% no teste");
            float hp=a.Health.CurrentHealth;
            CombatResolver.ResolveAreaEffect(a.transform.position,2f,7f,w,Ability("Mage_Basic"));
            Check(Near(a.Health.CurrentHealth,hp),"Órbita defende também área direta sem hitbox, preservando defesa antiga das outras classes");
            var clashA=SpawnHit(w,Ability("Mage_Basic"),a.transform.position);
            var clashB=SpawnHit(a,Ability("Mage_Basic"),a.transform.position);
            CombatResolver.ResolveHitboxInteraction(clashA,clashB);
            Check(clashA.Cancelled && clashB.Cancelled && Near(a.Health.CurrentHealth,hp),
                "Órbita pode repelir explosão I07 sem impedir choque mágico ou seu cancelamento");
            Destroy(clashA.gameObject); Destroy(clashB.gameObject);
            rule.excludedAbilityIds=new[]{"Mage_Basic"};
            CombatResolver.ResolveAreaEffect(a.transform.position,2f,7f,w,Ability("Mage_Basic"));
            Check(Near(a.Health.CurrentHealth,hp-7f),"Área direta respeita exceção do perfil R02, sem exclusão global de ultimates");
            a.Abilities.EquipVariation(Ability("Assassin_Defense_Base")); yield return null;
            Check(!a.Abilities.HasOrbitingDaggers,"Troca de defesa não mantém órbita escondida da variante antiga");
            ResetPair(6f);
        }
    }
}
#endif
