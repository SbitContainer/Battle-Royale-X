# Matriz de testes — Prototype 01

## Compilação
- [ ] Nenhum erro C#.
- [ ] Menu `Battle Royale X` aparece.
- [ ] `Create Default Data` gera assets.
- [ ] `Build Test Scene` gera e salva a arena.

## Movimento
- [ ] P1 move com WASD.
- [ ] P2 move com setas.
- [ ] Personagens permanecem no plano XZ.
- [ ] Câmera enquadra os dois.

## Ataque / dano
- [ ] Ataque do Guerreiro causa dano uma vez por ativação.
- [ ] Ataque do Assassino causa dano uma vez por ativação.
- [ ] Não existe friendly fire entre objetos da mesma equipe.
- [ ] Cooldown impede spam acima do configurado.

## Clash
- [ ] Dois ataques físicos simultâneos colidem.
- [ ] Ambos recebem dano reduzido.
- [ ] Ambos recebem feedback de micro-impacto sem hard stun de movimento.
- [ ] Hitboxes são canceladas depois do Clash.

## Defesa
- [ ] Guarda reduz dano.
- [ ] Parry perfeito zera dano, interrompe a ação ofensiva e não congela movimento.
- [ ] Parry fora da janela perfeita vira defesa parcial.
- [ ] Esquiva do Assassino ignora qualquer dano por 1 s; primeiro contato próximo provoca travessia; proteção dura até terminar a viagem, depois expira (regra V4 revisada).
- [ ] Invulnerabilidade explicitamente configurada em outras habilidades continua evitando dano durante seu intervalo.

## Movimento especial
- [ ] Dash percorre distância configurada.
- [ ] Travessia permite cruzar o adversário.
- [ ] Retorno permite voltar dentro da janela.
- [ ] Retorno expira corretamente.

## Ultimate
- [ ] Buff inicia e expira.
- [ ] Modificadores voltam a 1.0 ao fim.
- [ ] Caçada: primeiro acionamento persegue, causa dano leve e empurra; segundo atravessa o mesmo alvo, inclusive após dash inimigo. Respeita paredes, custo único e expiração (substitui o buff antigo por pedido do usuário).
- [ ] Execução aumenta ameaça mas não remove controle do adversário automaticamente.

## Inventário
- [ ] Mochila inicia com 3 slots.
- [ ] Não aceita 4º item sem upgrade.
- [ ] Mochila de drop aumenta para 4.
- [ ] Item usado desaparece.
- [ ] Cura regenera 30 HP progressivamente em 5 s e é consumida ao ativar.
- [ ] Cura não bloqueia movimento, ataque nem skills e não é interrompida por dano.
- [ ] Uma segunda poção renova/substitui o HoT sem empilhar duas rotinas.
- [ ] Essência mantém seu tempo de uso e restaura energia.
- [ ] Troca de variação demora ~0,8 s e pode ser interrompida por dano.
- [ ] Variação só é consumida quando troca conclui.

## Itens táticos
- [ ] Repulsão desloca o adversário.
- [ ] Barreira cria obstáculo temporário.
- [ ] Campo nulo cancela ataques nullifiable.
- [ ] Granada cria fumaça; Contra-Sombra cria área maior de fuga. Ocultação retira mira automática; atores dentro da área de fuga não atacam e golpes de fora podem acertar.

## Interface mobile V4
- [ ] Botões iniciais têm diâmetros duplicados, sem sobreposição no layout padrão.
- [ ] Editor de layout move e redimensiona sem ativar habilidades; salva e restaura preferências.
- [ ] Menu mostra os dados técnicos reais das habilidades equipadas.
- [ ] Troca de personagem deixa exatamente um jogador e um bot, inclusive ao trocar de volta.
- [ ] Travessia aplica bônus de velocidade por 2 s após chegar, sem substituir buffs da ultimate.

## Airdrop
- [ ] Surge após o tempo configurado.
- [ ] Apresenta três opções.
- [ ] Pegar uma elimina as outras.

## Framework futuro de magia
- [ ] Dois hitboxes mágicos/projéteis colidindo geram explosão em área.
- [ ] Ataque físico marcado `canDestroyMagicalProjectiles` anula magia.
- [ ] A propriedade especial entra em cooldown por 30 s.
- [ ] Durante cooldown, novo ataque físico não anula outra magia.

## Locks independentes — BRX-LAB-001
- [ ] SkillLock bloqueia Defesa/Movimento/Ultimate.
- [ ] SkillLock não impede movimento.
- [ ] MovementLock impede movimento sem bloquear skills.
- [ ] BasicAttackLock impede somente ataque básico.
- [ ] Recovery restringe a ação apropriada sem congelar deslocamento.
- [ ] Corte Pesado do Guerreiro aplica SkillLock de 0,10 s e não aplica hard stun.

## Prioridade Basic x Skill — BRX-LAB-001
- [ ] Basic em startup pode ser cancelado por Defesa, Movimento ou Ultimate.
- [ ] Basic em active é cancelado e sua hitbox não causa dano fantasma.
- [ ] Basic em recovery cede prioridade à skill.
- [ ] A skill começa no mesmo input que cancelou o basic, sem custo duplo.
- [ ] Basic não inicia durante skill incompatível.

## Velocidade runtime — BRX-LAB-001
- [ ] Guerreiro usa velocidade base 4,9.
- [ ] Assassino usa velocidade base 6,6.
- [ ] Em corrida de 5 s, sem skills, o Assassino termina claramente à frente.

## Barras de vida — BRX-LAB-001
- [ ] Barra mundial subscreve `HealthComponent.Changed`.
- [ ] Barra diminui com dano e sobe a cada progressão do HoT.
- [ ] MaxHealth diferente é representada corretamente.
- [ ] Barra funciona no Guerreiro e no Assassino e permanece legível para a câmera.

## Painel de laboratório — BRX-LAB-001
- [ ] Slot adversário permanece Guerreiro com bot existente.
- [ ] Slot jogador alterna entre Guerreiro e Assassino, restaurando HP/energia/loadout base e limpando temporários/cooldowns.
- [ ] Mago e Arqueiro não ficam jogáveis.
- [ ] Defesa Base/A/B troca instantaneamente e sem runa.
- [ ] Movimento Base/A/B troca instantaneamente e sem runa.
- [ ] Ultimate Base/A/B troca instantaneamente e sem runa.
- [ ] Basic Attack não possui variação.
- [ ] Troca livre não consome nem adiciona item ao inventário.
- [ ] Sistema normal de runas continua disponível.

## Perseguição do Guerreiro e bot de treino
- [ ] Investida de Escudo, Impacto e Avanço Defensivo adquirem um inimigo visível à frente dentro do alcance configurado.
- [ ] As três variações corrigem a trajetória enquanto o alvo se move, mas encerram no tempo máximo e não viram perseguição infinita.
- [ ] Paredes interrompem a perseguição e impedem dano remoto.
- [ ] Cada perseguição causa dano apenas uma vez por ativação.
- [ ] Impacto empurra o inimigo para longe na direção real do contato.
- [ ] Contra-ataque manual do Guerreiro empurra para longe do Guerreiro mesmo quando o alvo está fora do centro da hitbox.
- [ ] Modo `Bot: Parado` não anda nem ataca.
- [ ] Modo `Bot: Parado + ataque` mantém posição, olha para o jogador e usa somente ataque básico em alcance.
- [ ] Modo `Bot: Normal` restaura a IA completa.
- [ ] Mobile mostra botões `1`, `2` e `3` no canto inferior esquerdo para Defesa, Movimento e Ultimate.
- [ ] Cada botão alterna sua habilidade entre Base, A e B sem consumir runa.
- [ ] Controle de bot alterna entre Normal, Parado e Parado + ataque.

## Sangue e morte
- [ ] Todo `Hit` confirmado gera spray de sangue direcional no ponto de contato.
- [ ] Bloqueio que ainda recebe dano gera apenas sangue discreto; parry, esquiva e dano negado não geram sangue.
- [ ] Um golpe fatal emite exatamente um evento `Death` para o personagem derrotado.
- [ ] A morte gera explosão de sangue, névoa curta e marca orgânica temporária no chão.
- [ ] O estado `Dead` do Animator permanece ativo no Android até o reinício automático da rodada.
- [ ] A restauração de vida devolve o personagem à animação normal sem alterar vida, dano ou cooldowns.

## Regressão BRX-LAB-001
- [ ] Guarda, Parry, Dodge/iframe e Clash continuam funcionando.
- [ ] Inventário continua aceitando itens.
- [ ] Cooldown e energia continuam funcionando.
