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
- [ ] Ambos sofrem pequeno stagger.
- [ ] Hitboxes são canceladas depois do Clash.

## Defesa
- [ ] Guarda reduz dano.
- [ ] Parry perfeito zera dano e staggera atacante.
- [ ] Parry fora da janela perfeita vira defesa parcial.
- [ ] Esquiva durante iframe evita dano.

## Movimento especial
- [ ] Dash percorre distância configurada.
- [ ] Travessia permite cruzar o adversário.
- [ ] Retorno permite voltar dentro da janela.
- [ ] Retorno expira corretamente.

## Ultimate
- [ ] Buff inicia e expira.
- [ ] Modificadores voltam a 1.0 ao fim.
- [ ] Caçada reduz dano e aumenta mobilidade conforme dados.
- [ ] Execução aumenta ameaça mas não remove controle do adversário automaticamente.

## Inventário
- [ ] Mochila inicia com 3 slots.
- [ ] Não aceita 4º item sem upgrade.
- [ ] Mochila de drop aumenta para 4.
- [ ] Item usado desaparece.
- [ ] Cura tem tempo de uso e pode ser interrompida.
- [ ] Essência tem tempo menor que cura.
- [ ] Troca de variação demora ~0,8 s e pode ser interrompida por dano.
- [ ] Variação só é consumida quando troca conclui.

## Itens táticos
- [ ] Repulsão desloca o adversário.
- [ ] Barreira cria obstáculo temporário.
- [ ] Campo nulo cancela ataques nullifiable.
- [ ] Fumaça aparece como placeholder de área.

## Airdrop
- [ ] Surge após o tempo configurado.
- [ ] Apresenta três opções.
- [ ] Pegar uma elimina as outras.

## Framework futuro de magia
- [ ] Dois hitboxes mágicos/projéteis colidindo geram explosão em área.
- [ ] Ataque físico marcado `canDestroyMagicalProjectiles` anula magia.
- [ ] A propriedade especial entra em cooldown por 30 s.
- [ ] Durante cooldown, novo ataque físico não anula outra magia.
