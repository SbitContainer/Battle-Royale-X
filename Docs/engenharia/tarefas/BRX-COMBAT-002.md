# Assassino e mapeamento das interações existentes

Data: 03/10/2026. Autor: agente principal, sem delegação. Baseline main com alterações anteriores preservadas; sem pull/commit/push.

## Esperado / proibido

- Execução persegue, golpeia uma vez e oculta por 2 s após contato, sem invulnerabilidade.
- Travessia lança adaga, dano leve; um segundo toque teleporta até ela e golpeia mais forte.
- Substituir somente Duplo Passo por cinco adagas orbitais, 3 s, 50% de repelir golpes elegíveis e dano contínuo próximo. Preservar Esquiva Sombria/fumaça/Caçada/Retorno.
- Catalogar e numerar as interações já implementadas antes de estabelecer novas exceções. Não excluir todas as ultimates; lista ainda não definida pelo usuário.
- Não rebalancear outras classes, mudar equipamentos, rede, arte externa ou publicar/instalar APK por autorização antiga. Preservar flags antigas enquanto R01/R02 são disponibilizados.

Dados iniciais ajustáveis: Execução 18, alcance 9 m; Travessia 4 + 12, alcance 8 m, janela 3 s; órbita 2 por pulso de 0,5 s, raio 1,8 m, recarga 8 s. Exceções orbitais vazias aguardam decisão.

## Impacto e evidências

A: controller/dados/DefenseController. B/C: motor, Hitbox/resolver, animação/visibilidade, bot, mira, informações técnicas, UI de recast, gerador, testes e catálogo. Novos valores do enum são append-only; assets preservam GUID. Atualização de dados limitada às três variantes e perfis compartilhados novos, sem rebuild de cena.

Oráculo: perseguição de alvo em dash; dano único; janela de invisibilidade e vulnerabilidade mantida; adaga/recast/custo/recarga/expiração; cinco lâminas e pulsos; RNG 50% por golpe sem multiplicar colliders; perfil R02 impede defesa de um ID excluído em fixture, sem alterar skills reais. Regressores antigos das duas esquivas e Travessia/buff foram atualizados somente onde o pedido substitui a regra.

Estado: VALIDADO_EDITOR / AGUARDANDO_VALIDACAO_ANDROID. Unity 6000.6.1f1 executou atualização restrita e matriz. Sem alegar Android/toque validado.

Não executados: APK, instalação, medição FPS no Moto G54 ou aceite humano. Recuperação deve retirar apenas este diff; nunca reset amplo da árvore suja.

## Rodadas e diagnóstico

- Rodada inicial: 232 casos de combate/UI passaram, zero falhas; 48 verificações estruturais visuais passaram. Unity compilou sem erros.
- Ampliação: 236 passaram, 1 falhou (teleporte da adaga parada perto de parede); 48 verificações visuais passaram. Áreas diretas/choque mágico e parede de fase passaram.
- Causa da falha: o recuo da âncora no contato era limitado ao deslocamento positivo do frame; adaga de raio 0,175 podia parar a 0,35 m da parede, mas o personagem tem cápsula de raio 0,45. O teste de destino corretamente recusava o teleporte. Corrigido permitindo recuo até a margem segura do contato; não relaxado o teste nem atravessada a parede.
- A defesa orbital foi integrada às duas rotas de área direta, sem mudar as guardas/parries tradicionais. A adaga reutiliza identidade/trigger de projétil para a parede de fase e cancela recast se sua hitbox for cancelada. Lâminas têm mesh afilado próprio, sem arte externa.
- Validador de mapa/documentação: 12 setores/46 referências, PASS; 16 testes Node de tooling, zero falhas. `git diff --check` limitado aos scripts/documentos da tarefa passou; whitespace antigo de prefabs fora do escopo não foi corrigido.
- Apenas agente principal. Papéis descritos em AGENTES.md não foram ativados; lista de agentes em execução retornou somente /root.

## Arquivos de implementação desta tarefa

Reexecução final: **237 passaram, 0 falharam**; **48 verificações estruturais visuais passaram, 0 falharam**. Sem erro de compilação ou exceção runtime. A falha de parede foi corrigida sem alterar o resultado esperado. Log final: Logs/Assassin-Rework-20261003-verified.log; relatório caso a caso: Docs/TEST_RESULTS_LAB_001.md. Existem avisos de APIs obsoletas em WorldPickup, PrototypeLiveTests.Lab001 e PrototypeVisualValidation anteriores ao pedido; não foram editados para limpar avisos. Comando Unity: `-batchmode -projectPath <raiz> -executeMethod BattleRoyaleX.EditorTools.PrototypeDataFactory.UpdateAssassinAndRunTests -logFile <log>`, sem `-quit` para permitir concluir o PlayMode.

Novos scripts (e respectivos `.meta` gerados pela Unity):

- Assets/BattleRoyaleX/Runtime/Abilities/AbilityController.Assassin.cs
- Assets/BattleRoyaleX/Runtime/Abilities/AssassinDaggerAnchor.cs
- Assets/BattleRoyaleX/Runtime/Abilities/AssassinDaggersPresentation.cs
- Assets/BattleRoyaleX/Runtime/Data/DefenseInteractionRule.cs
- Assets/BattleRoyaleX/Runtime/Debug/PrototypeLiveTests.Assassin.cs
- Assets/BattleRoyaleX/Editor/AbilityInteractionMapping.cs

Scripts existentes ajustados:

- Runtime/Abilities/AbilityController.cs, AbilityController.V1.cs, AbilityController.Hunt.cs e SeekingProjectileMover.cs
- Runtime/Combat/DefenseController.cs e CombatResolver.cs
- Runtime/Core/BRXTypes.cs
- Runtime/Data/AbilityDefinition.cs e AbilityTechnicalInfo.cs
- Runtime/Characters/SmokeVisibility.cs, CharacterVisualAnimator.cs e PrototypeTrainingBot.cs
- Runtime/Input/PrototypeMobileTouchControls.cs
- Runtime/Debug/PrototypeLiveTests.cs e PrototypeLiveTests.MobileV4.cs
- Editor/PrototypeDataFactory.cs

Todos esses caminhos são relativos a Assets/BattleRoyaleX. Dados: GeneratedData/Abilities/Assassin_Ult_A.asset, Assassin_Move_A.asset, Assassin_Defense_B.asset; novos GeneratedData/InteractionRules/Defense_Code_1.asset e Defense_Code_2.asset, respectivos `.meta` e `.meta` da pasta. Dados das outras classes não foram regenerados.

Documentação: Docs/INTERACTION_RULES.md, Docs/ABILITY_INTERACTIONS.md, Docs/TEST_RESULTS_LAB_001.md, Docs/MAPA.md, Docs/HANDOFF.md, Docs/engenharia/mapa.json e este registro. Logs em Logs/Assassin-Rework-20261003*.log, ignorados pelo Git. Não atribuir diffs anteriores do Mago/Arqueiro a esta tarefa.
