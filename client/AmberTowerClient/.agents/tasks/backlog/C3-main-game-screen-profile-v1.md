# Main Game Screen Profile V1

## Goal

Replace the authentication-only first screen with an authenticated game home screen that displays the current player identity and opens a profile card.

## Scope

1. After login or session restore, call protected backend endpoints using:
   - `Authorization: Bearer <accessToken>`
2. Load current profile data from:
   - `GET /api/profile/me`
3. Show a main game screen with:
   - background image or placeholder visual background;
   - top-left profile portrait placeholder;
   - current nickname if present;
   - email fallback if nickname is empty.
4. Open a detailed profile card when the profile area is clicked.
5. The profile card should show:
   - portrait placeholder;
   - email;
   - user/player identity;
   - nickname.
6. Add edit mode for nickname:
   - enter edit mode;
   - edit nickname field;
   - save through `PATCH /api/profile/me`;
   - cancel without saving.
7. Handle loading, success, validation error, unauthorized, and network failure states.
8. If a protected call returns `401`, try refresh-token flow once and retry the protected call.
9. If refresh fails, clear local session and return to login.
10. Implement protected requests through a shared authentication-aware API layer:
   - attach `Authorization: Bearer <accessToken>` automatically;
   - on `401`, try refresh-token flow once;
   - if refresh succeeds, retry the original protected request once;
   - if refresh fails, clear local session and return to login.

## Constraints

- The Unity client must call only `ApiGateway`.
- Do not store or log passwords, access tokens, refresh tokens, authorization headers, or private user data.
- Keep protected request handling isolated in the infrastructure/auth layer.
- Do not hardcode production URLs.
- Do not change backend code in this task unless explicitly requested.
- Keep UI changes under `Assets/` and avoid touching generated/cache folders.

## Out Of Scope

- wallet/currency display;
- avatar upload or selection;
- profile privacy settings;
- friend lists;
- gameplay navigation;
- production-grade secure token storage implementation;
- advanced request queuing or parallel refresh coordination.

## Notes

This task depends on `B5-profile-service-v1`.

Currency UI is planned for `C4-main-game-screen-wallet-v1`.
