# Domain: Compliance

**Phase:** 40 · **Out of MVP scope** — documented for pattern completeness.

| | |
|---|---|
| **Purpose** | Map organizational controls/policies to regulatory frameworks and track compliance status. |
| **Business Capability** | Regulatory Compliance Mapping |
| **Actors** | Compliance Officer, Control Owner (shared with Risk domain) |

**Entities/Aggregates:** `Policy`, `Control` (shared/relates-to Risk's
`Control`, not duplicated), `ComplianceTask` (extends `task`).

**Commands/Queries/Events:** `MapPolicyToControl`, `RecordComplianceTask`;
`GetComplianceStatus`; `ComplianceTaskCompleted`.

**APIs:** `/api/v1/compliance/*`.

**Database tables:** `policy`, `policy_control_link`, `compliance_task`
(extends `task`).

**Security:** Compliance/governance-role restricted, same tier as Risk.

**Audit:** Full audit — this domain's entire purpose is often to produce
audit evidence for external assessors, so its own audit trail must be
unimpeachable.

**Workflow:** compliance-task assignment/tracking reuses the standard
Workflow/Task machinery — no bespoke compliance workflow engine.

**Dependencies:** Risk (shared Control entity), Security (control evidence
may originate from Security Incident/Vulnerability data).

**Failure scenarios:** Policy↔Control mapping drifts from the Risk
domain's Control definitions because they were modeled as separate,
duplicated entities — explicitly prevented by relating to, not duplicating,
Risk's `Control` (same generic-relationship discipline used everywhere else
in this program).

**KPIs:** Compliance coverage %, overdue compliance tasks, audit finding
count trend.

**Build trigger:** only alongside Risk, as part of a future GRC product
line.

---

*Next: `docs/domains/project.md`.*
