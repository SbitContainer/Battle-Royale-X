# Battle Royale X — Matriz de Testes V1

## 1. Compilação/editor
- [ ] Nenhum erro C#.
- [ ] Menu Battle Royale X aparece.
- [ ] Gerador de dados cria as 4 classes.
- [ ] Gerador de VFX placeholder executa.
- [ ] Scene Builder cria laboratório.
- [ ] Perfis visuais podem ficar null sem quebrar gameplay.

## 2. Classes
- [ ] Guerreiro selecionável.
- [ ] Assassino selecionável.
- [ ] Mago selecionável.
- [ ] Arqueiro selecionável.
- [ ] Trocar classe não recarrega cena.
- [ ] Troca restaura HP/energia.
- [ ] Troca limpa cooldowns/estados antigos.
- [ ] Nenhuma coroutine/hitbox/projétil inválido da classe anterior permanece.

## 3. Slots
- [ ] Basic Attack fixo.
- [ ] Skill1 A/B/C.
- [ ] Skill2 A/B/C.
- [ ] Ultimate A/B/C.
- [ ] Terceiro slot normal não existe.
- [ ] UI mostra seleção atual.

## 4. Input/recovery
- [ ] Basic não executa simultaneamente com skill.
- [ ] Skill cancela Basic no mesmo input.
- [ ] SkillLock não implica MovementLock.
- [ ] Recovery não congela movimento sem necessidade.
- [ ] Cooldown impede spam.

## 5. Guerreiro
- [ ] Basic pesado acerta uma vez.
- [ ] SkillLock de 0,10 s não bloqueia movimento.
- [ ] Guarda Retaliação bloqueia e libera resposta.
- [ ] Parry exige timing.
- [ ] Guarda Arcana responde a projétil compatível.
- [ ] Investida funciona.
- [ ] Caçada corrige direção apenas no início.
- [ ] Avanço Protegido não é iframe total.
- [ ] Bastião Sísmico telegráfico.
- [ ] Domínio usa pulsos.
- [ ] Ruptura possui alto risco.
- [ ] Interceptação só funciona em Interceptable.
- [ ] Referência de redução por interceptação ≈60%.

## 6. Assassino
- [ ] Basic rápido funciona.
- [ ] Esquiva possui iframe curto.
- [ ] Duplo Passo executa 2 direções independentes.
- [ ] Intervalo de referência ~0,20 s.
- [ ] Contra-Sombra arma resposta.
- [ ] Passo Fantasma funciona.
- [ ] Retorno marca e retorna.
- [ ] Retorno pode expirar.
- [ ] Caçada não possui tracking infinito.
- [ ] Execução é evitável.
- [ ] Predação aumenta mobilidade sem burst inevitável.
- [ ] Véu Fantasma não gera invisibilidade perfeita longa.

## 7. Mago
- [ ] Basic Orbe Arcano funciona.
- [ ] Faíscas Caçadoras perseguem com dano baixo.
- [ ] Orbe Pesado é lento e alto dano.
- [ ] Campo de Lentidão reduz velocidade e não stun.
- [ ] Blink funciona na direção escolhida.
- [ ] Ecos criam 3 clones.
- [ ] Mago pode teleportar para clone.
- [ ] Mago pode optar por não teleportar.
- [ ] Pulso de Repulsão empurra sem hard stun.
- [ ] Convergência fase lenta funciona.
- [ ] Convergência fase rápida funciona.
- [ ] Colisão correto entre fases gera explosão.
- [ ] Sucesso aplica cooldown maior.
- [ ] Falha aplica cooldown menor.
- [ ] Tempestade permite sair da área.
- [ ] Prisma Fraturado cria leque/ângulos.

## 8. Arqueiro
- [ ] Basic Disparo Preciso funciona.
- [ ] Rastreadoras têm tracking e dano baixo.
- [ ] Flecha Pesada tem startup maior/dano maior.
- [ ] Armadilha puxa ao centro.
- [ ] Alvo da armadilha continua podendo andar.
- [ ] Alvo da armadilha continua podendo usar skills.
- [ ] Recuo Ofensivo desloca + dispara.
- [ ] Gancho exige ponto válido.
- [ ] Passos Laterais lê 2 direções.
- [ ] Rajada permite correção de mira.
- [ ] Sobrecarga aumenta cadência/movimento, não dano por flecha.
- [ ] Disparo de Ruptura possui telegraph.

## 9. Clash/defesa
- [ ] Clash físico continua funcional.
- [ ] Block funciona.
- [ ] Parry funciona.
- [ ] Dodge/iframe funciona.
- [ ] Nenhum controle normal cria chain hard CC.

## 10. Cura/regeneração
- [ ] Poção cura 30 HP / 5 s como referência.
- [ ] Pode andar durante HoT.
- [ ] Pode atacar durante HoT.
- [ ] Pode usar skill durante HoT.
- [ ] Dano não cancela HoT.
- [ ] Novo HoT substitui/renova, não empilha infinito.
- [ ] Regen natural começa após atraso configurável.
- [ ] Dano causado/recebido reinicia atraso.
- [ ] Regen natural para imediatamente ao entrar em combate.

## 11. HP/UI
- [ ] Barra acompanha Current/Max.
- [ ] Barra cai com dano.
- [ ] Barra sobe com HoT.
- [ ] Barra sobe com regen natural.
- [ ] Funciona em todas as classes.

## 12. VFX genéricos
- [ ] Hit.
- [ ] Blood Light.
- [ ] Blood Heavy.
- [ ] Block.
- [ ] Parry.
- [ ] Clash.
- [ ] Dodge.
- [ ] Heal.
- [ ] Nenhum VFX altera gameplay.

## 13. VFX habilidades
- [ ] Todas as 4 Basics têm feedback distinto.
- [ ] Todas as 12 Skill1 têm feedback.
- [ ] Todas as 12 Skill2 têm feedback.
- [ ] Todas as 12 Ultimates têm feedback.
- [ ] VFX null não quebra habilidade.
- [ ] VFX não esconde telegraph crítico.

## 14. Evolução Titânica
- [ ] Flag pode ser ativada no laboratório.
- [ ] Acompanha slot Ultimate.
- [ ] Trocar Ultimate mantém estado de evolução.
- [ ] Placeholder visual muda.
- [ ] Não alterar balanceamento final sem dados aprovados.

## 15. Objetos arcanos demo
- [ ] Parede de Fase: personagem atravessa.
- [ ] Parede de Fase: projétil compatível não atravessa.
- [ ] Prismática: reflexão limitada.
- [ ] Amplificação: não acumula múltiplas vezes.
- [ ] Cristal fragmenta.
- [ ] Runa de Velocidade funciona.
- [ ] Moita pode desaparecer/retornar no placeholder.

## 16. Performance/regressão
- [ ] 20+ VFX simultâneos não quebram lógica.
- [ ] Sem spam de exceptions.
- [ ] Projectiles são destruídos corretamente.
- [ ] Clones não recebem gameplay indevido.
- [ ] Trocas de classe repetidas não vazam objetos.
