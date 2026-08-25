# Coding Rules

**Phase:** 32 · Strict, binding rules for all .NET and React code in this
repository. Violations are review-blocking, not stylistic suggestions.

## SOLID / DRY / KISS / YAGNI

- **Single Responsibility**: a handler does one command; a service has one
  reason to change. If a class needs "and" to describe its purpose, split it.
- **Open/Closed**: extend via new Workflow activities, Business Rules, or
  metadata — not by adding `if (tenantId == X)` branches into shared code.
- **Liskov/Interface Segregation**: `Task` subtypes must be substitutable
  wherever `TaskBase` is expected; interfaces are consumer-shaped, not
  one giant `IEverything`.
- **Dependency Inversion**: `Application` depends on interfaces it defines;
  `Infrastructure` implements them — never the reverse.
- **DRY**: shared logic (ACL evaluation, audit, notification, tenant
  resolution) lives in the platform kernel, called by every module — never
  copy-pasted per module (see `docs/rules/anti-patterns.md` — "duplicate
  master data" / "duplicate identity logic").
- **KISS**: the simplest design that satisfies today's actual requirement.
  A generic/configurable solution for a problem with one known instance is
  premature.
- **YAGNI**: do not build for a hypothetical Wave-N+3 requirement now. Three
  similar lines beat a premature abstraction.

## Separation of concerns

- Business logic lives in `Application` handlers and `Domain` entities —
  **never** in React components/hooks, and **never** in ASP.NET Core
  controllers/minimal-API endpoint lambdas (those map HTTP ↔ command/query
  only).
- Presentation logic (formatting, layout) never leaks into
  `Application`/`Domain`.

## Immutability

- Domain events, integration events, and command/query records are
  immutable (`record` types).
- Entities are mutable through explicit domain methods
  (`incident.Resolve(...)`), never through public property setters that let
  external code put an entity into an invalid state.

## Async, cancellation, nullability

- `async`/`await` throughout; no blocking calls (`.Result`, `.Wait()`,
  `Task.Run` used to fake async over sync code).
- `CancellationToken` accepted and propagated on every I/O-bound method.
- Nullable reference types **on**, warnings as errors, no `!` null-forgiving
  operator without a comment explaining the invariant that makes it safe.

## Naming

- C#: `PascalCase` types/members, `camelCase` locals/parameters,
  `IPrefixed` interfaces, async methods suffixed `Async`.
- React/TypeScript: `kebab-case` file names, `PascalCase` components,
  `camelCase` hooks/members (hooks additionally prefixed `use`), feature
  folders named for the domain concept, not the technical pattern
  (`incidents/`, not `services/incident-stuff/`).
- Domain vocabulary matches `docs/architecture/bounded-contexts.md` exactly
  — no synonyms drifting in ("Ticket" instead of "Task"/"Incident",
  "Company" instead of "Tenant") — ubiquitous language is enforced, not
  aspirational.

## Exception handling

- Domain/application exceptions are specific types (`NotFoundException`,
  `ValidationException`, `AuthorizationDeniedException`), mapped centrally
  to `ProblemDetails` (Phase 31) — never caught-and-swallowed, never a bare
  `catch (Exception)` without rethrow or explicit, justified handling.
- No exception is used for ordinary control flow (e.g. "not found" in a
  hot-path query should return null/Result, not throw, if the caller
  routinely handles absence).

## Logging

- Structured logging only (`ILogger` message templates with named
  parameters) — no string concatenation, no `Console.WriteLine`.
- Never log `Restricted`-classified data (`docs/governance/data-governance.md`)
  even at Debug level.
- Every log statement inside a request/command handler is automatically
  scoped with `TenantId`, `CorrelationId`, `TraceId` (Phase 24) — do not
  manually re-add these per call site.

## Testing (see full strategy in Phase 38)

- New logic ships with tests at the layer it lives in: `Domain` logic gets
  unit tests with no infrastructure dependencies; `Application` handlers
  get tests against in-memory/test doubles; cross-cutting behavior (ACL,
  tenancy) gets integration tests.
- A bug fix includes a regression test reproducing the bug before the fix,
  in the same PR.

## Performance

- No N+1 queries — EF Core `Include`/projection reviewed for any new list
  endpoint; `docs/rules/database-rules.md` has the query-specific detail.
- Pagination is mandatory on every list endpoint — no "return all rows"
  endpoint ships, even internally.

## Security (see full architecture in Phase 17; this is the code-level restatement)

- No secret, connection string, or key in source — ever, including test
  fixtures (use local `dev` placeholders sourced from environment/User
  Secrets, never committed).
- Every new table/entity handling tenant data gets a tenant query filter and
  an RLS policy in the **same PR** as its migration — not a follow-up task.
- Every new mutating endpoint gets an ACL policy and an audit entry in the
  **same PR**.

---

*Next: `docs/rules/database-rules.md` (Phase 33).*
