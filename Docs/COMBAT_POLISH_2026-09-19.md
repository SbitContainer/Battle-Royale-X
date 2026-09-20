# Combat polish — 19/09/2026

## Escopo

Roupas Ranger CC0 de Quaternius nos dois personagens, materiais separados para pele e roupa, efeitos procedurais URP e dano de habilidades de movimento. Nenhum multiplayer, nova classe ou hard CC.

Os modelos são do arquivo já baixado `Modular Character Outfits - Fantasy[Standard].zip`. Fonte: https://quaternius.com/packs/modularcharacteroutfitsfantasy.html. Licença preservada em `Assets/ThirdParty/Quaternius/FantasyOutfits/License_Standard.txt`. São roupas Ranger, não armaduras Knight. Modelos e animações anteriores foram preservados.

## Regras desta rodada

- Dash do Assassino: 24 de dano por alvo por passagem; retorno é outra passagem física, sem teleporte através de paredes.
- Esquivas do Assassino: 12 por contato no trajeto.
- Investidas do Guerreiro: 6 por contato.
- Defesas estacionárias: pulso curto de 4; ultimates: pulso de 8, além do buff existente.
- Valores provisórios. Guarda, parry, invulnerabilidade e equipes passam pelo resolvedor central existente.
- Caminhar normalmente não causa dano. A varredura considera o deslocamento real, não apenas o destino pretendido.
- Partículas com transparência radial suave, arcos de golpe, rastros de deslocamento, escudo e runa de ultimate. Os efeitos não aplicam dano.

## Arquivos desta rodada

- `Editor/PrototypeDataFactory.cs`: danos iniciais.
- `Editor/PrototypeVisualFactory.cs`: roupas e materiais.
- `Editor/PrototypePolishTools.cs`: preparação da cena, diagnóstico, captura e build local no Editor.
- `Editor/PrototypeVisualRuntimeSmoke.cs`: relatório persistido do smoke.
- `Runtime/Abilities/AbilityController.cs`: varredura de movimento, retorno físico e pulsos.
- `Runtime/Characters/CharacterMotor25D.cs`: callback de percurso e restauração de colisões.
- `Runtime/Combat/Hitbox.cs`: resolução compartilhada de contatos.
- `Runtime/Combat/CombatEventVfxPresenter.cs`: efeitos de combate.
- `Runtime/Debug/PrototypeLiveTests.cs`: cenários de travessia/defesa/parede.
- `Resources/ArcaneGlow.shader`, `Resources/ArenaStone.shader`.
- Modelos/texturas/licença em `Assets/ThirdParty/Quaternius/FantasyOutfits`, materiais, dados e cena gerados, e respectivos `.meta`.
- `ProjectSettings/ProjectSettings.asset`: `activeInputHandler` de Both para Input Manager, usado pelos controles existentes e compatível com Android.

Os caminhos abreviados acima são relativos a `Assets/BattleRoyaleX`. O checkout já continha outras alterações anteriores a esta rodada.

## Validação

- Matriz ao vivo final: 58 passaram, 0 falharam. Lista exata em `TEST_RESULTS_COMBAT_POLISH_2026-09-19.md`.
- Smoke visual final: 15 passaram, 0 falharam (`Logs/polish-smoke.txt`).
- Compilação C#, criação dos dados, geração e salvamento da cena: concluídos sem erros.
- Prévia real capturada em `Logs/combat-polish.png`, com roupas, rosto, partículas suaves e confronto durante as ultimates.
- Primeira execução: 53/5; corrigida margem de contato do dash do Guerreiro. Os quatro erros de airdrop eram isolamento do temporizador na inicialização do teste. Smoke inicial 14/1: corrigida medição por relógio do Editor, que não garante frames de jogo; repetido com tempo de jogo e resultado 15/0.
- Teclas físicas, interação multitoque, desempenho sustentado, balanceamento e aparência no Moto G54: não validados nesta rodada. Sem dispositivo ADB conectado.
- APK Android: `Builds/BattleRoyaleX-arcane-combat.apk`, build concluído com sucesso e zero erros (`Logs/polish-build.txt`). Não instalado nesta rodada: Moto G54 desconectado.
- APK inspecionado com aapt: `com.sbitcontainer.battleroyalex.prototype`, nome `Battle Royale X`, versão `0.1.0-dev`/1, ABI efetiva `armeabi-v7a`, mínimo API 26, alvo API 36. Arquivo de 47.637.501 bytes (~47,6 MB). É um pacote local de teste, não uma entrega ARM64 para publicação em loja.
- SHA256: `488E82342B95505622F1A275A69DF39717AD5FE52FC51C18DCFDB8F8C6D98D48`.
- A primeira tentativa de APK foi cancelada no aviso de Active Input Handling = Both. Corrigido para o sistema clássico usado por `Input` e `StandaloneInputModule`, sem migrar controles.

Os testes de movimento usam a API real do motor e verificam os mapeamentos de teclado; não simulam dedos na tela nem pressionam fisicamente WASD/setas. Testes verdes não representam aprovação artística final. O cenário, itens e UI ainda contêm elementos de protótipo.
