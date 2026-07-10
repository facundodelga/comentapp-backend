# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Product

ComentApp lets authenticated users send a **paid comment** to a single creator via Mercado Pago; creators receive confirmed comments in near real time. Keep scope narrow — it is not a subscription platform, CMS, messaging app, or multi-creator marketplace. Read `AGENTS.md` for the full mission, architecture rules, auth/business rules, and known gaps before changing product behavior.

## Commands

```bash
# Build the whole solution (solution file is .slnx, not .sln)
dotnet build comentapp-backend.slnx

# Run an API (from its project dir)
dotnet run --project comentapp.authentication.manager      # auth API
dotnet run --project comentapp.business.endpoint           # business API

# EF Core migrations — run from the persistence project.
# DesignTimeDbContextFactory drives design-time; migrations live in comentapp.persistence/Migrations.
dotnet ef migrations add <Name> --project comentapp.persistence
dotnet ef database update --project comentapp.persistence
```

There is **no test project** in the solution yet. When adding test-worthy behavior, note the test gap explicitly rather than inventing a test setup.

## Architecture

Five projects, layered:

- **comentapp.authentication.manager** — Auth Web API (controllers, `Program.cs`, cookie/OAuth wiring, AutoMapper profile). Thin controllers.
- **comentapp.authentication.businessLogic** — Auth services + auth providers (local/Google) via `AuthProviderFactory`. Registered by `AuthenticationBusinessModule`.
- **comentapp.business.endpoint** — Business Web API (comments/creators/payments). **Auth setup is currently commented out** in its `Program.cs` and it only registers `DatabaseModule` — most of this API is still a stub.
- **comentapp.persistence** — EF Core 10 `ComentappDbContext`, entity models, repositories, migrations. Targets SQL Server LocalDB (`DefaultConnection`).
- **comentapp.infrastructure** — External adapters exposed as Autofac modules: `EmailModule` (MailKit SMTP), `JwtModule`.

### Dependency injection

Uses **Autofac**, not the built-in container. `Program.cs` calls `UseServiceProviderFactory(new AutofacServiceProviderFactory())` and registers modules. Registration is **convention-based** via assembly scanning:
- Types ending in `Service` → `AsImplementedInterfaces` (in `AuthenticationBusinessModule`)
- Types ending in `Repository` → `AsImplementedInterfaces` (in `DatabaseModule`)
- `IAuthProvider` implementations are auto-collected

So a new service/repository is wired up just by matching the naming convention and interface — no manual registration needed.

### Authentication model

Multi-scheme cookie auth (see `comentapp.authentication.manager/Program.cs`):
- **`AppCookie`** (`__Host-app_session`, SameSite=Strict, 8h sliding) — the app session issued for both local and Google login. `AppCookieEvents.ValidatePrincipal` runs on every request.
- **`ExternalCookie`** (`__Host-external_session`, SameSite=Lax, 30min) — temporary holder for Google's result; the `google-callback` controller action then issues `AppCookie`.
- Google's middleware `CallbackPath` is the default `/signin-google` and is intentionally **different** from the controller action `/Authentication/google-callback` — the middleware intercepts `/signin-google` before routing, so they must not collide (see the detailed comment in `Program.cs`).
- API paths `/me` and `/session` return 401/403 instead of redirecting.
- DataProtection keys are persisted to the DB via `PersistKeysToDbContext<ComentappDbContext>`.

Read current user id from `ClaimTypes.NameIdentifier` (the auth cookie), never from the request body.

### Data model

`ComentappDbContext` (`comentapp.persistence/ComentappDbContext.cs`) owns: `User`, `Creator` (1:1 with User), `Comment`, `Payment`, `RefreshToken`, `Setting`, `DataProtectionKey`. Relationship config in `OnModelCreating`: `Comment`→`User`/`Creator` are `Restrict`; `Payment`→`Comment` is a 1:1 with cascade. A `Comment` should only become **confirmed after server-side payment verification** — do not trust Mercado Pago frontend return URLs as proof.

## Config & secrets

Local config in each API's `appsettings.json`: `DefaultConnection` (LocalDB `COMENTAPP`), `Frontend.BaseUrl` / `Cors:AllowedOrigins` (`http://localhost:5173`), `Email` (dev SMTP on `localhost:1025`), `Jwt`, `Google`. Real `Jwt:Secret`, `Google:ClientId/Secret`, and SMTP creds are blank in the repo — supply them via **user secrets or environment variables**, never commit real secrets.

## Specs

Product intent lives in `spec/constitution.md`, `spec/features.md`, and `spec/features/`. If code and specs disagree, treat **code as current state, specs as intended direction**, and update specs when you change intended behavior.
