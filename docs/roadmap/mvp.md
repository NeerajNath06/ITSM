# MVP Definition

**Phase:** 44 · The smallest production-capable platform — not the entire
ServiceNow-equivalent ecosystem.

## MVP scope

```
Identity
Organization / Tenant
RBAC (+ ACL)
Audit
Notification (basic: email + in-app, Group/Shift/Escalation)
Task (base)
Workflow (+ Business Rules)
Service Catalog
Knowledge
Incident
Request
Change
CMDB (foundation: manual + basic Discovery, no full Service Mapping)
Observability
Integration (basic inbound/outbound, webhook)
```

Corresponds to Implementation Roadmap Phases 1–6
(`implementation-roadmap.md`).

## Why these, and only these, come first

| Included | Why it can't wait |
|---|---|
| Identity, Tenant, RBAC/ACL, Audit | Every other capability's data is meaningless/unsafe without tenant isolation and authorization already proven — this is the foundation everything else is built on, not a feature to add later |
| Notification (basic) | Workflow approvals and Incident assignment are non-functional without a way to reach a human — even the MVP needs at least email + in-app |
| Task | The shared base every operational record (Incident/Change/Request) extends — must exist before any of them can |
| Workflow + Business Rules | Approval and fulfillment are core to Request and Change from day one, not an enhancement |
| Service Catalog + Knowledge | Both are platform capabilities consumed immediately by Request (catalog) and by agents/employees (knowledge) — deferring either cripples the products that need them |
| Incident, Request, Change | The three highest-value, most universally recognized ITSM capabilities — the smallest set that makes the product usable as "an ITSM tool," per the original guide's Year-1 ITSM surface reasoning, re-derived independently here |
| CMDB (foundation) | Change risk assessment and Incident impact context are materially weaker without it — but full Discovery/Service Mapping depth is explicitly deferred (see below) |
| Observability | Cannot safely operate any of the above in production without it — not deferrable |
| Integration (basic) | Real ITSM deployments need at minimum an inbound webhook path (monitoring tool → Incident) to be useful on day one |

## Explicitly deferred past MVP

- **Problem Management** — valuable, but Incident/Request/Change alone
  constitute a viable, sellable ITSM product; Problem is the next addition,
  not a blocker to first production tenants.
- **Full CMDB** (Discovery at scale, Service Mapping, data-quality
  automation depth) — foundation ships in MVP, full depth is Roadmap Phase
  6–7.
- **Asset Management (ITAM)** — light linkage only if time permits; full
  lifecycle is Roadmap Phase 7.
- **Case Management, Customer Management, Employee Management, Security,
  Risk, Compliance, Project, Portfolio, Vendor, Contract** — all documented
  for pattern completeness (`docs/domains/`) but not built until Roadmap
  Phases 9–11, and only as deliberate product decisions with their own
  business case.
- **AI/Virtual Agent** — Roadmap Phase 11, after real production data and a
  proven ACL/tenancy model exist to ground it safely.
- **Full ITOM/Event Management/AIOps** — foundation ingest only in MVP+1.

## MVP exit criteria

- [ ] Isolation test suite green (two tenants, zero leakage, per
      `docs/testing/testing-strategy.md`).
- [ ] ACL matrix suite green for every MVP table.
- [ ] Playwright golden path: employee opens Incident or Catalog Request →
      agent resolves → CSAT captured.
- [ ] SLA clocks start/pause/breach correctly under test calendars.
- [ ] Change risk assessment uses real CMDB impact data (not stubbed).
- [ ] External penetration test passed before first paying production
      tenant (Roadmap Phase 8 gate, pulled forward as an MVP-exit
      requirement given the security stakes of a multi-tenant platform).

---

*Next: `docs/architecture/repository-structure.md` (Phase 45).*
