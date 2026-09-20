# Próximo passo — Sol alto

Status: pronto para implementação; nenhuma funcionalidade V2 foi implementada pela etapa de especificação.

## Instrução para continuar nesta mesma conversa

Implemente `Docs/MOBILE_COMBAT_V2_SPEC.md` no projeto Unity da raiz `C:/Users/netor/OneDrive/Desktop/Battle Royale X`. A etapa Astra já examinou o código e fechou as decisões para o próximo APK. Use Sol com raciocínio alto. Preserve as alterações não commitadas; não trabalhe nas cópias `My project`/`BTX`.

Leia integralmente README, CODEX_START_HERE, COMBAT_PRINCIPLES, ASTRA_HANDOFF e TEST_MATRIX, além da especificação V2. A solicitação atual autoriza as mudanças de habilidades descritas em V2; os documentos de primeira integração são históricos onde divergirem. Não reinterpretar isso como autorização para multiplayer, equipamentos, OTA ou novas classes.

## Sequência de trabalho

1. Corrigir origem do analógico e implementar região ampla, seguimento do dedo, direção pela câmera, safe area, círculos em arco e multitoque. Validar geometria e captura antes de ligar às habilidades.
2. Implementar cinco recasts manuais, fases/cancelamento, dados idempotentes e guarda/parry/counter manual; testar dano único, parede, cooldown, energia e reset.
3. Integrar clipes UAL já presentes por classe/fase, roupas e armas visuais, efeitos sincronizados; registrar lacunas de asset com honestidade. Não fazer download/pesquisa ampla se os clipes existentes atendem.
4. Atualizar testes existentes onde as regras mudaram e executar a matriz V2 + regressões. Conferir apresentação no Editor e no aparelho separadamente.
5. Gerar APK com identificação de versão diferente da entrega anterior, manter package `com.sbitcontainer.battleroyalex.prototype` e assinatura compatível. Instalar com atualização preservando dados no Moto G54 se conectado (autorizado nesta conversa). Se estiver offline, pedir apenas a reconexão/dados ADB atuais; não usar códigos de pareamento antigos.
6. Entregar `Docs/TEST_RESULTS_MOBILE_V2.md` com testes exatos, falhas/pendências, arquivos realmente alterados, caminho/hash/versão do APK e confirmação de instalação quando verificada.

## Atenção aos pontos já diagnosticados

- O pivot do joystick é atualmente o canto: vetor local não é delta ao centro.
- Motor espera XZ mundial; converter câmera no input móvel, sem alterar teclado/bot.
- UI atual é retangular, apesar do comentário "MOBA layout".
- Controller existente retorna cedo na factory: editar só o caminho de criação não atualiza o projeto atual.
- Clipes reais incluem `Armature|Shield_Dash`, `Armature|Sword_Dash`, `Armature|Sword_Regular_A/B/C`, `Armature|Sword_Block`, `Armature|Shield_OneShot`.
- Coroutines de dash e recovery hoje podem se sobrepor; não presumir que método `Dash` retornando significa viagem concluída.
- `Dodge` usa hoje Max(iframe,duração); corrigir para respeitar os 0,12 s especificados.
- Parry atual devolve 22% automaticamente: retirar ao adicionar counter manual para não duplicar dano.
- API velha `TryUse(slot)` precisa continuar funcionando; enum novo entra no fim para preservar assets serializados.
- Cinco dashes têm 12 de dano cada, não os 24 de Passo Fantasma. Primeiro toque já executa um dos cinco.
- Smoke 17/17 anterior testava poucos instantes: não usar como aprovação visual de V2.

## Unity e recursos locais

Editor instalado/registrado no projeto: Unity 6000.6.1f1. Helpers existentes: `PrototypePolishTools`, `PrototypeLiveTestLauncher`, `PrototypeVisualRuntimeSmoke`. `Prepare Combat Polish` regenera cena, dados e visuais; salvar trabalho do Editor antes de usá-lo. Rodar um Editor por projeto, importante no notebook de 8 GB. Uma inspeção de processo/log deve preceder qualquer nova compilação longa.

O helper `PrototypePolishTools` usa `Temp/brx-polish-request.txt` e relatório `Logs/polish-command.txt`; comandos `prepare`, `test`, `smoke`, `build` com sufixo único. O estado OK de lançamento do teste NÃO é seu resultado final: ler o relatório do teste. Inspecionar requisição residual antes de reiniciar Editor, porque `SessionState` não sobrevive a toda reinicialização. Não deixar um pedido de build disparando de novo inadvertidamente.

Build atual fica em `Builds/BattleRoyaleX-arcane-combat.apk`; preservar pacote/assinatura, atualizar identificação de versão para distinguir a nova instalação. Verificar resultado real do build, arquivo e package instalado. Log de instalação sem saída não prova sucesso.

## Quando trocar modelo novamente

Não solicitar Astra para implementação rotineira, importação, compilação ou falha localizada com causa conhecida. Se um problema de comportamento persistir após duas correções com evidência, registrar reprodução e resultado esperado, e indicar Astra alto para diagnóstico limitado. Não baixar critérios de teste nem acumular reescritas sem observar o aparelho.

Mensagem curta que o usuário pode enviar após trocar: **"Pode implementar a V2 conforme Docs/SOL_NEXT_STEP.md e Docs/MOBILE_COMBAT_V2_SPEC.md, testar e preparar o APK para meu celular."**
