# Battle Royale X — Plano de Assets Gratuitos (2026)

> Objetivo: usar assets gratuitos como matéria-prima, mantendo gameplay e identidade próprios.
> Não commitar pacotes de terceiros no repositório até revisar licença e necessidade.

## Princípio

Usar:
- modelos/rig/animações genéricos prontos;
- cenário modular pronto;
- natureza/props prontos;
- VFX gratuitos como matéria-prima.

Customizar:
- materiais;
- paleta;
- armas;
- VFX;
- timing;
- UI;
- telegraphs;
- objetos arcanos;
- Ultimates;
- feedback de combate.

---

# 1. Personagens base

## Quaternius — Universal Base Characters
Página:
https://quaternius.com/packs/universalbasecharacters.html

Uso:
- base humanoide para Guerreiro, Assassino, Mago e Arqueiro;
- rig comum;
- retarget consistente.

A página específica informa:
- Humanoid rig;
- compatibilidade com Universal Animation Library;
- formatos FBX/glTF;
- licença indicada como CC0 na página do pack.

Destino sugerido:
`Assets/ThirdParty/Quaternius/Characters/Base/`

## Quaternius — Modular Character Outfits - Fantasy
https://quaternius.com/packs/modularcharacteroutfitsfantasy.html

Uso:
- roupas/armaduras;
- construir silhuetas diferentes mantendo mesma família visual.

Sugestão:
- Guerreiro: peças pesadas, ombros/escudo.
- Assassino: roupa leve/capuz.
- Mago: peças de tecido/arcano.
- Arqueiro: couro/leve.

Destino:
`Assets/ThirdParty/Quaternius/Characters/OutfitsFantasy/`

---

# 2. Animações

## Quaternius — Universal Animation Library
https://quaternius.com/packs/universalanimationlibrary.html

## Universal Animation Library 2
https://quaternius.com/packs/universalanimationlibrary2.html

Uso prioritário:
- idle;
- locomotion 8 direções;
- sprint;
- melee;
- dodge;
- parkour/movimento;
- ataques armados.

Destino:
`Assets/ThirdParty/Quaternius/Animations/`

## Adobe Mixamo
https://www.mixamo.com/
FAQ:
https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html

Uso:
- preencher animações específicas ausentes;
- testar rapidamente casts, dodge, slash, bow, reactions.

Adobe informa atualmente que Mixamo é gratuito com Adobe ID e permite uso royalty-free em videogames.

Destino:
`Assets/ThirdParty/Mixamo/`

Não depender de Mixamo para identidade final de timing; retarget e ajustar velocidade/eventos.

---

# 3. Cenário

## Quaternius — Medieval Village MegaKit
https://quaternius.com/packs/medievalvillagemegakit.html

Uso:
- paredes;
- pisos;
- escadas;
- arcos;
- ruínas;
- módulos grid-based;
- base para módulos do mapa futuro.

Destino:
`Assets/ThirdParty/Quaternius/Environment/MedievalVillage/`

## Fantasy Props MegaKit
https://quaternius.com/packs/fantasypropsmegakit.html

Uso:
- armas;
- poções;
- livros;
- props;
- caixas;
- objetos decorativos.

Destino:
`Assets/ThirdParty/Quaternius/Props/Fantasy/`

## Stylized Nature MegaKit
https://quaternius.com/packs/stylizednaturemegakit.html

Uso:
- árvores;
- arbustos/moitas;
- pedras;
- plantas;
- base da Moita Reativa.

Destino:
`Assets/ThirdParty/Quaternius/Nature/`

Observação de licença:
- as páginas específicas acima atualmente rotulam esses packs como CC0;
- o site também publica termos gerais próprios atualizados.
- Antes de release comercial, salvar uma cópia da licença aplicável e registrar em THIRD_PARTY_NOTICES.

---

# 4. UI

## Kenney — UI Pack
https://kenney.nl/assets/ui-pack

Uso:
- painel do laboratório;
- botões;
- barras;
- seleção provisória.

A página informa CC0.

Destino:
`Assets/ThirdParty/Kenney/UI/`

A UI final deve ser customizada depois.

---

# 5. VFX gratuitos

## Magic Effects FREE — Unity Asset Store
https://assetstore.unity.com/packages/vfx/particles/spells/magic-effects-free-247933

Uso como matéria-prima:
- slash;
- hit;
- magia;
- explosão;
- portal;
- partículas.

Compatibilidade publicada inclui URP.

Destino:
`Assets/ThirdParty/UnityAssetStore/MagicEffectsFree/`

## Free Game VFX - Magic Circle (URP)
https://assetstore.unity.com/packages/vfx/particles/free-game-vfx-magic-circle-urp-344984

Uso:
- telegraphs;
- runas;
- slow;
- armadilha;
- Ultimates mágicas.

Destino:
`Assets/ThirdParty/UnityAssetStore/MagicCircleURP/`

## Free Stylized URP Shaders
https://assetstore.unity.com/packages/vfx/shaders/free-stylized-urp-shaders-353190

Uso:
- testes de shader;
- superfícies arcanas;
- estilização.

Destino:
`Assets/ThirdParty/UnityAssetStore/FreeStylizedURPShaders/`

Pacotes do Asset Store seguem a licença/EULA indicada pelo próprio Asset Store; não redistribuir o pacote como asset.

---

# 6. Efeitos que NÃO precisam de asset externo

Criar internamente:
- sangue/fumaça vermelha;
- hit flash;
- block sparks;
- parry ring;
- Clash ring;
- afterimage;
- telegraph simples;
- runa de velocidade;
- indicador de amplificação;
- dissolução de moita.

Esses podem ser montados com:
- Particle System;
- TrailRenderer;
- LineRenderer;
- materiais URP simples;
- meshes primitivas;
- texturas procedurais simples.

---

# 7. Ordem de download/importação

1. Universal Base Characters.
2. Modular Character Outfits - Fantasy.
3. Universal Animation Library 1.
4. Universal Animation Library 2.
5. Medieval Village MegaKit.
6. Fantasy Props MegaKit.
7. Stylized Nature MegaKit.
8. Kenney UI Pack.
9. Magic Effects FREE.
10. Magic Circle URP.
11. Free Stylized URP Shaders.
12. Mixamo somente para animações que realmente faltarem.

Importar e validar um grupo por vez.
Não jogar todos os packs na cena de uma vez.

---

# 8. Checklist de compatibilidade

Para cada asset:
- [ ] importa sem erro em Unity 6;
- [ ] material funciona em URP;
- [ ] escala 1 unidade = 1 metro;
- [ ] humanoid avatar válido;
- [ ] animação retargeta corretamente;
- [ ] sem scripts antigos quebrando compilação;
- [ ] licença registrada;
- [ ] asset utilizado realmente é necessário.

---

# 9. O que o Astra deve fazer

Depois do Sol preparar a arquitetura:
- importar os assets;
- corrigir material rosa/pipeline;
- configurar Humanoid Avatar;
- montar Animator;
- escolher animações que combinam com cada skill;
- ligar VFX prefabs;
- calibrar offsets/escala;
- testar visualmente em Play Mode;
- corrigir problemas específicos da Unity.

O Astra NÃO deve redesenhar gameplay por conta própria.
