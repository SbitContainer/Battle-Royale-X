# Battle Royale X — Prototype 01

Pacote de código para validar o combate local 2.5D entre **Guerreiro** e **Assassino** antes de investir créditos do Astra/Codex em integração visual e multiplayer.

## Objetivo

O pacote deixa pronto o máximo possível de lógica determinística:

- movimento 2.5D;
- vida e energia;
- ataque básico;
- hitbox / hurtbox;
- Clash físico x físico;
- colisão magia x magia com explosão em área;
- ataque físico capaz de anular magia com cooldown especial de interação;
- Guard, Parry, Dodge, Nullify e Reflect;
- habilidades data-driven via ScriptableObject;
- variantes de Defesa, Movimento e Ultimate;
- troca consumível de variantes durante combate;
- mochila de 3 slots e upgrade para 4;
- cura, essência, redução de cooldown;
- fumaça, repulsão, barreira e campo nulo;
- loot no chão;
- airdrop com três escolhas;
- câmera 2.5D em perspectiva;
- gerador de dados e cena de teste dentro do Editor.

## Compatibilidade pretendida

- Unity 6.x
- Projeto 3D/URP recomendado
- Código sem dependências de assets de terceiros

### Entrada temporária

O protótipo usa `UnityEngine.Input` para reduzir dependências. No Unity, deixe **Active Input Handling = Both** ou **Input Manager (Old)** durante o teste inicial. Depois o Astra pode substituir `PrototypeLocalInput` pelo Input System oficial sem alterar o núcleo de combate.

## Instalação

1. Crie um projeto Unity 6 3D/URP vazio.
2. Copie a pasta `Assets/BattleRoyaleX` deste pacote para a pasta `Assets` do projeto.
3. Aguarde a compilação.
4. No menu da Unity execute:
   - `Battle Royale X > Prototype 01 > Create Default Data`
   - `Battle Royale X > Prototype 01 > Build Test Scene`
5. Abra/rode `Assets/BattleRoyaleX/GeneratedScenes/Prototype01_Arena.unity`.
6. Pressione Play.

## Controles locais

### Player 1 — Guerreiro
- Movimento: WASD
- Ataque: F
- Defesa: G
- Movimento especial: H
- Ultimate: R
- Inventário: 1 / 2 / 3 / 4

### Player 2 — Assassino
- Movimento: setas
- Ataque: Numpad 1
- Defesa: Numpad 2
- Movimento especial: Numpad 3
- Ultimate: Numpad 0
- Inventário: Numpad 4 / 5 / 6 / 7

## Estrutura

```text
Assets/BattleRoyaleX/
  Runtime/
    Core/          enums, eventos e tipos comuns
    Data/          ScriptableObjects
    Characters/    vida, energia, motor e runtime
    Combat/        hitbox, hurtbox, resolver e interações
    Abilities/     execução e troca de habilidades
    Inventory/     mochila, consumíveis e pickups
    World/         airdrop e itens táticos
    Camera/        câmera 2.5D
    Debug/         HUD provisório
  Editor/
    PrototypeDataFactory.cs
    PrototypeSceneBuilder.cs
```

## Regra arquitetural

A arte não decide gameplay. O sistema deve funcionar com cápsulas. Modelos, Animator, VFX, som e UI entram como camadas de apresentação sobre estados e eventos já existentes.

`CombatEvents` é o ponto principal para VFX/SFX futuros: Clash, Parry, Block, Hit, Dodge, Nullify etc. não dependem diretamente de partículas ou animações.

## Pontos ainda intencionalmente provisórios

- números de balanceamento;
- animações e Animator Controller;
- VFX/SFX;
- UI final;
- targeting por mouse/controller;
- multiplayer/network prediction;
- sistema de visão real da fumaça;
- navegação/IA;
- autoridade de servidor.

Esses pontos devem ser adicionados depois de validar o feel do 1x1 local.

---

## Codex / Astra continuation

When continuing this project with Codex/Astra, begin with [`Docs/CODEX_START_HERE.md`](Docs/CODEX_START_HERE.md).

The latest combat philosophy, including the **no hard-CC / continuous player control** rule, is recorded in [`Docs/COMBAT_PRINCIPLES.md`](Docs/COMBAT_PRINCIPLES.md).

Full design exports from the project Drive are versioned under [`Docs/Design/`](Docs/Design/).
