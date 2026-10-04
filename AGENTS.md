# Trabalho no Battle Royale X

## Entrada obrigatória

Confirme raiz, branch, remoto e alterações locais antes de editar. Abra [Docs/HANDOFF.md](Docs/HANDOFF.md), [Docs/PROJETO.md](Docs/PROJETO.md), [Docs/engenharia/FLUXO.md](Docs/engenharia/FLUXO.md) e [Docs/engenharia/AGENTES.md](Docs/engenharia/AGENTES.md). Escolha o setor em [Docs/MAPA.md](Docs/MAPA.md) e confira chamadores com `rg`. Na primeira sessão, leia também README, CODEX_START_HERE, COMBAT_PRINCIPLES, ASTRA_HANDOFF e TEST_MATRIX em Docs, conforme a orientação existente. Leia a política específica quando a mudança envolver bugs, dados, assets ou release.

## Limites do projeto

- **Permissão prévia para edições fora do pedido:** editar somente o que estiver abrangido pela solicitação do usuário. Se for necessário alterar algo que não foi pedido, explicar a alteração proposta, os arquivos ou sistemas afetados, o motivo e o impacto, pedir permissão explícita e aguardar a resposta antes de editar essa parte. Isso também vale para bugs incidentais, refatorações, dependências, configurações, assets e documentação fora do escopo. Necessidade técnica, conveniência, recomendação do kit, silêncio ou autorização de uma tarefa diferente não permitem ampliar o pedido. Enquanto aguarda, pode continuar leituras e trabalho já autorizado que não dependa dessa alteração. Uma permissão já concedida para a mesma alteração não precisa ser pedida novamente.
- Preserve mudanças locais, GUIDs e arquivos `.meta`. Não recrie a arquitetura, não mova pastas nem atualize dependências como efeito colateral.
- Guerreiro e Assassino foram aprovados pelo usuário: não rebalancear ou redesenhar sem solicitação específica.
- Mago e Arqueiro existem. Instruções antigas de duas classes/cápsulas descrevem a primeira integração, não uma ordem de remover o trabalho posterior.
- A lógica resolve dano, defesa e timing; animação, UI e partículas apresentam o resultado. Não tornar Animation Events a autoridade de dano.
- Sem hard CC prolongado, progressão vertical, backend, contas, multiplayer ou OTA novo apenas por estar previsto em um roteiro.
- A solicitação atual define a tarefa. CODEX_TASK.md e ASTRA_TASK.md não são acionados automaticamente; não fazer pull por força em árvore suja.

## Execução e evidências

Defina esperado, proibido, setor A e consumidores B/C antes do reparo. Um escritor por arquivo. Não iniciar subagentes sem autorização do usuário ou instrução aplicável explícita; papéis documentados não ativam agentes. Não trocar modelos ou gravar memória global automaticamente.

Use [Docs/engenharia/BUGS_E_TESTES.md](Docs/engenharia/BUGS_E_TESTES.md). Teste verde no Editor não prova toque ou desempenho Android. Bug recorrente exige reprodução representativa, não mais dois reparos por hipótese. Relate NÃO EXECUTADO e AGUARDANDO_VALIDACAO quando faltar evidência.

Build, publicação e instalação são ações distintas, dependentes do pedido vigente. Não herdar permissões de outro projeto ou de uma entrega antiga. Atualize HANDOFF, mapa quando necessário e registro da tarefa. Não alegar push, modelo observado, economia de tokens ou validação física sem prova.

## Registro no Git — pedido do usuário em 03/10/2026

Todo trabalho autorizado neste projeto deve ser registrado em commits e enviado ao remoto `origin`, preservando o histórico e verificando o resultado do push. Incluir código, assets usados com seus `.meta`, testes e documentação/evidências sanitizadas. Não versionar segredos, credenciais, caches Unity, temporários ou anexos do chat. APKs são artefatos de GitHub Releases, não arquivos do código-fonte. Não usar force push ou substituir mudanças remotas; se houver divergência, informar antes de reconciliar. Esta autorização de registro não amplia o escopo funcional nem autoriza publicação/instalação de APK sem pedido vigente.
