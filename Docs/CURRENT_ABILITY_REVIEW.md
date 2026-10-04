# Battle Royale X — catálogo de habilidades para o laboratório 0.9.1

Leitura inicial do APK 0.8.0; as seções de Mago e Arqueiro abaixo refletem as
mudanças locais mais recentes. A confirmação visual no aparelho ainda é necessária. `A`, `B` e `C` são as
três opções de cada botão. No laboratório, a opção A começa equipada; as
outras são alternadas pelo painel ou por runas. Em uma partida BR futura,
elas devem vir do chão.

## Regras gerais já ativas

- Ataques básicos do Guerreiro e Assassino têm três golpes consecutivos.
- Fumaça bloqueia visão e, para quem está dentro dela, bloqueia ataques
  ofensivos. Golpes disparados de fora ainda podem atingir alguém oculto.
- Não há hard stun nas habilidades normais.
- Dano, tempo, alcance e cooldown abaixo são valores de laboratório, não
  números finais de balanceamento.

## Guerreiro

Base: 125 HP, 90 energia, 4,9 de movimento, 7 de regeneração de energia/s.

| Botão | Opção atual | Como funciona agora |
|---|---|---|
| Ataque | Corte Pesado | Combo de 3 golpes corpo a corpo: 13, 14,6 e 20,15 de dano. Alcance 2,2 m, knockback 1,5 m, startup 0,15 s e recuperação 0,25 s. Pode interceptar projéteis mágicos compatíveis. Cooldown 1,55 s. |
| Skill 1 A | Guarda de Aço | Parry/bloqueio frontal por 0,7 s; janela perfeita 0,12 s; redução de dano de 75%. Ao defender, arma por 1,15 s o próximo ataque básico: +6 de dano e empurrão 4 m. Cooldown 3,2 s. |
| Skill 1 B | Parry | Parry curto por 0,35 s; janela perfeita 0,12 s; redução de 50%. O próximo básico, se a defesa funcionar, recebe +6 de dano e empurra 4,5 m. Cooldown 6,5 s. |
| Skill 1 C | Fortaleza | Guarda por 1,1 s, redução de 85%. Uma defesa bem-sucedida arma contra-ataque de +3 de dano e 3,5 m de empurrão. Cooldown 6 s. |
| Skill 2 A | Investida de Escudo | Investida em direção ao alvo visível até 7 m; acompanha o alvo durante no máximo 0,55 s. Dano de contato 6 e empurrão 2 m. Distância-base 3 m. Cooldown 7 s. |
| Skill 2 B | Impacto | Investida perseguidora: encontra alvo até 8 m e continua corrigindo a rota por até 0,55 s. Dano 6, empurrão 3,5 m, distância-base 3,3 m. Cooldown 8 s. |
| Skill 2 C | Avanço Defensivo | Avanço perseguidor até 7 m, por até 0,60 s. Dano 6, empurrão 1,5 m e iframe de 0,12 s. Distância-base 3 m. Cooldown 9 s. |
| Ultimate A | Postura de Guerra | Impacto curto ao ativar (8 dano em 1,8 m) e buff de 5 s: +12% dano, +35% resistência a stagger e +10% janela defensiva. Cooldown 30 s. |
| Ultimate B | Retaliação | Impacto curto de 8 dano e buff de 5 s: +8% dano, -8% movimento, +30% resistência e +35% janela defensiva. Cooldown 32 s. |
| Ultimate C | Avanço Implacável | Impacto curto de 8 dano e buff de 6 s: +18% dano, +8% movimento e +80% resistência a stagger. Cooldown 32 s. |

## Assassino

Base: 85 HP, 110 energia, 6,6 de movimento, 11 de regeneração de energia/s.

| Botão | Opção atual | Como funciona agora |
|---|---|---|
| Ataque | Corte Rápido | Combo de 3 cortes: 15, 15,75 e 20,7 de dano. Alcance 1,65 m, startup 0,10 s e recuperação 0,18 s. Pode destruir projéteis mágicos compatíveis. Cooldown 1,2 s. |
| Skill 1 A | Esquiva Sombria | Esquiva curta de 1,2 m. Durante 1 s ignora dano; se um ataque atinge durante a defesa, redireciona através do oponente por até 10 m e causa 2 de dano. Cooldown 5 s. |
| Skill 1 B | Duplo Passo | Hoje é uma única esquiva de 1,2 m com 1 s de invulnerabilidade. Se defendido um golpe, cruza o alvo por até 10 m e causa 3 de dano. **Ainda não é o duplo deslocamento com nova leitura do analógico.** Cooldown 3,2 s. |
| Skill 1 C | Contra-Sombra | Hoje não é contra-ataque: cria fumaça de raio 6 m por 4 s. Dentro dela o Assassino fica oculto para o inimigo e não pode atacar; golpes de fora ainda podem acertá-lo. Cooldown 12 s. |
| Skill 2 A | Passo Fantasma | Dash atravessável de 4,2 m, 0,18 s. Causa 24 dano ao cruzar inimigo. Cooldown 5 s. |
| Skill 2 B | Travessia | Dash atravessável de 4,6 m, 0,15 s, também causa 24 dano no cruzamento. Após concluir, +35% de movimento por 2 s. Cooldown 6 s. |
| Skill 2 C | Retorno | Primeiro uso: dash atravessável de 4 m, com dano de contato 24, e salva a origem. Segundo uso, dentro de 1,8 s: volta para a origem. Cooldown 7 s. |
| Ultimate A | Cinco Cortes | Ativação inicial + até quatro novos toques, totalizando cinco dashes separados em até 6 s. Cada dash tem 4,2 m, 0,16 s e 12 de dano de contato. A direção pode mudar a cada toque. Cooldown 28 s. |
| Ultimate B | Execução | Hoje é buff de 4 s, não uma sequência de execução: impacto de 8 dano em 1,8 m, +35% dano e -8% movimento. Cooldown 32 s. |
| Ultimate C | Caçada | Habilidade em dois toques. Primeiro: trava alvo visível até 12 m, segue-o por até 1,5 s, causa 6 e empurra 2 m. Se acertar, há 5 s para o segundo toque: segue o mesmo alvo, causa 18 e o atravessa, terminando 3,5 m depois dele. Cooldown 28 s. |

## Mago

Base: 95 HP, 120 energia, 5,4 de movimento, 10 de regeneração/s.

| Botão | A | B | C |
|---|---|---|---|
| Ataque | Orbe Arcano: 9 dano, alcance 8 m, curva no máximo 20° para o inimigo mais próximo; cooldown 0,85 s. | — | — |
| Skill 1 | Faíscas Caçadoras: 3 projéteis com tracking moderado, 4 cada, alcance 9 m, cooldown 6 s. | Orbe Pesado: projétil maior, 28 dano, área de contato 2,1 m, alcance 9 m, interceptável, cooldown 8,5 s. | Pântano de Lentidão: raio 3,8 m por 4 s, reduz 28% da velocidade e causa 3 dano a cada 0,5 s, cooldown 10 s. |
| Skill 2 | Blink: vulto de 4,5 m que atravessa e causa 6 dano; alma visual segue atrás, cooldown 7 s. | Ecos Arcanos: 3 clones móveis, cada um dispara orbe de 3 dano; segundo toque direcionado escolhe clone em até 2,5 s, cooldown 12 s. | Pulso de Repulsão: círculo de chamas de raio 3,2 m, 5 dano, empurra 4,5 m e reduz 30% da velocidade por 1,5 s, cooldown 9 s. |
| Ultimate | Convergência Arcana: projétil lento e segundo projétil rápido; se o rápido acerta o lento, explode em 4,5 m. 46 dano; 34 s se combinar, 14 s se falhar. | Tempestade Arcana: área de 5 m por 2,6 s com pulsos de 7 dano; cooldown 30 s. | Prisma Fraturado: 7 projéteis em leque de 72 graus, 9 dano cada, cooldown 26 s. |

## Arqueiro

Base: 95 HP, 110 energia, 5,8 de movimento, 9 de regeneração/s.

| Botão | A | B | C |
|---|---|---|---|
| Ataque | Disparo Preciso: 8 dano, alcance 15 m; mira automaticamente no inimigo mais próximo dentro do alcance, cooldown 0,75 s. | — | — |
| Skill 1 | Flechas Rastreadoras: 3 flechas com tracking, 4 cada, cooldown 6,5 s. | Flecha Pesada: tiro manual fino, 65 m/s, alcance 22 m; 20–36 dano conforme a distância, cooldown 9 s. | Armadilha Gravitacional: arma em 0,2 s, detona quando inimigo entra a 1,5 m; 4 dano e puxa em raio 4 m, cooldown 11 s. |
| Skill 2 | Recuo Ofensivo: mortal para trás de 3,5 m, dispara flecha de 4 dano que empurra, cooldown 7 s. | Gancho de Reposição: lança corda até 7 m e puxa o arqueiro; se acertar inimigo, aproxima e chuta por 5 dano com empurrão, cooldown 11 s. | Passos Laterais: até dois dashes de 2,5 m em 1,5 s; segunda direção pode ser diferente, cooldown 6,5 s. |
| Ultimate | Rajada Perfurante: 6 tiros em leque estreito, 8 dano cada, cooldown 25 s. | Sobrecarga Cinética: 4 s de +30% movimento, +50% frequência do ataque básico, 30% menos cooldown de mobilidade e curva nas flechas básicas; cooldown 28 s. | Chuva de Flechas: área de 3,5 m durante 3,5 s, pulsos de 5 dano a cada 0,35 s; cooldown 34 s. |

## Itens táticos atuais

- Poção de Cura: recupera 30 HP ao longo de 5 s.
- Poção de Essência: recupera 32 de energia; uso de 0,75 s e interrompível.
- Orbe de Recarga: reduz 2,5 s dos cooldowns; uso de 0,5 s e interrompível.
- Granada de Fumaça: fumaça de raio 3,5 m por 5 s.
- Orbe de Repulsão: efeito de 3 m, 0,25 s, força 8.
- Cristal de Barreira: barreira por 4 s.
- Selo Nulo: campo de 3 m por 3 s.

## Pontos que hoje não correspondem à proposta aprovada

1. Guerreiro: as três ultimates são buffs com impacto curto. Bastião Sísmico,
   Domínio do Caçador e Ruptura ainda não estão implementados como descritos.
2. Guerreiro: `Impacto` acompanha o alvo continuamente na curta janela; a
   proposta de `Caçada` previa apenas correção inicial e depois trajetória fixa.
3. Guerreiro: `Fortaleza` não é uma Guarda Arcana especializada em projéteis.
4. Assassino: `Duplo Passo` ainda não tem dois toques/direções independentes.
5. Assassino: `Contra-Sombra` virou a fumaça de fuga; portanto falta o
   contra-ataque preciso originalmente proposto.
6. Assassino: `Execução` é apenas um buff no estado atual; não é sequência
   ofensiva. `Caçada` é a habilidade nova em dois estágios que você descreveu,
   mas ocupa a terceira Ultimate e substituiu a ideia de Predação.
7. Assassino: Véu Fantasma com clones/engano não existe ainda como Ultimate.

Este documento é diagnóstico: não altera as regras do jogo. Use a seção final
como lista de decisões para a próxima rodada de implementação.
