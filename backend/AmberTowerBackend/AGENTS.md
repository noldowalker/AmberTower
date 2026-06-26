# AmberTower Backend Agent Instructions

## Backend Overview

This directory contains the .NET backend for AmberTower.

AmberTower backend is a learning microservice-oriented game backend platform built with .NET 8.

The backend should be production-inspired, but not over-engineered. Prefer clear service boundaries, explicit contracts, simple working implementations, and good tests.

Backend agents must always read and follow:

```text
backend/AmberTowerBackend/.agents/CODESTYLE.md
```

Code style rules are mandatory for all backend changes.

## Backend Solution

Expected backend solution location:

```text
backend/AmberTowerBackend/AmberTowerBackend.sln
```

Expected backend layout:

```text
backend/
  AmberTowerBackend/
    AGENTS.md
    AmberTowerBackend.sln

    src/
      ApiGateway/
      AuthService/
      ProfileService/
      WalletService/
      MatchmakingService/
      Contracts/
      BuildingBlocks/

    tests/
      AuthService.Tests/
      ProfileService.Tests/
      WalletService.Tests/
      MatchmakingService.Tests/
      IntegrationTests/
```

The current layout may be incomplete at early stages. Do not create all services at once unless explicitly requested.

## Technology Stack

Target backend stack:

* .NET 8
* ASP.NET Core
* gRPC
* PostgreSQL
* Kafka
* Redis
* Docker Compose
* Serilog
* OpenTelemetry
* Prometheus
* Grafana
* NUnit

Do not introduce additional major frameworks or infrastructure dependencies without explicit approval.

## Service Boundaries

Target service areas:

* `ApiGateway`

    * public REST/HTTPS API for Unity client;
    * authentication middleware;
    * request aggregation;
    * calls internal services via gRPC;
    * publishes client-originated game events when appropriate.

* `AuthService`

    * registration;
    * login;
    * password hashing;
    * refresh tokens;
    * JWT issuing;
    * authentication identity;
    * future external auth providers.

* `ProfileService`

    * player profile;
    * nickname;
    * avatar;
    * public player identity;
    * profile settings;
    * mapping between auth user identity and game player identity.

* `WalletService`

    * player currencies;
    * gold;
    * crystals;
    * balance operations;
    * transaction history;
    * idempotent credit/debit operations.

* `MatchmakingService`

    * rating;
    * MMR/ELO;
    * matchmaking-related state;
    * future leaderboard-related logic.

Service names may evolve, but service boundary changes must be deliberate and documented.

## Public API Boundary

The Unity client communicates only with `ApiGateway`.

Expected external communication:

```text
Unity Client -> REST/HTTPS -> ApiGateway
```

Do not expose internal gRPC services directly to the Unity client.

Do not let the Unity client communicate directly with:

* Kafka;
* PostgreSQL;
* Redis;
* internal service databases;
* internal gRPC endpoints.

## Internal Communication

Use gRPC for synchronous internal service-to-service calls.

Use Kafka for asynchronous domain events.

Examples:

```text
ApiGateway -> gRPC -> AuthService
ApiGateway -> gRPC -> ProfileService
ApiGateway -> gRPC -> WalletService
ApiGateway -> gRPC -> MatchmakingService
```

Examples:

```text
RunCompleted event -> Kafka
WalletService consumes RunCompleted -> updates wallet
MatchmakingService consumes RunCompleted -> updates rating/MMR
Analytics consumers may be added later
```

Do not use Kafka for direct request/response queries.

Do not use gRPC to broadcast facts to multiple independent consumers.

## Data Ownership

Each service owns its own data.

A service must not directly read or write another service's tables.

Allowed ways to access data from another service:

* gRPC call to the owning service;
* Kafka event followed by local read model update;
* documented public/internal API contract.

For local development, one PostgreSQL container may host multiple logical databases or schemas.

This does not remove ownership boundaries.

## Database Rules

Use PostgreSQL as the primary persistent storage.

Prefer one logical database or schema per service.

Avoid cross-service foreign keys.

Avoid cross-service joins.

Do not design a shared database as the main integration mechanism.

Use explicit migrations when database schema is introduced.

Do not store passwords, tokens, or secrets in plain text.

## Redis Rules

Redis may be used for:

* caching;
* rate limiting;
* session-related temporary data;
* matchmaking queues;
* rating/leaderboard fast reads;
* distributed locks only if explicitly justified.

Do not use Redis as the only source of truth for important persistent data such as wallet balances.

If Redis is used as a cache, document cache invalidation or expiration strategy.

## Kafka Rules

Use Kafka for asynchronous domain events.

Events should represent facts that already happened.

Prefer event names in past tense:

```text
PlayerRegistered
ProfileCreated
RunCompleted
CurrencyCredited
CurrencyDebited
RatingChanged
```

Kafka consumers must be designed with at-least-once delivery in mind.

Consumer logic must be idempotent when processing events that affect persistent state.

Use idempotency keys for operations that must not be applied twice.

Do not assume that Kafka guarantees global ordering.

Only ordering inside a partition should be relied upon.

If ordering matters, document the chosen message key.

## gRPC Rules

Use `.proto` files for internal service contracts.

Keep gRPC contracts explicit and stable.

Do not expose EF Core entities through gRPC.

Do not put business logic into generated contract classes.

Prefer request/response DTOs designed for the use case.

Use deadlines/cancellation where appropriate.

Propagate `CancellationToken` from ASP.NET Core endpoints into gRPC calls when possible.

## REST API Rules

REST endpoints are exposed by `ApiGateway`.

Use REST/JSON for Unity-facing APIs.

Prefer clear endpoint names:

```text
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh
GET  /api/me
GET  /api/profile/me
PATCH /api/profile/me
GET  /api/wallet/me
POST /api/runs/complete
```

Do not expose internal microservice structure in public REST routes.

The public API should remain stable even if internal services change.

## Authentication and Authorization

Authentication is handled through `AuthService` and `ApiGateway`.

Expected model:

```text
Client logs in through ApiGateway
ApiGateway calls AuthService
AuthService validates credentials
AuthService issues access token and refresh token
Client sends access token as Bearer token
ApiGateway validates token for protected endpoints
```

Do not store raw passwords.

Use a proper password hashing strategy.

Do not put JWT signing keys into source code.

Use local development configuration for local secrets.

Authorization rules should be explicit and testable.

## Wallet Rules

Wallet operations are important and must be treated carefully.

Wallet balances must not become negative unless explicitly allowed by business rules.

Currency changes should be recorded as transactions.

Prefer append-only transaction history plus current balance.

Operations that credit or debit currency should support idempotency.

Example wallet operation fields:

```text
operationId
playerId
currencyCode
amount
reason
sourceEventId
createdAt
```

Do not trust currency amounts sent directly by the client.

For game rewards, the backend should calculate reward amounts.

## Matchmaking and Rating Rules

Matchmaking, rating, MMR/ELO, and leaderboard-like behavior belong to the matchmaking/rating domain, not to profile or wallet.

At early stages, this may live in `MatchmakingService`.

Later it may be split into:

* MatchmakingService
* RatingService
* LeaderboardService

Do not split these services prematurely.

## Project Structure Inside Services

Preferred early-stage structure for each service:

```text
ServiceName/
  Program.cs
  Grpc/
  Application/
  Domain/
  Infrastructure/
  Persistence/
  Options/
```

For services with Kafka consumers:

```text
ServiceName/
  Consumers/
```

For services exposing REST endpoints:

```text
ServiceName/
  Controllers/
  Endpoints/
```

Do not create separate projects for every layer unless explicitly requested.

Prefer:

```text
One service = one executable project
```

plus separate test projects.

## Contracts

Prefer dedicated contract projects.

At early stages, separate contracts by domain when practical:

```text
AmberTower.Auth.Contracts
AmberTower.Profile.Contracts
AmberTower.Wallet.Contracts
AmberTower.Matchmaking.Contracts
AmberTower.Events.Contracts
```

Contract projects may contain:

* `.proto` files;
* generated gRPC types;
* Kafka event DTOs;
* simple enums used in wire contracts.

Contract projects must not contain:

* business logic;
* EF Core entities;
* database access;
* service implementation code.

## Building Blocks

A shared `BuildingBlocks` project may contain small reusable infrastructure pieces.

Allowed examples:

* correlation id helpers;
* result/error types;
* time provider abstractions;
* logging helpers;
* OpenTelemetry setup helpers;
* Kafka producer/consumer wrappers if kept generic;
* validation primitives if simple.

Do not turn `BuildingBlocks` into a shared domain model.

Do not put service-specific business logic into `BuildingBlocks`.

## Error Handling

Use explicit error handling.

Do not let internal exceptions leak into public API responses.

For gRPC, map known errors to appropriate gRPC statuses.

For REST, map known errors to appropriate HTTP status codes.

Examples:

```text
Validation error -> 400 Bad Request
Unauthenticated -> 401 Unauthorized
Forbidden -> 403 Forbidden
Not found -> 404 Not Found
Conflict / duplicate -> 409 Conflict
Unexpected error -> 500 Internal Server Error
```

Log unexpected errors.

Do not log secrets, raw passwords, access tokens, or refresh tokens.

## Observability

Use structured logging.

Prefer Serilog for application logs.

Use correlation/request IDs across service calls.

Use OpenTelemetry for traces and metrics when observability is introduced.

Important flows should be traceable across:

```text
ApiGateway
gRPC calls
Kafka publish
Kafka consume
database write
```

Do not add observability libraries without updating configuration and documentation.

## Async and Cancellation

Use async/await correctly.

Do not block async code with:

```text
.Result
.Wait()
.GetAwaiter().GetResult()
```

Pass `CancellationToken` where appropriate.

Avoid fire-and-forget tasks unless explicitly justified and safely handled.

Use background services for long-running consumers.

## Testing

Use NUnit for unit tests unless the existing project uses another test framework.

Add or update unit tests for non-trivial application logic.

Prefer testing domain/application logic without requiring real infrastructure.

Integration tests may be added later for:

* PostgreSQL;
* Kafka;
* Redis;
* gRPC service calls;
* ApiGateway flows.

When changing backend code, run or suggest running:

```text
dotnet build backend/AmberTowerBackend/AmberTowerBackend.sln
```

When tests exist and relevant logic changed, run or suggest running:

```text
dotnet test backend/AmberTowerBackend/AmberTowerBackend.sln
```

If commands cannot be run, explain why.

## Configuration

Use strongly typed options for configuration.

Do not hardcode ports, connection strings, credentials, or external URLs in application logic.

Use environment variables or local development configuration.

Local development defaults are allowed only when clearly safe.

## Docker Compose

Docker Compose should be used for local infrastructure.

Expected local infrastructure over time:

* PostgreSQL
* Kafka
* Redis
* Prometheus
* Grafana
* OpenTelemetry Collector if needed

Do not add all infrastructure at once unless requested.

Add infrastructure gradually as services start using it.

## Documentation

Update documentation when backend architecture changes.

Relevant documentation locations:

```text
docs/architecture/
docs/decisions/
```

Use ADR-style documents for important decisions.

Important backend decisions include:

* service boundaries;
* authentication flow;
* gRPC contracts;
* Kafka event design;
* database ownership;
* wallet transaction model;
* matchmaking/rating model;
* Redis usage;
* observability strategy;
* testing strategy.

## Agent Workflow

Before editing backend files:

1. Inspect the current backend solution and project structure.
2. Read this file.
3. Read and follow `backend/AmberTowerBackend/.agents/CODESTYLE.md`.
4. Propose a short implementation plan.
5. Wait for explicit approval.
6. Make focused changes.
7. Run relevant checks when possible.
8. Summarize changed files and verification results.

`Program.cs` must stay declarative. Do not declare HTTP or gRPC endpoints inline in `Program.cs`; map them through dedicated extension methods instead.

Backend task files must use the `B<number>-name-of-task.md` format inside:

```text
.agents/
  tasks/
    backlog/
    current/
    done/
```

Examples:

```text
B1-api-gateway-bootstrap.md
B2-wallet-service-outline.md
```

Do not create multiple services, infrastructure, tests, and documentation in one large change unless explicitly requested.

Do not silently change architecture.

Do not create placeholder services that pretend to be implemented.

## Current Backend Direction

Current intended direction:

```text
Unity Client
  -> REST/HTTPS
  -> ApiGateway

ApiGateway
  -> gRPC
  -> AuthService
  -> ProfileService
  -> WalletService
  -> MatchmakingService

Backend services
  -> Kafka
  -> asynchronous events

Services
  -> own PostgreSQL database/schema

Redis
  -> cache, rate limiting, matchmaking/rating/leaderboard support when needed
```

This direction may evolve, but changes must be deliberate and documented.
