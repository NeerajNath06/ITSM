# Domain: Change Management (Change Enablement)

**Phase:** 40 · MVP scope.

| | |
|---|---|
| **Purpose** | Control the risk of intentional infrastructure/application changes while enabling change velocity. |
| **Business Capability** | Change Risk Governance |
| **Actors** | Change Requester, Change Manager, CAB (Change Advisory Board), Implementer |

**Use cases:** submit a Standard/Normal/Emergency change; risk-assess
against CMDB impact graph and blackout calendar; route through CAB approval
(Normal) or fast-track (Standard, pre-approved template; Emergency,
post-hoc CAB review); implement in the scheduled window; record outcome;
review.

**Entities/Aggregates:** `Change` (extends `TaskBase`): `ChangeType`
(Standard/Normal/Emergency), `Risk`, `CabDecision` (composed), `Window`
(start/end), `BackoutPlan`.

**Commands:** `CreateChange`, `AssessChangeRisk`, `SubmitForCabApproval`,
`ScheduleChange`, `ImplementChange`, `ReviewChange`, `RecordUnauthorizedChange`.

**Queries:** `GetChangeById`, `ListUpcomingChanges`, `CheckWindowConflicts`.

**Events:** `ChangeSubmitted`, `ChangeApproved`, `ChangeImplemented`,
`ChangeFailed`, `UnauthorizedChangeDetected`.

**APIs:** `POST /api/v1/changes`, `POST /api/v1/changes/{id}/assess-risk`,
`POST /api/v1/changes/{id}/implement`.

**Database tables:** `task`, `change`, `cab_decision`,
`change_conflict_check_result`.

**Security:** **Segregation of duties enforced in ACL**: the requester
cannot be the sole CAB approver for a Normal change on their own request —
modeled as an authorization policy condition, not a UI-only warning.

**Audit:** Every state transition, every CAB decision, every emergency
change (highest scrutiny — audited with explicit justification field
required) — compliance-critical per the earlier security architecture.

**Workflow:** CAB approval flow (shared `Approval` activity, potentially
multi-approver); risk-based routing (Standard skips CAB, Emergency routes to
expedited post-implementation review).

**Notifications:** CAB approval requests, implementation window reminders,
unauthorized-change alerts to Change Manager.

**Dependencies:** CMDB (impact graph for risk scoring — Phase 10/11),
Calendar/blackout window data, DevOps pipeline integration (Phase 2 finding
— pipeline callbacks can create Standard Changes automatically, future
wave), Notification.

**Failure scenarios:** risk score computed from stale CMDB data
understates real impact — mitigated by CMDB data-quality rules and by never
treating the computed risk score as the sole gate (CAB judgment remains in
the loop for Normal/Emergency changes). Two changes scheduled in
overlapping windows against the same CI without conflict detection catching
it — mitigated by `CheckWindowConflicts` being mandatory before scheduling,
not optional.

**KPIs:** Change success rate, unauthorized-change count, average CAB
cycle time, change-caused-incident rate.

---

*Next: `docs/domains/request-management.md`.*
