# ASTRA TASK — Battle Royale X Visual Integration V1

> Execute somente depois que `CODEX_TASK.md` estiver DONE/PARTIAL com a base compilável.
> Quando o usuário disser **"leia ASTRA_TASK.md e execute"**, atualize `main` e siga esta ordem.

# Objetivo

Usar Unity real para:
- compilar/corrigir integração;
- importar assets gratuitos;
- montar 4 personagens;
- retargetar animações;
- conectar VFX;
- deixar laboratório visualmente bom;
- não redesenhar gameplay.

# Referências

Leia:
- `CODEX_TASK.md` e relatório final;
- `Docs/ABILITY_CATALOG_V1_TEST.md`;
- `Docs/VFX_VISUAL_SPEC_V1.md`;
- `Docs/FREE_ASSET_SOURCING_2026.md`;
- `Docs/PRODUCTION_BUILD_PLAN_V1.md`;
- `Docs/COMBAT_PRINCIPLES.md`.

# FASE A — COMPILAÇÃO

1. Pull `main`.
2. Abrir em Unity 6 URP.
3. Corrigir somente erros reais de API/versão.
4. Não alterar valores/design sem bug concreto.
5. Executar geradores de dados/cena/VFX.

# FASE B — DOWNLOAD/IMPORT DE ASSETS

Importar nesta ordem:

1. Quaternius Universal Base Characters
   https://quaternius.com/packs/universalbasecharacters.html

2. Quaternius Modular Character Outfits - Fantasy
   https://quaternius.com/packs/modularcharacteroutfitsfantasy.html

3. Universal Animation Library
   https://quaternius.com/packs/universalanimationlibrary.html

4. Universal Animation Library 2
   https://quaternius.com/packs/universalanimationlibrary2.html

5. Medieval Village MegaKit
   https://quaternius.com/packs/medievalvillagemegakit.html

6. Fantasy Props MegaKit
   https://quaternius.com/packs/fantasypropsmegakit.html

7. Stylized Nature MegaKit
   https://quaternius.com/packs/stylizednaturemegakit.html

8. Kenney UI Pack
   https://kenney.nl/assets/ui-pack

9. Magic Effects FREE
   https://assetstore.unity.com/packages/vfx/particles/spells/magic-effects-free-247933

10. Free Game VFX - Magic Circle (URP)
   https://assetstore.unity.com/packages/vfx/particles/free-game-vfx-magic-circle-urp-344984

11. Free Stylized URP Shaders
   https://assetstore.unity.com/packages/vfx/shaders/free-stylized-urp-shaders-353190

12. Mixamo somente se faltar animação específica:
   https://www.mixamo.com/

Se download exigir login/ação humana, interromper somente aquele download, continuar com os demais e registrar.

# FASE C — PERSONAGENS

Usar mesma família visual/rig.

Warrior:
- corpo mais largo;
- armadura pesada;
- espada + escudo.

Assassin:
- silhueta estreita;
- dual blades;
- roupa leve/capuz opcional.

Mage:
- silhueta média;
- staff/orb;
- detalhes arcanos.

Archer:
- silhueta leve;
- bow;
- couro/tecido.

Configurar Humanoid Avatar e retarget.

# FASE D — ANIMATOR

Mínimo por classe:
- Idle;
- Move/Run;
- BasicAttack;
- Skill1;
- Skill2;
- Ultimate;
- HitReaction;
- Death.

Usar Animator Override/estrutura genérica quando possível.

Gameplay timing prevalece.
Não deslocar hitbox para encaixar animação errada; ajuste animation speed/event.

# FASE E — VFX

Seguir `VFX_VISUAL_SPEC_V1.md`.

Prioridade:
1. Hit/Blood/Block/Parry/Clash.
2. Basic das 4 classes.
3. Skill1.
4. Skill2.
5. Ultimates.
6. Titan Evolved visual placeholder.
7. objetos arcanos.

Reaproveitar efeitos gratuitos, mas:
- recolorir;
- redimensionar;
- combinar camadas;
- alterar timing;
- não deixar aparência de pack genérico.

# FASE F — VFX CRÍTICOS

Criar/ajustar especialmente:

Mage Convergência:
- slow orb;
- fast lance;
- explosão combinada muito legível.

Mage Clones:
- três clones reconhecíveis;
- não confundir clone com personagem real de forma ilegível.

Archer Gravity Trap:
- centro/raio claros;
- partículas puxando para dentro.

Assassin:
- afterimages;
- trails;
- Retorno com marca clara.

Warrior:
- interceptação de projétil;
- Parry;
- impacto pesado.

# FASE G — ÁUDIO PROVISÓRIO

Se houver áudio gratuito já importado, ligar:
- Hit;
- Block;
- Parry;
- Clash;
- Dash;
- Magic cast;
- Explosion.

Não gastar tempo procurando biblioteca premium.

# FASE H — ARENA VISUAL

Substituir cubos/plane por uma pequena arena estilizada usando:
- Medieval Village;
- Nature;
- Fantasy Props.

Manter leitura de gameplay.

Incluir:
- moitas;
- ruínas;
- objetos arcanos de demonstração.

Não construir mapa BR completo.

# FASE I — UI

Usar Kenney somente como base provisória.

Painel:
- classe;
- Skill1 A/B/C;
- Skill2 A/B/C;
- Ultimate A/B/C;
- cooldowns;
- HP;
- energia;
- Titan Evolved toggle.

# FASE J — PLAY MODE

Testar todas as combinações pelo menos uma vez.

Foco:
- bugs;
- material rosa;
- escala;
- Animator;
- VFX fora de posição;
- projétil invisível;
- hitbox/visual desalinhado;
- performance.

# REGRA DE TOKENS

Não fazer pesquisa/redesign extensa.

Se uma animação não encaixa:
- escolha a mais próxima;
- ajuste velocidade;
- registre a pendência.

Se um VFX não existe:
- use placeholder existente;
- não redesenhe a habilidade.

# ENTREGA

Ao final:
- commit das correções/integração que podem ser versionadas;
- lista de assets importados;
- licenças/fontes registradas;
- testes que passaram;
- pendências;
- screenshots se a ferramenta suportar;
- não avançar para multiplayer/mapa final.
