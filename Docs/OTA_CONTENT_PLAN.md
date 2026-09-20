# Battle Royale X — plano de atualizacao remota

## Limite importante

No Unity, uma atualizacao remota nao substitui codigo C#, cenas incluidas no player,
configuracoes nativas nem pacotes do motor. Alteracoes no combate, no bot, nos controles
ou correcoes de codigo exigem um novo APK assinado e instalado.

O canal remoto deve servir somente para conteudo enderecavel: prefabs visuais, materiais,
audio, tabelas de dados e outros assets que ja estejam previstos pelo APK instalado.

## Modelo proposto

Usar Addressables com catalogo remoto versionado. Cada APK de base guarda seu estado de
conteudo e consulta um catalogo remoto compativel quando abre. Nunca reutilizar nem
sobrescrever uma pasta de release ja publicada.

Estrutura esperada no servidor, a ser escolhida antes da primeira publicacao:

```
https://<host>/battle-royale-x/android/
  0.1.0-dev/
    catalog.json
    catalog.hash
    bundles/...
  0.1.1-dev/
    catalog.json
    catalog.hash
    bundles/...
```

## Fluxo de release

1. Gerar e guardar o APK base e seu `addressables_content_state.bin`.
2. Testar o APK no aparelho antes de anunciar qualquer conteudo remoto.
3. Para um update de conteudo, gerar o content update a partir do estado arquivado do APK.
4. Publicar bundles e catalogo em uma nova pasta imutavel; registrar versao, data, hash,
   compatibilidade minima e notas da release.
5. Testar primeiro no canal `device-test`; manter a release anterior para rollback.
6. Promover somente o mesmo conjunto de arquivos validado para o canal estavel.

## Ainda depende de decisao do responsavel pelo projeto

Antes de ativar a publicacao real, definir o host (por exemplo, Cloudflare R2, S3 ou outro),
o dono das credenciais, a URL definitiva e quem assina os APKs. Nenhum arquivo remoto deve
ser publicado com uma URL provisoria.

## Estado desta etapa

O projeto esta preparado para o APK de teste Android e para receber Addressables numa etapa
posterior. Nenhum asset foi marcado como remoto ainda, pois o combate-base usa dados e cena
locais e nao deve depender de internet para funcionar.
