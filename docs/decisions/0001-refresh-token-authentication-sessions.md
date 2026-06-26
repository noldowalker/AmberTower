# ADR 0001: Refresh Token Authentication Sessions

## Status

Accepted

## Context

AmberTower uses a Unity client that communicates only with the public REST API exposed by `ApiGateway`.

`B2-authentication-backend-v1` introduced email/password login and JWT access tokens. A client can cache an access token, but access tokens should be short-lived and should not be the only mechanism for keeping a player authenticated across application restarts.

Storing the user's password locally would keep the client simple, but it would also turn the password into a long-lived client-side secret. If leaked, the password can be reused outside the current session and potentially outside AmberTower if the user reused credentials.

## Decision

AmberTower will use refresh-token based sessions for durable client authentication.

The AuthService issues:

- a short-lived JWT access token for authenticated API calls;
- a longer-lived opaque refresh token for obtaining a new token pair.

The Unity client sends refresh requests only to `ApiGateway`:

```text
Unity Client
  -> REST/HTTPS
  -> ApiGateway
  -> gRPC
  -> AuthService
```

Refresh tokens are stored by AuthService as hashes, not as raw tokens. A successful refresh rotates the refresh token: the old token is revoked and a new refresh token is issued.

Explicit logout revokes the presented refresh token.

## Consequences

The Unity client can keep a player authenticated across application restarts without storing the player's password locally.

If an access token expires, the client can use the refresh token to obtain a new token pair. If refresh fails because the refresh token is missing, expired, revoked, or invalid, the client clears local auth state and returns to login.

Refresh-token storage becomes part of AuthService persistence and requires database schema migration.

Refresh-token reuse must be rejected. A later version may add broader session-chain revocation or per-device session management.

## Initial Defaults

- Access token lifetime: currently configured as `60` minutes for local development.
- Refresh token lifetime: `30` days for local development.

These values are development defaults and can be adjusted through configuration.
