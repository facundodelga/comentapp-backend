# Feature: Creator Activation And Profile

## Goal

An authenticated user can create and maintain one creator profile. Other users can discover public creator profiles.

## Current State

Status: `planned`.

Implemented foundation:

- `Creator` EF model.
- `CreateCreator` migration.
- Unique indexes on `UserId` and `CreatorName`.

Missing:

- Public `DbSet<Creator>`.
- Creator repository/service.
- Creator endpoints.
- Frontend contract alignment.

## Proposed Endpoints

### POST /Creators

Creates creator profile for current user.

Request:

```json
{
  "creatorName": "ada",
  "description": "Math and computing streams",
  "instagramLink": null,
  "tikTokLink": null,
  "youTubeLink": null,
  "twitchLink": null,
  "kickLink": null
}
```

Response: `201 Created`.

```json
{
  "id": 10,
  "creatorName": "ada",
  "userId": 1,
  "description": "Math and computing streams",
  "mercadoPagoConnected": false
}
```

### GET /Creators/me

Returns creator profile for current user, or 404 if user is not creator.

### PATCH /Creators/me

Updates editable profile fields for current user.

### GET /Creators

Public creator search.

Query:

- `query`
- `page`
- `pageSize`

## Rules

- One user owns at most one creator profile.
- `creatorName` is unique.
- User id comes from auth session.
- Public search returns only public fields.
- Mercado Pago connection state should not expose tokens/secrets.

## Acceptance Criteria

- User can create creator profile once.
- Duplicate creator name returns conflict.
- `GET /Authentication/me` returns `isCreator: true` after activation.
- Public search supports basic query and pagination.

