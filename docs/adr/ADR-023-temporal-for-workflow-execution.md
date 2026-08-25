# ADR-023 — Temporal for Workflow Execution (Runtime), Metadata-Driven Graphs Preserved (Authoring)

**Status:** Accepted — a synthesis, not a straight replacement.
**Full context:** `docs/workflow/workflow-engine-architecture.md` (Phase 12),
`docs/adr/ADR-013-workflow-engine.md`, `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`.

## Context

`ADR-013` locked a **custom, data-defined workflow engine**: an admin
edits a versioned JSON activity graph (Trigger/Condition/Action/Approval/
Wait/Event/Branch/Subworkflow) in Admin Studio, and `Host.Workers` executes
it — the single most consistently repeated lesson from researching
ServiceNow (`docs/architecture/servicenow-reference-architecture.md`):
**workflow-as-data, not workflow-as-code**, so a tenant can change an
approval flow without an engineering release.

The user's OSS stack direction (`ADR-022`) names **Temporal** for workflow
orchestration. Temporal's actual model is **workflow-as-code**: a developer
writes a deterministic function (in a supported language SDK) that
Temporal durably executes via event-sourced replay, with retries, timers,
and signals handled by the Temporal server. This is not, by itself, a
data-edited JSON graph a tenant admin can change without a deploy — it is
the opposite authoring model from what `ADR-013` chose and reasoned about
at length.

**This is a genuine conflict, not a simple product substitution** (unlike
Redis→Valkey or Blob→MinIO in `ADR-022`, which are protocol-compatible
swaps with no model change). Per this program's own rule ("if an existing
implementation conflicts with the new architecture, document the conflict
first instead of silently rewriting it"), that conflict is resolved here
explicitly, not by quietly picking one side.

## Decision

**Use Temporal as the durable execution runtime. Keep the metadata-driven
graph as the authoring/editing model.** Concretely:

1. Workflow Definitions remain exactly what `docs/workflow/workflow-engine-architecture.md`
   already specifies: a versioned, tenant-editable JSON activity graph
   (`workflow_definition.graph`), edited in Admin Studio, no deploy
   required to change one.
2. A **single, generic Temporal Workflow** (`GraphExecutorWorkflow`) —
   written once, in code, as part of the platform — interprets that JSON
   graph at runtime: it walks the graph's nodes, and for each node type
   (Action, Approval, Wait, Branch, Subworkflow) invokes a corresponding
   **Temporal Activity**.
3. Temporal supplies the durability/retry/timer/signal machinery this
   program's `ADR-013` had to hand-build in `Host.Workers` (step tables,
   `scheduled_for` polling, idempotency-key bookkeeping, exponential
   backoff) — that hand-built machinery is **retired in favor of
   Temporal's**, because Temporal's is more battle-tested and removes a
   meaningful chunk of bespoke code this program would otherwise have to
   build and operate itself.
4. `Approval`/`Wait` steps map to Temporal **Signals** (external event
   resumes a paused workflow) and **Timers** respectively — the same
   semantics `docs/workflow/workflow-engine-architecture.md` already
   specified, now backed by Temporal instead of a custom `scheduled_for`
   polling worker.
5. `Subworkflow` maps directly to a Temporal **Child Workflow** invocation
   of the same `GraphExecutorWorkflow`, parameterized with the child
   definition's ID — composition works exactly as already designed.

## Why this synthesis, not a straight adoption of Temporal's native model

Abandoning the data-defined graph for hand-written Temporal workflow code
per business process would mean every tenant-specific approval-flow tweak
becomes an engineering release — the exact outcome `ADR-013` was written to
prevent, and the single strongest lesson this entire program drew from
researching ServiceNow's architecture (`docs/servicenow-research/product-landscape.md`
§1: "Workflow Automation / Flow Designer... business logic as data"). That
lesson does not stop being true because the *execution engine* underneath
changed.

Conversely, rejecting Temporal entirely and keeping the fully hand-built
`Host.Workers` execution machinery would mean re-implementing (and
continuing to operate) a bespoke durable-execution system that a mature,
purpose-built OSS product already solves — real, unjustified engineering
and operational cost once Temporal is available in the stack anyway
(`ADR-022`).

## What changes in `docs/workflow/workflow-engine-architecture.md`

- §"Execution model" is superseded: step execution, retry/backoff, and
  Wait-timer polling are now Temporal's job, not a `Host.Workers` polling
  loop against a `workflow_step.scheduled_for` column.
- The `workflow_instance`/`workflow_step` tables are retained as a
  **read-optimized projection** (for Admin Studio's "view running
  instances" UI and for Reporting), populated from Temporal's own
  Workflow/Activity execution history via Temporal's visibility API and/or
  activity-side writes — they are no longer the durability mechanism
  itself, only an observability/reporting mirror of it.
- Idempotency: each Activity invocation still carries the same
  `idempotency_key` derived from `(instance_id, node_id, attempt_count)` —
  Temporal guarantees at-least-once Activity execution, so idempotent
  Activities remain required exactly as `docs/integration/event-driven-architecture.md`
  Rule 11 already mandates platform-wide.
- Compensation: implemented as Temporal's standard **Saga pattern**
  (explicit compensating Activities invoked on failure) — same concept
  `ADR-013` already specified, now with a named implementation mechanism.

## What does NOT change

- Workflow Definitions are still metadata, still tenant-editable without a
  deploy, still versioned with pinned-version instances.
- Business Rules (Phase 13) are unaffected — they remain synchronous,
  in-transaction, and explicitly separate from Workflow, per the existing
  decision table in `docs/workflow/business-rule-engine.md`.
- The activity type vocabulary (Trigger/Condition/Action/Approval/Wait/
  Event/Branch/Subworkflow) is unchanged.

## Trade-offs

- The team now operates a Temporal cluster (its own Postgres/Elasticsearch-
  backed persistence, its own workers) in addition to everything else in
  `ADR-022`'s self-hosted stack — real operational surface, accepted for
  the durability/observability it buys versus the hand-built alternative.
- The `GraphExecutorWorkflow` interpreter is a piece of code with real
  complexity of its own (it is, in effect, a small workflow-graph virtual
  machine) — but it is written **once**, centrally, reviewed like any other
  platform-kernel code, not per business process.

## Consequences

- `docs/workflow/workflow-engine-architecture.md` gets a superseding note
  pointing here for the execution model.
- `docs/architecture/microservice-boundary-rules.md`'s Workflow row
  ("modular monolith + satellite workers") is refined: Temporal Workers
  (processes that poll Temporal task queues and run Activities) become the
  new home for what `Host.Workers` previously did for workflow specifically;
  `Host.Workers` remains for the *other* things it always owned (outbox
  relay, SLA clock evaluation, scheduled jobs via Hangfire/Quartz.NET per
  `ADR-022`).

## Rejected alternatives

1. **Full Temporal adoption, drop the data-defined graph** — rejected: loses
   the no-deploy tenant customization capability that is this program's
   single strongest ServiceNow-derived lesson.
2. **Reject Temporal, keep the fully hand-built engine** — rejected: forces
   the team to build and operate bespoke durable-execution machinery a
   mature OSS product already provides, once that product is in the stack
   for other reasons anyway.
