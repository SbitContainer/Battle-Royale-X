# Continuação do Battle Royale X

## Entrega atual — BRX-RELEASE-008 (03/10/2026 local)

Pedido explícito do usuário: publicar efeitos VISUAL-007 e registrar todo trabalho no Git. Fonte integrada e histórico local enviados a `origin/main`: commit `0d28b6b` (398 arquivos), seguido de `f60282a` com lockfile idêntico e quinze capturas reais em Docs/engenharia/evidencias/BRX-VISUAL-007. Regra de registro contínuo no Git salva em AGENTS.md; caches/temporários/anexos não entram na fonte. Nada foi rebalanceado nesta entrega.

**APK 0.9.15-lab/código15 publicado e verificado:** [release](https://github.com/SbitContainer/Battle-Royale-X/releases/tag/android-lab-15), [APK](https://github.com/SbitContainer/Battle-Royale-X/releases/download/android-lab-15/BattleRoyaleX-15.apk). Registro em [android-lab-15.json](engenharia/releases/android-lab-15.json), tarefa [BRX-RELEASE-008](engenharia/tarefas/BRX-RELEASE-008.md). Zero erros de build; mesmo pacote/assinatura; downloads públicos APK/catálogo/inventário validados por SHA256 e latest API15 confirmada. Tag remota aponta f60282a: todos os 1144 inputs recuperáveis no Git, sem diferenças canônicas/inventário. Matriz314/0 e48/0, Node16/0 e publisher10/0. Quinze capturas do Editor, não do celular.

Sem instalação ADB nesta entrega; instalar sobre versão existente (não desinstalar) pelo APK ou confirmar atualização no jogo. Android pode pedir consentimento/permissão de instalação. Toque, FPS, aparência física e ciclo real A→B **AGUARDANDO_VALIDACAO**. Release14 abaixo é histórica. Publicador desta entrega executado com Windows PowerShell5: PS7 rejeitou corretamente sem sobrescrever por diferença cosmética de indentação JSON; não alteramos a ferramenta fora do escopo.

## Etapa visual concluída — BRX-VISUAL-007 (03/10/2026)

Adaptação visual autorizada das três referências fornecidas: básico crescente/cross/espiral, Guarda curva azul/dourada, Usurpador com meia-cúpula hexagonal, Fortaleza com os três escudos chanfrados originais em azul translúcido, Bastião com coluna/fragmentos, Domínio com oito lâminas espectrais e Ruptura com dupla hélice. Carga, execução e contato continuam separados. Geometria/shader procedurais originais, sem importar JPEG como plano ou adicionar pacote. Dano, alcance, timing, energia, cooldown e interações preservados: SHA256 de 136 arquivos GeneratedData e cinco AbilityController*.cs idênticos à entrada desta tarefa.

Compilação inicial falhou CS0266 no novo helper (literal double); corrigido para float. Candidato final: **314 testes combate/UI e 48 verificações estruturais visuais, zero falhas**, `Logs/visual007-matrix2.log`; 15 capturas inspecionadas em `Logs/Warrior007`. Revisão estática independente aprovada. Novo shader em `Assets/Resources/WarriorEnergy.shader`, helper em Runtime/Abilities; lifecycle sem autoridade de dano e sem destruir hosts físicos. Registro/files em [BRX-VISUAL-007](engenharia/tarefas/BRX-VISUAL-007.md). Sem APK/publicação/instalação nova, commit ou push. Qualidade artística final, toque e FPS no Android **AGUARDANDO_VALIDACAO**; latest release permanece 14 histórica abaixo. Próximo passo requer autorização de gerar/publicar APK.

## Etapa anterior — BRX-VISUAL-006 (03/10/2026)

Revisão autorizada dos efeitos do Guerreiro: escudos retangulares chanfrados com faces translúcidas e bordas, gestos/carga de conjuração e impacto no alvo, explosão/campo/ondas distintos. Ultimates agora locais ao Guerreiro: Bastião 18 dano/raio 2,2; Domínio 3 × 5 pulsos/raio 2,5; Ruptura 6 × 3 ondas/raio máximo 3. Centro nasce na posição real ao ativar e fica no chão; não permite lançamento remoto. Cooldowns, energia, lentidão e proteção preservados. Outras classes sem alteração (30 assets comparados por SHA256). Registro completo e arquivos em [BRX-VISUAL-006](engenharia/tarefas/BRX-VISUAL-006.md).

Candidato final validado Editor: 294 combate/interface e 48 estrutural visual, zero falhas (`Logs/visual006-matrix4.log`). Primeira compilação falhou CS0136 no gesto e foi corrigida; giro Fortaleza e cancelamento visual revisados. Capturas reais revelaram sobreposição do apresentador legado; removida apenas nos eventos das habilidades dedicadas Warrior. Dez capturas finais em `Logs/Warrior006`, com escudos e fases das ultimates inspecionados. Após autorização explícita "pode publicar", APK **0.9.14-lab/código14** gerado e publicado: [release](https://github.com/SbitContainer/Battle-Royale-X/releases/tag/android-lab-14), [registro](engenharia/releases/android-lab-14.json). Pacote/assinatura compatíveis, APK/catálogo/inventário públicos conferidos por SHA256 e latest API confirmado. Sem instalação ADB, commit ou push da fonte. Instalar sobre a versão existente pelo link, sem desinstalar; consentimento Android pode ser necessário. Qualidade visual/toque/FPS/ciclo de atualização no Android AGUARDANDO_VALIDACAO. Release13 abaixo é histórica.

Atualizado em 03/10/2026. Pedido vigente: reformulação autorizada do Guerreiro/mira/básicos em [BRX-COMBAT-004](engenharia/tarefas/BRX-COMBAT-004.md), seguida de correção do analógico e nova publicação GitHub sem depuração em [BRX-INPUT-005](engenharia/tarefas/BRX-INPUT-005.md). Papéis executados em rodadas, não serviços permanentes. A release 12 abaixo é histórica; não aprova a fonte nova.

## Candidato novo em validação

Analógico sincroniza somente seu fingerId físico; linha de mira não captura toques. Guarda 2 s, Fortaleza 1,5 s/três escudos, Usurpador reenvia comportamento capturado manualmente em 2 s; Arremesso de Escudo não desloca o Guerreiro; perseguições curta/longa e três ultimates de área. Básicos sem cooldown de habilidade, limitados por cadência, repetidos ao segurar. Mira compartilhada para áreas/rotas e contato do Gancho. Imunidade própria da Fortaleza é limpa na troca; não altera estado genérico de esquiva.

Matriz final: 278/0 e 48/0 visual estrutural (Logs/input005-matrix4.log), compilação sem erros. Única falha da rodada anterior era fixture da Chuva de Flechas: troca de classe reativava bot, que saía da área e defendia. Isolamento corrigido, sem alterar habilidade. APK 0.9.13-lab/código13 publicado e downloads públicos verificados: [release](https://github.com/SbitContainer/Battle-Royale-X/releases/tag/android-lab-13), [registro](engenharia/releases/android-lab-13.json). Mesma assinatura/pacote da versão12, shader de mira incluído e compilado no Android. Nenhuma instalação ADB nesta entrega. Logs anteriores preservados. Validação real de dois dedos/qualidade visual/FPS e ciclo do atualizador permanecem AGUARDANDO_VALIDACAO_ANDROID. Abrir jogo com internet e confirmar download/instalador; alternativa pelo link do APK, sem desinstalar.

## Release Android em execução

Runtime consulta nova versão, baixa após confirmação e verifica bytes/hash/pacote/código/certificado. Ponte privada chama instalador Android com consentimento; primeiro APK deve ser instalado pelo link do navegador. Builder gera a cena existente em pasta por código, sem sobrescrever APK anterior. Publisher precisa `-Publish`, mantém credenciais somente em memória no computador e verifica downloads públicos. Assinatura debug preservada; catálogo não assinado separadamente; não é Addressables/Expo.

Evidência atual: 35 testes puros do contrato passaram; matriz reexecutada na fonte runtime atual: 237 combate/UI e 48 visual estrutural passaram, zero falhas. Java compilou; 10 testes do publisher e 16 Node passaram. Revisões independentes estáticas aprovadas, incluindo distinção cloud OneDrive versus junction/symlink no builder. APK 0.9.12-lab/código 12 compilado, pacote/certificado verificados, provider/permissões/classes Java conferidos no APK. [Release publicada](https://github.com/SbitContainer/Battle-Royale-X/releases/tag/android-lab-12); downloads públicos de APK/catálogo/inventário conferidos por hash. [Registro exato](engenharia/releases/android-lab-12.json). Continuação: instalação ADB autorizada no Moto G54 terminou com Success, código 11→12 e activity/processo ativos. Porém logcat mostrou shader null em MobileAbilityAimPreview.Awake e ClassNotFoundException de AssetPackManager. Não corrigidos: pedido atual só instalação; solicitar autorização. Toque/visual/layout salvo e ciclo pelo atualizador do jogo continuam NÃO EXECUTADOS. Editor verde não invalida esse erro Android.

## Gameplay e mapa de interações

Execução persegue/golpeia por 18 e oculta por 2 s. Travessia lança adaga por 4 e permite segundo toque em 3 s para teleportar/golpear por 12. Duplo Passo foi substituído pelo Círculo de Adagas: cinco lâminas, 3 s, 2 de dano por pulso de 0,5 s, raio 1,8 m, 50% por golpe elegível. Esquiva Sombria, fumaça, Caçada, Retorno e outras classes preservados.

Inventário de 19 mecanismos existentes em [INTERACTION_RULES.md](INTERACTION_RULES.md); I20 identifica a nova órbita. [ABILITY_INTERACTIONS.md](ABILITY_INTERACTIONS.md) mapeia as 40 habilidades por ID/flags/códigos I. R00 preserva compatibilidade antiga; R01/R02 são perfis reutilizáveis de elegibilidade. Nenhuma ultimate foi excluída automaticamente; lista R02 da órbita vazia aguardando decisão do usuário.

Registro e limites da tarefa de gameplay anterior: [BRX-COMBAT-002](engenharia/tarefas/BRX-COMBAT-002.md). Atualização de dados limitada às três variantes, sem rebuild de cena. Naquela tarefa: 237 casos de combate/UI passaram, zero falhas, 48 verificações estruturais visuais passaram; sem APK, instalação ou teste de toque. A tarefa atual reexecutou a matriz e prepara release separadamente. Relatório em [TEST_RESULTS_LAB_001.md](TEST_RESULTS_LAB_001.md). Estado do gameplay: VALIDADO_EDITOR / AGUARDANDO_VALIDACAO_ANDROID.

## Estado

Governança local adicionada, preservando a arquitetura Unity. Entrada: AGENTS.md → PROJETO.md → MAPA.md → política do setor. Relatório da adoção em [engenharia/ADOCAO.md](engenharia/ADOCAO.md). O mapa possui referências verificáveis em `engenharia/mapa.json`; ferramentas ficam fora de Assets, em `Tools/Engineering`.

Baseline: branch main, HEAD d2b0b067fe7613523c1d740351699f7a7fe63d9c; mudanças anteriores de Mago/Arqueiro, assets e dados preservadas. Não sincronizado com remoto nesta tarefa.

## Evidências e limites

A entrega anterior [EFFECTS_LAB_092.md](EFFECTS_LAB_092.md) registra 214 testes de combate/UI, 48 verificações da cena e instalação da versão 0.9.2-effects-lab em 26/09. São resultados históricos; não foram reexecutados como parte da adoção documental. Não representam aceite humano de todas as skills ou qualidade visual final.

As ferramentas novas validam referências e integridade de bytes; não aprovam gameplay nem substituem assinatura de APK. Componentes de rede/sync do kit foram classificados, não instalados no runtime.

## Próximo passo seguro

APK 12 já instalado por ADB a pedido do usuário. Pedir autorização para corrigir o erro real de shader do indicador de mira; investigar separadamente o aviso/erro AssetPackManager antes de classificá-lo ou mexer em dependências. Validar toque/visual e preservação do layout com usuário. Em uma futura versão maior autorizada, testar também o ciclo pelo atualizador do jogo. Não gerar versão vazia para afirmar validação física nem desinstalar para contornar certificado incompatível.

Pendências: validação humana do Mago/Arqueiro, medição real de FPS/memória e reprodutibilidade do lockfile. Builder novo grava hashes da fonte real, não backup recuperável. Não ampliar escopo para resolver pendências automaticamente. A adoção documental anterior não autorizava release; a autorização de build/canal da BRX-RELEASE-003 veio do pedido atual.
