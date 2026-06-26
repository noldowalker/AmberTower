# API Gateway JWT Validation

## Goal

Add JWT Bearer validation to `ApiGateway` and expose the first protected current-user endpoint.

## Scope

1. Configure `ApiGateway` to validate access tokens issued by `AuthService`.
2. Add authentication and authorization middleware to `ApiGateway`.
3. Add a protected public REST endpoint:
   - `GET /api/me`
4. Return current authenticated user identity from token claims:
   - `userId`
   - `email`
5. Ensure unauthenticated requests receive `401 Unauthorized`.
6. Ensure malformed or expired tokens receive `401 Unauthorized`.
7. Keep Unity client communication limited to `ApiGateway`.
8. Do not expose internal gRPC services directly.
9. Update configuration and Docker Compose as needed for JWT validation settings.
10. Add focused tests or document manual verification steps if endpoint-level integration tests are not yet available.

## Constraints

- Do not log access tokens, refresh tokens, authorization headers, passwords, or private user data.
- Do not hardcode production secrets.
- Use only local-development placeholder secrets in committed configuration.
- Keep the implementation small and focused on `ApiGateway` authentication.
- Do not introduce roles or permissions in this task.

## Out Of Scope

- role-based authorization;
- profile service integration;
- refresh-token changes;
- client UI changes;
- production secret management;
- external identity providers.

## Notes

This task follows:

- `B2-authentication-backend-v1`
- `B3-refresh-token-authentication-sessions-v1`
- `C2-authentication-client-v1`

The client can already obtain and cache access tokens. This task lets the backend verify those access tokens on protected public endpoints.
