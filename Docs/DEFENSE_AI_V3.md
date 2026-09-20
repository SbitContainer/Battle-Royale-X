# Defesa reativa e bot de duelo — V3

Pedido atual: movimentação e leitura de skills pelo bot, repulsão no contra-ataque do Guerreiro, esquiva reativa do Assassino com travessia de dano muito baixo e efeitos em todas as variações. Esta etapa implementa o pedido e substitui os números defensivos anteriores; mantém resolução central, dados por habilidade e controle contínuo.

## Regras implementadas

| Defesa | Resposta ao contato | Dano da resposta | Deslocamento |
|---|---|---|---|
| Guarda de Aço | Parry abre próximo básico manual | básico + 6 | empurra 4 m em 0,28 s |
| Parry | Janela perfeita abre próximo básico manual | básico + 6 | empurra 4,5 m em 0,28 s |
| Fortaleza | Bloqueio abre próximo básico manual | básico + 3 | empurra 3,5 m em 0,28 s |
| Esquiva Sombria | Evitar ataque físico durante a viagem redireciona para atravessar o atacante próximo | 2 por alvo por ativação | até 3,2 m; respeita paredes |
| Contra-Sombra | Parry físico perto gera contra-travessia | 2 por alvo | até 3,2 m; respeita paredes |
| Duplo Passo | Esquiva reativa com segunda direção mais rápida | 3 por alvo por ativação | até 3,6 m; respeita paredes |

Counter do Guerreiro expira em 1,15 s. Precisa acertar um básico; não causa dano ao apertar defesa. Só o primeiro golpe recebe bônus/repulsão. Guarda inimiga reduz dano conforme seus dados e reduz o empurrão a 25%; parry/iframe evitam a resposta. Repulsão é aditiva ao movimento, sem stun longo.

Assassino muda direção somente após uma defesa real contra ataque físico próximo e bloqueável (ou um parry). A viagem original conserva o registro de alvos, portanto retornar sobre o mesmo alvo não repete dano. Uma única redireção por ativação, sem renovar iframe. Contra-Sombra inicia viagem sem iframe adicional após o parry. Não teleporta, não persegue projétil distante. O dano ofensivo de movimento (24) e cada carga da ultimate (12) permanecem separados do dano defensivo.

## Bot

- Estados locais: aproximação, circulação, evasão, defesa, counter, ataque, recuperação e busca.
- Observa somente eventos públicos de início de habilidades que pode enxergar. Guarda posição/direção vistas, alcance e duração dos sinais; não lê toque futuro, energia ou cooldown inimigo.
- Atraso mínimo de 0,20 s (padrão 0,22 s), mais frequência de decisão. Não garante parry contra golpes mais rápidos que sua reação.
- Testa se a trajetória vista ameaça sua posição. Tenta defender; se indisponível, tenta sair lateralmente.
- Circula enquanto recupera ataque ou quando reconhece defesa inimiga, aproxima para usar counter, usa investida para perseguir e ultimate em abertura próxima.
- Consultas físicas evitam obstáculos locais. Não é navegação global/NavMesh: obstáculos grandes ou concavos continuam uma limitação.
- Fumaça e paredes impedem observação; pode buscar por tempo curto a última posição vista, sem acompanhar o alvo oculto.

## Apresentação

Guardas têm aro de cobertura completa e painel de escudo ligado ao tempo real de defesa. Fortaleza usa contorno mais espesso/hexagonal. Parry tem flash compacto; oportunidade de counter recebe marca dourada; acerto gera leque de impacto direcionado. Assassino usa rastro fino e selo curto na redireção, menor que o dash ofensivo.

Todas as 12 variações têm paleta própria por habilidade; retornos marcam a origem, ultimates usam desenhos de aura distintos. Os efeitos partem de fases Active reais; o VFX não repete a espera de startup e nunca causa dano. Limite compartilhado: 70 objetos de efeito. Qualidade visual, conforto e FPS no Moto G54 exigem avaliação física separada dos testes lógicos.

Resultados desta rodada: `TEST_RESULTS_DEFENSE_AI_V3.md`.
