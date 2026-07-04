# Feature: Authentication And Session

## Goal

Users can register, confirm email, login, refresh session, logout, and hydrate the current user from an HTTP-only cookie session.

Google login is tracked separately in `spec/features/feature-google-auth.md`, but must produce the same app session shape.

## Current State

Status: `partial`.

Implemented:

- `POST /Authentication/register`
- `POST /Authentication/confirm-email`
- `POST /Authentication/login`
- `POST /Authentication/refresh`
- `POST /Authentication/logout`
- `GET /Authentication/me`
- Cookie auth with refresh token persistence
- External cookie scaffold for OAuth handoff
- Email confirmation through SMTP template

Gap:

- `GET /Authentication/me` does not return full user contract.
- Creator role is not included.
- Google OAuth flow is not wired into controller endpoints yet.
- Some async cookie service methods use `async void`.

## Target Contract

### GET /Authentication/me

Requires auth cookie.

Response:

```json
{
  "id": 1,
  "name": "Ada",
  "surname": "Lovelace",
  "userName": "ada",
  "email": "ada@example.com",
  "isCreator": false
}
```

## Rules

- Login fails until email is confirmed.
- Local login uses email/password; Google login uses verified Google identity.
- Refresh token comes from cookie, not body.
- Logout revokes current refresh token when present.
- Current user id comes from `ClaimTypes.NameIdentifier`.
- `isCreator` should reflect existence/active state of creator profile owned by current user.

## Acceptance Criteria

- Authenticated `me` returns full target contract.
- Unauthenticated `me` returns 401.
- Frontend can hydrate route guards from one `me` call.
- Existing login/refresh/logout behavior remains compatible.
- Local and Google sessions hydrate through the same `me` endpoint.
