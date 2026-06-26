# Wallet Service V1

## Goal

Introduce the first wallet vertical slice so the game client can display the authenticated player's currencies on the main game screen.

Target flow:

```text
Unity Client
  -> REST/HTTPS
  -> ApiGateway
  -> gRPC
  -> WalletService
```

## Scope

1. Create `WalletService` as an internal backend service.
2. Define gRPC contracts for reading the current player's wallet.
3. Add a protected public `ApiGateway` endpoint:
   - `GET /api/wallet/me`
4. Wallet data should include at least:
   - `playerId` or `authUserId`
   - `gold`
   - `crystals`
5. Store wallet data in PostgreSQL with service-owned persistence.
6. Decide and implement initial wallet creation:
   - create on first read; or
   - create when profile/player is created.
7. Add focused tests for wallet creation and balance read behavior.
8. Add Docker Compose wiring for `WalletService`.
9. Document wallet ownership and initial currency behavior if needed.

## Constraints

- Unity client must call only `ApiGateway`.
- `WalletService` must not be exposed directly to the Unity client.
- `WalletService` owns wallet data.
- Do not trust client-provided currency amounts.
- V1 is read-only from the client.
- Wallet balances must not become negative.
- Do not introduce reward calculation, purchases, or transaction history unless required for the minimal safe implementation.

## Out Of Scope

- credit/debit operations;
- transaction history;
- purchases;
- rewards;
- idempotency keys;
- Kafka-driven wallet updates;
- leaderboards or rating.

## Notes

This task can follow `B5-profile-service-v1`, but it should remain a separate wallet-owned vertical slice.

The first client consumer is expected to be `C4-main-game-screen-wallet-v1`.
