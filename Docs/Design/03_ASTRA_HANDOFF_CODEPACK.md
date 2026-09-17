BATTLE ROYALE X — HANDOFF TÉCNICO PARA ASTRA / CODEX

Objetivo  
Este documento acompanha o arquivo BattleRoyaleX\_Prototype01\_CodePack.zip. O pacote foi preparado para reduzir o consumo de créditos do Astra/Codex: decisões de gameplay, arquitetura lógica, ScriptableObjects, geradores de dados e cena de teste já estão implementados. O Astra deve concentrar-se em integração real dentro da Unity, assets, Animator, VFX, áudio, compilação e testes.

1\. CONTEÚDO JÁ IMPLEMENTADO

Core  
\- Tipos de classe, slots, ataques, defesas, interações, itens e eventos de combate.  
\- CombatEvents desacopla gameplay de VFX/SFX.

Personagens  
\- CharacterRuntime.  
\- CharacterMotor25D.  
\- HealthComponent.  
\- EnergyComponent.  
\- CharacterStateController com locks de input seguros para stagger, item e recovery simultâneos.

Combate  
\- DamagePacket.  
\- Hitbox e Hurtbox.  
\- CombatResolver.  
\- Clash físico x físico com dano reduzido nos dois.  
\- Guard.  
\- Parry com janela perfeita.  
\- Dodge / iframe.  
\- Nullify.  
\- Reflect.  
\- Cooldown separado para interações especiais.  
\- Estrutura para ataque físico anular magia; a propriedade entra em cooldown de 30 s.  
\- Estrutura para colisão magia x magia gerar explosão em área.  
\- ProjectileHitboxMover para projéteis futuros.

Habilidades  
\- AbilityDefinition via ScriptableObject.  
\- AbilityController.  
\- Ataque básico sem variações.  
\- Defesa, Movimento e Ultimate substituíveis por variantes.  
\- Restrição de variante por classe.  
\- Dash, DashThrough e DashReturn.  
\- Buffs temporários de Ultimate.

Inventário / loot  
\- Mochila inicial de 3 slots.  
\- Upgrade para 4 slots.  
\- Poção de cura com tempo de uso e interrupção por dano.  
\- Poção de Essência mais rápida.  
\- Orbe de redução de cooldown.  
\- Runas de variação com aproximadamente 0,8 s de ativação e interrupção por dano.  
\- Variação só é consumida quando a troca conclui.  
\- Pickups de chão.  
\- Airdrop com três escolhas; escolher uma elimina as outras.

Itens táticos  
\- Fumaça placeholder.  
\- Repulsão.  
\- Barreira temporária.  
\- Campo Nulo.

Câmera  
\- IsometricCameraRig em perspectiva.  
\- Pitch 52°.  
\- Yaw 45°.  
\- FOV 38°.  
\- Distância dinâmica para enquadrar os dois jogadores.

2\. DADOS INICIAIS

O editor gera automaticamente os ScriptableObjects de Guerreiro e Assassino, habilidades base, duas variações de Defesa/Movimento/Ultimate, consumíveis e itens táticos.

Os valores são propositalmente de protótipo e devem ser calibrados jogando; não tratá-los como balanceamento final.

3\. AUTOMAÇÃO DO EDITOR

Menu criado:  
Battle Royale X \> Prototype 01 \> Create Default Data  
Battle Royale X \> Prototype 01 \> Build Test Scene

Build Test Scene gera:  
\- arena de aproximadamente 45 x 45;  
\- obstáculos simples;  
\- cápsula Guerreiro;  
\- cápsula Assassino;  
\- Hurtboxes;  
\- controles locais de dois jogadores;  
\- câmera;  
\- luz;  
\- pickups;  
\- airdrop;  
\- HUD de debug.

4\. CONTROLES DE TESTE

P1 Guerreiro  
WASD \= movimento  
F \= ataque  
G \= defesa  
H \= movimento especial  
R \= Ultimate  
1-4 \= inventário

P2 Assassino  
Setas \= movimento  
Numpad 1 \= ataque  
Numpad 2 \= defesa  
Numpad 3 \= movimento especial  
Numpad 0 \= Ultimate  
Numpad 4-7 \= inventário

5\. PRIMEIRA TAREFA DO ASTRA

1\. Criar/abrir projeto Unity 6 URP.  
2\. Extrair o CodePack e copiar Assets/BattleRoyaleX para Assets do projeto.  
3\. Para o protótipo inicial, deixar Active Input Handling como Both ou Input Manager (Old), pois PrototypeLocalInput usa UnityEngine.Input para não depender de pacotes adicionais.  
4\. Compilar.  
5\. Corrigir somente erros concretos de versão/API encontrados pela Unity; não redesenhar arquitetura sem necessidade.  
6\. Executar Create Default Data.  
7\. Executar Build Test Scene.  
8\. Pressionar Play e validar a matriz de testes incluída em Docs/TEST\_MATRIX.md.  
9\. Somente depois importar modelos, animações, VFX e UI.  
10\. Não implementar multiplayer nesta etapa.

6\. ASSETS RECOMENDADOS

Família visual preferida para economizar e manter consistência: Quaternius.  
\- Universal Base Characters.  
\- Modular Character Outfits \- Fantasy.  
\- Universal Animation Library 1 e 2\.  
\- Fantasy Props MegaKit.  
\- Medieval Village MegaKit.  
Todos possuem opções gratuitas e o autor indica licença CC0.

Complementos  
\- Adobe Mixamo para animações específicas.  
\- Kenney UI Pack / RPG Expansion para UI provisória.  
\- Magic Effects FREE no Unity Asset Store para VFX inicial.

Os URLs e a ordem de download estão dentro de Docs/SOURCING\_PLAN.md.

7\. NÃO FAZER AINDA

\- multiplayer/netcode;  
\- matchmaking;  
\- backend;  
\- conta de usuário;  
\- ranking;  
\- loja;  
\- monetização;  
\- XP ou níveis;  
\- raridade de equipamento;  
\- otimização prematura;  
\- ECS;  
\- refatoração ampla sem bug comprovado.

8\. CRITÉRIO PARA LIBERAR A PRÓXIMA FASE

Antes de colocar arte definitiva, o duelo Guerreiro x Assassino usando cápsulas deve ser divertido e tecnicamente consistente. Clash, defesa, esquiva, movimentação, troca de variação e itens precisam gerar decisões reais. A arte entra depois para amplificar feedback, não para consertar gameplay.

9\. ARQUIVO PRINCIPAL

BattleRoyaleX\_Prototype01\_CodePack.zip  
Dentro dele:  
\- Assets/BattleRoyaleX — código drop-in para Unity.  
\- README.md — instalação e controles.  
\- Docs/ASTRA\_HANDOFF.md — handoff detalhado.  
\- Docs/TEST\_MATRIX.md — testes obrigatórios.  
\- Docs/SOURCING\_PLAN.md — assets e links.  
\- Docs/PROJECT\_SETTINGS.md — configuração do projeto.

Estado: pronto para primeira importação e compilação dentro da Unity. A compilação final ainda precisa ser validada no editor Unity real, pois esta preparação foi feita fora do runtime da Unity.  
