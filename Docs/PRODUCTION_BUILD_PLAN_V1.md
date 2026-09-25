# Battle Royale X — Plano de Construção V1 (ChatGPT → Codex Sol → Astra)

## Meta desta fase

Entregar um laboratório Unity onde seja possível:

- jogar com Guerreiro, Assassino, Mago e Arqueiro;
- trocar classe sem reiniciar;
- trocar Skill 1 A/B/C;
- trocar Skill 2 A/B/C;
- trocar Ultimate A/B/C;
- testar Evolução Titânica visualmente;
- editar valores via ScriptableObject;
- usar personagens 3D e animações gratuitas;
- visualizar VFX claros de todas as habilidades;
- testar objetos arcanos básicos;
- manter tudo data-driven.

Não construir ainda:
- multiplayer;
- mapa BR completo;
- bots híbridos;
- Titã real;
- Guardiões completos;
- matchmaking/backend.

---

# Divisão de responsabilidade

## Aqui / planejamento
- arquitetura;
- catálogo de habilidades;
- nomes/IDs;
- VFX spec;
- assets selecionados;
- estrutura de pastas;
- testes;
- tarefa do Codex.

## Codex Sol
- refatoração C#;
- ScriptableObjects;
- editor tooling;
- painel de laboratório;
- classe switch;
- execução das habilidades;
- hooks de VFX;
- placeholders procedurais;
- testes determinísticos possíveis;
- atualizar documentação.

## Astra
- abrir Unity real;
- importar assets;
- retarget;
- Animator;
- materiais;
- VFX real;
- Play Mode;
- corrigir erros de versão/API;
- polimento visual.

---

# FASE 0 — preservar gameplay atual

Antes de refatorar:
- compilar;
- registrar comportamento Guerreiro/Assassino;
- não perder Clash/Guard/Parry/Dodge;
- criar commit de segurança se necessário.

---

# FASE 1 — nova arquitetura de slots

Substituir arquitetura final:
- BasicAttack;
- Skill1;
- Skill2;
- Ultimate.

Manter adaptadores temporários para dados antigos se isso reduzir risco.

Criar enums/estruturas:
- AbilitySlot.BasicAttack
- AbilitySlot.Skill1
- AbilitySlot.Skill2
- AbilitySlot.Ultimate

Skill1/Skill2 aceitam qualquer Behavior válido.

CharacterDefinition deve possuir:
- basicAttack;
- skill1Variants[3];
- skill2Variants[3];
- ultimateVariants[3].

No laboratório todos começam equipados para teste.
No BR futuro o loadout começa vazio.

---

# FASE 2 — comportamentos genéricos

A engine precisa suportar, no mínimo:
- MeleeAttack;
- ProjectileAttack;
- SeekingProjectile;
- AreaAttack;
- Guard;
- Parry;
- Dodge;
- Dash;
- DashThrough;
- DashReturn;
- Blink;
- Repulsion;
- SlowField;
- TrapPull;
- CloneTeleport;
- MultiDash;
- TimedBuff;
- MultiShot;
- ComboProjectileUltimate.

Evitar criar uma classe C# exclusiva para cada habilidade quando um comportamento data-driven resolve.

Quando uma habilidade exigir lógica realmente única, criar executor/componente específico pequeno.

---

# FASE 3 — tags de interação

Adicionar flags/tags:
- Physical;
- Magical;
- Projectile;
- Melee;
- Area;
- Seeking;
- Interceptable;
- Reflectable;
- Amplifiable;
- Nullifiable;
- Heavy;
- FragmentTrigger.

Guerreiro:
- interação de interceptação por timing;
- referência: 60% redução em projétil interceptável.

Mapa futuro usa as mesmas tags.

---

# FASE 4 — catálogo das 4 classes

Implementar `Docs/ABILITY_CATALOG_V1_TEST.md`.

Gerar ScriptableObjects automaticamente por menu Editor.

Todos os valores:
- damage;
- cooldown;
- startup;
- recovery;
- speed;
- tracking;
- slow;
- pull;
- radius;
- iframe;
- duration;
- etc.
devem ser editáveis no Inspector.

---

# FASE 5 — laboratório

Criar cena/painel:

Controles:
- Classe: Warrior / Assassin / Mage / Archer;
- Skill1: A / B / C;
- Skill2: A / B / C;
- Ultimate: A / B / C;
- Titan Evolved: on/off;
- Reset;
- Spawn dummy/bot Guerreiro;
- HP/energia/cooldowns visíveis.

Trocar classe:
- substitui CharacterDefinition;
- troca visual profile;
- reinicializa stats;
- limpa estados/cooldowns;
- reaplica loadout selecionado.

Não recarregar cena.

---

# FASE 6 — camada visual desacoplada

Criar:
- AbilityVisualProfile;
- CharacterVisualProfile;
- CombatVFXRouter / AbilityVFX hooks;
- prefab slots.

Gameplay funciona mesmo com prefab visual null.

Criar placeholders procedurais para todos os VFX, mesmo antes dos assets externos:
- primitive meshes;
- ParticleSystem;
- TrailRenderer;
- LineRenderer.

Assim o projeto compila/testa sem depender dos downloads.

---

# FASE 7 — personagens/animações

Astra:
- Quaternius base/outfits;
- mesmo Humanoid rig;
- Warrior sword+shield;
- Assassin dual blades;
- Mage staff/orb;
- Archer bow.

Animator mínimo:
- Idle;
- Move;
- Basic;
- Skill1;
- Skill2;
- Ultimate;
- HitReaction;
- Death.

Usar override/controller/profile por classe para manter código genérico.

---

# FASE 8 — VFX

Seguir `Docs/VFX_VISUAL_SPEC_V1.md`.

Primeiro:
- hit;
- blood;
- block;
- parry;
- clash.

Depois:
- classe por classe.

Não alterar timing da hitbox só para combinar com animação.
Ajustar Animation Event/visual timing ao gameplay aprovado.

---

# FASE 9 — mini arena visual

Ainda não fazer mapa BR.

Criar uma arena de laboratório bonita com:
- chão;
- ruínas;
- algumas moitas;
- 1 Parede de Fase;
- 1 Parede Prismática;
- 1 Barreira de Amplificação;
- 1 Cristal de Fragmentação;
- 1 Runa de Velocidade.

Objetivo:
testar interação visual/mecânica.

---

# FASE 10 — validação

Matriz mínima:
- cada classe troca corretamente;
- 4 ataques básicos;
- 24 skills normais (4 classes × 2 slots × 3 opções);
- 12 Ultimates;
- VFX não muda dano;
- clone não duplica hitbox;
- seeking não vira inevitável;
- slow não vira stun;
- trap pull ainda permite movimento/skill;
- Guerreiro intercepta somente Interceptable;
- Assassino consegue esquivar;
- combo do Mago funciona;
- cooldown maior em sucesso do combo;
- cooldown menor em falha;
- Arqueiro trap funciona;
- troca em runtime não deixa coroutine antiga ativa.

---

# Entrega do Sol

O Sol deve finalizar com:
- projeto compilável;
- todos os scripts;
- geradores de dados;
- placeholders de VFX;
- laboratório funcional;
- documentação atualizada;
- lista clara do que exige Unity/Astra.

Não gastar tempo escolhendo asset visual final dentro do Sol.
