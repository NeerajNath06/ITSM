# ADR-020 — API Gateway

**Status:** Superseded (product) by `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`;
role/reasoning below unchanged · **Full rationale:** `docs/infrastructure/self-hosted-production-architecture.md`,
`docs/api/api-design-rules.md` (Phase 16)

> **Amendment:** **Caddy** or **Nginx** (edge TLS termination/reverse
> proxy) in front of **YARP** (.NET-native reverse proxy handling
> per-tenant rate limiting, routing, and versioning at the application
> tier) replaces Azure Front Door + API Management. No WAF-as-a-managed-
> service is assumed by default under the self-hosted stack; a
> self-hostable WAF (e.g. a ModSecurity-based Nginx module) is evaluated
> separately if/when the threat model requires it beyond what Caddy/Nginx +
> YARP + the application-layer defenses in `docs/security/security-architecture.md`
> already provide. A partner developer portal is served as a static
> OpenAPI/Swagger UI + published Postman collection rather than a managed
> APIM developer portal.

## Decision
Caddy or Nginx (TLS termination, edge reverse proxy) in front of YARP
(rate limiting, per-tenant quotas, routing, versioning/deprecation
enforcement at the .NET tier) in front of `Host.Api`.

## Why
Centralizes cross-cutting API concerns (rate limiting, routing,
versioning) outside application handler code, self-hostable per `ADR-022`,
and keeps the rate-limiting/routing logic in a .NET-native proxy (YARP)
the team can extend in the same language as the rest of the platform.

## Trade-offs
The team now operates and secures the gateway layer itself (TLS cert
management via Caddy's automatic HTTPS or an operated Nginx + cert
pipeline, YARP configuration/scaling) instead of consuming a managed
edge/gateway service — accepted as part of the broader self-hosting
trade-off `ADR-022` documents; a dedicated WAF is a follow-up evaluation,
not assumed included by default.

## Consequences
`Host.Api` trusts that authentication/rate-limiting has already been
partially enforced upstream but **still independently re-validates JWT and
ACL** on every request (defense in depth, Phase 17) — the gateway is not
treated as a substitute for in-app authorization.

## Rejected alternative
No gateway layer, `Host.Api` directly internet-facing — rejected: no
central point for WAF, per-tenant rate limiting, or partner API versioning
enforcement.
