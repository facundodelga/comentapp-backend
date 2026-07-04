# Feature: Mercado Pago Connect

## Goal

A creator connects a Mercado Pago account so they can receive paid comments.

## Current State

Status: `planned`.

No Mercado Pago OAuth/connect flow exists yet.

## Proposed Endpoints

### GET /MercadoPago/connect

Requires creator auth. Starts Mercado Pago OAuth/connect flow and returns or redirects to provider authorization URL.

### GET /MercadoPago/callback

Handles provider callback. Exchanges authorization code for account/token data and stores required account reference securely.

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

