# Battle Royale X — Catálogo de Habilidades V1 para Laboratório

> Objetivo: fornecer ao Codex/Astra uma lista fechada de habilidades para implementar no laboratório.
> Valores são iniciais e editáveis. O foco é validar interação, leitura, mobilidade e VFX.

## Arquitetura

Cada classe possui:
- Ataque Básico fixo;
- Skill 1 com variações A/B/C;
- Skill 2 com variações A/B/C;
- Ultimate com 3 opções específicas da classe.

Na partida BR real:
- começa apenas com Ataque Básico;
- Skill 1/2/Ultimate começam vazias;
- habilidades são encontradas.

No laboratório:
- todas podem ser trocadas instantaneamente;
- sem consumo de item;
- classe pode ser trocada a qualquer momento;
- ao trocar classe, restaurar HP/energia e limpar cooldowns/estados temporários.

Regra universal:
- habilidade fácil de acertar = dano menor;
- habilidade difícil = dano maior;
- homing = baixo dano;
- controle = pouco dano direto;
- sem hard stun em classes normais.

---

# GUERREIRO

Identidade visual/mecânica:
- pesado;
- menor mobilidade;
- cooldowns altos;
- forte interação com projéteis;
- impacto físico.

## Ataque Básico — Corte Pesado
- lento;
- largo;
- alto impacto;
- dano inicial de teste: 18;
- startup: ~0,22 s;
- recovery: ~0,30 s;
- knockback curto;
- SkillLock no alvo: 0,10 s;
- não bloqueia movimento do alvo;
- projéteis marcados Interceptable podem ser atingidos;
- interceptação padrão: reduzir em ~60% o dano do projétil que acertaria o Guerreiro;
- algumas habilidades específicas podem ser destruídas totalmente.

VFX:
- trail largo;
- arco de corte pesado;
- faísca;
- poeira;
- micro shake;
- sangue/fumaça vermelha no acerto.

## Skill 1-A — Guarda de Retaliação
- bloqueio frontal;
- duração curta;
- se realmente bloquear, habilita segundo aperto por ~0,7 s;
- segundo aperto = avanço curto 2,5–3 m + golpe leve;
- contra-ataque ainda pode ser esquivado;
- cooldown alto: referência 9–12 s.

VFX:
- impacto no escudo;
- pulso curto;
- segundo aperto com trail pesado.

## Skill 1-B — Parry
- janela curta ~0,14–0,16 s;
- anula ataque compatível;
- não aplica hard stun;
- habilita resposta curta de maior dano;
- cooldown maior: ~12–14 s.

VFX:
- flash branco/dourado;
- anel de contato;
- hit-stop muito curto.

## Skill 1-C — Guarda Arcana
- defesa frontal especializada contra projéteis/magia;
- menos eficiente contra melee que Guarda de Retaliação;
- pode reduzir fortemente/neutralizar um projétil compatível;
- cooldown longo.

VFX:
- escudo físico + runas;
- arco translúcido frontal;
- dissipação do projétil.

## Skill 2-A — Investida
- dash frontal simples;
- sem tracking relevante;
- alcance moderado;
- dano baixo;
- cooldown ~9–10 s.

## Skill 2-B — Caçada
- perseguição curta;
- corrige direção apenas no início (~0,20–0,25 s);
- depois trava trajetória;
- dano baixo;
- ótima contra mobilidade mal usada;
- esquivável;
- cooldown ~12–14 s.

## Skill 2-C — Avanço Protegido
- avanço moderado;
- proteção frontal durante deslocamento;
- sem iframe total;
- dano baixo;
- bom contra ranged;
- ruim para perseguir Assassino lateralmente.

## Ultimate A — Bastião Sísmico
- impacto circular telegráfico;
- dano moderado;
- knockback;
- depois concede defesa temporária;
- inimigo pode sair/esquivar.

## Ultimate B — Domínio do Caçador
- vários pulsos próximos ao Guerreiro;
- ótimo contra quem insiste em dive;
- pouco valor contra ranged distante;
- dano individual moderado/baixo.

## Ultimate C — Ruptura
- grande golpe de área;
- maior dano entre as Ultimates do Guerreiro;
- telegraph maior;
- cooldown maior;
- alto risco se errar.

---

# ASSASSINO

Identidade:
- maior velocidade;
- maior mobilidade;
- entrada/saída;
- pouco HP;
- não precisa interceptar projéteis; evita.

## Ataque Básico — Corte Rápido
- rápido;
- curto alcance;
- dano moderado;
- ótimo para punir janela;
- Clash físico compatível.

VFX:
- trail fino violeta/prateado;
- slash rápido;
- pouca poeira;
- sangue estilizado.

## Skill 1-A — Esquiva Sombria
- dash/esquiva curta;
- iframe curto;
- se atravessar/acertar muito próximo do alvo, dano leve-moderado;
- dano maior que o Duplo Passo;
- cooldown médio.

## Skill 1-B — Duplo Passo
- primeiro deslocamento na direção atual do analógico;
- esperar ~0,20 s;
- ler analógico novamente;
- segundo deslocamento independente;
- cada avanço causa dano leve se conectar;
- excelente evasão;
- recompensa menor por dano.

## Skill 1-C — Contra-Sombra
- esquiva precisa;
- se usada no timing correto, arma um contra-ataque;
- próximo ataque ganha propriedade mecânica/punição;
- evitar multiplicador bruto exagerado.

## Skill 2-A — Passo Fantasma
- dash rápido;
- reposicionamento puro;
- distância boa;
- cooldown baixo/médio.

## Skill 2-B — Retorno
- primeiro uso: dash e marca de origem;
- janela curta;
- segundo uso: retorna à marca;
- pode optar por não retornar;
- foco em bait.

## Skill 2-C — Caçada
- dash perseguidor com pequena correção;
- dano baixo;
- boa ferramenta de contato;
- não deve ganhar tracking infinito.

## Ultimate A — Execução
- sequência ofensiva curta;
- maior dano;
- menor mobilidade;
- claramente esquivável/bloqueável;
- recuperação alta se errar.

## Ultimate B — Predação
- dano por golpe menor;
- mais deslocamentos;
- mobilidade muito alta durante janela;
- foco em perseguição.

## Ultimate C — Véu Fantasma
- foco em engano;
- afterimages/clones visuais;
- ocultação visual parcial, nunca invisibilidade longa perfeita;
- dano menor;
- reposicionamento imprevisível.

---

# MAGO

Identidade:
- combina habilidades;
- previsão;
- projéteis;
- controle de espaço;
- mobilidade própria;
- slow em vez de stun.

## Ataque Básico — Orbe Arcano
- projétil mágico rápido/médio;
- dano moderado-baixo;
- sem tracking;
- colide com magia compatível;
- reflectable/amplifiable conforme mapa.

VFX:
- núcleo ciano;
- trail arcano;
- pequeno burst no impacto.

## Skill 1-A — Faíscas Caçadoras
- 3 pequenos projéteis perseguidoras;
- tracking moderado;
- dano individual baixo;
- objetivo: pressionar mobilidade e consumir dodge;
- bom contra Assassino;
- ruim para burst.

## Skill 1-B — Orbe Pesado
- projétil grande e lento;
- alto dano se acertar;
- fácil de esquivar;
- bom contra alvos comprometidos/previsíveis;
- interceptable pelo Guerreiro.

## Skill 1-C — Campo de Lentidão
- área no chão;
- reduz velocidade ~25–30% enquanto dentro;
- sem stun;
- dano baixo ou zero;
- ótimo para controlar dive/rotas.

## Skill 2-A — Blink
- teleport curto na direção escolhida;
- sem dano;
- cooldown médio;
- reposicionamento generalista.

## Skill 2-B — Ecos Arcanos
- cria 3 clones/projeções;
- durante janela curta pode selecionar/teleportar para um clone;
- também pode optar por não teleportar;
- clones não causam burst;
- foco em mind game.

## Skill 2-C — Pulso de Repulsão
- área curta ao redor;
- dano baixo;
- empurra;
- sem hard stun;
- bom contra dive;
- fraco contra ranged distante.

## Ultimate A — Convergência Arcana
Ultimate combinada definida:
1. primeira ativação dispara projétil lento com pouco dano;
2. segunda ativação dispara projétil rápido com pouco dano;
3. se o rápido atingir corretamente o lento: grande explosão e dano alto.

Cooldown:
- combo acertou: cooldown maior;
- combo falhou: cooldown reduzido.

VFX:
- orbe lento grande e instável;
- lança rápida brilhante;
- ao combinar: implosão curta + explosão ampla + onda no chão.

## Ultimate B — Tempestade Arcana
- área telegráfica;
- vários pulsos;
- inimigo continua podendo sair;
- dano total alto apenas se permanecer dentro.

## Ultimate C — Prisma Fraturado
- múltiplos projéteis/lâminas arcanas em leque;
- cria ângulos;
- alguns podem refletir uma vez em superfícies compatíveis;
- dano por projétil moderado;
- foco em leitura geométrica.

---

# ARQUEIRO

Identidade:
- astuto;
- prepara terreno;
- precisão;
- alcance;
- kite;
- controle por atração/slow, sem stun.

## Ataque Básico — Disparo Preciso
- projétil físico rápido;
- longo alcance;
- dano moderado-baixo;
- sem tracking;
- precisa mirar;
- interceptable em alguns casos pelo Guerreiro.

VFX:
- trail curto claro;
- impacto seco;
- pequeno puff/fragmento.

## Skill 1-A — Flechas Rastreadoras
- 2–3 flechas com correção de trajetória;
- dano baixo;
- pressão contra mobilidade;
- não serve para burst.

## Skill 1-B — Flecha Pesada
- carregamento/startup maior;
- projétil mais lento;
- dano alto;
- ótima contra alvo previsível;
- difícil contra Assassino;
- interceptable.

## Skill 1-C — Armadilha Gravitacional
- coloca dispositivo no chão;
- inimigos no raio são puxados gradualmente ao centro;
- não prende;
- podem andar, dashar e usar skills;
- dano baixo;
- forte contra dive;
- pouco útil contra Mago distante.

## Skill 2-A — Recuo Ofensivo
- deslocamento para trás;
- dispara flecha leve simultaneamente;
- foco em criar distância;
- trajetória previsível.

## Skill 2-B — Gancho de Reposição
- puxa o Arqueiro até ponto válido;
- alcance maior;
- exige superfície/ponto válido;
- cooldown alto;
- foco em domínio do mapa.

## Skill 2-C — Passos Laterais
- dois pequenos deslocamentos;
- segundo lê direção novamente após ~0,20 s;
- pouca ou nenhuma invulnerabilidade;
- ótimo para mudar ângulo de tiro.

## Ultimate A — Rajada Perfurante
- sequência de tiros;
- permite correção de mira entre disparos;
- alto dano se vários acertarem;
- sem auto-aim forte.

## Ultimate B — Sobrecarga Cinética
- aumenta velocidade de movimento;
- aumenta cadência/preparação;
- NÃO aumenta dano bruto por flecha;
- foco em kite.

## Ultimate C — Disparo de Ruptura
- tiro muito telegráfico;
- alto dano;
- pode atravessar/destruir uma interação/projétil compatível;
- perde força depois de romper;
- alto risco.

---

# EVOLUÇÃO TITÂNICA

A Evolução Titânica acompanha o slot Ultimate.

Cada Ultimate acima deve ter uma versão Titânica futura.
Para o laboratório visual:
- preparar um hook/estado `isTitanEvolved`;
- VFX pode trocar material/emissivo/escala;
- NÃO implementar números finais de evolução sem design específico.

---

# TESTES DE LABORATÓRIO

O painel deve permitir:
- trocar classe entre Guerreiro / Assassino / Mago / Arqueiro;
- trocar Skill 1 A/B/C;
- trocar Skill 2 A/B/C;
- trocar Ultimate A/B/C;
- resetar HP/energia/cooldown;
- ativar/desativar Evolução Titânica para teste visual;
- testar contra Guerreiro dummy/bot inicialmente.

Cada troca deve ser possível sem reiniciar a Unity.
