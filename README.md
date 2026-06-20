# 💰 Wallet API

API RESTful de carteira digital desenvolvida em **C# / .NET 8**, com **Entity Framework Core**, **SQL Server** e documentação via **Swagger**.

Permite criar contas, realizar depósitos, saques, transferências entre contas e consultar o extrato de transações.

## 🛠 Tecnologias

- C# / .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- Docker (SQL Server via docker-compose)

## 📂 Estrutura do projeto

```
WalletApi/
├── WalletApi.sln
├── docker-compose.yml
└── WalletApi/
    ├── Controllers/      # Endpoints da API
    ├── Models/           # Entidades de domínio
    ├── DTOs/             # Objetos de request/response
    ├── Data/             # DbContext (EF Core)
    ├── Services/         # Regras de negócio
    └── Program.cs        # Configuração e startup
```

## ▶️ Como rodar localmente

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (para rodar o SQL Server) ou uma instância local de SQL Server

### Passo a passo

1. **Suba o banco de dados com Docker:**
   ```bash
   docker-compose up -d
   ```

2. **Restaure os pacotes do projeto:**
   ```bash
   cd WalletApi
   dotnet restore
   ```

3. **Crie e aplique as migrations:**
   ```bash
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Rode a API:**
   ```bash
   dotnet run
   ```

5. **Acesse o Swagger** no navegador:
   ```
   http://localhost:5080/swagger
   ```

## 📌 Endpoints principais

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/accounts` | Cria uma nova conta |
| GET | `/api/accounts` | Lista todas as contas |
| GET | `/api/accounts/{id}` | Busca uma conta específica |
| GET | `/api/accounts/{id}/statement` | Extrato de transações |
| POST | `/api/accounts/{id}/deposit` | Realiza um depósito |
| POST | `/api/accounts/{id}/withdraw` | Realiza um saque |
| POST | `/api/accounts/{id}/transfer` | Transfere valores entre contas |

## 🧪 Exemplo de uso

**Criar conta:**
```json
POST /api/accounts
{
  "ownerName": "Michael Rodrigues",
  "document": "123.456.789-00"
}
```

**Depositar:**
```json
POST /api/accounts/{id}/deposit
{
  "amount": 500.00,
  "description": "Depósito inicial"
}
```

**Transferir:**
```json
POST /api/accounts/{id}/transfer
{
  "toAccountId": "guid-da-conta-destino",
  "amount": 100.00,
  "description": "Pagamento"
}
```

## 📄 Licença

Projeto livre para fins de estudo e portfólio.
