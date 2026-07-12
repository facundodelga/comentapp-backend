# Feature: Donation Comment Checkout

## Goal

An authenticated user selects one creator, enters a short comment and amount, and receives a Mercado Pago checkout URL for payment.

## Current State

Status: `implemented` (backend, not yet exercised end-to-end — needs a connected MP creator + credentials).

Implemented in `comentapp.business.endpoint`:

- `POST /DonationComments` (auth-enforced) via `DonationCheckoutService`.
- Creates `Payment` (Pending) + `Comment` (unconfirmed), links them, then a Checkout Pro preference with marketplace split.
- Fee: `MercadoPago:MarketplaceFeePercent` (default 3%). TODO: move to `Setting`.
- Blocks creators without an active MP connection (422).
- Uses the creator's OAuth token (auto-refreshes if expired) via `IMercadoPagoPreferenceService`.
- `external_reference` = `Payment.Id` for webhook reconciliation.

Note: the old `CommentsController.POST` echo stub still exists and is unrelated.

Still missing:

- Webhook confirmation (`feature-mercadopago-callbacks`) — comment stays unconfirmed until then.
- No test project.

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

