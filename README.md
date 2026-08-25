# ITSM

A ServiceNow-class enterprise service management platform (ITSM/ITAM
first) — see [`docs/MASTER-ARCHITECTURE.md`](docs/MASTER-ARCHITECTURE.md)
for the full architecture and [`CLAUDE.md`](CLAUDE.md) for the binding
rules any contributor (human or AI) follows.

**Status:** Wave 0 — foundation scaffolding. No product features yet.

## Stack

.NET 10 / ASP.NET Core, React 19 + TypeScript + Vite, PostgreSQL, self-hosted
OSS infrastructure (Valkey, RabbitMQ, MinIO, OpenSearch, Keycloak, OpenBao,
Temporal, Kubernetes/K3s) — see
[`docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`](docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md).

## Repository layout

See [`docs/architecture/repository-structure.md`](docs/architecture/repository-structure.md)
for the full, annotated layout and the reasoning behind it.

```
src/            .NET solution (Host.Api, Host.Workers, Platform kernel, BuildingBlocks)
apps/           React workspace (employee-portal, agent-workspace, admin-console, design-system)
tests/          Architecture, Unit, Integration, Isolation, Contract, E2E
deploy/         docker-compose.yml (local dev), IaC (OpenTofu), CI
docs/           The complete architecture/rules documentation program
```

## Local development

```bash
# 1. Start dependencies (PostgreSQL, Valkey, RabbitMQ, MinIO, OpenSearch,
#    Keycloak, OpenBao, Mailpit, Temporal)
docker compose -f deploy/docker-compose.yml up -d

# 2. Backend
dotnet build ITSM.slnx
dotnet test ITSM.slnx

# 3. Frontend
cd apps
npm install
npm run build --workspace=@itsm/design-system   # build the shared library first
npm run dev --workspace=@itsm/employee-portal    # or agent-workspace / admin-console
```

## Where to start

- Building something? Read [`CLAUDE.md`](CLAUDE.md) first — it's the
  binding entry point and links to every relevant document.
- Understanding the architecture? Start at
  [`docs/MASTER-ARCHITECTURE.md`](docs/MASTER-ARCHITECTURE.md).
- What's built vs. planned? See
  [`docs/roadmap/implementation-roadmap.md`](docs/roadmap/implementation-roadmap.md)
  and [`docs/PROGRAM-STATUS.md`](docs/PROGRAM-STATUS.md).
