#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BattleRoyaleX.EditorTools
{
    public static class AbilityInteractionMapping
    {
        [MenuItem("Battle Royale X/Prototype 01/Export Ability Interaction Map")]
        public static void Export()
        {
            var abilities = AssetDatabase.FindAssets("t:AbilityDefinition", new[] { "Assets/BattleRoyaleX/GeneratedData/Abilities" })
                .Select(g => AssetDatabase.LoadAssetAtPath<AbilityDefinition>(AssetDatabase.GUIDToAssetPath(g)))
                .OrderBy(a => a.abilityId).ToArray();
            if (abilities.Any(a => string.IsNullOrEmpty(a.abilityId)) || abilities.Select(a => a.abilityId).Distinct().Count() != abilities.Length)
                throw new System.InvalidOperationException("Ability IDs must be unique and nonempty.");
            var text = new StringBuilder("# Mapeamento das interações de habilidades\n\n" +
                "Gerado pelos dados reais; regenerar pelo menu Export Ability Interaction Map. IDs não mudam com o nome visual. Catálogo numerado dos mecanismos existentes em [INTERACTION_RULES.md](INTERACTION_RULES.md).\n\n" +
                "Código 1: aceita todos os ataques. Código 2: aceita todos EXCETO os IDs da lista do perfil. " +
                "Código 0: flags antigas preservadas, sem perfil atribuído. Elegibilidade não define chance, redução, reflexão ou imunidade. " +
                "Guarda/parry/reflexão ainda exigem suas flags; não foram rebalanceados. " +
                "Círculo de Adagas usa código 2, com 50% por golpe elegível. Exceções aguardam decisão do usuário; lista vazia não exclui nenhuma ultimate.\n\n" +
                "Códigos I abaixo são capacidades/configuração; a interação concreta ainda exige as condições de contato, fase e cooldown do catálogo.\n\n" +
                "| ID | Nome | Comportamento | Dano | Bloqueável | Parry | Refletível | Anulável | Código de defesa | Exceções | Interações I |\n" +
                "|---|---|---|---:|---|---|---|---|---:|---|---|\n");
            foreach (var a in abilities)
            {
                var rule = a.defenseInteractionRule;
                string excluded = rule == null || rule.excludedAbilityIds == null ? "—" : string.Join(", ", rule.excludedAbilityIds);
                text.AppendLine($"| {a.abilityId} | {a.displayName} | {a.behavior} | {a.damage:0.##} | {a.blockable} | {a.parryable} | {a.reflectable} | {a.nullifiable} | {(rule == null ? 0 : (int)rule.code)} | {excluded} | {CodesFor(a)} |");
            }
            File.WriteAllText("Docs/ABILITY_INTERACTIONS.md", text.ToString());
            Debug.Log("[BRX INTERACTIONS] mapped=" + abilities.Length + "; existing interactions preserved.");
        }

        static string CodesFor(AbilityDefinition a)
        {
            var codes = new List<string>();
            if (a.damage > 0f)
            {
                if (a.blockable) codes.Add("I01");
                if (a.parryable) codes.Add("I02");
                if (a.nullifiable) { codes.Add("I04"); codes.Add("I13"); }
                if (a.reflectable) codes.Add("I05");
                if (a.attackKind == AttackKind.Physical && a.clashable) codes.Add("I06");
                if (a.attackKind == AttackKind.Projectile || a.attackKind == AttackKind.Magical) codes.Add("I07");
            }
            if (a.canDestroyMagicalProjectiles) codes.Add("I08");
            if (a.interceptable || (a.tags & AbilityTags.Interceptable) != 0) codes.Add("I09");
            if (a.counterBonusDamage > 0f) codes.Add("I10");
            if (a.redirectOnDefense) codes.Add("I11");
            if (a.behavior == AbilityBehavior.SmokeEscape) codes.Add("I12");
            bool projectile = a.behavior == AbilityBehavior.ProjectileAttack || a.behavior == AbilityBehavior.SeekingProjectile ||
                a.behavior == AbilityBehavior.MultiShot || a.behavior == AbilityBehavior.ComboProjectileUltimate ||
                a.behavior == AbilityBehavior.DaggerTeleport || a.fireProjectileOnMove;
            if (projectile)
            {
                codes.Add("I14");
                if (a.reflectable) codes.Add("I15");
                if (a.amplifiable || (a.tags & AbilityTags.Amplifiable) != 0) codes.Add("I16");
            }
            if (a.fragmentTrigger || (a.tags & AbilityTags.FragmentTrigger) != 0) codes.Add("I17");
            if (a.heavy || (a.tags & (AbilityTags.Heavy | AbilityTags.Area)) != 0) codes.Add("I18");
            if (a.behavior == AbilityBehavior.Repulsion) codes.Add("I19");
            if (a.behavior == AbilityBehavior.OrbitingDaggers) codes.Add("I20");
            if (a.behavior == AbilityBehavior.Guard && !codes.Contains("I01")) codes.Add("I01");
            if (a.behavior == AbilityBehavior.Parry && !codes.Contains("I02")) codes.Add("I02");
            if (a.invulnerabilityDuration > 0f) codes.Add("I03");
            return string.Join(", ", codes);
        }
    }
}
#endif
