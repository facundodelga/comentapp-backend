# Feature: Auth Business Proxy

## Goal

The authentication host exposes a YARP reverse proxy that forwards business API traffic to the business microservice at `https://localhost:7113`.

This lets the frontend talk to a single origin during local development while keeping authentication and business logic separated.

## Current State

Status: `planned`.

Implemented foundation:

- Auth and business are separate ASP.NET Core hosts.
- The business host already exposes controller routes such as `/Comments`.
- Auth uses HTTP-only cookie sessions and is the expected entrypoint for the frontend.

Missing:

- YARP package and proxy configuration in the auth host.
- Route mapping from the auth host to the business host.
- Forwarding rules for cookies, headers, query string, and request bodies.
- Local development configuration for the business destination.

## Target Contract

The auth host forwards business routes unchanged to `https://localhost:7113`.

Example target behavior:

- `GET /Comments` on the auth host reaches `GET /Comments` on the business host.
- `POST /Comments` on the auth host reaches `POST /Comments` on the business host.
- Future business endpoints such as creator, donation, Mercado Pago, and dashboard routes use the same proxy boundary.

## Rules

- The proxy must preserve the incoming HTTP method, path, query string, and body.
- The proxy must forward auth cookies so the business host can validate the same session.
- The proxy must not invent or transform business responses beyond standard reverse-proxy behavior.
- Auth routes such as `/Authentication/*` stay owned by the auth host and must not be proxied away.
- The business host remains the source of truth for business workflows.
- Local destination configuration should point to `https://localhost:7113` by default in development.

## Acceptance Criteria

- Frontend calls to business routes can go through the auth host.
- Requests arrive at the business host without losing cookies or payload data.
- Auth endpoints continue to work normally on the auth host.
- Business host can be swapped or moved later by changing proxy configuration only.