# ADR-013 — Workflow Engine

**Status:** Accepted · **Full rationale:** `docs/workflow/workflow-engine-architecture.md`, `business-rule-engine.md` (Phase 12–13)

## Decision
A custom, data-defined workflow engine (Trigger/Condition/Action/Approval/
Wait/Event/Branch/Subworkflow), versioned definitions with pinned-version
instances, durable step execution in `Host.Workers`, idempotent actions,
explicit compensation — kept strictly separate from a lighter, synchronous
Business Rule engine for record-scoped validation/defaulting logic.

## Why
This mirrors the single most consistent architectural lesson from
researching ServiceNow (Phase 1/3): workflow-as-data, not workflow-as-code.
Splitting Workflow from Business Rules prevents the common failure mode of
either blocking a request thread on long-running orchestration, or paying
orchestration overhead for simple synchronous validation.

## Trade-offs
Building a custom engine (vs. adopting an off-the-shelf workflow
orchestrator) is real implementation investment; accepted because our
domain model (Task-aware conditions, ACL-integrated actions) needs tight
integration a generic orchestrator wouldn't provide out of the box, and we
explicitly reject replicating ServiceNow's own scripting-engine complexity
(no arbitrary tenant-authored server-side scripts).

## Consequences
Every multi-step, human-involving, or potentially long-running process in
the platform is a Workflow Definition, not hand-coded orchestration logic
scattered across handlers.

## Rejected alternative
Full tenant-scriptable business rules (ServiceNow's model) — rejected per
Master AI Command Rule 2/3 as an unjustified ongoing security-review burden
for our scope.
