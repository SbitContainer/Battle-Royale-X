# BRX-RELEASE-003 — Agentes e atualização Android sem ADB

Data: 03/10/2026. Estado: PUBLICADO_VERIFICADO / AGUARDANDO_VALIDACAO_ANDROID.

Continuação de instalação no mesmo dia: APK instalado e aberto, mas smoke Android encontrou erro de apresentação. Estado atual: INSTALADO_COM_ERRO_DE_INICIALIZACAO / AGUARDANDO_AUTORIZACAO_PARA_CORRECAO. Evidência detalhada ao final; resultados anteriores de publicação permanecem históricos e válidos para bytes, não aceite físico.

## Pedido e autorização humana

O usuário autorizou ativar todos os papéis e pediu um modo de gerar APK e atualizar o celular sem abrir a depuração. A autorização abrange investigar a distribuição e preparar o fluxo local pertinente; não define canal público, credenciais ou instalação silenciosa. Alterações fora desse pedido continuam exigindo permissão explícita, conforme AGENTS.md.

Raiz confirmada: `C:/Users/netor/OneDrive/Desktop/Battle Royale X`; branch `main`; remoto `https://github.com/SbitContainer/Battle-Royale-X.git`. A árvore já contém mudanças de gameplay, assets e documentos. Não limpar, sincronizar por força nem regenerar dados/cena como efeito colateral.

## Esperado, proibido e consumidores

- Esperado: esclarecer o caminho de atualização sem ADB, separar geração/publicação/instalação e preparar builder local isolado com identificação correta do candidato.
- Proibido: prometer instalação silenciosa universal, reutilizar código de versão 11 para uma atualização nova, expor chaves ou tokens, publicar em canal não escolhido, alterar gameplay ou instalar APK antigo como se fosse novo.
- Setor A: cena/build e release. Consumidor B: artefato Android, pacote, versão e assinatura. Consumidor C: aparelho/instalador Android e futuro canal de distribuição.
- Um escritor por arquivo. Sem agentes recursivos, serviços permanentes ou memória global.

## Papéis autorizados e execução em rodadas

O limite disponível é quatro agentes concorrentes, incluindo o principal. Todos os papéis podem contribuir em rodadas; ativação não significa sete processos simultâneos nem trabalhadores permanentes.

| Papel | Contribuição atribuída | Limite |
|---|---|---|
| Principal | Coordenação, integração e relatório ao usuário | Consolidar evidência atual; preservar mudanças locais |
| architect_impact | Contratos de atualização e impactos no projeto | Leitura e proposta, sem publicação |
| implementer | Novo builder local isolado | Apenas arquivos acordados; não gerar/publicar/instalar nesta rodada |
| qa_reviewer | Revisão independente do candidato exato | Sem corrigir arquivos em paralelo; parecer ainda pendente |
| security_release | APK existente, assinatura e restrições Android | Não distribuir credenciais, não prometer instalação silenciosa |
| test_executor | Verificação local de ferramentas e compilação quando coordenada | Não mudar o oráculo nem confundir teste histórico com atual |
| domain_specialist | Preservação do combate e fronteiras de gameplay | Sem rebalanceamento ou redesign |
| memory_recorder | Este registro sanitizado no projeto | Somente este arquivo; nenhuma escrita na memória global |

Não há evidência registrada de modelo observado, custo ou economia de tokens; não inferir esses valores. A duração máxima atribuída a este registro foi três minutos; não equivale a medição de duração total dos demais papéis.

## Inspeção inicial e decisões pendentes

O builder legado `PrototypePolishTools` fixa `bundleVersion` em `0.9.2-effects-lab` e `versionCode` em 11. A tarefa prepara um caminho separado, evitando regeneração de dados/cena e sem modificar o legado por conveniência.

A inspeção de release comunicada pela coordenação encontrou APK de depuração, pacote `com.sbitcontainer.battleroyalex.prototype`, código 11. SHA-256 do certificado observado: `9cb3ed07529afefbd8f304a78fe349484ccf445357d7f777ca530a493598dc17`. Esse identificador é do certificado público, não uma chave privada nem o hash dos bytes do APK. Não prova vínculo entre a árvore atual e o artefato histórico.

Não foi identificado atualizador implementado; Addressables está ausente do manifest de pacotes atual. O plano OTA existente não demonstra atualização funcional. Código C#/plugins/configuração nativa exigem APK novo no fluxo atual.

Pendente decisão humana: distribuição pública via GitHub ou canal privado. Não escolher por conta própria nem publicar antes dessa decisão e das verificações correspondentes. Nenhum backend, conta ou transferência de credenciais de outro projeto está autorizado implicitamente.

No Android, atualização pelo próprio aplicativo depende de versão do sistema, instalador, permissões, assinatura e demais requisitos da plataforma. O caminho seguro prevê consentimento do usuário para instalar quando exigido; não prometer atualização totalmente silenciosa em qualquer aparelho. Ainda falta definir e validar o fluxo representativo no Moto G54.

## Evidências e limites desta tarefa

| Camada | Estado parcial | Limite |
|---|---|---|
| Ferramentas Node | 16 testes PASS, informados pela coordenação | Não prova gameplay ou Android |
| Governança | Checagem PASS, informada pela coordenação | Não prova compilação do novo builder |
| Revisão QA do novo builder | PENDENTE | Candidato ainda precisa ser congelado |
| Compilação Unity do novo builder | PENDENTE | Não herdar aceite do código anterior |
| Combate/UI: 237 casos; visual estrutural: 48 verificações | Histórico da tarefa anterior neste dia | Não reexecutados nesta tarefa de release |
| Novo APK | NÃO GERADO nesta rodada | Identificação do APK antigo não prova nova entrega |
| Publicação | NÃO EXECUTADA | Canal ainda não escolhido |
| Instalação e toque no celular | NÃO EXECUTADOS | AGUARDANDO_VALIDACAO_ANDROID |

Este registro reúne achados sanitizados da equipe e leituras locais de governança; não é parecer final de segurança nem validação física. A coordenação atualizará os resultados finais com candidato, comandos, falhas e evidências verificadas.

## Próximo passo e recuperação

Concluir/revisar o builder isolado, executar a compilação coordenada, obter a decisão de canal e então definir o mecanismo de distribuição/instalação. A decisão pendente bloqueia publicação, não a investigação ou preparação local independente autorizada.

Preservar o APK anterior e a assinatura. Não desinstalar para contornar incompatibilidade e não prometer downgrade. Eventual recuperação deve retirar apenas o diff desta tarefa ou produzir versão corretiva com código maior, conforme a situação e autorização.

## Continuação após escolha humana do canal

O usuário escolheu "pode fazer com link no git". GitHub connector e API anônima confirmaram `SbitContainer/Battle-Royale-X` público; não houve alteração de privacidade. Credential Manager lista a conta SbitContainer; credenciais não foram exibidas nem embutidas no aplicativo.

Implementação nova e isolada: `PrototypeApkReleaseBuild.cs`, `PrototypeApkUpdateValidation.cs`, `Runtime/Release/PrototypeGithubApkUpdates.cs`, biblioteca `Assets/Plugins/Android/BRXUpdates.androidlib` (manifest, Gradle, properties, ApkUpdates.java, ApkProvider.java) e `Tools/Release` (publisher, testes, README). Sem alterações de gameplay/cena nesta integração. Importação Unity é responsável pelos `.meta` novos.

O principal acrescentou ao builder inventário por SHA-256 dos inputs Assets/Packages/ProjectSettings; inclui árvore suja e lockfile local. Somente hashes/caminhos, sem contents/chaves. Antes de flags temporárias de versão; não é archive recuperável nem prova reprodutibilidade do toolchain.

QA independente aprovou revisão estática do protótipo; ressalvou assinatura debug, catálogo não assinado e falta de validação física. Publisher revisado SHA-256 `71d36bbdfa1acabf9b0328263c5a941d7bc1011d0c1d58eab00245a2833bca02`. Java compilou com SDK36/Java8 (avisos de API legada/bootstrap, nenhum erro); 10 probes offline do publisher passaram; 16 testes Node passaram; governança atual 13 setores/53 referências. Unity executou 35 testes puros do contrato: pass=35 fail=0 em `Logs/GitHub-Updates-Contract-20261003.log`. Rede/update/instalação não são cobertos por esses testes.

Matriz atual, build, inspeção do APK, publicação e instalação serão registrados separadamente abaixo. A decisão de canal está resolvida; as pendências registradas na inspeção inicial acima são históricas, não o estado mais recente.

## Resultado final da entrega local/publicação

- Matriz reexecutada: 237 combate/UI PASS, 0 FAIL; 48 visual estrutural PASS, 0 FAIL. Log `Logs/GitHub-Updates-Matrix-20261003.log`; relatório `Docs/TEST_RESULTS_LAB_001.md` atualizado pelo runner. Nenhum teste de toque/FPS implícito.
- Builder ajustado por incompatibilidade real com a pasta OneDrive: CLOUD_A `0x9000a01a` permitido; junction/symlink/tags desconhecidas rejeitados. Revisão incremental independente APROVADA. P/Invoke compilou/executou no build e inventário da fonte foi produzido.
- Build Unity Android terminou com zero erros em aproximadamente 212 s (tempo do BuildReport, não da tarefa). Log `Logs/GitHub-Updates-APK-20261003.log`. APK `Builds/Releases/12/BattleRoyaleX-12.apk`, 57.144.203 bytes, versão `0.9.12-lab`, código 12. SHA-256 `bb594fe0aecaec1d053823faaa11821fb077a91ed4c91ac86b0c8a707b689b1e`.
- Publisher sem switch passou gates locais de pacote/código/hash/log/assinatura. Aapt/apksigner conferiram certificado original. Inspeção adicional confirmou INTERNET, REQUEST_INSTALL_PACKAGES, provider privado/authority correta/grant-read e ambas as classes Java no DEX.
- Publicação executada com `-Publish` usando autenticação existente no computador, sem solicitar login, exibir/gravar tokens ou incluí-los no APK. Novo draft `android-lab-12`, uploads conferidos antes de publicar e downloads públicos de APK/catálogo/inventário conferidos depois. Recibo `PUBLISHED_VERIFIED` às 21:54:44 UTC. [Release](https://github.com/SbitContainer/Battle-Royale-X/releases/tag/android-lab-12), [manifesto preservável](../releases/android-lab-12.json).
- Fonte não commitada/pushada. Tag GitHub aponta para branch remota como contexto; não recupera a árvore suja local. Inventário de hashes atribui inputs, não backup. APK anterior mantido.
- Instalação no Moto G54, permissão Android, toque/pause/resume e ciclo A→B pelo aplicativo: NÃO EXECUTADOS / AGUARDANDO_VALIDACAO. Usuário recebe link para instalação inicial pelo navegador sem depuração.

Arquivos desta tarefa: novos `Editor/PrototypeApkReleaseBuild.cs`, `Editor/PrototypeApkUpdateValidation.cs`, `Runtime/Release/PrototypeGithubApkUpdates.cs` (e `.meta`/pasta Release.meta), `Assets/Plugins.meta`, `Assets/Plugins/Android.meta`, `Assets/Plugins/Android/BRXUpdates.androidlib.meta` e os cinco arquivos da biblioteca (AndroidManifest.xml, build.gradle, project.properties, ApkUpdates.java, ApkProvider.java). Novos `Tools/Release/Publish-GitHubApk.ps1`, `Publish-GitHubApk.tests.ps1`, `README.md`, `Docs/ANDROID_GITHUB_UPDATES.md`, este registro e `Docs/engenharia/releases/android-lab-12.json`. Atualizados `Docs/HANDOFF.md`, `Docs/MAPA.md`, `Docs/engenharia/mapa.json`, `Docs/engenharia/AGENTES.md`, `Docs/engenharia/PUBLICACAO_E_ROLLBACK.md`, `README.md` e relatório gerado `Docs/TEST_RESULTS_LAB_001.md`. PlayerSettings de versão/flags foram alterados temporariamente para o build e restaurados; não houve edição manual de QualitySettings, gameplay, dados ou cena. Mudanças preexistentes preservadas.

## Instalação ADB autorizada e smoke Android — 03/10/2026

Usuário habilitou depuração e pediu sincronizar/atualizar primeiro por ADB. Descoberta atual por `adb devices -l` e `adb mdns services` identificou Moto G54 5G autorizado; não reutilizado IP/porta histórico. SHA do APK local conferido contra publicação. `adb -s <serial atual> install -r Builds/Releases/12/BattleRoyaleX-12.apk` retornou `Performing Streamed Install / Success`.

`dumpsys package` confirmou antes código 11/0.9.2-effects-lab, depois código 12/0.9.12-lab, minSdk26/targetSdk36. lastUpdateTime=2026-10-03 19:03:42; firstInstallTime=2026-09-18 21:15:52 preservado. Sem uninstall, clear ou limpeza de PlayerPrefs. Isso não prova visualmente que todos os valores/layout salvos permanecem corretos.

Activity UnityPlayerActivity aberta; PID presente e topResumedActivity pertence ao jogo. Logcat restrito ao processo encontrou:

- `ArgumentNullException: Parameter name: shader` em `MobileAbilityAimPreview.Awake`, chamado durante `PrototypeMobileTouchControls.Build/Start`. Leitura da fonte confirma `new Material(Shader.Find("Universal Render Pipeline/Unlit"))` sem guarda; o shader não foi encontrado no player observado. É falha Android real apesar do Editor verde; retenção/stripping precisa reprodução dirigida antes do reparo.
- `ClassNotFoundException` para `com.google.android.play.core.assetpacks.AssetPackManager`. Ainda não determinado se aviso de integração opcional ou falha funcional; não adicionar biblioteca por hipótese.

Não houve crash fatal observado nessa janela: processo/activity ativos. Isso não prova input/visual íntegros. Nenhum código, asset ou configuração foi corrigido nessa continuação; apenas HANDOFF e este registro atualizados. Solicitar autorização antes de reparar apresentação/dependências, conforme regra explícita do usuário. Fluxo de atualização pelo próprio jogo, consentimento do instalador, toque, FPS, pausa/retomada e aceite humano continuam AGUARDANDO_VALIDACAO.
