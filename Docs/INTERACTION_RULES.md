# Catálogo numerado das interações

Inventário do runtime em 03/10/2026. A numeração I identifica a interação; R identifica a regra de elegibilidade. São dimensões diferentes. Este catálogo descreve o comportamento existente, sem converter todas as defesas para regras novas ou declarar ultimates imbloqueáveis.

## Interações já implementadas

| Código | Interação | Condição atual | Resultado atual / fonte |
|---|---|---|---|
| I01 | Bloqueio / guarda | Defesa Guard ativa e ataque `blockable` | Aplica `damageReduction`; empurrão cai para 25%. `DefenseController`, `CombatResolver`. |
| I02 | Parry | Defesa Parry ativa e ataque `parryable`; janela perfeita, uso não consumido e cooldown especial pronto | Perfeito: zero dano, cancela golpe, interrompe ação e micro-stagger 0,10 s no agressor. Fora da janela: bloqueio parcial. |
| I03 | Esquiva / invulnerabilidade | Janela de invulnerabilidade ativa | Zero dano. A imunidade é também conferida por HealthComponent; não depende de `blockable`. |
| I04 | Anulação defensiva | Defesa Nullify e ataque `nullifiable`, cooldown especial pronto | Zero dano e cancela hitbox. |
| I05 | Reflexão defensiva | Defesa Reflect e ataque `reflectable`, cooldown especial pronto | Zero dano no defensor, aplica dano ao agressor e cancela hitbox. Não é a reflexão geométrica do mapa. |
| I06 | Choque físico | Ambos ataques Physical e ambos `clashable`, equipes diferentes | Cada ator recebe dano do outro × `clashDamageFactor`, ambos sofrem micro-stagger 0,14 s e as hitboxes são canceladas. |
| I07 | Choque mágico / projéteis | Dois ataques Magical ou Projectile | Cancela ambos; explosão: 35% da soma dos danos, raio maior dos dois (mínimo 1 m). Atualmente a explosão pode acertar os próprios envolvidos e não passa pelo bloqueio normal. |
| I08 | Golpe físico anula magia | Physical com `canDestroyMagicalProjectiles` e cooldown de interação pronto | Cancela os dois golpes; usa cooldown separado `AttackNullifyMagic`, definido no ataque. |
| I09 | Interceptação do Guerreiro | Golpe Physical do Guerreiro encontra ataque `interceptable`, ainda não interceptado | Cancela golpe físico; reduz dano do projétil por `interceptDamageReduction` (padrão 60%); destrói projétil somente se `destroyWhenIntercepted`. Tem prioridade sobre I08. |
| I10 | Contra-ataque manual | Parry perfeito do Guerreiro ou defesa com `counterOnBlock` | Abre janela para próximo golpe básico receber bônus de dano/empurrão; defender não ataca automaticamente. |
| I11 | Redirecionamento defensivo | Esquiva reativa com `redirectOnDefense`, agressor válido e janela disponível | Reage uma vez, atravessa agressor, causa dano baixo; paredes e limite de distância restringem deslocamento. |
| I12 | Fumaça | Ator dentro de SmokeField com supressão ativa | Não inicia/aplica ataque; oculta perspectiva inimiga, sem invulnerabilidade. Ataques originados fora podem acertar quem está dentro. |
| I13 | Campo nulo do mapa | Hitbox entra em NullField e ataque `nullifiable` | Cancela hitbox; independente da defesa do personagem. |
| I14 | Parede de fase | Projétil reconhecido entra em PhaseWall | Cancela e destrói projétil, independentemente de `nullifiable`. |
| I15 | Parede prismática | Projétil `reflectable` e ainda não refletido | Reflete direção geometricamente uma vez; não devolve dano imediato nem troca dono/equipe. |
| I16 | Barreira amplificadora | Projétil `amplifiable` ou tag Amplifiable; ainda não amplificado | Dano × 1,12 uma vez. |
| I17 | Cristal fragmentador | Ataque `fragmentTrigger` ou tag FragmentTrigger; ainda não fragmentado | Cria 8 fragmentos com 22% do dano e 75% da velocidade; cancela/destrói original. |
| I18 | Arbusto reativo | Ataque Heavy ou Area reconhecido pelo objeto | Oculta arbusto por 18 s. |
| I19 | Repulsão / lentidão | Golpe Repulsion acerta ou é bloqueado | Empurra e aplica lentidão configurada; não cria stun prolongado. |

`Seeking` é regra de mira, não de bloqueio; `Heavy` não significa imbloqueável. Flags de compatibilidade e tags não são um ranking de força. Uma habilidade pode participar de várias interações. A lista de todos os IDs e flags efetivos fica em [ABILITY_INTERACTIONS.md](ABILITY_INTERACTIONS.md), exportada pelo Editor.

## Regras reutilizáveis de elegibilidade

- **R00:** sem perfil; preserva as flags e condições antigas acima. Não é “sem defesa”.
- **R01 / código 1:** todos os ataques são elegíveis.
- **R02 / código 2:** todos são elegíveis, exceto `excludedAbilityIds` no asset do perfil. IDs estáveis, não nomes visuais. Um ataque sem ID conhecido é recusado por esta regra.

O perfil não transforma guarda em imunidade, nem elimina o requisito `parryable` de um parry. Define compatibilidade; o mecanismo ainda determina janela, chance, redução, consumo e cancelamento. `Defense_Code_1.asset` e `Defense_Code_2.asset` podem ser reutilizados; para uma defesa com lista diferente, crie outro perfil R02, sem modificar silenciosamente todas as defesas que compartilham o perfil.

## Nova interação pedida nesta tarefa

**I20 — Repulsão por adagas orbitais:** Círculo de Adagas, 3 s, cinco adagas, 50% por golpe elegível, dano de proximidade 2 a cada 0,5 s. Sorteio ocorre uma vez por golpe/ator, não por collider. Não devolve dano ao agressor. Utiliza R02; lista de exceções ainda vazia conforme decisão do usuário. Nenhuma ultimate foi excluída automaticamente.

Novas variantes: Execução persegue e golpeia por 18, depois oculta por 2 s (sem imunidade); Travessia lança adaga por 4, segundo toque em até 3 s teleporta e golpeia por 12. Estes números são baseline ajustável, não novos requisitos de balanceamento de outras classes.

## Limites observados, não alterados por este inventário

Explosão I07 e dano de convergência aplicado diretamente pelo resolver não usavam a mesma passagem de hitbox das defesas tradicionais. A defesa orbital nova é conferida também nessas duas rotas, com o mesmo perfil/chance; guardas/parries antigos foram preservados. Mudar guardas/parries nessas rotas exige decisão de regra, não uma migração silenciosa. A adaga utiliza a identidade de projétil existente para que a parede de fase possa cancelá-la, incluindo seu teleporte. Nenhum efeito de marcação/puxão, alcance ou animação de outras classes foi redesenhado nesta tarefa.
