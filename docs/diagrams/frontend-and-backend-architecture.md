# Diagrams: React & .NET Architecture

**Phase:** 42 · Diagrams 19–20 (completes the required diagram set).
Frontend framework per `docs/adr/ADR-021-react-over-angular.md`.

## 19. React architecture

```mermaid
flowchart TD
  subgraph Apps["Three React applications"]
    EMP[employee-portal]
    AGT[agent-workspace]
    ADM[admin-console]
  end
  DS[design-system library] --> EMP & AGT & ADM
  subgraph FeatureShape["Per feature (repeated in every app)"]
    ROUTES[feature routes - React.lazy + Suspense]
    DA[data-access: API client + TanStack Query hooks]
    UI[ui-* presentational components]
    SMART[feature-* container components / hooks]
  end
  EMP -.-> FeatureShape
  BFF[BFF - Host.Api] --> DA
```
Source: `docs/frontend/react-architecture.md` (Phase 30).

## 20. .NET architecture (per-module Clean Architecture)

```mermaid
flowchart LR
  API[Api: minimal API endpoints] --> APP[Application: Commands/Queries/Handlers/Validators]
  INFRA[Infrastructure: EF Core, external clients] --> APP
  APP --> DOMAIN[Domain: entities, value objects, domain events]
  API -.depends on.-> APP
  INFRA -.depends on.-> APP
  APP -.depends on.-> DOMAIN
  DOMAIN -.no outward dependency.-> DOMAIN
```
Source: `docs/backend/dotnet-architecture.md` (Phase 31).

---

*This completes the required 20-diagram set (Phase 42). Index of all
diagram files:*

| # | Diagram | File |
|---|---|---|
| 1–2 | Enterprise, Platform | `enterprise-and-platform-architecture.md` |
| 3–4 | Domain, Microservices | `domain-and-microservices-architecture.md` |
| 5, 13, 14 | Database, CMDB, CSDM | `database-and-cmdb-architecture.md` |
| 6–7 | AuthN, AuthZ | `security-and-auth-flows.md` |
| 8–12 | Request, Event, Workflow, Audit, Notification | `lifecycle-flows.md` |
| 15–18 | CI/CD, Self-Hosted Infra, DR, Observability | `cicd-and-infrastructure.md` |
| 19–20 | React, .NET | this file |

*Next: `docs/roadmap/implementation-roadmap.md` (Phase 43).*
