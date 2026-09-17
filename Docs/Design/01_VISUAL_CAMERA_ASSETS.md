BATTLE ROYALE X — DIREÇÃO VISUAL, CÂMERA E ASSETS  
Protótipo 01 — Guerreiro vs Assassino

1\. DIREÇÃO VISUAL GERAL

Estilo recomendado  
\- Dark fantasy estilizado, com personagens 3D e câmera 2.5D/isométrica.  
\- Não buscar realismo fotográfico no protótipo.  
\- Formas e silhuetas fortes para leitura rápida durante combate.  
\- Cenário com cores mais neutras e dessaturadas.  
\- Golpes, defesa, loot e ultimates usam cores e emissivos mais fortes.  
\- Evitar excesso de partículas, bloom e detalhes pequenos que atrapalhem a leitura.

Regra de consistência  
\- Personagens devem vir preferencialmente da mesma família visual ou serem adaptados para materiais semelhantes.  
\- Cenário deve seguir o mesmo nível de estilização.  
\- VFX podem vir de packs diferentes, mas devem ser recoloridos e redimensionados para parecer parte do mesmo jogo.

2\. ESCALA DO MUNDO E PERSONAGENS

Padrão técnico  
\- Unity: 1 unidade \= 1 metro.  
\- A escala física precisa ser consistente desde o início para hitboxes, dash, alcance e câmera.

Guerreiro  
\- Altura visual: aproximadamente 1,90 m.  
\- Silhueta larga e pesada.  
\- Ombros e armadura maiores para identificar a classe instantaneamente.  
\- Collider inicial aproximado: raio 0,40–0,45 m.  
\- Equipamento visual recomendado: espada de uma mão \+ escudo grande.  
\- Postura mais plantada no chão, centro de gravidade baixo e movimentos deliberados.  
\- Paleta inicial: aço escuro, couro, detalhes quentes em vermelho queimado/dourado.

Assassino  
\- Altura visual: aproximadamente 1,75 m.  
\- Silhueta estreita e leve.  
\- Collider inicial aproximado: raio 0,30–0,35 m.  
\- Equipamento visual recomendado: duas lâminas curtas ou adagas longas.  
\- Postura inclinada e pronta para movimento.  
\- Roupa leve, capuz opcional e poucas peças rígidas.  
\- Paleta inicial: grafite/preto, metal frio e emissivo violeta ou azul-violeta.

Leitura na tela  
\- Personagem deve ocupar aproximadamente 130–180 pixels de altura em 1080p durante gameplay normal.  
\- Guerreiro deve parecer mais largo mesmo sem observar a barra de vida.  
\- Assassino deve parecer claramente menor e mais rápido.

3\. CÂMERA DO PROTÓTIPO

Tipo  
\- Perspectiva 3D, não ortográfica.  
\- Aparência isométrica/2.5D semelhante a ARPGs.

Configuração inicial para teste  
\- Yaw: aproximadamente 45 graus.  
\- Inclinação para baixo: aproximadamente 50–55 graus.  
\- FOV inicial: 35–40 graus.  
\- Distância aproximada do personagem: 12–15 m, calibrada pela leitura em tela.  
\- Câmera não gira livremente durante o combate no primeiro protótipo.  
\- Altura e ângulo permanecem praticamente fixos.

Comportamento  
\- Movimento suave acompanhando o personagem.  
\- Pequeno look-ahead na direção do movimento para aumentar leitura da área à frente.  
\- Sem balanço forte ou câmera cinematográfica durante combate comum.  
\- Ultimate pode usar zoom muito pequeno e breve, nunca escondendo informação.  
\- Impactos fortes podem usar micro shake e hit-stop, mas com intensidade limitada.

Objetivo  
\- O jogador deve conseguir enxergar simultaneamente personagem, adversário, projéteis, áreas no chão, loot e obstáculos próximos.

4\. LINGUAGEM VISUAL DOS GOLPES

Regra principal  
\- Cada efeito deve comunicar função antes de beleza.  
\- Jogador precisa identificar imediatamente: ataque, defesa, counter, dash, área perigosa e interação bem-sucedida.

Ataque físico  
\- Trail curto acompanhando a arma.  
\- Arco de corte visível por poucos frames.  
\- Faíscas no contato.  
\- Poeira ou pequeno impacto no chão em golpes pesados.

Clash físico  
\- Flash claro no ponto de encontro.  
\- Faíscas radiais.  
\- Pequena onda circular de impacto.  
\- Hit-stop extremamente curto para dar peso.  
\- Som metálico exclusivo para diferenciar Clash de acerto normal.

Defesa / bloqueio  
\- Não usar escudo mágico gigante em toda defesa física.  
\- Priorizar faísca, impacto na arma/escudo e pequeno arco de energia para confirmar que o bloqueio funcionou.

Parry perfeito  
\- Flash mais limpo e intenso que bloqueio normal.  
\- Anel curto partindo do ponto de contato.  
\- Hit-stop ligeiramente maior que Clash.  
\- Feedback visual exclusivo e impossível de confundir com bloqueio comum.

5\. GUERREIRO — APARÊNCIA E VFX

Ataque Básico — Corte Pesado  
\- Espada com trail branco/quente e leve dourado.  
\- Arco grosso e curto.  
\- Poeira no chão em finalizações mais fortes.  
\- Animação deve vender peso antes de velocidade.

Guarda  
\- Escudo físico claramente visível.  
\- Impacto produz faíscas e pulso curto no escudo.  
\- Sem bolha permanente em volta do personagem.

Parry  
\- Flash dourado/branco no contato.  
\- Pequena expansão circular.  
\- Animação curta de desvio da arma inimiga.

Fortaleza  
\- Postura firme com escudo levantado.  
\- Runas discretas no escudo/chão.  
\- Aura baixa junto ao corpo, sem encobrir ataques inimigos.

Investida  
\- Poeira nos pés.  
\- Trail curto do escudo/corpo.  
\- Corpo inclinado para frente.

Impacto  
\- Onda curta no chão ao colidir.  
\- Pequenos fragmentos/poeira.  
\- Deve comunicar empurrão, não explosão de dano.

Avanço Defensivo  
\- Arco translúcido curto à frente do escudo.  
\- Efeito frontal, nunca uma esfera 360 graus.

Ultimate — Retaliação  
\- Aura quente/dourada contida.  
\- Escudo e arma recebem emissivo discreto.  
\- Cada defesa perfeita produz resposta visual mais forte.

Ultimate — Avanço Implacável  
\- Energia concentrada no corpo e arma.  
\- Trail pesado durante avanço.  
\- Poeira e passos mais marcados.  
\- Não usar aura gigante que esconda o modelo.

6\. ASSASSINO — APARÊNCIA E VFX

Ataque Básico — Corte Rápido  
\- Trail fino violeta/prateado.  
\- Arcos menores e rápidos que os do Guerreiro.  
\- Poucas partículas; foco em velocidade.

Esquiva Sombria  
\- Afterimage curto.  
\- Pequeno rastro de fumaça/sombra.  
\- O personagem deve permanecer rastreável visualmente.

Contra-Sombra  
\- Esquiva perfeita gera marca visual breve no Assassino ou no alvo.  
\- Próximo ataque especial recebe trail diferenciado para avisar que o counter está armado.

Duplo Passo  
\- Primeira esquiva deixa um afterimage.  
\- Segunda esquiva usa afterimage com intensidade ligeiramente diferente para indicar consumo da segunda carga.

Passo Fantasma  
\- Trail direcional curto no chão.  
\- Distorção/sombra leve no ponto de saída.

Travessia  
\- Efeito visual atravessa a silhueta do inimigo sem ocultá-lo.  
\- Pequeno risco ou corte visual no eixo da travessia.

Retorno  
\- Ao realizar o primeiro dash, fica uma marca pequena e claramente visível no chão no ponto de origem.  
\- Ao retornar, o Assassino reaparece sobre essa marca com efeito de recolhimento da sombra.

Ultimate — Execução  
\- Sequência de trails mais brilhantes.  
\- Cada golpe precisa continuar legível individualmente.  
\- Final possui impacto mais forte, mas não tela inteira branca.

Ultimate — Caçada  
\- Aura violeta discreta.  
\- Afterimages adicionais e trail de movimento mais longo.  
\- Dano visual dos golpes é deliberadamente menos pesado que na Execução para comunicar a diferença da variante.

7\. LOOT E APARÊNCIA DOS ITENS

Regra  
\- Item deve ser reconhecível sem ler texto.  
\- Ícone e objeto no chão devem compartilhar forma e cor.

Cura  
\- Frasco vermelho/rubi com brilho pulsante suave.

Energia/Essência  
\- Frasco azul/ciano ou roxo-azulado.

Orbe de Recarga  
\- Orbe claro com símbolo circular/relógio/runa giratória.

Fumaça  
\- Granada escura com faixa cinza/prateada.

Repulsão  
\- Orbe compacto com anéis externos ou setas para fora.

Cristal de Barreira  
\- Cristal azul-claro facetado.

Selo Nulo  
\- Objeto/runa escura com círculo cortado ou símbolo de cancelamento mágico.

Runa de Variação  
\- Cristal/runa de cor da classe, com símbolo indicando Defesa, Movimento ou Ultimate.  
\- Deve ser visualmente mais valiosa que consumíveis comuns.

Mochila avançada  
\- Pequeno equipamento/ícone claramente diferente de consumível.  
\- Pode usar detalhe dourado para indicar raridade funcional, sem implicar aumento de dano.

8\. O QUE PEGAR PRONTO

Pode vir praticamente pronto  
\- Modelos base de Guerreiro e Assassino.  
\- Rig humanoide.  
\- Idle, caminhada, corrida e parte dos dashes.  
\- Parte das animações de ataque e defesa.  
\- Espada, escudo, adagas e props.  
\- Ruínas, pedras, pilares e piso.  
\- Ícones e molduras de UI.  
\- Partículas-base de fogo, magia, fumaça, faísca e impacto.  
\- Sons temporários para protótipo.

Deve ser adaptado/customizado  
\- Timing de cada animação de combate.  
\- Hitboxes e hurtboxes.  
\- Cancelamento de animação.  
\- Janelas de Parry e esquiva.  
\- Clash.  
\- Hit-stop.  
\- Cores e escala dos VFX.  
\- Telegraphs no chão.  
\- Efeitos de variação.  
\- Camera shake e zoom.  
\- Aparência final das ultimates.  
\- Feedback de cooldown/interação.

Deve ser próprio do jogo  
\- Sistema de interação entre habilidades.  
\- Regras de colisão e anulação.  
\- Lógica das variações.  
\- Sistema de mochila e troca durante combate.  
\- Balanceamento.

9\. FONTES DE ASSETS PARA O PROTÓTIPO

Adobe Mixamo  
\- Uso: rig e biblioteca de animações humanoides.  
\- Mixamo é gratuito com Adobe ID e a Adobe permite uso dos personagens/animações em videogames, inclusive comerciais.  
\- Bom para idle, locomotion, esquivas e prototipagem rápida.  
\- Site: https://www.mixamo.com/

Unity Asset Store  
\- Uso: personagens, ambiente, VFX, animações, shaders e áudio.  
\- Prioridade: pacotes compatíveis com URP e Unity 6\.  
\- Exemplo atual de VFX: Stylized Vfx Fantasy Pack, compatível com URP.  
\- Site: https://assetstore.unity.com/

Fab  
\- Uso: personagens e pacotes 3D de qualidade alta; muitos incluem FBX e versões para Unity.  
\- Há packs de personagens fantasy estilizados já rigados/modulares.  
\- Site: https://www.fab.com/

Kenney  
\- Uso: UI, ícones, props e alguns cenários de protótipo.  
\- Muitos assets usam licença CC0.  
\- UI Pack possui centenas de elementos; Fantasy UI Borders também é CC0.  
\- Site: https://kenney.nl/assets

10\. ESTRATÉGIA DE AQUISIÇÃO

Fase 1 — protótipo funcional  
\- Usar personagens e animações prontas.  
\- Usar cenário modular simples.  
\- Usar VFX prontos recoloridos.  
\- Não gastar tempo criando arte exclusiva antes de validar combate.

Fase 2 — identidade  
\- Depois que Guerreiro vs Assassino estiver divertido, substituir ou modificar partes visuais genéricas.  
\- Criar VFX exclusivos para Clash, Parry, variações e ultimates.  
\- Ajustar modelos, materiais e armas para identidade própria.

Fase 3 — produção  
\- Escolher uma família definitiva de personagens e ambientes.  
\- Padronizar shaders, texturas, iluminação, UI e áudio.

11\. DECISÃO INICIAL RECOMENDADA

Engine visual: Unity \+ URP.  
Estética: dark fantasy estilizado.  
Personagens: 3D humanoides rigados.  
Animações base: Mixamo \+ packs prontos, depois ajuste de timing.  
Cenário: pacote modular estilizado.  
VFX: pack pronto como base, todos recoloridos e ajustados.  
UI: Kenney no protótipo.  
Câmera: perspectiva fixa isométrica, aproximadamente 45° de yaw e 50–55° de inclinação.

Meta: conseguir um protótipo visualmente coerente com o mínimo de produção própria, preservando trabalho customizado apenas onde ele afeta diretamente a identidade e a habilidade do combate.  
