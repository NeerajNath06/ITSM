# Microservice vs. Modular Monolith — Boundary Rules

**Phase:** 6 · The single most consequential decision in this program. Per
Master AI Command Rule 1: **do not automatically create a microservice for
every bounded context.**

## Evaluation criteria

Every context from `bounded-contexts.md` is evaluated against:

| Criterion | Question |
|---|---|
| Business ownership | Does a distinct team/business unit own this independently? |
| Data ownership | Does it have genuinely independent data, or does it constantly join with Task/CMDB? |
| Deployment independence | Does it need to ship on its own schedule, faster/slower than the core? |
| Scaling requirements | Does its load profile differ enough from the core to need independent scaling? |
| Failure isolation | Should its failure NOT take down the core (or vice versa)? |
| Security isolation | Does it handle data sensitive enough to warrant a hard network/process boundary? |
| Team ownership | Is there (or will there be) a dedicated team whose Conway's-Law boundary matches this context? |
| Change frequency | Does it change at a very different cadence than the core (e.g. daily vs. quarterly)? |
| Transaction requirements | Does it need strong consistency/joins with Task or CMDB in the same transaction? |
| Communication requirements | Is it naturally synchronous-and-joined, or naturally async-and-eventual? |

**Default answer unless multiple criteria say otherwise: modular monolith
module.** A "yes" on one criterion (e.g. "it could scale independently") is
not sufficient justification — extraction cost (network hop, eventual
consistency, deployment/versioning overhead, distributed tracing burden) has
to be paid back by a real, present pressure, not a hypothetical future one
(Master AI Command Rule 4).

## Decision matrix

| Domain | Modular Monolith | Microservice | Decision | Reason |
|---|---|---|---|---|
| Task (base) | ✅ | | **Modular monolith** | Everything joins against it; extraction would force every other context into distributed transactions for the most common operation in the system |
| Incident / Problem / Change / Request | ✅ | | **Modular monolith** | Constant joins with Task, CMDB, SLA in single UI views (agent record page); no independent team/deploy cadence today |
| CMDB | ✅ (write path may extract later) | | **Modular monolith**, with a named extraction trigger | Impact queries (Incident↔CI, Change↔CI) need low-latency joins today; **extract the write/ingest path only if Discovery throughput saturates the API host** (mirrors the original guide's §4.4 trigger) |
| Discovery ingest | | ✅ (from day it exists) | **Satellite service** | Must run at/near customer network edge (MID-Server-analog), bursty and asynchronous by nature, failure must not affect the API host |
| Identity | ✅ | | **Modular monolith kernel** | Every request depends on it synchronously; extracting it would put a network hop on the critical path of literally everything |
| Authorization (ACL engine) | ✅ | | **Modular monolith kernel** | Same reasoning as Identity — must be in-process for acceptable latency on every table access |
| Workflow engine | ✅ (definition authoring/metadata stays in-process) | Execution runtime | **Modular monolith core (authoring) + Temporal (durable execution)** | Definition editing/graph evaluation context stays in-process (needs table context); durability, retries, timers, and signals are Temporal's job per `docs/adr/ADR-023-temporal-for-workflow-execution.md` — superseded from the original `Host.Workers`-based design |
| Notification delivery | | ✅ | **Satellite service from day one** | Provider-isolation (SMS/voice/email vendor outages must not affect anything else), independent scaling under alert storms, already named as an extraction candidate |
| Audit | ✅ (write path) | | **Modular monolith kernel** | Must commit in the same transaction as the business change it records — cannot be a network call away without risking un-audited writes |
| Search indexing | | ✅ | **Satellite service** | Read-optimized, rebuildable, naturally async (eventual consistency is acceptable and expected) |
| SLA engine | ✅ (evaluation), workers for timers | | **Modular monolith + satellite workers** | Same split as Workflow — evaluation needs table context, timers need durability independent of API process lifecycle |
| Service Catalog | ✅ | | **Modular monolith** | Tightly coupled to Request fulfillment workflow; low independent scaling need |
| Knowledge | ✅ | | **Modular monolith** | Constant joins with Task (attach-to) and Search; not high enough write volume to justify isolation |
| AI inference | | ✅ (when built, Phase 46/47) | **Satellite service** | GPU/rate-limit isolation, distinct deployment/versioning cadence (model updates independent of app releases) |
| Integration fabric (inbound/outbound) | ✅ (core), workers for delivery | | **Modular monolith + satellite workers** | Definition/ACL stays in-process; outbound delivery (retries, backoff) runs in workers to isolate slow/failing external endpoints from the API host |
| Reporting/analytics | ✅ (query layer) | | **Modular monolith read path**, consider a read replica before a service | Read-heavy but not independent business logic — scale via PostgreSQL read replicas first, not a new service |
| Asset (ITAM) | ✅ | | **Modular monolith** | Shares team/cadence with core ITSM in year 1; revisit only if ITAM becomes a separately staffed product line |
| Security Incident (SecOps-style, future) | | Likely ✅ if built | **Would be a separate service/module with a hard security boundary**, not just an ACL domain, if ever built | Data sensitivity and access model diverge enough from general Incident that even the *modular-monolith module* boundary should be a hard one; full network isolation only if a dedicated security team owns it |

## Extraction triggers (when a "modular monolith module" is later allowed to become a service)

An extraction is justified **only** when at least one of these is true, with
evidence (a metric, not a guess):

1. **Measured throughput saturation** on the API host attributable to one
   module (e.g. Discovery/CMDB write path).
2. **Measured provider-isolation need** — one module's external dependency
   failures are degrading unrelated modules (justifies Notification,
   Search, AI from day one because this is *known* in advance, not
   hypothetical).
3. **A distinct team forms** with its own release cadence and the module's
   change frequency genuinely diverges from the core (Conway's Law
   alignment, not organizational chart aspiration).
4. **A hard security boundary is mandated** by compliance/customer
   requirement that a shared-process module cannot satisfy (e.g. a future
   regulated Security Operations product).

**Never** extract for: "it feels like it should be its own service,"
"microservices are best practice," or "this domain has a clean name."

## What we do NOT extract by default

Incident, Problem, Change, Request, Task, CMDB (read/query path), Identity,
ACL, Audit, Workflow evaluation, SLA evaluation, Service Catalog, Knowledge,
Asset — all stay in the modular monolith (`Host.Api`). This mirrors what
Phase 3 found ServiceNow itself does: one platform, application-scoped
modules, not a service mesh.

---

*Next: `docs/database/database-architecture.md` (Phase 7) — how this
decision shapes the PostgreSQL data layer.*
