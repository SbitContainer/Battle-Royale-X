# Contrato atual do Battle Royale X

Inventário local de 03/10/2026. Este documento aplica o kit de engenharia ao laboratório existente; não modifica regras de combate.

- Raiz: `C:/Users/netor/OneDrive/Desktop/Battle Royale X`, contendo Assets, Packages e ProjectSettings.
- Git: `main`, HEAD observado `d2b0b067fe7613523c1d740351699f7a7fe63d9c`, origin `https://github.com/SbitContainer/Battle-Royale-X.git`. Árvore já continha alterações e arquivos não rastreados. Não foi feito fetch; alinhamento com remoto não comprovado.
- Engine: `ProjectSettings/ProjectVersion.txt` fixa Unity 6000.6.1f1; `Packages/manifest.json` declara URP 17.6.0, Input System 1.20.0 e Test Framework 1.8.0. Não atualizar versões nesta adoção.
- Produto: laboratório local 2.5D, Guerreiro, Assassino, Mago e Arqueiro, bot, variantes A/B/C, inventário/loot e apresentação URP. Android é a plataforma física de teste; Editor é outra camada.
- Android: pacote `com.sbitcontainer.battleroyalex.prototype`. Última entrega documentada: 0.9.2-effects-lab, versionCode 11, em 26/09/2026. Não confundir esse registro com verificação atual do aparelho.

## Fontes de verdade

| Informação | Fonte e limite |
|---|---|
| Regra desejada | Pedido humano vigente e especificação aprovada; divergência com código deve ser exposta |
| Catálogo e defaults | `Assets/BattleRoyaleX/Editor/PrototypeDataFactory.cs` gera ScriptableObjects em GeneratedData; alterações manuais podem ser sobrescritas |
| Execução por personagem | AbilityController, CharacterRuntime, HealthComponent, EnergyComponent e CharacterStateController; não salvar cooldown/carga em ScriptableObject compartilhado |
| Acerto e defesa | Hitbox/Hurtbox, CombatResolver e DefenseController; apresentações não decidem dano |
| Visual | AbilityVisualProfile/VisualProfileRegistry, CombatVFXRouter, CharacterVisualAnimator e fábricas Editor |
| Layout salvo | PlayerPrefs, chave `BRX.MobileLayout.v4`; não equivale a save de partida, conta ou sincronização |
| Artefato entregue | Bytes do APK, manifesto/hash, pacote/versão inspecionados e evidência de instalação separada |

## Fronteiras preservadas

Usar os módulos existentes; o mapa descreve conexões, não exige nova camada, banco ou event bus. CombatEvents já oferece eventos de apresentação. Sem multiplayer/autenticação/servidor/outbox no escopo. `Docs/OTA_CONTENT_PLAN.md` é plano: Addressables não está declarado no manifest atual; não há OTA implementado por esta adoção.

Na adoção inicial, `.gitignore` ignorava `Packages/packages-lock.json`. Em BRX-RELEASE-008, o usuário pediu registrar todo trabalho no Git: o lockfile existente passa a ser rastreado byte a byte, sem resolver/atualizar dependências. SHA256 da entrada usada no APK15: `2e761a905e58eeefda4a979da21975e1458ff77754a7a15d33a2b83984614723`. Logs e Builds continuam ignorados: arquivar evidências sanitizadas necessárias em Docs; APKs ficam nas GitHub Releases, não no código-fonte.

## Comandos e autorização

Comandos locais de governança em [engenharia/BUGS_E_TESTES.md](engenharia/BUGS_E_TESTES.md). Testes Unity e builds em [engenharia/PUBLICACAO_E_ROLLBACK.md](engenharia/PUBLICACAO_E_ROLLBACK.md). Não executar geradores apenas para ler documentação: eles sobrescrevem dados/cena gerados.

Esta tarefa autoriza adaptação local da engenharia. Não inclui commit/push, publicação, novo APK, instalação, migrações ou reequilíbrio. Responsável por decisões de produto, aprovação física e distribuição: usuário. Nenhuma credencial do INDUSTRIOM foi transferida.
