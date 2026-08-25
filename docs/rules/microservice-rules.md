# Microservice Rules

**Phase:** 35 · Binding rules for the small set of services that **are**
extracted per `docs/architecture/microservice-boundary-rules.md` (Notification,
Search indexer, AI inference, and any future justified extraction). These
rules do not apply to modules inside the `Host.Api` modular monolith — those
follow `docs/backend/dotnet-architecture.md` module rules instead.

## Service ownership

- Each extracted service has **one owning team** (even if currently the
  same team owns several services) and one on-call rotation responsible for
  it — a service with no clear owner should not exist.
- A service's public contract (API/event schema) is owned exclusively by
  that service's team — no other team modifies it directly.

## Database ownership

- Each extracted service owns its **own PostgreSQL database**, per
  `docs/database/database-architecture.md`. No other service or the core
  connects to it directly, ever — not "just this once for a report."
- Cross-service data needs are satisfied by: (a) calling the owning
  service's API, or (b) consuming its published events and maintaining a
  local denormalized read model.

## Contracts

- REST (external-facing) or gRPC (internal, per the Phase 15 checklist)
  contracts are versioned, documented (OpenAPI/`.proto`), and changed via a
  deprecation window — never a breaking in-place change.
- Event contracts (integration events, Phase 14) are versioned separately
  from internal domain event shapes — a service's internal refactor must
  never break a subscriber.

## Events

- Every extracted service both publishes (its own outcomes) and consumes
  (via a durable queue with an inbox for idempotency) — no extracted
  service polls another service's database or API in a tight loop as a
  substitute for subscribing to events.

## Retries, timeouts, circuit breakers

- Every outbound call from one service to another (or to an external
  provider) has an explicit timeout — no unbounded waits.
- Retries are bounded with exponential backoff (matches
  `event-driven-architecture.md`'s consumer retry rule) — never infinite.
- A **circuit breaker** (e.g. via `Microsoft.Extensions.Http.Resilience` or
  Polly) wraps calls to any service/provider with a history of transient
  failure, to avoid cascading latency into the caller when the callee is
  degraded — required for Notification's external provider calls at
  minimum.

## Observability

- Every extracted service emits OpenTelemetry traces/metrics/logs
  identically to the core (`docs/observability/observability.md`) — an
  extracted service that can't be traced end-to-end with the rest of the
  system defeats the purpose of unified observability.
- Every extracted service exposes `/healthz/live` and `/healthz/ready`.

## Deployment and versioning

- Independently deployable — an extracted service's release does not
  require a `Host.Api` deploy, and vice versa, **as long as its contract
  version is unchanged**. A contract-breaking change requires coordinated
  rollout (new contract version supported alongside the old one during
  transition).
- Each service has its own CI/CD pipeline (`docs/devops/cicd.md`), but
  follows the same quality gates (Phase 54) as the core.

## What disqualifies something from being a microservice

If a proposed extraction cannot name **which specific criterion** from
`docs/architecture/microservice-boundary-rules.md`'s evaluation table
justifies it, it does not get extracted — it stays a module inside
`Host.Api`. This rule exists specifically to prevent slow, undocumented
drift toward a microservice-per-domain topology the architecture explicitly
rejected in Phase 6.

---

*Next: `docs/rules/security-rules.md` (Phase 36).*
