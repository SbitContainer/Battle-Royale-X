# BATTLE ROYALE X — PROTÓTIPO 01

## Arena Guerreiro vs Assassino

## Objetivo do documento

Definir a primeira versão jogável do Battle Royale X, focada em validar combate 1x1, leitura de habilidades, interação entre ataques e defesas, inventário curto, troca de variações no meio da luta e loot tático sem progressão vertical de poder.

# 1\. VISÃO GERAL DO PROTÓTIPO

Formato  
\- Combate 2.5D com câmera isométrica, leitura visual semelhante a um ARPG no estilo Diablo.  
\- Sem elevação jogável, vantagem de altura ou salto vertical real.  
\- Arena pequena e fechada para acelerar o encontro entre os jogadores.  
\- Primeiro confronto: Guerreiro vs Assassino.  
\- O protótipo termina quando um dos dois jogadores é eliminado.

Princípios centrais  
\- Jogabilidade e habilidade acima de equipamento e progressão.  
\- Nenhum loot aumenta permanentemente dano, vida máxima ou armadura.  
\- Ataque Básico não possui variações.  
\- Defesa, Movimento e Ultimate possuem variações.  
\- Variações podem ser encontradas, guardadas e consumidas para troca durante a luta.  
\- Toda habilidade ofensiva precisa possuir contrajogada.  
\- Nenhuma ultimate deve causar morte inevitável.  
\- Jogadores habilidosos devem prolongar o combate por esquiva, defesa, clash, counter e posicionamento — não apenas por barras de vida enormes.

# 2\. ARENA

Dimensão inicial  
\- Aproximadamente 45 x 45 metros equivalentes.  
\- Formato quadrado ou levemente retangular.

Estrutura  
\- Centro relativamente aberto para confronto direto.  
\- Quatro obstáculos médios próximos às diagonais.  
\- Quatro obstáculos menores nas laterais.  
\- Bordas com espaço para coleta de loot e reorganização rápida de inventário.  
\- Pequenos corredores formados pelos obstáculos para criar leitura de aproximação, fuga e emboscada.  
\- Nenhum obstáculo cria vantagem de altura.

Função dos espaços  
\- Centro: confronto direto e disputa do drop aéreo.  
\- Laterais: coleta rápida de consumíveis.  
\- Obstáculos: quebra de linha de visão e mind game.  
\- Bordas: momento de recuperação, cura ou troca de variação quando o jogador consegue criar distância.

Estilo visual inicial  
\- Ruínas simples.  
\- Pedras e pilares quebrados.  
\- Marcas arcanas no chão.  
\- Poucos elementos decorativos para preservar leitura clara de ataques e efeitos.

# 3\. INVENTÁRIO E MOCHILA

Mochila base  
\- 3 espaços.

Mochila avançada  
\- 4 espaços.  
\- Encontrada prioritariamente em drop aéreo.  
\- Enquanto o jogador possuir a mochila avançada, o quarto espaço permanece disponível.

Itens que podem ocupar slots  
\- Poção de Cura.  
\- Poção de Energia/Essência.  
\- Orbe de Recarga.  
\- Granada de Fumaça.  
\- Orbe de Repulsão.  
\- Cristal de Barreira.  
\- Selo Nulo.  
\- Runa de Variação.

Regras do inventário  
\- Cada item ocupa um espaço.  
\- Item consumido desaparece.  
\- Runa de Variação usada desaparece.  
\- O inventário deve permanecer simples o suficiente para ser operado durante combate sem abrir menus complexos.  
\- O jogador pode trocar ou descartar itens para reorganizar a mochila.

# 4\. VARIAÇÕES DE HABILIDADE

Regra geral  
\- Ataque Básico é fixo e nunca recebe variação.  
\- Defesa, Movimento e Ultimate possuem versão base e duas variações possíveis.  
\- A variação encontrada pode ser guardada na mochila em vez de ativada imediatamente.  
\- Ao consumir uma variação, ela substitui a versão atualmente equipada daquela habilidade.  
\- A troca pode ocorrer no meio da luta.  
\- A variação consumida desaparece após a troca.

Tempo de troca  
\- A troca não deve ser instantânea.  
\- Referência inicial: entre 0,6 e 1,0 segundo de comprometimento.  
\- O jogador deve criar uma pequena janela segura antes de trocar.  
\- Ser atingido durante a troca pode interromper a ação.

Objetivo da troca dinâmica  
\- Permitir adaptação à leitura do adversário.  
\- Criar mind game sem aumentar atributos.  
\- Fazer com que dois jogadores da mesma classe possam lutar de formas diferentes dentro da mesma partida.

# 5\. LOOT DE CHÃO

Poção de Cura  
\- Recupera aproximadamente 20% a 25% da vida.  
\- Possui tempo de uso.  
\- Uso pode ser interrompido por dano ou controle.

Poção de Energia / Essência  
\- Recupera recurso usado pelas habilidades.  
\- Uso mais rápido que a cura, porém não instantâneo.

Orbe de Recarga  
\- Reduz parcialmente cooldowns básicos.  
\- Não deve resetar completamente uma Ultimate.

Granada de Fumaça  
\- Cria uma área temporária com visibilidade reduzida.  
\- Pode servir para fuga, aproximação, troca de variação ou cura.  
\- Afeta leitura dos dois lados.

Orbe de Repulsão  
\- Item lançável.  
\- Empurra inimigos próximos do ponto de impacto.  
\- Dano mínimo ou inexistente.  
\- Serve para criar distância, interromper pressão ou reposicionar o combate.

Cristal de Barreira  
\- Item lançável ou posicionável.  
\- Cria pequena barreira temporária.  
\- Pode bloquear passagem, linha de visão e ataques compatíveis.  
\- Não concede armadura direta ao usuário.

Selo Nulo  
\- Item lançável.  
\- Cria uma pequena área temporária de anulação.  
\- Neutraliza projéteis mágicos e efeitos compatíveis.  
\- Não bloqueia ataques físicos corpo a corpo.

Runa de Variação  
\- Representa uma variação específica de Defesa, Movimento ou Ultimate.  
\- Pode ser ativada imediatamente ou guardada na mochila.  
\- Ao ser usada, substitui a variação atual e desaparece.

# 6\. DROP AÉREO

Funcionamento  
\- O local de queda é sinalizado no chão antes da chegada.  
\- O drop cria um objetivo temporário e força disputa.  
\- O drop não entrega arma com dano superior nem bônus permanente de atributos.

Conteúdo possível  
\- Mochila avançada com 4 espaços.  
\- Poção de Cura Grande.  
\- Poção de Energia Grande.  
\- Pacote de item tático.  
\- Runa de Variação selecionável.

Formato recomendado  
\- O drop apresenta três opções e o jogador escolhe uma.  
\- Após a escolha, as opções restantes desaparecem.  
\- Isso transforma o drop em escolha estratégica, não recompensa automática.

# 7\. ASSASSINO — KIT DO PROTÓTIPO

Identidade  
\- Frágil.  
\- Alto dano.  
\- Maior mobilidade das classes iniciais.  
\- Forte em esquiva, reposicionamento, leitura e punição.  
\- Deve perder rapidamente se errar mobilidade e defesa repetidamente.

Ataque Básico — Corte Rápido  
\- Sem variação.  
\- Curto alcance.  
\- Alta velocidade.  
\- Alto potencial de punição.  
\- Pode gerar Clash contra ataques físicos quando os golpes coincidem.

Defesa Base — Esquiva Sombria  
\- Esquiva curta.  
\- Pequena janela de invulnerabilidade.

Defesa Variação A — Contra-Sombra  
\- Se a esquiva acontecer no timing correto contra um ataque, ativa propriedade de contra-ataque.  
\- O próximo ataque recebe uma vantagem mecânica de punição, sem simplesmente multiplicar dano bruto.  
\- Foco: precisão e leitura.

Defesa Variação B — Duplo Passo  
\- Permite uma segunda esquiva curta.  
\- Reduz ou remove a recompensa ofensiva da versão de counter.  
\- Foco: sobrevivência e mobilidade.

Movimento Base — Passo Fantasma  
\- Dash rápido de reposicionamento.

Movimento Variação A — Travessia  
\- O dash pode atravessar o inimigo.  
\- Permite inverter lado e atacar de ângulo diferente.

Movimento Variação B — Retorno  
\- Depois do dash, existe uma curta janela para retornar ao ponto de origem.  
\- Foco em bait, engano e punição de reação antecipada.

Ultimate Base — Execução Fantasma  
\- Sequência ofensiva curta de alta ameaça.  
\- Deve continuar sendo evitável, bloqueável ou interrompível por respostas corretas.

Ultimate Variação A — Execução  
\- Mais dano.  
\- Menor mobilidade durante a execução.  
\- Alto risco se errar.

Ultimate Variação B — Caçada  
\- Dano reduzido.  
\- Mobilidade fortemente ampliada.  
\- Concede deslocamentos adicionais durante a janela da Ultimate.  
\- Foco em perseguição, reposicionamento e expressão mecânica.

# 8\. GUERREIRO — KIT DO PROTÓTIPO

Identidade  
\- Mais resistente.  
\- Menor capacidade de esquiva.  
\- Forte em bloqueio, parry, avanço e contra-ataque.  
\- Deve vencer por leitura defensiva, não apenas por possuir mais vida.

Ataque Básico — Corte Pesado  
\- Sem variação.  
\- Mais lento que o ataque do Assassino.  
\- Maior área e impacto.  
\- Bom para controlar aproximação.  
\- Pode gerar Clash quando encontra outro ataque físico no timing adequado.

Defesa Base — Guarda  
\- Bloqueio frontal confiável.  
\- Reduz dano recebido de ataques compatíveis.

Defesa Variação A — Parry  
\- Janela curta e precisa.  
\- Se o timing for correto, anula o ataque compatível e abre janela de resposta.  
\- Foco em habilidade pura e leitura.

Defesa Variação B — Fortaleza  
\- Defesa mais estável durante curto período.  
\- Menos recompensa ofensiva.  
\- Mobilidade reduzida durante o uso.  
\- Foco em consistência.

Movimento Base — Investida  
\- Avanço curto para aproximação ou reposicionamento ofensivo.

Movimento Variação A — Impacto  
\- Colidir com inimigo provoca empurrão.  
\- Dano baixo; principal função é reorganizar espaço.

Movimento Variação B — Avanço Defensivo  
\- Concede proteção frontal durante a investida.  
\- Não deve conceder invulnerabilidade total.  
\- Foco em aproximação segura.

Ultimate Base — Postura de Guerra  
\- Estado temporário que melhora simultaneamente capacidade ofensiva e defensiva sem tornar o Guerreiro inevitável.

Ultimate Variação A — Retaliação  
\- Reforça respostas após defesa perfeita, bloqueio ou parry.  
\- Foco em punir agressividade do adversário.

Ultimate Variação B — Avanço Implacável  
\- Aumenta resistência a interrupção durante pressão ofensiva.  
\- Melhora capacidade de avançar enquanto ataca.  
\- Continua vulnerável a esquiva, counter, reposicionamento e decisões corretas do adversário.

# 9\. INTERAÇÕES PRIORITÁRIAS DO PRIMEIRO TESTE

Ataque físico vs ataque físico  
\- Quando coincidem dentro da janela definida, ocorre Clash.  
\- Ambos recebem dano reduzido ou impacto reduzido.  
\- Nenhum jogador deve receber punição total como se tivesse ignorado o ataque adversário.

Assassino vs Guarda  
\- Ataque frontal previsível deve favorecer a defesa do Guerreiro.  
\- Reposicionamento, Travessia e Retorno permitem ao Assassino tentar criar outro ângulo.

Assassino vs Parry  
\- Ataque previsível pode ser completamente punido.  
\- O Assassino deve poder baitar o Parry e atacar depois da janela.

Guerreiro vs Duplo Passo  
\- O Guerreiro controla espaço, mas não deve conseguir acompanhar todas as mudanças de direção do Assassino.

Repulsão e Barreira  
\- Podem interromper pressão de ambas as classes.  
\- Servem como terceira camada de decisão além das habilidades do personagem.

# 10\. FLUXO COMPLETO DA PARTIDA DE TESTE

1\. Guerreiro e Assassino aparecem em lados opostos.  
2\. Ambos iniciam apenas com kit base e mochila de 3 espaços.  
3\. O loot de chão cria escolhas rápidas de rota.  
4\. O primeiro contato testa ataque, defesa e movimentação base.  
5\. O jogador pode recolher variações e guardá-las.  
6\. Durante o duelo, um jogador pode criar distância e consumir uma variação para alterar o matchup.  
7\. O drop aéreo cria um novo ponto de disputa.  
8\. A mochila avançada ou uma nova variação pode mudar as opções do jogador sem aumentar seu poder bruto.  
9\. A luta continua até uma eliminação.

# 11\. REGRAS INICIAIS DE BALANCEAMENTO

Equilíbrio 1x1  
\- Guerreiro e Assassino devem possuir rota real de vitória um contra o outro.  
\- O objetivo de balanceamento é manter matchups próximos o suficiente para que execução tenha mais peso que a classe escolhida.

Anti-snowball  
\- Matar, pegar loot ou chegar primeiro ao drop não concede crescimento permanente de atributos.  
\- A vantagem vem de espaço, informação, recursos consumíveis e opções táticas.

Dano  
\- Nenhum combo deve remover 100% da vida sem devolver controle ao adversário.  
\- Mesmo o Assassino precisa deixar janela de resposta entre grandes picos de dano.

Defesa  
\- Defesa não pode ser infinita.  
\- Guarda, Parry, esquiva e interações especiais devem possuir custo, cooldown ou janela vulnerável.

Cura  
\- Cura deve exigir oportunidade.  
\- O jogador pressionado não deve conseguir recuperar vida gratuitamente durante troca direta.

Troca de variação  
\- Deve ser poderosa porque altera estratégia, mas arriscada o bastante para exigir criação de espaço.

Critério de sucesso do Protótipo 01  
\- Um 1x1 deve continuar interessante mesmo repetido várias vezes.  
\- O jogador deve conseguir identificar por que venceu ou perdeu.  
\- A troca de variações deve gerar adaptação real.  
\- O loot deve gerar decisões, não simplesmente vantagem estatística.  
\- Guerreiro e Assassino precisam parecer radicalmente diferentes sem tornar o matchup decidido automaticamente pela classe.

# 12\. ITENS A CALIBRAR DURANTE OS TESTES

\- Vida base de cada classe.  
\- Dano do Ataque Básico.  
\- Velocidade de ataque.  
\- Tempo de recuperação após ataques.  
\- Distância dos dashes.  
\- Janelas de invulnerabilidade.  
\- Janela de Parry.  
\- Redução de dano da Guarda.  
\- Cooldowns de Defesa, Movimento e Ultimate.  
\- Custo de Energia/Essência.  
\- Quantidade e posição do loot.  
\- Tempo para troca de variação.  
\- Frequência do drop aéreo.  
\- Duração da fumaça, barreira e selo nulo.  
\- Força de repulsão.  
\- Tempo médio desejado de um duelo entre jogadores habilidosos.

Estado do documento: base inicial aprovada para construção do primeiro protótipo.  
