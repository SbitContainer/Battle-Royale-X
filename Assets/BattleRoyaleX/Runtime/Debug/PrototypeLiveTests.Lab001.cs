#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed partial class PrototypeLiveTests
    {
        IEnumerator TestCombatLab001()
        {
            stage = "BRX-LAB-001: locks independentes e prioridade de ações";
            ResetPair(8f);
            w.State.ApplySkillLock(0.35f);
            Vector3 before = w.transform.position;
            w.Motor.SetMoveInput(Vector2.up);
            bool skillBlocked = !w.Abilities.TryUse(AbilitySlot.Defense);
            yield return new WaitForSeconds(0.2f);
            w.Motor.SetMoveInput(Vector2.zero);
            Check(skillBlocked && w.transform.position.z > before.z + 0.25f, "SkillLock bloqueia skill sem impedir movimento");

            ResetPair(8f);
            before = w.transform.position;
            w.State.ApplyMovementLock(0.3f);
            w.Motor.SetMoveInput(Vector2.up);
            bool skillDuringMovementLock = w.Abilities.TryUse(AbilitySlot.Defense);
            yield return new WaitForSeconds(0.15f);
            w.Motor.SetMoveInput(Vector2.zero);
            Check(skillDuringMovementLock && Near(w.transform.position.z,before.z,0.08f), "MovementLock não bloqueia skill e impede apenas deslocamento");

            ResetPair(6f);
            bool basicStartup = w.Abilities.TryUse(AbilitySlot.BasicAttack);
            bool defenseSameInput = w.Abilities.TryUse(AbilitySlot.Defense);
            yield return new WaitForSeconds(0.45f);
            Check(basicStartup && defenseSameInput && Near(a.Health.CurrentHealth,a.Health.MaxHealth), "Defesa cancela basic em startup e inicia no mesmo input");

            ResetPair(8f);
            w.Abilities.TryUse(AbilitySlot.BasicAttack);
            yield return new WaitForSeconds(0.17f);
            bool movementFromActive = w.Abilities.TryUse(AbilitySlot.Movement);
            bool oldBasicAlive = FindObjectsByType<Hitbox>().Any(h=>h.Owner==w && !h.Cancelled && h.Packet.ability!=null && h.Packet.ability.slot==AbilitySlot.BasicAttack);
            Check(movementFromActive && !oldBasicAlive, "Movimento cancela basic ativo sem manter hitbox fantasma");
            yield return new WaitForSeconds(0.5f);

            ResetPair(8f);
            w.Abilities.TryUse(AbilitySlot.BasicAttack);
            yield return new WaitForSeconds(0.31f);
            before = w.transform.position;
            w.Motor.SetMoveInput(Vector2.up);
            bool ultimateFromRecovery = w.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(0.15f);
            w.Motor.SetMoveInput(Vector2.zero);
            Check(ultimateFromRecovery && w.transform.position.z>before.z+0.15f, "Ultimate assume prioridade no recovery do basic e recovery não congela movimento");

            ResetPair(8f);
            bool skillStarted=w.Abilities.TryUse(AbilitySlot.Movement);
            bool basicRejected=!w.Abilities.TryUse(AbilitySlot.BasicAttack);
            Check(skillStarted && basicRejected, "Basic não inicia durante skill incompatível");
            yield return new WaitForSeconds(0.6f);

            stage = "BRX-LAB-001: micro SkillLock, parry e clash";
            ResetPair(1.4f);
            w.Abilities.TryUse(AbilitySlot.BasicAttack);
            yield return new WaitForSeconds(0.24f);
            before=a.transform.position; a.Motor.SetMoveInput(Vector2.up);
            bool victimSkillBlocked=!a.Abilities.TryUse(AbilitySlot.Defense);
            yield return new WaitForSeconds(0.06f); a.Motor.SetMoveInput(Vector2.zero);
            Check(victimSkillBlocked && a.transform.position.z>before.z+0.02f && !a.State.MovementLocked,
                "Corte Pesado aplica SkillLock de 0,10 s sem hard stun de movimento");

            ResetPair(1.4f); w.Abilities.TryUse(AbilitySlot.Defense); yield return new WaitForSeconds(0.04f);
            a.Abilities.TryUse(AbilitySlot.BasicAttack); yield return new WaitForSeconds(0.2f);
            before=a.transform.position; a.Motor.SetMoveInput(Vector2.up); yield return new WaitForSeconds(0.12f); a.Motor.SetMoveInput(Vector2.zero);
            Check(a.transform.position.z>before.z+0.05f && !a.State.InputLocked, "Parry interrompe ataque, preserva feedback e não trava movimento");

            stage = "BRX-LAB-001: velocidades e cura em combate";
            ResetPair(8f);
            w.Motor.Teleport(new Vector3(-2f,1f,-18f)); a.Motor.Teleport(new Vector3(2f,1f,-18f));
            w.Motor.SetMoveInput(Vector2.up); a.Motor.SetMoveInput(Vector2.up);
            yield return new WaitForSeconds(5f);
            w.Motor.SetMoveInput(Vector2.zero); a.Motor.SetMoveInput(Vector2.zero);
            Check(Near(w.Definition.moveSpeed,4.9f) && Near(a.Definition.moveSpeed,6.6f) && a.transform.position.z>w.transform.position.z+6f,
                "Corrida de 5 s aplica Guerreiro 4,9 e Assassino 6,6 com vantagem clara do Assassino");

            ResetPair(8f); w.Health.ApplyDamage(70f); w.Inventory.TryAdd(Item("Heal"));
            int changedTicks=0; System.Action<float,float> countTick=(current,max)=>changedTicks++;
            w.Health.Changed+=countTick; bool potionUsed=w.Inventory.UseSlot(0);
            before=w.transform.position; w.Motor.SetMoveInput(Vector2.up);
            bool attackDuringHeal=w.Abilities.TryUse(AbilitySlot.BasicAttack);
            bool skillDuringHeal=w.Abilities.TryUse(AbilitySlot.Defense);
            yield return new WaitForSeconds(0.6f); w.Motor.SetMoveInput(Vector2.zero);
            float afterFirstWindow=w.Health.CurrentHealth; w.Health.ApplyDamage(4f);
            yield return new WaitForSeconds(0.5f);
            bool survivedDamage=w.Health.CurrentHealth>afterFirstWindow-4f && w.HealthRegeneration.IsRegenerating;
            w.Inventory.TryAdd(Item("Heal")); bool renewed=w.Inventory.UseSlot(0);
            float remainingAfterRenew=w.HealthRegeneration.RemainingAmount;
            w.Health.Changed-=countTick;
            Check(potionUsed && attackDuringHeal && skillDuringHeal && w.transform.position.z>before.z+0.4f && survivedDamage &&
                renewed && remainingAfterRenew<=30f && changedTicks>10 && w.Inventory.Slots.Count==0,
                "HoT permite movimento/ataque/skill, resiste a dano, renova sem stacking e notifica a vida por ticks");
            w.HealthRegeneration.CancelRegeneration();

            stage = "BRX-LAB-001: barras e painel de laboratório";
            var lab=FindFirstObjectByType<PrototypeCombatLabController>();
            var warriorBar=w.GetComponent<WorldHealthBar>(); var assassinBar=a.GetComponent<WorldHealthBar>();
            RectTransform warriorFill=w.transform.Find("WorldHealthBar/Fill") as RectTransform;
            float fillBefore=warriorFill!=null?warriorFill.anchorMax.x:-1f; w.Health.ApplyDamage(10f); yield return null;
            Check(warriorBar!=null && assassinBar!=null && warriorFill!=null && warriorFill.anchorMax.x<fillBefore &&
                !Near(w.Health.MaxHealth,a.Health.MaxHealth), "Barras mundiais respondem ao dano e a MaxHealth diferente nas duas classes");

            bool mageRejected=lab!=null&&!lab.SwitchPlayerClass(CharacterClass.Mage);
            bool marksmanRejected=lab!=null&&!lab.SwitchPlayerClass(CharacterClass.Marksman);
            bool switchedWarrior=lab!=null&&lab.SwitchPlayerClass(CharacterClass.Warrior);
            AbilityDefinition warriorBasic=a.Abilities.GetEquipped(AbilitySlot.BasicAttack);
            int inventoryBefore=a.Inventory.Slots.Count;
            bool variants=true;
            foreach(AbilitySlot slot in new[]{AbilitySlot.Defense,AbilitySlot.Movement,AbilitySlot.Ultimate})
                for(int variant=0;variant<3;variant++) variants&=lab.EquipVariation(slot,variant);
            bool basicUnchanged=a.Abilities.GetEquipped(AbilitySlot.BasicAttack)==warriorBasic;
            bool switchedAssassin=lab.SwitchPlayerClass(CharacterClass.Assassin);
            Check(switchedWarrior && switchedAssassin && mageRejected && marksmanRejected && w.Definition.characterClass==CharacterClass.Warrior,
                "Laboratório alterna Guerreiro/Assassino, rejeita classes futuras e mantém oponente Guerreiro");
            Check(variants && basicUnchanged && a.Inventory.Slots.Count==inventoryBefore,
                "Defesa/Movimento/Ultimate Base-A-B trocam grátis; basic e inventário permanecem intactos");
            Check(Item("Var_Assassin_Travel")!=null && a.Inventory.TryAdd(Item("Var_Assassin_Travel")),
                "Sistema normal de runas continua disponível fora da troca livre do laboratório");
            a.Inventory.Initialize(3);
        }
    }
}
#endif
