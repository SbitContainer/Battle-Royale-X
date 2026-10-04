# Effects Lab 0.9.2

## Changes

- Restored the three combat-screen skill-variation selectors. Class selection and detailed skill information remain in the settings menu.
- Added an EventSystem pointer-down regression check for all three selectors and all Mage/Archer variations; direct controller calls alone did not cover the previous UI defect.
- Cast presentation now selects the active character model, not an inactive class model.
- Assisted area casts target the nearest eligible enemy's position instead of always landing at maximum range. Drag aiming adds a ground preview and controls placement distance.
- Recast-ready abilities display REATIVAR rather than an unusable-looking cooldown. Failed casts show feedback; a short 0.22-second buffer allows a skill pressed just before the current action finishes.
- Normal and seeking projectiles sweep their travel segment for hurtboxes between frames. Heavy arrows retain their separate distance-damage sweep.
- Mage effects: animated plasma orbs, textured energy trails, bubbling swamp and expanding fire repulsion. Archer effects: visible arrow shafts, luminous precision shot, falling arrow prefabs and casting sparks.
- Archer casting uses a procedural bow-draw/release pose on the active humanoid model.

## Third-party assets

Kenney Particle Pack 1.1: https://kenney.nl/assets/particle-pack

License: CC0. Selected original PNGs and the original license are included in `Assets/ThirdParty/Kenney/ParticlePack`. No paid assets or account login are required. The project adds its own URP shader, material setup and particle animation; the downloaded pack supplies textures, not gameplay code.

## Validation

- Unity 6000.6.1f1 compilation: passed. No C# or shader errors in the release run.
- Scene/presentation structural validation: 48 passed, 0 failed.
- Live combat and UI matrix: 214 passed, 0 failed, including EventSystem selector callbacks for Mage and Archer. These are simulated UI events, not a physical touchscreen test.
- Presentation captures inspected: swamp, repulsion flames, heavy orb and arrow rain, under `Logs/Presentation`. Camera captures do not include the screen-overlay HUD.
- Android build: succeeded with 0 build errors. APK `Builds/BattleRoyaleX-effects-lab.apk`, 57,131,167 bytes, generated 2026-09-26 12:00 local time.
- APK manifest independently checked with aapt: package `com.sbitcontainer.battleroyalex.prototype`, versionName `0.9.2-effects-lab`, versionCode 11.
- Phone installation: PASSED on Moto G54 after wireless debugging was re-enabled. `adb install -r` returned Success; `dumpsys package` confirmed versionName `0.9.2-effects-lab`, versionCode 11 and lastUpdateTime `2026-09-26 12:04:31`. The game activity was launched successfully. Physical touchscreen feel and visual quality still require the user's playtest.

Detailed checks: `Docs/TEST_RESULTS_LAB_001.md`. Release/build evidence: `Logs/effects-release.log` and `Logs/polish-build.txt`.

## Source files changed in this correction pass

Editor:
- `PrototypeDataFactory.cs`
- `PrototypeMobileBuildSetup.cs`
- `PrototypePolishTools.cs`
- `PrototypeVFXFactory.cs`
- `PrototypeSkillVfxPolish.cs` (new)
- `PrototypePresentationReview.cs` (new)

Runtime:
- `Input/PrototypeMobileTouchControls.cs`
- `Input/PrototypeMobileTouchControls.Settings.cs`
- `Input/MobileAbilityAimPreview.cs` (new)
- `Abilities/AbilityController.cs`
- `Abilities/AbilityController.V1.cs`
- `Abilities/OwnedAbilityEffect.cs`
- `Abilities/SeekingProjectileMover.cs`
- `Abilities/ArcherAreaEffects.cs`
- `Combat/ProjectileHitboxMover.cs`
- `Characters/CharacterVisualAnimator.cs`
- `Visual/CombatVFXRouter.cs`
- `Visual/SkillVfxMotion.cs` (new)
- `Debug/PrototypeLiveTests.MobileV4.cs`

Also added `Assets/Resources/BRXCombatFx.shader` and selected Kenney textures/license; rebuilt Mage/Archer VFX prefabs and materials, ability data and arena using the existing generators. Unity generated the corresponding meta files and updated Android version settings. Earlier uncommitted Mage/Archer work was preserved, so the full Git diff includes changes from before this correction pass.
