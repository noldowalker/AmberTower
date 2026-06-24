# AmberTower Agent Instructions

## Project Overview

AmberTower is a monorepo for a learning game backend platform and a future Unity client.

The project is not a throwaway test application. It should be treated as a foundation that may grow into a real game-related backend system.

Main goals:

* Build a practical .NET 8 microservice backend.
* Learn and apply gRPC, Kafka, PostgreSQL, Redis, Docker Compose, observability, and testing.
* Keep the architecture understandable, production-inspired, and not over-engineered.
* Leave clear documentation for future development and AI-assisted work.

## Repository Structure

Expected repository layout:

```text
AmberTower/
  AGENTS.md
  README.md

  backend/
    AmberTowerBackend/
      AGENTS.md
      AmberTowerBackend.sln
      src/
      tests/

  client/
    AmberTowerClient/
      AGENTS.md

  deploy/
    docker-compose.yml

  docs/
    architecture/
    decisions/

  scripts/
```

The current structure may be incomplete at early stages. Do not create missing folders unless the current task requires it.

## General Agent Rules

* Always inspect the existing repository structure before making changes.
* Always propose a plan before editing files.
* Keep changes small, focused, and reviewable.
* Do not mix backend, client, infrastructure, and documentation changes in one task unless explicitly requested.
* Do not perform broad refactoring, mass renaming, formatting-only changes, or project restructuring unless explicitly requested.
* Do not introduce new infrastructure dependencies without explaining why and updating documentation.
* Do not add placeholder code that looks complete but does not work.
* Prefer simple working implementations over abstract frameworks and unnecessary patterns.
* Preserve existing user decisions unless the task explicitly asks to revisit them.
* After making changes, summarize:

    * what was changed;
    * which files were changed;
    * which checks were run;
    * what was not done and why.

## Language

Use English for repository instructions, documentation files, code comments, commit-style summaries, and technical names unless the existing file clearly uses another language.

User-facing explanations outside repository files may be in Russian.

## Product Boundaries

AmberTower is planned as a game backend platform with the following target backend areas:

* API Gateway
* Authentication
* Player profile
* Wallet and currencies
* Matchmaking, rating, MMR/ELO, and future leaderboard-related logic
* Game events and analytics in later stages

Do not assume that all target services already exist.

## Client Boundary

The Unity client must communicate only with the public API Gateway.

Expected external communication:

```text
Unity Client -> REST/HTTPS -> API Gateway
```

The Unity client must not directly call:

* internal gRPC services;
* Kafka;
* PostgreSQL;
* Redis;
* internal service databases.

## Backend Communication Rules

Expected backend communication model:

```text
API Gateway -> gRPC -> internal backend services
Backend services -> Kafka -> asynchronous domain events
```

Use gRPC for synchronous service-to-service calls.

Use Kafka for asynchronous facts/events that other services may react to independently.

Do not use Kafka as a replacement for direct request/response queries.

Do not use gRPC as a replacement for event distribution.

## Data Ownership Rules

Each service owns its own data.

A service must not directly read or write tables owned by another service.

For local development, multiple logical databases or schemas may run inside one PostgreSQL container. This does not remove service ownership boundaries.

If one service needs data owned by another service, use one of these approaches:

* gRPC query to the owning service;
* Kafka event and local read model;
* documented API contract.

## Contracts

At the early stage, shared contracts may be kept in dedicated contract projects.

Prefer separate contract projects by domain when practical, for example:

* Auth contracts
* Profile contracts
* Wallet contracts
* Matchmaking/rating contracts
* Event contracts

Do not put domain logic into contract projects.

Contract projects should contain only things needed for communication, such as:

* `.proto` files;
* generated gRPC contracts;
* event DTOs;
* simple shared enums used by wire contracts.

## Backend Project Style

Avoid over-complicating service structure at the beginning.

Preferred early-stage approach:

```text
One service = one executable project
```

Inside each service, use folders such as:

```text
Domain/
Application/
Infrastructure/
Persistence/
Grpc/
Consumers/
Options/
```

Do not split every service into separate `Api`, `Application`, `Domain`, and `Infrastructure` projects unless explicitly requested.

## Testing Rules

Add or update unit tests for non-trivial application logic.

Prefer NUnit for backend unit tests unless the existing project uses another test framework.

When backend code changes, run or suggest running:

```text
dotnet build backend/AmberTowerBackend/AmberTowerBackend.sln
```

When tests exist and the change affects testable logic, run or suggest running:

```text
dotnet test backend/AmberTowerBackend/AmberTowerBackend.sln
```

If a command cannot be run, explain why.

## Documentation Rules

Important architectural decisions should be documented.

Use:

```text
docs/architecture/
docs/decisions/
```

Use ADR-style notes for important decisions.

Examples of decisions that should be documented:

* monorepo layout;
* REST API Gateway for Unity client;
* gRPC for internal backend communication;
* separate Wallet service;
* service data ownership;
* Kafka event flow;
* Redis usage;
* testing strategy;
* observability strategy.

Do not update documentation with vague text. Documentation should explain the decision, context, consequences, and trade-offs.

## Security Rules

Never commit real secrets.

Do not add real:

* passwords;
* API keys;
* private keys;
* JWT signing keys;
* database credentials for non-local systems;
* tokens;
* certificates.

Use safe local-development examples only.

Configuration files may contain obvious local placeholders such as:

```text
changeme
local-dev-only
localhost
```

Do not add secrets to source code.

## AI Workflow Rules

For every non-trivial task:

1. Inspect the relevant files.
2. Propose a short plan.
3. Wait for explicit approval before editing files.
4. Make focused changes.
5. Run relevant checks when possible.
6. Summarize results and changed files.

For very small tasks, still state the intended change before making it.

Do not create or modify Skills unless explicitly requested.

Do not create autonomous task-runner folders such as:

```text
/tasks/
/results/
/state/
/context/
```

unless explicitly requested.

## Git and Generated Files

Do not commit generated, temporary, cache, build, or IDE-specific files unless they are intentionally part of the project.

Expected ignored content includes:

* `bin/`
* `obj/`
* `.vs/`
* Rider/IDE caches where appropriate
* Unity `Library/`
* Unity `Temp/`
* Unity `Obj/`
* Unity `Build/`
* local environment files with secrets

Do not modify `.gitignore` broadly without explaining the reason.

## Error Handling and Honesty

If a requested change is unsafe, unclear, or likely to damage the project structure, explain the concern and propose a safer alternative.

If something was not verified, say so.

Do not claim that build, tests, migrations, or Docker Compose were run unless they were actually run.

Do not hide errors. Report them with the relevant command and a concise explanation.

## Current Architectural Direction

Current intended architecture:

```text
Unity Client
  -> REST/HTTPS
  -> API Gateway
  -> gRPC
  -> internal backend services

Backend services
  -> Kafka
  -> asynchronous event consumers

Services
  -> own PostgreSQL database/schema

Redis
  -> caching, rate limiting, matchmaking/rating/leaderboard support when needed
```

This direction may evolve, but changes should be deliberate and documented.
