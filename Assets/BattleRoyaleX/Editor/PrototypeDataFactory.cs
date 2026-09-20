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
            warriorAttack.startup = 0.15f; warriorAttack.recovery = 0.25f; warriorAttack.knockback = 1.5f;
            warriorAttack.skillLockOnHit = 0.10f;
            warriorAttack.comboSteps = 3; warriorAttack.comboInputBuffer = 0.20f; warriorAttack.comboSecondDamageMultiplier = 1.12f; warriorAttack.comboThirdDamageMultiplier = 1.55f;
            AbilityDefinition warriorGuard = Defense("Warrior_Defense_Base", "Guarda de Aço", AbilityBehavior.Parry, DefenseKind.Parry, 3.2f, 7f, 0f, 0f, 0f, 0.7f, 0.12f, 0.75f);
            warriorGuard.counterBonusDamage = 6f; warriorGuard.counterWindow = 1f;
            AbilityDefinition warriorParry = Defense("Warrior_Defense_A", "Parry", AbilityBehavior.Parry, DefenseKind.Parry, 6.5f, 9f, 0f, 0f, 0f, 0.35f, 0.12f, 0.5f);
            warriorParry.specialInteractionCooldown = 8f;
            warriorParry.counterBonusDamage = 6f; warriorParry.counterWindow = 1f;
            AbilityDefinition warriorFortress = Defense("Warrior_Defense_B", "Fortaleza", AbilityBehavior.Guard, DefenseKind.Guard, 6f, 12f, 0f, 0f, 0f, 1.1f, 0.05f, 0.85f);
            AbilityDefinition warriorMove = Move("Warrior_Move_Base", "Investida de Escudo", AbilityBehavior.Dash, 7f, 10f, 3f, 0.24f, false);
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

            Restrict(CharacterClass.Assassin, assassinAttack, assassinDefense, assassinCounter, assassinDouble, assassinMove, assassinTravel, assassinReturn, assassinUlt, assassinExec, assassinHunt);
            Restrict(CharacterClass.Warrior, warriorAttack, warriorGuard, warriorParry, warriorFortress, warriorMove, warriorImpact, warriorAdvance, warriorUlt, warriorRet, warriorPush);

            CharacterDefinition assassin = Character("Assassin", "Assassino", CharacterClass.Assassin, 85f, 110f, 11f, 6.6f);
            SetLoadout(assassin, assassinAttack, assassinDefense, assassinMove, assassinUlt, assassinCounter, assassinDouble, assassinTravel, assassinReturn, assassinExec, assassinHunt);
            CharacterDefinition warrior = Character("Warrior", "Guerreiro", CharacterClass.Warrior, 125f, 90f, 7f, 4.9f);
            SetLoadout(warrior, warriorAttack, warriorGuard, warriorMove, warriorUlt, warriorParry, warriorFortress, warriorImpact, warriorAdvance, warriorRet, warriorPush);

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
            Variation("Var_Warrior_Parry", "Runa: Parry", warriorParry);
            Variation("Var_Warrior_Fortress", "Runa: Fortaleza", warriorFortress);
            Variation("Var_Warrior_Impact", "Runa: Impacto", warriorImpact);
            Variation("Var_Warrior_Advance", "Runa: Avanço Defensivo", warriorAdvance);
            Variation("Var_Warrior_Ret", "Runa: Retaliação", warriorRet);
            Variation("Var_Warrior_Push", "Runa: Avanço Implacável", warriorPush);

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

        static CharacterDefinition Character(string id,string name,CharacterClass cls,float hp,float energy,float regen,float speed)
        {
            string path=$"{Root}/Characters/{id}.asset"; var a=AssetDatabase.LoadAssetAtPath<CharacterDefinition>(path) ?? ScriptableObject.CreateInstance<CharacterDefinition>(); a.characterId=id; a.displayName=name; a.characterClass=cls; a.maxHealth=hp; a.maxEnergy=energy; a.energyRegenPerSecond=regen; a.moveSpeed=speed; if(!AssetDatabase.Contains(a)) AssetDatabase.CreateAsset(a,path); EditorUtility.SetDirty(a); return a;
        }

        static void SetLoadout(CharacterDefinition c, AbilityDefinition basic, AbilityDefinition def, AbilityDefinition move, AbilityDefinition ult, AbilityDefinition defA, AbilityDefinition defB, AbilityDefinition moveA, AbilityDefinition moveB, AbilityDefinition ultA, AbilityDefinition ultB)
        { c.basicAttack=basic;c.defenseBase=def;c.movementBase=move;c.ultimateBase=ult;c.defenseVariantA=defA;c.defenseVariantB=defB;c.movementVariantA=moveA;c.movementVariantB=moveB;c.ultimateVariantA=ultA;c.ultimateVariantB=ultB;EditorUtility.SetDirty(c); }

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

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent=Path.GetDirectoryName(path)?.Replace('\\','/'); string name=Path.GetFileName(path); if(!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent); AssetDatabase.CreateFolder(parent,name);
        }
    }
}
#endif
