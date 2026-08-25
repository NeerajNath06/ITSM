# ADR-006 — CQRS

**Status:** Accepted · **Full rationale:** `docs/backend/dotnet-architecture.md` (Phase 31)

## Context
Application-layer structure needed a consistent pattern for handling writes
vs. reads across every module.

## Decision
CQRS at the **application layer** (separate Command/Query handler types via
MediatR), **not** separate physical read/write databases at our current
scale.

## Why
Separating write intent (commands, validated, state-changing) from read
intent (queries, often projection-shaped, no side effects) keeps handlers
single-purpose and makes the FluentValidation pipeline behavior apply
cleanly only where it's meaningful (commands).

## Trade-offs
Adds a small amount of ceremony (a Command/Query type + handler per
operation) versus a simpler service-method style; accepted because it keeps
`Application` layer code consistent and testable across every module.

## Consequences
Every module's `Application` layer follows the same
Command/Query/Handler/Validator vertical-slice shape (Phase 31).

## Rejected alternative
Full CQRS with separate read-model databases/event sourcing — rejected as
unjustified complexity at current scale; may be revisited per-module (e.g.
Reporting) if read-side scaling pressure appears (Phase 6's "read replica
before a service" guidance applies here too).
