# Estado e integridade no Battle Royale X

Adaptado de DADOS_E_COMUNICACAO e contratos dos componentes v1.1. Não adiciona armazenamento ou rede.

## Aplicável agora

- Identificar habilidade por abilityId/slot/classe e ator por sua identidade runtime, não texto de botão ou posição na lista. GUID `.meta` é identidade de asset, não identidade de ativação.
- Cada golpe/ativação tem seu ciclo; colisões repetidas do mesmo contato não multiplicam dano. Nova ativação legítima não deve ser descartada porque tem o mesmo nome.
- Configuração compartilhada permanece em ScriptableObjects; energia, cooldown, cargas, alvo e efeito ativo pertencem ao personagem/ativação.
- Troca de classe/variante e reset precisam cancelar efeitos/coroutines antigos antes de reutilizar o ator. A→B→A é caso explícito de regressão.
- `BRX.MobileLayout.v4` contém preferências locais. Validar JSON e geometria, preservar dados anteriores e não usar DeleteAll como solução. Ausência de medição não é FPS zero nem sucesso.
- Logs devem incluir versão, ator/classe, abilityId, fase/ação e timestamp relevantes sem registrar cada toque ou dados pessoais continuamente.

## Somente quando houver persistência ou sincronização aprovada

Definir sessão/conta/entidade/operação/tentativa separadamente. IP é endereço transitório, não identidade. Troca de conta deve invalidar resultados em voo antes e depois de await. Não reutilizar endpoints/credenciais da origem.

Mudança de negócio e evento durável exigem transação/journal no produto. Retry preserva identidade e bytes do evento tentado; edição nova usa nova operação. ACK deve identificar operação, escopo e hash após commit idempotente no receptor; HTTP 200 genérico não basta. Conflito de uma entidade não pode bloquear todas as outras. Consulta/export pode precisar esperar o conjunto de alterações capturado, não edições futuras ilimitadas.

Arquivos, índice e vínculos precisam sobreviver a reinício. Mídia lenta não deve bloquear o núcleo sem decisão, mas não pode ser marcada enviada antes da confirmação. Testar duplicado, resposta perdida, offline/reconexão, lease expirado, troca de identidade e replay. Timeout, tamanho de lote e retenção devem ser medidos para o novo sistema, não copiados do CNC.

Fila durável e barreira Node/Python do kit não são netcode: não usá-las para sincronizar combate frame a frame. Requerem transporte, autenticação, receptor e contratos próprios; não foram instaladas neste jogo.
