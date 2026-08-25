# Business Rule Engine

**Phase:** 13 · Business rules are kept **separate** from the Workflow
engine (Phase 12) on purpose — they solve a different problem with different
runtime guarantees.

## Definition

A **Business Rule** is synchronous, record-scoped server logic that runs
within the same request/transaction as a table write: `Before Insert`,
`Before Update`, `After Update`, `Before Delete`. It must complete fast
(target: low single-digit milliseconds) and must never call out to an
external system synchronously.

## Rule categories

| Category | Example | Where enforced |
|---|---|---|
| **Validation rules** | Change window start must be before end; Incident priority requires impact+urgency set | Before Insert/Update, throws a domain validation error (→ HTTP 400/`ProblemDetails`) |
| **Authorization rules** | (Delegates to the ACL engine — a Business Rule does not reimplement authorization, it may add record-specific *conditions* the ACL engine evaluates) | Referenced from ACL condition expressions, not duplicated here |
| **Routing rules** | Assign an Incident to a group based on category + CI | Before Insert (defaulting) |
| **SLA rules** | Attach the correct SLA Definition to a new Task based on Service Offering | After Insert — triggers SLA Engine instance creation |
| **Escalation rules** | (Time-based escalation is a Workflow/Notification concern, not a Business Rule — see Phase 12) | N/A here |
| **Assignment rules** | Auto-balance load across group members | After Insert, synchronous lookup against current workload |
| **Notification rules** | "Notify watch list on state change" | After Update — **must** publish an event/enqueue, never call the Notification provider synchronously in-request |
| **Automation rules** | Auto-close a Resolved Incident after N days with no reopen | Scheduled job (`Host.Workers`), not a Business Rule — this is time-driven, not record-write-driven, so it belongs to Workflow/scheduling, not this engine |

## Decision: application code vs. business rule vs. workflow vs. event handler vs. background job

| Need | Use |
|---|---|
| Logic must run synchronously, in-transaction, on every write to a table, and is simple/deterministic | **Business Rule** |
| Logic requires multiple steps, may wait on a human or external system, may span minutes/hours/days | **Workflow** |
| Logic reacts to something happening but doesn't need to block the original write's transaction | **Event handler** (subscribes to a domain event, runs after commit) |
| Logic runs on a schedule or in response to a queue message, independent of any single record write | **Background job** (`Host.Workers`) |
| Logic is truly one-off, deeply specific to a single module's internal invariant, with no reuse value | **Application code** (a handler/service method) — the default when none of the above patterns fit, not a dumping ground for everything |

**Anti-pattern this table exists to prevent:** using Business Rules for
things that actually need Workflow (blocking a request thread on a
multi-minute external call), or using Workflow for things that need a
Business Rule (paying orchestration overhead for a single synchronous
validation).

## Where rules live and how they're edited

- Business Rules are **metadata-registered, code-implemented**: a rule is a
  registered `.NET` class implementing `IBusinessRule<TEntity>`, associated
  with a table + trigger phase via configuration — not free-form scripting
  (unlike ServiceNow's embedded script engine, which we deliberately do not
  replicate, since arbitrary tenant-authored server-side scripting is a
  large, ongoing security-review burden the Master AI Command's Rule 2/3
  argues against copying wholesale).
- Tenant-specific rule **parameters** (thresholds, routing targets) are
  configuration/metadata; rule **logic** ships as code, reviewed and
  versioned like any other code change — this is a deliberate simplification
  vs. ServiceNow's fully tenant-scriptable model, trading some low-code
  flexibility for a materially smaller security surface (no arbitrary
  customer-authored server-side script execution).
- `REQUIRES_VALIDATION` / future consideration: a constrained, sandboxed
  scripting option (e.g. a restricted expression language, not general
  JavaScript) for validation/condition rules only, if low-code demand proves
  strong post-MVP — explicitly deferred, not designed now.

## Execution guarantees

1. A Business Rule that throws **aborts the transaction** — the record is
   not persisted in a partially-valid state.
2. A Business Rule must not perform I/O to another bounded context's
   database, another module's tables outside its declared read set, or any
   external network call — those needs indicate the logic belongs in a
   Workflow Action or an async event handler instead.
3. Execution order across multiple rules on the same table/phase is
   explicit (a declared `Order` value), never registration-order-dependent.

---

*Next: `docs/integration/event-driven-architecture.md` (Phase 14).*
