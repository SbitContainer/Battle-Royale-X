# Assets a buscar prontos — Prototype 01

Não comprar nada antes de validar a cena com cápsulas.

## Personagens
Objetivo: dois humanoides stylized dark fantasy compatíveis com rig Humanoid.
- Guerreiro: silhueta larga, armadura média/pesada, espada + escudo.
- Assassino: silhueta fina, roupa leve, duas lâminas/adagas.

Prioridade de origem:
1. Unity Asset Store / Fab — packs stylized coerentes entre si.
2. Mixamo — personagem provisório se necessário.

## Animações
Mixamo como primeira fonte:
- idle;
- walk/run;
- sword attacks;
- dual wield attacks;
- block;
- dodge/roll;
- hit reaction;
- death.

Ajustar velocidades e transições na Unity. Não deixar root motion decidir alcance do gameplay; o `CharacterMotor25D` é a referência lógica.

## Cenário
Pacote modular de ruínas stylized:
- piso de pedra;
- pilares;
- blocos/ruínas;
- paredes baixas;
- props discretos.

Evitar cenário visualmente complexo até validar legibilidade.

## VFX
Usar packs URP stylized como matéria-prima:
- slash trails;
- sparks;
- hit flash;
- shield/block;
- smoke;
- shockwave;
- arcane circle;
- null/dispersion effect.

Customizar cor, escala, duração e timing para cada `CombatEventKind`.

## UI
Kenney ou UI simples provisória:
- barras de HP/energia;
- 4 slots de inventário;
- ícones de habilidades;
- cooldown radial/numérico.

## Áudio
Bibliotecas royalty-free / Asset Store:
- sword swing;
- metal clash;
- shield impact;
- dash whoosh;
- magic nullify;
- pickup;
- potion;
- airdrop impact.

O som de Clash e Parry precisa ser muito distinto do Hit comum.
