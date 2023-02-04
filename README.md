# ASP.NET Core - Fluxo de Caixa Simples

App - Este aplica alguns conceito comumente utilizado em arquitetura desacopladas (Sistemas Distribuídos, Microserviços). 

Com possibilidade de evolução como serviços de mensageria (RabbitMq, Kafka).

Nesta aplicação temos 3 contextos mapeados, Contas (Conexto auxiliar), Lançamentos e Saldos (Contexto principal), a comunicação entres tais contextos é feita a partir de eventos.

Utilizando a abordagem DDD aplicando conceitos como CQRS, Event Sourcing.

Testes unitários e de integração devidamente implementados

# Desenho da Solução

![fluxo-de-caixa-diagram](https://user-images.githubusercontent.com/38221988/216742061-2428c595-2fe2-4649-a216-532263b9d380.png)

* **Server Side**: ASP.NET Core

Pre reqs

* Net.Core 5
* Docker

Techs:

* ASP.NET Core
* Swagger
* Sql Server
* EF Core
* Event Sourcing
* MongoDb

# Como Executar

1. Download do código
  * Abrir pasta
  * Abrir terminal
  * Executar docker-compose up --build
