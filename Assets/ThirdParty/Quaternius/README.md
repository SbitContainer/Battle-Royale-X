# Quaternius assets used by Battle Royale X

The files in this folder are the free standard editions published by Quaternius under the
CC0 1.0 Universal public-domain dedication. The original license text is preserved beside
each imported pack.

Sources checked on 2026-09-17:

- Universal Base Characters: https://quaternius.com/packs/universalbasecharacters.html
- Universal Animation Library: https://quaternius.com/packs/universalanimationlibrary.html
- Universal Animation Library 2: https://quaternius.com/packs/universalanimationlibrary2.html
- CC0 1.0: https://creativecommons.org/publicdomain/zero/1.0/

Only the Unity FBX files and textures needed for the local prototype are included. No paid
Source/Pro content, engine sample project, Blender source file, Unreal asset, or Godot asset
was imported.

Prototype mapping:

- `Superhero_Male_FullBody.fbx`: Guerreiro presentation mesh.
- `Superhero_Female_FullBody.fbx`: Assassino presentation mesh.
- `UAL1_Standard.fbx`: locomotion/idle animation source.
- `UAL2_Standard.fbx`: combat animation source.

The character mesh remains a child of the logical character root. Gameplay collision,
movement, health, abilities, hitboxes, and hurtboxes do not depend on these assets or on
animation events.
