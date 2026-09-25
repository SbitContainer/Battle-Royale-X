# Battle Royale X — Especificação Visual/VFX V1

## Objetivo

Criar uma camada visual forte sem alterar as regras de gameplay.

Regra:
> mecânica decide o resultado; VFX comunica o estado.

Todos os efeitos devem ser substituíveis por prefab/profile sem alterar código de combate.

---

# Paleta de classe — referência inicial

## Guerreiro
- aço escuro;
- dourado/quente;
- vermelho queimado;
- VFX largos/pesados;
- poeira/faísca.

## Assassino
- grafite;
- violeta;
- prata fria;
- VFX finos/rápidos;
- afterimage/fumaça.

## Mago
- ciano;
- azul arcano;
- violeta secundário;
- emissivo forte controlado;
- círculos/runas.

## Arqueiro
- âmbar;
- verde frio;
- branco de energia;
- trails limpos;
- geometria precisa.

Evitar saturar a tela.

---

# Camadas obrigatórias de feedback

## Hit físico
- flash de contato;
- faísca;
- reação corporal;
- sangue/fumaça vermelha;
- som curto.

## Hit mágico
- burst emissivo;
- fragmentos/partículas;
- decal/pulso opcional;
- reação corporal.

## Block
- faísca no escudo/arma;
- pulso pequeno;
- som exclusivo.

## Parry
- flash mais limpo e intenso;
- anel rápido;
- hit-stop ligeiramente maior que Block;
- som próprio.

## Clash
- flash central;
- faíscas radiais;
- onda circular;
- micro hit-stop;
- som metálico forte.

## Dodge
- afterimage/trail;
- sem tela cheia;
- personagem permanece rastreável.

## Cura
- partículas discretas ascendentes;
- emissivo curto;
- barra de vida é feedback principal.

## Skill pronta/cooldown
- mudança pequena de ícone/material;
- não usar efeito constante grande no personagem.

---

# Sangue estilizado

Criar internamente, sem depender de pack pago.

Composição:
- spray direcional curto;
- puff de fumaça vermelho escuro;
- dissolve rápido;
- duração alvo ~0,3–0,5 s;
- sem gore realista;
- escala baseada no tipo de impacto, não no dano numérico exato.

Prefabs:
- VFX_Blood_Light
- VFX_Blood_Heavy

---

# Padrão de prefab

Pasta:
`Assets/BattleRoyaleX/Visual/VFX/`

Subpastas:
- Shared
- Warrior
- Assassin
- Mage
- Archer
- World
- Titan

Naming:
- `VFX_Warrior_Basic_Hit`
- `VFX_Assassin_DoubleStep_Trail`
- `VFX_Mage_Convergence_Orb`
- `VFX_Archer_GravityTrap_Field`
- etc.

---

# Perfis visuais

Criar ScriptableObject `AbilityVisualProfile` contendo, no mínimo:
- abilityId;
- castPrefab;
- projectilePrefab;
- impactPrefab;
- areaPrefab;
- trailPrefab opcional;
- AudioClip opcional;
- escala;
- offsets;
- duração;
- cor primária/secundária se útil.

Criar `CharacterVisualProfile`:
- CharacterClass;
- modelPrefab;
- animatorController;
- weapon sockets/prefabs;
- material overrides;
- generic hit VFX;
- blood VFX;
- class aura opcional;
- UI portrait/ícone provisório.

Gameplay nunca deve depender desses assets.

---

# VFX por habilidade

## Guerreiro
- Basic: trail largo + impacto pesado + poeira.
- Guarda Retaliação: escudo + pulso; resposta com linha pesada.
- Parry: flash dourado + anel.
- Guarda Arcana: runas frontais + dissipação.
- Investida: poeira nos pés.
- Caçada: trail de corpo/escudo controlado.
- Avanço Protegido: arco frontal translúcido.
- Bastião: rachadura/onda circular.
- Domínio: pulsos repetidos no chão.
- Ruptura: impacto grande, telegraph evidente.

## Assassino
- Basic: slash violeta fino.
- Esquiva: afterimage + sombra.
- Duplo Passo: dois afterimages diferenciados.
- Contra-Sombra: marca breve + trail especial armado.
- Passo Fantasma: trail direcional.
- Retorno: marca de origem + recolhimento da sombra.
- Caçada: trail longo, sem esconder modelo.
- Execução: sequência de cortes legíveis.
- Predação: afterimages e velocidade.
- Véu Fantasma: clones/eco visual.

## Mago
- Basic: orbe ciano.
- Faíscas: 3 projéteis pequenos.
- Orbe Pesado: esfera grande lenta e pulsante.
- Slow: círculo no chão com fluxo para dentro.
- Blink: distorção + partículas de entrada/saída.
- Ecos: 3 clones translúcidos.
- Repulsão: onda circular para fora.
- Convergência: slow orb + fast lance + explosão combinada.
- Tempestade: telegraph + pulsos.
- Prisma: leque de cristais/projéteis.

## Arqueiro
- Basic: trail limpo.
- Rastreadoras: trails finos diferenciados.
- Pesada: preparação visual + flecha maior.
- Armadilha: dispositivo/runa + campo puxando para centro.
- Recuo: poeira + tiro simultâneo.
- Gancho: linha/cabo estilizado + impacto no ponto.
- Passos Laterais: streak curto.
- Rajada: sequência de muzzle/trails.
- Sobrecarga: aura discreta + trails mais rápidos.
- Ruptura: linha telegráfica + grande impacto.

---

# Objetos do mapa — placeholders visuais

Mesmo antes do mapa final, preparar efeitos testáveis:
- Parede de Fase: shader/transparência;
- Parede Prismática: superfície reflexiva arcana;
- Barreira de Amplificação: plano emissivo que muda projétil;
- Cristal de Fragmentação: cristal + burst;
- Runa de Velocidade: círculo no chão;
- Moita Reativa: efeito de desaparecer/retornar.

Não precisa implementar mapa modular nesta etapa.
Apenas prefabs de demonstração no laboratório visual.

---

# Qualidade/performance

Para protótipo:
- preferir Particle System + URP;
- evitar VFX Graph obrigatório se um Particle System simples resolver;
- usar pooling quando os efeitos começarem a gerar GC;
- limitar luzes reais dinâmicas;
- evitar partículas transparentes gigantes;
- testar com vários efeitos simultâneos.

---

# Ordem visual

1. Hit/Block/Parry/Clash/Blood.
2. Ataques básicos das 4 classes.
3. Mobilidade.
4. Skill 1 A/B/C.
5. Skill 2 A/B/C.
6. Ultimates.
7. Evolução Titânica placeholder.
8. Objetos arcanos do mapa.
9. Som/polish.
