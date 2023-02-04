# ASP.NET Core - Fluxo de Caixa Simples

App - Este aplica alguns conceito comumente utilizado em arquitetura desacopladas (Sistemas Distribu�dos, Microservi�os). 

Com possibilidade de evolu��o como, incluir recursos de mensageria como (RabbitMq, Kafka).

Nesta aplica��o temos 3 contextos mapeados. Contas (Conexto auxiliar), Lan�amentos e Saldos (Contexto principal), a comunica��o entres tais contextos � feita a partir de eventos.

Utilizando a abordagem DDD aplicando conceitos como CQRS, Event Sourcing.

Testes unit�rios e de integra��o devidamente implementados

Desennho Solu��o



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

1. Download do c�digo
  * Abrir pasta
  * Abrir terminal
  * Executar docker-compose up --build
