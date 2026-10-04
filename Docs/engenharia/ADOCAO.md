# Arquitetura reutilizável aplicada ao Battle Royale X

Em 03/10/2026, o kit v1.1 da pasta Arquitetura-Reutilizavel-Projetos foi adaptado ao laboratório Unity existente. O resultado é uma estrutura de engenharia para orientar mudanças, verificar dependências e distinguir teste, build e instalação. **Não houve reescrita do combate, mudança de balanceamento ou adição de serviços ao jogo.**

## Como ficou

Entrada única em AGENTS.md e HANDOFF, contrato atualizado para quatro classes, mapa de dez setores com arquivos/símbolos reais e consumidores, seis políticas locais, modelos de tarefa/incidente/decisão/testes/release e ferramentas offline em Tools/Engineering. README e CODEX_START_HERE agora distinguem a fase inicial de duas classes do estado atual.

O fluxo é: pedido e esperado → setor afetado → consumidores → reparo delimitado → testes da camada correta → revisão do candidato → entrega autorizada e identificada. Os papéis são responsabilidades opcionais; nenhuma configuração de agentes, modelo ou permissão do outro projeto foi ativada.

## Destino de cada parte do kit

| Parte de origem | Aplicação no jogo |
|---|---|
| LEIA_PRIMEIRO, MODELO_DE_ADOCAO e adaptador BATTLE_ROYALE_X | Entrada local, adoção incremental e preservação do runtime/Assets |
| AUDITORIA, VALIDACAO, MANIFESTO e PROVENIENCIA | Limites de evidência, origem e hashes; não importar a aprovação de outro produto |
| FLUXO | Um escritor, impacto A/B/C, menor reparo causal e handoff curto |
| AGENTES e sete contratos TOML | Responsabilidades de revisão adaptadas ao domínio de combate; configurações não ativadas |
| BUGS_E_TESTES e APRENDIZADOS | Diagnóstico do gesto até o dano/feedback, regressões e protocolo para falha recorrente |
| DADOS_E_COMUNICACAO | Identidade de ator/ativação, estado por owner, A→B→A e preferências; contratos de sync somente se futuramente aprovados |
| PUBLICACAO_E_ROLLBACK | Fonte distinta de APK/publicação/instalação, manifesto por bytes e recuperação sem apagar dados |
| QUALIDADE | GUID/meta, shaders, origem/licença de assets, medição física e ambiente |
| Templates PROJETO/HANDOFF/MAPA/modelos | Preenchidos com caminhos reais; históricos preservados em Docs |
| verifyFile de primitives.cjs | Reutilizado como verificador streaming de APK, com adaptador e testes locais |
| createSingleFlight | Não integrado: UI de combate já possui actionBusy/cooldown e depende de timing por ação; wrapper genérico compartilharia argumentos incorretamente |
| compareVersions/inRange | Não integrado: formato numérico estrito rejeita sufixos de versões Unity como effects-lab; Android usa versionCode |
| writeJsonAtomic | Contrato registrado para futuros arquivos de configuração; não substituir PlayerPrefs ou criar save sem necessidade |
| confirmation-barrier.cjs | Referência futura para consultas após ACK; sem serviço/ACK no jogo atual, nada a integrar |
| durable_outbox.py e testes | Referência futura para eventos duráveis de backend aprovado, nunca netcode frame a frame; sem SQLite/servidor novo |
| validadores e testes do kit | Inspiram validação local do mapa/links e casos negativos de integridade; nenhuma alegação de que seu PASS valida Unity |
| adaptador APPS_E_SISTEMAS | Separação de runtimes mantida: Node em tooling, C# no jogo; sem Expo/EAS ou regras CNC |

## Aprendizados transformados em regras locais

| Conhecimento do kit | Regra verificável no Battle Royale X |
|---|---|
| Suite verde contradiz uso real | Testar toque e gesto; chamada direta a TryUse não valida o botão |
| Identidade trocada reaproveita estado | Classe/variante A→B→A com cleanup de efeitos, coroutines, alvo e cargas |
| Apresentação se mistura à fonte de cálculo | VFX/Animator/HUD não resolvem dano; visualizar sem mudar resultado |
| Operação repetida ou resposta perdida | Colisão repetida não duplica dano; futura rede exige operação/ACK próprios |
| Conflito/lote bloqueia independentes | Estado de um ator não deve contaminar outro; fila futura precisa isolamento comprovado |
| Mídia lenta trava núcleo | VFX/asset pesado exige medição; apresentação ausente não pode quebrar combate |
| Consulta antes da confirmação | UI deve refletir a ação aceita/estado real; ACK remoto é futuro, não existe aqui |
| Associação por nome/último registro | Habilidade/owner/GUID explícitos, nunca “modelo encontrado primeiro” inativo |
| Recorte de tempo incorreto | Registrar startup/ativo/recovery/cooldown e pausa; não importar cálculos de período CNC |
| Árvore suja produz pacote incerto | Snapshot real de fonte por release; HEAD sozinho não basta |
| URL muda após publicação | Download/hash da URL final somente quando houver publicação autorizada |
| Ambiente causa falso erro de produto | Distinguir SDK/licença/encoding de defeito de habilidade |
| Agente/modelo indisponível | Não alegar revisão executada nem trocar silenciosamente; registrar limite |

## Ferramentas e piloto

`node Tools/Engineering/validate.cjs` confere dez setores, referências, nomes de símbolos e links locais dos documentos de governança. Não é análise semântica completa de C#.

`node --test Tools/Engineering/engineering.test.cjs` exercita casos válidos e falhas: bytes alterados/truncados/excessivos, hash inválido, descritor de pacote errado, caminho fora da raiz, arquivo/teste ausente, símbolo removido, consumidor inexistente e setor duplicado.

`node Tools/Engineering/verify-artifact.cjs Docs/engenharia/releases/effects-lab-092.json` compara o APK existente com tamanho e SHA256 registrados. Não abre o APK nem valida assinatura; o manifesto explicitamente não atribui o build antigo ao HEAD atual.

O piloto é a verificação local de engenharia/artefato, sem reconstrução Unity ou modificação do produto. Reversão: retirar apenas os arquivos novos desta adoção e os trechos de entrada adicionados ao README/CODEX_START_HERE/ASTRA_HANDOFF; não reverter o diff anterior de habilidades/assets. O teste de referências roda em fixtures temporárias para falhas deliberadas, sem remover arquivos reais do projeto.

## Validação e pendências

Resultados desta adoção em [tarefas/BRX-ENG-001.md](tarefas/BRX-ENG-001.md): mapa e links conferidos, ferramenta de hash aplicada ao APK existente e 938 arquivos Unity comparados sem mudança. Resultados antigos do jogo permanecem em [../EFFECTS_LAB_092.md](../EFFECTS_LAB_092.md) e [../TEST_RESULTS_LAB_001.md](../TEST_RESULTS_LAB_001.md). Não foram reexecutados nem elevados a aprovação física.

Pendências não resolvidas por documentação: validação humana de skills/efeitos, FPS/memória em Android, lockfile atualmente ignorado e fluxo futuro de snapshot de fonte aprovado. Sem novo APK, push, publicação, login, sincronização ou OTA nesta tarefa. O mapa é ponto de partida mantido por mudança, não auditoria linha a linha de todo o jogo.
