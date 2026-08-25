# React Architecture

**Phase:** 30 · Frontend for Employee Portal, Agent Workspace, Admin
Console. Framework per `docs/adr/ADR-021-react-over-angular.md` (React +
TypeScript via Vite, reinstating the original guide's choice over the
Angular decision `ADR-000` made when the Master AI Command's stack was
first adopted).

## Application shell strategy

- **Three React applications** (not one monolithic app with mode-switching):
  `employee-portal`, `agent-workspace`, `admin-console` — each has a
  distinct user base, information density, and update cadence, mirroring
  the Phase 3 finding that ServiceNow's channels are distinct experience
  shells over one backend, not one UI trying to serve every audience.
- A shared **`design-system`** package (npm workspace library, not a
  fourth deployable) provides tokens, primitives, and the metadata-driven
  form/list/record renderer consumed by all three apps — no app duplicates
  its own button/table/form components.
- Each app is a Vite-built React 19 + TypeScript SPA, matching the original
  guide's stack choice and giving fast dev-server startup/HMR for a large
  multi-team codebase.

## Module structure (per app)

```
src/
  app/
    core/            # auth context/provider, fetch/query client setup, error boundary
    shared/           # dumb/presentational components, hooks, shared with intra-app features only
    features/
      incidents/
        routes.tsx             # React Router route objects, lazy-loaded via React.lazy
        data-access/            # API client + TanStack Query hooks (useIncident, useIncidents)
        feature-*/               # container components (own data fetching + composition)
        ui-*/                    # presentational components (props in, JSX out, no fetching)
      changes/
      catalog/
      ...
```

- **Function components + hooks throughout** — no class components, no
  legacy lifecycle-method patterns.
- **Feature-based, not type-based, top-level organization** (`features/incidents/`
  not `components/` + `services/` at the root) — keeps a feature's data
  access, state, and UI colocated, so removing or extracting a feature
  doesn't require hunting across unrelated folders.
- **Code-splitting per feature route** via `React.lazy` + `Suspense` —
  `agent-workspace` in particular must not load Change/Catalog/Knowledge
  code on initial paint if the user opens straight into the Incident
  inbox.
- **React Router** (data router / loader pattern) for routing — route
  loaders fetch the data a page needs before render, keeping fetch-waterfall
  and loading-state logic out of individual components where practical.

## State management

- **TanStack Query** for all server state (data fetched from the API) —
  caching, refetch-on-focus, optimistic updates, and request
  deduplication are handled by the library, not hand-rolled per feature.
- **React Context + `useState`/`useReducer`** for local/UI-only state
  (form draft state, panel open/closed, wizard step) — kept close to
  where it's used, not hoisted to a global store by default.
- A heavier global state library (Redux/Zustand-class) is **not** adopted
  by default — only introduced for a feature with genuinely complex,
  deeply cross-cutting client state that TanStack Query + Context
  demonstrably can't express cleanly, evaluated case by case (mirrors the
  "don't add abstractions because they're popular" rule). If one is
  needed, Zustand is preferred over Redux for its lower ceremony, decided
  per-feature via a documented justification, not platform-wide by
  default.
- **A thin data-access hook per resource** (`useIncident(id)`,
  `useIncidents(filters)`) wraps the generated API client — components
  never call `fetch`/`axios` directly.

## API clients

- Generated from the OpenAPI spec (`docs/api/api-design-rules.md`) via a
  code-gen step in CI (e.g. `openapi-typescript` + a thin typed fetch
  wrapper) — the TypeScript client types are never hand-maintained
  separately from the backend contract, preventing drift.
- A single fetch wrapper (used by every generated client call) handles:
  attaching the auth cookie/CSRF token, correlation ID propagation, and
  centralized error mapping from `ProblemDetails` responses into a
  UI-friendly error shape that TanStack Query surfaces to components.

## Authentication and authorization (frontend side)

- BFF cookie-session model (per `docs/security/security-architecture.md`)
  — React never handles a raw JWT.
- **Route-level guards** (a wrapper component or router loader check)
  check coarse capability (does this role even have a route to this
  feature) for UX/navigation purposes only — this is **never** the
  authorization boundary; every guarded route's data still goes through
  full ACL evaluation server-side. A route guard hiding a menu item is a
  convenience, not a security control.
- Field-level permission hints (from the metadata API) drive which form
  fields render as read-only/hidden — again UX only, enforced for real by
  the server's field-level ACL strip on the response.

## Reusable components and design system

- Metadata-driven **record page**, **list view**, and **form renderer**
  components consume the Table/Metadata API directly — a new custom field
  or a new table does not require a new React component or a release,
  matching the platform's metadata-driven principle end to end from backend
  to frontend.
- Design tokens (color, spacing, typography) defined once in
  `design-system`, consumed via CSS custom properties — supports theming
  (tenant branding, Phase 18) without component-level overrides.

## Accessibility

- WCAG 2.2 AA baseline, automated checks (axe-core, via `jest-axe` or the
  Playwright axe integration) in CI on critical paths (login, incident
  create, catalog checkout) for each of the three apps — a11y regressions
  fail the build, not just a lint warning.

## Error handling

- A root **Error Boundary** per app (and per major route segment) catches
  render-time exceptions, reports to the observability backend (Grafana
  stack, Phase 24) with the correlation ID, and shows a non-technical
  fallback UI — raw stack traces never reach end users.
- TanStack Query's error state + `ProblemDetails` field-level errors map
  directly onto form field error states via the shared form renderer, so a
  backend validation change doesn't require frontend code changes to
  surface correctly.

## Telemetry

- OpenTelemetry Web SDK instruments route changes, API call duration, and
  unhandled errors, exported through the same OTel Collector →
  Prometheus/Loki/Tempo pipeline as the backend, tagged with the same
  correlation ID scheme — a user-reported "the page was
  slow" is traceable end-to-end.

## Testing

- **React Testing Library** + **Vitest** for component tests — tests
  render components and assert on user-visible behavior, not internal
  implementation/state, matching `docs/testing/testing-strategy.md`'s
  Component layer.
- **Playwright** for E2E golden paths, shared with the rest of the testing
  strategy.

---

*Next: `docs/backend/dotnet-architecture.md` (Phase 31).*
