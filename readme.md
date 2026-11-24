# 🧟 Tonho Defense

> Um Tower Defense 2D em um mundo tomado pelo apocalipse zumbi.

## 📖 História do jogo (Lore)

O mundo já não é mais o mesmo. Após um misterioso evento provocar o surgimento de uma epidemia, cidades foram abandonadas, estradas ficaram desertas e hordas de zumbis passaram a dominar os territórios que antes eram habitados por seres humanos.

Em meio ao caos, Tonho conseguiu encontrar um pequeno refúgio. Porém, sobreviver não seria tão simples. Os zumbis começaram a se aproximar cada vez mais de sua base, ameaçando destruir o último lugar que ele poderia chamar de lar.

Sem possibilidade de fugir e com recursos limitados, Tonho precisa resistir aos ataques, enfrentar as hordas e proteger sua base a qualquer custo.

É nesse cenário que surge **Tonho Defense**, um jogo de estratégia no estilo Tower Defense, onde cada inimigo representa uma ameaça e cada decisão pode ser determinante para a sobrevivência.

## 🎮 Sobre o jogo

Tonho Defense é um jogo 2D do gênero Tower Defense, desenvolvido utilizando a engine Unity e a linguagem de programação C#.

O objetivo do jogo é defender a base do Tonho contra ondas de zumbis que percorrem um caminho em direção à área protegida. O jogador deve enfrentar os inimigos, acompanhar os recursos disponíveis e cumprir os objetivos da partida para alcançar a vitória.

O projeto foi desenvolvido como uma atividade acadêmica, colocando em prática conceitos de desenvolvimento de jogos, programação orientada a objetos, organização de componentes, lógica de gameplay e gerenciamento de sistemas.

## 🕹️ Como funciona o jogo?

Durante a partida, os inimigos surgem em um ponto inicial e percorrem um caminho previamente definido, utilizando pontos de referência chamados waypoints.

O jogador precisa impedir que os zumbis alcancem a base.

O jogo possui sistemas de gerenciamento de partida, movimentação de inimigos, controle de vida, geração de ondas e acompanhamento dos objetivos.

### Principais elementos

- **Base:** representa o local que deve ser protegido. Possui uma quantidade de vida que pode ser reduzida quando os inimigos conseguem alcançá-la.
- **Zumbis:** inimigos que percorrem o caminho em direção à base. Podem possuir atributos diferentes, como velocidade, vida e dano.
- **Caminho:** trajeto definido por waypoints, que orientam a movimentação dos inimigos.
- **Ondas:** grupos de inimigos organizados em sequências de surgimento.
- **Dinheiro:** recurso relacionado às recompensas obtidas ao eliminar inimigos e destinado ao sistema de gameplay.
- **Sistema de partida:** controla o tempo, os abates, os estados do jogo e as condições de encerramento.

> Os recursos de construção, defesa e ataque das torres devem ser considerados conforme o estágio de implementação presente na versão disponibilizada do jogo.

## 🏆 Objetivo

O objetivo principal é sobreviver aos ataques dos zumbis e alcançar a meta de abates estabelecida na partida, sem deixar que a base seja destruída.

O jogador pode enfrentar diferentes condições de encerramento:

- **Vitória:** atingir o objetivo de abates definido para a partida.
- **Derrota:** perder a base ou esgotar o tempo disponível, conforme as condições implementadas.

## 🛠️ Tecnologias utilizadas

- **Unity:** engine utilizada para o desenvolvimento, organização da cena, componentes, gameplay e execução do jogo.
- **C#:** linguagem utilizada para programar os sistemas e comportamentos.
- **Git:** ferramenta de controle de versões utilizada para registrar alterações no projeto.
- **GitHub:** plataforma utilizada para armazenar e disponibilizar o repositório do projeto.

## ⚙️ Sistemas desenvolvidos

O projeto possui diferentes sistemas que trabalham em conjunto para permitir o funcionamento da partida.

### GameManager
Responsável pelo gerenciamento geral da partida, incluindo estados do jogo, tempo, dinheiro, abates, inimigos contabilizados e condições de vitória e derrota.

### Enemy
Controla o comportamento dos inimigos, incluindo movimentação, vida, recebimento de dano, morte, recompensas e chegada à base.

### EnemyPath
Define o percurso que os inimigos devem seguir através de uma sequência de waypoints.

### BaseTonho
Gerencia a vida da base e identifica quando os inimigos alcançam sua área de proteção.

### WaveSpawner
Organiza e executa as ondas de inimigos, controlando os grupos, quantidades e intervalos de surgimento.

### Sistema de interface
Apresenta informações importantes durante a partida, como tempo, dinheiro, abates, vida da base e informações das ondas, além de telas de início e encerramento.

## 📥 Como baixar e executar o jogo

Para jogar Tonho Defense, não é necessário instalar a Unity.

1. Acesse o link de download do jogo disponibilizado abaixo.
2. Baixe o arquivo compactado do jogo.
3. Extraia o conteúdo do arquivo ZIP para uma pasta de sua preferência.
4. Abra a pasta extraída.
5. Execute o arquivo `Tonho Defense.exe`.
6. Aguarde o carregamento e siga as instruções apresentadas no jogo.

## 🎮 Download do jogo

A versão executável do Tonho Defense está disponível para Windows.

👉 [**Baixar Tonho Defense – Versão 1.0.0**](https://github.com/ApexBoost/TonhoDefense/releases/tag/v1.0.0)

Baixe o arquivo ZIP, extraia os arquivos e execute `Tonho Defense.exe`. Não é necessário ter a Unity instalada.

---

## 🎥 Vídeo de demonstração

Confira o vídeo de demonstração do funcionamento do jogo:

▶️ [**Assistir à demonstração do Tonho Defense**](https://youtu.be/HRsnhewIrWA)

---

## 📂 Repositório do projeto

O código-fonte e os arquivos de desenvolvimento estão disponíveis no GitHub:

🔗 [**Acessar repositório Tonho Defense**](https://github.com/ApexBoost/TonhoDefense)
## 💻 Desenvolvimento do projeto

O desenvolvimento do Tonho Defense foi realizado utilizando a Unity, com a organização dos elementos da cena por GameObjects e Components e a implementação dos comportamentos por meio de scripts em C#.

A estrutura do projeto foi organizada de maneira a separar as responsabilidades de cada sistema, facilitando a manutenção, os testes e a implementação de futuras funcionalidades.

Foram utilizados conceitos como:

- Programação orientada a objetos.
- Componentes e GameObjects.
- Arrays e estruturas de dados.
- Métodos e propriedades.
- Controle de estados.
- Instanciação e destruição de objetos.
- Movimentação através de waypoints.
- Colisões e eventos Trigger.
- Coroutines e temporização.
- Gerenciamento de recursos e condições de gameplay.

## 📂 Repositório do projeto

O repositório contém os arquivos e estruturas necessários para o desenvolvimento do jogo na Unity, incluindo scripts, recursos, configurações e demais componentes do projeto.

🔗 **GitHub:** [Tonho Defense — Repositório do projeto](COLOCAR_LINK_DO_REPOSITORIO_AQUI)

## 🚧 Melhorias futuras

O projeto pode continuar sendo expandido com novas funcionalidades, como:

- Implementação e aprimoramento de diferentes torres defensivas.
- Novos tipos de inimigos.
- Ampliação e balanceamento das ondas.
- Melhorias na interface do jogador.
- Efeitos visuais e sonoros.
- Animações dos personagens e ataques.
- Novos mapas e cenários.
- Aprimoramento da experiência de gameplay.

## 👨‍💻 Projeto acadêmico

**Tonho Defense** foi desenvolvido como projeto acadêmico com o objetivo de aplicar conceitos de programação e desenvolvimento de jogos utilizando Unity e C#.

O projeto representa o processo de aprendizagem, implementação, testes e evolução de sistemas de gameplay, desde a organização inicial da cena até a construção dos mecanismos que controlam a partida.

---

> **Tonho Defense — Sobreviver é apenas o começo.** 🧟
