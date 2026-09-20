# Controles e habilidades mobile — V4

Pedido desta rodada: botões personalizáveis e com diâmetros duplicados; informações técnicas; Caçada em dois acionamentos; bônus da Travessia; Contra-Sombra como fumaça de fuga; esquiva reativa de 1 segundo; troca entre Guerreiro e Assassino jogador. A escolha confirmada é **golpes de fora podem atingir quem está na fumaça**.

## Interface

- MENU / SKILLS pausa o combate e mostra dados reais das quatro habilidades equipadas. Não é tooltip fixo desatualizado.
- EDITAR BOTÕES: arrastar ataque/defesa/movimento/ultimate/itens/pegar. Selecionar e usar MENOR/MAIOR ajusta tamanho entre 100% e 300% do tamanho antigo; padrão 200%.
- SALVAR persiste posição normalizada e escala via PlayerPrefs, por instalação. PADRÃO 2× restaura o arranjo inicial; salvar confirma. Não altera o analógico flutuante.
- TROCAR PERSONAGEM muda controle, entrada, perspectiva de ocultação e bot; mantém inventário e variações de cada personagem, cancela ações transitórias. Não reinicia a cena.

## Números iniciais para validação

- Caçada: 45 energia, 28 s recarga a partir do primeiro acionamento. Primeiro toque busca inimigo visível até 12 m, persegue a 22 m/s por até 1,5 s, causa 6 e empurra 2 m. Segundo toque, até 5 s depois da chegada, persegue o mesmo alvo e atravessa, causa 18 e termina 3,5 m além dele no sentido origem→alvo. Sem novo custo. Um dano por etapa. Dash do alvo não rompe a perseguição; morte/ocultação interrompem e paredes bloqueiam. Sem perseguição infinita ou teleporte.
- Travessia: mantém dano e avanço existentes (24, 4,6 m/0,15 s), uma viagem. Ao terminar, +35% velocidade por 2 s. Canal de bônus separado dos modificadores da ultimate.
- Retorno: sem mudança nas regras.
- Contra-Sombra: 18 energia, 12 s recarga; fumaça de raio 6 m por 4 s. Quem está dentro não causa dano nem ativa básico/ultimate; movimentos e defesa permitem fuga. Não concede imunidade. Dano de inimigo fora continua possível. Alvos ocultos saem da mira automática/perseguição visual do bot. Personagem local fosco com névoa reduzida para legibilidade; inimigo oculto não é renderizado. Granada mantém suas regras anteriores (ocultação sem supressão de ataque).
- Esquiva Sombria / Duplo Passo: pequeno passo inicial de 1,2 m, janela reativa de 1 s independente desse passo. Próximo contato de inimigo a até 7 m é negado e provoca travessia até 10 m, terminando 4 m além do agressor, em 0,25 s; 2/3 de dano. Uma reação. **Revisão confirmada pelo usuário: ignora qualquer dano durante 1 s, desde o acionamento, mantendo proteção até terminar a travessia se necessário.** A proteção também vale para dano direto, área, clash e reflexão. Paredes bloqueiam. Não persegue agressor distante que lançou um projétil, mas evita seu dano durante a janela.

Limitações deliberadas: protótipo local (não rede/stealth multiplayer); joystick permanece flutuante; edição permite sobrepor botões por escolha do usuário; valores acima são de balanceamento inicial. Qualidade visual, ergonomia e FPS precisam de validação no Moto G54 além dos testes automatizados.

Resultados desta rodada: `TEST_RESULTS_MOBILE_V4.md`.
