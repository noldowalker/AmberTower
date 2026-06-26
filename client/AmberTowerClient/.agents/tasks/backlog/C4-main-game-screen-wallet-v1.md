# Main Game Screen Wallet V1

## Goal

Add currency display to the authenticated game home screen.

## Scope

1. Load wallet data from the protected backend endpoint:
   - `GET /api/wallet/me`
2. Send access token as:
   - `Authorization: Bearer <accessToken>`
3. Display current currencies in the top-right area:
   - gold;
   - crystals.
4. Handle loading, success, unauthorized, refresh retry, and network failure states.
5. If wallet request returns `401`, try refresh-token flow once and retry.
6. If refresh fails, clear local session and return to login.
7. Reuse the shared protected-request API layer introduced for authenticated endpoints:
   - attach `Authorization: Bearer <accessToken>` automatically;
   - on `401`, try refresh-token flow once;
   - if refresh succeeds, retry the original protected request once;
   - if refresh fails, clear local session and return to login.

## Constraints

- The Unity client must call only `ApiGateway`.
- Do not let UI scripts perform raw HTTP calls directly.
- Keep wallet networking in infrastructure/API layer.
- Do not store or log access tokens, refresh tokens, authorization headers, or private user data.
- Do not add client-side currency mutation in this task.

## Out Of Scope

- purchases;
- rewards;
- transaction history;
- wallet mutation UI;
- inventory;
- gameplay economy balancing;
- advanced request queuing or parallel refresh coordination.

## Notes

This task depends on `B6-wallet-service-v1`.

It should build on the main game screen introduced by `C3-main-game-screen-profile-v1`.
