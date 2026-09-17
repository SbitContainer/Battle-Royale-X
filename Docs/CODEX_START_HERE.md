# Codex / Astra — Start Here

This repository is the handoff package for **Battle Royale X — Prototype 01 (Warrior vs Assassin)**.

## First objective

Do not expand scope. First make the existing local 1v1 prototype compile and run correctly in Unity using placeholder capsules.

## Required order

1. Read `README.md`.
2. Read `Docs/COMBAT_PRINCIPLES.md`.
3. Read `Docs/ASTRA_HANDOFF.md` and `Docs/Design/03_ASTRA_HANDOFF_CODEPACK.md`.
4. Open/import the project into the agreed Unity version and URP.
5. Fix only real compile/API compatibility errors; preserve architecture and design semantics.
6. Run **Battle Royale X > Prototype 01 > Create Default Data**.
7. Run **Battle Royale X > Prototype 01 > Build Test Scene**.
8. Enter Play Mode and execute `Docs/TEST_MATRIX.md`.
9. Only after the capsule prototype is mechanically stable, integrate sourced character models, animations, VFX, audio and UI.

## Do not add yet

- multiplayer/netcode;
- accounts/auth;
- matchmaking;
- ranking;
- monetization;
- large map systems;
- additional classes;
- hard crowd-control/stun systems;
- major architectural rewrites.

## Non-negotiable gameplay principles

- 2.5D/isometric ARPG presentation.
- No hard CC as a core mechanic.
- Continuous player agency.
- No instant-kill normal attacks or unavoidable ultimates.
- Defense and dodge are as important as attack.
- Physical/magical interactions are resolved centrally.
- Variants alter behavior and strategy rather than producing gear-like vertical progression.
- Prototype balance target: both Warrior and Assassin must have a credible skill-based path to win the 1v1.

## What Astra should primarily do

Use editor/runtime access for work that requires Unity:

- compile validation;
- prefab/scene wiring;
- animation setup;
- Humanoid rigs;
- VFX/audio binding;
- asset replacement;
- play-mode testing;
- visual tuning;
- fixing Unity-specific integration defects.

Avoid spending tokens redesigning systems that are already specified and implemented unless a concrete defect requires it.
