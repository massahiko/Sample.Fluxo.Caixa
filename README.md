# ASP.NET Core - Fluxo de Caixa Simples

App - Este aplica alguns conceitos comumente utilizado em arquitetura desacopladas (Sistemas Distribuídos, Microserviços). 

Com possibilidade de evolução como serviços de mensageria (RabbitMq, Kafka).

Nesta aplicação temos 3 contextos mapeados, Contas (Conexto auxiliar), Lançamentos e Saldos (Contexto principal), a comunicação entres eles é feita a partir de eventos.

Utilizando a abordagem DDD aplicando conceitos como CQRS, Event Sourcing.

Testes unitários e de integração devidamente implementados

# Desenho da Solução

![fluxo-de-caixa-diagram](https://user-images.githubusercontent.com/38221988/217388285-6268f3d5-b247-446e-9da8-62b9832f85cd.jpg)

* **Server Side**: ASP.NET Core

Pre reqs

* Net.Core 5
* Docker

Techs:

* ASP.NET Core
* Swagger
* Sql Server
* EF Core
* Event Sourcing (Eventstore)
* MongoDb

# Como Executar

1. Download do código
  * Abrir pasta
  * Abrir terminal
  * Executar docker-compose up --build
  * Aguardar atualização do banco 'FluxoCaixaDB'
