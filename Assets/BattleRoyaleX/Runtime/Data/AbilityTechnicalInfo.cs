namespace BattleRoyaleX
{
    public static class AbilityTechnicalInfo
    {
        public static string Describe(AbilityDefinition a)
        {
            if (a == null) return "";
            string detail = "Dano " + a.damage.ToString("0.#") + " · alcance " + a.range.ToString("0.#") + " m";
            switch (a.behavior)
            {
                case AbilityBehavior.MeleeAttack:
                    if (a.comboSteps > 1) detail += $" · combo manual {a.comboSteps} golpes: {a.damage:0.#} / {a.damage*a.comboSecondDamageMultiplier:0.#} / {a.damage*a.comboThirdDamageMultiplier:0.#}.\n" +
                        $"Janela para encadear {a.comboInputBuffer:0.##} s; recuperação cresce a cada golpe. Defesa interrompe o combo.";
                    break;
                case AbilityBehavior.HuntSequence:
                    detail = $"1º toque: persegue, dano {a.huntFirstDamage:0.#}, empurra {a.huntFirstPush:0.#} m. 2º: dano {a.damage:0.#}, atravessa +{a.huntOvershoot:0.#} m.\n" +
                        $"Reativar em {a.chargeWindow:0.#} s; alvo até {a.huntAcquireRange:0.#} m; perseguição {a.huntSpeed:0.#} m/s por até {a.huntMaxDuration:0.#} s. Paredes bloqueiam."; break;
                case AbilityBehavior.SmokeEscape:
                    detail = $"Fumaça de fuga: raio {a.smokeRadius:0.#} m por {a.smokeDuration:0.#} s. Oculta, proíbe ataques dentro, sem imunidade."; break;
                case AbilityBehavior.Guard:
                case AbilityBehavior.Parry:
                    detail = $"Defesa {a.defenseDuration:0.##} s · redução {a.damageReduction * 100:0}% · parry {(a.behavior == AbilityBehavior.Parry ? a.perfectWindow : 0f):0.##} s.\n" +
                        $"Counter manual: +{a.counterBonusDamage:0.#} dano, empurra {a.counterKnockback:0.#} m, janela {a.counterWindow:0.##} s."; break;
                case AbilityBehavior.Dodge:
                    detail = a.redirectOnDefense ? $"Esquiva reativa {a.defenseDuration:0.#} s: próximo golpe próximo é evitado; atravessa até {a.redirectDistance:0.#} m, dano {a.damage:0.#}.\n" +
                        "Imune a todo dano por 1 s e até terminar a travessia; uma reação; sai 4 m além do agressor. Paredes bloqueiam." :
                        $"Avança {a.movementDistance:0.#} m em {a.movementDuration:0.##} s · dano {a.damage:0.#} · invulnerabilidade {a.invulnerabilityDuration:0.##} s."; break;
                case AbilityBehavior.Dash:
                case AbilityBehavior.DashThrough:
                case AbilityBehavior.DashReturn:
                case AbilityBehavior.ChargedDashSequence:
                    detail = $"Dano {a.damage:0.#} por viagem · {a.movementDistance:0.#} m em {a.movementDuration:0.##} s.";
                    if (a.speedBonusDuration > 0f) detail += $" Após chegar: +{(a.speedBonusMultiplier-1)*100:0}% velocidade por {a.speedBonusDuration:0.#} s.";
                    if (a.behavior == AbilityBehavior.DashReturn) detail += $" Reative em {a.returnWindow:0.#} s para retornar.";
                    if (a.behavior == AbilityBehavior.ChargedDashSequence) detail += $" {a.chargeCount} avanços manuais em {a.chargeWindow:0.#} s.";
                    break;
                case AbilityBehavior.UltimateBuff:
                    detail = $"Dano inicial {a.damage:0.#} · duração {a.buffDuration:0.#} s · dano ×{a.damageMultiplier:0.##} · velocidade ×{a.moveSpeedMultiplier:0.##}.\n" +
                        $"Resistência ×{a.staggerResistanceMultiplier:0.##} · janela de defesa ×{a.defenseWindowMultiplier:0.##} · recarga de avanço ×{a.movementCooldownMultiplier:0.##}."; break;
            }
            return $"<b>{a.displayName}</b> · energia {a.energyCost:0.#} · recarga {a.cooldown:0.#} s · preparação {a.startup:0.##} s · recuperação {a.recovery:0.##} s\n{detail}";
        }
    }
}
