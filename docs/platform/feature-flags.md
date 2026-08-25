# Feature Flags

**Phase:** 29 · **OpenFeature** (vendor-neutral SDK/API) + **Unleash**
(self-hosted flag server), per
`docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`.

## Flag scopes

| Scope | Use case | Example |
|---|---|---|
| **Global** | Kill switch for a platform-wide capability | Disable AI features platform-wide during an incident |
| **Environment** | Enable a capability in `staging` before `prod` | New Workflow activity type, tested in staging first |
| **Tenant** | Gradual rollout / entitlement-gated capability | Enable Major Incident workspace only for tenants on the relevant plan |
| **User / role** | Internal dogfooding, beta cohorts | Enable a new Agent Workspace layout for `beta_tester` role only |

## Design

- Backed by **Unleash**, accessed through the **OpenFeature** SDK/API in
  both `Host.Api` (.NET OpenFeature provider) and the React apps
  (JavaScript OpenFeature provider) — not a hand-rolled database table, to
  get built-in targeting filters (percentage rollout, targeting groups)
  without reimplementing them. OpenFeature's vendor-neutral API means
  Unleash itself is swappable behind the same abstraction other providers
  in this program use (`IModelProvider`, `INotificationProvider`,
  `ISearchProvider`), consistent with the whole stack's provider-abstraction
  discipline.
- Evaluated server-side only for anything security- or data-shape-relevant
  (a flag must never be "on" in the React bundle but silently rejected by
  the API — the API's evaluation is authoritative, the client's is a UX
  hint at most).
- Every flag has a documented **owner and removal plan** at creation time —
  a flag with no planned removal date is a governance smell (tracked as
  technical debt per `docs/governance/architecture-governance.md`).

## Gradual rollout and kill switch

- **Gradual rollout**: percentage-based targeting increased over time,
  monitored against the observability dashboards (Phase 24) for error-rate
  regression before widening further.
- **Kill switch**: any flag protecting a capability with a plausible failure
  mode (a new integration, a new AI feature) must support instant global
  disable without a deploy — verified as part of that feature's launch
  checklist, not assumed to work.

## Interaction with Tenant Configuration (Phase 28)

Feature flags and Tenant Configuration can look similar (both vary by
tenant) but serve different purposes: **Tenant Configuration is a durable
business setting** the tenant controls (their SLA defaults, their branding);
**a Feature Flag is a temporary rollout/kill mechanism the platform team
controls**, expected to eventually either become permanently-on code (flag
removed) or permanently-off/deleted. A flag that's "always been on for
everyone for a year" should be retired, not left as permanent
infrastructure.

---

*Next: `docs/frontend/react-architecture.md` (Phase 30).*
