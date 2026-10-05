# BRX-VISUAL-009 — Assassino: sombras, fumaça e furtividade

Pedido de 04/10/2026: aplicar as seis referências visuais do Assassino, melhorar fumaça e mostrar Execução translúcida ao proprietário durante a furtividade existente.

## Contrato antes da implementação

Esperado: paleta violeta/prata/grafite, efeitos procedurais distintos de conjuração/movimento/contato, adagas com geometria legível, fumaça orgânica densa de fora e silhueta translúcida local. Fim do estado restaura materiais e sombras originais. O adversário não vê o personagem furtivo.

Proibido: alterar dano, imunidade, alcance, cooldown, energia, duração, interação, aquisição de alvo, física ou modelo do personagem. Nenhum JPEG como plano, nova dependência, nova classe, build/publicação/instalação Android nesta tarefa.

Entrada: raiz Unity confirmada, main, origin SbitContainer/Battle-Royale-X, árvore limpa. Setor A: apresentação; B: CombatEvents/OwnedAbilityEffect/controllers existentes; C: perspectiva local, troca/reset e shaders importados na Unity. Baseline SHA256 de GeneratedData e AbilityController*.cs capturado antes das alterações. Não executar gerador de dados/cena.

Um escritor por arquivo: principal em SmokeVisibility, SmokeField, router/presenter, attachment, testes e documentação. Delegação delimitada ao implementer (autorização vigente em AGENTES): apenas AssassinReferenceVfx, AssassinMist e AssassinDaggersPresentation; sem subdelegação nem troca de modelo. Revisão independente após congelamento.

As imagens são referências de composição (ativação/uso/impacto), não frames de animação. Geometria e shader originais produzidos em código, sem assets externos novos nem anexos do chat no Git. O personagem Quaternius atual permanece: estas referências não equivalem à substituição do modelo 3D.

## Verificação prevista

PrototypeLiveTestLauncher.RunBatch sem -quit, testes reais de furtividade/contato existente, material transparente/texturas/restauração, fumaça/perspectivas, limites visuais e cleanup. Capturas Editor 1280×720 inspecionadas. Android toque/FPS/aparência: NÃO EXECUTADO / AGUARDANDO_VALIDACAO.

Resultados e arquivos finais serão registrados depois da execução, com falhas iniciais preservadas.

Primeira rodada compilou e executou: 339 PASS / 1 FAIL, 48 verificações estruturais visuais passaram. Falha: acabamento fosco local em teste existente MobileV4. Hipótese inicial atribuiu a falha a smoothness .3; ajustado para zero, sem alterar o oráculo. Essa hipótese foi incompleta, como esclarecido abaixo. Capturas iniciais mostraram fumaça esparsa no primeiro instante: escape passou a emitir 96 partículas (mesmo máximo existente), maiores/densas desde o início; raio/duração/regra não mudaram. Rotação aleatória restrita ao escape para preservar fumaça tática genérica. Log inicial conservado em Logs/visual009-matrix1.log; rerun completo em visual009-matrix2.log.

Correção do diagnóstico anterior: o teste existente MobileV4 não verifica smoothness; a condição real é `_BaseColor.r < .6`. A primeira identificação acima foi incorreta. Segunda rodada também339/1 pelo mesmo caso, pois o vermelho .62 permaneceu. Leitura direta da asserção confirmou causa exata: tint local ajustado para .48/.38/.68 mantendo alpha .34, texturas, blend e regras intactos. O oráculo não foi alterado. Matte smoothness0 permanece. Log1 SHA256 c924b9e3969dccd9eee865a3b5f2737137da8a0fcd524d4c055d0e28fc424cd1; log2 e85ee16e0b55cc9a2e9185937e9bff57e02f2c7086dba959e77a1c6ed1493a39. Captura densa de fora revelou corte seco das partículas contra o chão; shader ganha fade de altura puramente visual. Terceira execução completa validará esse candidato, sem remendos de gameplay.

## Resultado final

- Candidato3 compilou: 340 PASS / 0 FAIL de combate/interface, incluindo26 casos novos; 48 PASS / 0 FAIL de estrutura visual. Log `Logs/visual009-matrix3.log`, SHA256 `4162b8a93d23f832d0e1e02d9e609e1283a22f53726bfd04959741bb55501c75`. As cinco advertências CS0618 preexistentes permanecem; não corrigidas fora do escopo.
- `node Tools/Engineering/validate.cjs`: PASS,13 setores/60 referências; `node --test Tools/Engineering/engineering.test.cjs`:16/0. `git diff --check`: sem erro de whitespace (avisos CRLF do checkout Windows).
- Casos novos: material transparente real e alpha local .34, texturas preservadas, sombra desativada enquanto furtivo, adversário não vê modelo, restauração exata dos materiais/MPB/sombras, fumaça shader novo/limite96/supressão intacta, proprietário visível e adversário oculto, cleanup sem custo/dano; nove estilos sem Collider/Hitbox e com lifetime limitado. Casos existentes continuam cobrindo dano único, imunidade defensiva, parede, recasts, pulso/probabilidade/exceções das adagas, troca/reset, quatro classes, input/UI/inventário e sangue/morte.
- Comparação SHA256 de entrada/saída:136 arquivos GeneratedData e5 AbilityController*.cs idênticos. Gitdiff dessas fontes contra cbebab6 vazio. Modelo/cena/PackageManifest/ProjectSettings não alterados.
- Quatorze PNG1280×720 finais inspecionados: `execution-owner-stealth`, `execution-enemy-hidden`, `smoke-owner`, `smoke-outside`, `five-daggers`, `reference-style-0` até `reference-style-8`. Capturas de estados Execução/fumaça/adagas reais e fixtures dos nove estilos; não são vídeo de gesto físico. Arquivadas em `Docs/engenharia/evidencias/BRX-VISUAL-009/`, sem anexos originais do chat.
- Helper até7 renderers e88 partículas configuradas; host do router limitado28; fumaça96; órbita mantém5meshes+5trails. São limites de composição, não benchmark nem promessa de FPS Android.
- Revisão independente security_release APROVADO no candidato3, sem escrita/subdelegação. Hash do shader `b459b0a711454d149f35242e877d484b0a3dddae2890ac1ebdf10a73133eec89`; SmokeVisibility `e9084377956fcf3bd687536f71a8b1b453a638f7eaf7f6d7b8541fd3a3ba3c15`. Contribuição implementer: três arquivos originais de apresentação/shader; principal: integração/perspectiva/testes/registros. Modelo observado/tokens/duração exata de papéis não medidos; nenhum claim de economia.

## Arquivos alterados nesta tarefa

1. `Assets/BattleRoyaleX/Runtime/Abilities/AssassinDaggersPresentation.cs`: geometria, trails e ocultação das lâminas, sem alterar órbita física.
2. `Assets/BattleRoyaleX/Runtime/Abilities/OwnedAbilityEffect.cs`: evita prefab de projétil antigo sobre a adaga já apresentada.
3. `Assets/BattleRoyaleX/Runtime/Characters/SmokeVisibility.cs`: transparência local e restauração.
4. `Assets/BattleRoyaleX/Runtime/Combat/CombatEventVfxPresenter.cs`: evita casts antigos duplicados, conserva sangue/morte.
5. `Assets/BattleRoyaleX/Runtime/Visual/CombatVFXRouter.cs`: fases/contatos e marcador Retorno.
6. `Assets/BattleRoyaleX/Runtime/World/SmokeField.cs`: apresentação exclusiva da fumaça de escape, regras originais intactas.
7. `Assets/BattleRoyaleX/Runtime/Visual/AssassinReferenceVfx.cs` e `.meta` Unity novos: helper procedural.
8. `Assets/BattleRoyaleX/Runtime/Visual/AssassinVisualAttachment.cs` e `.meta` novos: follow/visibilidade sem mover o ator.
9. `Assets/Resources/AssassinMist.shader` e `.meta` novos: ruído procedural/fade de chão URP.
10. `Assets/BattleRoyaleX/Runtime/Debug/PrototypeLiveTests.AssassinVisual.cs` e `.meta` novos, `PrototypeLiveTests.cs`: testes/capturas e entrada da suite.
11. `Docs/HANDOFF.md`, `Docs/MAPA.md`, `Docs/engenharia/mapa.json`, `Docs/TEST_RESULTS_LAB_001.md`, este registro e14 PNG de evidências: continuidade, mapa e resultados sanitizados.

Metas anteriores preservados. Nenhum asset externo/dependência nova; código procedural original, referências apenas inspecionadas. Sem APK, publicação ou instalação. **Android visual/toque/FPS NÃO EXECUTADO / AGUARDANDO_VALIDACAO**. Registro no Git e envio ao origin autorizados; hash/resultado final confirmados na entrega e no histórico. Recuperação: revert normal apenas do commit desta tarefa, sem reset destrutivo ou force push; não desfazer a release15 anterior.
