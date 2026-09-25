# Sangue e morte — protótipo

## Regra de apresentação

- Sangue é apenas feedback visual e nunca aplica dano adicional.
- `Hit` confirmado produz gotas direcionais e uma névoa curta no contato.
- Bloqueio parcial produz menos gotas.
- Parry, esquiva, imunidade e dano negado não produzem sangue.
- O efeito usa o shader transparente `BloodSoft`, evitando partículas quadradas e brilho mágico aditivo.

## Morte

- O primeiro dano que reduz a vida a zero emite um único evento `Death`.
- A sequência combina spray maior, névoa curta, marca irregular temporária no chão e animação `Death01`.
- O modo de treino mantém a morte visível até o reinício automático, que restaura vida, posição e Animator.
- A sequência é somente local/presentacional e não altera o resultado do combate.
