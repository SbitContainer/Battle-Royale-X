# Matriz ao vivo — 19/09/2026 15:27:12

Testes no Play Mode, com comandos às APIs reais e colisões da Unity. Não substituem avaliação humana de diversão.

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
- PASSOU: Travessia do Assassino causa 24 de dano uma única vez
- PASSOU: Guarda reduz travessia de 24 para 6 de dano
- PASSOU: Parry nega travessia e abre contra-ataque manual
- PASSOU: Contra-ataque do Guerreiro exige básico e adiciona 6 de dano uma vez
- PASSOU: Investida do Guerreiro causa apenas 6 de dano uma vez
- PASSOU: Travessia não causa friendly fire
- PASSOU: Travessia fora do trajeto não acerta
- PASSOU: Parede bloqueia dash e dano além dela
- PASSOU: Buff de ultimate inicia
- PASSOU: Buff expira e todos os modificadores voltam a 1.0
- PASSOU: Cinco Cortes cobra energia uma vez e aceita exatamente cinco dashes manuais
- PASSOU: Cinco Cortes causa 12 por travessia e 60 no total sem defesa
- PASSOU: Cinco Cortes encerra cargas e mantém cooldown iniciado no primeiro cast
- PASSOU: Caçada reduz dano causado e aumenta mobilidade conforme dados
- PASSOU: Execução aumenta ameaça e preserva controle do oponente
- PASSOU: Mochila inicia com três slots
- PASSOU: Quarto item recusado sem upgrade
- PASSOU: Usar mochila de drop aumenta capacidade para quatro
- PASSOU: Item consumido desaparece
- PASSOU: Cura tem tempo de uso e dano a interrompe sem consumir
- PASSOU: Cura concluída restaura vida e consome item
- PASSOU: Essência conclui antes da cura e restaura energia
- PASSOU: Todas as 12 variações existem como pickups no chão
- PASSOU: Runa de outra classe é recusada e permanece no chão
- PASSOU: Botão de coleta adiciona uma runa compatível e emite identificação do item
- PASSOU: Usar runa coletada muda a habilidade de movimento exibida e consome o item
- PASSOU: Troca de runa aguarda e pode ser interrompida por dano
- PASSOU: Runa só é consumida ao concluir troca de cerca de 0.8 s
- PASSOU: Coleta real de poção por colisão entra no inventário
- PASSOU: Poção coletada do chão conclui a cura e é consumida
- PASSOU: Repulsão desloca o adversário
- PASSOU: Barreira bloqueia movimento e expira
- PASSOU: Granada cria nuvem de fumaça suave com volume legível
- PASSOU: Fumaça bloqueia a linha de visão usada pelo bot
- PASSOU: Campo nulo cancela ataque nullifiable via física
- PASSOU: Airdrop aguarda relógio configurado (12 s padrão; limiar de teste reduzido)
- PASSOU: Airdrop apresenta três opções
- PASSOU: Pegar uma escolha elimina as outras duas
- PASSOU: Airdrop não concede a mesma recompensa duas vezes
- PASSOU: Magia versus magia explode em área sem dano duplicado
- PASSOU: Ataque físico marcado anula magia via física
- PASSOU: Anulação entra em cooldown separado de 30 s
- PASSOU: Durante cooldown novo ataque não anula magia
- PASSOU: Warrior_Defense_Base: defesa real abre counter sem causar dano automático
- PASSOU: Warrior_Defense_Base: próximo básico empurra longe, dano único e controle preservado
- PASSOU: Warrior_Defense_A: defesa real abre counter sem causar dano automático
- PASSOU: Warrior_Defense_A: próximo básico empurra longe, dano único e controle preservado
- PASSOU: Warrior_Defense_B: defesa real abre counter sem causar dano automático
- PASSOU: Warrior_Defense_B: próximo básico empurra longe, dano único e controle preservado
- PASSOU: Counter também pode ser bloqueado: dano e empurrão reduzidos
- PASSOU: Oportunidade de counter expira sem ataque automático
- PASSOU: Assassin_Defense_Base: defesa no contato muda direção e atravessa o atacante
- PASSOU: Assassin_Defense_Base: travessia defensiva causa só 2–3 de dano uma vez
- PASSOU: Assassin_Defense_Base: viagem encerra sem invulnerabilidade ou ação presa
- PASSOU: Assassin_Defense_A: defesa no contato muda direção e atravessa o atacante
- PASSOU: Assassin_Defense_A: travessia defensiva causa só 2–3 de dano uma vez
- PASSOU: Assassin_Defense_A: viagem encerra sem invulnerabilidade ou ação presa
- PASSOU: Assassin_Defense_B: defesa no contato muda direção e atravessa o atacante
- PASSOU: Assassin_Defense_B: travessia defensiva causa só 2–3 de dano uma vez
- PASSOU: Assassin_Defense_B: viagem encerra sem invulnerabilidade ou ação presa
- PASSOU: Esquiva defensiva respeita paredes e não causa dano atrás delas
- PASSOU: Bot se aproxima por movimento e investida usando APIs normais
- PASSOU: Bot observa skill real, mas não reage antes do atraso humano
- PASSOU: Bot identifica trajetória ofensiva e reage após pelo menos 0,20 s
- PASSOU: Bot não lê habilidades inimigas através da fumaça
- PASSOU: Bot busca nova posição sem ler skill através da parede
- PASSOU: Bot reconhece uso público de variação defensiva
- PASSOU: Reset cancela viagem, iframe e efeitos defensivos transitórios
- PASSOU: Warrior_Defense_Base: ativação real gera evento Active e efeito novo
- PASSOU: Warrior_Defense_A: ativação real gera evento Active e efeito novo
- PASSOU: Warrior_Defense_B: ativação real gera evento Active e efeito novo
- PASSOU: Warrior_Move_Base: ativação real gera evento Active e efeito novo
- PASSOU: Warrior_Move_A: ativação real gera evento Active e efeito novo
- PASSOU: Warrior_Move_B: ativação real gera evento Active e efeito novo
- PASSOU: Warrior_Ult_Base: ativação real gera evento Active e efeito novo
- PASSOU: Warrior_Ult_A: ativação real gera evento Active e efeito novo
- PASSOU: Warrior_Ult_B: ativação real gera evento Active e efeito novo
- PASSOU: Assassin_Defense_Base: ativação real gera evento Active e efeito novo
- PASSOU: Assassin_Defense_A: ativação real gera evento Active e efeito novo
- PASSOU: Assassin_Defense_B: ativação real gera evento Active e efeito novo
- PASSOU: Assassin_Move_Base: ativação real gera evento Active e efeito novo
- PASSOU: Assassin_Move_A: ativação real gera evento Active e efeito novo
- PASSOU: Assassin_Move_B: ativação real gera evento Active e efeito novo
- PASSOU: Assassin_Ult_Base: ativação real gera evento Active e efeito novo
- PASSOU: Assassin_Ult_A: ativação real gera evento Active e efeito novo
- PASSOU: Assassin_Ult_B: ativação real gera evento Active e efeito novo
- PASSOU: Efeitos das variações respeitam o limite compartilhado de 70 objetos
- PASSOU: Golpe final gera vitória do Guerreiro na HUD
- PASSOU: Nenhum erro ou exceção durante a execução

Resultado: 112 passaram; 0 falharam.

## Escopo e evidência

- Unity 6000.6.1f1, URP, Play Mode. Foram usadas as APIs reais, CharacterController, hitboxes e os eventos de combate.
- Validação estrutural da cena: 40 passaram, 0 falharam (`Logs/Editor.log`, execução `validate:defense-ai-v3-1`).
- Smoke test visual: 17 passaram, 0 falharam (`Logs/polish-smoke.txt`, execução de 19/09/2026 15:33:19).
- Captura inspecionada: `Logs/defense-ai-guards.png`, guarda Fortaleza e Contra-Sombra. É evidência visual estática no Editor, não teste de fluidez no celular.
- Primeira tentativa: 80 passaram, 12 falharam porque o comando de geração executou antes da recompilação do gerador. Os assets ainda tinham esquiva com dano 8 e counter sem deslocamento. Regeneração após compilação gravou 2/3 de dano defensivo e 3,5/4/4,5 m de counter; segunda matriz passou integralmente. Foram acrescentadas cobertura de parede/IA e 18 verificações de apresentação.
- Os testes de VFX provam emissão na fase ativa e criação de efeitos; não equivalem à aprovação artística das 18 habilidades. A captura mostra duas guardas.
- Bot: cenário de aproximação, atraso mínimo de resposta, observação de habilidades reais e oclusão por fumaça/parede. Não é prova de inteligência geral, navegação global ou equilíbrio competitivo.
- Pendente no Moto G54: instalação, conforto de toque, duelo de pelo menos 3 minutos, avaliação de dificuldade e medição de FPS. ADB não tinha dispositivo conectado nesta rodada.

## Arquivos editados nesta rodada

Relativos à raiz do projeto:

- `Assets/BattleRoyaleX/Runtime/Data/AbilityDefinition.cs`
- `Assets/BattleRoyaleX/Runtime/Core/BRXTypes.cs`
- `Assets/BattleRoyaleX/Runtime/Combat/DamagePacket.cs`
- `Assets/BattleRoyaleX/Runtime/Combat/DefenseController.cs`
- `Assets/BattleRoyaleX/Runtime/Combat/CombatResolver.cs`
- `Assets/BattleRoyaleX/Runtime/Combat/CombatEventVfxPresenter.cs`
- `Assets/BattleRoyaleX/Runtime/Characters/CharacterStateController.cs`
- `Assets/BattleRoyaleX/Runtime/Characters/CharacterMotor25D.cs`
- `Assets/BattleRoyaleX/Runtime/Characters/CharacterVisualAnimator.cs`
- `Assets/BattleRoyaleX/Runtime/Characters/PrototypeTrainingBot.cs`
- `Assets/BattleRoyaleX/Runtime/Abilities/AbilityController.cs`
- `Assets/BattleRoyaleX/Runtime/Inventory/InventoryController.cs`
- `Assets/BattleRoyaleX/Runtime/Debug/PrototypeCombatHUD.cs`
- `Assets/BattleRoyaleX/Runtime/Debug/PrototypeLiveTests.cs`
- `Assets/BattleRoyaleX/Editor/PrototypeDataFactory.cs`
- `Assets/BattleRoyaleX/Editor/PrototypeMobileBuildSetup.cs`
- `Assets/BattleRoyaleX/Editor/PrototypePolishTools.cs`
- `Docs/CODEX_START_HERE.md`
- `Docs/DEFENSE_AI_V3.md` (novo)
- `Docs/TEST_RESULTS_DEFENSE_AI_V3.md` (novo)

Regerados pelo Editor: definições em `Assets/BattleRoyaleX/GeneratedData/Abilities/` (novos campos e valores das seis defesas), `Assets/BattleRoyaleX/GeneratedScenes/Prototype01_Arena.unity` (bot e dados), visuais pelo gerador existente e versão Android em `ProjectSettings/ProjectSettings.asset`. Nenhum commit, push ou modificação em servidor foi feito.

## APK verificado

- Build Android Release concluído em 19/09/2026 15:35:36: `Succeeded errors=0`, duração aproximada 110 s.
- Arquivo: `Builds/BattleRoyaleX-defense-ai-v3.apk`, 47.738.105 bytes.
- Package confirmado por aapt2: `com.sbitcontainer.battleroyalex.prototype`.
- VersionName `0.4.0-defense-ai`, versionCode `4`.
- SHA-256: `59C1854461E18BFEADCD888B9BE9D6D6C4627DB1C3DD32F44A9E3CF3286E7366`.
- APK anterior preservado. Nenhum aparelho listado em `adb devices -l` na verificação final, portanto esta versão não foi instalada nem testada fisicamente.
- Pequeno ajuste de texto posterior à matriz: HUD traduz eventos de habilidade para o nome da skill e ignora fases não ativas. Build compila a alteração; a captura anterior ainda mostra o rótulo técnico antigo.
