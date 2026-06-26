# Authentication Client V1

## Goal

Create the first Unity client authentication flow against the public `ApiGateway` REST API using the refresh-token session model introduced by `B3-refresh-token-authentication-sessions-v1`.

This task should start after the backend refresh-token contract is implemented and stable enough for client integration.

## Scope

1. Create registration UI.
2. Create login UI.
3. Call public REST endpoints:
   - `POST /api/auth/register`
   - `POST /api/auth/login`
   - `POST /api/auth/refresh`
   - `POST /api/auth/logout`
4. Store the returned access token, access-token expiration, refresh token, and refresh-token expiration at a minimal viable level.
5. On application/auth UI startup, restore the cached session:
   - use the cached access token if it has not expired;
   - otherwise use the cached refresh token to request a new token pair;
   - otherwise show the login/register UI.
6. Handle successful and failed responses explicitly.
7. Show clear authenticated, loading, success, and error states in the UI.
8. Add a "Change account" action that logs out when possible, clears local auth data, and returns to login/register UI.

## Constraints

- The Unity client must call only `ApiGateway`.
- The Unity client must not know about gRPC or internal backend services.
- Client network code should remain isolated from gameplay code.
- Do not store the user's password locally.
- Do not log passwords, access tokens, refresh tokens, authorization headers, or private user data.
- Token storage must be isolated behind a client-side session storage abstraction so it can be replaced later with platform-specific secure storage.
- If `PlayerPrefs` is used for the first implementation, document it in code or task notes as a local-development/minimal viable storage choice, not production-grade secure storage.

## Out Of Scope For V1

- external auth providers
- production-grade platform-specific secure token storage implementation
- roles and permissions
- full account management
- password reset flow
- scene-wide gameplay authorization rules

## Notes

- This task depends on `B3-refresh-token-authentication-sessions-v1`.
- The client should be able to keep a user authenticated across application restarts as long as the refresh token is valid.
- If refresh fails, the client should clear local session data and show the login/register UI.
