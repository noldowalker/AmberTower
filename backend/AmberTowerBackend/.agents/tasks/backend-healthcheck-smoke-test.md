# Backend Healthcheck Smoke Test

## Goal

Keep a minimal backend entry point available for Unity connectivity checks.

## Current State

- `ApiGateway` exists in `backend/AmberTowerBackend/src/ApiGateway/`
- `GET /` returns a simple JSON payload
- `GET /health` returns a health-check JSON payload
- Docker compose exposes the service on `http://localhost:8080`

## Next Relevant Backend Tasks

1. Keep `ApiGateway` health endpoint stable for client smoke tests.
2. Preserve local Docker workflow for `http://localhost:8080/health`.
3. Extend `ApiGateway` only after Unity client smoke test is working.
