# Battle Royale X — referência de timing do combate

Este é um guia de implementação original para o protótipo. Ele usa princípios públicos de legibilidade, resposta e leitura de ataque; não reproduz código, arte, personagens ou valores proprietários de outro jogo.

## Regras de sensação

- Todo golpe deve ter três fases claras: preparação, janela ativa e recuperação.
- A preparação usa animação, cor e som coerentes. O dano nunca surge antes dela.
- Efeitos fortes não escondem o personagem, o alvo nem a próxima decisão do jogador.
- O botão deve responder de imediato; o ataque pode começar em seguida, mas o jogador sempre recebe confirmação visual/sonora ao apertar.
- O combo é uma escolha de ritmo: o próximo golpe só é aceito perto do fim do anterior, sem exigir precisão impossível no touch.

## Tabela inicial de timings

Valores em segundos. São ponto de partida para testes no Moto G54, não valores finais de balanceamento.

| Ação | Preparação | Ativa | Recuperação | Buffer de comando | Objetivo |
| --- | ---: | ---: | ---: | ---: | --- |
| Ataque leve 1 | 0.10 | 0.10 | 0.18 | 0.18 | resposta imediata |
| Ataque leve 2 | 0.12 | 0.11 | 0.20 | 0.20 | continuação legível |
| Ataque leve 3 | 0.18 | 0.14 | 0.34 | 0.20 | finalizador com risco |
| Ataque pesado/área | 0.32 | 0.16 | 0.46 | 0.16 | alto impacto, fácil de ler |
| Dash do assassino | 0.05 | 0.16 | 0.12 | 0.12 | reposicionamento responsivo |
| Guarda | 0.06 | 0.42 | 0.14 | 0.10 | defesa sempre confiável |
| Parry perfeito | 0.00 | 0.11 | 0.28 | 0.10 | recompensa por leitura |
| Ultimate | 0.30 | instantânea | 0.25 | 0.15 | momento visível e respondível |

## Combo original proposto

### Assassino — Trilha da Sombra

1. **Corte rápido:** golpe curto, aproximação leve.
2. **Corte cruzado:** só encadeia se o comando chegar no buffer do primeiro golpe.
3. **Ruptura:** golpe final com rastro roxo; causa mais dano, mas deixa recuperação maior.

O dash cancela apenas a recuperação do primeiro ou segundo golpe. Ele não cancela o terceiro: assim existe mobilidade sem transformar o combo em dano sem risco.

Ultimate: **Caçada Sombria**. O jogador recebe aura dourada/roxa, onda de choque no início e bônus temporário já existente. Durante a aura, cada acerto deixa um breve marcador luminoso no alvo. Não cria uma nova mecânica de progressão.

### Guerreiro — Guarda de Impacto

1. **Golpe frontal:** alcance médio e telegraph curto.
2. **Varredura:** arco mais largo para controlar espaço.
3. **Impacto:** finalizador lento, com pequeno knockback.

O Guerreiro usa o parry como resposta ao ritmo do Assassino. O bot deve alternar ataque, aproximação e defesa em vez de apertar ataque sem pausa.

## Linguagem visual

| Evento | Cor | Forma/feedback |
| --- | --- | --- |
| Ataque básico | laranja | faísca curta no contato |
| Guarda | azul | arco compacto na frente |
| Dash | roxo | rastro curto na direção de saída |
| Parry | amarelo | flash concentrado e pausa mínima |
| Ultimate | dourado + cor da classe | aura contínua e onda circular |
| Cura | verde | pulso ascendente |
| Energia | ciano | partículas subindo |
| Airdrop | laranja | farol vertical e rótulo no mundo |

## Critérios para aceitar cada ajuste

- Em tela de celular, o jogador identifica ataque, dash, guarda e ultimate sem ler a HUD.
- Um ataque adversário tem telegraph suficiente para permitir uma defesa intencional, não uma reação por sorte.
- O jogador consegue encadear o combo básico sem apertar no frame exato.
- A ultimate chama atenção, mas os dois personagens continuam visíveis.
- Se o combate ficar visualmente confuso, reduzir partículas é preferível a aumentar dano ou velocidade.

## Próxima implementação recomendada

1. Transformar o ataque básico atual em uma cadeia leve de três golpes para cada classe.
2. Adicionar buffer de comando e feedback de hit-stop muito curto no impacto.
3. Dar ao bot uma cadência deliberada que ensine esses ataques, dash e defesa.
4. Testar no Moto G54 e ajustar primeiro clareza e resposta; depois dano/cooldown.
