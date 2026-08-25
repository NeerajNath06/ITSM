# Anti-Patterns

**Phase:** 53 · Named, specific things this codebase must not do — each
with the failure it causes and the document that establishes the correct
alternative.

| Anti-pattern | What it looks like here | Failure it causes | Correct alternative |
|---|---|---|---|
| **God Service** | A single satellite service accreting unrelated responsibilities (e.g. Notification also handling Search indexing "since it's already a background service") | Loses the isolation benefit extraction was supposed to buy; couples unrelated failure domains | One responsibility per extracted service, per `docs/architecture/microservice-boundary-rules.md` |
| **God Controller** | A minimal-API endpoint group with business logic, branching, and direct EF Core calls inline | Untestable, unreviewable, violates layering | `Application`/`Domain` hold logic; endpoints map HTTP↔command only (`docs/backend/dotnet-architecture.md`) |
| **God Database** | Every module writing to a shared set of "convenience" tables outside its own ownership | Breaks the bounded-context ownership model; nobody can safely change a table's shape | Each table has exactly one owning context (`docs/architecture/bounded-contexts.md`) |
| **Shared Database Across Independent Services** | A satellite service querying the core PostgreSQL database directly "just for this one report" | Couples the satellite's schema to the core's, defeats independent deployability | API/event access only (`docs/rules/microservice-rules.md`) |
| **Distributed Monolith** | Extracted services that must always deploy together / share a database / call each other synchronously in a tight chain | All the network-call overhead of microservices, none of the independence benefit | Extraction only per the justified-trigger criteria (`docs/architecture/microservice-boundary-rules.md`); async event-driven coupling between extracted services |
| **Chatty Services** | A request handler making many sequential synchronous calls to another service/module to assemble a response | High latency, cascading failure risk | BFF-side aggregation (Phase 16) for UI needs; async/event-driven for cross-context effects (Phase 14) |
| **Circular Dependencies** | Module A's Application layer depends on Module B's, and B depends back on A | Impossible to reason about or extract independently | Enforced acyclic via architecture tests; shared needs go through `BuildingBlocks`/`Platform`, never a peer-to-peer cycle |
| **Anemic Domain Model (where inappropriate)** | `Incident` as a plain data bag with all logic living in a service class | Business rules scattered, invariants unenforced at the entity boundary | Domain methods enforce invariants (`incident.Resolve()`), per `docs/backend/dotnet-architecture.md` — used where the domain has real behavior, not forced onto trivial reference data |
| **Over-engineered Repository Pattern / Generic Repository everywhere** | `IRepository<T>` applied uniformly to every entity including trivial lookups | Indirection with no benefit, hides query intent | Direct `DbContext` use for simple CRUD; repositories only where they encapsulate real complexity (`docs/backend/dotnet-architecture.md`) |
| **Business logic in controllers** | See God Controller above | — | — |
| **Business logic in React** | A component computing SLA breach status, ACL-equivalent visibility, or approval routing client-side | Client-side logic is bypassable and drifts from server truth | React renders server-computed state; server is the only source of truth (`docs/frontend/react-architecture.md`) |
| **Service-to-service database access** | See Shared Database above | — | — |
| **Hard-coded configuration** | A tenant-specific value, environment URL, or threshold baked into source | Requires a code deploy to change an operational value | Configuration/Tenant Configuration/Feature Flag per the decision table in `docs/platform/configuration-management.md` |
| **Secrets in Git** | A connection string, API key, or certificate committed to the repository, even in a "test" fixture | Credential compromise, expensive rotation | OpenBao only, pre-commit/CI secret scanning (`docs/rules/security-rules.md`) |
| **Synchronous distributed transactions** | A handler trying to commit changes across the core database and a satellite's database "atomically" | Not actually possible reliably across a network boundary; produces silent inconsistency | Outbox/inbox + compensation (`docs/integration/event-driven-architecture.md`, `docs/workflow/workflow-engine-architecture.md`) |
| **Unlimited retries** | A consumer or HTTP call retrying forever on failure | Resource exhaustion, poison-message loops | Bounded retry + backoff + DLQ, everywhere (`docs/integration/event-driven-architecture.md`) |
| **Missing idempotency** | A mutating endpoint or event consumer that double-applies on retry/redelivery | Duplicate side effects (double-charged, double-notified) | `Idempotency-Key` on mutating POSTs; inbox pattern on consumers (`docs/api/api-design-rules.md`, Phase 14) |
| **Missing correlation IDs** | A request/event with no way to trace it across services | Debugging a production incident becomes guesswork | Correlation ID generated at origin, propagated everywhere (Phase 14/24) |
| **Missing audit** | A mutating operation with no audit trail | No forensic/compliance record; violates Rule 13 in `CLAUDE.md` | Shared `SaveChanges` interceptor (`docs/audit/audit-architecture.md`) |
| **Missing authorization** | Any endpoint or internal call that assumes its caller is already trusted | The single most severe class of bug this program exists to prevent | ACL check at every table/field access, no exceptions (`docs/security/security-architecture.md`) |
| **Duplicate master data** | A satellite service maintaining its own copy of `identity_user` fields it re-derives independently instead of consuming `UserRoleChanged`-style events | Data drifts out of sync silently | Denormalized snapshots kept current via domain events, explicitly, not independently re-derived (`docs/database/database-architecture.md` cross-boundary rules) |

## How this list is used

Referenced explicitly in `docs/rules/ai-agent-rules.md`'s review step and in
`CLAUDE.md` — a PR/change is checked against this table before being
considered done, and a reviewer citing a row here is sufficient
justification to request changes.

---

*Next: `docs/governance/definition-of-done.md` (Phase 54).*
