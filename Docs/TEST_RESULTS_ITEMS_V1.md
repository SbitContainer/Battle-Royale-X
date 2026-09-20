# Matriz ao vivo — 19/09/2026 15:02:31

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
- PASSOU: Todas as 12 variações existem como pickups no chão
- PASSOU: Runa de outra classe é recusada e permanece no chão
- PASSOU: Botão de coleta adiciona uma runa compatível e emite identificação do item
- PASSOU: Usar runa coletada muda a habilidade de movimento exibida e consome o item
- PASSOU: Troca de runa aguarda e pode ser interrompida por dano
- PASSOU: Runa só é consumida ao concluir troca de cerca de 0.8 s
- PASSOU: Coleta real de poção por colisão entra no inventário
- PASSOU: Poção coletada do chão conclui a cura e é consumida
- PASSOU: Repulsão desloca o adversário
- PASSOU: Barreira bloqueia movimento e expira
- PASSOU: Granada cria nuvem de fumaça suave com volume legível
- PASSOU: Fumaça bloqueia a linha de visão usada pelo bot
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

Resultado: 68 passaram; 0 falharam.

## Validações complementares

- Validação estrutural/visual da cena: 40 passaram; 0 falharam.
- Smoke test visual em Play Mode: 17 passaram; 0 falharam.
- Build Android Release: passou; 0 erros; 150033802 bytes processados.
- APK: `Builds/BattleRoyaleX-items-v1.apk` (47.732.777 bytes).
- Pacote: `com.sbitcontainer.battleroyalex.prototype`.
- Versão: `0.3.0-items-v1` (`versionCode` 3).
- SHA-256: `CE8FEAA17B881C2FC9A545DB1A241C6740A4A451FCA457D42F0406822A447D9A`.
- Instalação física: não executada nesta rodada porque nenhum dispositivo ADB estava conectado.
