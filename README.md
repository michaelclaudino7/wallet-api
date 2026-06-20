# 💰 Wallet API

A RESTful API for a digital wallet built with **C# / .NET 8**, **Entity Framework Core**, **SQL Server**, and **Swagger** documentation.

Supports account creation, deposits, withdrawals, transfers between accounts, and transaction statement queries.

## 🛠 Tech Stack

- C# / .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- Docker (SQL Server via docker-compose)

## 📂 Project Structure

```
WalletApi/
├── WalletApi.sln
├── docker-compose.yml
└── WalletApi/
    ├── Controllers/      # API endpoints
    ├── Models/           # Domain entities
    ├── DTOs/             # Request/response objects
    ├── Data/             # DbContext (EF Core)
    ├── Services/         # Business logic
    └── Program.cs        # App configuration and startup
```

## ▶️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)

### Steps

1. **Start the database with Docker:**
   ```bash
   docker-compose up -d
   ```

2. **Restore project packages:**
   ```bash
   cd WalletApi
   dotnet restore
   ```

3. **Apply migrations:**
   ```bash
   dotnet tool install --global dotnet-ef --version 8.0.10
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Run the API:**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI:**
   ```
   http://localhost:5080/swagger
   ```

## 📌 Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/accounts` | Create a new account |
| GET | `/api/accounts` | List all accounts |
| GET | `/api/accounts/{id}` | Get account by ID |
| GET | `/api/accounts/{id}/statement` | Get transaction statement |
| POST | `/api/accounts/{id}/deposit` | Make a deposit |
| POST | `/api/accounts/{id}/withdraw` | Make a withdrawal |
| POST | `/api/accounts/{id}/transfer` | Transfer between accounts |

## 🧪 Usage Examples

**Create account:**
```json
POST /api/accounts
{
  "ownerName": "Michael Rodrigues",
  "document": "123.456.789-00"
}
```

**Deposit:**
```json
POST /api/accounts/{id}/deposit
{
  "amount": 500.00,
  "description": "Initial deposit"
}
```

**Transfer:**
```json
POST /api/accounts/{id}/transfer
{
  "toAccountId": "target-account-guid",
  "amount": 100.00,
  "description": "Payment"
}
```