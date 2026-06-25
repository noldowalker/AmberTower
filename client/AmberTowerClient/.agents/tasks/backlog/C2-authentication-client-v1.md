# Authentication Client V1

## Goal

Create the first Unity client authentication flow against the public `ApiGateway` REST API.

## Scope

1. Create registration UI.
2. Create login UI.
3. Call public REST endpoints:
   - `POST /api/auth/register`
   - `POST /api/auth/login`
4. Handle successful and failed responses explicitly.
5. Store the returned access token at a minimal viable level.
6. Show a clear success or error state in the UI.

## Constraints

- The Unity client must call only `ApiGateway`.
- The Unity client must not know about gRPC or internal backend services.
- Client network code should remain isolated from gameplay code.

## Out Of Scope For V1

- refresh token flow
- automatic re-authentication
- scene-wide session management
- external auth providers
- production-grade secure token storage discussion

## Notes

- This task should start only after the backend auth REST contract is implemented and stable enough for client integration.
