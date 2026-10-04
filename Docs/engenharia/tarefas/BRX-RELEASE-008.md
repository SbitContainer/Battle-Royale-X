# BRX-RELEASE-008 — APK dos efeitos e registro Git

03/10/2026. Pedido: usuário autorizou gerar/publicar APK de VISUAL-007 e registrar todo trabalho deste projeto no Git. Sem autorização nova de instalação ADB. Estado EM EXECUÇÃO.

Setor A: fonte Git e release existente; consumidores B/C: builder, manifesto/hash/pacote/assinatura, GitHub release/tag/catálogo e atualizador instalado. Proibido: rebalancear ou alterar combate, dependências, assinatura, configurações permanentes, implementar OTA ou apagar dados do aparelho.

Baseline confirmado: raiz Battle Royale X; `main` em d2b0b067fe7613523c1d740351699f7a7fe63d9c; remoto https://github.com/SbitContainer/Battle-Royale-X.git. Fetch somente leitura da árvore: origin/main em 22325f58c095d36e074bb9224c65b67378f4fe1c, HEAD dez commits à frente e zero atrás. Mais de trezentas alterações/arquivos locais das tarefas anteriores, preservados. Nenhum pull/reset/force push.

Registrar snapshot integrado da fonte local validada, com assets originais/licenciados e `.meta`, scripts, testes, governança e registros de releases anteriores. Não incluir caches, Builds/Logs ignorados nem anexos privados do chat. Padrão explícito salvo no AGENTS.md por pedido do usuário; .gitignore exclui somente anexos adicionais. Lockfile já ignorado continua limite conhecido; não atualizar dependências nesta tarefa.

Evidência de entrada: VISUAL-007 validado Editor com 314 combate/UI e 48 estrutural visual, zero falhas; `Logs/visual007-matrix2.log`. Escopo do APK é essa fonte existente, sem executar fábricas de dados/cena. Preservar versão14 anterior e assinatura/pacote; nova versão reservada pelo builder. Verificar bytes públicos, catálogo e fonte após publicação. Testes físicos Android e ciclo real do instalador NÃO EXECUTADOS.
