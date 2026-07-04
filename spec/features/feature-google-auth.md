# Feature: Google Authentication

## Goal

Users can authenticate to ComentApp with Google, then receive the same HTTP-only app session used by local login.

## Current State

Status: `partial`.

Implemented foundation:

- `Microsoft.AspNetCore.Authentication.Google` package is referenced by auth endpoint project.
- `ExternalCookie` scheme is configured for OAuth handoff.
- `GoogleAuthProvider` type exists.
- `LoginDTO` and `AuthTokens` include provider-oriented fields.
- `CookieService` stores `auth_provider` claim and clears external cookie.
- `comentapp.authentication.manager/Security/GOOGLE_OAUTH_TEMPLATE.cs` documents intended setup.

Missing:

- Active `.AddGoogle(...)` registration in `Program.cs`.
- `Google:ClientId` and `Google:ClientSecret` configuration through secrets/env vars.
- Controller endpoint to start Google challenge.
- Controller callback endpoint.
- User lookup/create/link flow from Google identity.
- Refresh token creation with `AuthProvider = "google"`.
- Frontend button and callback handling.

## Proposed Endpoints

### GET /Authentication/google-login

Starts Google OAuth challenge.

Query:

- `returnUrl` optional frontend-relative URL.

Behavior:

- Builds authentication properties.
- Sets callback to `/Authentication/google-callback`.
- Uses Google challenge scheme.

### GET /Authentication/google-callback

Handles Google callback.

Behavior:

1. Reads external principal from `ExternalCookie`.
2. Requires verified email from Google.
3. Finds existing user by email or creates a new user.
4. Marks Google-created users as email confirmed.
5. Issues normal app cookies through `ICookieService.SetAuthCookies`.
6. Clears external cookie.
7. Redirects to allowed frontend return URL.

## Target User Linking Rule

Email is the account key.

- If Google email matches existing local user, link/authenticate same user.
- If no user exists, create user from Google claims.
- Never create duplicate users for the same email.
- Do not require password for Google-created accounts.

## Target Session Contract

Google login must produce the same session as local login:

- App cookie: `__Host-app_session`
- Refresh token cookie managed by current cookie service behavior
- Claims include `NameIdentifier`, `Name`, `Email`, `auth_provider = "google"`
- `GET /Authentication/me` returns the same shape as local login

## Security Rules

- Do not commit Google client secret.
- Use user secrets or environment variables for real credentials.
- Validate/limit `returnUrl` to known frontend origin or relative paths.
- Require Google email claim.
- Prefer verified Google email when available.
- Clear `ExternalCookie` after callback success or failure.
- Do not expose Google tokens to frontend.

## Acceptance Criteria

- User can click Google login and complete OAuth flow.
- Existing local account with same email logs into same user id.
- New Google user is created once and email is confirmed.
- App session cookies are issued exactly like local login.
- `GET /Authentication/me` works after Google login.
- Logout clears app and external cookies.

