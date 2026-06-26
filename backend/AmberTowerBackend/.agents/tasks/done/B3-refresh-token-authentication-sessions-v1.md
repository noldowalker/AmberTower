# Refresh Token Authentication Sessions V1

## Goal

Extend backend authentication with refresh-token based sessions so the Unity client can keep a user authenticated across application restarts without storing the user's password locally.

Target flow:

```text
Unity Client
  -> REST/HTTPS
  -> ApiGateway
  -> gRPC
  -> AuthService
```

## Context

`B2-authentication-backend-v1` introduced registration, login, password hashing, PostgreSQL-backed auth persistence, and JWT access tokens.

`C2-authentication-client-v1` should use the session contract produced by this task. The intended client behavior is:

```text
App starts
  -> use cached access token if it is still valid
  -> otherwise use refresh token to obtain a new access token
  -> otherwise show login/register UI
```

Storing the user's password locally should not be part of the normal client session model.

## Scope

1. Add refresh-token support to `AuthService`.
2. Generate refresh tokens as high-entropy opaque values.
3. Store refresh tokens securely, for example as hashed tokens with expiration and revocation metadata.
4. Return both access token and refresh token from successful login.
5. Add refresh-token rotation: a successful refresh invalidates the old refresh token and issues a new one.
6. Add a public `ApiGateway` endpoint:
   - `POST /api/auth/refresh`
7. Add a public `ApiGateway` endpoint for explicit logout/session revocation:
   - `POST /api/auth/logout`
8. Consider adding a public endpoint for validating or describing the current authenticated user:
   - `GET /api/me`
9. Ensure the Unity client still communicates only with `ApiGateway`.
10. Add focused backend tests for refresh-token issuing, expiration, rotation, reuse detection, and revocation logic.
11. Update Swagger metadata for the new public REST endpoints.
12. Update relevant architecture or decision documentation for the authentication session model.

## Suggested REST Contract

Successful login response should include:

```json
{
  "userId": "...",
  "email": "...",
  "accessToken": "...",
  "accessTokenExpiresAtUnixSeconds": 1234567890,
  "refreshToken": "...",
  "refreshTokenExpiresAtUnixSeconds": 1234567890
}
```

Refresh request:

```http
POST /api/auth/refresh
Content-Type: application/json
```

```json
{
  "refreshToken": "..."
}
```

Successful refresh response should return a new access token and a new refresh token.

Logout request may accept the current refresh token and revoke it:

```json
{
  "refreshToken": "..."
}
```

## Expected Client Behavior Enabled By This Task

1. If the cached access token has not expired, the Unity client continues using it.
2. If the access token has expired and a refresh token exists, the Unity client calls `POST /api/auth/refresh`.
3. If refresh succeeds, the Unity client stores the new token pair and continues the session.
4. If refresh fails, the Unity client clears local auth data and returns to login.
5. If the user selects "Change account", the Unity client calls `POST /api/auth/logout` when possible, clears local auth data, and shows the login/register UI.

## Constraints

- The Unity client must call only `ApiGateway`.
- Internal gRPC services must not be exposed directly to the Unity client.
- Do not store raw refresh tokens in the database.
- Do not log access tokens, refresh tokens, passwords, or authorization headers.
- Do not use local password storage as the backend-supported session strategy.
- Refresh-token expiration, rotation, and revocation behavior must be explicit.
- Refresh token reuse should be handled deliberately. At minimum, reject reused revoked tokens; preferably revoke the affected token chain or user session.
- Local development configuration may use safe placeholder secrets only.

## Out Of Scope For V1

- external auth providers;
- email verification;
- password reset;
- roles and permissions;
- multi-device session management UI;
- production-grade device trust or risk scoring.

## Notes

- This task is intentionally separate from `C2-authentication-client-v1`.
- `C2-authentication-client-v1` should be implemented after this task so the client can be built around refresh tokens from the start.
- Prefer short-lived access tokens and longer-lived refresh tokens.
- Suggested initial lifetimes: access token `15-60` minutes, refresh token `7-30` days. Exact values should be documented when implemented.
