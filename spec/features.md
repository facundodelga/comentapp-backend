# ComentApp Features

## Status Legend

- `done`: implemented and usable.
- `partial`: usable foundation exists, but missing required behavior.
- `planned`: documented intent, no real implementation yet.

## Feature Map

| Feature | Status | Spec |
| --- | --- | --- |
| Authentication and session | partial | `spec/features/feature-auth.md` |
| Google authentication | partial | `spec/features/feature-google-auth.md` |
| Creator activation/profile | planned | `spec/features/feature-creators.md` |
| Donation comment checkout | planned | `spec/features/feature-comments-donations.md` |
| Mercado Pago connect | planned | `spec/features/feature-mercadopago-connect.md` |
| Mercado Pago callbacks | planned | `spec/features/feature-mercadopago-callbacks.md` |
| Creator realtime dashboard | planned | `spec/features/feature-dashboard-realtime.md` |

## Current Implementation Snapshot

- Registration exists with duplicate email/username checks, password hashing, and confirmation email.
- Email confirmation exists.
- Login/logout/refresh exist with HTTP-only cookies and refresh token rotation.
- Google OAuth has package/template/provider scaffolding, but no working endpoints yet.
- `GET /Authentication/me` exists but returns only `name` and `email`.
- `Creator` EF model and migration exist, but no creator API exists.
- `Comment` EF model and migration exist, but `POST /Comments` only echoes request data.
- Mercado Pago integration is not implemented.
- SignalR realtime dashboard is not implemented.

## Recommended Delivery Order

1. Align `GET /Authentication/me` contract and frontend user shape.
2. Complete Google OAuth if social login is part of current auth milestone.
3. Add creator API and `isCreator` calculation.
4. Implement creator settings/onboarding flow.
5. Implement Mercado Pago connect.
6. Implement donation comment preference creation.
7. Implement Mercado Pago webhook/callback verification.
8. Persist and expose confirmed comments.
9. Add creator realtime dashboard with SignalR.

## Cross-Feature Decisions Needed

- Whether creator activation can happen before Mercado Pago connection.
- Whether `Creator.MercadoPagoAccount` should be nullable while connection is pending.
- Whether donation/payment state lives on `Comment` or a separate `Donation`/`Payment` entity.
- Whether auth and business endpoints are deployed under one host or separate hosts sharing cookie/data protection configuration.
- Whether Google-created users need username completion before creator activation or donation comments.
