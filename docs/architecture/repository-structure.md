# Repository Structure

**Phase:** 45 · Adapted from the Master AI Command's example structure to
our actual finalized architecture (modular monolith core + a small set of
extracted satellites, React multi-app frontend, per `ADR-021-react-over-angular.md`).

```text
ITSM/
  src/
    BuildingBlocks/            # cross-cutting kernel primitives shared by every module:
                                # base entity types, domain-event base classes, the
                                # MediatR pipeline behaviors (validation, auditing),
                                # the ITenantContext abstraction, ProblemDetails mapping
    Platform/                  # the platform kernel bounded contexts (Phase 5):
                                # Identity, Tenancy, Acl, Audit, Configuration, Metadata
    Modules/                   # ITSM/CMDB/ITAM product modules, each Clean-Architecture-layered:
      Incident/
      Problem/
      Change/
      Request/
      ServiceCatalog/
      Knowledge/
      Cmdb/
      Asset/
      Workflow/                # the Workflow + Business Rule engines (Phase 12-13)
    Host.Api/                  # ASP.NET Core composition root - the modular monolith process
    Host.Workers/               # background worker host: outbox relay, workflow timers,
                                # SLA clock evaluation, scheduled jobs
    Services/                  # extracted satellite services, each independently deployable:
      NotificationService/
      SearchIndexerService/
      AiInferenceService/       # added when Phase 46/47 ships
    Gateway/                   # YARP configuration/routing beyond what ships inside Host.Api
                                # (usually just IaC/config, not application code)

  apps/                        # React workspace (npm workspaces + Vite)
    employee-portal/
    agent-workspace/
    admin-console/
    design-system/             # shared React component library: tokens, primitives,
                                # metadata-driven form/list/record renderer

  tests/
    Unit/
    Integration/                # Testcontainers-backed (PostgreSQL, Valkey, RabbitMQ)
    Contract/
    Isolation/                  # the tenant-isolation suite - highest-priority suite in the repo
    E2E/                        # Playwright

  docs/
    MASTER-ARCHITECTURE.md
    PROGRAM-STATUS.md
    ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md   # superseded on stack specifics, see ADR-000
    architecture/
    database/
    backend/
    frontend/
    domain/
    domains/
    workflow/
    cmdb/
    security/
    audit/
    platform/
    integration/
    api/
    ai/
    azure/
    observability/
    testing/
    devops/
    governance/
    roadmap/
    adr/
    diagrams/
    servicenow-research/
    rules/

  deploy/
    docker-compose.yml          # local dev dependencies: PostgreSQL, Valkey, RabbitMQ,
                                # MinIO, OpenSearch, Keycloak, OpenBao, Mailpit, Temporal
    iac/                        # OpenTofu modules - see
                                # docs/infrastructure/self-hosted-production-architecture.md
    .gitea/workflows/           # or Jenkinsfile - see docs/devops/cicd.md (final CI pick pending ADR)

  CLAUDE.md
```

## Why this differs from the Master AI Command's example

- **`Services/` replaces a generic microservices folder** — it holds
  exactly the small, named set of extracted satellites from
  `microservice-boundary-rules.md`, not a folder implying every domain gets
  its own service.
- **`Modules/` groups product code**, `Platform/` groups kernel code — kept
  separate so the NetArchTest-equivalent boundary rule ("modules may not
  reference each other's Infrastructure; kernel is referenced by everyone")
  maps directly onto folder structure, not just a convention.
- **`apps/` uses an npm-workspaces convention** (`design-system` as a
  shared library, not a fourth deployable app) rather than a generic `Web/`
  folder.
- No `docs/domain/` vs `docs/domains/` collision left ambiguous: `domain/`
  holds the cross-cutting Task-model design (Phase 9), `domains/` holds the
  per-product-domain profiles (Phase 40) — kept as the two names already in
  use throughout this program rather than renamed now and breaking every
  existing cross-reference.

## What must NOT happen

- A new bounded context must not get its own top-level folder outside
  `Modules/` "because it feels important" — it either fits the modular
  monolith layering or it goes through the extraction-justification process
  in `microservice-boundary-rules.md` and lands in `Services/`.
- `apps/` must not gain a fourth full React application without an
  explicit product/UX decision — three shells (Employee Portal, Agent
  Workspace, Admin Console) is a deliberate count, not a placeholder.

---

*Next: `docs/ai/ai-architecture.md` (Phase 46).*
