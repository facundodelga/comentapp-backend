# Feature: Mercado Pago Connect

## Goal

A creator connects a Mercado Pago account so they can receive paid comments.

## Current State

Status: `implemented` (backend, not yet exercised end-to-end — needs MP app credentials + a connected test account).

Implemented in `comentapp.business.endpoint`:

- `MercadoPagoConnectController` (route base `/MercadoPago`) with `connect`, `callback`, `status`, `DELETE connection`.
- `MercadoPagoConnectService` orchestration; `state` anti-CSRF persisted in `MercadoPagoOAuthState` (carries `CreatorId`, since the Strict session cookie does not survive MP's cross-site callback).
- `IMercadoPagoOAuthService` (infrastructure) exchanges/refreshes tokens via raw HTTP `/oauth/token` (SDK 3.3.0 has no OAuth client).
- Tokens encrypted at rest via `ITokenProtector` (DataProtection). Both APIs share `SetApplicationName("ComentApp")`.
- Requires an existing `Creator` (step 2 / `POST /Creators`) before connect can start.

## Proposed Endpoints

### GET /MercadoPago/connect  ✅

Requires creator auth. Returns `{ authorizationUrl }`. (Does not redirect server-side; frontend redirects the user.) Fails if the user is not yet a creator (step 2).

### GET /MercadoPago/callback  ✅

Handles provider callback. Validates `state`, exchanges `code`, stores encrypted tokens, then redirects to `Frontend.BaseUrl/creator/mercadopago?connect=success|error`.

### GET /MercadoPago/status

Requires creator auth.

Response:

```json
{
  "connected": true,
  "accountId": "masked-or-public-account-id"
}
```

### DELETE /MercadoPago/connection

Disconnects current creator account where product rules allow it.

## Rules

- Never expose access tokens to frontend.
- Store secrets using secure configuration/storage pattern, not plain committed config.
- Creator owner comes from auth session.
- Callback must validate state/anti-forgery value.
- A creator without connection cannot receive paid comments.

## Acceptance Criteria

- Creator can start connection flow.
- Callback stores account reference.
- Status endpoint reports connected/disconnected state.
- Donation checkout blocks disconnected creators.

