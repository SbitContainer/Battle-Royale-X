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
                case AbilityBehavior.WarriorSkillCapture:
                    detail = $"Absorve uma habilidade (não ataque básico) em {a.defenseDuration:0.##} s. Toque novamente em até {a.captureRecastWindow:0.#} s para reenviar a habilidade original, sem segunda energia. Segure para mirar; sem cópias infinitas.";
                    break;
                case AbilityBehavior.WarriorFortress:
                    detail = $"Imune a todo dano por {a.defenseDuration:0.#} s. Três escudos giram e depois saem nas direções iniciais separadas por 120°, fase aleatória 15°/30°/45°. Cada um causa {a.damage:0.#} dano e empurra {a.knockback:0.#} m, alcance {a.range:0.#} m.";
                    break;
                case AbilityBehavior.WarriorShieldCharge:
                    detail = $"Arremessa escudo sem mover o Guerreiro: {a.damage:0.#} dano, alcance {a.range:0.#} m, empurra {a.knockback:0.#} m e lentidão {a.slowPercent*100:0}% por {a.fieldDuration:0.#} s. Segure para mirar.";
                    break;
                case AbilityBehavior.WarriorPursuitStrike:
                    detail = $"Persegue até {a.pursuitAcquireRange:0.#} m; {a.damage:0.#} dano no contato e outro golpe de {a.secondStrikeDamage:0.#} ao chegar perto. Não atravessa paredes.";
                    break;
                case AbilityBehavior.WarriorPursuitLong:
                    detail = $"Persegue até {a.pursuitAcquireRange:0.#} m; {a.damage:0.#} dano sem lentidão. Não atravessa paredes.";
                    break;
                case AbilityBehavior.WarriorGroundBlast:
                    detail = $"Conjura por {a.startup:0.##} s e explode ao redor do Guerreiro: {a.damage:0.#} dano, raio {a.fieldRadius:0.#} m, lentidão {a.slowPercent*100:0}% por {a.defenseDuration:0.#} s. Redução de dano {a.damageReduction*100:0}% por {a.buffDuration:0.#} s. Segure para visualizar a área local.";
                    break;
                case AbilityBehavior.WarriorGroundField:
                    detail = $"Conjura por {a.startup:0.##} s e cria área centrada na posição de ativação: raio {a.fieldRadius:0.#} m por {a.fieldDuration:0.#} s, {a.damage:0.#} dano a cada {a.fieldTickInterval:0.#} s e lentidão {a.slowPercent*100:0}%. Redução de dano {a.damageReduction*100:0}% por {a.buffDuration:0.#} s. Segure para visualizar a área local.";
                    break;
                case AbilityBehavior.WarriorGroundWaves:
                    detail = $"Conjura por {a.startup:0.##} s e libera três ondas ao redor do Guerreiro a cada {a.fieldTickInterval:0.##} s, até raio {a.fieldRadius:0.#} m; {a.damage:0.#} dano por onda e lentidão {a.slowPercent*100:0}%. Redução de dano {a.damageReduction*100:0}% por {a.buffDuration:0.#} s. Segure para visualizar a área local.";
                    break;
                case AbilityBehavior.ExecutionStrike:
                    detail = $"Persegue alvo até {a.huntAcquireRange:0.#} m, golpe único de {a.damage:0.#} dano. Invisível por {a.stealthDuration:0.#} s após contato; não dá imunidade. Paredes bloqueiam.";
                    break;
                case AbilityBehavior.DaggerTeleport:
                    detail = $"1º toque: adaga de {a.damage:0.#} dano até {a.range:0.#} m. 2º toque em {a.secondActivationWindow:0.#} s: teleporta até a adaga e golpeia por {a.secondStrikeDamage:0.#} em raio {a.explosionRadius:0.#} m, sem nova energia. Segure para direcionar.";
                    break;
                case AbilityBehavior.OrbitingDaggers:
                    detail = $"5 adagas por {a.fieldDuration:0.#} s. {a.damage:0.#} dano a cada 0,5 s em raio {a.fieldRadius:0.#} m; {a.repelChance*100:0}% de repelir cada golpe permitido pelo perfil. Sem imunidade garantida.";
                    break;
                case AbilityBehavior.ProjectileAttack:
                    if (a.abilityId == "Mage_Basic")
                        detail = $"Orbe {a.damage:0.#} dano, alcance {a.range:0.#} m. Curva até 20° para o oponente mais próximo; segure e arraste para mirar.";
                    else if (a.abilityId == "Archer_Basic")
                        detail = $"Flecha {a.damage:0.#} dano, alcance {a.range:0.#} m. Mira automaticamente no inimigo mais próximo dentro do alcance.";
                    else if (a.abilityId == "Archer_S1_B")
                        detail = $"Tiro manual fino a {a.projectileSpeed:0.#} m/s, alcance {a.range:0.#} m. Dano {a.damage:0.#} de perto, até {a.damage*1.8f:0.#} no limite. Segure e arraste ou use o cursor.";
                    else detail += $" · velocidade {a.projectileSpeed:0.#} m/s · largura {a.width:0.#} m.";
                    break;
                case AbilityBehavior.PullTrap:
                    if (a.abilityId == "Archer_S1_C")
                        detail = $"Arma em 0,2 s. Inimigo a 1,5 m detona: {a.damage:0.#} dano e puxa em raio {a.fieldRadius:0.#} m.";
                    break;
                case AbilityBehavior.ArrowRain:
                    detail = $"Chuva de flechas em raio {a.fieldRadius:0.#} m durante {a.fieldDuration:0.#} s; {a.damage:0.#} dano por pulso de 0,35 s.";
                    break;
                case AbilityBehavior.Grapple:
                    detail = $"Gancho com corda até {a.movementDistance:0.#} m. Puxa até o ponto; se acertar inimigo, aproxima e chuta: {a.damage:0.#} dano, empurrão {a.knockback:0.#} m.";
                    break;
                case AbilityBehavior.TimedBuff:
                    if (a.abilityId == "Archer_Ult_B")
                        detail = $"Sobrecarga por {a.buffDuration:0.#} s: +30% movimento, +50% ataques básicos, -30% recarga de mobilidade e flechas básicas curvas.";
                    break;
                case AbilityBehavior.SlowField:
                    detail = $"Pântano: {a.damage:0.#} dano a cada 0,5 s durante {a.fieldDuration:0.#} s; raio {a.fieldRadius:0.#} m; lentidão {a.slowPercent*100:0}%.";
                    break;
                case AbilityBehavior.Blink:
                    detail = $"Vulto de {a.movementDistance:0.#} m; a alma segue logo atrás. A travessia causa {a.damage:0.#} dano.";
                    break;
                case AbilityBehavior.CloneTeleport:
                    detail = $"Cria 3 ecos que avançam e disparam um orbe de {a.damage:0.#} dano cada. Arraste o segundo toque na direção do clone escolhido em até {a.secondActivationWindow:0.#} s.";
                    break;
                case AbilityBehavior.Repulsion:
                    detail = $"Círculo de chamas: {a.damage:0.#} dano em {a.explosionRadius:0.#} m, empurra {a.knockback:0.#} m e reduz velocidade em {a.slowPercent*100:0}% por {a.fieldDuration:0.#} s.";
                    break;
                case AbilityBehavior.MeleeAttack:
                    if (a.comboSteps > 1) detail += $" · combo {a.comboSteps} golpes: {a.damage:0.#} / {a.damage*a.comboSecondDamageMultiplier:0.#} / {a.damage*a.comboThirdDamageMultiplier:0.#}.\n" +
                        $"Janela para encadear {a.comboInputBuffer:0.##} s; recuperação cresce a cada golpe. Defesa interrompe o combo.";
                    break;
                case AbilityBehavior.HuntSequence:
                    detail = $"1º toque: persegue, dano {a.huntFirstDamage:0.#}, empurra {a.huntFirstPush:0.#} m. 2º: dano {a.damage:0.#}, atravessa +{a.huntOvershoot:0.#} m.\n" +
                        $"Reativar em {a.chargeWindow:0.#} s; alvo até {a.huntAcquireRange:0.#} m; perseguição {a.huntSpeed:0.#} m/s por até {a.huntMaxDuration:0.#} s. Paredes bloqueiam."; break;
                case AbilityBehavior.SmokeEscape:
                    detail = $"Fumaça de fuga: raio {a.smokeRadius:0.#} m por {a.smokeDuration:0.#} s. Oculta, proíbe ataques dentro, sem imunidade."; break;
                case AbilityBehavior.Guard:
                case AbilityBehavior.Parry:
                    if (a.requiredClass == CharacterClass.Warrior && a.behavior == AbilityBehavior.Guard)
                    { detail = $"Aura por {a.defenseDuration:0.#} s: anula todo golpe, inclusive ataque básico, sem contra-ataque automático."; break; }
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
                    if (a.pursueTarget) detail += $" Persegue alvo visível à frente até {a.pursuitAcquireRange:0.#} m por no máximo {a.pursuitMaxDuration:0.##} s; empurra {a.knockback:0.#} m.";
                    if (a.speedBonusDuration > 0f) detail += $" Após chegar: +{(a.speedBonusMultiplier-1)*100:0}% velocidade por {a.speedBonusDuration:0.#} s.";
                    if (a.behavior == AbilityBehavior.DashReturn) detail += $" Reative em {a.returnWindow:0.#} s para retornar.";
                    if (a.behavior == AbilityBehavior.ChargedDashSequence) detail += $" {a.chargeCount} avanços manuais em {a.chargeWindow:0.#} s.";
                    break;
                case AbilityBehavior.UltimateBuff:
                    detail = $"Dano inicial {a.damage:0.#} · duração {a.buffDuration:0.#} s · dano ×{a.damageMultiplier:0.##} · velocidade ×{a.moveSpeedMultiplier:0.##}.\n" +
                        $"Resistência ×{a.staggerResistanceMultiplier:0.##} · janela de defesa ×{a.defenseWindowMultiplier:0.##} · recarga de avanço ×{a.movementCooldownMultiplier:0.##}."; break;
            }
            if (a.slot == AbilitySlot.BasicAttack) detail += "\nSegure para atacar continuamente. Sem recarga: cadência limitada pela velocidade de ataque e pelas fases do golpe.";
            return $"<b>{a.displayName}</b> · energia {a.energyCost:0.#} · recarga {(a.slot == AbilitySlot.BasicAttack ? 0f : a.cooldown):0.#} s · preparação {a.startup:0.##} s · recuperação {a.recovery:0.##} s\n{detail}";
        }
    }
}
