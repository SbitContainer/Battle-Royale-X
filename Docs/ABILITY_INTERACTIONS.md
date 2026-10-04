# Mapeamento das interações de habilidades

Gerado pelos dados reais; regenerar pelo menu Export Ability Interaction Map. IDs não mudam com o nome visual. Catálogo numerado dos mecanismos existentes em [INTERACTION_RULES.md](INTERACTION_RULES.md).

Código 1: aceita todos os ataques. Código 2: aceita todos EXCETO os IDs da lista do perfil. Código 0: flags antigas preservadas, sem perfil atribuído. Elegibilidade não define chance, redução, reflexão ou imunidade. Guarda/parry/reflexão ainda exigem suas flags; não foram rebalanceados. Círculo de Adagas usa código 2, com 50% por golpe elegível. Exceções aguardam decisão do usuário; lista vazia não exclui nenhuma ultimate.

Códigos I abaixo são capacidades/configuração; a interação concreta ainda exige as condições de contato, fase e cooldown do catálogo.

| ID | Nome | Comportamento | Dano | Bloqueável | Parry | Refletível | Anulável | Código de defesa | Exceções | Interações I |
|---|---|---|---:|---|---|---|---|---:|---|---|
| Archer_Basic | Disparo Preciso | ProjectileAttack | 8 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I07, I09, I14, I15, I16 |
| Archer_S1_A | Flechas Rastreadoras | SeekingProjectile | 4 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I07, I09, I14, I15, I16 |
| Archer_S1_B | Flecha Pesada | ProjectileAttack | 20 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I07, I09, I14, I15, I16, I18 |
| Archer_S1_C | Armadilha Gravitacional | PullTrap | 4 | True | True | False | False | 0 | — | I01, I02, I18 |
| Archer_S2_A | Recuo Ofensivo | Dash | 4 | True | True | False | False | 0 | — | I01, I02, I07, I14 |
| Archer_S2_B | Gancho de Reposição | Grapple | 5 | True | True | False | False | 0 | — | I01, I02, I06 |
| Archer_S2_C | Passos Laterais | MultiDash | 0 | True | True | False | False | 0 | — |  |
| Archer_Ult_A | Rajada Perfurante | MultiShot | 8 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I07, I09, I14, I15, I16 |
| Archer_Ult_B | Sobrecarga Cinética | TimedBuff | 0 | True | True | False | False | 0 | — |  |
| Archer_Ult_C | Chuva de Flechas | ArrowRain | 5 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I09, I18 |
| Assassin_Basic | Corte Rápido | MeleeAttack | 15 | True | True | False | False | 0 | — | I01, I02, I06, I08 |
| Assassin_Defense_A | Contra-Sombra | SmokeEscape | 0 | True | True | False | False | 0 | — | I12 |
| Assassin_Defense_B | Círculo de Adagas | OrbitingDaggers | 2 | True | True | False | False | 2 |  | I01, I02, I18, I20 |
| Assassin_Defense_Base | Esquiva Sombria | Dodge | 2 | True | True | False | False | 0 | — | I01, I02, I11, I03 |
| Assassin_Move_A | Travessia | DaggerTeleport | 4 | True | True | False | False | 0 | — | I01, I02, I07, I14 |
| Assassin_Move_B | Retorno | DashReturn | 24 | True | True | False | False | 0 | — | I01, I02, I06 |
| Assassin_Move_Base | Passo Fantasma | DashThrough | 24 | True | True | False | False | 0 | — | I01, I02, I06 |
| Assassin_Ult_A | Execução | ExecutionStrike | 18 | True | True | False | False | 0 | — | I01, I02 |
| Assassin_Ult_B | Caçada | HuntSequence | 18 | True | True | False | False | 0 | — | I01, I02 |
| Assassin_Ult_Base | Cinco Cortes | ChargedDashSequence | 12 | True | True | False | False | 0 | — | I01, I02 |
| Mage_Basic | Orbe Arcano | ProjectileAttack | 9 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I07, I14, I15, I16 |
| Mage_S1_A | Faíscas Caçadoras | SeekingProjectile | 4 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I07, I14, I15, I16 |
| Mage_S1_B | Orbe Pesado | ProjectileAttack | 28 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I07, I09, I14, I15, I16, I18 |
| Mage_S1_C | Campo de Lentidão | SlowField | 3 | True | True | False | False | 0 | — | I01, I02, I18 |
| Mage_S2_A | Blink | Blink | 6 | True | True | False | False | 0 | — | I01, I02, I07 |
| Mage_S2_B | Ecos Arcanos | CloneTeleport | 3 | True | True | False | False | 0 | — | I01, I02, I07 |
| Mage_S2_C | Pulso de Repulsão | Repulsion | 5 | True | True | False | False | 0 | — | I01, I02, I18, I19 |
| Mage_Ult_A | Convergência Arcana | ComboProjectileUltimate | 46 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I07, I09, I14, I15, I16 |
| Mage_Ult_B | Tempestade Arcana | AreaAttack | 7 | True | True | False | False | 0 | — | I01, I02, I18 |
| Mage_Ult_C | Prisma Fraturado | MultiShot | 9 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I07, I14, I15, I16 |
| Warrior_Basic | Corte Pesado | MeleeAttack | 13 | True | True | False | False | 0 | — | I01, I02, I06, I08, I18 |
| Warrior_Defense_A | Escudo Usurpador | WarriorSkillCapture | 0 | True | True | False | False | 0 | — |  |
| Warrior_Defense_B | Fortaleza | WarriorFortress | 14 | True | True | False | False | 0 | — | I01, I02, I07 |
| Warrior_Defense_Base | Guarda de Aço | Guard | 0 | True | True | False | False | 0 | — | I01 |
| Warrior_Move_A | Caçada Brutal | WarriorPursuitStrike | 18 | True | True | False | False | 0 | — | I01, I02 |
| Warrior_Move_B | Avanço Implacável | WarriorPursuitLong | 9 | True | True | False | False | 0 | — | I01, I02 |
| Warrior_Move_Base | Arremesso de Escudo | WarriorShieldCharge | 14 | True | True | False | False | 0 | — | I01, I02, I07 |
| Warrior_Ult_A | Domínio de Ferro | WarriorGroundField | 3 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I18 |
| Warrior_Ult_B | Ruptura Titânica | WarriorGroundWaves | 6 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I18 |
| Warrior_Ult_Base | Bastião Explosivo | WarriorGroundBlast | 18 | True | True | True | True | 0 | — | I01, I02, I04, I13, I05, I18 |
