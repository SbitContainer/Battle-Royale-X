# BRX-VISUAL-006 — Guerreiro: conjuração, escudos e alcance local

Pedido de 03/10/2026: melhorar acabamento visual de todas as habilidades do Guerreiro, sobretudo carga e impacto das ultimates; escudos retangulares translúcidos chanfrados; reduzir dano e alcance das ultimates ao redor do personagem.

## Contrato

Setor A: apresentação Warrior, factory das três ultimates, execução de área. Consumidores B/C: geometria de mira e informações técnicas no HUD; matriz Unity e shaders Resources incluídos no Android. Sem alteração de outras classes, regras de captura, imunidade, movimento, cooldown/energia ou atualização GitHub. Não gerar/publicar/instalar APK nesta etapa.

| Ultimate | Antes | Candidato |
|---|---|---|
| Bastião Explosivo | 28 dano; raio 2,8; centro até 5 m | 18 dano; raio 2,2; centro no Guerreiro; carga 0,85 s |
| Domínio de Ferro | 6 × 5 pulsos; raio 3,4; centro até 4 m | 3 × 5 pulsos; raio 2,5; centro na ativação; carga 0,75 s |
| Ruptura Titânica | 10 × 3 ondas; raio 4,5; centro até 4 m | 6 × 3 ondas; raio máximo 3; centro na ativação; carga 1 s |

Valores são bases sem modificadores e exigem todos os pulsos acertando. Área persistente não segue o personagem após liberação; conjuração acompanha-o até ativar. Lentidão e redução de dano preservadas.

## Apresentação e procedência

Pesquisa de referências: https://romilton11.itch.io/shields-pack (escudos CC0 declarados pelo autor), https://quaternius.com/faq.html (CC0), documentação Unity ParticleSystem Burst/Shape. Nenhum modelo externo incorporado: geometria original e materiais existentes, sem custo/licença nova/dependências. Contorno luminoso, face transparente, carga de energia, partículas e explosão devem ser exclusivamente visuais, nunca autoridade de dano ou vida útil do projétil.

Delegação delimitada autorizada por AGENTES: implementer escreve somente WarriorSkillPresentation; qa_reviewer leitura independente. Principal: gameplay, dados, consumidores, testes, registros e capturas. Sem subdelegação/modelo alterado.

## Evidência

Unity 6000.6.1f1 / URP 17.6.0, execução local com target Android (Editor, não APK): `PrototypeDataFactory.UpdateWarriorAndRunTests`, sem reconstruir cena. Primeira tentativa `Logs/visual006-matrix1.log` falhou na compilação: CS0136, variável `side` do novo gesto conflitava com variável existente. Corrigida para `chargeSide`; matriz não executou nessa tentativa. Revisão estática identificou giro de escudos de 180 graus antes do disparo; corrigido para uma volta completa no período de 1,5 s, preservando as direções iniciais de lançamento.

Segunda tentativa `Logs/visual006-matrix2.log`: **294 passaram / 0 falharam** em combate/interface, **48 passaram / 0 falharam** nas verificações estruturais visuais. Zero erros de compilação e cinco avisos obsoletos já existentes. Novos critérios: valores congelados das três ultimates, conjuração presente no startup, dano máximo de cada pulso, alvo distante não atingido por mira remota, Ruptura atinge alvo a 2,8 m somente na terceira onda, cancelamento da carga não causa dano, três escudos com mesh de sete vértices/face alfa <= 0,25 e sem colisores visuais. Testes existentes de captura, imunidade, movimento, básicos e demais classes preservados.

Capturas reais no Editor em `Logs/Warrior006`: Fortaleza, carga/liberação/pico das três ultimates; inspecionadas forma/transparência dos escudos, pose e áreas. A liberação capturada no primeiro frame não mostra todo o pico da explosão; adicionada captura 0,2 s depois. Terceira matriz `Logs/visual006-matrix3.log` também 294/0 e 48/0. As capturas revelaram outro apresentador legado, `CombatEventVfxPresenter`, que ainda sobrepunha Ring/Aura iguais aos efeitos exclusivos. Suprimido somente nos nove comportamentos dedicados Warrior e eventos de habilidade; sangue, morte, básicos e demais classes preservados. Quarta matriz/capturas em execução. `Logs` é ignorado pelo Git. Não transformar esses quadros em aceite de qualidade gráfica no aparelho.

Preservação: hashes SHA256 das 30 habilidades não Guerreiro antes/depois do gerador são idênticos. Sem nova dependência/asset externo, shader existente ArcaneGlow em `Assets/BattleRoyaleX/Resources` e BRXAimPreview em `Assets/Resources`. Materiais/meshes próprios liberados. VFX em projétil não destrói host/Hitbox. Conjuração vinculada à identidade da ação, versão do reset, classe e vida. Nenhum commit/push/APK/publicação/instalação nesta tarefa; Android/toque/FPS **NÃO EXECUTADOS / AGUARDANDO_VALIDACAO**.

Revisão independente `qa_reviewer`: AJUSTAR no primeiro candidato (giro); APROVADO estático após correção. `implementer` realizou apresentação/ground anchoring e encerrou; não são agentes permanentes. Principal corrigiu compilação/consumidores, executou e inspecionou evidência. Sem modelo alterado ou tokens estimados.

## Arquivos da etapa

- `Assets/BattleRoyaleX/Runtime/Abilities/WarriorSkillPresentation.cs`
- `Assets/BattleRoyaleX/Runtime/Abilities/AbilityController.Warrior.cs`
- `Assets/BattleRoyaleX/Runtime/Visual/CombatVFXRouter.cs`
- `Assets/BattleRoyaleX/Runtime/Combat/CombatEventVfxPresenter.cs`
- `Assets/BattleRoyaleX/Runtime/Characters/CharacterVisualAnimator.cs`
- `Assets/BattleRoyaleX/Runtime/Input/AbilityAimSolution.cs`
- `Assets/BattleRoyaleX/Runtime/Data/AbilityTechnicalInfo.cs`
- `Assets/BattleRoyaleX/Runtime/Debug/PrototypeLiveTests.WarriorRework.cs`
- `Assets/BattleRoyaleX/Editor/PrototypeDataFactory.cs`
- `Assets/BattleRoyaleX/GeneratedData/Abilities/Warrior_Ult_Base.asset`, `Warrior_Ult_A.asset`, `Warrior_Ult_B.asset`
- `Docs/ABILITY_INTERACTIONS.md` (exportação do catálogo atual), `Docs/TEST_RESULTS_LAB_001.md` (matriz atual), `Docs/HANDOFF.md`, `Docs/MAPA.md`, este registro.

Lista limitada ao trabalho desta etapa; árvore contém alterações anteriores de outras tarefas. Metas/GUIDs existentes preservados.

## Fechamento

### Publicação posteriormente autorizada

Usuário confirmou "pode publicar" após o fechamento local. Gerado APK da cena existente com `PrototypeApkReleaseBuild.BuildVersionedApk`, argumento `-brxApprovedTestsLog Logs/visual006-matrix4.log`; log `Logs/visual006-apk14.log`, zero erros. Código14 / 0.9.14-lab / 57.160.349 bytes. Shader ArcaneGlow e AimPreview compilados no Android. `Publish-GitHubApk.tests.ps1`: 10/0. Publisher sem `-Publish` confirmou pacote, código, certificado original e hashes; após autorização executado com `-Publish`. Recibo PUBLISHED_VERIFIED e API latest confirmam android-lab-14. APK, catálogo e inventário de hashes baixados anonimamente conferidos antes do recibo. [Registro durável](../releases/android-lab-14.json). APK anterior preservado. Sem instalação, teste físico, commit/push da fonte ou alterações novas de gameplay neste turno; árvore suja identificada pelo inventário, não pelo tag/HEAD isolado. Registro/HANDOFF atualizados além deste arquivo.

**VALIDADO_EDITOR / AGUARDANDO_VALIDACAO_ANDROID.** Candidato final com supressão das sobreposições aprovado na revisão estática independente. Quarta matriz `Logs/visual006-matrix4.log`: **294 passaram / 0 falharam**, visual estrutural **48 passaram / 0 falharam**, zero erros de compilação/exceções durante execução. Dez capturas finais: escudos e carga/liberação/pico de cada ultimate; inspecionadas no Editor. Formato dos escudos, transparência, volume de explosão, onda em raio progressivo e ausência de anel/aura legado duplicado conferidos. Esses quadros ainda são efeitos estilizados, não aprovação estética do usuário nem benchmark físico. Governança `node Tools/Engineering/validate.cjs`: PASS, 13 setores/53 referências/22 documentos/27 links, antes do fechamento. Sem APK novo: pedir autorização de geração/publicação no próximo passo.
