# Battle Royale X — Roster, Variações e Escopo de Teste

## Estado do protótipo

Implementação ativa agora:
- Guerreiro
- Assassino

Registrados apenas para orientar balanceamento futuro:
- Mago
- Arqueiro

Durante a fase de laboratório:
- o bot permanece sempre Guerreiro;
- o jogador pode trocar a classe no app/seletor de teste;
- apenas Guerreiro e Assassino precisam estar jogáveis nesta etapa;
- Mago e Arqueiro podem aparecer como "design registrado / ainda não implementado";
- Defesa, Movimento e Ultimate podem ser trocados livremente no modo de teste, sem depender de runa, consumo ou drop;
- a regra final de runas volta somente depois que os matchups forem validados.

## Filosofia

Nenhuma variação deve dizer "causa mais dano contra classe X".

As variações respondem a comportamentos:
- perseguição contra mobilidade;
- defesa de projétil contra ranged;
- área próxima contra dive;
- avanço protegido contra pressão frontal;
- parry contra ações previsíveis;
- mobilidade contra golpes lentos;
- negação de projétil contra magia/tiros.

Isso permite kits especializados e híbridos sem criar hard counters.

---

# GUERREIRO

Identidade:
- melhor troca frontal;
- defesa mais consistente;
- ataques pesados;
- cooldowns maiores;
- mobilidade menor;
- consegue punir quem entra mal;
- não deve perseguir indefinidamente quem decidiu fugir.

Fraquezas:
- alcance;
- mobilidade;
- cooldown alto;
- ataques previsíveis;
- pode sofrer contra zonamento e projéteis se escolher kit anti-melee.

Especializações futuras:
- anti-mobilidade: Retaliação + Caçada + Ultimate de domínio próximo;
- anti-ranged: Guarda Arcana + Avanço Protegido + Bastião Arcano;
- híbrido: combinação livre entre as famílias.

---

# ASSASSINO

Identidade:
- maior velocidade base;
- maior mobilidade;
- entrada e saída;
- esquiva como principal defesa;
- alto risco se permanecer em troca frontal;
- jogador experiente deve conseguir escolher quando a luta acontece.

Fraquezas:
- pouca resistência;
- perde valor se ficar preso em combate prolongado;
- erro de esquiva precisa ser punível;
- não pode entrar repetidamente sem correr risco real contra Guerreiro preparado.

Especializações futuras:
- counter ofensivo;
- dupla esquiva direcional;
- travessia;
- retorno;
- execução;
- caçada/mobilidade.

---

# MAGO — DESIGN REGISTRADO, NÃO IMPLEMENTAR AINDA

## Identidade

Mago controla espaço por:
- projéteis;
- áreas telegráficas;
- colisões mágicas;
- previsão;
- negação de rota.

Não possui hard CC.

O Mago não prende o inimigo; ele força decisões de posicionamento.

Forças:
- melhor controle de área;
- alto valor quando prevê movimento;
- ameaça forte em média distância;
- pode colidir magia contra magia;
- pune Guerreiro que avança de forma previsível.

Fraquezas:
- pressão corpo a corpo;
- cooldowns importantes;
- projéteis podem ser bloqueados, anulados, refletidos ou colidir;
- mobilidade apenas moderada;
- precisa antecipar o Assassino, não reagir tarde.

## Ataque básico — Orbe Arcano

- projétil mágico de velocidade média;
- dano moderado;
- colide com outros projéteis mágicos;
- pode ser bloqueado/anulado/refletido conforme a defesa;
- sem variações;
- leitura visual muito clara.

Objetivo:
servir como ferramenta constante de pressão e de criação de colisões mágicas, não como burst gratuito.

## Defesa base — Barreira Arcana

- proteção curta;
- reduz dano frontal/compatível;
- não é invulnerabilidade;
- duração curta;
- cooldown médio.

Uso:
generalista.

## Defesa A — Espelho Prismático

Perfil:
anti-projétil / anti-Mago / anti-Arqueiro.

- janela curta;
- primeiro projétil compatível recebido é refletido;
- reflexão entra em cooldown especial;
- proteção física corpo a corpo é pior que a Barreira Base.

Fraqueza:
Assassino que chega por perto continua perigoso.

## Defesa B — Pulso de Repulsão

Perfil:
anti-dive / anti-Assassino / anti-Guerreiro próximo.

- janela curta de defesa;
- se receber ataque compatível em alcance curto, gera pulso de repulsão;
- empurra sem stun;
- dano baixo ou inexistente;
- não é boa resposta contra inimigo mantendo distância.

Fraqueza:
ranged pode simplesmente continuar atacando de fora.

## Movimento base — Blink

- deslocamento curto instantâneo/rapidíssimo para a direção escolhida;
- sem dano;
- cooldown médio;
- não atravessa mapa inteiro.

Uso:
reposicionamento generalista.

## Movimento A — Blink Longo

Perfil:
anti-melee / fuga.

- distância maior;
- cooldown bem maior;
- pequeno compromisso visual antes/depois;
- ótimo para criar espaço de Guerreiro/Assassino.

Fraqueza:
se usado cedo, o Mago fica sem sua principal saída.

## Movimento B — Âncora Arcana

Perfil:
mind game / híbrido.

- primeiro uso cria âncora e faz Blink curto;
- segundo uso dentro da janela retorna à âncora;
- retorno claramente sinalizado no chão;
- pode ser previsto e punido.

Uso:
bom contra perseguição, mas não é fuga garantida.

## Ultimate base — Tempestade Arcana

- área telegráfica;
- múltiplos pulsos de dano;
- nenhum stun;
- inimigo pode sair da área;
- excelente para negar espaço.

## Ultimate A — Núcleo Meteórico

Perfil:
dano concentrado / anti-alvo previsível.

- grande telegraph no chão;
- atraso maior;
- explosão única forte;
- área menor que Tempestade;
- Guerreiro parado em Guarda ou avançando previsivelmente corre risco;
- Assassino atento deve conseguir esquivar.

## Ultimate B — Anéis Arcanos

Perfil:
anti-dive / controle por dano.

- cria 2–3 anéis/ondas sucessivas ao redor do Mago;
- cada onda causa dano moderado;
- nenhuma impede movimento;
- entrar no timing errado é perigoso;
- inimigo pode esperar do lado de fora.

Observação:
as duas variações de Ultimate continuam orientadas a dano, como definido originalmente.

## Kits emergentes do Mago

Anti-melee:
- Pulso de Repulsão
- Blink Longo
- Anéis Arcanos

Anti-ranged:
- Espelho Prismático
- Âncora Arcana
- Núcleo Meteórico

Generalista:
- Barreira Arcana
- Blink
- Tempestade Arcana

Híbridos são permitidos.

---

# ARQUEIRO — DESIGN REGISTRADO, NÃO IMPLEMENTAR AINDA

## Identidade

Arqueiro é o especialista em alcance e precisão.

Forças:
- maior alcance consistente;
- projétil rápido;
- pressão contínua;
- excelente quando mantém distância;
- consegue castigar movimentação previsível.

Fraquezas:
- frágil;
- defesa limitada;
- pior quando Assassino/Guerreiro chegam perto;
- não deve ganhar troca corpo a corpo;
- depende fortemente de posicionamento.

## Ataque básico — Disparo Preciso

- projétil físico rápido;
- alcance longo;
- dano moderado;
- cadência controlada;
- sem variações;
- pode ser bloqueado ou interceptado;
- não possui tracking.

## Defesa base — Rolamento Reativo

- esquiva curta;
- pequena janela de invulnerabilidade;
- distância menor que a esquiva do Assassino;
- cooldown médio.

Uso:
generalista.

## Defesa A — Tiro de Interceptação

Perfil:
anti-projétil / anti-Mago / anti-Arqueiro.

- pequena janela de precisão;
- dispara um tiro defensivo;
- se acertar projétil compatível, destrói/intercepta;
- não fornece boa defesa contra ataque corpo a corpo;
- cooldown especial impede spam.

## Defesa B — Recuo Ofensivo

Perfil:
anti-dive.

- salto/deslocamento curto para trás no plano;
- dispara um tiro leve na direção oposta ao recuo;
- dano baixo;
- sem stun;
- Assassino pode ler a direção e continuar perseguindo.

## Movimento base — Dash Tático

- deslocamento médio;
- direção livre;
- sem dano;
- cooldown médio.

## Movimento A — Gancho de Reposição

Perfil:
escape / reposicionamento de longo alcance.

- puxa rapidamente o Arqueiro até um ponto válido no chão;
- sem elevação;
- alcance maior que Dash Tático;
- cooldown alto;
- trajetória/telegraph visível;
- pode ser lido por quem persegue.

## Movimento B — Passos Laterais

Perfil:
duelo ranged / evasão.

- duas pequenas cargas laterais;
- segunda direção pode ser escolhida separadamente;
- distância total menor que Gancho;
- ótimo para alterar ângulo de tiro;
- não atravessa grandes distâncias.

## Ultimate base — Rajada Perfurante

- sequência curta de tiros alinhados;
- exige mira;
- sem auto-aim;
- dano alto se vários acertarem;
- jogador mantém controle e pode esquivar.

## Ultimate A — Disparo Perfurante

Perfil:
dano concentrado.

- um disparo muito longo e muito telegráfico;
- alto dano;
- atravessa alvos/efeitos compatíveis conforme regras futuras;
- errou = grande perda de oportunidade;
- não é morte inevitável.

## Ultimate B — Sobrecarga Cinética

Perfil:
mobilidade + cadência.

- aumenta velocidade de movimento;
- aumenta velocidade/cadência de ataque;
- NÃO aumenta o dano bruto de cada tiro;
- duração curta;
- serve para kite e reposicionamento.

## Kits emergentes do Arqueiro

Anti-Mago/ranged:
- Tiro de Interceptação
- Passos Laterais
- Disparo Perfurante

Anti-dive:
- Recuo Ofensivo
- Gancho de Reposição
- Sobrecarga Cinética

Generalista:
- Rolamento Reativo
- Dash Tático
- Rajada Perfurante

Híbridos são permitidos.

---

# Regra de balanceamento entre as quatro classes

Não existe hard counter.

Uma especialização pode criar vantagem de ferramentas, nunca vitória automática.

Alvo de design:
- Guerreiro domina troca frontal, mas sofre para alcançar;
- Assassino domina escolha de distância/entrada, mas sofre se errar a entrada;
- Mago domina espaço, mas sofre sob pressão próxima;
- Arqueiro domina alcance, mas sofre quando encurralado.

Toda classe deve possuir:
- pelo menos uma rota de vitória contra cada outra;
- pelo menos uma fraqueza explorável;
- respostas baseadas em leitura e execução;
- cooldowns que impeçam defesa perfeita permanente.

## Status de implementação

Agora:
- Guerreiro: desenvolver e rebalancear.
- Assassino: desenvolver, manter identidade e ajustar detalhes.
- Mago: somente documentação.
- Arqueiro: somente documentação.

Não implementar Mago/Arqueiro até o 1x1 Guerreiro x Assassino estar estável o suficiente para servir como referência mecânica.
