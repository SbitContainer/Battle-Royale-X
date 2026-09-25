# Battle Royale X — Game Direction Roadmap

> Documento vivo de direção do projeto.
>
> Este arquivo registra **para onde o jogo está indo**, quais decisões já foram aprovadas, o que ainda está em validação e quais sistemas devem ser desenvolvidos depois.
>
> Ele NÃO substitui `CODEX_TASK.md`.
>
> - `CODEX_TASK.md` = tarefa operacional atual para o Codex.
> - `Docs/GAME_DIRECTION_ROADMAP.md` = visão de produto e evolução futura.
> - `Docs/COMBAT_PRINCIPLES.md` = regras fundamentais de combate.
> - `Docs/CLASS_ROSTER_AND_VARIATIONS.md` = identidade e variações das classes.
> - `Docs/MATCHUP_MATRIX.md` = relações mecânicas entre ferramentas e matchups.

---

# 1. VISÃO DO JOGO

Battle Royale X será um Battle Royale/ARPG competitivo 2.5D com câmera isométrica, combate rápido e foco em:

- habilidade mecânica;
- leitura do adversário;
- timing;
- posicionamento;
- defesa;
- esquiva;
- clash;
- counter;
- gerenciamento de cooldown;
- escolha de variações de habilidade;
- loot que muda opções táticas sem criar crescimento estatístico exagerado.

A proposta central é evitar que uma vantagem obtida cedo transforme a partida em vitória automática.

O jogador deve ganhar principalmente por:

- execução;
- conhecimento das interações;
- adaptação;
- escolha de kit;
- leitura de cooldown;
- uso do mapa;
- capacidade de aproveitar o loot encontrado.

---

# 2. REGRA DE OURO DO COMBATE

## Habilidade acima de poder bruto

Loot, raridade e sorte podem dar vantagem.

Essa vantagem nunca deve eliminar a necessidade de habilidade.

Não usar como eixo principal:

- aumento massivo de dano;
- aumento massivo de vida;
- armadura permanente;
- níveis durante a partida;
- equipamento que torne o personagem matematicamente dominante.

A diferença entre um jogador com loot comum e outro com loot raro deve continuar permitindo vitória por execução superior.

---

# 3. SEM HARD CC

O jogo não deve depender de:

- stun longo;
- root prolongado;
- silêncio longo;
- knock-up que remove controle;
- cadeia de CC;
- ultimate inevitável.

São permitidos:

- knockback;
- deslocamento;
- micro-impacto;
- interrupção curta de ataque;
- SkillLock extremamente curto;
- barreiras;
- zonas;
- negação de projétil;
- alteração de rota;
- pressão espacial.

Regra:

> Controle deve alterar espaço, timing ou opções — não retirar do jogador a capacidade de jogar.

---

# 4. CLASSES INICIAIS

Classes planejadas:

1. Guerreiro
2. Assassino
3. Mago
4. Arqueiro

## Implementação atual

Desenvolver agora:

- Guerreiro
- Assassino

Somente registrar e usar como referência de balanceamento:

- Mago
- Arqueiro

Não implementar Mago/Arqueiro até Guerreiro x Assassino fornecer uma base de combate confiável.

---

# 5. IDENTIDADE DAS CLASSES

## Guerreiro

Forças:
- troca frontal;
- bloqueio;
- impacto;
- parry;
- punição;
- resistência.

Fraquezas:
- mobilidade;
- alcance;
- cooldowns mais altos;
- ataques mais previsíveis.

Princípio:
- consegue punir quem entra mal;
- não deve conseguir perseguir indefinidamente quem decidiu fugir.

## Assassino

Forças:
- maior velocidade;
- entrada e saída;
- esquiva;
- mobilidade;
- burst;
- mind game.

Fraquezas:
- resistência;
- combate frontal prolongado;
- erro de esquiva;
- entrar sem leitura.

Princípio:
- controla melhor quando a luta começa e termina;
- ao entrar no alcance perigoso do Guerreiro, precisa correr risco real.

## Mago

Forças:
- controle de área;
- projéteis;
- previsão;
- colisão de magia;
- negação de rota.

Fraquezas:
- pressão corpo a corpo;
- cooldown;
- dependência de antecipação;
- mobilidade apenas moderada.

## Arqueiro

Forças:
- alcance;
- precisão;
- pressão contínua;
- kite;
- mudança de ângulo.

Fraquezas:
- proximidade;
- resistência;
- defesa limitada;
- posicionamento ruim.

---

# 6. ARQUITETURA DE HABILIDADES — PRIMEIRA VERSÃO

## Estrutura do personagem

Cada classe possui:

- **Ataque Básico** fixo;
- **Skill 1**;
- **Skill 2**;
- **Ultimate**.

Na primeira versão existem somente **2 slots normais de skill + 1 slot exclusivo de Ultimate**.

Não implementar um terceiro slot normal agora.

## Início da partida

O personagem começa somente com:
- Ataque Básico.

No início:
- Skill 1 vazia;
- Skill 2 vazia;
- Ultimate vazia.

As habilidades são encontradas durante a partida.

Isso transforma a construção do kit em parte central do Battle Royale.

## Slots normais não são categorias rígidas

Skill 1 e Skill 2 não significam obrigatoriamente:
- Defesa;
- Movimento;
- Ataque.

Uma classe pode montar combinações como:
- duas mobilidades;
- mobilidade + defesa;
- ataque + mobilidade;
- controle + ataque;
- duas defesas;
- outras combinações válidas da própria classe.

A Ultimate permanece separada e somente Ultimates podem ocupar o slot Ultimate.

## Drops adaptativos de Skill 1 e Skill 2

Os itens normais de habilidade são **adaptativos à classe atual**.

Exemplo:
- item `Skill 1 — Variação B`;
- se Mago coleta, recebe a Skill 1-B do Mago;
- se Guerreiro coleta, recebe a Skill 1-B do Guerreiro;
- se Assassino coleta, recebe a Skill 1-B do Assassino;
- se Arqueiro coleta, recebe a Skill 1-B do Arqueiro.

O mesmo princípio vale para Skill 2 e suas variações.

Portanto o item físico pode ser o mesmo sistema de loot para todas as classes, mas resolve para a habilidade correspondente da classe que o coleta.

## Ultimate é específica

Ultimate é exceção.

Para preencher/trocar o slot Ultimate:
- o jogador precisa encontrar uma Ultimate válida da própria classe;
- Ultimate não é convertida automaticamente entre classes como Skill 1/Skill 2.

Isso mantém Ultimates como loot mais específico e valioso.

## Princípio

> Ataque Básico define a identidade mínima da classe; o loot encontrado define a build daquela partida.

---

# 7. FAMÍLIAS INICIAIS DE KIT

Meta inicial:

- kit Base generalista;
- 3 famílias de variações por classe.

Isso é suficiente para lançamento/protótipo inicial sem criar uma quantidade impossível de balancear.

## Assassino

### Caçador
Foco:
- mobilidade;
- perseguição;
- manter contato.

### Executor
Foco:
- dano;
- punição;
- alto risco / alto retorno.

### Fantasma
Foco:
- furtividade;
- afterimage;
- engano;
- retorno;
- reposicionamento.

## Guerreiro

### Caçador
Foco:
- anti-mobilidade;
- retaliação;
- perseguição curta.

### Bastião
Foco:
- defesa;
- projéteis;
- aproximação protegida.

### Demolidor
Foco:
- impacto;
- dano;
- knockback;
- ataques pesados;
- cooldown maior.

## Mago

### Controlador
Foco:
- área;
- pressão;
- negação de espaço.

### Destruidor
Foco:
- dano concentrado;
- telegraph;
- alto risco.

### Ilusionista
Foco:
- engano;
- retorno;
- reflexão;
- leitura difícil.

## Arqueiro

### Precisão
Foco:
- longa distância;
- tiros de alto valor;
- mira.

### Escaramuçador
Foco:
- kite;
- mobilidade;
- cadência.

### Sobrevivente
Foco:
- anti-dive;
- reposicionamento;
- escape.

---

# 8. LABORATÓRIO DE COMBATE

Antes de transformar as habilidades em loot definitivo, o laboratório pode liberar troca direta para testes.

No laboratório:
- bot/adversário permanece Guerreiro enquanto essa fase estiver ativa;
- jogador pode testar as classes já implementadas;
- Skill 1 pode ser trocada entre versões disponíveis;
- Skill 2 pode ser trocada entre versões disponíveis;
- Ultimate pode ser trocada entre Ultimates da própria classe;
- nenhum item precisa ser consumido durante o teste.

Objetivo:

> validar interação, matchup, dano, mobilidade e cooldown antes de prender a habilidade à economia de loot.

O laboratório é ferramenta de teste e não representa o início real de uma partida BR, onde o personagem começa apenas com Ataque Básico.

---

# 9. LOOT FUTURO

A maior parte do loot de chão deve ser composta por:

## Skill 1 / Skill 2 adaptativas
Itens universais que resolvem para a habilidade/variação correspondente da classe do jogador que coleta.

## Ultimates específicas de classe
Precisam corresponder à classe atual do personagem.

## Cura
Regeneração durante combate.

## Energia / Essência
Recuperação de recurso.

## Redução de cooldown
Consumível tático.

## Itens de espaço/utilidade
Exemplos:
- parede;
- fumaça;
- repulsão;
- campo nulo;
- outros itens táticos futuros.

Evitar inventário baseado em dezenas de peças de equipamento com atributos.

---

# 10. SKILLS COMO "ARMAS" DO BATTLE ROYALE

Skill 1, Skill 2 e Ultimates cumprem o papel que armas diferentes cumprem em Battle Royales tradicionais.

O jogador:
- começa somente com Ataque Básico;
- encontra habilidades;
- escolhe o que equipar;
- troca uma habilidade por outra;
- adapta sua build ao matchup e ao loot encontrado.

Os drops normais de Skill 1/Skill 2 são adaptativos à classe.

Ultimates continuam específicas.

Uma partida pode começar com uma intenção de build e terminar com outra.

Isso gera:
- adaptação;
- improviso;
- risco;
- decisões;
- partidas diferentes.

---

# 11. RARIDADE DAS HABILIDADES

Pode existir raridade real.

Porém:

> raridade melhora **mecânica**, não dano bruto.

## Teto de vantagem

A versão mais rara não deve ser mais do que aproximadamente **20% melhor em valor mecânico total** que a referência comum.

Esse teto é uma regra de direção, não uma fórmula rígida única.

Exemplos de melhoria permitida:

- alcance levemente maior;
- recovery menor;
- cooldown levemente menor;
- janela um pouco mais confortável;
- duração levemente maior;
- melhor controle de direção;
- pequena função adicional;
- reposicionamento mais eficiente;
- menor risco operacional.

Não usar raridade para:

- +40% dano;
- +50% vida;
- imunidade;
- stun adicional;
- ultimate inevitável.

## Referência de escala

Comum:
- aproximadamente 100% de valor mecânico.

Rara:
- aproximadamente 105–110%.

Épica:
- aproximadamente 110–115%.

Excepcional:
- teto próximo de 120%.

A vantagem pode ser distribuída entre várias propriedades pequenas.

---

# 12. NÃO EXISTEM "SKILLS LIXO"

Uma habilidade pode ser:

- generalista;
- especializada;
- difícil;
- situacional;
- alto risco / alto retorno;
- utilitária.

Mas não deve existir habilidade cuja conclusão seja:

> "nunca existe motivo para usar isso."

O pior caso aceitável é:

> "não é a melhor habilidade para este matchup ou situação."

---

# 13. ZONA DE CONFRONTO

Direção aprovada para substituir o conceito tradicional de airdrop.

Nome provisório:
> **Zona de Confronto**

Uma pequena região do mapa fica destacada como área de combate de alto valor.

Dentro dela:
- eliminações possuem aproximadamente 50% de chance inicial de gerar Drop de Combate;
- o loot cai fisicamente no chão;
- pode ser disputado ou roubado;
- o drop de habilidade é adaptativo à classe de quem o coleta/equipa.

Objetivo:

> quem quer acelerar a construção do kit precisa se expor a combate.

A Zona de Confronto muda de posição durante a partida.

---

# 14. DROP DE COMBATE

A chance aproximada de 50% refere-se inicialmente à chance de uma eliminação dentro da Zona de Confronto gerar um Drop de Combate.

O Drop pode conter:
- Skill 1/Skill 2 adaptativa;
- cura especial;
- redução de cooldown;
- tático;
- outro recurso aprovado no sistema de loot.

Quando for uma Skill normal:
- o item resolve para a classe de quem o coleta;
- não gera uma habilidade inútil de outra classe.

Ultimates seguem regra separada e continuam específicas de classe.

A distribuição final será calibrada em testes.

---

# 15. DROP ADAPTATIVO DE CLASSE

A regra antiga de habilidade aleatória de outra classe foi substituída.

Para Skill 1 e Skill 2:
- o item é universal;
- ao ser coletado, entrega a versão correspondente da classe atual do jogador.

Exemplo:
- um drop `Skill 2 — Variação B` é disputado no chão;
- Mago coleta → recebe Skill 2-B do Mago;
- Guerreiro rouba antes → recebe Skill 2-B do Guerreiro.

Isso mantém disputa pelo loot sem gerar drops mortos para a classe vencedora.

Ultimate continua sendo específica e precisa ser encontrada corretamente.

---

# 16. MOVIMENTO DA ZONA DE CONFRONTO

A Zona de Confronto não deve necessariamente permanecer no mesmo lugar durante a partida.

Direção preferida:

1. zona ativa em um ponto;
2. permanece por um período;
3. aviso de encerramento/mudança;
4. nova zona surge em outra região.

Objetivos:

- movimentar jogadores;
- impedir domínio permanente de um ponto;
- criar novos confrontos;
- alterar rotas da partida.

Tempo, tamanho e frequência serão definidos em testes futuros.

---

# 17. ANTI-SNOWBALL DA ZONA

A Zona de Confronto não pode transformar o primeiro time vencedor em um time impossível de remover.

Proteções conceituais:

- habilidade rara não aumenta dano bruto significativamente;
- slots continuam limitados;
- encontrar nova habilidade exige substituir/guardar uma opção;
- cooldown continua sendo custo;
- loot cai no chão, não diretamente no inventário;
- disputa pelo drop continua depois da eliminação;
- raridade é limitada pelo teto mecânico;
- habilidades especializadas têm fraquezas situacionais.

---

# 18. AIRDROP

Direção atual:

- airdrop tradicional não é mais necessário como sistema principal;
- Zona de Confronto pode assumir o papel de fonte de loot especial;
- manter o código atual de airdrop somente enquanto for útil ao protótipo;
- remover/substituir definitivamente apenas quando a Zona de Confronto estiver implementada e validada.

Não gastar tempo agora apagando o sistema antigo se ele ainda ajuda nos testes.

---

# 19. CURA

Direção atual:

- cura deve funcionar durante combate;
- regeneração progressiva;
- jogador continua podendo se movimentar;
- dano recebido não cancela necessariamente a regeneração;
- cura não deve ser instantânea;
- múltiplas curas não devem empilhar de forma abusiva.

Referência atual de laboratório:
- 30 HP;
- ao longo de 5 segundos.

Valores finais dependem de TTK real.

---

# 20. VELOCIDADE E MOBILIDADE

Princípio:

> Assassino deve conseguir abrir distância do Guerreiro apenas se ambos estiverem correndo normalmente.

Guerreiro:
- mais lento;
- ataques mais pesados;
- cooldowns maiores.

Assassino:
- mais rápido;
- cooldowns menores;
- maior controle de entrada/saída.

Guerreiro pode possuir ferramenta especializada de perseguição.

Essa ferramenta:
- possui cooldown alto;
- não garante contato;
- pode ser esquivada;
- não elimina a vantagem natural de velocidade do Assassino.

---

# 21. ATAQUE BÁSICO, SKILLS E INTERAÇÕES

## Controle

- Ataque Básico não executa simultaneamente com skill.
- Skills possuem prioridade sobre Ataque Básico.
- Se uma skill for ativada durante o Ataque Básico, o Ataque Básico é cancelado.
- A skill responde no mesmo input.

## Lei de risco x acerto

A facilidade de acertar determina a recompensa mecânica.

Direção:
- habilidade perseguidora / homing → mais fácil de acertar → dano menor;
- projétil rápido → dificuldade média → dano médio;
- projétil lento / muito telegráfico → difícil de acertar → dano alto;
- combinação que exige dois timings corretos → pode gerar recompensa muito alta;
- controle/área fácil de aplicar → dano direto baixo.

Não balancear apenas por classe. Balancear pela dificuldade real de execução e pela quantidade de contrajogo.

## Mobilidade

Todas as classes devem possuir acesso a jogadas de mobilidade.

A mobilidade pode variar em:
- quantidade;
- distância;
- cooldown;
- previsibilidade;
- flexibilidade.

Assassino continua sendo a referência de maior mobilidade natural.

## Interação do Guerreiro com projéteis

Guerreiro deve possuir forte expressão mecânica contra ataques de Mago/Arqueiro.

Alguns projéteis serão marcados como interceptáveis.

Se o Guerreiro acertar no timing correto um projétil interceptável com Ataque Básico ou habilidade compatível:
- o efeito pode ser parcialmente anulado;
- referência inicial: Guerreiro recebe aproximadamente **60% menos dano** daquele projétil;
- algumas habilidades específicas podem ser destruídas por completo;
- outras não serão interceptáveis.

Não permitir que o Guerreiro anule toda magia automaticamente.

A defesa vem de timing e leitura.

## Assassino

Assassino não precisa depender de interceptação de projéteis como identidade principal.

Sua resposta predominante é:
- mobilidade;
- dodge;
- mudança de direção;
- sair da trajetória.

Isso preserva identidades diferentes:
- Guerreiro enfrenta/intercepta;
- Assassino evita.

---

# 22. FEEDBACK VISUAL FUTURO

O visual atual é provisório.

Melhorias planejadas:

- cortes mais legíveis;
- trails;
- flashes de impacto;
- explosões;
- poeira;
- hit reaction;
- camera shake discreto;
- hit-stop curto;
- VFX distintos por variação;
- VFX distintos por classe;
- melhor telegraph;
- efeitos de Clash;
- efeitos de Parry;
- efeitos de Block;
- sangue estilizado.

---

# 23. SANGUE

Direção visual:

- estilizado;
- não realista;
- pequena explosão/spray;
- fumaça vermelha;
- dissolve rapidamente;
- serve como confirmação de dano.

Referência:
- spray curto no contato;
- nuvem vermelha escura;
- duração aproximada de 0,3–0,5 s;
- combinar com reação corporal do personagem.

---

# 24. REAÇÃO A DANO

Personagem atingido deve reagir visualmente.

Mas:

- reação não significa hard stun;
- movimento pode continuar quando a mecânica permitir;
- animação pode deslocar tronco/ombro;
- hit reaction deve vender impacto sem destruir responsividade.

---

# 25. BARRAS DE VIDA

Adicionar:

- barra de vida legível;
- redução visual conforme dano;
- aumento progressivo durante regeneração;
- ligação com `HealthComponent.Changed`;
- lógica de gameplay permanece fora da UI.

No futuro:
- avaliar HUD fixa;
- avaliar barra sobre personagem;
- avaliar uso simultâneo dos dois formatos.

---

# 26. ITENS E RUNAS — LEGIBILIDADE

Todo loot deve explicar rapidamente:

- nome;
- classe;
- slot;
- comportamento;
- o que substitui;
- raridade;
- efeito principal.

Exemplo:

**Contra-Sombra**
- Classe: Assassino
- Slot: Defesa
- Esquiva perfeita prepara contra-ataque.
- Substitui: Esquiva Sombria.

Evitar depender de nomes abstratos sem descrição.

---

# 27. CONTEÚDO PÓS-LANÇAMENTO

A estrutura de variações permite adicionar conteúdo sem precisar lançar nova classe constantemente.

Atualizações futuras podem adicionar:

- nova Defesa;
- novo Movimento;
- nova Ultimate;
- novo item tático;
- nova habilidade rara;
- nova combinação;
- nova Zona de Confronto;
- novos elementos de mapa.

Isso permite evoluir o meta gradualmente.

Regra:

> adicionar variedade sem invalidar o conteúdo anterior.

---

# 28. ORDEM GERAL DE DESENVOLVIMENTO

## Fase A — Laboratório Guerreiro x Assassino

- corrigir movimento;
- locks;
- ataque x skill;
- cura;
- life bars;
- troca livre de variações;
- validar velocidade;
- validar defesa;
- validar dodge;
- validar Clash;
- validar TTK.

## Fase B — Kits V2 de Guerreiro e Assassino

- criar famílias;
- implementar Base + primeiras variações;
- testar híbridos;
- ajustar cooldown;
- ajustar alcance;
- ajustar risco/recompensa.

## Fase C — Feedback visual

- VFX;
- sangue estilizado;
- hit reaction;
- trails;
- impacto;
- explosões;
- telegraphs;
- áudio provisório melhor.

## Fase D — Loot de habilidades

- transformar variações aprovadas em loot;
- equipar/trocar/guardar;
- raridade;
- UI de descrição;
- testes de RNG.

## Fase E — Zona de Confronto

- substituir progressivamente a função do airdrop;
- detectar kills dentro da zona;
- 50% de chance inicial de Drop de Combate;
- loot físico disputável;
- movimentação da zona.

## Fase F — Mago e Arqueiro

- implementar uma classe por vez;
- usar Guerreiro/Assassino como referência;
- validar todos os matchups;
- revisar habilidades especializadas.

## Fase G — Battle Royale completo

Somente depois do combate ser comprovadamente divertido:

- multiplayer;
- networking;
- mapa maior;
- círculo/zona de partida;
- matchmaking;
- squads/solo conforme decisão futura;
- backend;
- progressão externa;
- ranking;
- monetização cosmética se aplicável.

---

# 29. REGRA PARA NOVAS IDEIAS

Quando surgir uma nova ideia:

1. registrar neste documento;
2. classificar como:
   - DIREÇÃO APROVADA;
   - EM TESTE;
   - IDEIA FUTURA;
   - DESCARTADA;
3. não mandar implementar automaticamente;
4. primeiro verificar impacto em:
   - skill expression;
   - matchup;
   - snowball;
   - clareza visual;
   - complexidade;
   - performance;
   - multiplayer futuro;
5. só então transformar em tarefa no `CODEX_TASK.md`.

---

# 30. ESTADO ATUAL

## DIREÇÃO APROVADA

- combate 2.5D/isométrico;
- sem hard CC;
- skill acima de progressão;
- Guerreiro + Assassino primeiro;
- Mago + Arqueiro registrados;
- variações misturáveis;
- Base + famílias de kits;
- loot de habilidades como progressão horizontal;
- raridade focada em mecânica;
- teto aproximado de 20% de ganho mecânico;
- nenhuma raridade baseada principalmente em aumento de dano;
- Zona de Confronto como substituta conceitual do airdrop;
- kills dentro da zona podem gerar loot especial;
- loot especial fica no chão e pode ser disputado;
- cura contínua;
- Assassino naturalmente mais rápido que Guerreiro;
- feedback visual precisa melhorar significativamente;
- sangue estilizado como fumaça/spray vermelho.

## EM TESTE

- valores de HP;
- dano;
- cooldown;
- alcance;
- velocidade;
- duração de cura;
- frequência de loot;
- chance de 50% da Zona de Confronto;
- tempo/tamanho da Zona;
- números de raridade;
- TTK;
- quantidade final de variações no lançamento.

## IDEIA FUTURA

- novas famílias de habilidade;
- temporadas/atualizações;
- expansão do loot;
- novas classes;
- mapa Battle Royale completo;
- multiplayer;
- ranking;
- progressão externa;
- cosméticos.

---

# 31. PRINCÍPIO FINAL

O Battle Royale X deve fazer o jogador pensar:

> "Eu encontrei opções diferentes e consegui montar uma estratégia melhor."

e não:

> "Eu ganhei porque encontrei um item que tinha números impossíveis de enfrentar."

Sorte cria oportunidade.

Habilidade transforma oportunidade em vitória.


---

# 32. TERRENO ARCANO REATIVO

## DIREÇÃO APROVADA

O mapa do Battle Royale X deve possuir elementos mágicos que **interagem diretamente com habilidades**.

O cenário não deve servir apenas como cobertura.

Objetivo:

> o jogador aprende o mapa da mesma forma que aprende uma classe.

Posicionamento perto de determinados elementos pode mudar completamente uma troca.

As interações precisam ser:
- previsíveis;
- visualmente claras;
- utilizáveis por qualquer jogador;
- baseadas em habilidade;
- sem kills inevitáveis;
- sem loops infinitos.

---

## 32.1 PAREDE DE FASE

Parede mágica atravessável por personagens, mas não por habilidades/projéteis compatíveis.

Regras iniciais:
- personagens atravessam normalmente;
- projéteis mágicos param;
- projéteis físicos compatíveis também podem ser bloqueados conforme material final;
- ataques corpo a corpo não são afetados se o personagem já atravessou;
- a parede precisa ser translúcida e indicar claramente que é diferente de parede sólida.

Uso tático:
- Assassino pode atravessar para quebrar linha de ataque;
- Guerreiro pode atravessar para forçar combate próximo;
- Mago/Arqueiro não conseguem simplesmente disparar através dela.

---

## 32.2 PAREDE PRISMÁTICA / ESPELHO ARCANO

Parede que reflete projéteis mágicos.

Regras iniciais:
- reflexão usa ângulo de incidência;
- apenas magia compatível é refletida;
- dano não aumenta pela reflexão;
- projétil refletido continua legível;
- limitar quantidade de reflexões por projétil.

Referência inicial:
- máximo de 1 reflexão por projétil no protótipo.

Objetivo:
permitir bank shots e jogadas de previsão sem criar ricochetes infinitos.

Arqueiro físico não recebe automaticamente o mesmo benefício; interações específicas podem existir futuramente.

---

## 32.3 NÚCLEO / CRISTAL DE FRAGMENTAÇÃO

Objeto mágico que reage quando atingido.

Ao receber uma habilidade compatível:
- gera explosão curta;
- cria fragmentos em várias direções;
- fragmentos causam dano reduzido;
- fragmentos podem atingir inimigos e, se necessário, o próprio jogador conforme regra futura.

Referência inicial:
- 6 a 8 fragmentos;
- cada fragmento com aproximadamente 20–30% do valor ofensivo de referência;
- impedir múltiplos fragmentos acertando o mesmo alvo de produzir burst absurdo;
- fragmentos não podem reativar o mesmo cristal imediatamente;
- sem reação em cadeia infinita.

Objetivo:
transformar posicionamento perto do cristal em risco e oportunidade.

---

## 32.4 BARREIRA DE AMPLIFICAÇÃO

Barreira mágica atravessável por projéteis.

Quando uma habilidade compatível atravessa:
- recebe amplificação mecânica de dano;
- efeito visual muda claramente;
- jogador consegue identificar que o ataque foi amplificado.

Referência:
- aumento entre 10% e 15% de dano;
- nunca ultrapassar o teto decidido sem novo balanceamento;
- amplificações não acumulam entre múltiplas barreiras.

Regra anti-abuso:
> uma mesma habilidade só pode receber uma amplificação de mapa.

Uso esperado:
- Mago e Arqueiro valorizam posicionamento próximo;
- adversários sabem que aquela região favorece ranged;
- Assassino/Guerreiro podem tentar expulsá-los dali.

A barreira não pertence a um time.
Qualquer jogador pode aproveitá-la.

---

## 32.5 RUNA DE VELOCIDADE

Runa fixa no chão.

Ao atravessar/ativar:
- aumenta temporariamente a velocidade de movimento;
- não aumenta dano;
- não concede invulnerabilidade.

Referência inicial:
- +10% a +15% de velocidade;
- duração curta;
- cooldown individual para reutilização;
- não acumula com outra runa igual.

Objetivo:
criar rotas de perseguição, fuga e reposicionamento conhecidas por jogadores experientes.

A runa deve beneficiar qualquer classe.

---

## 32.6 MOITA REATIVA

Moita continua servindo como elemento de ocultação.

Porém pode ser temporariamente destruída por ataques/efeitos compatíveis.

Regras:
- destruição NÃO é permanente;
- após um período, a vegetação se regenera;
- regeneração deve possuir sinal visual antes de voltar completamente;
- jogadores podem memorizar que determinada cobertura está temporariamente indisponível.

Referência inicial para teste:
- moita destruída entre 15 e 30 segundos;
- depois começa processo visual de regeneração.

Possíveis gatilhos:
- ataques pesados;
- explosões;
- magia de área;
- interações específicas futuras.

Evitar que todo ataque básico destrua vegetação automaticamente.

---

## 32.7 CADEIAS DE INTERAÇÃO DO MAPA

A identidade desejada permite combinações como:

- magia atravessa Barreira de Amplificação;
- atinge Parede Prismática;
- é refletida;
- acerta Cristal de Fragmentação;
- gera fragmentos em novas direções.

Essas combinações podem se tornar uma assinatura do Battle Royale X.

Porém devem possuir limites técnicos e de balanceamento.

Regra inicial:
- uma habilidade registra quais interações de mapa já utilizou;
- não repetir a mesma categoria infinitamente;
- limitar quantidade total de modificações ambientais por projétil;
- efeitos secundários possuem dano reduzido;
- efeitos secundários não reiniciam cadeias completas.

Objetivo:
permitir jogadas criativas sem criar loops imprevisíveis.

---

## 32.8 TAGS DE INTERAÇÃO

No futuro, habilidades devem possuir categorias/tags que o mapa consiga consultar.

Exemplos:
- Physical;
- Magical;
- Projectile;
- Melee;
- Area;
- Movement;
- Heavy;
- Reflectable;
- Amplifiable;
- FragmentTrigger.

O objeto do mapa reage à propriedade da habilidade, e não à classe do personagem.

Exemplo:
- Parede Prismática procura `Reflectable + Magical`;
- Barreira de Amplificação procura `Amplifiable`;
- Moita reage a `Heavy` ou `Area`.

Isso mantém o sistema escalável para novas classes.

---

## 32.9 PRINCÍPIO DE BALANCEAMENTO DO MAPA

Elementos arcanos não devem decidir a luta sozinhos.

Eles oferecem:
- ângulo;
- risco;
- oportunidade;
- rota;
- posicionamento;
- amplificação limitada.

Eles não oferecem:
- morte automática;
- dano gigantesco;
- hard CC;
- imunidade;
- vantagem permanente.

Regra:

> o mapa aumenta a expressão de habilidade do jogador; não joga por ele.

---

## 32.10 IDENTIDADE DAS REGIÕES

No futuro, diferentes regiões do mapa podem possuir combinações diferentes de elementos arcanos.

Exemplo:

### Santuário Prismático
- muitas paredes reflexivas;
- poucas moitas;
- favorece domínio de ângulos.

### Jardim Arcano
- moitas reativas;
- runas de velocidade;
- paredes de fase.

### Ruínas de Ressonância
- cristais de fragmentação;
- barreiras de amplificação;
- corredores estreitos.

Isso faz a localização da Zona de Confronto mudar também o estilo de batalha.

---



---

# 33. MAPA MODULAR RECOMBINÁVEL

## DIREÇÃO APROVADA

Todos os mapas/partidas devem oferecer **todas as categorias principais de interação arcana**.

Nenhuma classe deve possuir um "bioma obrigatório" onde sempre queira cair por ser a única região que contém a interação ideal para seu kit.

Cada setor pode favorecer certos estilos por:
- maior quantidade;
- melhor posicionamento;
- geometria;
- combinação entre elementos;

mas não por exclusividade absoluta.

Exemplo:
- uma região pode ter mais Barreiras de Amplificação;
- outra mais Paredes Prismáticas;
- outra mais Moitas Reativas;
- porém todas ainda possuem pelo menos algumas opções das demais categorias.

Objetivo:

> toda classe encontra ferramentas úteis em qualquer parte do mapa, mas jogadores experientes aprendem quais setores favorecem determinadas estratégias.

## 33.1 MÓDULOS / PEÇAS DE MAPA

O mapa futuro pode ser construído como um grande quebra-cabeça de módulos compatíveis.

Cada módulo deve possuir:
- bordas/conexões padronizadas;
- entradas e saídas compatíveis;
- navegação garantida;
- pontos reservados para loot;
- pontos reservados para elementos arcanos;
- cobertura;
- espaço de combate;
- regras de conexão com módulos vizinhos.

Os módulos podem ser:
- reposicionados entre partidas;
- rotacionados quando a geometria permitir;
- combinados em ordens diferentes;
- receber variações internas de objetos/interações.

A aleatoriedade deve modificar a leitura da partida sem gerar mapa inválido.

## 33.2 GARANTIA DE INTERAÇÕES

Cada macrozona deve possuir uma quantidade mínima de categorias.

Categorias principais iniciais:
- Parede de Fase;
- Parede Prismática;
- Cristal de Fragmentação;
- Barreira de Amplificação;
- Runa de Velocidade;
- Moita Reativa.

Não é necessário ter a mesma quantidade de cada uma.

Exemplo conceitual:
- setor A: 4 prismáticas, 1 amplificação, 2 runas, 2 cristais, 3 moitas, 1 parede de fase;
- setor B: 1 prismática, 4 amplificações, 1 runa, 3 cristais, 2 moitas, 2 paredes de fase.

Ambos oferecem todas as mecânicas, mas produzem combates diferentes.

## 33.3 ALEATORIEDADE EM CAMADAS

A variedade do mapa pode acontecer em três níveis:

### Camada 1 — posição dos módulos
Grandes setores mudam de posição/ordem.

### Camada 2 — rotação/ligação
Quando seguro para gameplay, módulos podem ser rotacionados ou conectados por lados diferentes.

### Camada 3 — população interna
Dentro do mesmo módulo, posições pré-validadas podem alternar:
- interação arcana;
- loot;
- cobertura;
- moita;
- runa;
- cristal;
- barreira.

Não usar geração totalmente livre.

Preferir sockets/pontos previamente testados para evitar:
- caminhos bloqueados;
- spawn injusto;
- interação impossível;
- combo ambiental quebrado;
- vantagem acidental de um lado.

## 33.4 REGRAS DE ENCAIXE

Todos os módulos precisam seguir um padrão comum de borda.

Cada lado pode possuir conectores como:
- passagem aberta;
- corredor;
- arco/portal;
- conexão larga;
- conexão estreita.

Um gerador futuro só pode encaixar lados compatíveis.

Depois da montagem, deve validar:
- todos os setores alcançáveis;
- múltiplas rotas entre áreas importantes;
- nenhuma região isolada;
- nenhum spawn preso;
- distância mínima entre pontos críticos;
- densidade mínima/máxima de interações.

## 33.5 MEMÓRIA + IMPROVISO

O objetivo não é tornar o mapa completamente desconhecido.

O jogador deve conseguir aprender os módulos individualmente.

Exemplo:
> "Reconheço o Jardim Prismático e sei como usar aquela parede."

Mas não necessariamente saber:
> "O Jardim sempre fica no nordeste e sempre conecta com as Ruínas."

Assim o jogo recompensa simultaneamente:
- conhecimento;
- adaptação;
- leitura rápida da partida.

## 33.6 ZONA DE CONFRONTO + MAPA MODULAR

A Zona de Confronto pode ativar sobre módulos diferentes a cada partida.

Como todos os módulos possuem o conjunto básico de interações, nenhuma ativação é inútil para determinada classe.

Porém a configuração local muda o estilo do confronto.

Exemplo:
- zona sobre módulo com muitas prismáticas = luta de ângulo/reflexão;
- zona sobre módulo com mais moitas/runas = luta de perseguição e emboscada;
- zona sobre módulo com mais cristais/amplificadores = luta explosiva e de posicionamento.

## 33.7 PRINCÍPIO DE EQUIDADE

Aleatoriedade do mapa deve criar variedade, não decidir vencedor.

Portanto:
- não remover completamente uma categoria de interação de uma grande região;
- não gerar corredor único obrigatório;
- não posicionar amplificador de forma que um spawn tenha acesso garantido e outro não;
- não gerar combinações ambientais capazes de morte inevitável;
- usar pesos e limites de distribuição.

Regra:

> o mapa muda a pergunta tática de cada partida, mas sempre oferece mais de uma resposta válida.


---

# 34. SISTEMA DE INÍCIO POR COORDENADA — SEM BIOMA E SEM CONTAGEM DE JOGADORES

## DIREÇÃO APROVADA

O jogador não escolhe "bioma" nem recebe informação de quantas pessoas escolheram determinada região.

O mapa modular pode mudar completamente de composição entre partidas, mas sua referência espacial em X/Y continua existindo.

Portanto a decisão de início deve ser:

> escolher uma área/coordenada física aproximada do mapa.

O jogador pode decidir:
- canto noroeste;
- centro-leste;
- sul;
- setor central;
- ou outra região espacial equivalente;

mas não sabe necessariamente qual módulo/bioma estará ocupando aquele espaço naquela partida até a configuração ser revelada.

## 34.1 O QUE O JOGADOR ESCOLHE

Escolha espacial, não temática.

Exemplo:
- quadrante superior esquerdo;
- região central;
- borda inferior direita.

O sistema converte a escolha em um conjunto de pontos válidos daquela região.

O jogador não seleciona:
- "Jardim Arcano";
- "Ruínas";
- "Santuário Prismático";
- ou qualquer outro nome de bioma/módulo.

Isso impede que uma classe escolha repetidamente o mesmo tipo de cenário.

## 34.2 O QUE O JOGADOR NÃO SABE

Antes do início, não mostrar:
- quantidade de jogadores por ponto;
- composição de classes;
- nível de disputa;
- indicador verde/amarelo/vermelho de população.

A densidade de jogadores precisa ser descoberta naturalmente ao iniciar a partida.

Isso preserva:
- risco;
- surpresa;
- leitura;
- decisões sociais;
- imprevisibilidade.

## 34.3 RELAÇÃO COM O MAPA MODULAR

A posição X/Y do mundo permanece estável como referência.

Porém o módulo que ocupa aquela região pode mudar.

Exemplo:

Partida A:
- noroeste = Jardim Arcano.

Partida B:
- noroeste = Ruínas de Ressonância.

Partida C:
- noroeste = Santuário Prismático.

Um jogador pode gostar de iniciar no noroeste por estratégia de rota, mas não consegue garantir que encontrará sempre o mesmo bioma.

## 34.4 DISTRIBUIÇÃO DENTRO DA REGIÃO ESCOLHIDA

O jogador escolhe uma região aproximada.

O sistema seleciona um spawn válido entre vários sockets daquela área.

Objetivos:
- impedir spawn pixel-perfect decorado;
- evitar jogadores materializando exatamente no mesmo ponto;
- manter controle estratégico sem dar precisão excessiva.

Não transformar a escolha em RNG total.

O jogador escolhe a região.
O sistema escolhe o ponto seguro dentro dela.

## 34.5 APRESENTAÇÃO TEMÁTICA

A mecânica pode continuar usando a fantasia de Fraturas/Nexo.

Porém as Fraturas representam posições espaciais do mapa, não biomas.

Exemplo:
- o Nexo mostra uma projeção do mapa;
- jogador marca uma área;
- uma Fratura se vincula àquela coordenada;
- na abertura da partida ele é materializado em um socket válido próximo.

Nome provisório do sistema:
- Fratura de Entrada;
- Fratura de Materialização;
- Sistema de Fraturas.

## 34.6 MAPA VISÍVEL OU PARCIALMENTE VISÍVEL

Questão ainda EM TESTE:

Decidir futuramente quanto da composição modular o jogador consegue ver antes de escolher a coordenada.

Possibilidades:
- mapa completo já revelado;
- silhueta/geometria geral sem mostrar todas as interações;
- mapa parcialmente oculto;
- montagem acontecendo depois da escolha.

A decisão deve equilibrar:
- estratégia;
- surpresa;
- RNG;
- justiça.

## 34.7 ZONA DE CONFRONTO

A Zona de Confronto é independente do ponto de entrada.

Ela muda de posição ao longo da partida e não deve ser usada para escolher o spawn inicial.

Direção:
- ponto inicial é definido primeiro;
- Zona de Confronto é sorteada/revelada separadamente;
- depois pode se mover novamente durante a partida.

Isso evita que o sistema de início se transforme em uma corrida garantida para a primeira Zona de Confronto.



---

# 35. SPAWN ELÁSTICO POR COORDENADA

## DIREÇÃO APROVADA

O jogador escolhe apenas uma posição aproximada X/Y no mapa.

Antes da partida:
- não enxerga o mapa;
- não enxerga o bioma/módulo;
- não vê quantidade de jogadores;
- não vê classes escolhidas naquela região.

O sistema nunca deve bloquear a escolha porque "acabaram os pontos de nascimento".

Se muitos jogadores escolherem a mesma região, a área de materialização se expande automaticamente de forma invisível.

## 35.1 REGIÃO ELÁSTICA

Cada escolha de coordenada gera uma região de spawn ao redor do ponto selecionado.

O sistema tenta primeiro usar sockets válidos mais próximos da coordenada.

Conforme esses sockets são ocupados:
1. usa o próximo anel de sockets;
2. aumenta gradualmente o raio de procura;
3. continua priorizando distância mínima em relação à escolha original.

Assim:
- pouca gente escolhendo o ponto = spawns muito próximos da coordenada desejada;
- muita gente escolhendo = grupo espalhado em área maior;
- todos continuam nascendo naquela parte geral do mapa.

## 35.2 SEM CAPACIDADE VISÍVEL

Não mostrar:
- lotação;
- quantidade de vagas;
- aviso de saturação;
- número de jogadores;
- intensidade estimada de combate.

Escolher uma região movimentada deve ser um risco oculto.

## 35.3 SOCKETS DE SPAWN

Cada módulo deve possuir muitos sockets de materialização previamente validados.

Cada socket precisa garantir:
- chão navegável;
- distância mínima de parede/obstáculo;
- espaço suficiente para o personagem;
- nenhuma interseção com objeto;
- nenhuma posição dentro de área impossível;
- acesso a pelo menos duas direções quando possível.

Os sockets não precisam aparecer visualmente.

## 35.4 DISTÂNCIA MÍNIMA ENTRE JOGADORES

No momento da atribuição, aplicar distância mínima entre spawns.

Referência inicial:
- aproximadamente 6–10 metros entre jogadores no mesmo cluster;
- valor final depende do TTK e do tamanho real do mapa.

Se não houver socket disponível dentro do raio inicial:
- expandir o raio;
- não empilhar personagens no mesmo ponto.

## 35.5 FALLBACK EM ANÉIS

O algoritmo deve procurar posições em anéis concêntricos.

Exemplo conceitual:
- Anel 1: raio curto;
- Anel 2: raio médio;
- Anel 3: raio maior;
- fallback final: setor vizinho mais próximo.

O fallback só ocorre quando não existe espaço seguro suficiente.

Mesmo no fallback:
- preservar o máximo possível a intenção espacial do jogador;
- nunca enviar o jogador para o outro lado do mapa apenas por saturação.

## 35.6 TODOS ESCOLHERAM O MESMO LUGAR

Caso extremo:
- todos os jogadores escolhem praticamente a mesma coordenada.

Resultado desejado:
- todos são aceitos;
- spawns são distribuídos pelos sockets válidos mais próximos;
- raio se expande progressivamente;
- forma-se naturalmente um hot drop massivo;
- ninguém recebe informação prévia de que isso acontecerá.

Isso é comportamento válido, não erro.

## 35.7 PROTEÇÃO DE MATERIALIZAÇÃO

Para evitar morte antes do jogador assumir controle:

Referência inicial:
- 1,0–1,5 s de fase de materialização;
- pode se movimentar;
- não pode causar dano;
- não recebe dano;
- não pode coletar loot durante a fase, se isso gerar abuso.

A proteção termina:
- ao acabar o tempo;
- ou imediatamente se o jogador tentar executar ação ofensiva, caso essa regra seja usada no futuro.

A duração deve ser curta para não virar ferramenta de aproximação gratuita.

## 35.8 LOOT E SPAWN

Evitar loot de alto valor exatamente em cima de sockets de spawn.

Aplicar distância mínima entre:
- spawn;
- habilidade rara;
- item excepcional;
- interação ambiental extremamente vantajosa.

Objetivo:
evitar que RNG de nascimento entregue recompensa instantânea impossível de disputar.

## 35.9 PRINCÍPIO

> O jogador escolhe onde quer começar; o sistema resolve onde exatamente ele pode materializar com segurança.

A escolha estratégica permanece com o jogador.

A resolução técnica de densidade fica invisível.


---

# 36. GUARDIÕES ARCANOS / MONSTROS DE ALTO VALOR

## DIREÇÃO APROVADA

O mapa terá monstros especiais de alto risco que funcionam como objetivos PvE disputáveis.

Objetivo:
- criar pontos de interesse além da Zona de Confronto;
- gerar risco/recompensa;
- forçar decisão entre lutar contra o monstro, economizar recursos ou disputar outros jogadores;
- permitir que PvE gere naturalmente PvP ao redor.

Nome provisório:
- Guardião Arcano;
- Guardião de Ruína;
- Criatura Ancestral;
- outro nome final futuro.

## 36.1 DROP RARO

Ao derrotar um Guardião:
- aproximadamente 30% de chance de gerar um drop raro/especial;
- o drop fica fisicamente no chão;
- não entra automaticamente no inventário;
- outros jogadores podem disputar.

O drop pode conter futuramente:
- variação rara de habilidade;
- item tático raro;
- redução de cooldown especial;
- cura de maior valor;
- outro recurso de alto valor aprovado no sistema de loot.

A raridade continua obedecendo à regra geral:
- vantagem principalmente mecânica;
- nunca mais que aproximadamente 20% de ganho mecânico total;
- sem salto absurdo de dano bruto.

## 36.2 ESCALA SOLO X EQUIPE

O Guardião precisa se adaptar ao modo de jogo.

### Modo equipe
- vida maior;
- padrões mais difíceis;
- pressão suficiente para exigir cooperação;
- deve ser muito difícil ou inviável derrotar rapidamente sozinho.

### Modo solo
- vida e/ou dano reduzidos;
- padrões ajustados para um único jogador;
- ainda deve custar recurso e vida;
- derrotá-lo sozinho é possível, mas arriscado.

Princípio do solo:
> o jogador consegue matar, porém normalmente sai da luta vulnerável e com parte relevante da vida/recursos consumidos.

Não criar um chefe que seja simplesmente uma esponja de HP.

A dificuldade deve vir também de:
- telegraphs;
- posicionamento;
- ataques evitáveis;
- leitura de padrão;
- uso das interações do mapa.

## 36.3 RESPAWN

Referência inicial:
- respawn aproximadamente a cada 60 segundos após a morte.

O respawn de 1 minuto é valor de teste e pode mudar depois de observar:
- duração média da partida;
- frequência de contestação;
- quantidade de drops;
- facilidade de farm;
- densidade do mapa.

## 36.4 ANTI-FARM

Como o respawn é frequente, impedir que uma equipe controle permanentemente o mesmo Guardião e acumule vantagem sem risco.

Possíveis proteções:
- posição do Guardião exposta/telegráfica;
- luta gera efeitos visuais/sonoros perceptíveis;
- drop fica no chão e pode ser roubado;
- Guardião não concede atributos permanentes;
- loot continua limitado por slots;
- chance de 30% não garante recompensa;
- considerar diminishing reward ou rotação futura apenas se testes mostrarem farm excessivo.

Não adicionar redução artificial por equipe antes de testar; primeiro observar comportamento real.

## 36.5 CURA DURANTE A LUTA

O Guardião deve causar pressão suficiente para que:
- cura tenha valor;
- gastar cura no PvE tenha custo estratégico;
- jogador possa ser surpreendido por outro jogador enquanto está enfraquecido.

No modo solo, a intenção é que o jogador frequentemente termine a luta com perda relevante de vida, mesmo vencendo corretamente.

## 36.6 INTERAÇÃO COM O MAPA

Guardião deve poder interagir com Terreno Arcano Reativo.

Exemplos futuros:
- magia do Guardião refletida por Parede Prismática;
- ataques pesados temporariamente destroem Moita Reativa;
- projétil do Guardião atravessa Barreira de Amplificação;
- jogador usa Cristal de Fragmentação contra o Guardião;
- Runa de Velocidade ajuda a evitar padrão de ataque.

Isso reforça a identidade do Battle Royale X:
> PvE e PvP usam o mesmo conjunto de regras do mundo.

## 36.7 TELEGRAPH E JUSTIÇA

Ataques fortes do Guardião devem ser evitáveis.

Não usar:
- dano inevitável excessivo;
- stun longo;
- chain CC;
- golpe surpresa impossível de ler.

Usar:
- círculos/linhas no chão;
- animações claras;
- som;
- preparação visual;
- padrões aprendíveis.

O jogador deve perder vida principalmente por erro de execução ou por decidir continuar lutando sob pressão.

## 36.8 POSICIONAMENTO NO MAPA

Não colocar todos os Guardiões em um único tipo de módulo.

O sistema modular deve permitir sockets de Guardião em diferentes regiões.

Nem toda posição precisa estar ativa ao mesmo tempo.

Possibilidades futuras:
- alguns Guardiões ativos por partida;
- posição sorteada entre sockets válidos;
- diferentes modelos/padrões usando a mesma categoria de recompensa.

Isso impede rota fixa de farm.

## 36.9 RELAÇÃO COM A ZONA DE CONFRONTO

Guardião e Zona de Confronto são sistemas independentes.

Pode acontecer:
- Guardião fora da Zona;
- Guardião dentro da Zona;
- Zona mover e passar por um Guardião.

Se ambos coincidirem, o local naturalmente vira ponto de altíssimo risco.

Não aumentar automaticamente o drop só porque está dentro da Zona, salvo decisão futura de balanceamento.

## 36.10 PRINCÍPIO

> O Guardião oferece uma chance de acelerar sua build, mas cobra vida, tempo, cooldowns e exposição.

Ele não deve ser obrigatório para vencer.

Jogador pode:
- ignorar;
- tentar sozinho;
- disputar em equipe;
- esperar outro grupo enfraquecê-lo;
- roubar o drop;
- usar o combate como emboscada.

Isso cria decisão emergente sem transformar PvE em requisito.


---

# 37. TITÃ ERRANTE — CHEFÃO AMBIENTAL DINÂMICO

## DIREÇÃO APROVADA

O Battle Royale X terá um super chefão móvel, provisoriamente chamado **Titã Errante**.

Ele não é um boss tradicional parado esperando ser atacado.

Função principal:
- atravessar o mapa;
- interferir em confrontos;
- quebrar posições seguras;
- atrapalhar jogadores escondidos;
- gerar decisões emergentes;
- poder ser usado indiretamente como ameaça contra outros jogadores;
- tornar-se progressivamente mais viável como objetivo de caça ao longo da partida.

Princípio central:

> Ignore o Titã e ele atrapalha. Ataque o Titã e ele luta de verdade.

---

## 37.1 COMPORTAMENTO GERAL

Quando existe uma Zona de Confronto ativa:
- o Titã prioriza caminhar em direção a ela;
- ao chegar, permanece circulando/atuando dentro ou próximo da região;
- interfere em lutas e jogadores dentro de seu raio de percepção.

Quando não existe Zona de Confronto ativa:
- entra em modo errante;
- procura atividade;
- combate;
- uso de habilidades;
- barulho/atividade futura equivalente;
- jogadores próximos;
- jogadores que o atacaram.

O roaming deve ser semi-inteligente, não aleatório puro.

---

## 37.2 PRIORIDADE DE ALVO

No comportamento normal, o Titã prioriza jogadores com maior **porcentagem de vida atual** dentro da área relevante.

Não usar vida absoluta.

Exemplo:
- Guerreiro com 80% HP;
- Assassino com 35% HP.

A tendência normal é o Titã escolher o Guerreiro.

Isso permite uso estratégico:
- jogador ferido pode fugir em direção ao Titã;
- perseguidor saudável corre risco de se tornar alvo prioritário.

A prioridade por HP% não é absoluta se existir provocação.

---

## 37.3 PROVOCAÇÃO

Se um jogador decide atacar o Titã de maneira relevante, passa a ser considerado **Provocador**.

Enquanto provocado:
- prioridade de alvo aumenta fortemente;
- Titã pode ignorar um jogador mais saudável para responder a quem o está enfrentando;
- regras de limitação de dano são reiniciadas conforme descrito abaixo.

Ordem conceitual de ameaça:

1. jogadores que provocaram o Titã recentemente;
2. entre provocadores, maior ameaça/dano recente;
3. se não houver provocação, maior % de HP;
4. proximidade/atividade como desempate.

---

## 37.4 DANO PERCENTUAL

Ataques ofensivos do Titã causam dano baseado na **vida máxima do alvo**.

Referência definida:
- uma habilidade ofensiva típica causa aproximadamente **20% da vida máxima** do personagem.

O objetivo é fazer o Titã ter impacto semelhante contra classes com HP diferentes.

Não usar dano fixo como regra principal.

---

## 37.5 ORÇAMENTO NORMAL DE DANO

Enquanto o jogador NÃO está lutando deliberadamente contra o Titã:

- o Titã pode causar no máximo aproximadamente **30% da vida máxima daquele jogador a cada janela de 10 segundos**.

Esse limite é individual por jogador.

Exemplo:
- alvo tem 100 HP;
- Titã acerta habilidade de 20%;
- ainda existe orçamento de 10% durante a mesma janela;
- novo golpe que causaria 20% fica limitado ao orçamento restante.

O dano continua letal:
- se o jogador já está com pouca vida, o Titã pode finalizá-lo.

Não existe proteção automática de 1 HP.

---

## 37.6 RESET AO ATACAR O TITÃ

A regra muda quando o jogador escolhe lutar contra ele.

Quando um jogador causa dano relevante ao Titã:
- a janela/orçamento de 10 segundos desse jogador é resetada;
- o Titã pode voltar a causar até o orçamento normal novamente;
- a provocação é atualizada;
- o jogador passa a correr risco real de ser focado.

Isso significa:

> atacar o Titã voluntariamente remove parte da proteção natural que existe para quem está apenas atravessando/fugindo.

Se o jogador continua atacando:
- novas provocações podem continuar reiniciando o risco conforme implementação final.

A frequência exata do reset deve respeitar um limiar de provocação para evitar abuso/acidente.

---

## 37.7 DANO ACIDENTAL

Pequeno dano incidental não deve necessariamente transformar o jogador em alvo principal.

Antes de considerar Provocação completa, usar um limiar.

Pode considerar:
- dano acumulado mínimo;
- múltiplos acertos;
- hit direto;
- intenção inferida por habilidade/targeting.

Referência inicial:
- aproximadamente 3–5% de uma vida normal equivalente em dano acumulado já pode contar como provocação real.

Valor final será testado.

---

## 37.8 STUN E CONTROLE

O Titã é uma exceção controlada à filosofia normal de hard CC.

Ele pode possuir:
- stun;
- knockback;
- empurrão;
- lançamento lateral;
- interrupção;
- ondas de choque;
- grandes deslocamentos.

Porém não deve criar cadeia infinita de controle.

Usar diminishing returns para controle repetido.

Exemplo conceitual:
- primeiro stun: forte;
- segundo stun próximo: duração menor;
- terceiro: muito curto;
- depois resistência temporária.

Valores finais serão definidos em teste.

O objetivo é:
> causar caos e reposicionamento, não deixar o jogador permanentemente sem jogar.

---

## 37.9 KIT CONCEITUAL DO TITÃ

### Pisão Sísmico
- grande área telegráfica;
- aproximadamente 20% de dano máximo;
- knockback;
- stun/control compatível;
- forte impacto visual.

### Varredura
- arco amplo;
- aproximadamente 20%;
- empurra jogadores lateralmente;
- ameaça posicionamento próximo de objetos arcanos.

### Impacto de Chão
- área muito grande;
- dano menor que habilidades principais se necessário;
- controle/repulsão forte;
- pode ativar elementos do cenário.

### Investida
- telegráfica;
- dano percentual;
- empurra/carrega o alvo;
- muda a posição da luta.

O kit final pode mudar, mas deve priorizar:
- área;
- controle;
- perturbação;
- telegraph;
- pouca capacidade de burst instantâneo.

---

## 37.10 INTERAÇÃO COM TERRENO ARCANO

O Titã utiliza as mesmas regras físicas/mágicas do mundo.

Possibilidades aprovadas conceitualmente:
- ataques dele podem ativar Cristais de Fragmentação;
- pode destruir Moitas Reativas temporariamente;
- projéteis/efeitos compatíveis podem interagir com Barreiras de Amplificação;
- paredes e elementos podem alterar seus ataques quando fizer sentido;
- jogadores podem usar elementos do mapa contra ele.

PvE e PvP devem parecer pertencentes ao mesmo sistema.

---

## 37.11 VIDA INICIAL

No começo da partida, o Titã possui vida extremamente alta.

Referência definida:
- aproximadamente **150 vezes a vida de um personagem normal**.

A intenção é:
- início: praticamente um elemento ambiental, não um alvo realista de solo;
- meio: grupos podem começar a considerar atacá-lo;
- final: torna-se um objetivo realmente matável.

Não tratar 150x como número final até testes de TTK/PvE.

---

## 37.12 REDUÇÃO DE VIDA POR CICLO

A cada ciclo de fechamento da área principal do Battle Royale:
- a vida máxima do Titã é reduzida.

Na última fase:
- deve chegar a no máximo aproximadamente **5 vidas normais**.

Progressão conceitual possível:
- 150x;
- 80x;
- 40x;
- 20x;
- 10x;
- 5x.

A sequência exata depende do número de ciclos da partida.

Regra fundamental:
- reduzir o teto não cura o Titã;
- dano causado anteriormente continua relevante.

Formalmente:

`HP atual novo = min(HP atual anterior, novo HP máximo)`

Se já estiver abaixo do novo teto:
- mantém a vida atual.

---

## 37.13 FUNÇÃO AO LONGO DA PARTIDA

### Início
- quase impossível de matar;
- cria caos;
- força deslocamento;
- quebra esconderijos;
- segue Zona de Confronto.

### Meio
- já acumulou dano e perdeu teto de HP;
- equipes podem considerar atacá-lo;
- continua perigoso por controle e intervenção.

### Final
- vida máxima reduzida significativamente;
- pode estar próximo de 5x HP normal;
- torna-se um objetivo real;
- jogadores podem decidir finalizar pelo item único.

---

## 37.14 DROP ÚNICO — EVOLUÇÃO DA ULTIMATE

Ao morrer:
- Titã deixa um **item único da partida**.

Regra definida:

> O item do Titã evolui **somente a Ultimate atualmente equipada** do jogador que conseguir coletá-lo.

Nunca evolui:
- Ataque Básico;
- Defesa;
- Movimento.

A evolução:
- dura somente até o fim daquela partida;
- melhora a Ultimate principalmente em comportamento/mecânica;
- não deve transformar a Ultimate em morte inevitável;
- não deve criar aumento bruto exagerado de dano;
- deve respeitar a filosofia de vantagem mecânica limitada;
- pode melhorar alcance, área, duração, janela, mobilidade, controle de direção, recovery, interação com mapa ou outra propriedade coerente com aquela Ultimate.

A evolução Titânica deve ser visualmente reconhecível.

Se o jogador trocar de variação de Ultimate depois de obter a evolução:
- a Evolução Titânica acompanha o **slot Ultimate**;
- a nova variação equipada passa a receber sua versão Titânica correspondente;
- o jogador não perde a evolução por trocar de variação;
- apenas uma Ultimate por vez recebe o efeito, sempre a que estiver atualmente equipada.

O item cai fisicamente no chão:
- não vai automaticamente para quem deu o último hit;
- pode ser disputado ou roubado;
- somente quem o coleta recebe a Evolução Titânica.

Não conceder evolução simultânea a múltiplos slots.

O design específico de cada **Ultimate Titânica** será definido junto com as Ultimates finais das classes.


---

## 37.15 USO ESTRATÉGICO COMO "ALIADO"

O Titã nunca pertence a um jogador/time.

Mas sua IA pode ser manipulada indiretamente.

Exemplo:
- jogador com pouca vida foge em direção ao Titã;
- perseguidor possui maior % HP;
- Titã tende a focar o perseguidor saudável;
- isso cria oportunidade de fuga.

Outro exemplo:
- perseguidor resolve atacar o Titã;
- provoca o boss;
- reset de orçamento;
- Titã passa a tratá-lo como alvo prioritário.

Nenhum comando direto existe.

A "aliança" emerge do comportamento do sistema.

---

## 37.16 COMBATE A CAMPING

O Titã contribui contra jogadores excessivamente escondidos por:
- aproximar-se de zonas de combate;
- usar ataques em área;
- destruir temporariamente moitas;
- alterar cobertura;
- forçar movimentação.

Não deve possuir wallhack arbitrário.

Ele reage ao mundo e à atividade, não "sabe" magicamente onde todo jogador está.

---

## 37.17 RAIO DE PERCEPÇÃO E COMBATE

Valores ainda EM TESTE.

Direção:
- um raio maior para detectar/considerar alvos;
- um raio menor para iniciar habilidades.

Referência de protótipo futuro:
- percepção: aproximadamente 20–30 m;
- combate: aproximadamente 12–18 m.

Não tratar como valores finais.

---

## 37.18 RELAÇÃO COM GUARDIÕES MENORES

Titã Errante e Guardiões Arcanos são sistemas diferentes.

### Guardiões
- objetivos PvE localizados;
- respawn;
- 30% de chance de drop raro;
- pensados para ser caçados deliberadamente.

### Titã
- único/grande;
- anda pelo mapa;
- não existe principalmente para ser farmado;
- atrapalha a partida inteira;
- vida decai com ciclos;
- dropa item único quando finalmente morre.

Não confundir os dois sistemas.

---

## 37.19 PRINCÍPIO FINAL DO TITÃ

> Fugiu dele: ele incomoda.
>
> Ignorou: ele muda a luta.
>
> Usou contra outro jogador: criou oportunidade.
>
> Resolveu atacar: ele pega pesado.
>
> Sobreviveu até o fim: agora talvez valha a pena caçá-lo.

O Titã deve ser parte da história de cada partida, não apenas um saco de HP com loot.


---

# 38. HYBRID BOT SIMULATION

## DIREÇÃO APROVADA

O Battle Royale X deve ser jogável mesmo com poucos ou nenhum jogador humano suficiente para preencher a partida.

A solução definida é um sistema híbrido:

> bots próximos de jogadores reais existem como personagens completos; bots distantes existem como simulação leve.

Objetivos:
- permitir partidas completas desde o início da vida do jogo;
- evitar custo desnecessário de IA/física para todos os bots ao mesmo tempo;
- manter ritmo natural de Battle Royale;
- impedir que um único humano termine uma partida de 40 participantes com dezenas de kills artificiais;
- preservar coerência espacial e de estado dos bots mesmo fora da visão do jogador.

## 38.1 TRÊS NÍVEIS DE DIFICULDADE

Os bots possuem três níveis:

### Iniciante
- reação lenta;
- erra mais habilidades;
- usa dodge/parry tarde;
- tende a perseguir demais;
- usa cooldowns de forma menos eficiente;
- entende poucas interações de mapa;
- toma decisões simples.

### Intermediário
- usa o kit corretamente;
- entende fuga e cura;
- sabe usar cobertura;
- utiliza algumas interações de mapa;
- controla melhor cooldown;
- consegue cancelar basic e priorizar skill corretamente.

### Experiente
- controla distância;
- conhece matchups;
- baita Parry;
- usa mapa de forma inteligente;
- usa Titã/Guardião como parte da decisão;
- troca variações de forma coerente;
- foge de lutas ruins;
- tenta finalizar quando possui vantagem real.

Regra fundamental:
- dificuldade de bot NÃO aumenta HP;
- dificuldade de bot NÃO aumenta dano;
- dificuldade vem de decisão, timing, precisão e uso dos sistemas.

## 38.2 BOT RECORD

Todo bot possui um registro persistente leve, mesmo quando não existe como GameObject completo.

Dados mínimos:
- ID;
- nome;
- posição aproximada;
- classe;
- HP;
- energia;
- kit/variações;
- inventário/loot;
- dificuldade;
- objetivo atual;
- estado de combate;
- cooldowns relevantes;
- adversário atual;
- última atividade;
- vivo/morto.

O bot não desaparece conceitualmente quando sai da área do jogador.

## 38.3 ACTIVE BOT

Quando está dentro da área de relevância de um jogador real:
- existe como personagem completo;
- usa GameObject;
- movimento real;
- hitbox/hurtbox;
- habilidades;
- colisões;
- navegação;
- VFX;
- interação com mapa;
- combate real.

Esse é o bot que o jogador pode ver e enfrentar.

## 38.4 VIRTUAL BOT

Longe de jogadores reais:
- não precisa existir como personagem completo;
- usa simulação abstrata;
- atualiza posição aproximada;
- toma decisões em frequência reduzida;
- pode procurar loot;
- pode mover-se para zona;
- pode encontrar outros bots;
- pode entrar em combate virtual;
- pode fugir;
- pode morrer.

Quando volta para a área de relevância:
- materializa com estado consistente;
- posição;
- HP;
- energia;
- kit;
- inventário;
- cooldowns aproximados;
- objetivo atual.

## 38.5 TRANSIÇÃO VIRTUAL ↔ ATIVO

Ao entrar no raio de relevância:
- VirtualBot → ActiveBot.

Ao sair por tempo/distância suficiente:
- ActiveBot → VirtualBot.

Antes de desmaterializar:
- salvar estado relevante.

Evitar transição constante na borda do raio:
- usar histerese;
- raio de ativação menor;
- raio de desativação um pouco maior;
- ou tempo mínimo fora da área.

## 38.6 BOLHAS DE SIMULAÇÃO

Valores exatos serão testados.

Estrutura conceitual:

### Próximo
- simulação completa.

### Médio
- simulação simplificada;
- atualizações menos frequentes;
- pode manter posição e decisões aproximadas.

### Distante
- somente estado virtual;
- sem física;
- sem animação;
- sem combate frame a frame.

Referências de distância serão definidas conforme:
- tamanho real do mapa;
- câmera;
- plataforma;
- multiplayer;
- performance.

## 38.7 BOT CONTRA BOT FORA DA VISÃO

Bots distantes não devem simplesmente morrer por sorte instantânea.

Quando dois VirtualBots entram em conflito:
- inicia um confronto virtual;
- duração plausível;
- HP vai sendo reduzido;
- dificuldade influencia execução;
- classe/kit influencia opções;
- vida atual influencia risco;
- loot pode influenciar;
- aleatoriedade existe, mas não decide tudo instantaneamente.

O confronto pode terminar em:
- eliminação;
- fuga;
- separação;
- interrupção por zona/outro evento.

## 38.8 MATERIALIZAÇÃO DURANTE COMBATE VIRTUAL

Se um jogador real entra na região de dois bots que estavam lutando virtualmente:
- a simulação virtual para;
- ambos materializam;
- mantêm HP compatível com o combate que já ocorreu;
- continuam a luta fisicamente quando coerente.

Exemplo:
- Bot A estava com 45%;
- Bot B com 28%;
- jogador chega;
- eles aparecem com aproximadamente esses estados, não com HP cheio.

Isso evita sensação de kill feed puramente inventado.

## 38.9 KILL FEED VIRTUAL

Eliminações virtuais aparecem normalmente no kill feed.

Exemplo:
- `Ragnar eliminou Nyx`.

A eliminação:
- reduz número de participantes vivos;
- não concede kill a jogador humano sem participação;
- mantém ritmo da partida.

Não rotular obrigatoriamente como BOT durante gameplay.

## 38.10 CRÉDITO DE KILL

Um jogador humano só recebe kill se realmente participou de forma relevante.

Nunca:
- bot distante morre;
- humano recebe kill sem interação.

Regra exata de crédito entre dano/último hit/assistência será definida futuramente.

## 38.11 POPULAÇÃO E RITMO DA PARTIDA

O sistema deve ajustar comportamento conforme número de humanos reais.

Exemplo conceitual numa partida de 40:

### 1 humano
- 39 bots;
- muitos confrontos virtuais;
- humano termina com quantidade plausível de kills, não 39.

### 10 humanos
- 30 bots;
- mistura de confrontos reais e virtuais.

### 30 humanos
- 10 bots;
- pouca intervenção de simulação abstrata.

### 40 humanos
- 0 bots;
- sistema de bots não interfere no ritmo.

## 38.12 CURVA DE SOBREVIVENTES

A partida possui uma curva-alvo aproximada de participantes vivos.

Não é uma regra rígida.

Serve apenas como regulador do comportamento dos bots distantes.

Exemplo inicial para 40 participantes:
- início: 40;
- fechamento 1: ~32;
- fechamento 2: ~24;
- fechamento 3: ~17;
- fechamento 4: ~11;
- fechamento 5: ~7;
- final: ~3–5.

Se eliminações estão rápidas demais:
- VirtualBots ficam menos agressivos;
- encontros simulados terminam mais em fuga/separação.

Se eliminações estão lentas demais:
- VirtualBots procuram mais contato;
- rotação para objetivos/zonas aumenta a chance de encontro.

Nunca matar bots arbitrariamente apenas para acertar um número exato.

## 38.13 OBJETIVOS DOS BOTS

Bots não devem ter como único comportamento "achar inimigo e atacar".

Podem possuir objetivos como:
- procurar loot;
- procurar habilidade;
- buscar cura;
- ir para Zona de Confronto;
- acompanhar fechamento;
- evitar combate;
- caçar Guardião;
- avaliar Titã;
- fugir do Titã;
- trocar variação;
- perseguir alvo;
- abandonar luta;
- buscar posição melhor.

O objetivo muda conforme contexto.

## 38.14 BOTS PODEM FUGIR

Bots, especialmente Intermediário/Experiente, devem reconhecer lutas ruins.

Exemplos:
- HP baixo;
- cooldowns principais indisponíveis;
- matchup desfavorável;
- outro jogador entrando;
- Titã próximo;
- zona fechando.

Podem:
- correr;
- usar moita;
- usar runa de velocidade;
- atravessar parede de fase;
- usar cura;
- correr em direção ao Titã;
- buscar cobertura.

Isso é necessário para parecerem participantes reais do Battle Royale.

## 38.15 NOMES E APRESENTAÇÃO

Durante a partida:
- bots podem usar nomes normais;
- não é obrigatório exibir `[BOT]`.

O jogo não precisa fingir explicitamente que são humanos, mas também não precisa quebrar a apresentação durante combate.

Transparência sobre população humana/bot pode aparecer:
- no lobby;
- resultado final;
- estatísticas;
- conforme decisão futura.

## 38.16 PRINCÍPIO

> Bots distantes não são personagens completos, mas também não são mortes aleatórias.

Eles vivem uma versão simplificada da mesma partida.

Quando entram no mundo do jogador, essa simulação se transforma em gameplay real.

---

# 39. REGENERAÇÃO NATURAL FORA DE COMBATE

## DIREÇÃO APROVADA

A barra de vida recupera continuamente quando o jogador permanece fora de combate.

Objetivos:
- reduzir dependência absoluta de poções;
- permitir recuperação após sobreviver a uma luta;
- manter ritmo de Battle Royale;
- evitar que pequeno dano antigo condene o jogador por muitos minutos;
- preservar valor das curas durante combate.

## 39.1 INÍCIO DA REGENERAÇÃO

A regeneração natural não começa imediatamente após receber dano.

Precisa existir uma janela sem combate.

Referência inicial para teste:
- aproximadamente 6–10 segundos sem receber nem causar dano.

Valor final será calibrado com TTK e ritmo real.

## 39.2 O QUE CONTA COMO COMBATE

A janela é reiniciada quando o jogador:
- recebe dano de outro jogador;
- causa dano a outro jogador;
- recebe/causa dano relevante a Guardião;
- recebe/causa dano relevante ao Titã;
- participa de outra interação ofensiva que futuramente seja classificada como combate.

Dano ambiental leve poderá ser tratado separadamente conforme sistema futuro.

## 39.3 VELOCIDADE DE REGENERAÇÃO

A cura natural deve ser contínua, não instantânea.

Referência inicial:
- recuperar alguns % da vida máxima por segundo.

Evitar valor alto demais.

Objetivo:
- após vencer e conseguir espaço, recuperar-se;
- durante perseguição, não conseguir resetar HP facilmente.

Valor exato fica EM TESTE.

## 39.4 REGENERAÇÃO X POÇÃO

São sistemas diferentes.

### Regeneração natural
- somente fora de combate;
- gratuita;
- gradual;
- mais lenta.

### Poção
- pode funcionar durante combate;
- consome recurso/slot;
- regenera mais rapidamente;
- continua importante em situação de pressão.

Assim poção não perde utilidade.

## 39.5 INTERRUPÇÃO

Ao entrar novamente em combate:
- regeneração natural para imediatamente;
- novo atraso começa.

Não remover HP já recuperado.

## 39.6 BOTS

Bots ativos e virtuais seguem a mesma regra conceitual de regeneração fora de combate.

VirtualBots podem calcular regen de forma matemática entre atualizações.

Isso mantém consistência ao materializar um bot novamente.

## 39.7 TITÃ E GUARDIÕES

Atacar Titã/Guardião conta como combate e impede regeneração natural.

Sair da luta e permanecer sem trocar dano pelo tempo exigido permite recuperação.

## 39.8 PRINCÍPIO

> Sobreviver e conseguir criar distância deve permitir recuperação.

Mas:

> fugir por dois segundos no meio da troca não deve resetar a luta inteira.



---

# 40. MAGO — ULTIMATE COMBINADA DE ALTO RISCO

## DIREÇÃO APROVADA

A habilidade combinada de alto dano do Mago ocupa **somente o slot Ultimate**.

Ela não consome dois slots normais.

Nome final ainda não definido.

## Funcionamento

A Ultimate possui duas ativações/fases:

### Primeira ativação
- dispara um projétil lento;
- individualmente causa pouco dano;
- é difícil acertar diretamente em personagens muito móveis.

### Segunda ativação
- dispara um projétil rápido;
- individualmente causa pouco dano.

### Combinação
Se o projétil rápido acertar corretamente o projétil lento:
- ocorre uma explosão de grande área;
- causa dano muito alto;
- recompensa precisão, posicionamento e timing.

A explosão continua precisando possuir contrajogo:
- sair da área;
- interceptar quando compatível;
- usar mobilidade;
- usar terreno;
- outras respostas futuras.

## Cooldown adaptativo por execução

Se a combinação for realizada com sucesso:
- o cooldown da Ultimate fica **maior**.

Se a tentativa falhar:
- o cooldown fica **reduzido** em relação ao sucesso.

Princípio:

> acertou a combinação poderosa → recebe recompensa alta e paga cooldown maior.
>
> errou a execução → causou pouco dano e recupera a oportunidade mais cedo.

Valores exatos de cooldown/dano permanecem EM TESTE.

## Matchups

Contra alvo menos móvel/previsível:
- maior possibilidade de preparar a combinação;
- alto potencial de dano.

Contra Assassino:
- projétil lento é difícil de conectar;
- projéteis perseguidores/controle de outras skills normais devem ser ferramentas mais adequadas;
- a Ultimate continua utilizável por previsão, combinação com mapa e erro do adversário, mas não é resposta automática anti-Assassino.

Essa Ultimate representa a filosofia:
> skill difícil de acertar pode causar muito dano.
