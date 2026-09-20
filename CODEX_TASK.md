# CODEX TASK CHANNEL — Battle Royale X

> ESTE ARQUIVO É EXECUTÁVEL COMO INSTRUÇÃO DE TRABALHO.
> Quando o usuário disser apenas **"leia CODEX_TASK.md e execute"**, não peça que ele cole outro prompt.
> Atualize a branch `main`, leia este arquivo inteiro e execute a seção `TAREFA ATUAL`.

---

# COMANDO HUMANO MÍNIMO

O único comando que o usuário precisa enviar ao Codex é:

> **Leia `CODEX_TASK.md` na branch `main` do repositório `SbitContainer/Battle-Royale-X`, atualize sua cópia local e execute integralmente a TAREFA ATUAL.**

Todo o restante das instruções necessárias está neste arquivo.

---

# PROTOCOLO DO CODEX

Antes de alterar qualquer arquivo:

1. Execute:
   - `git fetch origin`
   - `git checkout main`
   - `git pull origin main`
2. Leia este arquivo inteiro.
3. Leia os documentos obrigatórios indicados em `REFERÊNCIAS`.
4. Faça uma compilação/checagem inicial do estado atual antes de editar.
5. Execute a tarefa na **ORDEM DE IMPLEMENTAÇÃO** definida abaixo.
6. Não amplie escopo.
7. Não redesenhe sistemas que já possuem decisão explícita neste arquivo.
8. Quando houver escolha técnica pequena, use a solução de menor impacto arquitetural.
9. Se encontrar código mais recente que já implemente parte desta tarefa, preserve-o e adapte-o em vez de criar sistema duplicado.
10. Não implemente Mago ou Arqueiro.
11. Não implemente multiplayer.
12. Ao terminar, execute os testes obrigatórios.
13. Faça commit na `main`.
14. Atualize somente `STATUS` e `RELATÓRIO DO CODEX` deste arquivo com o resultado final.

Não aguarde confirmação intermediária para decisões que já estão definidas aqui.

---

# STATUS

- Task ID: `BRX-LAB-001`
- Estado: `READY`
- Prioridade: `ALTA`
- Implementação ativa: `Guerreiro + Assassino`
- Bot/slot adversário do laboratório: `sempre Guerreiro`
- Classe do jogador no laboratório: `Guerreiro ou Assassino`
- Mago/Arqueiro: `somente design registrado; NÃO implementar`
- Modelo recomendado: `GPT-5.6 Sol`
- Raciocínio recomendado: `Medium`

---

# REFERÊNCIAS OBRIGATÓRIAS

Leia antes de editar:

- `README.md`
- `Docs/CODEX_START_HERE.md`
- `Docs/COMBAT_PRINCIPLES.md`
- `Docs/CLASS_ROSTER_AND_VARIATIONS.md`
- `Docs/MATCHUP_MATRIX.md`
- `Docs/TEST_MATRIX.md`

Regra de precedência em caso de conflito:

1. `CODEX_TASK.md`
2. `Docs/COMBAT_PRINCIPLES.md`
3. `Docs/CLASS_ROSTER_AND_VARIATIONS.md`
4. demais documentos
5. implementação antiga

---

# MAPA DO CÓDIGO ATUAL

Use estes arquivos como pontos principais. Não desperdice tempo procurando outra arquitetura antes de inspecioná-los.

## Estado e controle

### `Assets/BattleRoyaleX/Runtime/Characters/CharacterStateController.cs`

Hoje:
- possui `IsInvulnerable`;
- possui `IsStaggered`;
- `InputLocked` mistura stagger com locks manuais;
- `ApplyStagger()` retira controle amplo.

Alterar aqui para separar:
- movement lock;
- skill lock;
- basic attack lock;
- action/recovery.

---

## Execução de habilidades

### `Assets/BattleRoyaleX/Runtime/Abilities/AbilityController.cs`

Hoje:
- `TryUse()` rejeita tudo quando `InputLocked`;
- `Execute()` usa um único fluxo para ataque/defesa/movimento/ultimate;
- recovery chama `LockInput(true)`;
- ataque básico e skill não possuem cancelamento/prioridade explícita;
- hitboxes são criadas dinamicamente.

Alterar aqui para:
- permitir skill cancelar ataque básico;
- impedir basic durante skill incompatível;
- controlar recovery sem necessariamente bloquear movimento;
- acompanhar/cancelar a ação/hitbox ativa quando necessário;
- manter cooldown/energia existentes.

---

## Hitboxes

### `Assets/BattleRoyaleX/Runtime/Combat/Hitbox.cs`

Hoje:
- cada hitbox possui `Cancel()`.

Usar esse mecanismo para garantir que:
- ao cancelar ataque básico para iniciar skill, a hitbox básica pendente/ativa não continue causando dano;
- não existam dois efeitos ofensivos simultâneos decorrentes do mesmo cancelamento.

Não criar um sistema global complexo se uma referência da hitbox ativa no `AbilityController` resolver.

---

## Resolução de combate

### `Assets/BattleRoyaleX/Runtime/Combat/CombatResolver.cs`

Hoje:
- Parry usa `ApplyStagger(0.32f)`;
- Clash usa `ApplyStagger(0.14f)`.

Alterar para:
- remover hard lock desnecessário;
- preservar feedback/interrupt/recovery;
- movimento deve continuar disponível quando a intenção é somente micro-impacto;
- preparar aplicação de SkillLock de 0,10 s para ataque pesado do Guerreiro.

---

## Dados das habilidades/personagens

### `Assets/BattleRoyaleX/Runtime/Data/AbilityDefinition.cs`

Adicionar somente campos data-driven necessários para esta tarefa.

Preferência:
- adicionar parâmetro de `skillLockOnHit` ou equivalente para ataques que precisam bloquear skills por poucos milissegundos;
- não hardcode 0,10 s dentro do resolver se puder ser dado da habilidade.

### `Assets/BattleRoyaleX/Runtime/Data/CharacterDefinition.cs`

Usar como fonte de velocidade base.

Meta atual:
- Guerreiro: `4.9`
- Assassino: `6.6`

---

## Dados default

### `Assets/BattleRoyaleX/Editor/PrototypeDataFactory.cs`

Hoje:
- Guerreiro = 4.9;
- Assassino = 6.2;
- cura = 24 HP após uso de 1.6 s;
- gera habilidades e variações Base/A/B.

Alterar:
- Assassino para 6.6;
- configuração da cura para HoT;
- parâmetro do micro SkillLock do ataque pesado do Guerreiro;
- manter Guerreiro 4.9;
- não redesenhar kits nesta tarefa.

---

## Movimento

### `Assets/BattleRoyaleX/Runtime/Characters/CharacterMotor25D.cs`

Verificar apenas:
- se `CharacterDefinition.moveSpeed` está sendo aplicado corretamente;
- se algum lock atual impede a diferença de velocidade de aparecer em runtime.

Não implementar tracking automático do Guerreiro.

---

## Vida

### `Assets/BattleRoyaleX/Runtime/Characters/HealthComponent.cs`

Hoje:
- já possui `Changed(current,max)`;
- já possui `Heal(amount)`.

Preferência:
- implementar regeneração contínua aqui ou em um componente pequeno dedicado;
- não colocar lógica de cura dentro da UI;
- uma cura nova deve substituir/renovar a anterior, não empilhar infinitamente.

Se criar componente dedicado, nome preferido:
- `Assets/BattleRoyaleX/Runtime/Characters/HealthRegenerationController.cs`

---

## Itens

### `Assets/BattleRoyaleX/Runtime/Data/ItemDefinition.cs`

Adicionar dados necessários para HoT de forma explícita.

Preferência:
- `healDuration` ou equivalente;
- manter `amount` como cura total.

### `Assets/BattleRoyaleX/Runtime/Inventory/InventoryController.cs`

Hoje:
- trava input durante `useDuration`;
- cura é aplicada instantaneamente ao final;
- dano pode cancelar uso.

Alterar especificamente para `ItemKind.Heal`:
- ativação rápida;
- não bloquear movimento/ataque/skills durante a regeneração;
- dano não cancela HoT;
- consumir item ao ativar com sucesso;
- iniciar/renovar regeneração.

Não alterar o comportamento dos outros itens sem necessidade.

---

## Runtime principal

### `Assets/BattleRoyaleX/Runtime/Characters/CharacterRuntime.cs`

Usar para:
- expor eventual componente novo de regeneração;
- reinicializar definição quando o laboratório trocar a classe do jogador, se necessário.

Não alterar identidade do bot Guerreiro.

---

## HUD atual

### `Assets/BattleRoyaleX/Runtime/Debug/PrototypeDebugHUD.cs`

Hoje:
- mostra texto de HP/energia/inventário.

Expandir para laboratório:
- mostrar classe atual;
- mostrar variação equipada de Defesa;
- mostrar variação equipada de Movimento;
- mostrar variação equipada de Ultimate;
- fornecer controles simples para alternar Base/A/B;
- fornecer seletor Guerreiro/Assassino para o jogador;
- mostrar Mago/Arqueiro como não implementados somente se isso não poluir a UI.

Não criar UI final.

Se ficar mais limpo, criar:
- `Assets/BattleRoyaleX/Runtime/Debug/PrototypeLabController.cs`

e deixar `PrototypeDebugHUD` somente como apresentação.

---

## Cena de teste

### `Assets/BattleRoyaleX/Editor/PrototypeSceneBuilder.cs`

Hoje:
- cria P1 Guerreiro;
- cria P2 Assassino;
- cria HUD;
- cria arena/pickups.

Alterar somente o necessário para o laboratório:
- garantir um slot adversário fixo em Guerreiro;
- garantir um slot de jogador capaz de usar Guerreiro ou Assassino;
- conectar o controlador/HUD de laboratório;
- conectar barras de vida;
- não implementar IA complexa se não existir uma já funcional.

### IMPORTANTE SOBRE "BOT"

Se a branch atual ainda não possuir um controlador de IA real:
- NÃO criar uma IA sofisticada nesta tarefa;
- trate o slot adversário como `Bot/Warrior Slot` fixo em Guerreiro para a infraestrutura;
- preserve qualquer controlador de bot que já tenha sido adicionado por trabalho posterior;
- a tarefa atual é laboratório/infraestrutura, não desenvolvimento de IA.

---

## Barra de vida

Criar preferencialmente um componente isolado, por exemplo:

- `Assets/BattleRoyaleX/Runtime/Debug/PrototypeWorldHealthBar.cs`

Requisitos:
- subscribir em `HealthComponent.Changed`;
- representar `CurrentHealth / MaxHealth`;
- nenhum cálculo de dano/cura dentro da UI;
- acompanhar o personagem;
- manter orientação legível para câmera;
- funcionar para Guerreiro e Assassino;
- simples, provisória e robusta.

---

# TAREFA ATUAL — BRX-LAB-001

## Objetivo

Transformar a arena atual em um laboratório rápido para validar Guerreiro x Assassino antes de redesenhar os kits definitivos.

Não implementar Mago ou Arqueiro.

---

# ORDEM DE IMPLEMENTAÇÃO

Siga esta ordem para reduzir retrabalho.

## ETAPA 1 — Baseline

1. Atualizar `main`.
2. Compilar o estado atual.
3. Registrar qualquer erro pré-existente.
4. Não atribuir à tarefa erros que já existiam antes das mudanças.

---

## ETAPA 2 — Separar locks

Modificar principalmente:

- `CharacterStateController.cs`
- `CharacterMotor25D.cs`
- `AbilityController.cs`

Resultado desejado:

Propriedades/estados equivalentes a:
- `MovementLocked`
- `SkillsLocked`
- `BasicAttackLocked`
- `IsInActionRecovery`

Não é obrigatório usar exatamente esses nomes, mas a semântica deve ser separada.

### Regras

- MovementLock impede movimento.
- SkillLock impede Defesa/Movimento/Ultimate.
- BasicAttackLock impede somente ataque básico.
- Recovery restringe a próxima ação apropriada, sem automaticamente congelar deslocamento.
- `InputLocked` antigo pode permanecer por compatibilidade, mas não deve continuar sendo a única decisão para tudo.

---

## ETAPA 3 — Prioridade de skills sobre ataque básico

Modificar principalmente:

- `AbilityController.cs`
- `Hitbox.cs`
- se necessário `PrototypeLocalInput.cs`

### Regra

Se ataque básico está em:
- startup;
- active;
- recovery;

e o usuário aperta:
- Defesa;
- Movimento;
- Ultimate;

a skill tem prioridade.

### Comportamento

1. cancelar coroutine/estado do basic atual;
2. cancelar hitbox básica ainda ativa;
3. limpar recovery exclusivo do basic;
4. executar a skill no mesmo input;
5. não cobrar input duas vezes.

### Inverso

Se skill incompatível estiver ativa:
- basic não inicia até ficar permitido.

### Invariante

Nunca permitir que basic antigo cause dano depois de já ter sido cancelado por uma skill.

---

## ETAPA 4 — Micro SkillLock do Guerreiro

Modificar principalmente:

- `AbilityDefinition.cs`
- `PrototypeDataFactory.cs`
- `CombatResolver.cs`
- `CharacterStateController.cs`

Configuração inicial:

- ataque básico pesado do Guerreiro:
  - `skillLockOnHit = 0.10 s`
  - movimento do alvo permanece liberado.

O alvo:
- pode continuar andando;
- não pode disparar nova skill nesses 0,10 s;
- não deve sofrer hard stun.

Não aumentar duração.

---

## ETAPA 5 — Parry e Clash sem hard stun

Modificar principalmente:

- `CombatResolver.cs`
- `CharacterStateController.cs`

Hoje:
- Parry: ~0.32 s de stagger amplo.
- Clash: ~0.14 s de stagger amplo.

Novo objetivo:
- cancelar/interromper ação ofensiva quando necessário;
- permitir feedback de impacto;
- preservar eventos de VFX;
- não congelar movimento por longos períodos.

Não criar nova cadeia de CC.

---

## ETAPA 6 — Velocidade real

Modificar/verificar:

- `PrototypeDataFactory.cs`
- `CharacterMotor25D.cs`
- dados gerados existentes quando necessário.

Valores iniciais:
- Guerreiro: `4.9`
- Assassino: `6.6`

### Teste obrigatório

Sem skills:
- colocar ambos lado a lado;
- mover na mesma direção por 5 s;
- Assassino precisa terminar claramente à frente.

Se isso não ocorrer:
- corrigir aplicação de velocidade/locks;
- não aumentar velocidade às cegas;
- não adicionar tracking ao Guerreiro.

---

## ETAPA 7 — Cura contínua

Modificar principalmente:

- `ItemDefinition.cs`
- `InventoryController.cs`
- `HealthComponent.cs` OU novo `HealthRegenerationController.cs`
- `CharacterRuntime.cs`
- `PrototypeDataFactory.cs`

Configuração inicial:
- cura total: `30 HP`
- duração: `5.0 s`

### Regras

Ao usar poção:
- efeito começa rapidamente;
- item é consumido;
- personagem continua andando;
- continua atacando;
- continua usando skills;
- dano recebido não cancela regeneração.

### Stacking

Não empilhar várias instâncias.

Ao usar nova poção durante HoT ativo:
- renovar/reiniciar/substituir a regeneração atual;
- nunca somar dois HoTs independentes.

### UI

Cada tick/chunk de cura deve passar por `HealthComponent.Heal()` ou equivalente para disparar `Changed`.

---

## ETAPA 8 — Barra de vida

Criar:
- `PrototypeWorldHealthBar.cs` ou equivalente simples.

Conectar por:
- `PrototypeSceneBuilder.cs`

Usar:
- `HealthComponent.Changed`

Visual de laboratório:
- fundo;
- preenchimento;
- atualização suave opcional;
- sempre legível;
- sem gameplay na UI.

A barra deve:
- diminuir com dano;
- subir progressivamente durante HoT.

---

## ETAPA 9 — Painel de laboratório / troca de variações

Modificar:
- `PrototypeDebugHUD.cs`
- opcional novo `PrototypeLabController.cs`
- `AbilityController.cs`
- `CharacterRuntime.cs`
- `PrototypeSceneBuilder.cs`

### Troca livre no laboratório

Permitir escolher instantaneamente:

Defesa:
- Base
- A
- B

Movimento:
- Base
- A
- B

Ultimate:
- Base
- A
- B

### Importante

- não consumir runa;
- não remover sistema normal de runas;
- método de troca livre deve existir somente em modo de laboratório;
- Basic Attack nunca possui variação.

Se necessário, adicionar no `AbilityController` método explícito de laboratório, por exemplo:
- `EquipLabVariation(slot, index)`

Não reutilizar artificialmente consumo de runa para simular o laboratório.

---

## ETAPA 10 — Seletor de classe

No laboratório:

Adversário/bot slot:
- sempre Guerreiro.

Jogador:
- Guerreiro;
- Assassino.

Mago:
- não implementar.

Arqueiro:
- não implementar.

Se Mago/Arqueiro forem exibidos:
- desabilitado;
- texto `Ainda não implementado`.

### Troca

Ao trocar Guerreiro ↔ Assassino:
- reinicializar definição;
- restaurar vida/energia para os máximos;
- carregar loadout base da classe;
- limpar estados temporários/cooldowns que não podem atravessar troca;
- manter o adversário como Guerreiro.

Não implementar troca durante uma partida real; isso é ferramenta de laboratório.

---

# O QUE NÃO FAZER

Nesta tarefa NÃO:

- implementar Mago;
- implementar Arqueiro;
- criar novas habilidades definitivas;
- rebalancear completamente Guerreiro;
- rebalancear completamente Assassino;
- criar perseguição automática;
- criar IA avançada;
- implementar multiplayer;
- matchmaking;
- backend;
- ranking;
- progressão;
- equipamentos;
- VFX finais;
- sangue final;
- animações finais;
- comprar/importar assets;
- fazer grande refatoração sem necessidade.

---

# TESTES OBRIGATÓRIOS

Atualizar `Docs/TEST_MATRIX.md`.

Validar no mínimo:

## Compilação
- [ ] nenhum novo erro C#;
- [ ] arena continua sendo criada;
- [ ] menu de geração continua funcionando.

## Locks
- [ ] SkillLock bloqueia skill;
- [ ] SkillLock não impede movimento;
- [ ] MovementLock funciona independentemente;
- [ ] recovery não congela movimento sem necessidade.

## Basic x Skill
- [ ] basic em startup pode ser cancelado por skill;
- [ ] basic em active é cancelado sem hit fantasma;
- [ ] basic em recovery pode ceder prioridade à skill quando especificado;
- [ ] skill começa no mesmo input;
- [ ] basic não inicia durante skill incompatível.

## Velocidade
- [ ] Guerreiro 4.9;
- [ ] Assassino 6.6;
- [ ] corrida de 5 s mostra Assassino claramente à frente.

## Cura
- [ ] 30 HP totais em 5 s;
- [ ] cura é progressiva;
- [ ] personagem anda durante cura;
- [ ] pode atacar durante cura;
- [ ] pode usar skill durante cura;
- [ ] tomar dano não cancela cura;
- [ ] segunda poção não cria stacking infinito;
- [ ] item é consumido corretamente.

## Barra de vida
- [ ] responde ao dano;
- [ ] responde a cada progressão da cura;
- [ ] MaxHealth diferente funciona;
- [ ] funciona em Guerreiro;
- [ ] funciona em Assassino.

## Laboratório
- [ ] adversário permanece Guerreiro;
- [ ] jogador pode alternar Guerreiro/Assassino;
- [ ] Mago/Arqueiro não ficam jogáveis;
- [ ] Defesa Base/A/B troca instantaneamente;
- [ ] Movimento Base/A/B troca instantaneamente;
- [ ] Ultimate Base/A/B troca instantaneamente;
- [ ] Basic não possui variação;
- [ ] troca de laboratório não consome runas;
- [ ] sistema normal de runas continua existente.

## Regressão
- [ ] Guard ainda reduz dano;
- [ ] Parry ainda funciona;
- [ ] Dodge/iframe ainda funciona;
- [ ] Clash ainda funciona;
- [ ] inventário continua aceitando itens;
- [ ] cooldown/energia continuam funcionando.

---

# CRITÉRIO DE CONCLUSÃO

Só marcar DONE quando:

1. projeto compilar;
2. nenhuma regressão crítica do combate base;
3. locks separados estiverem funcionando;
4. skill cancelar basic corretamente;
5. SkillLock de 0,10 s não bloquear movimento;
6. Assassino realmente for mais rápido em runtime;
7. cura contínua funcionar em combate;
8. barras de vida funcionarem;
9. painel trocar variações livremente;
10. jogador puder trocar Guerreiro/Assassino no laboratório;
11. slot adversário permanecer Guerreiro;
12. `Docs/TEST_MATRIX.md` estiver atualizado;
13. alterações estiverem commitadas;
14. relatório abaixo estiver preenchido.

---

# FORMATO DO COMMIT

Preferência de mensagem:

`feat: add combat lab controls and test infrastructure`

Se forem necessários commits intermediários, usar mensagens objetivas.

---

# RELATÓRIO DO CODEX

> NÃO preencher antes de executar a tarefa.

Ao terminar, substitua o conteúdo desta seção por:

- Estado: `DONE` / `PARTIAL` / `BLOCKED`
- Commit final:
- Branch:
- Arquivos criados:
- Arquivos alterados:
- Compilação inicial:
- Compilação final:
- Testes executados:
- Testes aprovados:
- Testes falhos:
- Regressões encontradas:
- Problemas não resolvidos:
- Decisões técnicas tomadas:
- Diferenças em relação à especificação:
- Próxima recomendação objetiva:

Se algum item ficar PARTIAL/BLOCKED, explique a causa concreta e não declare a tarefa concluída.
