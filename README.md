# API para Gerenciamento de Faturamento de Clientes

##### Por Kayque Almeida Piton

Este projeto é uma API REST desenvolvida em .NET 8.0 para gerenciar o faturamento de clientes. Inclui operações CRUD para clientes e produtos, bem como a importação e gerenciamento de faturas de uma API externa.

## Funcionalidades

#### Customer: Operações **CRUD** completas para clientes.
  * Id
  * Name
  * Email
  * Address
  * **Todos os campos são de preenchimento obrigatório.**

#### Product: Operações **CRUD** completas para produtos.
  * Id
  * Description
  * **Todos os campos são de preenchimento obrigatório.**

#### Billing
   * Importação de dados de faturamento de uma API externa.
   * Verificação e inserção de faturas no banco de dados local.
   * Tratamento de exceções para mau funcionamento ou interrupção do serviço da API externa.

## Tecnologias utilizadas

#### Frameworks Principais
   * .NET 8.0
   * Entity Framework Core 8.0.4
   * MySQL com Pomelo.EntityFrameworkCore.MySql 8.0.2
   * AutoMapper 13.0.1
   * FluentValidation 8.2.1
   * Refit 7.1.1
   * Swagger com Swashbuckle.AspNetCore 6.4.0

#### Bibliotecas de Apoio e Extensões
   * Microsoft.AspNetCore.OpenApi 8.0.4
   * Microsoft.Extensions.Http 8.0.0
   * Microsoft.Extensions.Configuration.Abstractions 8.0.0
   * Microsoft.Extensions.DependencyInjection.Abstractions 8.0.1

#### Ferramentas de Teste e Cobertura
   * coverlet.collector 6.0.2
   * coverlet.msbuild 6.0.2
   * FluentAssertions 6.12.0
   * Microsoft.AspNetCore.Mvc.Testing 8.0.6
   * Microsoft.NET.Test.Sdk 17.8.0
   * Moq 4.20.70
   * NSubstitute 5.1.0
   * NUnit 4.1.0
   * NUnit3TestAdapter 4.5.0
   * xunit 2.8.1
   * xunit.runner.visualstudio 2.8.0

## Configuração do Projeto 

**1. Clone o repositório:**
   ```sh
   git clone https://github.com/kayquepiton/ca-backend-test.git
   ```

**2. Navegue para o direório do projeto:**
   ```sh
   cd ca-backend-test
   ```

**3. Configure a string de conexão do MySQL:** `appsettings.json`:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=yuorport;Database=yourdatabase;Uid=root;Pwd=yourpassword;"
     }
   }
   ```

**4. Restaure as dependências e inicialize o banco de dados:**
   ```sh
   dotnet restore
   cd ca-backend-test.Infra.Data
   dotnet-ef migrations add "InitialMigration" -s ../Ca.Backend.Test.Api/Ca.Backend.Test.Api.csproj 
   dotnet-ef database update -s ../Ca.Backend.Test.Api/Ca.Backend.Test.Api.csproj 
   ```

**5. Execute o projeto:**
   ```sh
   dotnet run --project Ca.Backend.Test.API
   ```

**6. Acesse Swagger para testar endpoints:**
   - Abra seu navegador da web e digite o endereço local onde a aplicação está sendo executada
   `/swagger`, por exemplo: `https://localhost:{port}/swagger/index.html`.

## Endpoints da API

#### Customers
   * **GET** /api/customers
   * **GET** /api/customers/{id}
   * **POST** /api/customers
   * **PUT** /api/customers/{id}
   * **DELETE** /api/customers/{id}

#### Products
   * **GET** /api/products
   * **GET** /api/products/{id}
   * **POST** /api/products
   * **PUT** /api/products/{id}
   * **DELETE** /api/products/{id}

#### Billing
   * **GET** /api/billing
   * **GET** /api/billing/{id}
   * **POST** /api/billing
   * **PUT** /api/billing/{id}
   * **DELETE** /api/billing/{id}
   * **POST** /api/billing/import (Importação dos dados do faturamento (billing) para uma API externa)

### Futuras implementações
   * Melhorar a documentação do swagger.
   * Aperfeiçoamento das práticas de modelagem do projeto.
   * Melhorar cobertura de teste.

## Contato
   * Kayque Almeida Piton
   * Email: [kayquepiton@gmail.com](mailto:kayquepiton@gmail.com)  
   * LinkedIn: [Kayque Almeida Piton](https://www.linkedin.com/in/kayquepiton/)
