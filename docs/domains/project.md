# Domain: Project

**Phase:** 40 · **Out of MVP scope** — documented for pattern completeness.

| | |
|---|---|
| **Purpose** | Track delivery of a bounded body of work with a defined scope, timeline, and team. |
| **Business Capability** | Project Delivery Tracking |
| **Actors** | Project Manager, Team Member |

**Entities/Aggregates:** `Project`, `ProjectTask` (extends `task`),
`Milestone`.

**Commands/Queries/Events:** `CreateProject`, `AssignProjectTask`,
`CompleteMilestone`; `GetProjectStatus`; `MilestoneCompleted`.

**APIs:** `/api/v1/projects/*`.

**Database tables:** `project`, `task` (shared base for `ProjectTask`),
`milestone`.

**Security:** Standard tenant ACL/RLS, project-team-scoped visibility
(a finer ACL condition, not a new mechanism).

**Audit:** Milestone completion, scope changes.

**Workflow:** approval gates at milestone transitions reuse the shared
`Approval` activity.

**Dependencies:** Portfolio (a Project may roll up to a Portfolio/Program),
Change (a Project may trigger production Changes — relates-to, not
owns).

**Failure scenarios:** treating `ProjectTask` as unrelated to the shared
`task` base, duplicating assignment/state/journal logic — the same
duplication mistake this entire program's Task-hierarchy discipline exists
to prevent.

**KPIs:** On-time delivery %, milestone slippage, resource utilization.

**Build trigger:** only alongside a future Strategic/Project Portfolio
Management product line (`implementation-roadmap.md`, post-MVP).

---

*Next: `docs/domains/portfolio.md`.*
