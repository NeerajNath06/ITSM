# CLAUDE.md — Master Rules for AI Coding Agents

This file is the binding entry point for any AI coding agent (or human)
working in this repository. It summarizes the complete architecture/rules
system under `docs/`. **Read this file and `docs/MASTER-ARCHITECTURE.md`
before writing code.** When in doubt, the detailed phase document cited
next to each rule is authoritative.

**Stack:** ASP.NET Core / .NET 10, React + TypeScript, PostgreSQL, Valkey,
RabbitMQ, REST + gRPC, OAuth2/OIDC + JWT (Keycloak), OpenTelemetry
(Prometheus/Loki/Tempo/Grafana), Docker, Kubernetes (K3s) — self-hosted,
cloud-agnostic infrastructure per
`docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`, superseding
`ADR-000`'s Azure-managed-PaaS default (Azure remains a documented
alternative deployment target, not required). Secrets: OpenBao. Object
storage: MinIO. Search: OpenSearch. Vector DB: Qdrant. AI runtime: Ollama
(default) with a pluggable hosted-provider option. Workflow execution:
Temporal, per `docs/adr/ADR-023-temporal-for-workflow-execution.md`
(durable execution only — Workflow Definitions remain metadata-driven,
tenant-editable graphs; see that ADR before touching workflow code).
Feature flags: OpenFeature + Unleash. CI/CD: Jenkins or Gitea Actions
against Gitea (final pick pending a follow-up ADR). Registry: Harbor. IaC:
OpenTofu.

**Architecture:** Modular monolith (`Host.Api`) + a small, explicitly
justified set of extracted satellite services (Notification, Search
indexer, AI inference, Discovery) — never microservice-per-domain. See
`docs/architecture/microservice-boundary-rules.md`.

## The 20 non-negotiable rules

1. **Never violate architecture.** Every decision in `docs/architecture/`,
   `docs/adr/` is binding unless superseded by a newer ADR.
2. **Never create unnecessary microservices.** A new service requires a
   named, evidenced trigger from `docs/architecture/microservice-boundary-rules.md`
   — never "it feels like it should be separate."
3. **Never duplicate shared entities.** Extend `TaskBase`/`cmdb_ci`/existing
   relates-to patterns (`docs/database/shared-enterprise-data-model.md`)
   instead of inventing a parallel structure.
4. **Never bypass authorization.** Every table/field access is ACL-checked,
   every call — UI, API, workflow, integration, or AI — with no
   "trusted internal caller" exception (`docs/security/security-architecture.md`).
5. **Never store secrets in source code.** OpenBao references only
   (`docs/rules/security-rules.md` #11).
6. **Never directly modify production data.** Changes go through
   application code + migrations + the CI/CD pipeline, never manual
   `psql`/`UPDATE` against `staging`/`prod`.
7. **Never create database changes without migrations.** EF Core migrations
   only (`docs/rules/database-rules.md`).
8. **Never create APIs without documentation.** OpenAPI/`.proto` generated
   from code, every endpoint documented (`docs/api/api-design-rules.md`).
9. **Never create business logic in React.** React holds presentation
   and API orchestration only (`docs/frontend/react-architecture.md`).
10. **Never create business logic inside controllers.** Controllers/minimal-API
    endpoints map HTTP ↔ command/query only; logic lives in `Application`/
    `Domain` (`docs/backend/dotnet-architecture.md`).
11. **Never call another service's database directly.** Cross-boundary data
    access is API or event only (`docs/database/database-architecture.md`).
12. **Never ignore tenant boundaries.** Every tenant-scoped table has both
    an EF Core query filter and a PostgreSQL RLS policy, in the same
    migration (`docs/architecture/multi-tenancy.md`).
13. **Never ignore audit requirements.** Every mutation on Task-family,
    CMDB, ACL, Role, or Workflow Definition data is audited in the same
    transaction (`docs/audit/audit-architecture.md`).
14. **Never ignore observability.** No component ships without
    OpenTelemetry instrumentation and health checks
    (`docs/observability/observability.md`).
15. **Never introduce dependencies without justification.** A brief written
    reason accompanies any new NuGet/npm package
    (`docs/governance/architecture-governance.md`).
16. **Never change architecture without an ADR.** See the ADR process in
    `docs/governance/architecture-governance.md`.
17. **Never create duplicate workflow engines.** All multi-step/orchestration
    logic goes through the one Workflow engine
    (`docs/workflow/workflow-engine-architecture.md`).
18. **Never duplicate notification logic.** All alerting goes through the
    Notification platform (`docs/platform/notification-service.md`,
    `docs/architecture/09-notification-oncall-escalation-engine.md`).
19. **Never duplicate identity logic.** Identity/session/role resolution is
    the platform kernel's job, called by everyone else
    (`docs/architecture/bounded-contexts.md`).
20. **Never bypass domain boundaries.** A bounded context reads another's
    data only through its published interface/API/events, writes only its
    own tables (`docs/architecture/bounded-contexts.md` cross-context
    rules).

## Additional rules discovered during this research program

21. **AI must never bypass authorization**, and every AI-mutating action
    requires the same authorization + audit as a human-triggered one
    (`docs/ai/ai-governance.md`).
22. **Task is the base for every trackable unit of work.** Before adding a
    new record type, check whether it's actually a `TaskBase` subtype
    (`docs/domain/task-model.md`).
23. **CMDB and Asset are never merged.** CI = technical/relationship
    identity; Asset = commercial/lifecycle data; linked, not unified
    (`docs/database/shared-enterprise-data-model.md`).
24. **Security Incident data (if ever built) is never under the same ACL
    domain as general Incident data**, even though both extend `task`
    (`docs/domains/security.md`).
25. **REST for external/UI/partner traffic, gRPC only for internal
    service-to-service calls that pass the justification checklist** — never
    the default for a new internal call without checking
    (`docs/api/rest-grpc-rules.md`).
26. **A repository/generic-repository pattern is not applied uniformly** —
    use `DbContext` directly for simple CRUD; reserve a repository for
    genuinely complex, testability-driving query encapsulation
    (`docs/backend/dotnet-architecture.md`).
27. **No unlimited retries, anywhere.** Every retry loop (workflow steps,
    RabbitMQ consumers, provider calls) is bounded with backoff
    (`docs/integration/event-driven-architecture.md`).
28. **Every event consumer is idempotent** via the inbox pattern
    (`docs/integration/event-driven-architecture.md` Rule 11).
29. **Destructive migrations ship in two steps** (stop using, then drop) —
    never in the same deploy as the code change that stops using a column
    (`docs/rules/database-rules.md`).
30. **A flag with no removal plan is a governance smell.** Every feature
    flag has an owner and a removal date (`docs/platform/feature-flags.md`).

## Before implementing anything significant

Follow the loop in `docs/rules/ai-agent-rules.md` (Phase 52):
**Understand → Inspect → Plan → Validate architecture → Implement → Test →
Review → Document.** Do not start coding immediately on a non-trivial
request.

## Anti-patterns

Full list in `docs/rules/anti-patterns.md` (Phase 53) — check before
proposing a design: God Service/Controller/Database, distributed monolith,
chatty services, circular dependencies, generic-repository-everywhere,
service-to-service database access, hardcoded configuration, secrets in
Git, synchronous distributed transactions, unlimited retries, missing
idempotency/correlation-IDs/audit/authorization, duplicate master data.

## Definition of Done

A feature is not complete until every item in
`docs/governance/definition-of-done.md` (Phase 54) is satisfied —
architecture reviewed, migration created, API documented, authorization
implemented, audit implemented, logging/tracing/metrics implemented, unit +
integration tests implemented, security reviewed, performance considered,
documentation updated, ADR created if architecture changed.

## Where to look first

| Question | Document |
|---|---|
| "What's the overall architecture?" | `docs/MASTER-ARCHITECTURE.md` |
| "Where does this data/logic belong?" | `docs/architecture/bounded-contexts.md` |
| "Should this be a new service?" | `docs/architecture/microservice-boundary-rules.md` |
| "How do I model this table?" | `docs/database/shared-enterprise-data-model.md`, `docs/rules/database-rules.md` |
| "How do I build this API?" | `docs/api/api-design-rules.md`, `docs/rules/api-rules.md` |
| "Is this a Business Rule or a Workflow?" | `docs/workflow/business-rule-engine.md` decision table |
| "What ADRs exist?" | `docs/adr/` |
| "What's the current MVP scope?" | `docs/roadmap/mvp.md` |
| "What's already been built vs. deferred?" | `docs/roadmap/implementation-roadmap.md` |
| "Is this documentation program itself finished?" | `docs/PROGRAM-STATUS.md` |
| "What infrastructure/product runs a given capability?" | `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`, `docs/infrastructure/self-hosted-production-architecture.md` |
| "How does Workflow relate to Temporal?" | `docs/adr/ADR-023-temporal-for-workflow-execution.md` |

## If existing code conflicts with this documentation

**Document the conflict first, do not silently rewrite it** — follow the
pattern in `docs/adr/ADR-000-technology-stack-supersession.md`: state what
exists, what this documentation says, and which wins, before changing code.
