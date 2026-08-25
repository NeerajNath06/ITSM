# Domain: Problem Management

**Phase:** 40 · MVP scope.

| | |
|---|---|
| **Purpose** | Identify and eliminate root causes of recurring or high-impact incidents, preventing recurrence. |
| **Business Capability** | Root Cause Elimination |
| **Actors** | Problem Manager, Subject Matter Expert, Change Requester (for the eventual fix) |

**Use cases:** open a Problem from a pattern of related Incidents;
investigate root cause; document a workaround for immediate relief; publish
a Known Error; raise a Change to implement the permanent fix; close once
verified.

**Entities/Aggregates:** `Problem` (extends `TaskBase`), `KnownError` (flag
+ published workaround text on `Problem`).

**Commands:** `CreateProblem`, `LinkIncidentToProblem`, `PublishKnownError`,
`RaiseChangeFromProblem`, `CloseProblem`.

**Queries:** `GetProblemById`, `ListOpenProblems`, `SearchKnownErrors`.

**Events:** `ProblemCreated`, `KnownErrorPublished`, `ProblemClosed`.

**APIs:** `POST /api/v1/problems`, `POST /api/v1/problems/{id}/known-error`,
`POST /api/v1/problems/{id}/link-incident`.

**Database tables:** `task`, `problem`, `task_relationship` (Problem→Incident
links, many per Problem).

**Security:** Standard tenant ACL/RLS; Known Error visibility may be broader
than raw Problem detail (published workaround is agent-visible, root-cause
detail may remain restricted).

**Audit:** State changes, Known Error publish events (visible to agents —
publish is a meaningful business action worth its own audit trail entry
distinct from a field update).

**Workflow:** Known Error publish approval (reuses shared `Approval`
activity); Change-raise handoff.

**Notifications:** Notify linked-incident watchers when a Known Error/
workaround is published (self-service deflection value).

**Dependencies:** Incident (many-to-one link, does not own Incidents),
Change (raises, does not own), Knowledge (a published Known Error may also
become a Knowledge article).

**Failure scenarios:** Problem never closed because "fix" Change stalls
indefinitely — mitigated by SLA-style aging alerts on Problem backlog age,
not a hard system-enforced deadline (root-cause work is legitimately
variable-duration).

**KPIs:** Problem backlog age, recurrence rate of incidents linked to a
closed Problem (regression signal), Known Error deflection count.

---

*Next: `docs/domains/change-management.md`.*
