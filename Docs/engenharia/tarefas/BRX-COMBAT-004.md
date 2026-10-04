# BRX-COMBAT-004 — Mira precisa, básicos contínuos e Guerreiro

03/10/2026. VALIDADO_EDITOR / AGUARDANDO_VALIDACAO_ANDROID. Pedido atual autoriza a reformulação das nove variantes do Guerreiro e ajustes compartilhados de mira/básicos nas quatro classes. Guerreiro/Assassino anteriormente validados não impedem as alterações explicitamente pedidas agora.

## Esperado e limites

- Segurar/direcionar exibe trajetória, largura, área e ponto efetivo; ajustar distância só para habilidades com destino/área variável. Preview e executor devem compartilhar cálculo. Corrigir shader ausente do preview faz parte desse pedido, não instalar Play Core por hipótese.
- Básico de todas as classes: repetição enquanto segura; sem cooldown de skill/HUD; startup/atividade/recovery e velocidade de ataque limitam cadência. Básico do Guerreiro não empurra; choque físico empurra os dois.
- Guarda: anulação 2 s com aura. Fortaleza: imunidade 1,5 s, três escudos orbitais e lançamento radial com dano/repulsão. Escudo Usurpador substitui Parry: captura uma skill hostil (não básico), reenvio manual em até 2 s, respeitando comportamento real da skill copiada e limpeza por ator.
- Um arremesso de escudo sem deslocamento, duas perseguições distintas e três ultimates de área com lentidão/proteção; valores iniciais ajustáveis. Não mudar kits de outras classes fora da mira/básico compartilhados. Não adicionar hard CC, novas classes, dependências, assets externos, multiplayer ou infraestrutura.
- Não gerar/publishar/instalar por autorização herdada da release anterior. Validar código/Editor e shader para Android; distribuição é etapa separada.

## Setores e escritores

Input → AbilityController → hitboxes/Defense/Health → apresentação/UI/bot/inventário. Assets por GUID preservados; gerador deve manter os mesmos defaults em futuras execuções e atualizar somente Guerreiro/básicos nesta cena existente.

- Principal: coordenação, factory/atualização dirigida de dados, testes e documentação, revisão integrada. Sem regenerar cena ou dados de Mago/Arqueiro/Assassino como efeito colateral.
- combat_warrior_004: AbilityController principal/V1/novo partial Guerreiro, Defense/Resolver, tipos/dados/descrições técnicas e helpers do Guerreiro; limite 25 min com checkpoints.
- aim_input_004: controles touch/local, preview, solver compartilhado e shader Resources próprio; limite 20 min com checkpoints. Coordenação direta com runtime para contrato de endpoint.
- Revisão independente após congelar candidato e liberação de slot. Um escritor por arquivo. Plataforma recusou terceira delegação concorrente; principal cobre testes enquanto duas frentes implementam.

## Baselines de design autorizados

Fortaleza: três escudos a 120°; fase inicial sorteada entre 15/30/45°, uma volta em 1,5 s e saída nos ângulos iniciais; dano 14 por escudo, impulso 3, alcance 8 m. Guarda 2 s. Usurpador janela de recast 2 s após capturar.

Skill 2: Arremesso de Escudo em linha (8 m, dano14, slow30%/2 s), sem deslocar o Guerreiro, conforme confirmação do usuário; perseguição curta (5 m, dano18 + golpe6 ao chegar); longa (9 m, dano9, sem slow). Nome alterado com autorização explícita para corresponder ao arremesso. O ID Warrior_Move_Base permanece estável para preservar referências.

Ultimates: Bastião Explosivo (alcance5/raio2,8; startup0,65; dano28; slow30%/2 s; proteção45%/1,8 s); Domínio de Ferro (alcance4/raio3,4; startup0,45; campo3 s; dano6 por pulso0,6 s; slow35%; proteção35%); Ruptura Titânica (alcance4/raio4,5; startup0,8; três ondas a0,45 s; dano10/onda; slow35%/2 s; proteção50%/2 s). Sem controle que retire movimento por longo período. Confirmar clareza no aparelho separadamente.

## Evidência e recuperação

Baseline anterior: Editor237/48 e contratoatualização35 pass; Android12 instalado, mas preview teve shader null. Nenhum resultado anterior aprova fonte nova ou os efeitos pedidos. Acrescentar casos novos e ajustar somente expectativas antigas que o usuário substituiu; manter testes genéricos de parry/contra-ataque por fixtures explícitas quando necessário.

Guardar primeira falha, compilação, reruns e não executados. Não marcar touch/qualidade visual/FPS validado por APIs/Editor. Recuperação deve retirar apenas o diff desta tarefa preservando árvores sujas anteriores; não usar reset/checkout destrutivos.

Autorização posterior: usuário solicitou publicar a atualização pelo GitHub após corrigir o analógico; distribuição é acompanhada na BRX-INPUT-005, sem herdar a autorização antiga. Revisão independente encontrou/corrigiu alcance do golpe extra, prioridade de recasts sobre básico e preview do Gancho. Testes de parry/counter legados preservados por fixture explícita, separados das variantes novas.

Resultado integrado final: 278 testes de combate/UI e 48 verificações estruturais visuais passaram, zero falhas. APK13 publicado, sem instalar via ADB. Logs/causas das falhas intermediárias e arquivos alterados em BRX-INPUT-005. Trajetórias de perseguição são previsões de alvo móvel, não garantia contra fuga/parede. Toque/gráficos/FPS no aparelho não foram aprovados pelo Editor.
