# Domain: Employee Management

**Phase:** 40 · **Out of MVP scope** — documented for pattern completeness.
Highest PII sensitivity of any domain in this program if built.

| | |
|---|---|
| **Purpose** | Represent the employee profile superset needed for HR Service Delivery cases (compensation-adjacent, lifecycle events) beyond the base `identity_user` record. |
| **Business Capability** | Employee Master Data (HR-specific) |
| **Actors** | HR Agent, Employee (self-service), Manager |

**Entities/Aggregates:** `EmployeeProfile` (relates-to `identity_user`, not
a duplicate identity — per the cross-context rule that Identity owns the
user, this domain owns the HR-specific extension only), `LifecycleEvent`
(hire/transfer/leave/termination).

**Commands/Queries/Events:** `RecordLifecycleEvent`, `UpdateEmployeeProfile`;
`GetEmployeeProfile`; `EmployeeLifecycleEventRecorded`.

**APIs:** `GET /api/v1/hr/employees/{id}` — **must not** be reachable by
general ITSM agent roles; a distinct, isolated ACL domain (mirrors the
Security Incident isolation pattern from Phase 2/5).

**Database tables:** `employee_profile`, `lifecycle_event` — physically
separated schema/access path from general Identity tables given the
sensitivity difference.

**Security:** Field-level `Restricted` classification on compensation/
health-adjacent fields; access is the single strictest ACL domain in the
platform if this is ever built — explicitly **not** inheriting general
agent-role visibility even by accident.

**Audit:** Every read of `Restricted` fields is itself audited (not just
writes) given the sensitivity.

**Workflow:** onboarding/offboarding checklists as Workflow Definitions.

**Dependencies:** Identity (relates-to `identity_user`), Case Management
(HR case family).

**Failure scenarios:** an HR case accidentally surfaces compensation data
to a general ITSM agent via a shared Task list view — the concrete reason
this domain requires an isolated ACL domain, not a shared one, from day
one of design.

**KPIs:** Case resolution time (HR case family), employee satisfaction
(eNPS-style, if surveyed).

**Build trigger:** only alongside a future HRSD product line
(`implementation-roadmap.md` Phase 10, Employee) — and only after a
dedicated security review given the PII sensitivity.

---

*Next: `docs/domains/security.md`.*
