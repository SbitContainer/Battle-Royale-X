# Origem das ferramentas de engenharia

Kit local Arquitetura-Reutilizavel-Projetos v1.1, 03/10/2026, solicitado pelo usuário. A função `verifyFile` foi extraída de `componentes/node/primitives.cjs`, SHA256 `AE245406EDF29B93E99CB18C85882E711F26342D53FF6B13BF6FCB87BE578781`.

A origem documentada do componente é `SbitContainer/CNC-code`, commit `7eda2e7ad1fb6f197e0dacebce0dc4f5fd87d043`, `apps/desktop-app/release-catalog.cjs`; o kit adaptou verificação streaming SHA256. Não foram copiados caminhos de produção, dados, credenciais ou o aplicativo.

`verify-file.cjs` mantém o contrato de tamanho/hash do kit. `verify-artifact.cjs`, `validate.cjs` e seus testes são adaptadores novos para o Battle Royale X, sem dependências npm. Node é ferramenta de desenvolvimento fora de Assets, não dependência do APK. Usar Node com suporte a `node:test`; executado localmente em v24.14.1.

O helper de versão estrita x.y.z do kit não foi importado: ele não aceita `0.9.2-effects-lab`. Não substituir comparação de Android versionCode por esse helper. Hash comprova bytes, não autor, assinatura, conteúdo do manifest Android ou autorização de publicação. O caminho é confinado à raiz para evitar verificação acidental de outro projeto.

Permissão pessoal de reutilização não define licença de redistribuição pública do kit; verificar titularidade antes de vender/publicar os componentes como biblioteca. Ferramentas só leem arquivos; os testes escrevem exclusivamente fixtures sintéticas em diretórios temporários exclusivos.
