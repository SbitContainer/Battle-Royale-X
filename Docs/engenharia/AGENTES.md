# Responsabilidades de engenharia

Adaptado de AGENTES e dos sete contratos de papéis do kit. São responsabilidades disponíveis, não agentes ativados nem obrigação de executar sete processos.

## Autorização vigente — 03/10/2026

O usuário autorizou ativar todos os papéis para o Battle Royale X. Os sete papéis foram executados na tarefa BRX-RELEASE-003, em rodadas, respeitando quatro agentes concorrentes incluindo o principal. Essa autorização permite delegações delimitadas dentro dos pedidos do projeto; não permite ampliar escopo, agentes recursivos, processos permanentes nem alterar modelos por conta própria. Cada sessão deve verificar agentes realmente ativos; um papel concluído não continua trabalhando em segundo plano.

Na atualização Android, architect_impact recebeu excepcionalmente escrita isolada da ponte nativa, implementer do runtime e security_release do publisher. Um escritor por arquivo; revisão independente após congelar candidato. As atribuições específicas prevalecem sobre o limite padrão de leitura quando explicitamente delimitadas pela coordenação dentro do pedido autorizado.

| Papel | Uso no Battle Royale X | Limite |
|---|---|---|
| Principal | Coordenar, implementar tarefas locais e consolidar evidências | Um escritor por arquivo |
| architect_impact | Contratos entre input, combate, dados e apresentação | Leitura e proposta |
| implementer | Alteração delimitada de scripts/assets/tooling | Apenas arquivos da tarefa |
| qa_reviewer | Preservação dos consumidores e critério do usuário | Não corrigir em paralelo |
| security_release | APK exato, assinatura, origem de assets e recuperação | Não publicar por conta própria |
| test_executor | Comandos conhecidos, cenas e logs | Não alterar oráculo para passar |
| domain_specialist | Controle contínuo, defesa, matchup e regras aprovadas | Não importar regras CNC ou inventar balanceamento |
| memory_recorder | Propor registros sanitizados no projeto | Não editar memória global automaticamente |

Delegação exige autorização aplicável e ganho concreto. Sem subagentes recursivos; tarefa pequena fica com principal. Cada delegação informa setor, arquivos, esperado/proibido, duração-limite e entrega. Revisores recebem candidato exato, não “último código” mutável. Pareceres: APROVADO, AJUSTAR, BLOQUEADO ou AGUARDANDO_VALIDACAO.

Não foram copiados `.codex/config.toml` ou permissões do outro projeto. Não fixar modelo ou prometer que uma configuração foi carregada sem execução comprovada. Se o usuário pedir automação de papéis, validar compatibilidade e fazer piloto próprio. Escolha de capacidade deve ser proporcional, sem troca silenciosa.

Registrar papel, contribuição, início/fim quando medidos, modelo declarado/observado somente se disponível, candidato, decisão, evidência, falhas e retrabalho. Não estimar tokens ou economia. Falha encontrada pelo revisor é resultado útil da revisão. Correções de registros devem referenciar o anterior, não apagar a história.
