# Domain: Case Management

**Phase:** 40 · **Out of MVP scope** — documented for pattern completeness,
per `docs/roadmap/mvp.md`. This is the generic base for CSM/HR-style
"case" products (Phase 2/8 finding: architecturally identical to Task).

| | |
|---|---|
| **Purpose** | Track an inbound inquiry/issue from an external customer or internal employee (non-IT-ticket) through resolution. |
| **Business Capability** | Case Intake & Resolution |
| **Actors** | Case agent, Customer/Employee (case opener) |

**Entities/Aggregates:** `Case` — **extends the same `task` root as
Incident/Problem/Change** (per `shared-enterprise-data-model.md`), not a
parallel hierarchy.

**Commands/Queries/Events:** `CreateCase`, `ResolveCase`; `GetCaseById`,
`ListMyCases`; `CaseCreated`, `CaseResolved` — same shape as Incident's,
substituting the entity name.

**APIs:** `POST /api/v1/cases`, generic Table API `/api/v1/table/case`.

**Database tables:** `task` (shared base — no new base table), `case`.

**Security:** Standard ACL/RLS; CSM cases may reference customer/contract
entitlement checks (Customer Management domain) not required for HR cases.

**Audit/Workflow/Notifications:** identical mechanics to Incident — no
domain-specific reimplementation.

**Dependencies:** Entitlement/Contract data (CSM variant), Knowledge.

**Failure scenarios:** building a second, parallel base table for Case
instead of extending `task` — explicitly the mistake this document exists
to prevent (see `shared-enterprise-data-model.md` §Case).

**KPIs:** CSAT, case resolution time.

**Build trigger:** revisit only after Waves/Phases delivering ITSM/ITAM are
stable in production (`docs/roadmap/mvp.md`, `implementation-roadmap.md`).

---

*Next: `docs/domains/customer-management.md`.*
