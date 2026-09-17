# Handoff para Astra / Codex

## Missão

Não redesenhe a arquitetura do jogo. O núcleo lógico já está no pacote. Sua tarefa é **integrar, compilar, conectar assets, testar e corrigir incompatibilidades reais da Unity**.

## Ordem de trabalho

1. Criar/abrir projeto Unity 6 URP.
2. Copiar `Assets/BattleRoyaleX`.
3. Compilar e corrigir somente erros concretos de API/Unity encontrados.
4. Executar `Create Default Data`.
5. Executar `Build Test Scene`.
6. Confirmar a cena com cápsulas e controles locais.
7. Testar matriz mínima descrita em `TEST_MATRIX.md`.
8. Só depois importar modelos/animações/VFX.

## Não fazer nesta etapa

- não adicionar multiplayer;
- não reescrever para ECS;
- não criar sistema de itens com raridade;
- não adicionar XP/nível/ouro;
- não transformar ScriptableObjects em banco externo;
- não comprar assets automaticamente;
- não alterar regras de gameplay sem evidência de bug;
- não criar arquitetura genérica excessiva.

## Integração visual

Quando a lógica passar nos testes:

### Personagens
- substituir cápsula do Guerreiro por modelo humanoide de silhueta larga;
- substituir cápsula do Assassino por modelo humanoide estreito;
- manter `CharacterRuntime`, `CharacterController` e Hurtbox na raiz lógica;
- modelo visual deve ficar como filho do objeto lógico.

### Animator
Conectar animações aos eventos/estados, sem mover a lógica de dano para Animation Events como fonte de verdade. Animation Events podem chamar funções de apresentação, mas a janela ativa do golpe deve continuar definida pela AbilityDefinition/AbilityController.

### VFX
Criar um listener de `CombatEvents` para:
- Hit
- Block
- Parry
- Dodge
- Clash
- Nullify
- Reflect
- VariationSwap
- TacticalUsed

### Câmera
Usar os valores do `IsometricCameraRig` como referência visual. Se substituir por Cinemachine, preservar aproximadamente:
- pitch 52°;
- yaw 45°;
- FOV ~38°;
- framing dos dois combatentes;
- sem rotação manual durante o protótipo.

## Resultado esperado antes de continuar

Uma cena local em que Guerreiro e Assassino consigam:
- movimentar;
- atacar;
- colidir ataques físicos em Clash;
- bloquear/parry/esquivar;
- usar movimento especial;
- ativar ultimate;
- pegar e usar itens;
- trocar variação guardada;
- disputar airdrop;
- morrer e gerar resultado consistente.
