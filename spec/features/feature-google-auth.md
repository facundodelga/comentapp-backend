# Feature: Google Authentication

## Goal

Users can authenticate to ComentApp with Google, then receive the same HTTP-only app session used by local login.

## Current State

Status: `done` (end-to-end flow implemented; requires real `Google:ClientId`/`Google:ClientSecret` to run against Google).

Implemented:

- `Microsoft.AspNetCore.Authentication.Google` package referenced by auth endpoint project.
- Active `.AddGoogle(...)` registration in `Program.cs`, with `SignInScheme = "ExternalCookie"` (external principal is held temporarily in `ExternalCookie`, never signed directly into `AppCookie`).
- `GET /Authentication/google-login` starts the Google challenge with `RedirectUri` pointing to `/Authentication/google-callback`.
- Google redirects to `/signin-google`; the OAuth middleware exchanges the code, signs the external principal into `ExternalCookie`, then redirects to `/Authentication/google-callback`.
- `GET /Authentication/google-callback` reads the external principal, delegates to `GoogleAuthProvider` (via `IAuthProviderFactory.GetProvider("google")`), sets the app session cookies through `ICookieService.SetAuthCookies`, clears `ExternalCookie`, and redirects to a validated frontend return URL.
- `GoogleAuthProvider` (implements `IAuthProvider`) calls `IUserService.FindOrCreateGoogleUserAsync` and issues the same `AuthTokens` shape as `LocalAuthProvider`, tagged with `AuthProvider = "google"`.
- `IUserService.FindOrCreateGoogleUserAsync`: looks up the user by email; if found, marks `IsEmailConfirmed = true` when it wasn't already (Google's email is trusted as verified); if not found, creates a new `User` with `IsEmailConfirmed = true`, no `UserName`, and an unusable random password hash (Google is the only sign-in method for these accounts).
- `returnUrl` is validated to be a relative path (must start with `/`, reject `//` and absolute URLs) before being appended to `Frontend:BaseUrl`; anything else falls back to `/`.
- `Google:ClientId`/`Google:ClientSecret` read from configuration (empty by default in `appsettings.json`; must be supplied via user secrets/environment variables for real environments).

Remaining before production use:

- Populate real `Google:ClientId`/`Google:ClientSecret` via user secrets or environment variables (not committed).
- Register `https://<auth-host>/signin-google` as the authorized redirect URI in Google Cloud Console (middleware `CallbackPath`; not `/Authentication/google-callback`).

## Proposed Endpoints

### GET /Authentication/google-login

Starts Google OAuth challenge.

Query:

- `returnUrl` optional frontend-relative URL.

Behavior:

- Builds authentication properties.
- Sets `RedirectUri` to `/Authentication/google-callback` (with optional `returnUrl` query param) so the OAuth middleware redirects there after processing Google's response at `/signin-google`.
- Uses Google challenge scheme.

Google Cloud Console must register `https://<auth-host>/signin-google` as the authorized redirect URI (middleware `CallbackPath`, not the controller route).

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

