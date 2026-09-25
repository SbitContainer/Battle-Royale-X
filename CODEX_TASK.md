# CODEX TASK CHANNEL — Battle Royale X

> Quando o usuário disser **"leia CODEX_TASK.md e execute"**, atualize a branch `main`, leia este arquivo inteiro e execute a tarefa sem pedir outro prompt.

# STATUS
- Task ID: `BRX-PROD-002`
- Estado: `READY`
- Modelo recomendado: `GPT-5.6 Sol`
- Raciocínio recomendado: `Medium`
- Objetivo: construir a base completa do laboratório V1 antes da integração visual final no Astra.

# COMANDO HUMANO MÍNIMO

> **Leia `CODEX_TASK.md` na branch `main` de `SbitContainer/Battle-Royale-X` e execute integralmente.**

# REFERÊNCIAS OBRIGATÓRIAS

Leia nesta ordem:
1. `Docs/COMBAT_PRINCIPLES.md`
2. `Docs/GAME_DIRECTION_ROADMAP.md`
3. `Docs/CLASS_ROSTER_AND_VARIATIONS.md`
4. `Docs/MATCHUP_MATRIX.md`
5. `Docs/ABILITY_CATALOG_V1_TEST.md`
6. `Docs/VFX_VISUAL_SPEC_V1.md`
7. `Docs/PRODUCTION_BUILD_PLAN_V1.md`
8. `Docs/FREE_ASSET_SOURCING_2026.md`
9. `Docs/TEST_MATRIX.md`

A especificação mais recente prevalece sobre código/README antigo.

# REGRA PRINCIPAL

Faça o máximo possível em código/editor tooling sem depender de asset externo.

**NÃO ALTERAR O GAMEPLAY ATUAL DO GUERREIRO E DO ASSASSINO.**
Eles já estão aprovados pelo usuário para esta fase.

Preservar:
- dano atual;
- cooldowns atuais;
- distâncias atuais;
- timings atuais;
- comportamento das habilidades;
- lógica de esquiva/retorno/caçada;
- lógica de guarda/parry/investida;
- sensação atual do 1x1.

Só é permitido mexer em Guerreiro/Assassino quando for estritamente necessário para:
- compatibilidade com a nova infraestrutura de slots;
- troca de classe no laboratório;
- hooks visuais/VFX;
- correção de bug comprovado.

Se a migração de arquitetura exigir adaptadores, preferir adaptadores/wrappers sem mudar comportamento.

Não tente redesenhar gameplay.

Não implementar nesta tarefa:
- multiplayer;
- mapa BR final;
- bots híbridos;
- Guardiões;
- Titã real;
- backend;
- matchmaking.

# FASE 0 — BASELINE

1. `git fetch origin`
2. `git checkout main`
3. `git pull origin main`
4. registrar erros de compilação pré-existentes, se Unity estiver disponível;
5. preservar Clash/Guard/Parry/Dodge existentes até substituição funcional.

# FASE 1 — MIGRAR ARQUITETURA DE SLOTS

Alterar `BRXTypes.cs`:
- `AbilitySlot.BasicAttack`
- `AbilitySlot.Skill1`
- `AbilitySlot.Skill2`
- `AbilitySlot.Ultimate`

Remover dependência final de slots rígidos Defense/Movement.

Alterar `CharacterDefinition.cs` para:
- `basicAttack`
- `skill1Variants` com exatamente 3 entradas A/B/C;
- `skill2Variants` com exatamente 3 entradas A/B/C;
- `ultimateVariants` com exatamente 3 entradas A/B/C;
- `visualProfile` opcional.

Durante migração, pode criar helper `GetVariant(slot,index)`.

No laboratório:
- carregar A inicialmente;
- permitir troca instantânea.

# FASE 2 — EXTENDER ABILITY DEFINITION

Adicionar campos somente quando necessários:

Identity:
- `variantIndex` 0/1/2;
- tags/flags de interação.

Interações:
- `interceptable`;
- `interceptDamageReduction` default 0.60;
- `seeking`;
- `amplifiable`;
- `fragmentTrigger`;
- `heavy`.

Movimento/controle:
- `turnRate`;
- `slowPercent`;
- `pullStrength`;
- `fieldRadius`;
- `secondActivationDelay`;
- `secondActivationWindow`.

Combo Ultimate:
- `comboSuccessCooldown`;
- `comboFailureCooldown`.

Impacto:
- `skillLockOnHit`.

Manter valores data-driven.

# FASE 3 — COMPORTAMENTOS GENÉRICOS

Suportar:
- MeleeAttack
- ProjectileAttack
- SeekingProjectile
- AreaAttack
- Guard
- Parry
- Dodge
- Dash
- DashThrough
- DashReturn
- Blink
- Repulsion
- SlowField
- PullTrap
- CloneTeleport
- MultiDash
- TimedBuff
- MultiShot
- ComboProjectileUltimate

Criar componentes pequenos quando necessário:
- `SeekingProjectileMover.cs`
- `SlowField.cs`
- `PullField.cs`
- `CloneTeleportController.cs`
- `MageConvergenceController.cs`

Evitar um script monolítico por habilidade.

# FASE 4 — INTERCEPTAÇÃO DO GUERREIRO

No `CombatResolver`:
- projétil `interceptable` pode colidir com ataque compatível do Guerreiro;
- referência inicial: reduzir 60% do dano que alcançaria o Guerreiro;
- habilidades explicitamente configuradas podem ser destruídas;
- não tornar toda magia interceptável;
- timing é obrigatório.

Preservar Clash físico.

# FASE 5 — CATÁLOGO DE TESTE

Atualizar `PrototypeDataFactory.cs` para gerar integralmente as habilidades NOVAS de:
- Mago;
- Arqueiro.

Para Guerreiro e Assassino:
- reaproveitar os ScriptableObjects/valores/comportamentos já existentes;
- apenas criar o mapeamento/adaptação necessário para aparecerem corretamente no laboratório;
- NÃO substituir números ou mecânicas atuais pelo catálogo conceitual.

Gerar para Mago/Arqueiro:
- ataques básicos;
- Skill1 A/B/C;
- Skill2 A/B/C;
- Ultimate A/B/C.

Preservar Guerreiro/Assassino como estão.

# FASE 6 — MAGO

Implementar:
- Faíscas Caçadoras;
- Orbe Pesado;
- Campo de Lentidão;
- Blink;
- Ecos Arcanos;
- Pulso de Repulsão;
- Convergência Arcana;
- Tempestade Arcana;
- Prisma Fraturado.

Convergência:
- primeiro projétil lento;
- segundo rápido;
- individualmente pouco dano;
- colisão correta entre ambos gera explosão forte;
- sucesso usa cooldown maior;
- falha usa cooldown menor.

# FASE 7 — ARQUEIRO

Implementar:
- Flechas Rastreadoras;
- Flecha Pesada;
- Armadilha Gravitacional;
- Recuo Ofensivo;
- Gancho de Reposição;
- Passos Laterais;
- Rajada Perfurante;
- Sobrecarga Cinética;
- Disparo de Ruptura.

Armadilha:
- puxa ao centro;
- não stun;
- alvo mantém movimento/skills.

# FASE 8 — PRESERVAR GUERREIRO / ASSASSINO

Não recriar nem rebalancear as habilidades atuais do Guerreiro e do Assassino.

Objetivo desta fase:
- somente adaptar a infraestrutura para que eles continuem funcionando dentro do novo laboratório;
- preservar exatamente o comportamento atual aprovado;
- mapear as habilidades existentes aos slots de teste sem alterar sua mecânica;
- conectar VisualProfiles/VFX sem alterar hitbox, dano, cooldown ou timing.

Não implementar agora novas versões de:
- Guarda Retaliação;
- Guarda Arcana;
- novas Ultimates do Guerreiro;
- novas versões de Esquiva/Duplo Passo/Contra-Sombra;
- novas Ultimates do Assassino;
- qualquer rebalanceamento sugerido em documentos conceituais.

Os trechos de Guerreiro/Assassino em `Docs/ABILITY_CATALOG_V1_TEST.md` devem ser tratados como referência futura/visual, NÃO como autorização para substituir a implementação atual.

Se existir conflito entre catálogo e runtime atual:
> PRESERVAR O RUNTIME ATUAL DO GUERREIRO/ASSASSINO.

# FASE 9 — LABORATÓRIO

Criar `PrototypeLabController.cs`.

Painel deve permitir, em runtime:
- selecionar Warrior / Assassin / Mage / Archer;
- selecionar Skill1 A/B/C;
- selecionar Skill2 A/B/C;
- selecionar Ultimate A/B/C;
- ativar/desativar Titan Evolved somente como flag visual;
- resetar HP;
- resetar energia;
- resetar cooldowns;
- respawn/reinitialize;
- manter adversário Guerreiro inicialmente.

Troca de classe:
- cancelar coroutines da classe anterior;
- cancelar hitboxes/projéteis dependentes do antigo owner quando apropriado;
- limpar estados temporários;
- resetar stats/cooldowns;
- aplicar CharacterDefinition nova;
- atualizar visual profile.

Sem reload de cena.

# FASE 10 — INPUT

Protótipo:
- BasicAttack = F / equivalente atual;
- Skill1 = G;
- Skill2 = H;
- Ultimate = R.

No painel GUI também deve ser possível ativar/trocar para teste.

# FASE 11 — VIDA/CURA

Incluir decisões ainda não executadas:
- poção = 30 HP ao longo de 5 s;
- pode andar/atacar/usar skill;
- dano não cancela;
- nova poção substitui/renova HoT, não empilha infinito.

Regeneração natural:
- começa após janela fora de combate configurável;
- default inicial 8 s;
- cura gradual data-driven;
- causar/receber dano reinicia timer.

# FASE 12 — LOCKS

Separar:
- MovementLock;
- SkillLock;
- BasicAttackLock;
- Recovery.

Skill cancela Basic Attack imediatamente no mesmo input.

Ataque pesado Guerreiro:
- SkillLock alvo 0,10 s;
- movimento continua livre.

Não criar hard CC normal.

# FASE 13 — CAMADA VISUAL

Os seguintes arquivos já foram preparados e devem ser preservados/adaptados:
- `Runtime/Visual/AbilityVisualProfile.cs`
- `CharacterVisualProfile.cs`
- `CombatVFXLibrary.cs`
- `CombatVFXRouter.cs`
- `VisualProfileRegistry.cs`

Criar hooks para VFX específicos de habilidade.

Gameplay deve funcionar com profiles/prefabs null.

# FASE 14 — VFX PLACEHOLDERS PROCEDURAIS

Criar Editor tool:
`PrototypeVFXFactory.cs`

Menu:
`Battle Royale X > Visual > Create Prototype VFX`

Gerar prefabs simples usando:
- ParticleSystem;
- TrailRenderer;
- LineRenderer;
- primitives;
- materiais URP simples quando possível.

Criar pelo menos:
- Hit;
- BloodLight;
- BloodHeavy;
- Block;
- Parry;
- Clash;
- Dodge;
- Heal;
- Guerreiro Basic;
- Assassino Basic;
- Mago Basic;
- Arqueiro Basic;
- placeholder distinto para cada habilidade do catálogo.

Objetivo: TODA habilidade deve ter algum feedback visual mesmo sem assets externos.

# FASE 15 — BARRAS DE VIDA

Criar barra provisória de mundo:
- subscribe em HealthComponent.Changed;
- gameplay não depende da UI;
- funciona nas 4 classes;
- acompanha cura/dano.

# FASE 16 — OBJETOS ARCANOS DE DEMONSTRAÇÃO

Na arena de teste criar versões simples de:
- Parede de Fase;
- Parede Prismática;
- Barreira de Amplificação;
- Cristal de Fragmentação;
- Runa de Velocidade;
- Moita Reativa.

Não construir mapa modular final.

Implementar apenas o mínimo necessário para testar tags/interações.

# FASE 17 — SCENE BUILDER

Atualizar `PrototypeSceneBuilder.cs`:
- criar laboratório visual;
- player selecionável;
- adversário Guerreiro;
- câmera;
- HUD/painel;
- VFX router;
- barras;
- objetos arcanos;
- sem airdrop como foco principal.

Manter comando Editor para regenerar cena.

# FASE 18 — TESTES

Atualizar `Docs/TEST_MATRIX.md`.

Obrigatórios:
- compile;
- troca das 4 classes;
- 36 habilidades selecionáveis;
- 4 basics;
- combo Mago;
- tracking;
- slow;
- pull;
- clones;
- multi-dash;
- interceptação Guerreiro;
- cura HoT;
- regen fora de combate;
- barras;
- VFX null-safe;
- troca classe não deixa efeitos/coroutines antigos;
- objetos arcanos básicos.

# FASE 19 — ENTREGA

Não importar assets externos pesados nesta tarefa.

Ao finalizar:
- fazer commit;
- atualizar STATUS para DONE/PARTIAL/BLOCKED;
- preencher relatório;
- listar erros que só podem ser resolvidos dentro da Unity real;
- apontar explicitamente o que o Astra deve fazer em seguida lendo `ASTRA_TASK.md`.

# RELATÓRIO DO CODEX

Preencher ao final:
- Estado:
- Commit final:
- Arquivos criados:
- Arquivos alterados:
- Compilação disponível?:
- Testes executados:
- Testes aprovados:
- Falhas:
- Itens que exigem Astra/Unity:
- Próximo passo:
