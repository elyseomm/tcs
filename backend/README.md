# Order Management API

Backend para gestão de pedidos, implementado para o teste prático de Desenvolvedor .NET Senior.

## Stack

- .NET 10 + Minimal API
- Clean Architecture
- CQRS com MediatR
- EF Core 10 + MySQL
- Pomelo.EntityFrameworkCore.MySql
- JWT
- FluentValidation
- Serilog
- xUnit + Moq
- Docker e Docker Compose

## Decisões arquiteturais

- **Minimal API**: o desafio possui poucos endpoints e não exige recursos específicos de MVC. Os endpoints são apenas adaptadores HTTP; regras e orquestração permanecem nas camadas Application e Domain.
- **Clean Architecture**: `Domain` não depende de frameworks; `Application` contém CQRS e contratos; `Infrastructure` contém EF Core/MySQL e JWT; `Api` é a camada de entrada.
- **CQRS/MediatR**: commands e queries são separados e os handlers são diretamente testáveis.
- **Repository específico**: existe apenas `IOrderRepository`; não foi usado `IRepository<T>` genérico porque as operações são específicas do agregado `Order`.
- **Regras no domínio**: invariantes dos itens, pedido com ao menos um item, cálculo de `TotalAmount` e cancelamento pertencem ao domínio.

## Estrutura

```text
src/
  OrderManagement.Domain
  OrderManagement.Application
  OrderManagement.Infrastructure
  OrderManagement.Api
tests/
  OrderManagement.Application.Tests
```

## Banco de dados MySQL

A persistência foi alterada de SQLite para **MySQL**, usando:

```text
Pomelo.EntityFrameworkCore.MySql
```

A configuração local padrão está em:

`src/OrderManagement.Api/appsettings.json`

```text
Server=localhost;Port=3306;Database=tsc-ecommerce;User=root;Password=root;
```

A API continua aplicando automaticamente as migrations pendentes durante a inicialização:

```csharp
await db.Database.MigrateAsync();
```

### Executar MySQL com Docker

```bash
docker compose up -d mysql
```

O MySQL ficará disponível em:

- Host: `localhost`
- Porta: `3306`
- Database: `tsc-ecommerce`
- User: `root`
- Password: `root`

## Executar localmente

Pré-requisitos:

- .NET SDK 10
- MySQL 8.4+ local ou Docker

```bash

dotnet restore
dotnet build
dotnet test
dotnet run --project src/OrderManagement.Api
```

Se o MySQL estiver rodando com usuário ou senha diferentes, altere a connection string em `appsettings.json`.

## Executar com Docker

Para subir API e MySQL:

```bash
docker compose up --build
```

Serviços:

- API: `http://localhost:8080`
- MySQL: `localhost:3306`

A API aguarda o health check do MySQL antes de iniciar e executa as migrations automaticamente.

## Autenticação

`POST /auth/login`

```json
{ "email": "dev@martech.com", "password": "Senha@123" }
```

Use o token retornado:

```text
Authorization: Bearer <token>
```

## Endpoints

- `POST /auth/login`
- `POST /api/orders`
- `GET /api/orders?page=1&pageSize=10`
- `GET /api/orders/{id}`
- `PATCH /api/orders/{id}/cancel`

### Criar pedido

```json
{
  "customerId": "00000000-0000-0000-0000-000000000001",
  "items": [
    { "productName": "Keyboard", "quantity": 2, "unitPrice": 100.00 }
  ]
}
```

## Testes

Os testes unitários existentes cobrem os handlers de criação, cancelamento e consultas, incluindo cálculo do total, 
persistência, paginação e pedido inexistente.

```bash
dotnet test
```

## Migration Inicial

Um Migration será executado automaticamente toda vez que executar a aplicação "OrderManagement.Api" no terminal via comando: 

```bash
dotnet run --project src/OrderManagement.Api
```

Se deseja rodar manualmente para verificar a execução/criação dos artefatos ...
Precisa instalar anteriormente o dotnet-ef:

```bash
dotnet tool install --global dotnet-ef
```

No terminal navege até a pasta "..\src\OrderManagement.Infrastructure\";
Certifique-se que o arquivo "20260820002545_InitialCreate.cs" existe na pasta Migrations do projeto OrderManagement.Infrastructure;
Caso não exista, execute o comando:

```bash
 dotnet ef migrations add InitialCreate
```

um arquivo com nome no formato "YYYYMMDDHHMISS_InitialCreate.cs" deverá ser criado;
E então execute a aplicação OrderManagement.Api.

## Criar novas migrations

Após alterar o modelo:

```bash
dotnet ef migrations add NomeDaMigration \
  --project src/OrderManagement.Infrastructure \
  --startup-project src/OrderManagement.Api
```

## Observabilidade

O projeto mantém Serilog para requisições HTTP e um `LoggingBehavior` no pipeline do MediatR para registrar execução, resposta/erro e tempo dos commands/queries.
