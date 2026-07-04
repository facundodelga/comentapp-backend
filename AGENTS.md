# ComentApp Agents Guide

## Mission

Build ComentApp as a focused creator-support app: authenticated users send a paid comment to one creator through Mercado Pago, and creators receive confirmed comments in near real time.

Do not turn ComentApp into a subscription platform, content CMS, private messaging app, generic payment processor, multi-creator marketplace per transaction, or advanced finance dashboard.

## Solution Shape

- Solution file: `comentapp-backend.slnx`
- Auth API: `comentapp.authentication.manager`
- Auth business logic: `comentapp.authentication.businessLogic`
- Business API: `comentapp.business.endpoint`
- Shared persistence: `comentapp.persistence`
- Infrastructure: `comentapp.infrastructure`

Keep auth behavior in auth projects, business workflows in business endpoint/service layers, EF models and repositories in persistence, and external adapters in infrastructure.

## Current Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server LocalDB by default
- Autofac for DI
- AutoMapper
- Cookie authentication with HTTP-only cookies
- External cookie prepared for Google OAuth
- Refresh tokens persisted in DB
- DataProtection keys persisted in DB
- MailKit SMTP email
- Swagger in development

## Product Sources

Read these before changing product behavior:

- `spec/constitution.md`
- `spec/features.md`
- Feature specs in `spec/features/`
- Existing context docs: `features.md` and `backend.md`

If docs and code disagree, treat code as current state and specs as intended direction. Update specs when changing intended behavior.

## Architecture Rules

- Controllers stay thin: validate request shape, call services/use cases, map HTTP responses.
- Do not read user identity from request body when auth cookie can provide it.
- Use `ClaimTypes.NameIdentifier` for current user id.
- Keep DTOs explicit. Do not expose EF entities directly from public APIs.
- Keep persistence changes behind repositories or a clear DbContext access pattern already used by the project.
- Add EF migrations for model/table changes.
- Prefer one endpoint contract per feature and document it in `spec/features/`.
- Do not put real secrets in `appsettings.json`; use user secrets or environment variables for real environments.

## Auth Rules

- Session uses HTTP-only cookies.
- Login requires confirmed email.
- Google login is planned as a first-class auth provider, not a separate user system.
- Refresh token rotation must remain server-side.
- Logout must revoke refresh token and clear cookies.
- `GET /Authentication/me` is the canonical user hydration endpoint.
- Target `me` response must include `id`, `name`, `surname`, `userName`, `email`, and `isCreator`.
- Google OAuth must end by issuing the same app session cookies as local login.

## Business Rules

- Only authenticated users can create donation comments.
- Comment text limit is 300 characters.
- Donation amount must be greater than zero.
- One donation comment targets exactly one creator.
- A visible creator comment should only become confirmed after server-side payment verification.
- Frontend return URLs from Mercado Pago are not trusted as payment proof.
- Creator owner operations must use session user id, not user id from body.

## Known Gaps

- `GET /Authentication/me` returns only `name` and `email`.
- Google OAuth is partially scaffolded but not implemented end to end.
- `Creator` and `Comment` are mapped, but public `DbSet<Creator>` and `DbSet<Comment>` are missing.
- `CommentsController.POST` only echoes the comment.
- Business endpoint authentication setup is incomplete/commented.
- No creator endpoints exist yet.
- No Mercado Pago connect/preference/webhook flow exists yet.
- No SignalR dashboard exists yet.
- No backend test project exists in the solution.

## Development Workflow

1. Read relevant spec before coding.
2. Inspect current implementation before deciding shape.
3. Keep edits scoped to requested feature.
4. Update docs/specs if behavior changes.
5. Build solution with `dotnet build comentapp-backend.slnx`.
6. Add or update tests when a test project exists, or note test gap clearly.

## Naming

- Public API paths currently use controller names like `/Authentication` and `/Comments`.
- Prefer stable English DTO/property names: `amount`, `comment`, `creatorId`.
- Avoid mixing `price`, `amount`, and `monto` in new contracts.
- Keep class and namespace style consistent with nearby code.

## Multiagents

For tasks that cross frontend/backend, API contracts, auth, payments, persistence, or verification-heavy work, follow the root playbook:

- `../AGENTS.md`
- `../docs/multiagent-workflow.md`

In that flow, this project is the Backend Implementer area. A backend subagent must also read `backend.md`, `features.md`, and the affected feature spec before editing code.
