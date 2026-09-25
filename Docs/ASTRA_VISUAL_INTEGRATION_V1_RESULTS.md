# Integração visual V1 — 25/09/2026

Estado: **PARTIAL**. Integração funcional na Unity real; não representa acabamento visual final nem validação Android.

Continuação: `VISUAL_POLISH_V2_RESULTS.md` registra a rodada posterior sem login. As pendências abaixo são o retrato da V1; encaixe de armas, barreiras e campos receberam alterações adicionais na V2.

## Implementado

- Unity 6000.6.1f1 + URP: Hub instalado como aplicativo Windows reconhecido e licença Personal existente resolvida. Geradores de dados, cena e VFX executados.
- Quatro classes com roupas da família Ranger, cores/equipamentos distintos; Mago com override de conjuração, Assassino com segunda adaga e Arqueiro com arco provisório. Rig Humanoid existente preservado.
- Partículas com textura suave e transparência URP; projéteis com apresentação própria e trilhas. Orbe/lance da Convergência com silhuetas diferentes.
- Campos mágicos com dois limites circulares no chão, dimensionados pelo raio real. Clones com projeções do corpo vestido, sem duplicar componentes de combate.
- Arena com materiais texturizados, árvores, moitas, pedras, ruínas. Colisores de combate existentes preservados; decoração não muda navegação.
- Botões provisórios Kenney no painel de laboratório e identificadores de variantes A/B/C. Painel deslocado para baixo do HUD de vida após detectar sobreposição na captura real.
- Retorno do Animator de morte para locomoção no reinício; automação de testes e capturas por classe no editor.

## Assets e licenças

Importados subconjuntos gratuitos de Medieval Village, Fantasy Props, Stylized Nature e Kenney UI. Base Characters, Fantasy Outfits e UAL 1/2 já existiam. Arquivos de licença CC0 preservados; fontes e inventário em `THIRD_PARTY_NOTICES.md`. ZIPs completos ficam fora do Git.

## Validação

- Duas execuções anteriores na Unity: 189 testes de Play Mode aprovados, zero falhas; 44 verificações de cena aprovadas, zero falhas.
- Rodada com três verificações visuais adicionais: **192 aprovados, zero falhas**, em `Logs/astra-editor.log`. Resultado individual em `TEST_RESULTS_LAB_001.md`.
- Smoke visual inicial: 19 aprovados / 1 falha (`movimento alimenta o Animator`). O teste aplicava entrada e ataque no mesmo instante, e o ataque bloqueia locomoção. Corrigida a sequência para observar caminhada antes do ataque; sem alterar combate para satisfazer o teste.
- Smoke repetido após correção: **20 aprovados, zero falhas**, incluindo morte e retorno à locomoção no reinício. Validação final da cena: **44 aprovados, zero falhas**.
- Cabeças estáticas incluídas nas projeções e layout do painel corrigidos após a rodada de 192; estas alterações visuais têm compilação/smoke/capturas separados, não uma segunda execução integral dos 192 casos.
- Diff dos assets existentes de habilidades Guerreiro/Assassino: somente novos campos/referências, sem alteração dos valores serializados anteriores.
- Testes automatizados exercitam APIs e física reais, mas não comprovam qualidade artística, responsividade ao toque ou FPS no Moto G54.
- Auditoria de `.meta`: zero grupos de GUID duplicados. `git diff --check` aprovado no código, documentação e assets próprios; avisos restantes são espaços/quebras de linha dos quatro arquivos de licença externos, preservados como distribuídos.

## Pendências explícitas

- Três pacotes da Asset Store ainda não importados: fluxo abriu login. Nenhum termo novo aceito nem pacote pago adquirido.
- Armadura pesada do Guerreiro e roupa própria do Mago não estão no subconjunto local; continuam com roupas Ranger diferenciadas.
- Espada/escudo Fantasy Props importados, mas encaixe nas mãos pendente. Animação específica de arco ainda usa aproximação; nem toda skill tem animação exclusiva.
- VFX procedurais são base funcional: ainda falta acabamento individual, especialmente partículas de atração, explosão combinada e integração dos efeitos externos.
- Sem biblioteca de áudio importada; fase de áudio não executada.
- Não foi validado o produto cartesiano de todos os loadouts, nem desempenho real de cada combinação no celular.
- Nenhum APK novo gerado/instalado nesta etapa. Não houve alterações de multiplayer, backend ou mapa BR final.

## Próxima etapa

Concluir aquisição dos efeitos gratuitos com a conta do usuário, encaixar armas/animações pendentes e revisar cada classe visualmente. Depois testar desempenho no Android antes de chamar o visual de final.

## Evidência visual local

Capturas em `Logs/astra-mage.png`, `Logs/astra-archer.png`, `Logs/astra-assassin.png` e `Logs/astra-warrior.png` (Logs não versionado). A captura do Mago mostra os três projéteis com trilhas; formas dos objetos arcanos e parte dos equipamentos continuam provisórias. Capturas não são evidência de desempenho Android.

## Arquivos alterados

Inventário exato desta entrega em `ASTRA_VISUAL_INTEGRATION_V1_FILES.txt` (507 arquivos, incluindo assets gerados e seus `.meta`). As mudanças de fonte concentram-se nos geradores de cena/personagens/VFX, apresentação dos projéteis/clones/Animator, painel de laboratório e verificadores de teste. O inventário é da integração Astra, não dos commits anteriores do Sol. Commit local solicitado por `ASTRA_TASK.md`; sem push ou instalação no celular nesta etapa.
