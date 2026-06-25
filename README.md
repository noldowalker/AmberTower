# AmberTower

AmberTower is a monorepo for a learning game backend platform and a Unity client.

The repository is intended as a foundation for a production-inspired system, not as a throwaway prototype.

## Repository Structure

```text
AmberTower/
  AGENTS.md
  README.md
  backend/
    AmberTowerBackend/
      AGENTS.md
      AmberTowerBackend.sln
  client/
    AmberTowerClient/
      AGENTS.md
      Assets/
      Packages/
      ProjectSettings/
  docs/
  deploy/
  scripts/
```

## Current Status

- `backend/AmberTowerBackend/AmberTowerBackend.sln` exists but does not contain service projects yet.
- `client/AmberTowerClient/` contains the Unity project.
- Unity generated folders such as `Library/`, `Logs/`, and `UserSettings/` are local-only and ignored by Git.

## Intended Architecture

```text
Unity Client
  -> REST/HTTPS
  -> ApiGateway
  -> gRPC
  -> internal backend services

Backend services
  -> Kafka
  -> asynchronous domain events

Services
  -> own PostgreSQL database/schema

Redis
  -> caching and other supporting scenarios when needed
```

## Backend Direction

The backend is planned as a .NET 8 microservice-oriented system.

Initial target areas:

- ApiGateway
- Authentication
- Player profile
- Wallet and currencies
- Matchmaking, rating, and future leaderboard-related logic
- Game events and analytics in later stages

The Unity client must communicate only with the public ApiGateway and must not access internal gRPC services, Kafka, PostgreSQL, Redis, or internal service databases directly.

## Docker Compose Commands

Run these commands from the repository root.

Build images and start the local backend stack:

```powershell
docker compose -f deploy/docker-compose.yml up -d --build
```

Use this after code or Dockerfile changes. It rebuilds service images if needed and starts the containers in the background.

Restart the existing containers without rebuilding images:

```powershell
docker compose -f deploy/docker-compose.yml restart
```

Use this after configuration-only changes that do not require rebuilding images.

Stop and remove the containers, keeping the PostgreSQL Docker volume:

```powershell
docker compose -f deploy/docker-compose.yml down
```

Use this when you want a clean container restart but want to keep local database data.

Stop and remove containers plus local volumes:

```powershell
docker compose -f deploy/docker-compose.yml down -v
```

Use this when you want to reset local PostgreSQL data as well. This deletes the local Docker volume used by the compose stack.

Fully rebuild and restart from scratch while keeping database data:

```powershell
docker compose -f deploy/docker-compose.yml down
docker compose -f deploy/docker-compose.yml up -d --build
```

Check running containers:

```powershell
docker compose -f deploy/docker-compose.yml ps
```

Follow logs for all services:

```powershell
docker compose -f deploy/docker-compose.yml logs -f
```

Follow logs for one service:

```powershell
docker compose -f deploy/docker-compose.yml logs -f apigateway
docker compose -f deploy/docker-compose.yml logs -f authservice
docker compose -f deploy/docker-compose.yml logs -f postgres
```

## Next Safe Step

The next safe backend step is to create a minimal `ApiGateway` service under `backend/AmberTowerBackend/src/` and add it to the existing solution.
