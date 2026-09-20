# Battle Royale X — controles e combate mobile V2

Data: 19/09/2026. Estado: ESPECIFICADO, NÃO IMPLEMENTADO.

Esta é a especificação para a próxima implementação com Sol, raciocínio alto. Os números abaixo são valores iniciais de teste, não balanceamento aprovado. A solicitação atual do usuário autoriza redesenhar os controles móveis, distinguir os movimentos das duas classes, melhorar apresentação e substituir a ultimate base do Assassino por cinco dashes manuais. As restrições iniciais de integração dos documentos históricos não impedem este escopo autorizado.

## 1. Entrega e limites

Entregar um APK local em que o usuário joga de Assassino contra Guerreiro bot: analógico flutuante, habilidades circulares em arco, mira de dash, personagens vestidos e armados visualmente, movimentos distintos e ultimate de cinco cargas. Preservar o treino contínuo e as barras de vida.

Manter Unity 6000.6.1f1 + URP, resolução central de combate, ScriptableObjects e filhos visuais separados da raiz lógica. Não acrescentar classes, multiplayer, contas, progressão, sistema de equipamentos ou infraestrutura OTA. Armas e proteção descritas aqui são somente apresentação fixa da classe, sem inventário/equipamento de atributos. Não comprar assets.

O produto desta etapa Astra é documentação. Não foram executados novos testes de Unity, nem compilado ou instalado novo APK nesta etapa.

## 2. Evidências do código atual

| Local / símbolo | Situação encontrada | Consequência para V2 |
|---|---|---|
| `Runtime/Input/PrototypeMobileTouchControls.cs`, `CreateJoystick` / `UpdateJoystick` | Base fixa com pivot no canto; coordenada local dividida pelo raio sem descontar o centro | Corrigir referência e adotar origem móvel; toque central não pode produzir movimento |
| Mesmo arquivo, `CreateButton` | `Image` sem sprite circular; áreas quadradas; início somente no pequeno painel | Desenhar círculo e usar região ampla separada para capturar movimento |
| `Runtime/Characters/CharacterMotor25D.cs`, `SetMoveInput` | Vetor mapeado diretamente para XZ | Converter o analógico pela câmera na camada de input, mantendo contrato de coordenadas mundiais para teclado/bot |
| `Editor/PrototypeVisualFactory.cs` | Um controlador para as duas classes; `MoveSkill = Roll`, `Ultimate = Sword_Heavy_Combo` | Mapear ações por classe e por fase; não usar rolamento para investida/dash |
| `Runtime/Characters/CharacterVisualAnimator.cs` | Mesmos triggers para qualquer combo; reações podem interromper habilidades | Apresentar passo e fase confirmados pela lógica e definir prioridades |
| `Runtime/Abilities/AbilityController.cs` | Ultimate base é `UltimateBuff`; recovery comum começa logo após iniciar a coroutine do dash | Criar sequência manual de cargas e acompanhar término real do movimento |
| Mesmo arquivo | Combo e outras habilidades podem continuar em paralelo; VFX recalcula startup | Cancelar a ação anterior com limpeza e emitir eventos nas fases efetivas |
| `Runtime/Combat/DefenseController.cs` | `Guard` não usa a janela perfeita; `Parry` parcial reduz sempre 50% | Guarda precisa ser explicitamente `Parry` para janela perfeita e respeitar redução configurada |
| `Runtime/Combat/CombatResolver.cs` | Parry aplica 0,32 s de stagger e 22% do dano de volta automaticamente ao atacar Guerreiro | Substituir retorno automático por oportunidade manual de contra-ataque descrita abaixo; reduzir interrupção própria do parry |
| `Runtime/Characters/PrototypeTrainingBot.cs` | Guarda com chance 4%; reset apenas restaura vida/energia e posição | Bot deve mostrar defesa com reação legível; reset precisa limpar ações e cargas |
| `Editor/PrototypeVisualRuntimeSmoke.cs` | Verifica pés do Guerreiro em dois instantes de um ataque | Resultado anterior 17/17 não valida todos os clipes, cinco dashes, multitoque ou acabamento |

Caminhos abreviados da tabela são relativos a `Assets/BattleRoyaleX/`. O checkout já tem muitas mudanças não commitadas: preservá-las. A raiz correta é `C:/Users/netor/OneDrive/Desktop/Battle Royale X`, não as subpastas `My project` ou `BTX`.

## 3. Analógico flutuante e multitoque

### 3.1 Geometria e captura

- UI em landscape e dentro de `Screen.safeArea`; usar um `SafeAreaRoot` comum. Atualizar ao mudar resolução/orientação.
- Canvas de referência 1920 x 1080, escala por altura. Coordenadas abaixo são unidades do Canvas dentro da área segura, não pixels físicos.
- Região para iniciar movimento: x de 0 a 48% da largura segura; y de 0 a 82% da altura segura. Controles de inventário e menus têm prioridade de raycast sobre ela.
- Essa região é invisível e raycastable. Base e knob são apenas visuais, sem raycast. Aceitar um único `pointerId` de movimento enquanto houver contato.
- No PointerDown livre, colocar a origem exatamente no ponto do dedo e iniciar com vetor zero. Base circular com raio visual 112, raio de deslocamento do knob R=88, knob com raio 42. O visual pode ser recortado pela margem segura; não deslocar a origem matemática para compensar bordas.
- Capturar aquele dedo até PointerUp/Cancel, mesmo saindo da região inicial. Um segundo dedo não substitui o primeiro nem pode virar dono de habilidade se começou no analógico.

### 3.2 Regra de acompanhamento

Converter posição de tela para o mesmo retângulo local em que a origem vive. Para dedo p e centro c: d=p-c. Se |d|>R, mover c para p-normalize(d)*R; depois recalcular d. Assim o analógico acompanha um arrasto longo e permite inverter sem voltar ao ponto inicial.

Usar zona morta de 10%: m=clamp01(|d|/R); intensidade = 0 se m<=0,10, senão (m-0,10)/0,90. Direção = normalize(d). Não aplicar outra suavização na UI; o motor já suaviza. A posição visual do knob é c+clampMagnitude(d,R).

Para mundo: projetar `camera.forward` e `camera.right` no XZ, normalizar as bases e combinar right*x + forward*y. Preservar intensidade analógica e limitar magnitude a 1. Na câmera isométrica atual, dedo para cima deve mover para cima da imagem. Resolver câmera uma vez, sem `Camera.main` repetido por toque.

No PointerUp/Cancel: zerar entrada e ocultar/recolher base. Em perda de foco, pausa do aplicativo, desativação ou reset: limpar TODOS os ponteiros, mira, buffers e usar `StopMovementImmediately`. Nunca ficar andando depois de retornar ao app. Não interromper o movimento do dedo esquerdo ao tocar habilidade com o direito.

## 4. Botões circulares e mira

### 4.1 Arco inferior direito

Pivot central para todos os botões. Centro do ataque A=(largura segura-145, 155). Organizar as três habilidades num arco de raio 205 em torno de A; ângulos medidos do eixo horizontal para a direita, em sentido anti-horário.

| Botão | Posição em relação a A | Raio visual | Função |
|---|---|---:|---|
| Ataque | (0, 0) | 82 | Golpe / próximo golpe do combo |
| Defesa | 205 * (cos 180°, sin 180°) | 58 | Esquiva do Assassino / guarda do Guerreiro |
| Movimento | 205 * (cos 135°, sin 135°) | 58 | Dash / investida |
| Ultimate | 205 * (cos 90°, sin 90°) | 65 | Cinco Cortes / Postura de Guerra |

Isso deixa os centros das habilidades separados por aproximadamente 157 unidades. Área de toque circular = raio visual + 10; não permitir sobreposição. Criar malha/sprite circular com borda suavizada, ícone simples próprio e anel de cooldown; não basta colorir uma `Image` retangular. Pode usar UI procedural nativa do projeto, sem compra/download obrigatório. O teste circular deve ser um filtro de raycast geométrico, independente de textura legível na CPU.

Itens: quatro botões compactos centralizados embaixo, fora do arco. Diminuir painéis superiores no modo mobile para vida/energia/nome; remover instruções de teclado no celular. Textos decorativos, anéis e labels não interceptam toques. Exibir vida também sobre os combatentes se isso não duplicar informação a ponto de poluir a tela.

### 4.2 Gestos

- Ataque básico: PointerDown faz uma tentativa. Segurar repete a intenção a cada 0,12 s, usando a fila limitada do combo; nunca produz dano diretamente nem ultrapassa o timing permitido. Soltar encerra repetição.
- Defesa: ativar uma vez no PointerDown. Não repetir ao segurar. Nome e ícone dependem da habilidade equipada: não chamar esquiva de BLOQ.
- Movimento e ultimate de dashes: PointerDown inicia mira; PointerUp executa uma única ação. Arrasto acima de 18 unidades define direção. Segurar exibe linha/corredor até o alcance permitido; paredes limitam a prévia. Não executar automaticamente enquanto segura.
- Para cancelar mira, arrastar ao círculo X exibido acima do grupo, separado da ultimate e dentro da área segura, e soltar. Perda de foco ou cancelamento de ponteiro também cancela sem gasto.
- Toque rápido sem arrasto em ação ofensiva: usar direção do analógico se intensidade >0,15; senão direção para inimigo vivo mais próximo até 6 m, sem atravessar parede; se não houver, `Motor.Facing`. Esse auxílio não garante acerto nem acompanha alvo depois do lançamento.
- Esquiva defensiva usa analógico ou `Facing`, sem busca de inimigo. Ataque básico sem analógico pode mirar o inimigo mais próximo até seu alcance+0,5 m; com analógico, respeitar direção manual.
- A mira direita não gira a raiz lógica enquanto é apenas prévia. Aplicar direção quando `TryUse` aceitar; rejeição não gasta, não muda facing, não toca animação.
- Pressionar ultimate durante viagem/recovery não enfileira outro dash. Feedback discreto de indisponibilidade; outro toque deliberado após a recuperação é necessário.

Cooldown e carga exibidos vêm de `AbilityController`; não manter relógio paralelo na UI. Cada slot tem no máximo um dono de ponteiro. Um dedo que começou no botão não vira analógico ao deslizar para a esquerda.

## 5. Kit base e números iniciais

Manter vida, energia e velocidade atuais: Guerreiro 125/90/4,9; Assassino 85/110/6,2. Dano físico mesmo quando o efeito parece mágico. Block/parry/dodge continuam resolvidos centralmente. Valores absolutos abaixo são antes dos modificadores e da defesa.

### 5.1 Combos básicos

Preservar danos e multiplicadores existentes. Explicitar startup/ativo/recuperação por passo, sem sobreposição acidental de hitboxes. O total do passo é a soma das três fases. Buffer aceita um único próximo passo nos últimos 0,18 s da recuperação; tolerar intenção antecipada por até 0,12 s, sem executar duas vezes. Interromper sequência ao não haver intenção válida. Cooldown existente começa no primeiro golpe; novo combo somente quando o anterior acabar E cooldown permitir.

| Classe | Dano A / B / C | Startup A / B / C | Ativo cada | Recuperação A / B / C | Cooldown inicial |
|---|---|---|---|---|---:|
| Assassino | 15 / 15,75 / 20,70 | 0,10 / 0,115 / 0,145 s | 0,10 s | 0,18 / 0,202 / 0,315 s | 1,20 s |
| Guerreiro | 13 / 14,56 / 20,15 | 0,15 / 0,173 / 0,218 s | 0,12 s | 0,25 / 0,28 / 0,438 s | 1,55 s |

Manter livre a locomoção normal durante ataque básico. Usar camada de parte superior para golpes com locomoção inferior quando tecnicamente viável, ou blends que acompanhem velocidade real; não arrastar corpo em pose congelada enquanto anda. Capturar direção do golpe no início e não girar hitbox depois de criada. Finalizador precisa ser visualmente diferente dos dois primeiros golpes.

### 5.2 Assassino

| Slot | Nome | Dano | Custo / cooldown | Execução |
|---|---|---:|---|---|
| Defesa | Esquiva Sombria | 8 por alvo | 14 / 5 s | Antecipação 0,03 s; passo baixo de 2,6 m em 0,16 s; recuperação 0,08 s; iframe só nos primeiros 0,12 s da viagem; atravessa personagens, respeita paredes |
| Movimento | Passo Fantasma | 24 por alvo | 10 / 5 s | Antecipação 0,06 s; dash com corte de 4,2 m em 0,18 s; recuperação 0,10 s; atravessa personagens; sem iframe |
| Ultimate | Cinco Cortes | 12 por alvo POR dash | 45 uma vez / 28 s | Cinco dashes manuais de 4,2 m, antecipação 0,06 s + viagem 0,16 s + recuperação 0,12 s; janela total de 6 s |

Não usar `Roll` para nenhum avanço base. A esquiva tem recuo de corpo curto; o dash ofensivo mostra lâmina e rastro direcional. Dano de esquiva é menor que o dash de ataque. Os cinco acertos da ultimate somam 60 contra 125 de vida do Guerreiro sem defesa; não usar automaticamente os 24 do movimento em cada carga.

### 5.3 Cinco Cortes — contrato completo

1. Primeiro toque/soltura válido inicia janela de 6 s, cobra 45 de energia, inicia cooldown de 28 s e executa o primeiro dash. Há cinco viagens no total, não uma ativação seguida de seis ataques. UI mostra as cinco marcas no início e quatro restantes ao consumir a primeira.
2. Próximos quatro dashes custam zero e são novos toques no MESMO botão. Cooldown já correndo não bloqueia recasts válidos. A primeira ativação sem energia é rejeitada sem abrir janela.
3. Intervalo mínimo entre inícios de casts: 0,34 s (= antecipação + viagem + recuperação). Não usar 6 s de invulnerabilidade, buff de dano ou corte em área na ativação. Movimento comum funciona entre casts.
4. Cada recast pode escolher direção nova. Capturar direção no cast; não fazer homing, teleporte atrás do inimigo, retorno automático ou rajada automática.
5. Cada viagem possui hitbox de varredura com registro próprio de alvos. Um alvo recebe no máximo UM resultado nessa viagem, inclusive block/dodge/parry; pode ser atingido novamente por outro dash manual. Não reiniciar esse registro a cada frame. Mesma equipe recebe zero.
6. Dano ocorre só ao cruzar o volume do inimigo no caminho realmente percorrido pelo motor. Não usar uma caixa cobrindo todo o alcance antes de mover. Usar consulta entre posição anterior e posição real para não perder alvos em 30 FPS.
7. Sem invulnerabilidade; todos os dashes são bloqueáveis e aparáveis. Guarda reduz dano; esquiva pode evitar; parry cancela a hitbox daquela viagem. Para evitar restaurar colisões dentro de outro personagem, a viagem já iniciada pode concluir geometricamente SEM dano; o próximo cast respeita a interrupção curta e a recuperação. Na conclusão usar separação mínima em espaço livre se houver sobreposição. Nunca atravessar parede para separar.
8. Dano comum recebido não cancela automaticamente a janela. Morte, reset de treino, desativação, troca da ultimate e expiração a encerram e limpam estado. Parry não apaga as cargas restantes. Uma viagem já aceita antes do prazo pode terminar; nenhuma nova é aceita depois de 6 s.
9. Quando gasta a quinta carga ou expira: esconder cargas/mira/aura e mostrar cooldown restante. Não recomeçar cooldown nesse instante. Consumível de recarga reduz relógio existente, mas não aumenta cargas nem estende janela.
10. Ao tocar contra uma parede ou errar alvo, a carga é consumida se o cast foi aceito; sem dano atrás da parede. Toque durante indisponibilidade ou cancelamento de mira não consome.
11. Troca de variante conserva cooldown do slot e termina janela anterior; não duplica buff nem recarrega cargas. Só `Assassin_Ult_Base` recebe este novo comportamento: Execução/Caçada continuam buffs de variante.

### 5.4 Guerreiro

| Slot | Nome | Dano | Custo / cooldown | Execução |
|---|---|---:|---|---|
| Defesa | Guarda de Aço | Sem dano automático ao apertar | 7 / 3,2 s | Ativação imediata; defesa 0,70 s; primeiros 0,12 s permitem um parry; depois reduz 75% do dano bloqueável |
| Movimento | Investida de Escudo | 6 por alvo | 10 / 7 s | Antecipação 0,10 s; avanço de 3 m em 0,24 s; recuperação 0,16 s; para no corpo/parede; sem iframe |
| Ultimate | Postura de Guerra | 8, uma vez no pulso inicial | 45 / 30 s | Preparação 0,24 s; pulso de raio real 1,8 m, ativo 0,12 s; postura por 5 s; redução de dano e controle continuam pelas defesas existentes |

**Guarda e contra-ataque.** Configurar base como comportamento `Parry`, com defesa parcial usando `damageReduction=0,75`. Consumir janela perfeita no primeiro parry daquela ativação. Enquanto protegendo, pode andar e virar; o protótipo mantém defesa em 360 graus para responder à travessia. Não desenhar apenas escudo frontal como se costas fossem vulneráveis: pequeno aro discreto acompanha a cobertura total.

Parry zera o golpe, cancela sua hitbox e aplica feedback de interrupção de 0,10 s ao atacante, sem nova cadeia de stun. Para Guerreiro, abre oportunidade de 1 s: o próximo básico aceito recebe +6 de dano antes da defesa do alvo, uma única vez, e usa animação/efeito de resposta curta. Não dispara automaticamente. A oportunidade não acumula, é consumida no início desse básico (mesmo se errar/cancelar), expira e é limpa no reset. Remover o retorno automático atual de 22% para não somar os dois mecanismos. Variantes de parry do Guerreiro também usam esta regra; Assassino não ganha o +6.

A ativação da guarda é imediata e libera o controlador de ações; a janela defensiva persiste separadamente no `DefenseController`. Aceitar um básico, movimento ou ultimate encerra essa proteção, mas preserva a oportunidade de counter até consumi-la ou expirar. Assim o Guerreiro escolhe entre continuar protegido e responder, e não espera os 0,70 s inteiros para contra-atacar. Uma nova ação rejeitada não encerra a guarda. O movimento normal pelo analógico não a encerra.

Usar a multiplicação normal de dano uma única vez sobre (dano básico + bônus). Bônus aplica apenas à primeira hitbox desse básico, nunca aos três golpes do combo. Fora da janela perfeita, `DefenseKind.Parry` usa a redução da própria AbilityDefinition, em vez de 50% fixos; atualizar testes das variantes cujo dado difere desse valor.

**Investida.** Escudo à frente, corpo inclinado, pés sem cambalhota. Baixo dano de contato; não empurra a vítima à força nem recebe invulnerabilidade. Sua defesa vem da ação Guarda, não de um efeito visual enganoso. Não pode sobrepor um dash já ativo.

**Postura.** Manter modificadores atuais: dano 1,12; velocidade 1; resistência a stagger 1,35; janela defensiva 1,10; cooldown de movimento 1. Aplicar buff após o pulso inicial, para o próprio pulso não receber o novo multiplicador. Usar círculo físico real (`OverlapSphere` com deduplicação e `CombatResolver`), não a caixa atual de `SpawnAreaHitbox` que alcança cantos além do raio visual. Pulso centralizado no personagem, bloqueável/aparável, sem knock-up. Ultimate mostra firmar escudo/arma e aura curta; não executar combo pesado longo sem golpes correspondentes.

A defesa base deixa de causar 4 de dano automático: isso concretiza o pedido de contra-ataque defensivo. Mantém-se dano baixo nos deslocamentos e dano maior na mobilidade ofensiva do Assassino. Não alterar dano das demais variantes sem necessidade de compatibilidade.

## 6. Regras de execução e integração

- Uma ação ativa por combatente, com fase conhecida (`Startup`, `Active/Travel`, `Recovery`, `Idle`). É uma extensão local do `AbilityController`, não um novo framework.
- Movimento/defesa podem cancelar um básico antes de acertar ou depois de sua janela ativa. Durante o ativo, aceitar no máximo uma intenção defensiva recente por 0,12 s e executar ao fim se ainda válida. Cancelar elimina fila e hitbox pendente; nada pode causar dano atrasado depois.
- Básico não começa durante antecipação/viagem de dash. Defesa normal não se sobrepõe a outra viagem. Ultimate não se sobrepõe a básico em execução; terminar/cancelar a ação conforme regras, sem criar coroutines concorrentes. Recasts da ultimate nunca são enfileirados.
- Recuperação bloqueia apenas novas ações comprometidas, não o analógico global. Não usar `LockInput` durante toda a janela de 6 s. Manter locks existentes de item/stagger balanceados; limpeza não pode liberar lock de outro dono.
- Cada fase revalida morte, cancelamento e id da ação antes de gerar hitbox/evento. Um stagger legítimo cancela o básico ainda pendente; não deixar uma coroutine antiga retomar e atacar. Efeito de Hit comum é somente apresentação e não cria stagger novo.
- O motor continua responsável pelo deslocamento. Expor término/cancelamento real para que recuperação e visuais acompanhem a viagem, inclusive se bater em parede. Não tratar `StartCoroutine(Dash)` como conclusão.
- Reset deve cancelar ações, hitboxes ativas, retorno armado, oportunidade de counter, cargas e buffs, soltar colisões ignoradas e limpar a UI; depois restaurar posição/vida/energia. Não usar `Initialize` completo para reset comum se isso apagar inventário/variantes. Disponibilizar método dedicado de reset transitório.
- Preservar o treino atual sem painel de morte e retomada após 2,5 s. Suprimir animação de morte no modo de treino móvel, manter barras legíveis e retorno claro da vida. Não mudar regras normais de morte usadas pelos testes. Não adicionar regeneração durante o combo: ela mascararia os danos que o usuário quer avaliar.
- Durante perda de foco: pausar relógio de treino junto com gameplay; UI usa relógio de gameplay para duração/cooldown. Ao voltar não disparar toque/ação guardada.

### 6.1 Alterações pequenas de dados/API

Adicionar `ChargedDashSequence` ao FIM de `AbilityBehavior` para preservar valores serializados. Em `AbilityDefinition`, dados mínimos `chargeCount=5`, `chargeWindow=6`; usar startup/movementDuration/recovery/damage/cooldown já existentes para o restante. Dados de contra-ataque podem ficar na definição de defesa (`counterBonusDamage`, `counterWindow`) para evitar constantes espalhadas.

Manter `TryUse(AbilitySlot)` para teclado/bot/testes. Acrescentar overload ou comando com direção explícita para input móvel. Expor leitura do estado de sequência, cargas restantes, janela e possibilidade de recast; UI não deduz prontidão só de `GetCooldownRemaining`.

Não guardar cargas runtime em ScriptableObject compartilhado. Registrar id da ativação/viagem para cancelamento e deduplicação. `PrototypeDataFactory` deve escrever explicitamente todos os campos relevantes das habilidades alteradas, pois reutiliza assets existentes que podem conservar valores antigos. Executar geração duas vezes precisa produzir os mesmos valores.

### 6.2 Eventos de apresentação

Estender payload de eventos ou emitir eventos de fase tipados com: habilidade/slot, classe da fonte, id da ação, fase, passo/carga, direção, duração e ponto real de contato. Preservar chamadas antigas com argumentos opcionais/overload. Não sobrecarregar o mesmo `value` para dano, duração e cargas sem identificação.

Animação e VFX recebem início, fase ativa, fim/cancelamento da lógica. Remover temporizadores duplicados que tentam reproduzir startup dentro de VFX. Dano continua vindo do controlador/resolver. Recast precisa gerar uma nova animação mesmo usando o mesmo estado. Prioridade: morte normal > viagem/ação confirmada > reação leve > locomoção; golpe recebido não troca dash por pose full-body no meio da viagem. No treino, morte não toca animação.

## 7. Animações, aparência e efeitos

### 7.1 Assets confirmados localmente

UAL1: `Assets/ThirdParty/Quaternius/UniversalAnimationLibrary/UAL1_Standard.fbx`.
UAL2: `Assets/ThirdParty/Quaternius/UniversalAnimationLibrary2/UAL2_Standard.fbx`.
Os nomes completos começam com `Armature|`.

| Ação | Clipe candidato existente | Regra visual |
|---|---|---|
| Idle armado | UAL1 `Sword_Idle`; Guerreiro também UAL2 `Idle_Shield_Loop` | Escolher pose que combine com arma/escudo, sem braços atravessando roupa |
| Locomoção | UAL1 `Jog_Fwd_Loop` / `Sprint_Loop` | Velocidade visual proporcional à velocidade real; sem sprint parado |
| Básicos A/B/C | UAL2 `Sword_Regular_A`, `Sword_Regular_B`, `Sword_Regular_C` | Três ações distintas; timing separado por classe |
| Recuperação A/B | UAL2 `Sword_Regular_A_Rec`, `Sword_Regular_B_Rec` | Usar somente se melhorar transição, sem alongar regra lógica |
| Investida Guerreiro | UAL2 `Shield_Dash` | Escudo à frente e parada pesada |
| Dash e recast Assassino | UAL2 `Sword_Dash` | Baixo, corte em avanço, pés retornam ao chão, sem roll |
| Esquiva Assassino | Trecho de deslocamento de `Sword_Dash` | Pouco rastro, sem efeito de golpe forte; não inventar um clipe sidestep ausente |
| Guarda/counter | UAL2 `Sword_Block`; A/B para resposta | Defesa sustentada só pelo tempo lógico; saída limpa |
| Ultimate Guerreiro | UAL2 `Shield_OneShot` | Candidato para firmar escudo; avaliar se o gesto corresponde ao pulso antes de usar |

Os nomes foram verificados nos `.meta`, mas a adequação artística dos clipes NÃO foi validada nesta etapa. Não alegar que uma animação de espada virou acrobacia de adagas por troca de nome. Não usar `Sword_Heavy_Combo` inteiro como animação genérica de ultimate.

Gerar controladores por classe OU controlador compartilhado com overrides e perfis por ação, evitando dois sistemas diferentes para o mesmo fim. Preferência: um esquema de estados compartilhado com overrides e perfis por classe. O gerador atual retorna quando já existe controller: precisa atualizar os assets existentes de forma idempotente, sem depender de apagá-los manualmente.

Inspecionar clipe no Editor: marcar contato e selecionar trecho. Sincronizar pré-contato/contato/retorno com as fases lógicas (recortes e velocidade por fase, se necessário). Não comprimir clipe completo de segundos em 0,16 s com aceleração extrema. Se não houver trecho adequado, registrar lacuna visual específica e manter a melhor pose compatível; não alterar dano para combinar com uma animação ruim.

Preservar root motion desligado e separação visual/raiz. Avaliar bake Y/pés e IK por clipe: ativar IK indiscriminadamente não prova pé plantado. Sem deslocamento vertical da raiz e sem rotação acumulada após repetir dash. Manter orientação final coerente com direção lançada.

### 7.2 Personagens

Os únicos outfits importados são `FantasyOutfits/Male_Ranger.fbx` e `Female_Ranger.fbx`; não há armadura pesada ou pacote de armas confirmado nesta pasta. Não prometer visual de personagem final de MOBA a partir desse inventário.

Nesta entrega: preservar roupas completas e rosto; ajustar materiais URP sem escurecer detalhes com tint excessivo. Guerreiro com identidade aço/bronze e silhueta mais larga através de escudo e proteção visual fixa; Assassino com roupa escura/violeta, lâmina curta e contraste nas mãos. Vincular armas/escudo aos ossos corretos, sem colliders de gameplay. Usar uma lâmina principal compatível com animação de espada; segunda lâmina pode ficar embainhada até existir gesto adequado.

Se não houver mesh disponível, criar meshes simples próprios com silhueta acabada (lâmina afilada/guarda/cabo; escudo de borda suavizada), evitando blocos/cápsulas como arma final. Não substituir o outfit por corpo base exposto. Ajustar enquadramento apenas se necessário para ler mãos e silhueta; ambos os combatentes precisam continuar visíveis.

### 7.3 Linguagem visual dos golpes

| Evento | Apresentação e duração inicial |
|---|---|
| Básico Assassino | Arco estreito violeta/ciano, 0,12–0,18 s, orientado pela lâmina; C mais marcado |
| Básico Guerreiro | Arco curto branco/dourado, 0,16–0,22 s; partículas de impacto concentradas |
| Dash Assassino | Rastro cônico de 0,10–0,16 s; no máximo duas silhuetas residuais, só se legíveis; início e fim distintos |
| Investida Guerreiro | Faíscas frontais no escudo e poeira rente ao chão; termina quando motor para |
| Ultimate Assassino | Cinco pequenas marcas de carga; cada dash consome uma; mesma trajetória clara, impacto mais luminoso; aura termina com janela/cancelamento |
| Ultimate Guerreiro | Pulso de chão com limite visual de 1,8 m e aura baixa de 5 s; corpo continua legível |
| Block | Faísca curta no contato + proteção discreta; sem explosão igual a dano pleno |
| Parry | Flash compacto branco/azul e arco de resposta dourado; sinal do counter por 1 s |
| Clash | Encontro de dois arcos no contato e dispersão curta; não explodir a arena inteira |

Usar shader URP com transparência suave, trails afilados e poucos materiais compartilhados. Não renderizar caixas de hitbox nem quads com bordas opacas. Basear direção/posição no evento confirmado e sockets visuais quando disponíveis; não no personagem vários frames depois. Evitar hit-stop global via `Time.timeScale` nesta versão, pois prejudica multitoque e temporização das cargas. Preservar limite atual de efeitos e medir custo antes de aumentar partículas. Meta inicial: 30 FPS sustentados no aparelho, sem declarar sucesso sem medição.

## 8. Bot de treino

Continuar perseguindo o jogador, respeitando alcance e paredes. Incluir reações com atraso mínimo de 0,20 s a uma antecipação visível, sem ler o toque antes de ocorrer e sem parry perfeito garantido. Ao detectar ultimate ativada, pode preparar guarda para as próximas cargas; não precisa reagir magicamente a todo dash de 0,16 s.

Após parry, aproximar/alinha e tentar básico dentro da janela do counter, pelas mesmas APIs do jogador. Não ignorar cooldown/energia. Adicionar modo determinístico de teste para guarda/parry e bot pausado no Editor; evitar aleatoriedade em testes de dano. Bot não vira um sistema novo de IA/navmesh.

## 9. Mapa de implementação

| Arquivo / área | Mudança prevista |
|---|---|
| `Runtime/Input/PrototypeMobileTouchControls.cs` | Safe area, captura por dedo, origem móvel, arco circular, mira/cancelamento, cooldown/cargas |
| Novo helper pequeno em `Runtime/Input/` se necessário | Geometria pura de joystick/layout ou raycast circular testável; não criar framework de input |
| `Runtime/Abilities/AbilityController.cs` | Fases/cancelamento local, intenção limitada de combo, direção explícita, cinco cargas, counter e reset |
| `Runtime/Data/AbilityDefinition.cs`, `Runtime/Core/BRXTypes.cs` | Dados de sequência/counter, enum append-only e eventos de fase |
| `Runtime/Characters/CharacterMotor25D.cs` | Notificar viagem concluída, recuperação/cancelamento limpos, restauração de colisões |
| `Runtime/Combat/DefenseController.cs`, `CombatResolver.cs` | Parry base, redução pelos dados, janela perfeita única e counter manual |
| `Runtime/Combat/Hitbox.cs`, `DamagePacket.cs` somente se necessário | Identidade do cast/contato e deduplicação mantendo contrato existente |
| `Runtime/Characters/CharacterRuntime.cs`, `CharacterStateController.cs` | Reset transitório coordenado, sem deixar buff/lock de coroutine interrompida |
| `Runtime/Characters/CharacterVisualAnimator.cs`, `Editor/PrototypeVisualFactory.cs` | Apresentação por ação/classe/fase, atualização de controllers existentes, aparência fixa |
| `Runtime/Combat/CombatEventVfxPresenter.cs` | Fases reais, efeitos distintos, cancelamento e limite de custo |
| `Runtime/Characters/PrototypeTrainingBot.cs` | Defesa legível, counter e reset |
| `Runtime/Debug/PrototypeCombatHUD.cs` | HUD mobile compacta, cinco cargas, oportunidade de counter e treino |
| `Editor/PrototypeDataFactory.cs`, `PrototypeSceneBuilder.cs` | Dados V2 explícitos, assets idempotentes e integração da cena |
| `Runtime/Debug/PrototypeLiveTests.cs`, `Editor/PrototypeVisualRuntimeSmoke.cs` | Regressões das regras novas e smoke representativo |
| Dados/cena/controller/material gerados | Regenerar pelo Editor; reportar todos os caminhos realmente alterados |

## 10. Verificação necessária para entregar

Registrar cada caso com PASSOU/FALHOU/NÃO EXECUTADO e evidência em `Docs/TEST_RESULTS_MOBILE_V2.md`. Relatórios de 58/58 e 17/17 são históricos, não evidência desta versão. Sem Unity/aparelho acessível, não declarar compilação ou teste físico aprovado.

### Controles

- C01: nove pontos de início na região esquerda: vetor inicial zero, sem salto/andar sozinho; fora da região não captura.
- C02: 360 graus, intensidade baixa/máxima, diagonal sem velocidade extra e direções coerentes com câmera.
- C03: arrasto longo move a base; reversão rápida troca direção sem voltar ao primeiro ponto.
- C04: dois dedos: mover+atacar, mover+mirar e mover+recast; soltar um não cancela o outro; terceiro dedo não rouba captura.
- C05: toque nos cantos transparentes de um botão não ativa; círculos não se sobrepõem; itens não iniciam analógico.
- C06: safe areas e layouts 16:9/20:9 nas duas orientações landscape, sem corte nem sobreposição.
- C07: cancelar mira não gasta; perder foco/desativar/rotacionar limpa entrada; retomar não produz ação atrasada.
- C08: analógico responde enquanto ultimate está armada; cada toque direito aceito gera apenas uma carga.

### Habilidades e regressões

- H01: custo 45 uma vez, primeira ativação já dá dash, cinco no total, nenhum sexto; sem energia não abre sequência.
- H02: cinco dashes acertando alvo imóvel e sem defesa causam 60 no total; cada dash causa 12 uma vez mesmo a 30 FPS. Mesma equipe/fora do trajeto recebem zero.
- H03: guarda reduz cada 12 a 3; iframe evita; parry perfeito causa zero na vítima, não causa dano automático no atacante e cancela o restante da hitbox daquela viagem.
- H04: parry perfeito só uma vez por ativação; +6 consumido no próximo básico, sem duplicar nos passos B/C; expira após 1 s; pode errar/ser defendido.
- H05: novas viagens não começam antes de 0,34 s, nem automaticamente ao segurar/spammar; cinco cargas não tornam personagem invulnerável.
- H06: janela de 6 s não reinicia por recast; cooldown conta desde primeiro cast; reduzir cooldown não repõe cargas; cast iniciado antes de expirar termina corretamente.
- H07: parede e obstáculo bloqueiam movimento/dano; restauração de colisão após travessia não prende combatentes; nenhuma passagem pela parede para separação.
- H08: movimento normal/defesa/básico entre cargas; cancelamento de combo não deixa hitbox atrasada; não há dois dashes concorrentes.
- H09: troca de ultimate, reset, morte/desativação limpam carga/buff/hitbox/retorno/counter sem apagar inventário indevidamente.
- H10: Guerreiro investe 3 m ou para no contato, causa 6 uma vez, sem roll/iframe; Assassino passa pelo corpo e causa 24 uma vez.
- H11: esquiva dura 0,16 s mas iframe termina aos 0,12 s de viagem; não herdar `Max(invulnerabilityDuration,movementDuration)` atual.
- H12: postura Guerreiro pulso circular real causa 8 uma vez, não acerta canto fora de 1,8 m; buff inicia/expira e volta à identidade.
- H13: matriz `Docs/TEST_MATRIX.md` completa; adaptar somente expectativas explicitamente alteradas (ultimate base Assassino, parry/counter, timing/dano base). Continuar testando `UltimateBuff` através das variantes; não apagar cobertura antiga para ficar verde.
- H14: Create Default Data + Build Test Scene duas vezes mantém referências e valores V2, sem duplicar armas/controle/Canvas/event listeners.

### Apresentação e aparelho

- V01: verificar as DUAS classes: idle, andar, cada golpe, guarda/esquiva, dash, ultimate e recuperação; repetir 20 viagens sem acumular offset/rotação/root motion.
- V02: frame do contato visual coincide com fase ativa, dentro de um frame a 30 FPS; VFX cancelado não aparece depois de interromper ação.
- V03: recasts sucessivos reiniciam animação corretamente; nenhuma ultimate toca combo longo sem golpes reais.
- V04: roupa, rosto, arma e escudo legíveis, sem partes expostas indevidas, sem clipping evidente, sem quadrados opacos em VFX.
- V05: capturar vídeo curto no Moto G54 com controle simultâneo e a sequência de cinco dashes; registrar avaliação visual separada do teste automático. ADB tap simples não comprova multitoque.
- V06: duelo 3 minutos no aparelho, medir frame time/FPS por ferramenta apropriada e observar pausas/temperatura; alvo 30 FPS, reportar medição real. Se não houver medição, marcar pendente.
- V07: treino recupera vida e permite recomeçar, não fica preso em morte e não mantém cargas/efeitos antigos.

## 11. Ordem e passagem de modelo

1. **Astra alto — concluído nesta etapa:** leitura do código, decisões, valores iniciais, riscos e critérios escritos. Não implementar agora.
2. **Sol alto — próximo trabalho:** implementar controles e teste de captura; em seguida ações/cargas/defesa; depois animações/visual/VFX; regenerar, executar matriz, gerar APK e instalar no dispositivo autorizado quando conectado. Usar checkpoints locais, sem pedir nova aprovação a cada arquivo. Um novo APK com versão identificável e relatório.
3. **Usuário no Moto G54:** validar conforto, direção, velocidade dos cinco toques, defesa e legibilidade. Sol corrige falhas localizadas e concretas.
4. **Astra alto somente se restar decisão difícil:** analisar vídeo/reprodução e dados, resolver conflito entre controle/animação/balanceamento e escrever ajuste limitado; então retornar ao Sol. Não consumir Astra em compilações e tentativas repetitivas por padrão.

É uma divisão recomendada de trabalho, não uma exigência técnica de modelo. O usuário controla a troca no seletor. Não criar agentes/tarefas extras automaticamente.
