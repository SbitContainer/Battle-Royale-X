# Matriz ao vivo — 19/09/2026 10:54:02

Testes no Play Mode, com comandos às APIs reais e colisões da Unity. Não substituem avaliação humana de diversão.

- PASSOU: Câmera enquadra o corpo inteiro dos dois no spawn
- PASSOU: Câmera mantém corpos inteiros com maior separação
- PASSOU: P1: mapeamento WASD (entrada física verificada separadamente)
- PASSOU: P2: mapeamento setas (entrada física verificada separadamente)
- PASSOU: Motor do Guerreiro responde ao movimento
- PASSOU: Motor do Assassino responde ao movimento
- PASSOU: Personagens permanecem no plano XZ
- PASSOU: Ataque do Guerreiro causa dano uma vez por ativação via física
- PASSOU: Cooldown impede spam
- PASSOU: Ataque do Assassino causa dano uma vez por ativação via física
- PASSOU: Sem friendly fire na mesma equipe
- PASSOU: Dois ataques físicos colidem em Clash
- PASSOU: Clash causa apenas dano reduzido nos dois
- PASSOU: Micro-stagger do Clash inicia e expira
- PASSOU: Clash cancela ambas as hitboxes
- PASSOU: Guarda reduz dano
- PASSOU: Parry perfeito zera dano e aplica micro-stagger no atacante
- PASSOU: Parry fora da janela perfeita defende parcialmente
- PASSOU: Esquiva durante iframe evita dano de hitbox
- PASSOU: Dash percorre a distância configurada
- PASSOU: Travessia cruza o adversário
- PASSOU: Retorno volta dentro da janela
- PASSOU: Retorno expira e respeita cooldown
- PASSOU: Travessia do Assassino causa 24 de dano uma única vez
- PASSOU: Guarda reduz travessia de 24 para 6 de dano
- PASSOU: Parry nega travessia e abre contra-ataque manual
- PASSOU: Contra-ataque do Guerreiro exige básico e adiciona 6 de dano uma vez
- PASSOU: Investida do Guerreiro causa apenas 6 de dano uma vez
- PASSOU: Travessia não causa friendly fire
- PASSOU: Travessia fora do trajeto não acerta
- PASSOU: Parede bloqueia dash e dano além dela
- PASSOU: Buff de ultimate inicia
- PASSOU: Buff expira e todos os modificadores voltam a 1.0
- PASSOU: Cinco Cortes cobra energia uma vez e aceita exatamente cinco dashes manuais
- PASSOU: Cinco Cortes causa 12 por travessia e 60 no total sem defesa
- PASSOU: Cinco Cortes encerra cargas e mantém cooldown iniciado no primeiro cast
- PASSOU: Caçada reduz dano causado e aumenta mobilidade conforme dados
- PASSOU: Execução aumenta ameaça e preserva controle do oponente
- PASSOU: Mochila inicia com três slots
- PASSOU: Quarto item recusado sem upgrade
- PASSOU: Usar mochila de drop aumenta capacidade para quatro
- PASSOU: Item consumido desaparece
- PASSOU: Cura tem tempo de uso e dano a interrompe sem consumir
- PASSOU: Cura concluída restaura vida e consome item
- PASSOU: Essência conclui antes da cura e restaura energia
- PASSOU: Troca de runa aguarda e pode ser interrompida por dano
- PASSOU: Runa só é consumida ao concluir troca de cerca de 0.8 s
- PASSOU: Coleta real de esfera por colisão entra no inventário
- PASSOU: Repulsão desloca o adversário
- PASSOU: Barreira bloqueia movimento e expira
- PASSOU: Fumaça aparece como área placeholder
- PASSOU: Campo nulo cancela ataque nullifiable via física
- PASSOU: Airdrop aguarda relógio configurado (12 s padrão; limiar de teste reduzido)
- PASSOU: Airdrop apresenta três opções
- PASSOU: Pegar uma escolha elimina as outras duas
- PASSOU: Airdrop não concede a mesma recompensa duas vezes
- PASSOU: Magia versus magia explode em área sem dano duplicado
- PASSOU: Ataque físico marcado anula magia via física
- PASSOU: Anulação entra em cooldown separado de 30 s
- PASSOU: Durante cooldown novo ataque não anula magia
- PASSOU: Golpe final gera vitória do Guerreiro na HUD
- PASSOU: Nenhum erro ou exceção durante a execução

Resultado: 62 passaram; 0 falharam.

## Validação visual estática

Resultado: 36 passaram; 0 falharam.

- PASSOU: Assets/Guerreiro: modelo existe
- PASSOU: Assets/Guerreiro: avatar humanoide válido
- PASSOU: Assets/Guerreiro: malha skinned existe
- PASSOU: Assets/Assassino: modelo existe
- PASSOU: Assets/Assassino: avatar humanoide válido
- PASSOU: Assets/Assassino: malha skinned existe
- PASSOU: Animações/locomoção: clips obrigatórios existem
- PASSOU: Animações/combate: clips obrigatórios existem
- PASSOU: Animator/controller gerado existe
- PASSOU: Animator/parâmetros de apresentação completos
- PASSOU: Animator/estados de apresentação completos
- PASSOU: Cena/asset gerado existe
- PASSOU: Cena/exatamente dois personagens lógicos
- PASSOU: Cena/Guerreiro: CharacterController no root lógico
- PASSOU: Cena/Guerreiro: Hurtbox preservada
- PASSOU: Cena/Guerreiro: root lógico sem renderer
- PASSOU: Cena/Guerreiro: VisualModel filho existe
- PASSOU: Cena/Guerreiro: Animator configurado
- PASSOU: Cena/Guerreiro: root motion desativado
- PASSOU: Cena/Guerreiro: ponte visual configurada
- PASSOU: Cena/Guerreiro: modelo renderizável
- PASSOU: Cena/Guerreiro: sem fallback de cápsula
- PASSOU: Cena/Assassino: CharacterController no root lógico
- PASSOU: Cena/Assassino: Hurtbox preservada
- PASSOU: Cena/Assassino: root lógico sem renderer
- PASSOU: Cena/Assassino: VisualModel filho existe
- PASSOU: Cena/Assassino: Animator configurado
- PASSOU: Cena/Assassino: root motion desativado
- PASSOU: Cena/Assassino: ponte visual configurada
- PASSOU: Cena/Assassino: modelo renderizável
- PASSOU: Cena/Assassino: sem fallback de cápsula
- PASSOU: Cena/apresentador de VFX de combate existe
- PASSOU: Cena/HUD de combate referencia os dois jogadores
- PASSOU: Cena/controles touch para teste Android existem
- PASSOU: Cena/bot de treino do Guerreiro existe
- PASSOU: Cena/câmera preserva os dois alvos

## Smoke visual no Play Mode

Resultado: 17 passaram; 0 falharam.

- PASSOU: Runtime/exatamente dois personagens ativos
- PASSOU: Runtime/Guerreiro e Assassino inicializados
- PASSOU: Runtime/Animators ativos
- PASSOU: Runtime/avatares humanoides válidos
- PASSOU: Runtime/root motion desativado
- PASSOU: Runtime/ataque continua aceito pela lógica
- PASSOU: Runtime/movimento alimenta o Animator
- PASSOU: Runtime/ataque dispara estado visual
- PASSOU: Runtime/pés permanecem no chão durante golpe
- PASSOU: Runtime/evento de combate gera VFX
- PASSOU: Runtime/HUD de combate cria Canvas responsivo
- PASSOU: Runtime/HUD apresenta vida, energia, habilidades e inventário
- PASSOU: Runtime/animação não desloca o root lógico
- PASSOU: Runtime/personagem permanece no plano XZ
- PASSOU: Runtime/pés permanecem no chão na recuperação
- PASSOU: Runtime/modelos visuais permanecem anexados
- PASSOU: Runtime/nenhuma cápsula de fallback ativa

## Build e dispositivo

- PASSOU: build Android Release, 0 erros, APK `Builds/BattleRoyaleX-mobile-v2.apk`
- PASSOU: instalação por ADB no Moto G54
- PASSOU: pacote `com.sbitcontainer.battleroyalex.prototype`, versão `0.2.0-mobile-v2`, código 2
- PASSOU: atividade Android iniciou sem crash registrado

Falhas finais: nenhuma.
