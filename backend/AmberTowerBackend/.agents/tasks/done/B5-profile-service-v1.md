# Profile Service V1

## Goal

Introduce the first player profile vertical slice so the game client can display and edit the authenticated player's public profile information.

Target flow:

```text
Unity Client
  -> REST/HTTPS
  -> ApiGateway
  -> gRPC
  -> ProfileService
```

## Scope

1. Create `ProfileService` as an internal backend service.
2. Define gRPC contracts for:
   - get current player profile by authenticated user id;
   - update current player nickname.
3. Add public `ApiGateway` endpoints:
   - `GET /api/profile/me`
   - `PATCH /api/profile/me`
4. Protect public profile endpoints with JWT Bearer authentication.
5. Store profile data in PostgreSQL with service-owned persistence.
6. Profile data should include at least:
   - `playerId`
   - `authUserId`
   - `email`
   - `nickname`
   - `createdAtUtc`
   - `updatedAtUtc`
7. Decide and implement how profiles are created:
   - create on first `GET /api/profile/me`; or
   - create during/after registration through an event or explicit call.
8. Add focused tests for nickname validation and profile update behavior.
9. Add Docker Compose wiring for `ProfileService`.
10. Update architecture or decision documentation if profile ownership or creation flow is documented.

## Constraints

- Unity client must call only `ApiGateway`.
- `ProfileService` must not be exposed directly to the Unity client.
- `ProfileService` owns its profile data.
- Do not let `ApiGateway` directly read or write `ProfileService` tables.
- Keep V1 small: one executable service project plus focused tests.
- Do not introduce avatars, cosmetics, friends, or account management in this task.

## Out Of Scope

- wallet and currencies;
- profile avatars;
- public player search;
- friend lists;
- profile privacy settings;
- profile events through Kafka unless explicitly chosen for profile creation.

## Notes

This task follows `B4-api-gateway-jwt-validation`.

The first client consumer is expected to be `C3-main-game-screen-profile-v1`.
