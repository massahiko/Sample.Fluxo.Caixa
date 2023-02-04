# ASP.NET Core - Fluxo de Caixa Simples

App - Este aplica alguns conceito comumente utilizado em arquitetura desacopladas (Sistemas Distribuídos, Microserviços). 

Com possibilidade de evolução como, incluir recursos de mensageria como (RabbitMq, Kafka).

Nesta aplicação temos 3 contextos mapeados. Contas (Conexto auxiliar), Lançamentos e Saldos (Contexto principal), a comunicação entres tais contextos é feita a partir de eventos.

Utilizando a abordagem DDD aplicando conceitos como CQRS, Event Sourcing.

Testes unitários e de integração devidamente implementados

Desennho Solução



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
