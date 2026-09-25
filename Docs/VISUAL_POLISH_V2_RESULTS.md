# Acabamento visual V2 — 25/09/2026

Escopo autorizado: acabamento usando apenas recursos locais, sem login, aquisição de pacotes, novos termos ou alterações de balanceamento.

## Mudanças

- Guerreiro: espada Bronze e escudo Wooden do Fantasy Props já licenciado, com materiais texturizados e encaixes nos ossos das mãos. Nenhum collider adicionado às armas visuais.
- Assassino: duas lâminas menores derivadas do mesmo modelo licenciado, em lugar de cubos. Mago: orbe emissivo no cajado, sem point light.
- Campos mágicos: runas no perímetro e partículas em espiral para o centro. Máximo de 32 partículas por emissor, buffer reutilizado e sem luz dinâmica.
- Convergência: onda de choque ciano emitida somente no evento de combinação bem-sucedida, crescendo até o raio da explosão de gameplay. Não altera dano nem colisões.
- Demais áreas ativas passam a desenhar seus círculos no chão e no raio configurado, em vez de usar escala genérica na altura do personagem.
- Barreiras arcanas: shader URP próprio, translúcido, com bordas, linhas móveis e runas. Runa de velocidade deixa de ser um disco opaco preenchido. Triggers originais preservados.
- Luz direcional mais quente, com sombras suaves. Removidas luzes pontuais individuais dos pickups; seus materiais emissivos continuam.
- Textos dos pickups aparecem somente quando um combatente está a menos de 6 metros, sem alterar distância/regra de coleta.
- Perfis URP PC/Mobile com MSAA 2×; render scale Mobile mantido em 0,8. Custo de GPU e resultado final no celular continuam não medidos.
- Marcador de airdrop mais baixo/fino e letreiro menor; a captura revelou que o texto alto podia cobrir a luta ao surgir perto da câmera. Tempo de queda, sorteio e coleta preservados.

## Validação

- Compilação real Unity 6000.6.1f1: zero erros C#; continuam avisos preexistentes de APIs obsoletas.
- Matriz completa automatizada: **194 aprovados, zero falhas** em `TEST_RESULTS_LAB_001.md`. Inclui os dois novos testes de orçamento de partículas e onda da Convergência sem collider, no raio real.
- Auditoria de `.meta`: zero grupos de GUID duplicados.
- Smoke de animação/runtime: **20 aprovados, zero falhas**, incluindo pés no chão, Animator, VFX de contato, morte e reinício.
- Smoke repetido depois do ajuste MSAA: **20/20**. A rodada integral de 194 ocorreu antes dos ajustes finais de MSAA e tamanho do marcador de airdrop; não é medição de FPS.
- Compilação e smoke repetidos com o marcador final: zero erros C# e **20/20**. Logs locais em `Logs/astra-editor.log` e `Logs/polish-smoke.txt`.
- Dados de habilidades: nenhuma alteração nesta rodada. Revisão com skill de controle do computador permitiu resolver o aviso de recarga da cena e prosseguir na Unity real, sem acessar conta.

- Validação final da cena: **48 aprovados, zero falhas**. Inclui shader URP sem erros, triggers preservados, modelos licenciados presentes e dimensões das armas abaixo do limite humano.

Defeito encontrado na primeira inspeção visual: o maior eixo dos FBXs não era Y, produzindo armas enormes. Corrigida normalização pelo maior eixo e orientação para o encaixe; acrescentado teste de dimensões em todos os eixos. Primeira validação de cena antes dessa correção: 47 aprovados, mas ainda sem teste de escala. Isso não foi tratado como prova de qualidade visual.

## Continua pendente

- Magic Effects FREE, Magic Circle URP e Free Stylized URP Shaders: não acessados/adquiridos nesta rodada, conforme orientação do usuário.
- Animação específica de arco, roupas finais distintas de Mago/Guerreiro e áudio. Não foram inventados clipes nem importados pacotes pagos.
- Revisão estética humana e performance no Moto G54. Nenhum APK solicitado/gerado/instalado nesta rodada.
- O resultado continua sendo acabamento incremental do laboratório, não qualidade artística final de lançamento.

## Capturas locais

`Logs/polish-v2-warrior.png`, `Logs/polish-v2-assassin.png` e `Logs/polish-v2-mage-field.png`. Conferidas no editor; pasta Logs não versionada. Capturas iniciais de armas fora de escala foram substituídas por novas capturas após correção.

## Entrega

Lista exata dos 169 arquivos desta rodada em `VISUAL_POLISH_V2_FILES.txt`, incluindo assets gerados e `.meta`. Código, shader e documentação passam em `git diff --check`; YAML gerado mantém a formatação nativa da Unity. Commit local, sem push. Pendências que exigem login ficam para a próxima sessão com o usuário.
