BATTLE ROYALE X — PLANO DE IMPLEMENTAÇÃO

Objetivo  
Construir o máximo possível fora do Astra/Codex, deixando para eles apenas tarefas que exigem acesso direto à Unity, montagem de cena, importação de assets, ajuste visual e testes em Play Mode.

1\. STACK TÉCNICA DO PROTÓTIPO  
\- Unity 6 \+ URP.  
\- Input System.  
\- Cinemachine 3 para câmera isométrica.  
\- Código em C\# desacoplado de assets visuais.  
\- ScriptableObjects para personagens, habilidades, variações, loot e regras de interação.  
\- Primeiro protótipo offline/local. Multiplayer só depois do combate aprovado.

2\. CÓDIGO QUE SERÁ PRODUZIDO AQUI  
\- CharacterDefinition / CharacterStats.  
\- HealthSystem.  
\- EnergySystem.  
\- CooldownSystem.  
\- CharacterMotor 2.5D.  
\- CombatStateMachine.  
\- AbilityDefinition.  
\- AbilityVariantDefinition.  
\- AbilityController.  
\- AttackBasic.  
\- DefenseController.  
\- MovementAbilityController.  
\- UltimateController.  
\- Hitbox / Hurtbox.  
\- DamagePacket.  
\- CombatResolver.  
\- ClashResolver.  
\- Parry / Guard / Dodge / Counter.  
\- InteractionCooldownSystem.  
\- Inventory de 3 slots e upgrade para 4\.  
\- ItemDefinition.  
\- ConsumableItem.  
\- VariantRuneItem.  
\- LootPickup.  
\- AirDropController.  
\- LootTable.  
\- Smoke / Repulsion / Barrier / NullField como lógica independente do VFX.  
\- UI data bindings para vida, energia, cooldowns e inventário.  
\- Configurações iniciais de Guerreiro e Assassino.  
\- Testes unitários ou verificações determinísticas para interações importantes.

3\. O QUE PEGAR PRONTO  
Personagens / animações  
\- Mixamo para rig humanoide, idle, walk, run, ataques-base, dodge e outras animações de protótipo.  
\- Modelos stylized fantasy compatíveis com humanoid podem vir do Fab/Unity Asset Store.

UI  
\- Kenney UI Pack e RPG Expansion para inventário, barras e botões temporários.

VFX  
\- Packs URP do Unity Asset Store para slash, impacto, magia, fumaça, trail e explosão.  
\- Esses efeitos serão matéria-prima; cor, escala, duração e timing serão ajustados para a linguagem visual do Battle Royale X.

Câmera e render  
\- Cinemachine 3\.  
\- URP para iluminação, partículas e pós-processamento leve.

4\. O QUE NÃO VALE PEGAR PRONTO  
\- Regras de Clash.  
\- Regras de Parry.  
\- Resolução de ataque contra defesa.  
\- Cooldown especial de interação.  
\- Sistema de troca de variações em luta.  
\- Balanceamento.  
\- Hit-stop.  
\- Janelas de invulnerabilidade.  
\- Prioridade entre estados.  
\- Lógica do inventário e drops.  
\- Identidade mecânica de Guerreiro e Assassino.

5\. TAREFAS RESERVADAS PARA O ASTRA/CODEX  
\- Criar/organizar o projeto Unity na máquina.  
\- Instalar pacotes e ajustar Project Settings.  
\- Importar modelos, animações e VFX escolhidos.  
\- Configurar Avatar/Humanoid e Animator Controllers.  
\- Montar prefabs.  
\- Ligar referências no Inspector.  
\- Montar a arena visual.  
\- Configurar materiais, iluminação e pós-processamento.  
\- Ajustar Cinemachine vendo o resultado em tempo real.  
\- Conectar Animation Events quando necessário.  
\- Testar em Play Mode.  
\- Corrigir problemas específicos de integração da engine.  
\- Profiler, otimização e build.

6\. REGRA DE ECONOMIA DE CRÉDITOS  
Antes de mandar qualquer tarefa ao Astra:  
1\. Definir a mecânica aqui.  
2\. Gerar o código-base aqui.  
3\. Definir nomes de arquivos, classes e prefabs.  
4\. Definir quais assets usar.  
5\. Entregar ao Astra uma tarefa curta e objetiva de integração.

Evitar prompts como “crie o jogo”. Preferir prompts como: “importe estes assets, adicione estes scripts já fornecidos aos prefabs especificados, configure estas referências e execute estes cinco testes”.

7\. ORDEM DE CONSTRUÇÃO  
Fase 1 — Fundação  
\- Movement.  
\- Input.  
\- Health/Energy.  
\- State machine.  
\- Hitbox/Hurtbox.

Fase 2 — Combate  
\- Ataque básico.  
\- Guarda / esquiva.  
\- Clash.  
\- Parry.  
\- Cooldowns de interação.

Fase 3 — Habilidades  
\- Movimento especial.  
\- Ultimate.  
\- Variantes.  
\- Troca de variantes em luta.

Fase 4 — Loot  
\- Mochila 3 slots.  
\- Mochila 4 slots.  
\- Cura/Energia.  
\- Fumaça, Repulsão, Barreira e Selo Nulo.  
\- Runas.  
\- Drop aéreo.

Fase 5 — Visual  
\- Modelos.  
\- Animações.  
\- VFX.  
\- UI.  
\- Câmera.

Fase 6 — Balanceamento  
\- Guerreiro vs Assassino repetido até o 1x1 ser divertido e legível.

8\. PRINCÍPIO DE ARQUITETURA  
Toda lógica de combate deve funcionar mesmo que os personagens sejam cápsulas sem animação e sem VFX. O visual apenas apresenta o estado do sistema. Isso permite testar e balancear sem depender dos assets e reduz drasticamente o trabalho que precisa ser feito pelo Astra.  
