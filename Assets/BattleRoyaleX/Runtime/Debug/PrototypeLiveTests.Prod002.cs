#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed partial class PrototypeLiveTests
    {
        IEnumerator TestProductionV1()
        {
            stage = "BRX-PROD-002: arquitetura de quatro classes e slots V1";
            PrototypeLabController lab = FindAnyObjectByType<PrototypeLabController>();
            Check(lab != null && lab.warriorDefinition != null && lab.assassinDefinition != null &&
                lab.mageDefinition != null && lab.archerDefinition != null,
                "Laboratório possui definições das quatro classes");

            CharacterDefinition[] definitions = { lab.warriorDefinition, lab.assassinDefinition,
                lab.mageDefinition, lab.archerDefinition };
            bool completeCatalog = definitions.All(d => d != null && d.basicAttack != null &&
                Enumerable.Range(0, 3).All(i => d.GetVariant(AbilitySlot.Skill1, i) != null &&
                    d.GetVariant(AbilitySlot.Skill2, i) != null && d.GetVariant(AbilitySlot.Ultimate, i) != null));
            int uniqueAbilities = definitions.SelectMany(d => new[] { d.basicAttack }
                .Concat(Enumerable.Range(0, 3).Select(i => d.GetVariant(AbilitySlot.Skill1, i)))
                .Concat(Enumerable.Range(0, 3).Select(i => d.GetVariant(AbilitySlot.Skill2, i)))
                .Concat(Enumerable.Range(0, 3).Select(i => d.GetVariant(AbilitySlot.Ultimate, i))))
                .Where(x => x != null).Select(x => x.abilityId).Distinct().Count();
            Check(completeCatalog && uniqueAbilities == 40,
                "Catálogo oferece 4 Basics e 36 habilidades A/B/C selecionáveis");
            Check((int)AbilitySlot.Skill1 == (int)AbilitySlot.Defense &&
                (int)AbilitySlot.Skill2 == (int)AbilitySlot.Movement,
                "Slots V1 preservam serialização dos assets aprovados por aliases de compatibilidade");

            bool allClasses = lab.SwitchPlayerClass(CharacterClass.Warrior) &&
                lab.SwitchPlayerClass(CharacterClass.Assassin) && lab.SwitchPlayerClass(CharacterClass.Mage) &&
                lab.SwitchPlayerClass(CharacterClass.Archer);
            Check(allClasses && w.Definition.characterClass == CharacterClass.Warrior,
                "Troca runtime seleciona as quatro classes sem alterar o Guerreiro adversário");
            Check(a.Health.CurrentHealth == a.Health.MaxHealth && a.Energy.CurrentEnergy == a.Energy.MaxEnergy,
                "Troca de classe restaura HP e energia");

            stage = "BRX-PROD-002: Mago";
            lab.SwitchPlayerClass(CharacterClass.Mage);
            ResetPair(12f);
            lab.EquipVariation(AbilitySlot.Skill1, 0);
            bool sparksUsed = a.Abilities.TryUse(AbilitySlot.Skill1, Vector3.left);
            yield return new WaitForSeconds(0.35f);
            int seekingCount = FindObjectsByType<SeekingProjectileMover>().Count(m =>
                m != null && m.GetComponent<Hitbox>() != null && m.GetComponent<Hitbox>().Owner == a);
            Check(sparksUsed && seekingCount == 3, "Faíscas Caçadoras cria três projéteis seeking de baixo dano");
            Check(FindObjectsByType<SeekingProjectileMover>().Where(m => m.GetComponent<Hitbox>().Owner == a)
                .All(m => m.GetComponentInChildren<Renderer>() != null),
                "Projéteis seeking possuem apresentação visível durante o deslocamento");

            ResetPair(12f); lab.EquipVariation(AbilitySlot.Skill1, 2);
            a.Motor.Teleport(new Vector3(-4f, 1f, 0f)); a.Motor.FaceDirection(Vector3.right);
            w.Motor.Teleport(new Vector3(0f, 1f, 0f)); Physics.SyncTransforms();
            bool slowUsed = a.Abilities.TryUse(AbilitySlot.Skill1, Vector3.right);
            yield return new WaitForSeconds(0.5f);
            Check(slowUsed && w.MovementSlowMultiplier < 0.99f && !w.State.MovementLocked,
                "Campo de Lentidão reduz velocidade sem stun ou MovementLock");
            SlowField visibleField = FindAnyObjectByType<SlowField>();
            Check(visibleField != null && visibleField.GetComponentsInChildren<LineRenderer>().Length >= 2 &&
                Near(visibleField.GetComponentInChildren<LineRenderer>().transform.position.y, 0.06f),
                "Campo mágico mostra limites no chão na posição real da habilidade");

            ResetPair(10f); lab.EquipVariation(AbilitySlot.Skill2, 1);
            Vector3 cloneOrigin = a.transform.position;
            bool clonesUsed = a.Abilities.TryUse(AbilitySlot.Skill2, Vector3.left);
            yield return new WaitForSeconds(0.25f);
            int cloneCount = FindObjectsByType<OwnedAbilityEffect>().Count(e => e.Owner == a && e.name.StartsWith("ArcaneClone_"));
            Check(FindObjectsByType<ArcaneProjectionResources>().Count() == 3 &&
                FindObjectsByType<ArcaneProjectionResources>().All(c => c.GetComponentInChildren<Hitbox>() == null &&
                    c.GetComponentInChildren<Hurtbox>() == null && c.GetComponentInChildren<Renderer>() != null),
                "Clones usam silhuetas renderizadas sem duplicar hitboxes ou hurtboxes");
            bool cloneTeleport = a.Abilities.TryUse(AbilitySlot.Skill2, Vector3.forward);
            yield return null;
            Check(clonesUsed && cloneCount == 3 && cloneTeleport && Vector3.Distance(a.transform.position, cloneOrigin) > 2f,
                "Ecos Arcanos cria três clones e permite teleportar ou deixar a janela expirar");

            ResetPair(15f); lab.EquipVariation(AbilitySlot.Ultimate, 0);
            a.Motor.FaceDirection(Vector3.left);
            events.Clear();
            bool convergenceFirst = a.Abilities.TryUse(AbilitySlot.Ultimate, Vector3.left);
            yield return new WaitForSeconds(0.45f);
            bool convergenceSecond = a.Abilities.TryUse(AbilitySlot.Ultimate, Vector3.left);
            yield return new WaitForSeconds(0.5f);
            Check(convergenceFirst && convergenceSecond && events.Any(e => e.kind == CombatEventKind.Clash &&
                e.ability != null && e.ability.abilityId == "Mage_Ult_A") &&
                a.Abilities.GetCooldownRemaining(AbilitySlot.Ultimate) > 20f,
                "Convergência combina projétil lento e rápido e aplica cooldown de sucesso");

            stage = "BRX-PROD-002: Arqueiro e interações";
            lab.SwitchPlayerClass(CharacterClass.Archer);
            ResetPair(12f); lab.EquipVariation(AbilitySlot.Skill1, 2);
            a.Motor.Teleport(new Vector3(-5f, 1f, 0f)); a.Motor.FaceDirection(Vector3.right);
            w.Motor.Teleport(new Vector3(1.5f, 1f, 0f)); Physics.SyncTransforms();
            Vector3 pullBefore = w.transform.position;
            bool pullUsed = a.Abilities.TryUse(AbilitySlot.Skill1, Vector3.right);
            yield return new WaitForSeconds(0.8f);
            Check(pullUsed && w.transform.position.x < pullBefore.x - 0.05f && !w.State.MovementLocked && !w.State.SkillsLocked,
                "Armadilha Gravitacional puxa sem retirar movimento ou skills");

            ResetPair(10f); lab.EquipVariation(AbilitySlot.Skill2, 2);
            bool dashOne = a.Abilities.TryUse(AbilitySlot.Skill2, Vector3.forward);
            yield return new WaitForSeconds(0.35f);
            bool dashTwo = a.Abilities.TryUse(AbilitySlot.Skill2, Vector3.right);
            yield return new WaitForSeconds(0.35f);
            Check(dashOne && dashTwo && !a.Abilities.IsChargedSequenceActive,
                "Passos Laterais aceita dois deslocamentos com direções independentes");

            lab.SwitchPlayerClass(CharacterClass.Mage); ResetPair(8f);
            AbilityDefinition interceptable = lab.mageDefinition.GetVariant(AbilitySlot.Skill1, 1);
            Hitbox warriorStrike = SpawnHit(w, w.Definition.basicAttack, Vector3.zero);
            Hitbox incoming = SpawnHit(a, interceptable, Vector3.zero);
            float incomingDamage = incoming.Packet.damage;
            CombatResolver.ResolveHitboxInteraction(warriorStrike, incoming);
            Check(warriorStrike.Cancelled && !incoming.Cancelled && incoming.Packet.wasIntercepted &&
                Near(incoming.Packet.damage, incomingDamage * 0.4f, 0.05f),
                "Guerreiro intercepta apenas projétil marcado e reduz 60% do dano restante");
            Destroy(warriorStrike.gameObject); Destroy(incoming.gameObject);

            stage = "BRX-PROD-002: regeneração, VFX e limpeza";
            ResetPair(10f);
            float delay = a.HealthRegeneration.outOfCombatDelay;
            float rate = a.HealthRegeneration.naturalPercentPerSecond;
            a.HealthRegeneration.outOfCombatDelay = 0.12f;
            a.HealthRegeneration.naturalPercentPerSecond = 0.5f;
            a.Health.ApplyDamage(20f);
            float damagedHealth = a.Health.CurrentHealth;
            yield return new WaitForSeconds(0.3f);
            bool naturalHealed = a.Health.CurrentHealth > damagedHealth;
            a.Health.ApplyDamage(2f);
            float secondDamage = a.Health.CurrentHealth;
            yield return new WaitForSeconds(0.06f);
            bool stoppedInCombat = Near(a.Health.CurrentHealth, secondDamage, 0.15f);
            a.HealthRegeneration.outOfCombatDelay = delay;
            a.HealthRegeneration.naturalPercentPerSecond = rate;
            Check(naturalHealed && stoppedInCombat,
                "Regeneração natural inicia fora de combate e reinicia atraso ao receber dano");

            lab.EquipVariation(AbilitySlot.Skill2, 1);
            a.Abilities.TryUse(AbilitySlot.Skill2, Vector3.forward);
            yield return new WaitForSeconds(0.2f);
            bool hadOldEffects = FindObjectsByType<OwnedAbilityEffect>().Any(e => e.Owner == a);
            lab.SwitchPlayerClass(CharacterClass.Archer);
            yield return null;
            bool cleaned = !FindObjectsByType<OwnedAbilityEffect>().Any(e => e.Owner == a);
            Check(hadOldEffects && cleaned,
                "Troca de classe cancela efeitos, projéteis e coroutines pertencentes à classe anterior");

            bool profilesReady = definitions.SelectMany(d => new[] { d.basicAttack }
                .Concat(Enumerable.Range(0, 3).Select(i => d.GetVariant(AbilitySlot.Skill1, i)))
                .Concat(Enumerable.Range(0, 3).Select(i => d.GetVariant(AbilitySlot.Skill2, i)))
                .Concat(Enumerable.Range(0, 3).Select(i => d.GetVariant(AbilitySlot.Ultimate, i))))
                .All(ability => ability != null && ability.visualProfile != null);
            Check(profilesReady, "Todas as 40 habilidades possuem hook de VFX procedural substituível");
            Check(FindObjectsByType<ArcaneDemoObject>().Select(x => x.kind).Distinct().Count() == 6,
                "Arena contém as seis interações arcanas de demonstração");

            lab.SetTitanEvolved(true);
            AbilityDefinition beforeUltimate = a.Abilities.GetEquipped(AbilitySlot.Ultimate);
            lab.EquipVariation(AbilitySlot.Ultimate, 2);
            Check(lab.IsTitanEvolved && a.Abilities.GetEquipped(AbilitySlot.Ultimate) != beforeUltimate,
                "Evolução Titânica acompanha o slot ao trocar a Ultimate");
            lab.SetTitanEvolved(false);
            lab.SwitchPlayerClass(CharacterClass.Assassin);
            ResetPair(8f);
        }
    }
}
#endif
