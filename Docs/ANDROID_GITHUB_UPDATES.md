# Atualizações Android pelo GitHub

Canal autorizado pelo usuário em 03/10/2026: repositório público `SbitContainer/Battle-Royale-X`, GitHub Releases. Não alterar a privacidade nem colocar tokens no APK. Estado de execução e entrega: [BRX-RELEASE-003](engenharia/tarefas/BRX-RELEASE-003.md).

## Fluxo

1. Validar a fonte atual com a matriz Unity e guardar o log completo.
2. Gerar APK da cena existente com `PrototypeApkReleaseBuild.BuildVersionedApk`, Android selecionado e argumento `-brxApprovedTestsLog <log>`. Não regenerar dados/cena.
3. Conferir pacote, código crescente, certificado e SHA-256. O builder reserva uma pasta por código em `Builds/Releases`; preserva o APK anterior. `source-inputs.json` identifica por hashes os arquivos reais, incluindo os não commitados; não é backup recuperável da fonte.
4. Preparar/publicar com `Tools/Release/Publish-GitHubApk.ps1`. Publicação requer autorização, autenticação do publicador e revisão do candidato. Sem `-Publish`, não modificar o GitHub. O manifesto de distribuição é `brx-update.json`.
5. Primeira instalação: abrir o link do APK no navegador do celular e aceitar a instalação. Se solicitado, permitir instalação daquela fonte. Não desinstalar a versão anterior: pacote e certificado devem coincidir para preservar dados/layout.
6. Versões seguintes: o jogo consulta a última release publicada ao abrir. Uma versão maior apresenta aviso; o usuário escolhe baixar/instalar. O APK é validado por bytes/hash, pacote, versão e certificado antes de chamar o instalador Android. Permitir instalação pelo Battle Royale X quando o Android solicitar.

Não é instalação silenciosa garantida: o Android pode exigir confirmação a cada atualização. Não precisa ADB ou depuração. Sem internet ou sem release válida, o combate continua offline. A primeira versão instalada precisa conter o atualizador; o APK antigo não adquire código novo sozinho.

## Contrato de distribuição

`brx-update.json`: `schemaVersion=1`, `packageName`, `versionCode`, `versionName`, `apkUrl`, `apkBytes`, `apkSha256`. URL HTTPS de asset do próprio repositório. Não baixar APK de outro pacote/domínio arbitrário nem aceitar código igual/inferior. APKs/manifestos devem ficar em release por versão, sem sobrescrever assets antigos.

O canal de laboratório preserva a assinatura de depuração existente para compatibilidade. Não equivale a distribuição de produção; trocar chave exige planejamento explícito. Hash protege integridade de bytes, não autentica isoladamente o publicador: a fronteira utiliza HTTPS do canal fixo e certificado Android compatível. Catálogo assinado, Addressables e Play Store não foram adicionados.

## Validação no Moto G54 ainda necessária

Testar instalação inicial pelo navegador, confirmação de fontes desconhecidas, atualização dentro do jogo para código superior, pausa/retomada, ausência de rede, cancelamento, preservação do layout e retorno ao combate. Aprovação Editor ou Java compilado não substitui esse teste físico. Um único APK com o atualizador também não prova o ciclo versão A → B.

Referências: [GitHub Releases API](https://docs.github.com/en/rest/releases/releases), [assets de release](https://docs.github.com/en/rest/releases/assets), [permissão de instalação Android](https://developer.android.com/reference/android/content/pm/PackageManager#canRequestPackageInstalls()).
