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

# 6. SISTEMA DE KITS E VARIAÇÕES

Cada classe possui:

- Ataque Básico fixo;
- Defesa;
- Movimento;
- Ultimate.

Ataque Básico nunca recebe variações.

Defesa, Movimento e Ultimate recebem variações.

O jogador pode combinar variações de famílias diferentes.

Exemplo:

- Defesa de um kit;
- Movimento de outro;
- Ultimate de outro.

Portanto os "kits" são famílias de identidade, não loadouts obrigatórios.

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

Antes de transformar as variações em loot definitivo, o jogo deve possuir um modo de teste.

Nesse laboratório:

- bot/adversário permanece Guerreiro;
- jogador pode testar Guerreiro ou Assassino;
- variações podem ser trocadas livremente;
- não precisam ser encontradas;
- não consomem runa;
- Defesa Base/A/B pode ser alternada;
- Movimento Base/A/B pode ser alternado;
- Ultimate Base/A/B pode ser alternada.

Objetivo:

> descobrir se a habilidade é divertida e balanceável antes de colocá-la dentro da economia de loot.

Depois da validação, o sistema final volta a usar descoberta/loot.

---

# 9. LOOT FUTURO

A maior parte do loot de chão deve ser composta por:

## Variações de habilidade
Principal forma de progressão dentro da partida.

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

# 10. VARIAÇÕES COMO "ARMAS" DO BATTLE ROYALE

As variações de habilidade devem cumprir o papel que armas diferentes cumprem em Battle Royales tradicionais.

O jogador encontra no mapa opções diferentes e decide:

- equipar;
- guardar;
- trocar;
- abandonar;
- adaptar o kit ao matchup atual.

Uma partida pode começar com uma intenção de build e terminar com outra conforme o loot encontrado.

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

## Funcionamento

Uma pequena região do mapa fica destacada como área de combate de alto valor.

Dentro dela:

- eliminações têm aproximadamente 50% de chance de gerar um Drop de Combate;
- o loot não vai diretamente para quem matou;
- ele cai fisicamente no chão;
- outros jogadores podem disputar;
- o drop pode conter habilidade de qualquer classe.

Objetivo:

> quem quer acelerar a construção do próprio kit precisa se expor a combate.

---

# 14. DROP DE COMBATE

A chance de 50% refere-se inicialmente à chance de gerar um Drop de Combate.

O Drop de Combate pode conter:

- habilidade/variação;
- cura especial;
- redução de cooldown;
- tático;
- outro recurso raro futuro.

A distribuição exata será calibrada em testes.

Uma referência possível:

- maioria dos drops especiais = habilidade;
- minoria = consumível/tático de alto valor.

Não tratar esses percentuais como balanceamento final.

---

# 15. HABILIDADE DE CLASSE ALEATÓRIA

O Drop de Combate pode gerar habilidade de qualquer classe.

Exemplo:

- Assassino faz uma eliminação;
- cai habilidade de Guerreiro.

Isso gera novos pontos de disputa.

O jogador pode:

- pegar;
- deixar;
- guardar se as regras futuras permitirem;
- chamar companheiro;
- usar o drop como isca.

Visualmente, o drop deve comunicar imediatamente a classe da habilidade.

Paletas provisórias:

- Guerreiro: tons quentes / aço / dourado;
- Assassino: violeta / grafite;
- Mago: azul/ciano/arcano;
- Arqueiro: verde/dourado ou outra paleta final futura.

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

# 21. ATAQUE BÁSICO E SKILLS

Regra de controle:

- ataque básico não executa simultaneamente com skill;
- skills possuem prioridade;
- se uma skill for ativada durante ataque básico, o ataque básico é cancelado;
- a skill deve responder no mesmo input;
- ataques pesados podem aplicar micro SkillLock;
- movimento não deve ser bloqueado pelo micro SkillLock.

Objetivo:
combate responsivo sem animações/estados brigando entre si.

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

