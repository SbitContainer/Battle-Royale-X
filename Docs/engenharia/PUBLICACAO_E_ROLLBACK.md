# Entrega e recuperação do Battle Royale X

Adaptado de PUBLICACAO_E_ROLLBACK. Fonte, APK, publicação, instalação e aceite no aparelho são evidências distintas.

## Preparação e build quando autorizados

1. Conferir escopo, fonte aprovada, alterações locais, engine/pacotes e assinatura; capturar snapshot/lista de hashes da fonte real. HEAD sozinho não identifica árvore suja.
2. Fechar ou usar conscientemente a sessão Unity existente. O projeto é a raiz, não Assets. Não executar dois editores/builds na mesma pasta.
3. Para matriz já preparada: Unity `-batchmode -projectPath <raiz> -executeMethod BattleRoyaleX.EditorTools.PrototypeLiveTestLauncher.RunBatch -logFile <log>`. **Sem `-quit`**: a coroutine encerra ao final. Exigir `[BRX LIVE FINAL]`, falhas zero e término; retorno inicial do processo não prova conclusão.
4. `PrototypePolishTools.Prepare` recria dados/cena/apresentação. `BuildMageAndroid` prepara e compila; `BuildArcherAndroid` compila a cena existente e depois testa. Nomes são legados, não filtro de classes. `PrototypePresentationReview.RunRelease` também reconstrói, captura, compila e testa. Executar somente o fluxo que a tarefa requer.
5. Inspecionar log novo, timestamp e arquivo; `Logs/polish-build.txt` pode ser antigo. Arquivar manifesto com bytes/hash, packageId, versionCode/name, fonte/snapshot e configuração. Build uma vez; recompilar invalida aprovação do artefato anterior.
6. Verificar APK com aapt/apksigner apropriados. O helper local compara bytes, **não** assinatura, autoria, packageId interno ou integridade da fonte.
7. Antes de instalar, listar ADB e confirmar dispositivo exato. Usar `install -r`, conferir Success e `dumpsys package`; não fixar IP antigo nem limpar dados para contornar assinatura incompatível.
8. Registrar teste real de toque, pausa/retomada, layout, skills e gráficos. Uma activity aberta não comprova esses critérios.

## Ferramenta de integridade

`node Tools/Engineering/verify-artifact.cjs Docs/engenharia/releases/effects-lab-092.json`

O manifesto histórico foi calculado em 03/10 sobre o APK local existente. Não é reconstrução de qual árvore suja gerou o APK. `sourceCommit` é null deliberadamente. Não reutilizar esse manifesto para outro build; salvar outro registro sem apagar o anterior.

## Publicação e recuperação

Destino/canal/credenciais exigem decisão própria. Nenhum upload/OTA é habilitado aqui. Se houver distribuição pública, reconsultar URL final, baixar os bytes e comparar hash; catálogo assinado e sequência crescente são contratos futuros, não implementação existente. Hash não autentica publicador.

Exceção autorizada em BRX-RELEASE-003: GitHub público `SbitContainer/Battle-Royale-X` foi escolhido pelo usuário. Para novas atualizações, preferir `PrototypeApkReleaseBuild.BuildVersionedApk` com log aprovado e cena existente; o builder legado fixa código 11 e não deve ser usado para substituir versões posteriores. `Tools/Release/Publish-GitHubApk.ps1 -Publish` valida pacote/código/certificado/hash e publica novo draft/asset somente após aprovação. Detalhes em [../ANDROID_GITHUB_UPDATES.md](../ANDROID_GITHUB_UPDATES.md). A assinatura de APK existente é verificada; catálogo separado assinado continua futuro. Primeira instalação pelo navegador e atualizações pelo instalador do jogo dispensam ADB, mas pedem consentimento do Android. Não confundir publicar com instalar ou validar toque.

Guardar APK anterior, assinatura e backup separado dos dados/preferências relevantes. Não presumir que downgrade é permitido nem desinstalar para forçá-lo: pode apagar layout/dados. Preferir versão corretiva com versionCode maior quando retorno for inseguro. Ensaiar recuperação em laboratório antes de prometer rollback. Não restaurar estado antigo sobre gravações novas sem reconciliação.

C#/plugins/configuração nativa exigem APK novo neste fluxo. `OTA_CONTENT_PLAN.md` propõe Addressables para conteúdo compatível; não copiar Expo/EAS nem executar código remoto. Rollback de catálogo anti-replay exigiria nova sequência, não reapresentar catálogo antigo.
