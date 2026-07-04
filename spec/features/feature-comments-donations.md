# Feature: Donation Comment Checkout

## Goal

An authenticated user selects one creator, enters a short comment and amount, and receives a Mercado Pago checkout URL for payment.

## Current State

Status: `planned`.

Implemented foundation:

- `Comment` EF model.
- `CreateComments` migration.
- `CommentsController.POST` accepts a request and echoes it.

Missing:

- Real persistence.
- Payment/preference model.
- Creator lookup.
- Mercado Pago client.
- Auth enforcement in business endpoint.

## Proposed Endpoint

### POST /DonationComments

Requires auth cookie.

Request:

```json
{
  "creatorId": 123,
  "comment": "Gran stream",
  "amount": 1500
}
```

Response:

```json
{
  "donationId": 456,
  "preferenceId": "mp-pref-id",
  "checkoutUrl": "https://..."
}
```

## Rules

- `creatorId` must exist.
- Creator must be able to receive payments.
- `comment` is required and max 300 characters.
- `amount` must be greater than zero.
- Current user id comes from auth session.
- Record starts in pending state before redirecting to checkout.
- Confirmed comment visibility waits for payment verification.

## Data Note

Current `Comment` model has no amount or payment status. Implementing this feature likely needs either:

- Add payment fields to `Comment`, or
- Add `Donation`/`Payment` entity linked to `Comment`.

Prefer explicit payment lifecycle fields over deriving state from Mercado Pago query params.

## Acceptance Criteria

- Valid request creates pending donation/comment intent.
- Response contains checkout URL.
- Invalid creator/comment/amount returns validation error.
- Unauthenticated request returns 401.
- No comment appears as confirmed before server-side payment approval.

