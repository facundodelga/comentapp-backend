# Feature: Mercado Pago Payment Callbacks

## Goal

Backend receives Mercado Pago payment updates, verifies payment server-side, and updates donation/comment state.

## Current State

Status: `planned`.

No webhook/notification endpoint exists.

## Proposed Endpoints

### POST /MercadoPago/webhooks

Receives provider notifications. Should be idempotent.

### GET /Payments/{donationId}

Returns current payment/donation status for authenticated involved user or creator owner.

Response:

```json
{
  "donationId": 456,
  "status": "approved",
  "commentVisible": true
}
```

## Payment States

- `pending`
- `approved`
- `rejected`
- `cancelled`
- `failed`

Map Mercado Pago native statuses into these product states.

## Rules

- Webhook handler must be idempotent.
- Do not trust frontend return query params as final payment status.
- Verify payment/preference ids with Mercado Pago API before approving.
- Unknown donation/payment references should not crash handler.
- Approved payment marks comment visible/confirmed.

## Acceptance Criteria

- Repeated webhook for same payment does not duplicate comments.
- Approved payment confirms donation/comment.
- Failed/rejected payment does not show comment as confirmed.
- User can see payment result state after returning from checkout.

