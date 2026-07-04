# Feature: Creator Realtime Dashboard

## Goal

Creators see newly approved donation comments in near real time.

## Current State

Status: `planned`.

No SignalR hub or dashboard API exists yet.

## Proposed Components

- SignalR hub for creator notifications.
- Query endpoint for initial dashboard list.
- Event emitted when payment becomes approved.

## Proposed Endpoint

### GET /Creators/me/comments

Requires creator auth.

Response:

```json
[
  {
    "id": 456,
    "comment": "Gran stream",
    "amount": 1500,
    "fromUserName": "ada",
    "createdAt": "2026-07-04T19:00:00Z",
    "paymentStatus": "approved",
    "isRead": false
  }
]
```

## Proposed Event

SignalR event name: `commentReceived`.

Payload matches single comment item from endpoint response.

## Rules

- Only creator owner can subscribe to their creator stream.
- Emit only after server-side approved payment.
- Initial page load should query existing confirmed comments before listening for new events.
- Reconnect handling belongs in frontend, but backend events must be safe to receive more than once.

## Acceptance Criteria

- Creator can fetch confirmed comments.
- Non-creator receives 403 or 404 for creator dashboard data.
- Approved payment triggers realtime event for creator owner.
- No pending/failed comment appears as confirmed.

