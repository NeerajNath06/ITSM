# Domain: Portfolio

**Phase:** 40 · **Out of MVP scope** — documented for pattern completeness.

| | |
|---|---|
| **Purpose** | Aggregate and prioritize demand/investment across multiple Projects/Programs against strategic objectives. |
| **Business Capability** | Strategic Portfolio Management |
| **Actors** | Portfolio Manager, Executive Sponsor |

**Entities/Aggregates:** `Portfolio`, `Program`, `Demand`.

**Commands/Queries/Events:** `RegisterDemand`, `PrioritizeDemand`,
`AllocateToProgram`; `GetPortfolioView`; `DemandPrioritized`.

**APIs:** `/api/v1/portfolio/*`.

**Database tables:** `portfolio`, `program`, `demand`.

**Security:** Executive/leadership-role restricted typically; individual
Projects underneath may have broader team visibility (Project domain's own
ACL).

**Audit:** Prioritization decisions (governance-relevant).

**Workflow:** demand intake/approval reuses shared `Approval` activity.

**Dependencies:** Project (rolls up from), Business Capability (relates-to,
per the deferred CSDM layer, Phase 10 — only relevant if/when that layer is
built).

**Failure scenarios:** N/A at this design stage — genuinely deferred scope
with no implementation-specific risk to document yet.

**KPIs:** Portfolio ROI/value delivered, demand-to-delivery cycle time.

**Build trigger:** only alongside Project, as part of a future SPM/PPM
product line, and only once the Business Capability CSDM layer
(deferred in Phase 10) is actually needed.

---

*Next: `docs/domains/vendor.md`.*
