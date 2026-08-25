# Angular Architecture — Superseded

**Status:** Superseded by `docs/frontend/react-architecture.md`, per
`docs/adr/ADR-021-react-over-angular.md`.

This document originally specified the Phase 30 frontend architecture using
Angular, when `docs/adr/ADR-000-technology-stack-supersession.md` had
locked Angular as the frontend framework. The user has since directed that
React be used instead — see `ADR-021` for the full decision record.

**Everything framework-agnostic in the original document carries over
unchanged**: the three-app-shell strategy (`employee-portal`,
`agent-workspace`, `admin-console`), the shared `design-system` library,
feature-based folder organization, lazy-loaded feature routes, the BFF
cookie-session auth model, metadata-driven record/list/form rendering, the
WCAG 2.2 AA accessibility baseline, and the error-handling/telemetry
approach. Only the framework-specific mechanics changed (Angular modules/
signals/`NgRx` → React components/hooks/TanStack Query, etc.).

**Do not use this document for new work — read `docs/frontend/react-architecture.md`.**
This file is kept only so nothing in the documentation history is silently
deleted, per this program's own governance rule (`docs/governance/architecture-governance.md`).
