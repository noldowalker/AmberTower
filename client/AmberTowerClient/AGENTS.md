# AmberTower Unity Client Agent Instructions

## Client Overview

This directory contains the Unity client for AmberTower.

The Unity client is a game client for interacting with the AmberTower backend through the public API Gateway.

The client must stay decoupled from backend internal implementation details.

Additional client-specific coding conventions may be documented in:

```text
client/AmberTowerClient/.agents/CODESTYLE.md
```

When present, follow that document in addition to this file.

Client agents must always read and follow:

```text
client/AmberTowerClient/.agents/CODESTYLE.md
```

Code style rules are mandatory for all client changes.

## Expected Location

Expected Unity client location:

```text
client/AmberTowerClient/
```
Expected instruction file location:

```
client/AmberTowerClient/AGENTS.md
```
Communication Rules

The Unity client communicates only with the public API Gateway.

Expected communication model:

```Unity Client -> REST/HTTPS -> ApiGateway```

The Unity client must not directly call:

```internal gRPC services;
Kafka;
PostgreSQL;
Redis;
internal backend service endpoints;
internal service databases.
```

Use REST/JSON for backend communication unless explicitly requested otherwise.

Project Structure

Expected Unity project layout:


```client/
  AmberTowerClient/
    AGENTS.md
    Assets/
    Packages/
    ProjectSettings/
```

Do not move Unity project folders unless explicitly requested.

Do not restructure the Unity project without explaining the reason and receiving approval.

Unity Generated Files

Do not edit or commit generated, temporary, cache, or local machine-specific folders unless explicitly requested.

Generated/cache folders include:

```Library/
Temp/
Obj/
Logs/
Build/
Builds/
UserSettings/
```
These folders should normally be ignored by Git.

Do not modify generated solution or project files unless explicitly requested.

Unity, Rider, and IDE-generated files may be regenerated automatically and should not be treated as stable architecture.

Networking Rules

Keep backend communication isolated from gameplay code.

Prefer a dedicated API client layer, for example:

```Assets/
  Scripts/
    Infrastructure/
      BackendApi/
    Gameplay/
    UI/
```
Do not scatter raw HTTP calls across unrelated gameplay scripts.

Do not hardcode production URLs in gameplay code.

Use configuration for API base URL.

Local development URLs are allowed only when clearly marked as local development configuration.

Public API Assumptions

The client should expect the backend to expose public REST/HTTPS endpoints through ApiGateway.

Examples of future public endpoints:

```POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh
GET  /api/me
GET  /api/profile/me
PATCH /api/profile/me
GET  /api/wallet/me
POST /api/runs/complete
```
Do not expose internal microservice details in client-facing code.

The Unity client should not need to know whether the backend uses gRPC, Kafka, Redis, or multiple databases internally.

Authentication Rules

The client may receive and store access tokens for authenticated requests.

Do not store refresh tokens insecurely without explicit discussion.

Do not log:

```passwords;
access tokens;
refresh tokens;
authentication headers;
private user data.
```
Use the standard header for authenticated requests:

Authorization: Bearer <access_token>

If token storage is introduced, explain the storage strategy and its risks.

Error Handling

Network code should handle expected failure cases explicitly.

Examples:

```no connection;
timeout;
unauthorized response;
expired access token;
backend validation error;
server error;
malformed response.
```
Do not silently swallow networking errors.

Do not display raw internal backend errors directly to the player.

Prefer mapping backend errors into client-friendly messages or states.

Async Rules

Do not block the Unity main thread with long-running network operations.

Prefer async request handling, coroutines, or a clearly isolated networking abstraction.

Avoid mixing gameplay state changes directly into low-level networking code.

Configuration Rules

Do not hardcode environment-specific configuration in gameplay scripts.

Expected configurable values:

```API base URL;
request timeout;
development mode flags;
logging verbosity.
```
Keep local development values separate from production-like configuration.

Do not commit real secrets.

Code Organization Rules

Keep responsibilities separated.

Recommended high-level separation:

```Assets/
  Scripts/
    Infrastructure/
      BackendApi/
      Auth/
      Configuration/
    Gameplay/
    UI/
```
Networking DTOs should be explicit.

Do not let Unity scene scripts become responsible for raw HTTP details.

Do not introduce large frameworks or code generators without explicit approval.

Backend Boundary

Do not change backend code while working on a client-only task unless explicitly requested.

If a client task requires a backend API change, propose the backend change separately and wait for approval.

Documentation

If client-backend communication assumptions change, update relevant documentation in:

```docs/architecture/
docs/decisions/
```
Examples of changes that should be documented:

```switching from REST to gRPC/gRPC-Web;
adding WebSocket communication;
changing authentication flow;
changing token storage strategy;
adding offline mode or local persistence;
adding client-side telemetry.
Agent Workflow
```
Before editing Unity files:

```Inspect the current Unity project structure.
Read this file.
Read and follow client/AmberTowerClient/.agents/CODESTYLE.md.
Propose a short plan.
Wait for explicit approval.
Make focused changes.
Summarize changed files and what was not verified.
```

Unity client task files must use the `C<number>-name-of-task.md` format inside:

```text
.agents/
  tasks/
    backlog/
    current/
    done/
```

Examples:

```text
C1-healthcheck-ui.md
C2-auth-login-panel.md
```

Do not regenerate Unity project files unless explicitly requested.

Do not make broad project restructuring changes unless explicitly requested.

Do not modify generated/cache folders unless explicitly requested.

Verification

If Unity-specific verification cannot be run from the current environment, say so.

Do not claim that Unity was opened, the project was imported, or scenes were tested unless that was actually done.

When possible, keep changes limited to normal source files under:

```Assets/
```
and avoid changing Unity-generated metadata unless necessary.

Current Client Direction

Current intended direction:

```Unity Client
  -> REST/HTTPS
  -> ApiGateway
```
```ApiGateway
  -> internal backend services
```

```Unity Client
  does not call internal gRPC services directly
  does not know about Kafka
  does not know about backend databases
```
This direction may evolve, but changes must be deliberate and documented.
