# Diagnóstico e testes do Battle Royale X

Adaptado de BUGS_E_TESTES e APRENDIZADOS do kit. Primeiro congele classe, slot/variante, versão instalada, modo do bot, distância, energia/cooldown, gesto, resultado esperado e proibido.

Rastreie: toque/teclado → evento de UI → ator selecionado → TryUse/rejeição → fase/timing → hitbox/campo → defesa/resolver → HP/estado → CombatEvents → apresentação. Localize o primeiro desvio. Efeito ausente não prova ausência de dano; teste de API não prova que o botão recebe toque.

| Camada | Evidência | Não comprova |
|---|---|---|
| Governança/tooling | `node Tools/Engineering/validate.cjs` e `node --test Tools/Engineering/engineering.test.cjs` | Gameplay/Unity |
| Editor/integração | PrototypeLiveTestLauncher.RunBatch e resumo final; PrototypeVisualValidation | Toque físico, FPS ou visual final |
| Interface | Eventos reais do EventSystem, drag/release/cancel e menu | Usabilidade no aparelho |
| Android | APK identificado, toque real, pausa/retomada, orientação, screenshots/logs | Todas as versões ou dispositivos |
| Release | Hash, tamanho, assinatura, pacote/versão e recibo de instalação | Aceite humano de diversão/balanceamento |

Manter `Docs/TEST_MATRIX.md` como referência de critérios, com divergências de versões explicitadas. Não assinalar todos os checkboxes porque uma suite terminou verde. Testes antigos de 26/09 continuam históricos até reexecução.

## Regressões obrigatórias conforme o setor

- Input: pressionar/segurar/arrastar/cancelar, dois dedos, menu aberto/fechado, tamanho de tela e layout salvo.
- Combate: dano único, defesa invulnerável, bloqueio/parry, projétil rápido, parede, alvo fora de alcance, energia e cooldown insuficientes, fim de recast.
- Classes/estado: A→B→A, trocar variante durante efeito, reset/morte, coroutines e objetos do owner removidos. Preservar Guerreiro/Assassino.
- Dados/editor: regeneração idempotente, GUIDs e referências válidos; verificar assets produzidos, não só o gerador.
- Visual: capturar instante correto, modelo ativo, contraste e áreas de resposta; medir frame time/memória em Android antes de otimizar.

## Erro recorrente

Se contradiz suite verde ou voltou após dois reparos, interromper remendos: confirmar esperado com o usuário, reproduzir o gesto isolado na plataforma afetada, registrar logs/estado e só então automatizar a causa. Sem aparelho: AGUARDANDO_VALIDACAO, não aprovação física.

Separar ambiente, fixture, contrato, produto e release. Guardar primeira falha; repetir só um caso não equivale à suite completa. Sementes/frame/timing devem ser registrados quando aplicáveis. `Time.captureDeltaTime` nos testes serve à repetibilidade, não benchmark de FPS. Nenhum teste novo de rede/conta é obrigatório enquanto essas funções não existem.
