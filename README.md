# Trading Systems Monitoring

**Trading Systems Monitoring** is a monitoring system for trading systems operating on financial markets. The solution provides a REST API for retrieving and managing trading system state data, and includes infrastructure for data access and testing.

---

## Quick start

REST API for monitoring trading systems. Run the API project, or use Docker (see below).

### Configuration

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

## Docker quick start

Scripts in the repository root run and stop the system in containers:

- **`docker-start.cmd`**  
  Starts the required containers, runs the REST API, and executes all tests (unit and integration).

- **`docker-stop.cmd`**  
  Stops and removes all containers and shuts down the REST API.
