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
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            Check(a.Health.CurrentHealth == a.Health.MaxHealth && a.Energy.CurrentEnergy == a.Energy.MaxEnergy,
                "Troca de classe restaura HP e energia");

            stage = "BRX-PROD-002: Mago";
            lab.SwitchPlayerClass(CharacterClass.Mage);
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            ResetPair(6f);
            a.Motor.FaceDirection(Vector3.left);
            w.Motor.Teleport(w.transform.position + Vector3.forward * 2.5f);
            Physics.SyncTransforms();
            bool basicUsed = a.Abilities.TryUse(AbilitySlot.BasicAttack, Vector3.left);
            yield return new WaitForSeconds(0.26f);
            SeekingProjectileMover curvedBasic = FindObjectsByType<SeekingProjectileMover>()
                .FirstOrDefault(m => m != null && m.GetComponent<Hitbox>() != null &&
                    m.GetComponent<Hitbox>().Owner == a && m.GetComponent<Hitbox>().Packet.ability == a.Definition.basicAttack);
            Check(basicUsed && curvedBasic != null && Near(curvedBasic.MaximumAngle, 20f) &&
                Vector3.Angle(curvedBasic.InitialDirection, curvedBasic.transform.forward) <= 20.5f &&
                Vector3.Angle(curvedBasic.InitialDirection, curvedBasic.transform.forward) > 1f,
                "Orbe básico curva para o adversário até o limite de 20 graus");

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
            Check(w.Health.CurrentHealth < w.Health.MaxHealth,
                "Pântano causa dano periódico ao inimigo dentro da área");
            SlowField visibleField = FindAnyObjectByType<SlowField>();
            Check(visibleField != null && visibleField.GetComponentsInChildren<LineRenderer>().Length >= 2 &&
                Near(visibleField.GetComponentInChildren<LineRenderer>().transform.position.y, 0.06f),
                "Campo mágico mostra limites no chão na posição real da habilidade");
            Check(visibleField != null && visibleField.GetComponentInChildren<ArcaneFieldPresentation>() != null &&
                visibleField.GetComponentsInChildren<ParticleSystem>().All(p => p.main.maxParticles <= 32),
                "Campo refinado mantém fluxo visual limitado a 32 partículas por emissor");

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

            ResetPair(12f); lab.EquipVariation(AbilitySlot.Skill2, 1);
            bool clonesFireUsed = a.Abilities.TryUse(AbilitySlot.Skill2, Vector3.left);
            yield return new WaitForSeconds(0.52f);
            int cloneShots = FindObjectsByType<Hitbox>().Count(h => h.Owner == a &&
                h.Packet.ability != null && h.Packet.ability.abilityId == "Mage_S2_B");
            Check(clonesFireUsed && cloneShots == 3 && FindObjectsByType<ArcaneCloneMotion>().Length == 3,
                "Três ecos avançam e cada um dispara um orbe de dano leve");

            ResetPair(2.2f); lab.EquipVariation(AbilitySlot.Skill2, 0);
            a.Motor.FaceDirection(Vector3.left); Physics.SyncTransforms();
            bool blinkUsed = a.Abilities.TryUse(AbilitySlot.Skill2, Vector3.left);
            yield return new WaitForSeconds(0.35f);
            Check(blinkUsed && w.Health.CurrentHealth < w.Health.MaxHealth &&
                FindAnyObjectByType<MageSoulEcho>() != null,
                "Blink atravessa o inimigo com dano e deixa alma visual atrasada");

            ResetPair(2.0f); lab.EquipVariation(AbilitySlot.Skill2, 2);
            a.Motor.FaceDirection(Vector3.left); Physics.SyncTransforms();
            float repulseOrigin = w.transform.position.x;
            bool repulseUsed = a.Abilities.TryUse(AbilitySlot.Skill2, Vector3.left);
            yield return new WaitForSeconds(0.4f);
            Check(repulseUsed && w.transform.position.x < repulseOrigin - 0.1f &&
                w.MovementSlowMultiplier < 0.99f && w.Health.CurrentHealth < w.Health.MaxHealth,
                "Pulso de Repulsão causa dano, empurra radialmente e aplica lentidão");

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
            ExpandingImpactPresentation shockwave = FindAnyObjectByType<ExpandingImpactPresentation>();
            Check(shockwave != null && Near(shockwave.radius, a.Abilities.GetEquipped(AbilitySlot.Ultimate).explosionRadius) &&
                shockwave.GetComponentInChildren<Collider>() == null,
                "Convergência emite onda visual com raio real e sem colisores adicionais");

            stage = "BRX-PROD-002: Arqueiro e interações";
            lab.SwitchPlayerClass(CharacterClass.Archer);
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            ResetPair(12f); lab.EquipVariation(AbilitySlot.Skill1, 2);
            a.Motor.Teleport(new Vector3(-5f, 1f, 0f)); a.Motor.FaceDirection(Vector3.right);
            w.Motor.Teleport(new Vector3(1.5f, 1f, 0f)); Physics.SyncTransforms();
            Vector3 pullBefore = w.transform.position;
            bool pullUsed = a.Abilities.TryUse(AbilitySlot.Skill1, Vector3.right);
            yield return new WaitForSeconds(0.8f);
            Check(pullUsed && w.transform.position.x < pullBefore.x - 0.05f && !w.State.MovementLocked && !w.State.SkillsLocked,
                "Armadilha Gravitacional puxa sem retirar movimento ou skills");

            AbilityDefinition bowBasic = lab.archerDefinition.basicAttack;
            AbilityDefinition heavyArrow = lab.archerDefinition.GetVariant(AbilitySlot.Skill1, 1);
            AbilityDefinition trap = lab.archerDefinition.GetVariant(AbilitySlot.Skill1, 2);
            AbilityDefinition grapple = lab.archerDefinition.GetVariant(AbilitySlot.Skill2, 1);
            AbilityDefinition overload = lab.archerDefinition.GetVariant(AbilitySlot.Ultimate, 1);
            AbilityDefinition rain = lab.archerDefinition.GetVariant(AbilitySlot.Ultimate, 2);
            Check(Near(bowBasic.damage, 8f) && Near(bowBasic.range, 15f) &&
                heavyArrow.width < 0.3f && heavyArrow.projectileSpeed >= 60f && heavyArrow.range >= 20f,
                "Arqueiro: básico mais leve e Flecha Pesada fina, rápida e de longa distância");
            Check(trap.behavior == AbilityBehavior.PullTrap && grapple.behavior == AbilityBehavior.Grapple &&
                rain.behavior == AbilityBehavior.ArrowRain && Near(overload.buffDuration, 4f) &&
                Near(overload.moveSpeedMultiplier, 1.3f) && Near(overload.movementCooldownMultiplier, 0.7f),
                "Arqueiro: armadilha, gancho, chuva e Sobrecarga usam os parâmetros especificados");

            ResetPair(9f);
            a.Motor.Teleport(new Vector3(-5f, 1f, 0f));
            w.Motor.Teleport(new Vector3(3f, 1f, 2f));
            a.Motor.FaceDirection(Vector3.back); Physics.SyncTransforms();
            bool aimedBasic = a.Abilities.TryUse(AbilitySlot.BasicAttack);
            Vector3 enemyDirection = w.transform.position - a.transform.position; enemyDirection.y = 0f;
            Check(aimedBasic && Vector3.Dot(a.Motor.Facing, enemyDirection.normalized) > 0.98f,
                "Ataque básico do Arqueiro aponta para o inimigo dentro do alcance mesmo se estava virado para trás");

            GameObject rangedTest = new GameObject("HeavyArrow_Distance_Test");
            rangedTest.transform.position = Vector3.up * 100f;
            Hitbox rangedHit = rangedTest.AddComponent<Hitbox>();
            rangedHit.Configure(a, new DamagePacket(a, heavyArrow, Vector3.right), Vector3.one, 1f);
            float nearDamage = rangedHit.Packet.damage;
            rangedTest.AddComponent<ArcherDistanceDamage>().Configure(heavyArrow.range);
            rangedTest.transform.position += Vector3.right * (heavyArrow.range * 0.95f);
            yield return null;
            Check(rangedHit.Packet.damage > nearDamage * 1.7f,
                "Flecha Pesada ganha dano com a distância real percorrida");
            Destroy(rangedTest);

            ResetPair(12f);
            a.Motor.Teleport(new Vector3(-5f, 1f, 0f));
            w.Motor.Teleport(new Vector3(3f, 1f, 0f)); Physics.SyncTransforms();
            float beforeSweep = w.Health.CurrentHealth;
            GameObject sweptArrow = new GameObject("HeavyArrow_Sweep_Test");
            sweptArrow.transform.position = new Vector3(-3f, 1.8f, 0f);
            Hitbox sweptHit = sweptArrow.AddComponent<Hitbox>();
            sweptHit.Configure(a, new DamagePacket(a, heavyArrow, Vector3.right),
                new Vector3(heavyArrow.width, heavyArrow.height, heavyArrow.width), 0.5f);
            sweptArrow.AddComponent<ArcherDistanceDamage>().Configure(heavyArrow.range);
            sweptArrow.transform.position = new Vector3(5f, 1.8f, 0f);
            yield return null;
            Check(w.Health.CurrentHealth < beforeSweep,
                "Flecha Pesada detecta o alvo mesmo ao atravessar vários metros em um quadro");
            Destroy(sweptArrow);

            ResetPair(20f); lab.EquipVariation(AbilitySlot.Skill1, 2);
            a.Motor.Teleport(new Vector3(-5f, 1f, 0f));
            w.Motor.Teleport(new Vector3(8f, 1f, 0f)); Physics.SyncTransforms();
            bool trapUsed = a.Abilities.TryUse(AbilitySlot.Skill1, Vector3.right);
            yield return new WaitForSeconds(0.45f);
            bool stillArmed = FindAnyObjectByType<ArcherProximityTrap>() != null &&
                FindAnyObjectByType<PullField>() == null;
            float beforeTrap = w.Health.CurrentHealth;
            w.Motor.Teleport(new Vector3(0.8f, 1f, 0f)); Physics.SyncTransforms();
            yield return new WaitForSeconds(0.25f);
            Check(trapUsed && stillArmed && FindAnyObjectByType<PullField>() != null &&
                w.Health.CurrentHealth < beforeTrap,
                "Armadilha permanece armada até o inimigo entrar na proximidade e então causa dano");

            ResetPair(20f); lab.EquipVariation(AbilitySlot.Ultimate, 1);
            bool boosted = a.Abilities.TryUse(AbilitySlot.Ultimate);
            yield return new WaitForSeconds(0.25f);
            Check(boosted && Near(a.Modifiers.moveSpeedMultiplier, 1.3f) &&
                Near(a.Modifiers.movementCooldownMultiplier, 0.7f),
                "Sobrecarga aplica +30% movimento e -30% recarga de mobilidade por 4 s");

            ResetPair(12f); lab.EquipVariation(AbilitySlot.Ultimate, 2);
            a.Motor.Teleport(new Vector3(-5f, 1f, 0f)); a.Motor.FaceDirection(Vector3.right);
            w.Motor.Teleport(new Vector3(3f, 1f, 0f)); Physics.SyncTransforms();
            float rainBefore = w.Health.CurrentHealth;
            bool rainUsed = a.Abilities.TryUse(AbilitySlot.Ultimate, Vector3.right);
            yield return new WaitForSeconds(1.0f);
            var rainProbe = FindAnyObjectByType<ArcherArrowRain>();
            Debug.Log($"[BRX RAIN005 PROBE] used={rainUsed} field={(rainProbe != null ? rainProbe.transform.position.ToString() : "none")} a={a.transform.position} w={w.transform.position} hp={rainBefore}/{w.Health.CurrentHealth} immunity={w.Abilities.HasFortressDamageImmunity}/{w.State.IsInvulnerable} guard={w.Defense.IsActive} action={a.Abilities.IsActionBusy} energy={a.Energy.CurrentEnergy} events=" + string.Join(";", events.Where(e=>e.ability==Ability("Archer_Ult_C")).Select(e=>$"{e.kind}:{e.phase}:{e.position}:{e.value}")));
            Check(rainUsed && FindAnyObjectByType<ArcherArrowRain>() != null &&
                w.Health.CurrentHealth < rainBefore,
                "Chuva de Flechas permanece na área e aplica pulsos de dano");

            ResetPair(20f); lab.EquipVariation(AbilitySlot.Skill2, 1);
            a.Motor.Teleport(new Vector3(-5f, 1f, 0f)); a.Motor.FaceDirection(Vector3.right);
            w.Motor.Teleport(new Vector3(5f, 1f, 4f)); Physics.SyncTransforms();
            Vector3 hookStart = a.transform.position;
            bool hookUsed = a.Abilities.TryUse(AbilitySlot.Skill2, Vector3.right);
            yield return new WaitForSeconds(0.2f);
            Check(hookUsed && a.transform.position.x > hookStart.x + 0.1f &&
                FindAnyObjectByType<ArcherGrappleRope>() != null,
                "Gancho lança corda e puxa mesmo sem superfície ou inimigo acertado");

            ResetPair(10f); lab.EquipVariation(AbilitySlot.Skill2, 2);
            bool dashOne = a.Abilities.TryUse(AbilitySlot.Skill2, Vector3.forward);
            yield return new WaitForSeconds(0.35f);
            bool dashTwo = a.Abilities.TryUse(AbilitySlot.Skill2, Vector3.right);
            yield return new WaitForSeconds(0.35f);
            Check(dashOne && dashTwo && !a.Abilities.IsChargedSequenceActive,
                "Passos Laterais aceita dois deslocamentos com direções independentes");

            lab.SwitchPlayerClass(CharacterClass.Mage); ResetPair(8f);
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            a.Motor.Teleport(new Vector3(-5f, 1f, 0f));
            w.Motor.Teleport(new Vector3(2f, 1f, 2f));
            a.Motor.FaceDirection(Vector3.back); Physics.SyncTransforms();
            bool mageAimed = a.Abilities.TryUse(AbilitySlot.BasicAttack);
            Vector3 mageDirection = w.transform.position - a.transform.position; mageDirection.y = 0f;
            Check(mageAimed && Vector3.Dot(a.Motor.Facing, mageDirection.normalized) > 0.98f,
                "Mago também aponta o básico para o inimigo dentro do alcance sem mira manual");
            ResetPair(8f);
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
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
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
            foreach (var bot in FindObjectsByType<PrototypeTrainingBot>()) bot.enabled = false;
            ResetPair(8f);
        }
    }
}
#endif
