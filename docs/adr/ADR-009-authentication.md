# ADR-009 — Authentication

**Status:** Accepted · **Full rationale:** `docs/security/security-architecture.md` (Phase 17)

## Context
Need a standards-based authentication model supporting enterprise IdP
federation and API/service-to-service callers.

## Decision
OAuth2 / OIDC with Authorization Code + PKCE for React SPAs (via BFF
session cookie, not raw JWT to the browser); client-credentials grant for
service/integration identities; mTLS additionally for internal gRPC.

## Why
Standards-based federation with any enterprise IdP, brokered through the
self-hosted **Keycloak** instance (`ADR-022`) — Entra ID, Okta, or any
OIDC/SAML IdP a tenant already uses;
short-lived JWTs limit blast radius of a leaked token; BFF cookie pattern
keeps tokens out of browser JS entirely (XSS mitigation).

## Trade-offs
A dedicated platform token-exchange service adds a component versus
forwarding the IdP's token directly; accepted because it decouples our
token shape/lifetime from the IdP's and lets us revoke sessions
independently.

## Consequences
No API accepts a raw IdP token directly; every caller goes through the
platform token service or a service-identity client-credentials flow.

## Rejected alternative
Implicit OAuth2 flow — rejected as deprecated/insecure for SPAs; raw JWT in
`localStorage` — rejected due to XSS exposure.
