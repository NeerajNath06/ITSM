# Testing Strategy

**Phase:** 38

## Test layers

```
Unit Tests
Integration Tests
Contract Tests
API Tests
Component Tests
E2E Tests
Performance Tests
Security Tests
Chaos Tests
```

| Layer | What it proves | Tooling (indicative) | Runs |
|---|---|---|---|
| **Unit** | `Domain` logic (state machines, validation) and `Application` handlers in isolation, no infrastructure | xUnit + FluentAssertions, no DB/network | Every PR |
| **Integration** | EF Core mappings, RLS policies actually block cross-tenant reads, ACL evaluation against a real PostgreSQL instance | xUnit + Testcontainers (PostgreSQL, Valkey, RabbitMQ) | Every PR |
| **Contract** | Public REST/gRPC contracts haven't broken a consumer | OpenAPI diff gate; Pact-style consumer-driven contracts for internal gRPC | Every PR touching an API surface |
| **API** | End-to-end request behavior against a running `Host.Api` (auth, ACL, pagination, error shapes) | Integration test host (`WebApplicationFactory`) | Every PR |
| **Component (React)** | Individual component/feature behavior in isolation | React Testing Library + Vitest | Every PR |
| **E2E** | Golden paths across React + backend together (login → create incident → resolve → survey) | Playwright | Every PR (smoke subset) + nightly (full suite) |
| **Performance/Load** | Target class load (see below) doesn't regress latency/error rate | k6 or equivalent | Nightly + pre-release |
| **Security** | SAST, dependency scan, container scan, DAST on staging | Integrated scanners (Phase 36) + periodic pentest | Every PR (SAST/dep/container) + scheduled (DAST/pentest) |
| **Chaos** | Bus down, PostgreSQL failover, worker poison messages don't cascade | Manual/scripted fault injection in `staging` | Pre-release, and quarterly game days |

## The one test suite every phase of this program depends on: Isolation

- **Two tenants, identical business record numbers, prove zero leakage**
  across: Table/Domain API responses, search results, blob/attachment
  access, background job side effects, and (once built) AI/RAG retrieval.
- This suite is **not optional and not deferred** — it gates Wave/Phase
  progress the same way the original master guide treated it: no new
  product surface ships without the isolation suite covering its new
  tables.

## ACL matrix testing

- A generated matrix (role × table × operation × field) is tested against
  expected allow/deny outcomes — generated from the same metadata the ACL
  engine evaluates at runtime, so the test suite and the production rule
  set can never silently drift apart.

## Performance/load targets

- Baseline target class (revisit as real usage data arrives):
  ~5,000 concurrent agents / ~100,000 employees across the platform,
  SLA-timer evaluation under simulated backlog, search under load.
- Every extracted satellite service (Notification, Search) has its own
  load target reflecting its actual traffic shape (e.g. Notification: alert
  storm burst handling, not steady-state throughput).

## Minimum CI gates (maps onto the implementation roadmap's phases, Phase 43)

| Program stage | Must be green in CI |
|---|---|
| Foundation | Build, unit tests, SAST, architecture tests (NetArchTest-equivalent: Domain ↛ Infrastructure, no illegal cross-module Infrastructure reference) |
| Platform kernel (Identity/Tenancy/ACL/Audit) | Isolation suite, ACL matrix suite |
| Experience shells | React component tests, a11y automated checks (WCAG 2.2 AA) on critical paths |
| Service desk (Incident/Request/Knowledge) | Playwright golden paths, SLA integration tests |
| CMDB/Change | CMDB impact-graph tests, change-conflict-detection tests |
| Integrations/Notification | Contract tests, webhook/outbox idempotency tests, notification escalation-rule tests (per `09-notification-oncall-escalation-engine.md` §3) |
| AI (Phase 46/47) | Cross-tenant RAG adversarial tests, ACL-respecting-output tests |

## Regression discipline

- Every bug fix ships with a test reproducing the bug, failing before the
  fix and passing after, in the same PR (`docs/rules/coding-rules.md`).
- Flaky tests are quarantined with a tracked ticket, not silently
  `[Skip]`'d and forgotten — quarantine has an expiry; an un-fixed flaky
  test past its expiry blocks the pipeline it's quarantined from.

---

*Next: `docs/devops/cicd.md` (Phase 39).*
