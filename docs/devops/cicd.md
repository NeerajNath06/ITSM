# CI/CD

**Phase:** 39 · **Jenkins** or **Gitea Actions** against a **Gitea** git
server, per `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`
(either CI runner satisfies these requirements — `ADR-022` flags the final
pick between the two as a follow-up ADR before Wave 0 scaffolding, to avoid
running two CI systems by default drift).

## Pipeline stages

```
Developer
   |
Git (PR opened)
   |
Build (.NET + React)
   |
Test (unit, integration, component)
   |
Security Scan (SAST, dependency, secret scan)
   |
Container Build
   |
Registry (Azure Container Registry, image scan)
   |
Deploy Dev (automatic, on merge to main)
   |
Integration Tests (against live dev, incl. isolation suite)
   |
Deploy QA/Staging (automatic, gated on integration tests green)
   |
Approval (manual gate — release owner)
   |
Production (progressive rollout)
```

## Stage detail

| Stage | Detail |
|---|---|
| **Build** | `.NET 10` solution build + React (`employee-portal`, `agent-workspace`, `admin-console`, `design-system`) build, in parallel jobs where independent |
| **Test** | Unit + integration (Testcontainers-backed) + React component tests; architecture tests (module boundary enforcement) run here too — a boundary violation fails the build exactly like a failing unit test |
| **Security Scan** | SAST (e.g. CodeQL/equivalent), NuGet + npm dependency vulnerability scan, secret scan (gitleaks-class) — all build-blocking on high/critical |
| **Container Build** | Multi-stage Dockerfiles per deployable (`Host.Api`, `Host.Workers`, each satellite service, each React app served via a lightweight static host) |
| **Registry** | Push to **Harbor**; **image vulnerability scan (Trivy, built into Harbor)** gates promotion past `dev` |
| **Deploy Dev** | Automatic on every merge to `main`; migrations applied automatically in `dev` only |
| **Integration Tests** | Run against the live `dev` deployment: full isolation suite, ACL matrix, contract tests — this is the real gate, not just unit-level integration tests |
| **Deploy QA/Staging** | Automatic once `dev` integration tests are green; migrations applied with a reviewed migration script for anything destructive |
| **Approval** | Manual gate — a named release owner approves promotion to production; not a rubber-stamp bot approval |
| **Production** | Progressive rollout (see below), migrations applied via a controlled, monitored step, feature flags used to decouple deploy from release where the change is risky |

## Progressive production rollout

- Kubernetes rolling deploy with a canary step (a small-percentage traffic
  split via the ingress/YARP routing weights, or an Argo Rollouts-style
  progressive delivery controller if adopted later) — new revision gets a
  small traffic percentage, monitored against the observability dashboards
  (Phase 24, now Grafana-backed) for error-rate/latency regression, then
  ramped or rolled back automatically on threshold breach.
- Database migrations are applied as a **separate, backward-compatible
  step** before the new code revision receives traffic (expand/contract
  migration pattern) — never a migration that only the new code version can
  tolerate, applied simultaneously with the code deploy.

## Environment parity

- `dev`/`staging`/`prod` run the same container images, the same
  **OpenTofu** Infrastructure-as-Code modules (parameterized per
  environment), and the same migration pipeline — configuration differs
  (Phase 28), code and infrastructure topology do not.

## Rollback

- A failed progressive rollout auto-rolls-back to the previous revision on
  the observability-driven threshold breach above.
- A migration is only considered "safe" for automatic rollback compatibility
  if it follows the expand/contract pattern (Phase 33's destructive-migration
  rule) — this is why that rule exists, not an independent nicety.

## Satellite services

- Each extracted service (Notification, Search, AI inference) has its own
  pipeline instance following the same stage structure, independently
  deployable per `docs/rules/microservice-rules.md`.

---

*Next: `docs/domains/` (Phase 40) — per-domain documentation.*
