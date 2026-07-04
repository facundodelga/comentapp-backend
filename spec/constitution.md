# ComentApp Constitution

## Purpose

ComentApp helps a fan support a creator with money and a short public comment. Product value is the full loop: authenticate, choose creator, write comment, pay, verify payment, show confirmed comment to creator.

## Non Goals

- Subscriptions or memberships
- Content publishing or creator CMS
- Private messaging
- Payment providers other than Mercado Pago
- Multi-creator checkout in one transaction
- Advanced accounting, payouts, tax, or analytics
- Social network features beyond creator discovery and received comments

## Core Principles

### 1. Payment Truth Lives Server Side

Frontend success pages are feedback only. Confirmed payment state must come from Mercado Pago server-side verification through webhook/notification or explicit backend verification.

### 2. Identity Comes From Session

Authenticated actions derive `userId` from the HTTP-only auth cookie claims. Client-provided `userId` is not trusted for ownership or write operations.

### 3. Creator Is A Role With State

A user can become a creator. Creator state must be visible through `GET /Authentication/me` as `isCreator`, and creator profile details must live behind creator endpoints.

### 4. Comments Are Payment-Bound

A donation comment can start as pending, but public/dashboard visibility depends on payment status. The current `Comment` model is not enough for full payment lifecycle; future work may need payment/preference/status fields or a separate donation/payment aggregate.

### 5. Small Contracts Beat Implicit Behavior

Each feature should define request/response DTOs in `spec/features/`. Controllers should not return ad hoc shapes once a contract is documented.

### 6. Preserve Auth Boundary

Authentication endpoints own registration, login, refresh, logout, email confirmation, and user hydration. Business endpoints own creators, donations, comments, payments, and realtime dashboard behavior.

## System Boundaries

### Auth API

Owns:

- Register
- Confirm email
- Google OAuth login/callback
- Login/logout
- Refresh session
- Current user profile
- Cookie/session mechanics

Does not own:

- Creator profiles
- Donation comments
- Mercado Pago payments
- Creator dashboard

### Business API

Owns:

- Creator activation/profile/search
- Donation comment intent
- Mercado Pago preference creation
- Mercado Pago callbacks/webhooks
- Confirmed comments
- Realtime creator dashboard

Must share compatible cookie auth with auth API before protected endpoints are added.

### Persistence

Owns:

- EF Core models
- DbContext configuration
- Repositories
- Migrations

Domain invariants that affect data integrity should be reflected in EF constraints where possible.

### Infrastructure

Owns:

- Email sending
- Template rendering
- JWT options/helpers
- Future external adapters such as Mercado Pago client wrappers

## Required Invariants

- Email is unique.
- One email maps to one user account across local and Google authentication.
- Username is unique when present.
- Creator name is unique.
- One user can own at most one creator profile.
- Comment text is required and max 300 characters.
- Donation amount must be positive.
- Mercado Pago account connection is required before creator can receive paid comments.
- Refresh tokens are rotated and revocable.

## Documentation Contract

Agents must keep these docs useful:

- Update `spec/features.md` when feature status or priority changes.
- Update specific files in `spec/features/` when endpoint contracts change.
- Update `AGENTS.md` when solution structure, workflow, or cross-cutting rules change.
