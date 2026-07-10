# ComentApp Features

## Status Legend

- `done`: implemented and usable.
- `partial`: usable foundation exists, but missing required behavior.
- `planned`: documented intent, no real implementation yet.

## Feature Map

| Feature | Status | Spec |
| --- | --- | --- |
| Authentication and session | done | `spec/features/feature-auth.md` |
| Auth business proxy | planned | `spec/features/feature-auth-business-proxy.md` |
| Google authentication | done | `spec/features/feature-google-auth.md` |
| Creator activation/profile | planned | `spec/features/feature-creators.md` |
| Donation comment checkout | planned | `spec/features/feature-comments-donations.md` |
| Mercado Pago connect | planned | `spec/features/feature-mercadopago-connect.md` |
| Mercado Pago callbacks | planned | `spec/features/feature-mercadopago-callbacks.md` |
| Creator realtime dashboard | planned | `spec/features/feature-dashboard-realtime.md` |

## Current Implementation Snapshot

- Registration exists with duplicate email/username checks, password hashing, and confirmation email.
- Email confirmation exists.
- Login/logout/refresh exist with HTTP-only cookies and refresh token rotation.
- Google OAuth is fully wired (`google-login`/`google-callback`), issuing the same app session as local login; only needs real `Google:ClientId`/`Google:ClientSecret` to run against Google.
- `GET /Authentication/me` exists but returns only `name` and `email`.
- `Creator` EF model and migration exist, but no creator API exists.
- `Comment` EF model and migration exist, but `POST /Comments` only echoes request data.
- Mercado Pago integration is not implemented.
- SignalR realtime dashboard is not implemented.

## Recommended Delivery Order

1. Align `GET /Authentication/me` contract and frontend user shape.
2. Add creator API and `isCreator` calculation.
3. Implement creator settings/onboarding flow.
4. Implement Mercado Pago connect.
5. Implement donation comment preference creation.
6. Implement Mercado Pago webhook/callback verification.
7. Persist and expose confirmed comments.
8. Add creator realtime dashboard with SignalR.

## Cross-Feature Decisions Needed

- Whether creator activation can happen before Mercado Pago connection.
- Whether `Creator.MercadoPagoAccount` should be nullable while connection is pending.
- Whether donation/payment state lives on `Comment` or a separate `Donation`/`Payment` entity.
- Whether auth and business endpoints are deployed under one host or separate hosts sharing cookie/data protection configuration.
- Whether Google-created users need username completion before creator activation or donation comments.
