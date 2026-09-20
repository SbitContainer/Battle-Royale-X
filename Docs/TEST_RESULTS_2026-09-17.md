# Resultado dos testes — Battle Royale X — 17/09/2026

## Resultado desta sessão

Projeto: C:\Users\netor\OneDrive\Desktop\Battle Royale X
Editor: Unity 6.6 (6000.6.1f1), URP, Windows/DX11.
Cena: Assets/BattleRoyaleX/GeneratedScenes/Prototype01_Arena.unity.

- Matriz de integração ao vivo: **51 verificações passaram; 0 falharam**.
- Validação estrutural de apresentação: **34 verificações passaram; 0 falharam**.
- Compilação/importação C#: concluída no Editor, seguida de entrada em Play Mode.
- Menu Battle Royale X: conferido e utilizado pela interface.
- Create Default Data: executado pelo fluxo Build Test Scene, que chama o gerador antes de criar a arena.
- Build Test Scene: executado pela interface; arena salva e usada na rodada final.
- git diff --check: sem erros de whitespace (avisos de conversão LF/CRLF não são falhas).
- Não foi gerado executável standalone nesta sessão.

Os 51 checks NÃO são 51 itens distintos do documento original: incluem asserções adicionais e regressões. A matriz original possui 47 itens: 4 de compilação/geração, conferidos no Editor, e 43 de comportamento cobertos pelos grupos abaixo. A entrada de teclado foi conferida por amostragem na janela real, além das verificações de mapeamento e dos motores; não houve sessão humana extensa de balanceamento.

## Falhas encontradas e corrigidas

1. **Câmera cortava o corpo dos personagens.** O cálculo considerava distância entre raízes, sem garantir que os corpos coubessem na projeção. Agora calcula distância necessária com margem, aspecto e FOV, mantendo pitch/yaw e suavização. O primeiro posicionamento é imediato. Regressões de corpo inteiro no spawn e com separação de 25 unidades passaram.
2. **Esferas de loot não eram coletadas na altura normal do personagem.** O trigger escalado não alcançava o collider do jogador. O gerador agora usa raio local 1.5 nas esferas de escala 0.65. A cena foi regenerada; coleta por colisão passou.
3. **Partículas geravam erro de runtime.** O ParticleSystem começava a tocar antes de receber a configuração de duration. Agora é parado/limpo e playOnAwake é desligado antes da configuração. A rodada final inteira registrou zero erros/exceções.
4. **Problema na própria instrumentação inicial:** o teste de airdrop acessava o grupo depois de sua destruição correta. A referência de posição passou a ser capturada antes da coleta e o teste verifica destruição do grupo/opções. Não foi alterado ChoicePickupGroup.

A primeira rodada registrada em Logs/live-matrix-20260917-203804.md foi interrompida: 40 checks passaram e 2 falharam (câmera e coleta); o teste de airdrop ainda usava a versão anterior carregada no Editor. Os casos restantes daquela rodada não contam como aprovados. A rodada completa abaixo foi feita depois da importação das correções e reconstrução da cena.

## Conferência visível e limites

- Operação da Unity feita pela interface, com a arena visível.
- P1 respondeu a D/W e dash H; P2 respondeu às setas esquerda/cima e movimento especial Num3. Mapeamentos completos WASD/setas também verificados no teste.
- Corpos dos dois personagens visíveis na Game View maximizada e reduzida após a correção.
- HUD de vida/energia/cooldowns e mensagem GUERREIRO VENCEU observadas.
- Os testes de integração invocam as APIs existentes de habilidades, inventário e motor, com frames, temporização e colisões reais da Unity; não simulam todas as teclas e combinações de duas pessoas.
- O airdrop usa o relógio real com limiar reduzido no teste; o valor padrão de 35 s é verificado. Não equivale a uma medição manual de precisão de 35 s.
- A suíte usa os modelos que já estavam na cena. Não foi importada arte nova, nem executada uma rodada exclusivamente com cápsulas nesta sessão.
- Não avaliado: diversão/balanceamento, polimento de animação/HUD, desempenho de longa duração, build standalone, multiplayer. Esses aspectos não são declarados aprovados.
- Arquitetura de combate preservada; nenhuma nova classe, progressão, equipamento ou hard CC foi implementado.

## Resultado individual — integração ao vivo

Fonte: Logs/live-matrix-20260917-204257.md.

- PASSOU: Câmera enquadra o corpo inteiro dos dois no spawn
- PASSOU: Câmera mantém corpos inteiros com maior separação
- PASSOU: P1: mapeamento WASD (entrada física verificada separadamente)
- PASSOU: P2: mapeamento setas (entrada física verificada separadamente)
- PASSOU: Motor do Guerreiro responde ao movimento
- PASSOU: Motor do Assassino responde ao movimento
- PASSOU: Personagens permanecem no plano XZ
- PASSOU: Ataque do Guerreiro causa dano uma vez por ativação via física
- PASSOU: Cooldown impede spam
- PASSOU: Ataque do Assassino causa dano uma vez por ativação via física
- PASSOU: Sem friendly fire na mesma equipe
- PASSOU: Dois ataques físicos colidem em Clash
- PASSOU: Clash causa apenas dano reduzido nos dois
- PASSOU: Micro-stagger do Clash inicia e expira
- PASSOU: Clash cancela ambas as hitboxes
- PASSOU: Guarda reduz dano
- PASSOU: Parry perfeito zera dano e aplica micro-stagger no atacante
- PASSOU: Parry fora da janela perfeita defende parcialmente
- PASSOU: Esquiva durante iframe evita dano de hitbox
- PASSOU: Dash percorre a distância configurada
- PASSOU: Travessia cruza o adversário
- PASSOU: Retorno volta dentro da janela
- PASSOU: Retorno expira e respeita cooldown
- PASSOU: Buff de ultimate inicia
- PASSOU: Buff expira e todos os modificadores voltam a 1.0
- PASSOU: Caçada reduz dano causado e aumenta mobilidade conforme dados
- PASSOU: Execução aumenta ameaça e preserva controle do oponente
- PASSOU: Mochila inicia com três slots
- PASSOU: Quarto item recusado sem upgrade
- PASSOU: Usar mochila de drop aumenta capacidade para quatro
- PASSOU: Item consumido desaparece
- PASSOU: Cura tem tempo de uso e dano a interrompe sem consumir
- PASSOU: Cura concluída restaura vida e consome item
- PASSOU: Essência conclui antes da cura e restaura energia
- PASSOU: Troca de runa aguarda e pode ser interrompida por dano
- PASSOU: Runa só é consumida ao concluir troca de cerca de 0.8 s
- PASSOU: Coleta real de esfera por colisão entra no inventário
- PASSOU: Repulsão desloca o adversário
- PASSOU: Barreira bloqueia movimento e expira
- PASSOU: Fumaça aparece como área placeholder
- PASSOU: Campo nulo cancela ataque nullifiable via física
- PASSOU: Airdrop aguarda relógio configurado (35 s padrão; limiar de teste reduzido)
- PASSOU: Airdrop apresenta três opções
- PASSOU: Pegar uma escolha elimina as outras duas
- PASSOU: Airdrop não concede a mesma recompensa duas vezes
- PASSOU: Magia versus magia explode em área sem dano duplicado
- PASSOU: Ataque físico marcado anula magia via física
- PASSOU: Anulação entra em cooldown separado de 30 s
- PASSOU: Durante cooldown novo ataque não anula magia
- PASSOU: Golpe final gera vitória do Guerreiro na HUD
- PASSOU: Nenhum erro ou exceção durante a execução

## Resultado individual — validação estrutural

Fonte: Logs/Editor.log, última execução de Validate Visual Scene; resumo [BRX VISUAL SUMMARY] pass=34 fail=0.

- PASSOU: Assets/Guerreiro: modelo existe
- PASSOU: Assets/Guerreiro: avatar humanoide valido
- PASSOU: Assets/Guerreiro: malha skinned existe
- PASSOU: Assets/Assassino: modelo existe
- PASSOU: Assets/Assassino: avatar humanoide valido
- PASSOU: Assets/Assassino: malha skinned existe
- PASSOU: Animacoes/locomocao: clips obrigatorios existem
- PASSOU: Animacoes/combate: clips obrigatorios existem
- PASSOU: Animator/controller gerado existe
- PASSOU: Animator/parametros de apresentacao completos
- PASSOU: Animator/estados de apresentacao completos
- PASSOU: Cena/asset gerado existe
- PASSOU: Cena/exatamente dois personagens logicos
- PASSOU: Cena/Guerreiro: CharacterController no root logico
- PASSOU: Cena/Guerreiro: Hurtbox preservada
- PASSOU: Cena/Guerreiro: root logico sem renderer
- PASSOU: Cena/Guerreiro: VisualModel filho existe
- PASSOU: Cena/Guerreiro: Animator configurado
- PASSOU: Cena/Guerreiro: root motion desativado
- PASSOU: Cena/Guerreiro: ponte visual configurada
- PASSOU: Cena/Guerreiro: modelo renderizavel
- PASSOU: Cena/Guerreiro: sem fallback de capsula
- PASSOU: Cena/Assassino: CharacterController no root logico
- PASSOU: Cena/Assassino: Hurtbox preservada
- PASSOU: Cena/Assassino: root logico sem renderer
- PASSOU: Cena/Assassino: VisualModel filho existe
- PASSOU: Cena/Assassino: Animator configurado
- PASSOU: Cena/Assassino: root motion desativado
- PASSOU: Cena/Assassino: ponte visual configurada
- PASSOU: Cena/Assassino: modelo renderizavel
- PASSOU: Cena/Assassino: sem fallback de capsula
- PASSOU: Cena/apresentador de VFX de combate existe
- PASSOU: Cena/HUD de combate referencia os dois jogadores
- PASSOU: Cena/camera preserva os dois alvos

## Arquivos criados ou alterados nesta sessão

Caminhos abaixo relativos à raiz do projeto indicada no início.

### Correções

- Assets/BattleRoyaleX/Runtime/Camera/IsometricCameraRig.cs
- Assets/BattleRoyaleX/Runtime/Combat/CombatEventVfxPresenter.cs
- Assets/BattleRoyaleX/Editor/PrototypeSceneBuilder.cs
- Assets/BattleRoyaleX/GeneratedScenes/Prototype01_Arena.unity — regenerada.

### Testes e relatório

- Assets/BattleRoyaleX/Editor/PrototypeLiveTestLauncher.cs — novo menu.
- Assets/BattleRoyaleX/Editor/PrototypeLiveTestLauncher.cs.meta — gerado pela Unity.
- Assets/BattleRoyaleX/Runtime/Debug/PrototypeLiveTests.cs — nova suíte de integração visível, somente no Editor.
- Assets/BattleRoyaleX/Runtime/Debug/PrototypeLiveTests.cs.meta — gerado pela Unity.
- Docs/TEST_RESULTS_2026-09-17.md — este relatório.

### Saídas locais

- Logs/live-matrix-20260917-203804.md — primeira tentativa interrompida.
- Logs/live-matrix-20260917-204257.md — rodada final completa.
- Logs/Editor.log — log contínuo da Unity; caches/arquivos temporários do Editor também são atualizados automaticamente.

O gerador também executou as rotinas existentes de dados e configuração visual. Não foram observadas novas gravações dos assets de GeneratedData/GeneratedVisuals nessa reconstrução. Alterações anteriores já existentes no Git (inclusive AbilityController, CharacterMotor25D, Hitbox, .gitignore e assets não rastreados) não são atribuídas a esta sessão. Pastas My project e BTX foram preservadas. Nenhum commit ou push foi realizado.

## Repetir a suíte

Com Prototype01_Arena aberta e fora do Play Mode: Battle Royale X > Prototype 01 > Run Live Test Matrix. O teste entra em Play Mode e mostra o progresso na tela. Ao terminar, parar e iniciar Play restaura uma partida normal.

