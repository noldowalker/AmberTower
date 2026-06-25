# Authentication Backend V1

## Goal

Implement the first real authentication vertical slice for the backend:

```text
Unity Client
  -> REST/HTTPS
  -> ApiGateway
  -> gRPC
  -> AuthService
```

## Scope

1. Create `AuthService` as a real internal backend service.
2. Define gRPC contracts for:
   - `Register`
   - `Login`
3. Connect `ApiGateway` to `AuthService` over gRPC.
4. Add public REST endpoints to `ApiGateway`:
   - `POST /api/auth/register`
   - `POST /api/auth/login`
5. Add Swagger to `ApiGateway` for public REST endpoints.
6. Implement user storage in the auth domain.
7. Implement password hashing.
8. Return JWT access tokens from successful login.
9. Add focused backend tests for non-trivial auth logic.
10. Wrap the full local auth vertical slice in Docker Compose.

## Constraints

- Unity client must continue to communicate only with `ApiGateway`.
- `AuthService` remains internal and is not exposed directly to the client.
- Swagger belongs only to `ApiGateway`, not to internal gRPC services.
- Each service owns its own data.
- Local development for this task must run through Docker Compose.
- Docker Compose must include at least:
  - `ApiGateway`
  - `AuthService`
  - PostgreSQL for auth persistence

## Out Of Scope For V1

- refresh tokens
- roles and permissions
- email verification
- external auth providers
- password reset flow
- full account management

## Notes

- This is the first backend task that introduces a real internal microservice plus gRPC communication.
- PostgreSQL-backed persistence is expected if the task moves into implementation.
- The backend result is not complete unless the local Docker workflow can run the auth slice end-to-end.
