# BRX-VISUAL-007 — apresentação baseada nas três referências do usuário

03/10/2026. Autorização: usuário respondeu "pode seguir" à proposta de usar as três imagens apenas como referência visual, sem alterar dano/alcance/duração/regras. Etapa VALIDADO_EDITOR / AGUARDANDO_VALIDACAO_ANDROID, sem autorização nova de APK/publicação/instalação.

## Esperado e proibido

Setor A: WarriorSkillPresentation + helper procedural e CombatVFXRouter. Consumidores B/C: CombatEvents, shaders Resources, câmera/capturas, cleanup ao interromper/trocar classe/morrer e matriz Unity. Fases de carga, execução e impacto devem ganhar volume/lâminas/rastros/faíscas, mantendo personagem e área de acerto legíveis. VFX não altera autoridade/timing, não desloca personagem e não destrói projétil/Hitbox ao terminar.

Proibido: dano, alcance, defesa, energy/cooldown, duração de skill, variantes, classes, bot/input/loot, dependências e scene regeneration. Nenhum JPEG preto aplicado como cartão no mapa; imagens são concept boards, não flipbooks. Nenhum asset externo baixado ou criado com aparência achatada/perspectiva fixa.

## Mapeamento de referência

| Existente | Apresentação prevista |
|---|---|
| Básico, três passos | Crescente, corte cruzado e rastro circular; não cria três skills novas |
| Arremesso/perseguições | Energia/rastro em torno do escudo e do trajeto existente; não muda avanço em ataque novo |
| Guarda | Painel curvo azul translúcido/dourado, sem ampliar bloqueio real |
| Usurpador | Meia-cúpula/energia de absorção, sem invulnerabilidade nova |
| Fortaleza | Três escudos existentes com face azul/bordas douradas; preserva forma chanfrada e saída física |
| Bastião | Carga concentrada → coluna curta de energia + fragmentos/explosão |
| Domínio | Lâminas espectrais no campo, queda visual e impactos locais por pulso |
| Ruptura | Dupla espiral durante carga e ondas; raio e três pulsos reais preservados |

Fragmentos, lâminas e cúpula não possuem colisores/dano. Timing das animações é apresentação, não prova de acerto.

## Referências e autoria

Três imagens fornecidas pelo usuário: 1000201144.jpg (ataque), 1000201143.jpg (ultimate), 1000201142.jpg (defesa). Foram usadas como direção de arte; JPEGs não são adicionados ao build. Geometria/shader/partículas originais gerados no código, sem texturas adquiridas/novo pacote/compra. Reutilizar ArcaneGlow/BRXAimPreview quando possível. Sem imagem estática recortada como se fosse volume animado.

Principal escreve integração/router/testes/documentação. Delegação delimitada a implementer, autorizada por AGENTES: novo WarriorReferenceVfx.cs/.meta e shader Resources se realmente necessário, sem subdelegação. Revisão independente após congelar.

## Evidência

Baseline: main, remoto SbitContainer/Battle-Royale-X, árvore suja preservada; release14 anterior publicada. SHA256 de todos os 136 arquivos GeneratedData registrado antes da mudança visual. Matriz será executada por PrototypeLiveTestLauncher.RunBatch, sem gerador de dados/cena. Android visual/FPS/toque NÃO EXECUTADOS.

## Resultado e arquivos desta etapa

- Primeira compilação: CS0266 no literal `.5` da altura angular da cúpula; corrigido para `.5f`. Log `Logs/visual007-matrix1.log`, preservado.
- Candidato final compilado sem erros: `Logs/visual007-matrix2.log`, **314 PASS / 0 FAIL** combate/interface e **48 PASS / 0 FAIL** visual estrutural. Cinco avisos CS0618 preexistentes, não corrigidos fora do escopo.
- Casos novos: Guarda/Usurpador usam composição e reset a remove; três passos básicos criam estilos distintos; eventos visuais não gastam energia/aplicam dano; três volumes de ultimate sem Collider/Rigidbody, com shaders suportados, até 14 renderers auxiliares e cleanup ao expirar. Helper real máximo nove renderers/96 partículas por sistema; não é orçamento ou benchmark físico.
- Os demais casos incluem dano/cadência/defesa/clash/recapture, quatro classes, mira/input simulado, variantes/inventário, cancelamento e pulsos/limites locais do Guerreiro. Lista exata de cada caso em `Docs/TEST_RESULTS_LAB_001.md`.
- **15 capturas reais inspecionadas** em `Logs/Warrior007`: fortress; cada ultimate charging/release/peak (9); duas defesas reference; três básicos. Volumes e transparência observados, personagens legíveis. Não comprova satisfação artística do usuário nem animação/FPS no celular. Lâminas espectrais caem cosmeticamente; impactos de dano continuam vindo dos eventos reais, não da queda da geometria.
- Revisão estática independente aprovada: básico não instancia cast genérico duplicado, faces Fortaleza usam material exclusivo, lifecycle/cancelamento/captured-class filters preservados. Observação cosmética não bloqueante: Charge escolhe estilo pela variante equipada, não snapshot de perseguição capturada de outro Guerreiro; não alterado fora da revisão visual atual.
- SHA256 pós-execução: **136/136 GeneratedData e 5/5 AbilityController*.cs sem alteração**. Nenhuma regeneração de dados/cena ou rebalanceamento nesta tarefa.

Arquivos escritos nesta tarefa (alterações locais anteriores não fazem parte desta lista):

1. `Assets/BattleRoyaleX/Runtime/Abilities/WarriorReferenceVfx.cs` e `.meta` — novo helper procedural e GUID gerado pela Unity.
2. `Assets/Resources/WarriorEnergy.shader` e `.meta` — novo shader URP com feather/hex/transparência e GUID Unity.
3. `Assets/BattleRoyaleX/Runtime/Abilities/WarriorSkillPresentation.cs` — integra estilos e faces azuis; conserva hosts físicos e cleanup.
4. `Assets/BattleRoyaleX/Runtime/Visual/CombatVFXRouter.cs` — três cortes básicos, contato defensivo e supressão do cast antigo.
5. `Assets/BattleRoyaleX/Runtime/Combat/CombatEventVfxPresenter.cs` — evita corte/escudo legado duplicados sem retirar sangue/dano.
6. `Assets/BattleRoyaleX/Runtime/Debug/PrototypeLiveTests.WarriorRework.cs` — cobertura adicional/capturas.
7. `Docs/TEST_RESULTS_LAB_001.md` — relatório produzido pela matriz.
8. `Docs/HANDOFF.md`, `Docs/MAPA.md`, este registro — documentação de continuidade/consumidores/evidência.

Sem novos assets externos, texturas, dependências, ProjectSettings, gameplay, outras classes, APK, publicação ou instalação. Fontes e screenshots locais; sem commit/push. **Android visual/toque/FPS NÃO EXECUTADO / AGUARDANDO_VALIDACAO**. Próximo passo: obter autorização para gerar e publicar o APK com estes efeitos, mantendo assinatura/pacote existentes, depois validação física separada.
