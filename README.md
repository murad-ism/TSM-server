# Trading Systems Monitoring

**Trading Systems Monitoring** is a monitoring system for trading systems operating on financial markets. The solution provides a REST API for retrieving and managing trading system state data, and includes infrastructure for data access and testing.

## Stack

- **.NET 8** — ASP.NET Core Web API, SignalR
- **Data** — SQL Server (trading data), PostgreSQL (Identity), MongoDB (log records)
- **Messaging & cache** — Kafka, Redis, NetMQ
- **Auth** — JWT, ASP.NET Core Identity
- **Logging** — Serilog (console, file)
- **Deployment** — Docker, docker-compose

---

## Quick start

REST API for monitoring trading systems. Run the API project, or use Docker (see below).

### Environment

Required settings in `appsettings.json` (or environment / User Secrets in production):

| Section | Key | Description |
|--------|-----|-------------|
| **ConnectionStrings** | TradingDataDbConnection | SQL Server connection for trading data |
| **ConnectionStrings** | UsersDbConnection | PostgreSQL connection for Identity (users/roles) |
| **ConnectionStrings** | TradingLogRecordsDbConnection | MongoDB connection for log records |
| **Identity** | TokenKey | JWT signing key (keep secret) |
| **Identity** | AdminInitialPassword | Initial admin password when seeding (required only for seed) |
| **Kafka** | BootstrapServers | Kafka brokers (e.g. `localhost:9092`) |
| **Redis** | ConnectionString | Redis connection (e.g. `localhost:6379`) |
| **MsgQueueSubscriber** | Url, Channel | NetMQ subscriber URL and channel for live data |

Optional overrides: `Kafka:Topic`, `Kafka:DlqTopic`, `Kafka:GroupId`, `Kafka:WorkerCount`, `Kafka:QueueCapacity`.

---

## Docker quick start

Scripts in the repository root run and stop the system in containers:

- **`docker-start.cmd`**  
  Starts the required containers, runs the REST API, and executes all tests (unit and integration).

- **`docker-stop.cmd`**  
  Stops and removes all containers and shuts down the REST API.

---

## Project structure

```
TSM-server/
├── TradingSystemsMonitoring.RestAPI/     # REST API, controllers, hubs, hosted services
├── TradingSystemsMonitoring.DataModel/    # Entities, DbContext, EF migrations, settings
├── TradingSystemsMonitoring.Data/        # Repositories, data access, DTOs
├── TradingSystemsMonitoring.Tests/       # Unit tests
├── TradingSystemsMonitoring.ApiTests/    # REST API integration tests
├── docker-compose.yml
├── docker-compose.override.yml
├── docker-start.cmd
├── docker-stop.cmd
└── TradingSystemsMonitoring.sln
```

---

## Solution structure

The project consists of five main components:

- **TradingSystemsMonitoring.RestAPI**  
  Startup project. REST API for monitoring trading system state. Used for communication with client applications or external services.

- **TradingSystemsMonitoring.DataModel**  
  Data models used by the system. Contains entity and DTO definitions.

- **TradingSystemsMonitoring.Data**  
  Repository implementations and data access logic. Works with the database and encapsulates storage details.

- **TradingSystemsMonitoring.Tests**  
  Unit tests for system components. Covers business logic and models.

- **TradingSystemsMonitoring.ApiTests**  
  REST API integration tests. Used to verify correct interaction between components over HTTP.

---

## Features

- **Trading data API** — Search and count closed trades by system, security, and date range; pagination support. Retrieve current (open) trades, trading log records by date, system IDs, account IDs, and securities list.
- **Live trading data** — SignalR hub (`/api/tradingDataMonitoring`) for real-time updates; data ingested from Kafka and NetMQ.
- **Identity & auth** — JWT-based login/logout; admin-only user management (add/delete users). Role-based authorization.
- **Observability** — Serilog logging (console, JSON file); HTTP failure metrics; global exception handling with problem details.
