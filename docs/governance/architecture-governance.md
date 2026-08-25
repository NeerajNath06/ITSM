# Architecture Governance

**Phase:** 50

## Architecture Review Board (ARB)

- A small standing group (architecture leads + a rotating senior engineer
  seat) reviews: any new ADR, any proposed microservice extraction
  (against `microservice-boundary-rules.md`'s criteria), any new external
  dependency of significant weight, any change to the locked stack
  (`ADR-000`).
- The ARB does **not** review routine feature PRs — that's ordinary code
  review (`docs/rules/git-rules.md`). It reviews decisions that change this
  documentation set.

## Coding standards

Enforced per `docs/rules/coding-rules.md`, `database-rules.md`,
`api-rules.md`, `microservice-rules.md`, `security-rules.md`, `git-rules.md`
— reviewed and versioned like any other governance artifact; a proposed
rule change goes through the same PR process as a code change, with the ARB
as reviewer for anything altering a binding rule (not just an example).

## API governance

- Every public API version freeze/deprecation follows a documented window
  (minimum: announce → dual-support period → sunset), tracked centrally so
  no partner integration breaks without notice.
- New public endpoints reviewed against `docs/api/api-design-rules.md`
  before merge — API design review is part of ordinary PR review, escalated
  to the ARB only for a genuinely new pattern (e.g. introducing a fourth API
  surface beyond Table/Domain/BFF).

## Database governance

- Schema changes follow `docs/rules/database-rules.md`; any migration
  touching a `tenant_id`/RLS-bearing table or a shared kernel table
  (`task`, `cmdb_ci`, `identity_user`) requires a second reviewer beyond the
  PR's primary approver.

## Security review

- Security-sensitive PRs (auth, ACL, tenancy, encryption, new external
  integration) require review from someone with security-review authority,
  per `docs/rules/security-rules.md`.
- Quarterly access reviews (`docs/governance/data-governance.md`) are a
  standing ARB-tracked obligation, not ad hoc.

## Dependency review

- New third-party dependencies (NuGet/npm) above a low-risk threshold
  (license type, maintenance activity, transitive dependency count) get a
  brief written justification in the PR — matches Master AI Command Rule
  15 ("never introduce dependencies without justification").

## ADR process

1. Propose: a draft ADR PR, following the Context/Problem/Options/Decision/
   Why/Trade-offs/Consequences/Rejected-Alternatives structure used
   throughout `docs/adr/`.
2. Review: ARB + affected module owners.
3. Accept: merged, status `Accepted`, referenced from any document whose
   guidance it changes.
4. Supersede (never silently edit): a later decision that changes an
   earlier one is a **new** ADR that explicitly supersedes the old one's
   status — matches the pattern `ADR-000` itself established for the
   original guide.

## Release governance

- Production releases require the CI/CD quality gates
  (`docs/devops/cicd.md`) plus the Definition of Done
  (`docs/governance/definition-of-done.md`) for every included change —
  release governance is the aggregate of per-change gates, not a separate
  manual checklist duplicating them.

## Observability requirements as governance

- No component reaches production without OpenTelemetry instrumentation
  and health checks (Phase 24) — enforced as an ARB-reviewable gate for any
  new deployable (a new module doesn't need review since it lives inside
  the already-instrumented `Host.Api`; a new extracted service does).

## Technical debt management

- Technical debt (including "temporary" feature flags past their removal
  date, `REQUIRES_VALIDATION`/`UNKNOWN` markers left in research docs,
  deferred CSDM/search-migration triggers) is tracked as a visible backlog,
  reviewed quarterly by the ARB — not left implicit in code comments or
  forgotten Slack threads.

---

*Next: `CLAUDE.md` (Phase 51) — the master AI coding agent rules file.*
