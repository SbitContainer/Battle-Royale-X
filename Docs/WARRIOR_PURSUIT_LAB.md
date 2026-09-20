# Laboratório de perseguição do Guerreiro

Esta etapa substitui somente a antiga restrição que proibia tracking automático do Guerreiro. A autorização atual do usuário exige perseguição curta e testável; as demais regras de combate e o controle contínuo permanecem.

## Regras

- Investida de Escudo, Impacto e Avanço Defensivo podem adquirir um inimigo vivo e visível à frente.
- A perseguição é limitada por alcance, velocidade e duração definidos na habilidade. Não continua indefinidamente.
- A direção é corrigida durante a viagem se o alvo se mover.
- Paredes continuam bloqueando o personagem e o dano.
- O contato causa dano uma vez por ativação.
- Investida empurra 2 m; Impacto empurra 3,5 m; Avanço Defensivo empurra 1,5 m.
- Impacto é a opção de repulsão mais forte. O counter defensivo continua exigindo defesa bem-sucedida seguida de ataque básico manual.
- O empurrão do counter usa a direção real Guerreiro→alvo, evitando lançamento lateral causado por uma direção antiga da hitbox.

## Controles de teste

- `1`: Defesa, alterna Base → A → B → Base.
- `2`: Movimento, alterna Base → A → B → Base.
- `3`: Ultimate, alterna Base → A → B → Base.
- `BOT`: alterna Normal → Parado → Parado + ataque → Normal.

`Parado + ataque` nunca anda nem usa skills: ele apenas olha para o jogador e repete o ataque básico quando o alvo está no alcance. Isso permite validar guarda, parry, esquiva e counter sem interferência da navegação.
