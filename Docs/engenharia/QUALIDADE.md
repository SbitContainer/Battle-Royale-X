# Qualidade e proteção do projeto

Adaptado de QUALIDADE do kit para Unity/Android.

- Preservar arquitetura e fronteiras reais. Nenhuma extração de módulo, renomeação de pastas ou upgrade de pacote sem necessidade e teste.
- Registrar versões efetivas de Unity, URP, SDK/ADB e ferramentas. Erro de licença/PATH/build é classificado antes de mudar gameplay.
- Preservar `.meta`, referências de cena/prefab e valores serializados. Para cada asset externo guardar origem, licença, hash quando incorporado e import settings; consultar THIRD_PARTY_NOTICES. Não comprar assets por conta própria.
- Medir no dispositivo: frame time, FPS, memória, carga de VFX, temperatura/duração quando disponível. Não converter simulação acelerada do Editor em benchmark nem prometer 60 FPS sem medição. Definir orçamento após baseline físico.
- Interface precisa feedback de rejeição/cooldown, contraste de área, botões acessíveis, orientação e cancelamento de gesto. Capturas complementam teste de toque, não o substituem.
- Logging proporcional e sanitizado: IDs de habilidade/ação, versão, fase e duração; sem segredos, contas do usuário ou gravação contínua de input.
- Menor privilégio, temporários isolados e caminhos resolvidos; não apagar caches/dados para esconder falhas. Não portar `.env`, credenciais, IPs ou autorizações da origem.
- Logs/Builds/Library são ignorados. Selecionar evidências duráveis em Docs; não versionar caches/APKs por acidente. Lockfile Unity ignorado é risco conhecido, não “resolvido” pela documentação.
- Revisão estática é amostral. Validador de nomes não prova semântica, segurança completa, carregamento de agentes ou ausência de bugs.
