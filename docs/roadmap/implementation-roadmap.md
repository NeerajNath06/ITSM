# Implementation Roadmap

**Phase:** 43 · Sequenced delivery plan. Do **not** implement everything
simultaneously — each phase below gates the next.

## Phase 1 — Foundation

- **Objective:** a buildable, testable, deployable monorepo skeleton with
  CI/CD and architecture guardrails, before any product code.
- **Dependencies:** none.
- **Deliverables:** solution skeleton (`Host.Api`, `Host.Workers`
  placeholders), React workspace (three app shells + design-system),
  Docker Compose for local PostgreSQL/Valkey/RabbitMQ/MinIO/Keycloak/
  OpenBao, CI pipeline (build, test, SAST, architecture tests), OpenTofu
  IaC skeleton for the self-hosted Kubernetes/K3s target
  (`docs/infrastructure/self-hosted-production-architecture.md`).
- **Database changes:** none yet (empty migration baseline).
- **APIs:** none yet.
- **Frontend:** empty app shells only.
- **Tests:** architecture tests (module boundary enforcement) passing on
  an empty codebase.
- **Deployment:** `dev` environment stands up successfully via CI/CD.
- **Risks:** under-investing in CI/architecture-test rigor here compounds
  into every later phase — this phase is not skippable for speed.

## Phase 2 — Identity and Organization

- **Objective:** Identity, Tenant, RBAC/ACL foundation, Audit — the
  platform kernel's security half.
- **Dependencies:** Phase 1.
- **Deliverables:** `identity_user`/`identity_group`/roles, OIDC
  federation, tenant provisioning (shared + dedicated modes), RLS policies,
  ACL policy engine, audit interceptor.
- **Database changes:** identity, tenant, ACL rule, audit_log tables +
  RLS.
- **APIs:** `/api/v1/identity/*`, `/api/v1/tenants/*` (platform-admin only).
- **Frontend:** login flow, BFF session handling.
- **Tests:** **isolation test suite** (two tenants, zero leakage) — first
  gate that must be green before Phase 3.
- **Deployment:** `dev` + `staging` environments.
- **Risks:** getting RLS/ACL wrong here is the most expensive class of bug
  to fix later (every subsequent table depends on this foundation).

## Phase 3 — Platform Services

- **Objective:** Metadata, Configuration, Feature Flags, Notification
  (basic), Search (basic), File Storage, Observability.
- **Dependencies:** Phase 2.
- **Deliverables:** metadata-driven table/field/layout store, tenant
  configuration + feature flags, Notification platform (Group/Shift/
  Escalation, email+in-app channels first), OpenSearch search,
  MinIO attachment pipeline, OpenTelemetry wired end-to-end.
- **Tests:** metadata CRUD through Table API with ACL; notification
  delivery smoke test.
- **Risks:** scope creep — this phase should ship the minimum viable
  version of each capability, not the full Phase 20–24 depth (that depth
  lands incrementally as product phases need it).

## Phase 4 — Workflow

- **Objective:** Workflow engine + Business Rule engine, Approval activity,
  SLA engine foundation.
- **Dependencies:** Phase 3 (Notification for approval requests).
- **Deliverables:** Workflow Definition/Instance/Step runtime in
  `Host.Workers`, Business Rule registration mechanism, SLA Definition +
  clock evaluation.
- **Tests:** golden-path approval workflow test, SLA start/pause/breach
  test against a test business calendar.
- **Risks:** building Workflow before there's a real consumer (no product
  yet) risks over-engineering; mitigated by co-designing this phase against
  Incident's actual assignment-routing and Request's actual
  approval/fulfillment needs from Phase 5, not in the abstract.

## Phase 5 — Service Management (ITSM core)

- **Objective:** Incident, Problem, Change, Request, Service Catalog,
  Knowledge — the MVP ITSM surface.
- **Dependencies:** Phases 2–4.
- **Deliverables:** per `docs/domains/incident-management.md`,
  `problem-management.md`, `change-management.md`,
  `request-management.md`, `service-catalog.md`, `knowledge.md`.
- **Frontend:** Agent Workspace inbox + record pages, Employee Portal
  catalog/knowledge/my-requests.
- **Tests:** Playwright golden paths (create → resolve → CSAT), SLA
  integration tests, ACL matrix for all new tables.
- **Risks:** this is the largest product phase — resist adding
  out-of-MVP domains (Case, Customer, Employee) here regardless of
  pressure to "just add one more thing."

## Phase 6 — CMDB

- **Objective:** CI/relationship graph, CSDM-inspired service layers,
  impact analysis on Incident/Change.
- **Dependencies:** Phase 5 (Incident/Change need to consume impact data).
- **Deliverables:** per `docs/cmdb/cmdb-architecture.md`,
  `csdm-inspired-service-model.md`, `cmdb-data-quality-rules.md`. Manual CI
  entry first; Discovery ingest can follow once the graph model is proven.
- **Tests:** impact-graph query tests, data-quality rule tests.

## Phase 7 — IT Operations

- **Objective:** Discovery ingest (satellite), light Event Management,
  Asset Management (light, linked to CI).
- **Dependencies:** Phase 6.
- **Deliverables:** Discovery collector agent + ingest API, `asset`/
  `model`/`license`/`contract` tables per `docs/domains/asset-management.md`.
- **Risks:** full AIOps-grade event correlation is explicitly deferred
  (see `mvp.md`) — this phase ships foundation ingest only.

## Phase 8 — Security

- **Objective:** harden the platform for first regulated/enterprise
  tenants — pentest gate, dedicated-DB tenancy proven in production,
  compliance controls operationalized.
- **Dependencies:** Phases 2–7 stable in production.
- **Deliverables:** external penetration test passed, access review
  process running, DR runbook drilled.
- **Note:** the Security **domain** (Security Incident Response, Phase 2/5
  finding) is a separate, later product decision (Phase 8 here is
  hardening the platform, not building a SecOps product).

## Phase 9 — Customer

- **Objective:** CSM product line (Case Management, Customer Management),
  only if/when pursued as a product.
- **Dependencies:** Phases 1–8 in production with paying ITSM tenants.
- **Deliverables:** per `docs/domains/case-management.md`,
  `customer-management.md` — extending the existing `task` base, not a new
  hierarchy.

## Phase 10 — Employee

- **Objective:** HRSD product line, only if/when pursued.
- **Dependencies:** Phase 9 pattern proven; dedicated security review given
  Employee Management's PII sensitivity (`docs/domains/employee-management.md`).

## Phase 11 — Analytics and AI

- **Objective:** Reporting/Performance-Analytics depth, AI assist
  (Phase 46/47 architecture), Virtual Agent.
- **Dependencies:** Phases 1–8 with real production data (AI needs data to
  be useful and ACL-tested to be safe).
- **Deliverables:** per `docs/ai/ai-architecture.md`, `ai-governance.md`.

---

*Next: `docs/roadmap/mvp.md` (Phase 44) — the precise minimum slice of the
above that constitutes a shippable product.*
