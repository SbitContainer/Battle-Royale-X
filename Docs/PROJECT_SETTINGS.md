# Configurações recomendadas do projeto Unity

## Projeto inicial
- Unity 6.x
- Template 3D URP
- Color Space: Linear
- Active Input Handling: Both (temporariamente)

## Física
- Gameplay no plano XZ.
- CharacterController para jogadores no protótipo.
- Hitboxes são triggers temporários.
- Hurtbox é collider trigger filho do personagem.

## Layers sugeridas para a fase visual
- Characters
- Hitboxes
- Projectiles
- World
- Tactical

O pacote inicial funciona sem depender dessas layers customizadas. Criá-las quando a integração visual começar.

## Escala
- 1 unidade Unity = aproximadamente 1 metro.
- Guerreiro visual ~1,90 m.
- Assassino visual ~1,75 m.
- Arena inicial ~45 x 45 m.

## Câmera
- perspectiva, não ortográfica;
- pitch 52°;
- yaw 45°;
- FOV 38°;
- distância dinâmica ~12–18 m para manter os dois jogadores visíveis.
