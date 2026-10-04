# Fluxo de mudança no Battle Royale X

Adaptado do núcleo FLUXO do kit v1.1. As regras completas aplicáveis ficam neste repositório; não dependem da pasta externa.

1. Confirmar raiz Unity, Git/status e pedido atual. Não perder mudanças preexistentes.
2. Ler HANDOFF e contrato, escolher setor em MAPA e conferir chamadores reais.
3. Definir resultado funcional esperado e proibido. Exemplo: seletor muda A/B/C com menu fechado; não troca classe nem consome item.
   Se a execução exigir editar algo fora do pedido, descrever a proposta, arquivos/sistemas, motivo e impacto; pedir permissão explícita e aguardar antes dessa edição. Não corrigir bugs incidentais ou ampliar escopo automaticamente, mesmo por necessidade técnica. Continuar somente leituras e partes já autorizadas independentes. Aplicar a regra de permissão prévia de AGENTS.md.
4. Registrar tarefa com arquivos, consumidores, um autor, testes e recuperação. Usar modelos locais.
5. Fazer o menor reparo causal. Não acrescentar infraestrutura nem refatorar tudo para consertar bug local.
6. Validar setor e consumidores; usar regra do usuário como oráculo. Congelar candidato antes da revisão. Mudança posterior invalida o aceite afetado.
7. Avaliar revisão independente proporcional ao risco quando autorizada. Sem revisão disponível, registrar limite; silêncio ou timeout não aprovam.
8. Registrar comando, ferramenta, camada, falhas iniciais, reruns e não executados. Não transformar Editor verde em Android validado.
9. Commit/push/build/publicação somente no escopo autorizado. Verificar resultado, não intenção. Nunca pull/reset destrutivo para limpar árvore suja.
10. Atualizar HANDOFF curto e mapa se contratos mudarem. Histórico detalhado permanece em tarefas/incidentes/releases.

Até dois ciclos de reparo sem resolver a mesma falha: revisar hipótese e jornada real. Rollback da tarefa deve retirar apenas seu diff, preservando alterações anteriores. Regras de recuperação de dados e binários são separadas.
