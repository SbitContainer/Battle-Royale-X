#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BattleRoyaleX.EditorTools
{
    public static class PrototypeDataFactory
    {
        const string Root = "Assets/BattleRoyaleX/GeneratedData";

        [MenuItem("Battle Royale X/Prototype 01/Create Default Data")]
        public static void CreateDefaultData()
        {
            EnsureFolder(Root);
            EnsureFolder(Root + "/Abilities");
            EnsureFolder(Root + "/Characters");
            EnsureFolder(Root + "/Items");

            AbilityDefinition assassinAttack = Ability("Assassin_Basic", "Corte Rápido", AbilitySlot.BasicAttack, AbilityBehavior.MeleeAttack, 1.2f, 0f, 15f, 1.65f, 0.8f);
            assassinAttack.canDestroyMagicalProjectiles = true; assassinAttack.attackInteractionCooldown = 30f;
            assassinAttack.tags = AbilityTags.Physical | AbilityTags.Melee;
            assassinAttack.comboSteps = 3; assassinAttack.comboInputBuffer = 0.20f; assassinAttack.comboSecondDamageMultiplier = 1.05f; assassinAttack.comboThirdDamageMultiplier = 1.38f;
            assassinAttack.startup = 0.10f; assassinAttack.recovery = 0.18f;
            AbilityDefinition assassinDefense = Defense("Assassin_Defense_Base", "Esquiva Sombria", AbilityBehavior.Dodge, DefenseKind.None, 5f, 14f, 0.12f, 2.6f, 0.16f);
            AbilityDefinition assassinCounter = Defense("Assassin_Defense_A", "Contra-Sombra", AbilityBehavior.Parry, DefenseKind.Parry, 7f, 10f, 0f, 0f, 0f, 0.18f, 0.1f, 0.2f);
            AbilityDefinition assassinDouble = Defense("Assassin_Defense_B", "Duplo Passo", AbilityBehavior.Dodge, DefenseKind.None, 3.2f, 13f, 0.16f, 2.6f, 0.14f);
            AbilityDefinition assassinMove = Move("Assassin_Move_Base", "Passo Fantasma", AbilityBehavior.DashThrough, 5f, 10f, 4.2f, 0.18f, true);
            AbilityDefinition assassinTravel = Move("Assassin_Move_A", "Travessia", AbilityBehavior.DashThrough, 6f, 12f, 4.6f, 0.15f, true);
            AbilityDefinition assassinReturn = Move("Assassin_Move_B", "Retorno", AbilityBehavior.DashReturn, 7f, 12f, 4f, 0.15f, true, 1.8f);
            AbilityDefinition assassinUlt = Move("Assassin_Ult_Base", "Cinco Cortes", AbilityBehavior.ChargedDashSequence, 28f, 45f, 4.2f, 0.16f, true);
            assassinUlt.damage = 12f; assassinUlt.chargeCount = 5; assassinUlt.chargeWindow = 6f;
            assassinUlt.slot = AbilitySlot.Ultimate;
            assassinUlt.startup = 0.06f; assassinUlt.recovery = 0.12f; assassinUlt.width = 0.9f;
            AbilityDefinition assassinExec = Buff("Assassin_Ult_A", "Execução", 32f, 50f, 4f, 1.35f, 0.92f, 1f, 1f, 1f);
            AbilityDefinition assassinHunt = Buff("Assassin_Ult_B", "Caçada", 28f, 45f, 6f, 0.82f, 1.22f, 1.15f, 1f, 0.55f);

            AbilityDefinition warriorAttack = Ability("Warrior_Basic", "Corte Pesado", AbilitySlot.BasicAttack, AbilityBehavior.MeleeAttack, 1.55f, 0f, 13f, 2.2f, 1.25f);
            warriorAttack.canDestroyMagicalProjectiles = true; warriorAttack.attackInteractionCooldown = 30f;
            warriorAttack.tags = AbilityTags.Physical | AbilityTags.Melee | AbilityTags.Heavy; warriorAttack.heavy = true;
            warriorAttack.startup = 0.15f; warriorAttack.recovery = 0.25f; warriorAttack.knockback = 1.5f;
            warriorAttack.skillLockOnHit = 0.10f;
            warriorAttack.comboSteps = 3; warriorAttack.comboInputBuffer = 0.20f; warriorAttack.comboSecondDamageMultiplier = 1.12f; warriorAttack.comboThirdDamageMultiplier = 1.55f;
            AbilityDefinition warriorGuard = Defense("Warrior_Defense_Base", "Guarda de Aço", AbilityBehavior.Parry, DefenseKind.Parry, 3.2f, 7f, 0f, 0f, 0f, 0.7f, 0.12f, 0.75f);
            warriorGuard.counterBonusDamage = 6f; warriorGuard.counterWindow = 1f;
            AbilityDefinition warriorParry = Defense("Warrior_Defense_A", "Parry", AbilityBehavior.Parry, DefenseKind.Parry, 6.5f, 9f, 0f, 0f, 0f, 0.35f, 0.12f, 0.5f);
            warriorParry.specialInteractionCooldown = 8f;
            warriorParry.counterBonusDamage = 6f; warriorParry.counterWindow = 1f;
            AbilityDefinition warriorFortress = Defense("Warrior_Defense_B", "Fortaleza", AbilityBehavior.Guard, DefenseKind.Guard, 6f, 12f, 0f, 0f, 0f, 1.1f, 0.05f, 0.85f);
            AbilityDefinition warriorMove = Move("Warrior_Move_Base", "Arremesso de Escudo", AbilityBehavior.Dash, 7f, 10f, 3f, 0.24f, false);
            AbilityDefinition warriorImpact = Move("Warrior_Move_A", "Impacto", AbilityBehavior.Dash, 8f, 12f, 3.3f, 0.22f, false);
            AbilityDefinition warriorAdvance = Move("Warrior_Move_B", "Avanço Defensivo", AbilityBehavior.Dodge, 9f, 14f, 3f, 0.24f, false);
            warriorAdvance.invulnerabilityDuration = 0.12f;
            AbilityDefinition warriorUlt = Buff("Warrior_Ult_Base", "Postura de Guerra", 30f, 45f, 5f, 1.12f, 1f, 1.35f, 1.1f, 1f);
            warriorUlt.startup = 0.24f; warriorUlt.activeTime = 0.12f; warriorUlt.recovery = 0.16f;
            AbilityDefinition warriorRet = Buff("Warrior_Ult_A", "Retaliação", 32f, 50f, 5f, 1.08f, 1f, 1.3f, 1.35f, 1f);
            AbilityDefinition warriorPush = Buff("Warrior_Ult_B", "Avanço Implacável", 32f, 50f, 6f, 1.18f, 1.08f, 1.8f, 1f, 1f);

            // Contact damage is part of the ability data, never driven by animation or VFX.
            foreach (var move in new[] { assassinMove, assassinTravel, assassinReturn })
            { move.damage = 24f; move.passThroughCharacters = true; move.knockback = 0f; }
            assassinMove.startup = 0.06f; assassinMove.recovery = 0.10f;
            warriorMove.startup = 0.10f; warriorMove.recovery = 0.16f;
            assassinDefense.startup = 0.03f; assassinDefense.recovery = 0.08f;
            foreach (var dodge in new[] { assassinDefense, assassinDouble })
            { dodge.damage = dodge == assassinDefense ? 2f : 3f; dodge.passThroughCharacters = true; dodge.knockback = 0f; }
            foreach (var move in new[] { warriorMove, warriorImpact, warriorAdvance })
            { move.damage = 6f; move.pursueTarget = true; move.pursuitStopDistance = 0.72f; }
            warriorMove.pursuitAcquireRange = 7f; warriorMove.pursuitSpeed = 14f; warriorMove.pursuitMaxDuration = 0.55f; warriorMove.knockback = 2f;
            warriorImpact.pursuitAcquireRange = 8f; warriorImpact.pursuitSpeed = 16f; warriorImpact.pursuitMaxDuration = 0.55f; warriorImpact.knockback = 3.5f;
            warriorAdvance.pursuitAcquireRange = 7f; warriorAdvance.pursuitSpeed = 13f; warriorAdvance.pursuitMaxDuration = 0.60f; warriorAdvance.knockback = 1.5f;
            foreach (var guard in new[] { warriorParry, warriorFortress, assassinCounter })
            { guard.damage = 0f; guard.knockback = 0f; guard.clashable = false; }
            warriorGuard.damage = 0f; warriorGuard.knockback = 0f; warriorGuard.clashable = false;
            foreach (var guard in new[] { warriorGuard, warriorParry, warriorFortress })
            {
                guard.startup = 0.03f;
                guard.counterBonusDamage = guard == warriorFortress ? 3f : 6f;
                guard.counterWindow = 1.15f;
                guard.counterKnockback = guard == warriorParry ? 4.5f : guard == warriorFortress ? 3.5f : 4f;
                guard.counterOnBlock = guard == warriorFortress;
                guard.redirectOnDefense = false;
            }
            foreach (var evade in new[] { assassinDefense, assassinDouble, assassinCounter })
            {
                evade.redirectOnDefense = true;
                evade.redirectDistance = evade == assassinDouble ? 3.6f : 3.2f;
                evade.redirectDuration = evade == assassinDouble ? 0.16f : 0.18f;
                evade.passThroughCharacters = true;
                evade.damage = evade == assassinDouble ? 3f : 2f;
                evade.clashable = false;
                evade.counterBonusDamage = evade.counterKnockback = evade.counterWindow = 0f;
                evade.counterOnBlock = false;
                evade.startup = 0.03f;
                evade.recovery = 0.08f;
            }
            foreach (var ult in new[] { warriorUlt, warriorRet, warriorPush, assassinExec, assassinHunt })
            { ult.damage = 8f; ult.range = 0f; ult.explosionRadius = 1.8f; ult.knockback = 0f; ult.clashable = false; }

            // V4: explicit values override prior generated assets as well as fresh installs.
            foreach (var evade in new[] { assassinDefense, assassinDouble })
            {
                evade.defenseDuration = 1f; evade.redirectDistance = 10f;
                evade.redirectDuration = 0.25f; evade.invulnerabilityDuration = 1f;
                evade.movementDistance = 1.2f; evade.movementDuration = 0.18f;
            }
            assassinCounter.behavior = AbilityBehavior.SmokeEscape;
            assassinCounter.defenseKind = DefenseKind.None;
            assassinCounter.redirectOnDefense = false; assassinCounter.damage = 0f;
            assassinCounter.smokeRadius = 6f; assassinCounter.smokeDuration = 4f;
            assassinCounter.cooldown = 12f; assassinCounter.energyCost = 18f;
            assassinTravel.speedBonusDuration = 2f; assassinTravel.speedBonusMultiplier = 1.35f;
            assassinHunt.behavior = AbilityBehavior.HuntSequence;
            assassinHunt.damage = 18f; assassinHunt.huntFirstDamage = 6f; assassinHunt.huntFirstPush = 2f;
            assassinHunt.huntAcquireRange = 12f; assassinHunt.huntSpeed = 22f;
            assassinHunt.huntMaxDuration = 1.5f; assassinHunt.huntOvershoot = 3.5f;
            assassinHunt.chargeWindow = 5f; assassinHunt.startup = 0.08f; assassinHunt.recovery = 0.12f;
            assassinHunt.passThroughCharacters = true;
            assassinHunt.damageMultiplier = assassinHunt.moveSpeedMultiplier = assassinHunt.movementCooldownMultiplier = 1f;
            ConfigureAssassinRework(assassinExec, assassinTravel, assassinDouble);
            ConfigureWarriorRework(warriorAttack, warriorGuard, warriorParry, warriorFortress,
                warriorMove, warriorImpact, warriorAdvance, warriorUlt, warriorRet, warriorPush);

            Restrict(CharacterClass.Assassin, assassinAttack, assassinDefense, assassinCounter, assassinDouble, assassinMove, assassinTravel, assassinReturn, assassinUlt, assassinExec, assassinHunt);
            Restrict(CharacterClass.Warrior, warriorAttack, warriorGuard, warriorParry, warriorFortress, warriorMove, warriorImpact, warriorAdvance, warriorUlt, warriorRet, warriorPush);

            CharacterDefinition assassin = Character("Assassin", "Assassino", CharacterClass.Assassin, 85f, 110f, 11f, 6.6f);
            SetLoadout(assassin, assassinAttack, assassinDefense, assassinMove, assassinUlt, assassinCounter, assassinDouble, assassinTravel, assassinReturn, assassinExec, assassinHunt);
            CharacterDefinition warrior = Character("Warrior", "Guerreiro", CharacterClass.Warrior, 125f, 90f, 7f, 4.9f);
            SetLoadout(warrior, warriorAttack, warriorGuard, warriorMove, warriorUlt, warriorParry, warriorFortress, warriorImpact, warriorAdvance, warriorRet, warriorPush);

            // V1 Mage catalog. Values are deliberately editable and follow the closed laboratory specification.
            AbilityDefinition mageBasic = V1Projectile("Mage_Basic", "Orbe Arcano", AbilitySlot.BasicAttack,
                AbilityBehavior.ProjectileAttack, 0.85f, 0f, 9f, 17f, false, false);
            mageBasic.range = 8f; mageBasic.turnRate = 160f;
            AbilityDefinition mageS1A = V1Projectile("Mage_S1_A", "Faíscas Caçadoras", AbilitySlot.Skill1,
                AbilityBehavior.SeekingProjectile, 6f, 14f, 4f, 11f, false, true);
            mageS1A.projectileCount = 3; mageS1A.turnRate = 185f;
            mageS1A.range = 9f;
            AbilityDefinition mageS1B = V1Projectile("Mage_S1_B", "Orbe Pesado", AbilitySlot.Skill1,
                AbilityBehavior.ProjectileAttack, 8.5f, 20f, 28f, 6f, true, false);
            mageS1B.width = mageS1B.height = 1.5f; mageS1B.heavy = true; mageS1B.tags |= AbilityTags.Heavy;
            mageS1B.width = mageS1B.height = 2.1f; mageS1B.range = 9f;
            AbilityDefinition mageS1C = V1Field("Mage_S1_C", "Campo de Lentidão", AbilitySlot.Skill1,
                AbilityBehavior.SlowField, 10f, 18f, 3f, 3.8f, 4f);
            mageS1C.slowPercent = 0.28f; mageS1C.range = 4f;
            AbilityDefinition mageS2A = V1Movement("Mage_S2_A", "Blink", AbilitySlot.Skill2,
                AbilityBehavior.Blink, 7f, 16f, 4.5f, 0.05f);
            mageS2A.damage = 6f; mageS2A.passThroughCharacters = true; mageS2A.attackKind = AttackKind.Magical;
            mageS2A.range = mageS2A.movementDistance;
            AbilityDefinition mageS2B = V1Movement("Mage_S2_B", "Ecos Arcanos", AbilitySlot.Skill2,
                AbilityBehavior.CloneTeleport, 12f, 22f, 4.5f, 0.05f);
            mageS2B.secondActivationDelay = 0.12f; mageS2B.secondActivationWindow = 2.5f;
            mageS2B.damage = 3f; mageS2B.attackKind = AttackKind.Projectile;
            mageS2B.range = 7f; mageS2B.projectileSpeed = 15f;
            AbilityDefinition mageS2C = V1Field("Mage_S2_C", "Pulso de Repulsão", AbilitySlot.Skill2,
                AbilityBehavior.Repulsion, 9f, 18f, 5f, 3.2f, 0.15f);
            mageS2C.knockback = 4.5f;
            mageS2C.slowPercent = 0.3f; mageS2C.fieldDuration = 1.5f;
            AbilityDefinition mageUltA = V1Projectile("Mage_Ult_A", "Convergência Arcana", AbilitySlot.Ultimate,
                AbilityBehavior.ComboProjectileUltimate, 28f, 45f, 46f, 7f, true, false);
            mageUltA.explosionRadius = 4.5f; mageUltA.secondActivationDelay = 0.25f;
            mageUltA.secondActivationWindow = 3.2f; mageUltA.comboSuccessCooldown = 34f; mageUltA.comboFailureCooldown = 14f;
            AbilityDefinition mageUltB = V1Field("Mage_Ult_B", "Tempestade Arcana", AbilitySlot.Ultimate,
                AbilityBehavior.AreaAttack, 30f, 48f, 7f, 5f, 3.5f);
            mageUltB.startup = 0.55f; mageUltB.activeTime = 2.6f;
            mageUltB.range = 7f;
            AbilityDefinition mageUltC = V1Projectile("Mage_Ult_C", "Prisma Fraturado", AbilitySlot.Ultimate,
                AbilityBehavior.MultiShot, 26f, 42f, 9f, 15f, false, false);
            mageUltC.projectileCount = 7; mageUltC.spreadAngle = 72f; mageUltC.reflectable = true;

            // V1 Archer catalog.
            AbilityDefinition archerBasic = V1Projectile("Archer_Basic", "Disparo Preciso", AbilitySlot.BasicAttack,
                AbilityBehavior.ProjectileAttack, 0.75f, 0f, 8f, 26f, true, false);
            archerBasic.range = 15f;
            AbilityDefinition archerS1A = V1Projectile("Archer_S1_A", "Flechas Rastreadoras", AbilitySlot.Skill1,
                AbilityBehavior.SeekingProjectile, 6.5f, 14f, 4f, 16f, true, true);
            archerS1A.projectileCount = 3; archerS1A.turnRate = 145f;
            AbilityDefinition archerS1B = V1Projectile("Archer_S1_B", "Flecha Pesada", AbilitySlot.Skill1,
                AbilityBehavior.ProjectileAttack, 9f, 20f, 20f, 65f, true, false);
            archerS1B.startup = 0.55f; archerS1B.heavy = true; archerS1B.tags |= AbilityTags.Heavy;
            archerS1B.width = 0.28f; archerS1B.height = 0.55f; archerS1B.range = 22f;
            AbilityDefinition archerS1C = V1Field("Archer_S1_C", "Armadilha Gravitacional", AbilitySlot.Skill1,
                AbilityBehavior.PullTrap, 11f, 20f, 3f, 4f, 4.5f);
            archerS1C.range = 5f; archerS1C.pullStrength = 5.5f;
            archerS1C.damage = 4f;
            AbilityDefinition archerS2A = V1Movement("Archer_S2_A", "Recuo Ofensivo", AbilitySlot.Skill2,
                AbilityBehavior.Dash, 7f, 14f, 3.5f, 0.18f);
            archerS2A.damage = 6f; archerS2A.passThroughCharacters = false; archerS2A.reverseMovement = true;
            archerS2A.fireProjectileOnMove = true; archerS2A.movementDealsDamage = false;
            archerS2A.knockback = 3.5f; archerS2A.damage = 4f;
            archerS2A.range = 10f; archerS2A.projectileSpeed = 24f; archerS2A.attackKind = AttackKind.Projectile;
            AbilityDefinition archerS2B = V1Movement("Archer_S2_B", "Gancho de Reposição", AbilitySlot.Skill2,
                AbilityBehavior.Grapple, 11f, 20f, 7f, 0.32f);
            archerS2B.requiresSurfacePoint = false; archerS2B.movementDealsDamage = false;
            archerS2B.damage = 5f; archerS2B.knockback = 4f;
            archerS2B.range = archerS2B.movementDistance;
            AbilityDefinition archerS2C = V1Movement("Archer_S2_C", "Passos Laterais", AbilitySlot.Skill2,
                AbilityBehavior.MultiDash, 6.5f, 15f, 2.5f, 0.13f);
            archerS2C.chargeCount = 2; archerS2C.chargeWindow = 1.5f; archerS2C.secondActivationDelay = 0.2f;
            AbilityDefinition archerUltA = V1Projectile("Archer_Ult_A", "Rajada Perfurante", AbilitySlot.Ultimate,
                AbilityBehavior.MultiShot, 25f, 40f, 8f, 24f, true, false);
            archerUltA.projectileCount = 6; archerUltA.spreadAngle = 9f;
            AbilityDefinition archerUltB = V1Buff("Archer_Ult_B", "Sobrecarga Cinética", 28f, 42f, 4f, 1.30f);
            archerUltB.movementCooldownMultiplier = 0.70f;
            AbilityDefinition archerUltC = V1Field("Archer_Ult_C", "Chuva de Flechas", AbilitySlot.Ultimate,
                AbilityBehavior.ArrowRain, 34f, 50f, 5f, 3.5f, 3.5f);
            archerUltC.range = 8f; archerUltC.startup = 0.45f;
            ConfigureBasicCadence(assassinAttack, warriorAttack, mageBasic, archerBasic);

            Restrict(CharacterClass.Mage, mageBasic, mageS1A, mageS1B, mageS1C, mageS2A, mageS2B, mageS2C,
                mageUltA, mageUltB, mageUltC);
            Restrict(CharacterClass.Archer, archerBasic, archerS1A, archerS1B, archerS1C, archerS2A, archerS2B,
                archerS2C, archerUltA, archerUltB, archerUltC);
            CharacterDefinition mage = Character("Mage", "Mago", CharacterClass.Mage, 95f, 120f, 10f, 5.4f);
            SetLoadout(mage, mageBasic, mageS1A, mageS2A, mageUltA, mageS1B, mageS1C, mageS2B, mageS2C, mageUltB, mageUltC);
            CharacterDefinition archer = Character("Archer", "Arqueiro", CharacterClass.Archer, 95f, 110f, 9f, 5.8f);
            SetLoadout(archer, archerBasic, archerS1A, archerS2A, archerUltA, archerS1B, archerS1C,
                archerS2B, archerS2C, archerUltB, archerUltC);

            ItemDefinition heal = Item("Heal", "Poção de Cura", ItemKind.Heal, 30f); heal.healDuration = 5f; heal.useDuration = 0f; heal.interruptible = false;
            ItemDefinition energyItem = Item("Energy", "Poção de Essência", ItemKind.Energy, 32f); energyItem.useDuration = 0.75f; energyItem.interruptible = true;
            ItemDefinition cooldownItem = Item("Cooldown", "Orbe de Recarga", ItemKind.CooldownRefresh, 0f, 2.5f); cooldownItem.useDuration = 0.5f; cooldownItem.interruptible = true;
            ItemDefinition backpack = Item("Backpack4", "Mochila 4 Espaços", ItemKind.BackpackUpgrade, 0f, 0f, null, 4); backpack.useDuration = 0.2f; backpack.interruptible = false;
            Tactical("Smoke", "Granada de Fumaça", TacticalKind.Smoke, 3.5f, 5f, 0f);
            Tactical("Repulsion", "Orbe de Repulsão", TacticalKind.Repulsion, 3f, 0.25f, 8f);
            Tactical("Barrier", "Cristal de Barreira", TacticalKind.Barrier, 0f, 4f, 0f);
            Tactical("Null", "Selo Nulo", TacticalKind.NullField, 3f, 3f, 0f);

            Variation("Var_Assassin_Counter", "Runa: Contra-Sombra", assassinCounter);
            Variation("Var_Assassin_Double", "Runa: Duplo Passo", assassinDouble);
            Variation("Var_Assassin_Travel", "Runa: Travessia", assassinTravel);
            Variation("Var_Assassin_Return", "Runa: Retorno", assassinReturn);
            Variation("Var_Assassin_Exec", "Runa: Execução", assassinExec);
            Variation("Var_Assassin_Hunt", "Runa: Caçada", assassinHunt);
            Variation("Var_Warrior_Parry", "Runa: Escudo Usurpador", warriorParry);
            Variation("Var_Warrior_Fortress", "Runa: Fortaleza", warriorFortress);
            Variation("Var_Warrior_Impact", "Runa: Caçada Brutal", warriorImpact);
            Variation("Var_Warrior_Advance", "Runa: Avanço Implacável", warriorAdvance);
            Variation("Var_Warrior_Ret", "Runa: Domínio de Ferro", warriorRet);
            Variation("Var_Warrior_Push", "Runa: Ruptura Titânica", warriorPush);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Battle Royale X: default Prototype 01 data created under " + Root);
        }

        static AbilityDefinition Ability(string id, string name, AbilitySlot slot, AbilityBehavior behavior, float cooldown, float energy, float damage, float range, float width)
        {
            string path = $"{Root}/Abilities/{id}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<AbilityDefinition>(path) ?? ScriptableObject.CreateInstance<AbilityDefinition>();
            asset.abilityId=id; asset.displayName=name; asset.slot=slot; asset.behavior=behavior; asset.cooldown=cooldown; asset.energyCost=energy; asset.damage=damage; asset.range=range; asset.width=width;
            if (!AssetDatabase.Contains(asset)) AssetDatabase.CreateAsset(asset, path);
            EditorUtility.SetDirty(asset); return asset;
        }

        static AbilityDefinition Defense(string id, string name, AbilityBehavior behavior, DefenseKind kind, float cooldown, float energy, float invuln, float distance, float moveDuration, float duration=0.5f, float perfect=0.1f, float reduction=0.7f)
        {
            var a=Ability(id,name,AbilitySlot.Defense,behavior,cooldown,energy,0f,1f,1f); a.defenseKind=kind; a.defenseDuration=duration; a.perfectWindow=perfect; a.damageReduction=reduction; a.invulnerabilityDuration=invuln; a.movementDistance=distance; a.movementDuration=Mathf.Max(0.05f,moveDuration); return a;
        }

        static AbilityDefinition Move(string id, string name, AbilityBehavior behavior, float cooldown, float energy, float distance, float duration, bool through, float returnWindow=0f)
        {
            var a=Ability(id,name,AbilitySlot.Movement,behavior,cooldown,energy,0f,1f,1f); a.movementDistance=distance; a.movementDuration=duration; a.passThroughCharacters=through; a.returnWindow=returnWindow; return a;
        }

        static AbilityDefinition Buff(string id,string name,float cooldown,float energy,float duration,float dmg,float speed,float stagger,float defense,float moveCd)
        {
            var a=Ability(id,name,AbilitySlot.Ultimate,AbilityBehavior.UltimateBuff,cooldown,energy,0f,1f,1f); a.buffDuration=duration; a.damageMultiplier=dmg; a.moveSpeedMultiplier=speed; a.staggerResistanceMultiplier=stagger; a.defenseWindowMultiplier=defense; a.movementCooldownMultiplier=moveCd; return a;
        }

        static AbilityDefinition V1Projectile(string id, string name, AbilitySlot slot, AbilityBehavior behavior,
            float cooldown, float energy, float damage, float speed, bool interceptable, bool seeking)
        {
            AbilityDefinition a = Ability(id, name, slot, behavior, cooldown, energy, damage, 12f, 0.55f);
            a.attackKind = AttackKind.Projectile; a.projectileSpeed = speed; a.interceptable = interceptable;
            a.seeking = seeking; a.reflectable = true; a.amplifiable = true; a.nullifiable = true;
            a.tags = AbilityTags.Projectile | (seeking ? AbilityTags.Seeking : AbilityTags.None) |
                (interceptable ? AbilityTags.Interceptable : AbilityTags.None) | AbilityTags.Reflectable |
                AbilityTags.Amplifiable | AbilityTags.Nullifiable;
            a.startup = 0.16f; a.activeTime = 0.12f; a.recovery = 0.18f;
            return a;
        }

        static AbilityDefinition V1Field(string id, string name, AbilitySlot slot, AbilityBehavior behavior,
            float cooldown, float energy, float damage, float radius, float duration)
        {
            AbilityDefinition a = Ability(id, name, slot, behavior, cooldown, energy, damage, 0f, 1f);
            a.attackKind = AttackKind.Area; a.explosionRadius = radius; a.fieldRadius = radius; a.fieldDuration = duration;
            a.tags = AbilityTags.Magical | AbilityTags.Area; a.clashable = false; a.startup = 0.25f; a.recovery = 0.16f;
            return a;
        }

        static AbilityDefinition V1Movement(string id, string name, AbilitySlot slot, AbilityBehavior behavior,
            float cooldown, float energy, float distance, float duration)
        {
            AbilityDefinition a = Ability(id, name, slot, behavior, cooldown, energy, 0f, 0f, 0.8f);
            a.movementDistance = distance; a.movementDuration = duration; a.startup = 0.04f; a.recovery = 0.10f;
            return a;
        }

        static AbilityDefinition V1Buff(string id, string name, float cooldown, float energy, float duration, float speed)
        {
            AbilityDefinition a = Ability(id, name, AbilitySlot.Ultimate, AbilityBehavior.TimedBuff,
                cooldown, energy, 0f, 0f, 1f);
            a.buffDuration = duration; a.damageMultiplier = 1f; a.moveSpeedMultiplier = speed;
            a.staggerResistanceMultiplier = 1f; a.defenseWindowMultiplier = 1f;
            a.movementCooldownMultiplier = 0.78f; return a;
        }

        static CharacterDefinition Character(string id,string name,CharacterClass cls,float hp,float energy,float regen,float speed)
        {
            string path=$"{Root}/Characters/{id}.asset"; var a=AssetDatabase.LoadAssetAtPath<CharacterDefinition>(path) ?? ScriptableObject.CreateInstance<CharacterDefinition>(); a.characterId=id; a.displayName=name; a.characterClass=cls; a.maxHealth=hp; a.maxEnergy=energy; a.energyRegenPerSecond=regen; a.moveSpeed=speed; if(!AssetDatabase.Contains(a)) AssetDatabase.CreateAsset(a,path); EditorUtility.SetDirty(a); return a;
        }

        static void SetLoadout(CharacterDefinition c, AbilityDefinition basic, AbilityDefinition def, AbilityDefinition move, AbilityDefinition ult, AbilityDefinition defA, AbilityDefinition defB, AbilityDefinition moveA, AbilityDefinition moveB, AbilityDefinition ultA, AbilityDefinition ultB)
        {
            c.basicAttack=basic;c.defenseBase=def;c.movementBase=move;c.ultimateBase=ult;
            c.defenseVariantA=defA;c.defenseVariantB=defB;c.movementVariantA=moveA;c.movementVariantB=moveB;
            c.ultimateVariantA=ultA;c.ultimateVariantB=ultB;
            c.SetV1Loadout(def,defA,defB,move,moveA,moveB,ult,ultA,ultB);
            SetVariantIndexes(c.skill1Variants); SetVariantIndexes(c.skill2Variants); SetVariantIndexes(c.ultimateVariants);
            EditorUtility.SetDirty(c);
        }

        static void SetVariantIndexes(AbilityDefinition[] variants)
        {
            if (variants == null) return;
            for (int i = 0; i < variants.Length; i++) if (variants[i] != null)
            { variants[i].variantIndex = i; EditorUtility.SetDirty(variants[i]); }
        }

        static ItemDefinition Item(string id,string name,ItemKind kind,float amount,float cooldown=0f,AbilityDefinition variation=null,int capacity=4)
        { string path=$"{Root}/Items/{id}.asset"; var a=AssetDatabase.LoadAssetAtPath<ItemDefinition>(path) ?? ScriptableObject.CreateInstance<ItemDefinition>(); a.itemId=id;a.displayName=name;a.kind=kind;a.amount=amount;a.cooldownReductionSeconds=cooldown;a.variationAbility=variation;a.backpackCapacity=capacity;if(!AssetDatabase.Contains(a))AssetDatabase.CreateAsset(a,path);EditorUtility.SetDirty(a);return a; }
        static ItemDefinition Tactical(string id,string name,TacticalKind type,float radius,float duration,float force){var a=Item(id,name,ItemKind.Tactical,0f);a.tacticalKind=type;a.tacticalRadius=radius;a.tacticalDuration=duration;a.tacticalForce=force;a.useDuration=0.15f;a.interruptible=false;EditorUtility.SetDirty(a);return a;}
        static ItemDefinition Variation(string id,string name,AbilityDefinition ability){var a=Item(id,name,ItemKind.Variation,0f,0f,ability);a.useDuration=0.8f;a.interruptible=true;EditorUtility.SetDirty(a);return a;}


        static void Restrict(CharacterClass cls, params AbilityDefinition[] abilities)
        {
            foreach (var a in abilities)
            {
                if (a == null) continue;
                a.classRestricted = true;
                a.requiredClass = cls;
                EditorUtility.SetDirty(a);
            }
        }

        [MenuItem("Battle Royale X/Prototype 01/Update Assassin Variants")]
        public static void UpdateAssassinVariants()
        {
            ConfigureAssassinRework(
                AssetDatabase.LoadAssetAtPath<AbilityDefinition>(Root + "/Abilities/Assassin_Ult_A.asset"),
                AssetDatabase.LoadAssetAtPath<AbilityDefinition>(Root + "/Abilities/Assassin_Move_A.asset"),
                AssetDatabase.LoadAssetAtPath<AbilityDefinition>(Root + "/Abilities/Assassin_Defense_B.asset"));
            AssetDatabase.SaveAssets(); AbilityInteractionMapping.Export();
        }

        public static void UpdateAssassinAndRunTests()
        {
            UpdateAssassinVariants(); PrototypeLiveTestLauncher.RunBatch();
        }

        [MenuItem("Battle Royale X/Prototype 01/Update Warrior And Basic Cadence")]
        public static void UpdateWarriorAndBasicCadence()
        {
            AbilityDefinition Load(string id) => AssetDatabase.LoadAssetAtPath<AbilityDefinition>(Root + "/Abilities/" + id + ".asset");
            ConfigureWarriorRework(Load("Warrior_Basic"), Load("Warrior_Defense_Base"), Load("Warrior_Defense_A"),
                Load("Warrior_Defense_B"), Load("Warrior_Move_Base"), Load("Warrior_Move_A"), Load("Warrior_Move_B"),
                Load("Warrior_Ult_Base"), Load("Warrior_Ult_A"), Load("Warrior_Ult_B"));
            ConfigureBasicCadence(Load("Warrior_Basic"), Load("Assassin_Basic"), Load("Mage_Basic"), Load("Archer_Basic"));
            foreach (string id in new[] { "Var_Warrior_Parry", "Var_Warrior_Fortress", "Var_Warrior_Impact",
                "Var_Warrior_Advance", "Var_Warrior_Ret", "Var_Warrior_Push" })
            {
                var item = AssetDatabase.LoadAssetAtPath<ItemDefinition>(Root + "/Items/" + id + ".asset");
                if (item == null || item.variationAbility == null) continue;
                item.displayName = "Runa: " + item.variationAbility.displayName;
                EditorUtility.SetDirty(item);
            }
            AssetDatabase.SaveAssets();
            AbilityInteractionMapping.Export();
            Debug.Log("[BRX WARRIOR DATA] Warrior variants and four basics updated; other class skills and scene not regenerated.");
        }

        public static void UpdateWarriorAndRunTests()
        {
            UpdateWarriorAndBasicCadence(); PrototypeLiveTestLauncher.RunBatch();
        }

        static void ConfigureBasicCadence(params AbilityDefinition[] basics)
        {
            foreach (var basic in basics)
            {
                if (basic == null) throw new System.InvalidOperationException("Missing basic attack asset.");
                basic.cooldown = 0f;
                basic.attackSpeedMultiplier = 1f;
                if (basic.abilityId == "Warrior_Basic") basic.knockback = 0f;
                EditorUtility.SetDirty(basic);
            }
        }

        static void ConfigureWarriorRework(AbilityDefinition basic, AbilityDefinition guard, AbilityDefinition capture,
            AbilityDefinition fortress, AbilityDefinition shield, AbilityDefinition shortHunt, AbilityDefinition longHunt,
            AbilityDefinition blast, AbilityDefinition field, AbilityDefinition waves)
        {
            var all = new[] { basic, guard, capture, fortress, shield, shortHunt, longHunt, blast, field, waves };
            foreach (var ability in all)
                if (ability == null) throw new System.InvalidOperationException("Missing Warrior asset; scene/data were not regenerated.");
            basic.knockback = 0f; basic.cooldown = 0f;
            foreach (var defense in new[] { guard, capture, fortress })
            {
                defense.slot = AbilitySlot.Skill1; defense.damage = 0f; defense.knockback = 0f;
                defense.startup = 0.03f; defense.recovery = 0.08f;
                defense.counterBonusDamage = defense.counterKnockback = defense.counterWindow = 0f;
                defense.counterOnBlock = defense.redirectOnDefense = false;
                defense.defenseKind = DefenseKind.None; defense.defenseInteractionRule = null;
                defense.invulnerabilityDuration = 0f; defense.movementDistance = 0f;
                defense.clashable = false; defense.tags = AbilityTags.None;
                defense.damageReduction = 1f; defense.specialInteractionCooldown = 0f;
            }
            guard.displayName = "Guarda de Aço"; guard.behavior = AbilityBehavior.Guard;
            guard.defenseKind = DefenseKind.Guard; guard.defenseDuration = 2f;
            guard.cooldown = 6f; guard.energyCost = 10f; guard.fieldRadius = 1.2f;
            capture.displayName = "Escudo Usurpador"; capture.behavior = AbilityBehavior.WarriorSkillCapture;
            capture.defenseDuration = 0.65f; capture.captureRecastWindow = 2f;
            capture.cooldown = 9f; capture.energyCost = 15f;
            fortress.displayName = "Fortaleza"; fortress.behavior = AbilityBehavior.WarriorFortress;
            fortress.defenseDuration = 1.5f; fortress.cooldown = 10f; fortress.energyCost = 18f;
            fortress.projectileCount = 3; fortress.damage = 14f; fortress.knockback = 3f;
            fortress.range = 8f; fortress.width = 0.9f; fortress.projectileSpeed = 12f;
            fortress.attackKind = AttackKind.Projectile; fortress.tags = AbilityTags.Physical | AbilityTags.Projectile;

            foreach (var move in new[] { shield, shortHunt, longHunt })
            {
                move.slot = AbilitySlot.Skill2; move.defenseKind = DefenseKind.None;
                move.invulnerabilityDuration = 0f; move.passThroughCharacters = false;
                move.reverseMovement = move.fireProjectileOnMove = move.requiresSurfacePoint = false;
                move.startup = 0.1f; move.recovery = 0.16f; move.movementDealsDamage = true;
                move.clashable = false; move.attackKind = AttackKind.Physical;
                move.tags = AbilityTags.Physical; move.pursuitStopDistance = 0.72f;
                move.speedBonusDuration = 0f; move.damageMultiplier = move.moveSpeedMultiplier = 1f;
            }
            shield.displayName = "Arremesso de Escudo"; shield.behavior = AbilityBehavior.WarriorShieldCharge;
            shield.cooldown = 7f; shield.energyCost = 12f; shield.damage = 14f; shield.range = 8f;
            shield.movementDistance = 0f; shield.pursueTarget = false; shield.pursuitAcquireRange = 0f;
            shield.width = 0.75f; shield.projectileSpeed = 14f; shield.knockback = 2f;
            shield.attackKind = AttackKind.Projectile; shield.tags = AbilityTags.Physical | AbilityTags.Projectile;
            shield.slowPercent = 0.3f; shield.fieldDuration = 2f;
            shortHunt.displayName = "Caçada Brutal"; shortHunt.behavior = AbilityBehavior.WarriorPursuitStrike;
            shortHunt.cooldown = 8f; shortHunt.energyCost = 14f; shortHunt.damage = 18f; shortHunt.secondStrikeDamage = 6f;
            shortHunt.range = shortHunt.movementDistance = shortHunt.pursuitAcquireRange = 5f;
            shortHunt.movementDuration = 0.5f; shortHunt.pursueTarget = true;
            shortHunt.pursuitSpeed = 14f; shortHunt.pursuitMaxDuration = 0.6f;
            shortHunt.knockback = 0f; shortHunt.slowPercent = 0f;
            longHunt.displayName = "Avanço Implacável"; longHunt.behavior = AbilityBehavior.WarriorPursuitLong;
            longHunt.cooldown = 9f; longHunt.energyCost = 16f; longHunt.damage = 9f; longHunt.secondStrikeDamage = 0f;
            longHunt.range = longHunt.movementDistance = longHunt.pursuitAcquireRange = 9f;
            longHunt.movementDuration = 0.7f; longHunt.pursueTarget = true;
            longHunt.pursuitSpeed = 17f; longHunt.pursuitMaxDuration = 0.8f;
            longHunt.knockback = 0f; longHunt.slowPercent = 0f;

            foreach (var ultimate in new[] { blast, field, waves })
            {
                ultimate.slot = AbilitySlot.Ultimate; ultimate.attackKind = AttackKind.Area;
                ultimate.tags = AbilityTags.Physical | AbilityTags.Area | AbilityTags.Heavy;
                ultimate.clashable = false; ultimate.knockback = 0.8f;
                ultimate.blockable = ultimate.parryable = ultimate.reflectable = ultimate.nullifiable = true;
                ultimate.invulnerabilityDuration = 0f; ultimate.defenseKind = DefenseKind.None;
                ultimate.damageMultiplier = ultimate.moveSpeedMultiplier = ultimate.staggerResistanceMultiplier = 1f;
                ultimate.defenseWindowMultiplier = ultimate.movementCooldownMultiplier = ultimate.attackSpeedMultiplier = 1f;
                ultimate.recovery = 0.24f; ultimate.fieldTickInterval = 0.6f;
            }
            blast.displayName = "Bastião Explosivo"; blast.behavior = AbilityBehavior.WarriorGroundBlast;
            blast.cooldown = 25f; blast.energyCost = 38f; blast.startup = 0.85f; blast.damage = 18f;
            blast.range = 0f; blast.fieldRadius = blast.explosionRadius = 2.2f;
            blast.slowPercent = 0.3f; blast.fieldDuration = 2f;
            blast.buffDuration = 1.8f; blast.defenseDuration = 2f; blast.damageReduction = 0.45f;
            field.displayName = "Domínio de Ferro"; field.behavior = AbilityBehavior.WarriorGroundField;
            field.cooldown = 28f; field.energyCost = 42f; field.startup = 0.75f; field.damage = 3f;
            field.range = 0f; field.fieldRadius = field.explosionRadius = 2.5f;
            field.fieldDuration = 3f; field.fieldTickInterval = 0.6f; field.slowPercent = 0.35f;
            field.buffDuration = 3f; field.defenseDuration = 0.7f; field.damageReduction = 0.35f;
            waves.displayName = "Ruptura Titânica"; waves.behavior = AbilityBehavior.WarriorGroundWaves;
            waves.cooldown = 30f; waves.energyCost = 45f; waves.startup = 1f; waves.damage = 6f;
            waves.range = 0f; waves.fieldRadius = waves.explosionRadius = 3f;
            waves.projectileCount = 3; waves.fieldTickInterval = 0.45f; waves.slowPercent = 0.35f;
            waves.fieldDuration = 2f; waves.buffDuration = waves.defenseDuration = 2f; waves.damageReduction = 0.5f;
            for (int i = 0; i < all.Length; i++)
            {
                all[i].classRestricted = true; all[i].requiredClass = CharacterClass.Warrior;
                if (i > 0) all[i].variantIndex = (i - 1) % 3;
                EditorUtility.SetDirty(all[i]);
            }
        }

        static void ConfigureAssassinRework(AbilityDefinition execution, AbilityDefinition travel, AbilityDefinition orbit)
        {
            if (execution == null || travel == null || orbit == null) throw new System.InvalidOperationException("Missing Assassin variant assets.");
            execution.behavior = AbilityBehavior.ExecutionStrike; execution.damage = 18f;
            execution.huntAcquireRange = execution.range = 9f; execution.huntSpeed = 22f; execution.huntMaxDuration = 1.5f;
            execution.stealthDuration = 2f; execution.width = 0.9f;
            execution.startup = 0.08f; execution.recovery = 0.12f; execution.knockback = 0f;
            execution.damageMultiplier = execution.moveSpeedMultiplier = execution.movementCooldownMultiplier = 1f;
            travel.behavior = AbilityBehavior.DaggerTeleport; travel.damage = 4f; travel.secondStrikeDamage = 12f;
            travel.attackKind = AttackKind.Projectile; travel.tags = AbilityTags.Physical | AbilityTags.Projectile;
            travel.range = 8f; travel.projectileSpeed = 18f; travel.width = 0.35f;
            travel.secondActivationDelay = 0.2f; travel.secondActivationWindow = 3f; travel.explosionRadius = 1.5f;
            travel.startup = 0.06f; travel.recovery = 0.1f; travel.speedBonusDuration = 0f; travel.speedBonusMultiplier = 1f;
            orbit.behavior = AbilityBehavior.OrbitingDaggers; orbit.displayName = "Círculo de Adagas";
            orbit.damage = 2f; orbit.fieldRadius = 1.8f; orbit.fieldDuration = 3f; orbit.repelChance = 0.5f;
            orbit.cooldown = 8f; orbit.invulnerabilityDuration = 0f; orbit.redirectOnDefense = false;
            orbit.movementDistance = 0f; orbit.defenseKind = DefenseKind.None;
            orbit.tags = AbilityTags.Physical | AbilityTags.Area; orbit.knockback = 0f;
            EnsureFolder(Root + "/InteractionRules");
            foreach (var code in new[] { DefenseInteractionRule.RuleCode.All, DefenseInteractionRule.RuleCode.AllExcept })
            {
                string path = Root + "/InteractionRules/Defense_Code_" + (int)code + ".asset";
                var rule = AssetDatabase.LoadAssetAtPath<DefenseInteractionRule>(path);
                if (rule == null)
                {
                    rule = ScriptableObject.CreateInstance<DefenseInteractionRule>(); rule.code = code;
                    AssetDatabase.CreateAsset(rule, path);
                }
                // Regenerating data must preserve user-defined exception lists.
                if (code == DefenseInteractionRule.RuleCode.AllExcept) orbit.defenseInteractionRule = rule;
            }
            EditorUtility.SetDirty(execution); EditorUtility.SetDirty(travel); EditorUtility.SetDirty(orbit);
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent=Path.GetDirectoryName(path)?.Replace('\\','/'); string name=Path.GetFileName(path); if(!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent); AssetDatabase.CreateFolder(parent,name);
        }
    }
}
#endif
