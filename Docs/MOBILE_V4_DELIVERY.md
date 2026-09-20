# Entrega mobile V4 — 19/09/2026

## Escopo implementado

- Botões com diâmetro inicial 2×, editor de posição/tamanho e persistência local.
- Menu de dados técnicos gerados das habilidades equipadas; pausa enquanto consulta/edita.
- Troca Guerreiro jogador / Assassino bot e vice-versa, preservando inventários e variações.
- Caçada com dois acionamentos, perseguição de alvo em dash, dano/empurrão e travessia no lado oposto.
- Travessia com bônus de velocidade independente de 2 segundos; Retorno preservado.
- Contra-Sombra como fumaça densa de fuga; bloqueia ataques internos, esconde inimigo, mantém personagem local fosco. Golpes de fora podem acertar, conforme confirmação do usuário.
- Esquivas com janela reativa de 1 segundo, travessia longa de baixo dano e imunidade completa durante a janela/viagem, conforme a revisão mais recente.

Regras e números: `MOBILE_V4_RULES.md`. Resultados individuais: `TEST_RESULTS_MOBILE_V4.md`.

## Resultado final no Editor

- Matriz de integração em Play Mode: **140 passaram, 0 falharam**, incluindo todos os casos novos da esquiva, fumaça, Caçada, velocidade, interface e troca de controle.
- Validação estrutural da cena/modelos/animações: **40 passaram, 0 falharam**.
- Smoke test visual de runtime (animação, raiz lógica/pés no chão, modelos e HUD): **17 passaram, 0 falharam**.
- Sem erros/exceções de runtime na matriz. A compilação mantém avisos preexistentes de API de ordenação obsoleta (`FindObjectsSortMode`), sem erro C#.
- Diagnóstico após corrigir o bot: uma resposta defensiva, atraso observado de 0,30 s (mínimo 0,20 s), sem ação ofensiva ocupando a reação.
- Capturas conferidas: `Logs/mobile-v4-controls.png`, `Logs/mobile-v4-info.png`; artefatos de conferência do Editor, não prova de fluidez no aparelho.

## Histórico de verificação

1. Primeira execução: 135 passaram / 4 falharam. A proteção final havia sido colocada incorretamente em `Heal` em vez de `ApplyDamage`. Corrigido; cura não é bloqueada pela esquiva.
2. Segunda: 138 passaram / 1 falhou. Os quatro casos de imunidade passaram. O teste de parry parcial usava espera fixa mais startup do ataque, podendo ultrapassar a defesa sob queda de FPS. Agora sincroniza o contato com a janela real da defesa, sem alterar valores da habilidade.
3. Terceira: 139 passaram / 1 falhou (incluído teste de Caçada do bot). O teste de reação do bot exigia resposta após um dash mais curto que sua latência humana. Agora usa um cast real em fixture com preparação de 0,65 s; verifica que não reage cedo e reage depois de 0,20 s. Não altera o bot nem as definições de produção para tornar sua reação instantânea.
4. A repetição determinística ainda detectou uma falha real no bot: ele tratava a preparação ofensiva inimiga como abertura, começava a própria ultimate e ficava `actionBusy` antes da reação defensiva. Diagnóstico registrado em `Logs/Editor.log` (`BRX BOT REACTION PROBE`: resposta 0, ameaça válida, busy=True, sem dash/stagger). Corrigido: não começa ultimate enquanto uma ameaça ofensiva observada estiver em andamento. Latência humana preservada.
5. A matriz final usa passo de simulação fixo de 1/60 s, restaurado ao terminar/interromper. Isso testa lógica/física de forma reproduzível, **não mede FPS ou desempenho no celular**.

## Arquivos de código alterados/criados nesta rodada

Relativos à raiz do projeto; alterações de rodadas anteriores não foram revertidas ou incluídas como novas.

- `Assets/BattleRoyaleX/Editor/PrototypeDataFactory.cs`
- `Assets/BattleRoyaleX/Editor/PrototypePolishTools.cs`
- `Assets/BattleRoyaleX/Runtime/Abilities/AbilityController.cs`
- `Assets/BattleRoyaleX/Runtime/Abilities/AbilityController.Hunt.cs` (novo)
- `Assets/BattleRoyaleX/Runtime/Characters/CharacterMotor25D.cs`
- `Assets/BattleRoyaleX/Runtime/Characters/CharacterRuntime.cs`
- `Assets/BattleRoyaleX/Runtime/Characters/CharacterStateController.cs`
- `Assets/BattleRoyaleX/Runtime/Characters/CharacterVisualAnimator.cs`
- `Assets/BattleRoyaleX/Runtime/Characters/HealthComponent.cs`
- `Assets/BattleRoyaleX/Runtime/Characters/PrototypeTrainingBot.cs`
- `Assets/BattleRoyaleX/Runtime/Characters/SmokeVisibility.cs` (novo)
- `Assets/BattleRoyaleX/Runtime/Combat/CombatEventVfxPresenter.cs`
- `Assets/BattleRoyaleX/Runtime/Combat/CombatResolver.cs`
- `Assets/BattleRoyaleX/Runtime/Combat/DefenseController.cs`
- `Assets/BattleRoyaleX/Runtime/Combat/Hitbox.cs`
- `Assets/BattleRoyaleX/Runtime/Core/BRXTypes.cs`
- `Assets/BattleRoyaleX/Runtime/Data/AbilityDefinition.cs`
- `Assets/BattleRoyaleX/Runtime/Data/AbilityTechnicalInfo.cs` (novo)
- `Assets/BattleRoyaleX/Runtime/Debug/PrototypeLiveTests.cs`
- `Assets/BattleRoyaleX/Runtime/Debug/PrototypeLiveTests.MobileV4.cs` (novo, apenas Editor)
- `Assets/BattleRoyaleX/Runtime/Input/PrototypeMobileTouchControls.cs`
- `Assets/BattleRoyaleX/Runtime/Input/PrototypeMobileTouchControls.Settings.cs` (novo)
- `Assets/BattleRoyaleX/Runtime/World/SmokeField.cs`
- `Assets/BattleRoyaleX/Resources/SmokeCloud.shader` (novo)

Unity gerou os `.meta` dos seis novos arquivos acima. Documentação alterada/criada: `Docs/TEST_MATRIX.md`, `Docs/MOBILE_V4_RULES.md`, `Docs/TEST_RESULTS_MOBILE_V4.md`, `Docs/MOBILE_V4_DELIVERY.md`.

Regravados pelos geradores existentes: as 20 definições em `Assets/BattleRoyaleX/GeneratedData/Abilities/` para serialização dos novos campos (mudanças de comportamento em Assassin_Defense_Base/A/B, Assassin_Move_A, Assassin_Ult_B e correção do slot de Assassin_Ult_Base); cena `Assets/BattleRoyaleX/GeneratedScenes/Prototype01_Arena.unity`; assets visuais do gerador existente. A versão do APK é gravada em `ProjectSettings/ProjectSettings.asset`. Nenhuma arte externa nova, commit, push, servidor ou sistema multiplayer.

## Como testar no aparelho

APK Android gerado com sucesso em 19/09/2026 17:38:13 (`Succeeded errors=0`, aproximadamente 91 s):

- `Builds/BattleRoyaleX-mobile-v4.apk` — 47.750.675 bytes.
- App: `Battle Royale X`; pacote `com.sbitcontainer.battleroyalex.prototype`.
- VersionName `0.5.0-mobile-v4`, versionCode `5`, confirmados por `aapt2`.
- SHA-256 `0701B5CD4C663B73ABFB1BBFEFEAC826874C9FB3387CE9137A0A70E4AF62FC93`.
- APK anterior preservado.
- Instalado com `adb install -r`: `Success`. O Moto G54 confirmou `versionCode=5`, `versionName=0.5.0-mobile-v4` no pacote instalado.
- Comando de abertura enviado à `com.unity3d.player.UnityPlayerActivity`; processo do jogo identificado. Nenhuma exceção Unity/AndroidRuntime na consulta inicial filtrada pelo processo.
- Conferência física da interface aguardando desbloqueio: o telefone estava `Asleep` / `SCREEN_STATE_OFF`, com bloqueio de tela ativo. Tela despertada sem contornar o bloqueio; solicitado desbloqueio ao usuário. Não considerar menu/toque/persistência após reinício validados no aparelho até essa etapa ser concluída.

1. Abrir **Battle Royale X** e tocar **MENU / SKILLS**.
2. **EDITAR BOTÕES**: arrastar círculos, selecionar um e usar MENOR/MAIOR; SALVAR. Fechar/reabrir o app para conferir persistência física.
3. **TROCAR PERSONAGEM** alterna o personagem controlado; o outro vira bot.
4. Pegar runas de Caçada, Travessia, Contra-Sombra e Duplo Passo e usar no inventário para equipar.
5. Caçada: primeiro toque perto de inimigo; após chegar, tocar novamente durante a contagem da segunda etapa.
6. Esquiva: testar com vários golpes durante 1 s, inclusive golpe no fim que dispare travessia. A vida não deve cair enquanto protegido.
7. Fumaça: testar ambos dentro e depois atacante fora. Ocultação não deve ser confundida com invulnerabilidade.

Ergonomia, qualidade percebida da fumaça, fluidez/FPS, toque físico e retenção após encerramento do processo no Moto G54 ainda precisam de validação no aparelho; os testes de Editor não substituem isso.
