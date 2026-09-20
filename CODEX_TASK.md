# CODEX TASK CHANNEL — Battle Royale X

> Este arquivo é o canal oficial de tarefas entre o planejamento do projeto e o Codex.
> Sempre trabalhe a partir da branch `main`.

## COMO O CODEX DEVE USAR ESTE ARQUIVO

1. Execute `git fetch origin` e atualize `main`.
2. Leia este arquivo inteiro.
3. Execute **somente** a seção `TAREFA ATUAL`.
4. Leia os arquivos de referência indicados antes de alterar código.
5. Não amplie o escopo por iniciativa própria.
6. Preserve decisões registradas em `Docs/COMBAT_PRINCIPLES.md`, `Docs/CLASS_ROSTER_AND_VARIATIONS.md` e `Docs/MATCHUP_MATRIX.md`.
7. Ao terminar, atualize apenas as seções `STATUS` e `RELATÓRIO DO CODEX` deste arquivo.
8. Faça commit das alterações na branch `main`, salvo instrução contrária explícita nesta tarefa.

---

## STATUS

- Task ID: `BRX-LAB-001`
- Estado: `READY`
- Prioridade: `ALTA`
- Implementação ativa: `Guerreiro + Assassino`
- Bot de teste: `sempre Guerreiro`
- Mago/Arqueiro: `somente design registrado; NÃO implementar`
- Modelo recomendado: `GPT-5.6 Sol`
- Raciocínio recomendado: `Medium`

---

# TAREFA ATUAL

## Objetivo

Melhorar a infraestrutura do laboratório de combate antes de continuar o redesenho completo das habilidades.

**Não implementar Mago ou Arqueiro nesta tarefa.**
**Não redesenhar ainda os kits completos de Guerreiro/Assassino.**

Antes de editar código, leia:

- `README.md`
- `Docs/CODEX_START_HERE.md`
- `Docs/COMBAT_PRINCIPLES.md`
- `Docs/CLASS_ROSTER_AND_VARIATIONS.md`
- `Docs/MATCHUP_MATRIX.md`
- `Docs/TEST_MATRIX.md`

---

## 1. MODO DE TESTE DE VARIAÇÕES

Criar um modo de laboratório que permita trocar durante a partida, de forma instantânea e sem consumir runas:

- Defesa: Base / A / B
- Movimento: Base / A / B
- Ultimate: Base / A / B

Regras:

- disponível somente no modo de teste;
- não remover o sistema normal de runas;
- mostrar claramente qual variação está equipada;
- esse modo existe para validar matchups rapidamente antes da regra final de runas.

---

## 2. SELETOR DE CLASSE DO LABORATÓRIO

Nesta fase:

- o bot permanece sempre `Guerreiro`;
- o jogador pode escolher a própria classe no painel/app de teste.

Classes:

- Guerreiro — disponível
- Assassino — disponível
- Mago — registrado, mas NÃO implementar
- Arqueiro — registrado, mas NÃO implementar

Mago e Arqueiro devem aparecer desabilitados ou marcados como `Ainda não implementado`, caso apareçam no seletor.

---

## 3. REFATORAR LOCKS DE COMBATE

O estado atual usa `InputLocked` de forma ampla demais.

Separar conceitualmente:

- `MovementLock`
- `SkillLock`
- `BasicAttackLock`
- `Action/Recovery state`

Objetivo:

- um micro-impacto não deve necessariamente impedir movimento;
- recovery de ataque não deve automaticamente significar stun;
- manter a filosofia de **sem hard CC**.

---

## 4. ATAQUE BÁSICO X SKILLS

Ataque básico e skill nunca podem produzir efeitos simultaneamente.

### Se estiver executando ataque básico e o jogador apertar:
- Defesa;
- Movimento;
- Ultimate;

então:

1. cancelar imediatamente o ataque básico;
2. cancelar startup/hitbox pendente ou hitbox ativa quando aplicável;
3. iniciar a skill no mesmo input;
4. não exigir segundo aperto.

### Se uma skill estiver em execução:
- negar início de ataque básico enquanto o estado da skill não permitir.

**Skills têm prioridade sobre ataque básico.**

O controle deve parecer imediato e responsivo.

---

## 5. MICRO-IMPACTO DO GUERREIRO

Preparar suporte para ataques pesados aplicarem no alvo:

- `SkillLock = 0,10 s`
- `MovementLock = 0 s`

Ou seja:

- o alvo continua podendo andar;
- por 0,10 s não ativa uma nova skill;
- não usar o `ApplyStagger()` atual para esse comportamento se ele bloquear movimento.

Deixar esse valor data-driven.

---

## 6. PARRY E CLASH

Revisar o stagger atual.

Hoje Parry/Clash podem retirar controle demais.

Resultado esperado:

- preservar impacto visual;
- preservar hit-stop/VFX hooks;
- preservar recovery/interrupt de ataque quando necessário;
- **não criar hard stun**;
- movimento não deve ser bloqueado desnecessariamente.

Não aumentar duração de lock.

---

## 7. VELOCIDADE DO ASSASSINO

A intenção de design é:

> Se Guerreiro e Assassino simplesmente correrem na mesma direção sem skills, o Assassino consegue abrir distância.

Primeiro verificar se a velocidade configurada realmente está sendo aplicada pelo runtime.

Criar/realizar teste:

1. Guerreiro e Assassino lado a lado;
2. ambos correm na mesma direção;
3. duração: 5 s;
4. sem habilidades.

O Assassino deve terminar claramente à frente.

Referência inicial após validar implementação:

- Guerreiro: `4.9`
- Assassino: `6.6`

Não compensar isso com tracking automático do Guerreiro.

---

## 8. POÇÃO DE CURA CONTÍNUA

Substituir o comportamento atual de cura ao final da canalização.

Novo comportamento inicial:

- cura total: `30 HP`;
- duração: `5 s`;
- regeneração contínua ao longo desses 5 s;
- valores data-driven.

Durante a regeneração o jogador:

- continua andando;
- continua atacando;
- continua usando skills;
- pode receber dano;
- receber dano **não cancela** a cura.

Não permitir stacking infinito.

Uma nova poção de cura deve renovar/substituir a regeneração ativa, em vez de acumular múltiplos HoTs independentes.

---

## 9. BARRAS DE VIDA

Adicionar barra de vida provisória para Guerreiro e Assassino.

Requisitos:

- usar `HealthComponent.Changed`;
- refletir `CurrentHealth / MaxHealth`;
- diminuir conforme dano;
- aumentar progressivamente durante cura contínua;
- UI não contém lógica de gameplay;
- pode ser visual simples de laboratório.

Preferência:
- barra visível associada ao personagem;
- legível com a câmera atual.

---

## 10. NÃO IMPLEMENTAR NESTA TAREFA

Não fazer ainda:

- Mago;
- Arqueiro;
- novos kits completos do Guerreiro;
- novos kits completos do Assassino;
- multiplayer;
- matchmaking;
- backend;
- ranking;
- progressão;
- equipamentos;
- VFX finais;
- sangue final;
- animações finais;
- grande refatoração fora do necessário.

---

## 11. TESTES OBRIGATÓRIOS

Atualizar `Docs/TEST_MATRIX.md` e validar:

- [ ] troca livre Base/A/B funciona no laboratório;
- [ ] bot permanece Guerreiro;
- [ ] somente Guerreiro e Assassino estão jogáveis;
- [ ] skill cancela ataque básico no mesmo input;
- [ ] ataque básico não causa efeito simultaneamente com skill;
- [ ] SkillLock de 0,10 s impede skill e não impede movimento;
- [ ] Assassino percorre mais distância que Guerreiro em 5 s;
- [ ] cura ocorre continuamente durante 5 s;
- [ ] dano recebido não cancela cura;
- [ ] múltiplas curas não empilham indevidamente;
- [ ] barra de vida responde a dano;
- [ ] barra de vida responde gradualmente à cura;
- [ ] projeto continua compilando sem erro C#.

---

## 12. CRITÉRIO DE CONCLUSÃO

A tarefa só está concluída quando:

1. código compila;
2. laboratório permite trocar variações;
3. bot continua Guerreiro;
4. Guerreiro e Assassino podem ser usados nos testes;
5. velocidade relativa foi comprovada em runtime;
6. cura contínua funciona em combate;
7. life bars funcionam;
8. locks foram separados o suficiente para preservar movimento;
9. matriz de testes foi atualizada;
10. relatório final foi registrado abaixo.

---

# RELATÓRIO DO CODEX

> Não preencher antes de executar a tarefa.

Ao concluir, substitua esta seção por:

- Estado: DONE / PARTIAL / BLOCKED
- Commit final:
- Arquivos alterados:
- Testes executados:
- Testes aprovados:
- Testes falhos:
- Problemas encontrados:
- Decisões técnicas tomadas:
- Próxima recomendação objetiva:

Não remova a seção `TAREFA ATUAL`.
