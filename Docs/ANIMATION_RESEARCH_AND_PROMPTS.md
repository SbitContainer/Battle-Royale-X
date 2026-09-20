# Pesquisa de animações e prompts — Battle Royale X

Data da pesquisa: 19/09/2026.

## Decisão para o protótipo

Usar primeiro a **Universal Animation Library 2**, já importada no projeto. Ela oferece mais de 130 animações humanoides, incluindo ataques armados, combos, bloqueio e dash. A licença é CC0 e permite uso comercial. Assim evitamos misturar esqueletos e estilos enquanto corrigimos o combate.

Fonte oficial: https://quaternius.com/packs/universalanimationlibrary2.html

Clipes úteis já disponíveis:

- `Sword_Regular_A`, `Sword_Regular_B`, `Sword_Regular_C` e `Sword_Regular_Combo`;
- `Sword_Heavy_Combo`, `Sword_Block`, `Sword_Dash` e `Shield_Dash`;
- `Roll`, `Hit_Chest`, `Hit_Head` e `Hit_Knockback`;
- `Idle_Loop`, `Jog_Fwd_Loop` e `Sword_Idle`.

## Outras opções gratuitas

### Adobe Mixamo

Gratuito com Adobe ID, sem assinatura da Creative Cloud. Personagens e animações podem ser usados sem royalties em jogos pessoais ou comerciais. É bom para procurar movimentos genéricos como strafe, dodge, knockback, sword slash e death. Não podemos redistribuir os arquivos de animação como um pacote separado; eles devem ficar incorporados ao jogo.

Fonte oficial: https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html

### DeepMotion SayMotion

Gera movimento 3D por texto e exporta FBX/GLB/BVH. O plano gratuito atual oferece poucos créditos e um download por mês, mas **não inclui licença comercial**. Portanto serve somente para experimentação e referência, não para colocar a animação grátis no produto final.

Fontes oficiais:

- https://www.deepmotion.com/pricing-saymotion
- https://www.deepmotion.com/article/saymotion-text-prompt-guide

### Cascadeur

É útil para estudar e corrigir poses, mas o plano Free atual não exporta FBX/DAE/USD. Não resolve o pipeline gratuito do jogo. A exportação exige plano Indie ou superior.

Fonte oficial: https://cascadeur.com/blog/general/welcome-to-cascadeur-20241-new-features-free-upgrade-for-basic-users

## Por que os personagens flutuavam

Os FBXs estavam preservando a posição vertical original do corpo e não usavam os pés como referência da raiz. Em golpes, rolagens e na volta para o estado de locomoção, isso fazia a malha subir mesmo com `applyRootMotion` desligado.

A importação agora:

- alinha a altura da raiz pelos pés;
- aplica `Bake Into Pose` para Y, XZ e rotação;
- mantém o deslocamento real sob controle do motor de combate;
- ativa Foot IK nos estados do Animator.

Referência oficial Unity 6: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/ModelImporterClipAnimation.html

## Validação executada

- Compilação Unity 6: passou, sem erros.
- Teste visual automatizado: 17 de 17 verificações passaram.
- Pés no chão durante o ataque: passou.
- Pés no chão depois da recuperação do golpe: passou.
- Build Android: passou, sem erros.
- Instalação e abertura no Moto G54: passou em 19/09/2026 às 09:51.

O teste no aparelho ainda é necessário para avaliar sensação, peso, timing e possível deslizamento visual, porque esses pontos dependem da percepção durante o controle real.

## Prompts prontos para prototipar movimentos originais

Os prompts estão em inglês porque os geradores de movimento costumam interpretar melhor descrições de corpo, direção e ritmo nesse idioma. Cada clipe deve começar e terminar em postura de combate neutra. Não pedir cópia de personagem ou animação de outro jogo.

### Assassino — ataque básico 1

`A lightweight dual-dagger fighter starts in a low combat stance, makes one fast diagonal slash with the right hand, plants both feet firmly, and returns immediately to the same grounded guard pose. Short controlled motion, no jump, no forward travel, game combat animation.`

### Assassino — ataque básico 2

`A nimble dual-dagger fighter starts in a low guard, pivots on the front foot and performs a quick horizontal left-hand slash followed by a short right-hand stab, keeping the hips low and both feet close to the ground, then returns to the original guard pose.`

### Assassino — finalizador do combo

`A fast assassin in a low stance performs three rapid alternating dagger cuts, takes one short step through the target on the final cut, lands with bent knees and both feet planted, then recovers into the same combat stance. Sharp acceleration and clear recovery, no acrobatics.`

### Assassino — dash ofensivo

`A nimble assassin crouches briefly, bursts straight forward in one low explosive dash, crosses an opponent with a waist-level dagger slash, brakes with one foot, and settles into a balanced low guard. Keep the body close to the ground, no floating and no high jump.`

### Assassino — esquiva

`A lightweight fighter performs a fast diagonal sidestep dodge to the left, torso leaning away from an incoming strike, feet skimming the floor, then plants both feet and returns to a ready guard. Short distance, readable anticipation and recovery.`

### Assassino — ultimate

`A dual-dagger assassin lowers into a tense stance, makes four extremely fast slashes around an imaginary enemy using two short grounded steps, pauses for a clear final cross slash, and returns to the starting guard. Powerful but controlled, feet remain grounded, no flips.`

### Guerreiro — ataque básico 1

`A heavy sword-and-shield warrior begins in a stable wide stance, delivers one deliberate diagonal sword cut, keeps the shield protecting the torso, transfers weight through the hips, and returns to the same grounded guard pose. Heavy impact and clear recovery.`

### Guerreiro — combo defensivo

`A heavy warrior blocks an incoming strike with the shield, absorbs the impact with bent knees, immediately counters with a short horizontal sword slash, then resets behind the shield. Feet planted, strong weight, readable anticipation and recovery.`

### Guerreiro — parry e contra-ataque

`A sword-and-shield warrior makes a small precise shield parry to the outside, pivots the hips and answers with a compact sword thrust, then pulls back into guard. No spin, no jump, minimal forward travel, clear defensive timing.`

### Guerreiro — investida

`A heavy shield warrior leans forward and performs a short grounded shoulder-and-shield charge, drives through one impact, stops with a wide planted stance, and returns to guard. Powerful weight, low center of gravity, no jump and no sliding after the stop.`

### Guerreiro — ultimate

`A heavy warrior raises the sword briefly, steps forward with two forceful grounded cuts and finishes with a shield slam toward the ground, holding the final impact pose for a moment before returning to guard. Strong weight, no leap, no floating.`

## Critérios para aceitar qualquer animação

1. O primeiro e o último quadro precisam combinar com a postura de combate.
2. Os pés não podem deslizar ou subir durante antecipação e recuperação.
3. O golpe deve mostrar antecipação, contato e recuperação separadamente.
4. O deslocamento lógico continua no `CharacterMotor25D`; a animação apenas apresenta o corpo.
5. Um dash deve ser testado contra inimigo, guarda, parry e parede.
6. Validar a silhueta e o timing no Moto G54 antes de incorporar definitivamente.

## Próxima adaptação recomendada

Criar controladores separados para Guerreiro e Assassino e montar os ataques com os clipes A/B/C já disponíveis. Isso permite silhuetas e ritmos diferentes sem alterar dano, cooldown ou regras de combate.
