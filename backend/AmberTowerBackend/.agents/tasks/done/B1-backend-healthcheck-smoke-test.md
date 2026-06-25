# Backend Healthcheck Smoke Test

## Goal

Keep a minimal backend entry point available for Unity connectivity checks.

## Current State

- `ApiGateway` exists in `backend/AmberTowerBackend/src/ApiGateway/`
- `GET /` returns a simple JSON payload
- `GET /health` returns a health-check JSON payload
- Docker compose exposes the service on `http://localhost:8080`

## Outcome

1. `ApiGateway` health endpoint is available for Unity smoke tests.
2. Local Docker workflow is available for `http://localhost:8080/health`.
3. The Unity client health-check UI can use this endpoint as its current backend target.
