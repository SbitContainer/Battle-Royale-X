# Mapa de impacto do Battle Royale X

Mapeado por leitura local em 03/10/2026. Caminhos abaixo são relativos a `Assets/BattleRoyaleX`, salvo indicação contrária. As referências principais estão em [mapa.json](engenharia/mapa.json). O validador confere existência de arquivos e símbolos, não prova todo o grafo de chamadas; antes de alterar um contrato, buscar novamente com `rg`.

| Setor A e entrada | Consumidores B/C observados | Contrato e prova necessária |
|---|---|---|
| Input: `Runtime/Input/PrototypeMobileTouchControls.cs`, `SubmitCast`; `.Settings.cs`, `CycleLabVariation` | AbilityController, PrototypeLabController, Motor, HUD e preview | Toque/arraste/cancelamento; menu não bloqueia seletores de arena; teste EventSystem e Android |
| Habilidades: `Runtime/Abilities/AbilityController.cs`, `TryUseInternal`, `ResetTransientState`; partials V1/Hunt | Hitbox, campos/clones, Energy, State, Motor, eventos e recast na UI | Uma ativação válida/custo, cleanup por owner, cooldown e direção; matriz PlayMode |
| Acerto: `Runtime/Combat/Hitbox.cs`, `TryResolveHurtbox`; `CombatResolver.cs`, `ResolveAttack` | DefenseController, Health, Motor, CombatEvents, VFX/barras/bot | Dano único por contato, iframe, clash, block/parry e travessia; preservar quatro classes |
| Estado: `Runtime/Characters/CharacterRuntime.cs`, `Initialize`, `ResetTransientState` | Abilities, Health/Energy, Inventory, defesa, regeneração | Classe A→B→A não reutiliza efeitos/cargas; estado por ator, não por asset |
| Movimento/câmera/bot: CharacterMotor25D, IsometricCameraRig, PrototypeTrainingBot | Colisão, mira, enquadramento, AbilityController | Controle contínuo, paredes, modo parado/ataque; bot usa mesma API, não dano fictício |
| Inventário/mundo: `Inventory/WorldPickup.cs`, `TryCollect`; InventoryController; AirdropManager | EquipVariation, Health/Energy, TacticalEffectSpawner, fumaça e UI | Não consumir item se uso falhar; seleção única de drop; potion/regen e variant restriction |
| Laboratório: `Runtime/Debug/PrototypeLabController.cs`, `SwitchPlayerClass`, `EquipVariation` | CharacterRuntime, modelos ativos, bot, HUD e controles | Basic fixo, três slots A/B/C, cleanup; alias PrototypeCombatLabController preserva cenas serializadas |
| Dados: AbilityDefinition/CharacterDefinition e `Editor/PrototypeDataFactory.cs`, `CreateDefaultData` | Execução, descrições técnicas, pickups, SceneBuilder e perfis visuais | Enum/GUID estável; regerar duas vezes não conservar valor antigo; mudar gerador e verificar asset |
| Apresentação: `Runtime/Visual/CombatVFXRouter.cs`, `OnCombatEvent`; CharacterVisualAnimator | Perfis/prefabs, shader `Assets/Resources/BRXCombatFx.shader`, câmera e Android | VFX ausente não altera dano; modelo ativo recebe animação; capturas e legibilidade no aparelho |
| Cena/build: `Editor/PrototypeSceneBuilder.cs`, `BuildScene`; PrototypePolishTools, BuildArcherAndroid | GeneratedData, GeneratedScenes, ProjectSettings, APK, live tests | Gerador altera cena; build não prova instalação; conferir versão/hash e assinatura |
| Preferências: `.Settings.cs`, `SerializeLayout`, `ApplyLayout`, `SaveLayoutAndClose` | PlayerPrefs, safe area, botões e reinício | Layout inválido/restauração/orientação; nunca usar limpeza global de PlayerPrefs como reparo |

## Dependências indiretas que não podem ser esquecidas

- Alterar dano/timing: AbilityDefinition → Controller/Resolver → Health/Defense → barra, feedback, bot e testes. Corrigir só o texto da skill não altera o golpe.
- Alterar enum/ID: dados serializados → gerador → prefab/cena → UI. Não renumerar `BRXTypes` nem perder `.meta`.
- Alterar visual: fábrica → prefab/material → perfil → listener → shaders do APK. Código de geração correto não prova que o asset foi regenerado e instalado.
- Trocar classe: Initialize → reset de transient state → remoção de owned effects → seleção do visual ativo → UI/bot. A origem do input e a referência do ator precisam coincidir.
- Alterar artefato: fonte → build → hash → possível publicação → instalação → aparelho. Resultado de uma etapa não comprova a seguinte.

Rede/save de partida/sync não têm implementação mapeada nesta adoção. Só criar esses setores quando uma tarefa aprovada adicionar contratos e implementação reais.

## Apresentação/áreas locais do Guerreiro — BRX-VISUAL-006

Continuação visual [BRX-VISUAL-007](engenharia/tarefas/BRX-VISUAL-007.md): `WarriorSkillPresentation` possui `WarriorReferenceVfx` como filho estritamente visual; `Assets/Resources/WarriorEnergy.shader` oferece ribbons suaves e superfícies hexagonais azul/douradas. `CombatVFXRouter` mapeia três passos básicos/fases de ultimates e contatos de defesa; `CombatEventVfxPresenter` não duplica o corte básico nem a guarda antiga. Testes em `PrototypeLiveTests.WarriorRework` verificam ausência de física/dano no helper, shaders, limite de renderers e cleanup; capturas em Logs/Warrior007. Nada muda nos dados ou na autoridade da execução.

`AbilityController.Warrior.BeginWarriorGroundEffect` fixa o centro na posição real de ativação; `AbilityAimSolution` mostra área autocentrada. `CombatVFXRouter` conecta startup ao efeito de conjuração e Hit positivo ao impacto no alvo; `WarriorSkillPresentation` produz geometria/partículas próprias sem autoridade de dano. `CharacterVisualAnimator` gesticula durante carga sem root motion. Factory das ultimates e `AbilityTechnicalInfo` são consumidores obrigatórios; teste `PrototypeLiveTests.WarriorRework` cobre pulsos/alcance/cancelamento e captura imagens. Não regenerar cena nem alterar outras classes. Evidência em [BRX-VISUAL-006](engenharia/tarefas/BRX-VISUAL-006.md).

## Atualização Android — BRX-RELEASE-003

Setor release: `Editor/PrototypeApkReleaseBuild.cs` gera APK versionado da cena existente; `Runtime/Release` consulta o GitHub sem participar da resolução de combate; `Assets/Plugins/Android/BRXUpdates.androidlib` verifica pacote/certificado e fornece URI privada ao instalador. Consumidores: conexão HTTPS, cache privado, permissões Android e instalador. `Tools/Release/Publish-GitHubApk.ps1` valida/publica artefatos; nenhum token entra no jogo. Contrato e limites em [ANDROID_GITHUB_UPDATES.md](ANDROID_GITHUB_UPDATES.md). Exigir compilação Unity/Java, pacote/assinatura do APK e teste físico A → B separadamente. Não confundir com OTA de conteúdo/Addressables.

## Assassino e catálogo de interações — 03/10/2026

`AbilityController.Assassin.cs` executa Execução, Travessia e Círculo de Adagas. `AssassinDaggerAnchor` controla a posição física da adaga; `AssassinDaggersPresentation` somente apresenta lâminas/rastros. `DefenseInteractionRule` é o perfil reutilizável de elegibilidade, consumido por DefenseController e pela órbita. `AbilityInteractionMapping.Export` gera a lista de IDs/flags em ABILITY_INTERACTIONS; o catálogo numerado das regras existentes é [INTERACTION_RULES.md](INTERACTION_RULES.md).

Invisibilidade → SmokeVisibility, aquisição de alvos/preview, perseguição e bot. Janela de adaga → CanRecast/UI, reset e troca de variante. Teste representativo: `PrototypeLiveTests.Assassin.cs`. Gerador limitado: `PrototypeDataFactory.UpdateAssassinVariants`, sem reconstruir cena nem regenerar dados das outras classes.
