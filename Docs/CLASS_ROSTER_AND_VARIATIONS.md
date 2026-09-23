# Battle Royale X — Classes, Slots e Direção de Habilidades

## Estado

Implementação ativa:
- Guerreiro
- Assassino

Somente design/balanceamento por enquanto:
- Mago
- Arqueiro

A arquitetura antiga de slots rígidos `Defesa / Movimento / Ultimate` foi substituída para a versão final planejada.

---

# ARQUITETURA DA PRIMEIRA VERSÃO

Cada personagem possui:

- Ataque Básico fixo;
- Skill 1;
- Skill 2;
- Ultimate.

Na partida Battle Royale real:
- começa apenas com Ataque Básico;
- Skill 1 começa vazia;
- Skill 2 começa vazia;
- Ultimate começa vazia;
- habilidades são encontradas no chão/eventos.

Não implementar terceiro slot normal agora.

## Skill 1 e Skill 2

Não são categorias rígidas.

Uma habilidade normal pode ser:
- mobilidade;
- defesa;
- ataque;
- controle;
- utilidade;
- híbrida.

O jogador pode montar duas habilidades do mesmo estilo se a classe possuir opções compatíveis.

## Drops universais adaptativos

Skill 1/Skill 2 usam itens adaptativos por classe.

Exemplo:
- `Skill 1 — Variação B`;
- Mago coleta → Skill 1-B do Mago;
- Guerreiro coleta → Skill 1-B do Guerreiro;
- Assassino coleta → Skill 1-B do Assassino;
- Arqueiro coleta → Skill 1-B do Arqueiro.

Ultimate NÃO usa conversão universal.

O jogador precisa encontrar uma Ultimate válida da própria classe.

---

# FILOSOFIA DE BALANCEAMENTO

Nenhuma habilidade deve dizer:
- +X% contra Mago;
- +X% contra Assassino;
- bônus oculto por classe.

O equilíbrio vem de comportamento.

Lei principal:

> Quanto mais fácil acertar, menor a recompensa ofensiva.
>
> Quanto mais difícil acertar, maior pode ser a recompensa.

Referência:
- homing/perseguidora = dano baixo;
- projétil rápido = dano médio;
- projétil lento/telegráfico = dano alto;
- combo de execução difícil = dano muito alto;
- controle fácil de aplicar = dano baixo.

Toda classe deve possuir:
- mobilidade;
- pelo menos uma rota de vitória contra todas as outras;
- respostas baseadas em execução;
- fraquezas exploráveis.

Nenhuma classe possui hard counter automático.

---

# GUERREIRO

## Identidade

- melhor troca frontal;
- ataque pesado;
- cooldown maior;
- defesa por timing;
- forte interação física com projéteis;
- mobilidade menor que Assassino;
- consegue punir entrada ruim.

## Diferencial

Guerreiro deve conseguir **enfrentar certas habilidades diretamente**.

Alguns projéteis serão interceptáveis.

Acertar Ataque Básico/habilidade compatível no projétil no timing correto pode:
- reduzir aproximadamente 60% do dano recebido;
- destruir completamente algumas habilidades específicas;
- desviar outras conforme design futuro.

Não funciona contra tudo.

Skill expression:
- leitura;
- timing;
- decisão de enfrentar ou esquivar.

Guerreiro anti-ranged continua vencível por Mago/Arqueiro habilidosos porque:
- nem toda skill é interceptável;
- cooldowns são maiores;
- ataques lentos são puníveis;
- mobilidade/ângulos podem superar a defesa.

---

# ASSASSINO

## Identidade

- maior mobilidade natural;
- maior velocidade;
- entrada e saída;
- dodge;
- mudança de direção;
- execução;
- mind game.

## Diferencial

Assassino não precisa de forte sistema de interceptação.

Contra projéteis, sua resposta principal é:
- não estar na trajetória;
- dash;
- dodge;
- reposicionamento;
- bait.

Isso diferencia Assassino do Guerreiro.

Fraqueza:
- troca frontal prolongada;
- erro de mobilidade;
- gastar cooldown de fuga no momento errado.

---

# MAGO

## Identidade

Mago deve ser a classe de:
- combinação entre habilidades;
- previsão;
- projéteis;
- controle espacial;
- telegraph;
- mind game.

Também possui mobilidade própria.

Contra alvos muito móveis:
- usa habilidades perseguidoras de dano menor;
- controle de velocidade;
- engano/reposicionamento.

Contra alvos previsíveis/lentos:
- pode arriscar habilidades lentas de dano alto.

## Controle

Pode usar slow/atração/deslocamento.

Não usar stun como base normal da classe.

## Clone / engano — direção aprovada conceitualmente

Uma habilidade futura pode:
- criar 3 clones/projeções;
- permitir ao Mago teleportar para um deles;
- permitir também não teleportar;
- usar clones como mind game e fuga.

Detalhes ainda precisam ser fechados antes da implementação.

## Ultimate combinada de alto dano — definida

A Ultimate ocupa somente o slot Ultimate.

Fase 1:
- projétil lento;
- pouco dano isolado.

Fase 2:
- projétil rápido;
- pouco dano isolado.

Se fase 2 acerta corretamente fase 1:
- grande explosão;
- dano alto;
- área relevante.

Cooldown:
- combinação acertou → cooldown maior;
- tentativa falhou → cooldown reduzido.

Valores ainda serão calibrados.

---

# ARQUEIRO

## Identidade

Arqueiro precisa ser astuto e preparar o confronto.

Foco:
- alcance;
- precisão;
- armadilhas;
- kite;
- controle de rota;
- posicionamento;
- mobilidade inteligente.

Também possui habilidades fáceis/difíceis seguindo a mesma lei:
- perseguidora = dano menor;
- tiro difícil = dano maior.

## Controle

Pode possuir controle sem hard stun.

Direção conceitual forte:

### Armadilha de atração
- colocada no chão;
- inimigo dentro do raio é puxado em direção ao centro;
- não fica paralisado;
- pode continuar andando, usando mobilidade e skills;
- permite ao Arqueiro preparar terreno antes de ser atacado;
- naturalmente forte contra dive/melee;
- pouco útil contra Mago que mantém distância.

Essa habilidade ainda precisa de valores e detalhes antes da implementação.

---

# MATCHUPS

## Guerreiro × Assassino
Referência atual considerada próxima do equilíbrio desejado.

- Guerreiro domina troca direta;
- Assassino domina mobilidade/entrada/saída;
- Guerreiro pode punir entrada;
- Assassino pode evitar ferramentas lentas.

## Guerreiro × Mago
Equilíbrio deve vir de:
- Mago usando ângulos, mobilidade e skills não interceptáveis;
- Guerreiro usando timing para interceptar parte dos projéteis;
- Mago usando slow/homing para pressionar;
- Guerreiro punindo magias lentas/previsíveis quando lê corretamente.

## Guerreiro × Arqueiro
Equilíbrio deve vir de:
- Arqueiro mudando ângulo, trap e kite;
- Guerreiro interceptando alguns disparos;
- Arqueiro usando ataques difíceis de acertar para dano alto;
- Guerreiro tentando fechar distância sem possuir perseguição infinita.

## Assassino × Mago
- Assassino possui mobilidade suficiente para evitar skills lentas;
- Mago usa perseguidoras/slow/clone para criar leitura;
- homing do Mago causa pouco dano;
- Mago precisa sobreviver à aproximação, não impedir aproximação automaticamente.

## Assassino × Arqueiro
- Arqueiro prepara terreno e controla aproximação;
- Assassino tenta quebrar preparação por mobilidade;
- armadilhas/control podem dificultar dive sem stun;
- ranged fácil de acertar não pode causar burst alto.

## Mago × Arqueiro
- luta de espaço, linha, preparação e leitura;
- nenhum deve ser superior apenas por alcance;
- Mago possui combinações e área;
- Arqueiro possui precisão, trap e ângulos.

---

# STATUS

Agora:
- Guerreiro: implementação ativa;
- Assassino: implementação ativa;
- Mago: arquitetura e primeira Ultimate definidas; demais skills em definição;
- Arqueiro: identidade e conceito de armadilha definidos; demais skills em definição.

Não implementar Mago/Arqueiro no runtime até a especificação de suas Skills 1/2 e primeiras variações estar fechada.
