# ADR-021 — React Over Angular

**Status:** Accepted · **Supersedes:** the Frontend row of
`docs/adr/ADR-000-technology-stack-supersession.md` and everything built on
it (Phase 30's original Angular architecture, all Angular-specific
guidance across the documentation set).

## Context

`ADR-000` locked Angular + TypeScript as the frontend framework when the
Master AI Command's stack was adopted wholesale, superseding the original
`ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md`'s React 19 + TypeScript + Vite
choice. The user has since directed that React be used instead of Angular,
for the three Angular applications (`employee-portal`, `agent-workspace`,
`admin-console`) and the shared `design-system` library.

## Problem

Which frontend framework builds the Employee Portal, Agent Workspace, and
Admin Console?

## Options

1. Angular (per `ADR-000`).
2. React + TypeScript (per the original guide, and the user's current
   direction).

## Decision

**React + TypeScript**, via Vite, with React Router for routing and
TanStack Query for server-state/data-fetching — this also happens to
restore the original guide's frontend choice, so it is a full reversal of
that one row of `ADR-000` only, not a re-opening of the rest of the stack
(PostgreSQL, RabbitMQ, REST+gRPC, OAuth2/OIDC+JWT, Azure, etc. are
unaffected and remain as decided).

## Why

Explicit user direction. No architectural properties elsewhere in this
program depend on Angular specifically — the BFF pattern, the
metadata-driven form/list/record renderer, the auth-cookie/session model,
and the three-app-shell structure are all framework-agnostic and carry
over unchanged to React.

## Trade-offs

React's less opinionated structure (vs. Angular's built-in DI/module
system) means the team must hold its own conventions (file/folder
structure, state-management choice) deliberately rather than getting them
from framework defaults — addressed by `docs/frontend/react-architecture.md`
prescribing those conventions explicitly rather than leaving them
ad hoc.

## Consequences

- `docs/frontend/angular-architecture.md` is replaced by
  `docs/frontend/react-architecture.md` (Phase 30 content rewritten for
  React idioms: hooks, React Router, TanStack Query, code-splitting via
  `React.lazy`/`Suspense`).
- Every other document in this program that named "Angular" as the
  frontend technology is updated to say "React" — a terminology change
  only; no other architectural decision in those documents changes as a
  result (see the updated files' diffs for confirmation none of them
  encoded Angular-specific behavior beyond naming).
- `CLAUDE.md`'s stack line and Rule 9 are updated accordingly.

## Rejected alternative

Keeping Angular — rejected per explicit user direction; no technical
justification for Angular over React was strong enough on its own merits to
override that direction (the `ADR-000` rationale for Angular was "matches
the Master AI Command's stated stack," not an independent technical
argument).
