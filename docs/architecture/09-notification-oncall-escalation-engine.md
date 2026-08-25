# 09 — Notification, On-Call & Escalation Engine

**Status:** Design baseline, informed by xMatters' published architecture (help.xmatters.com, xmatters.com/trust). Changes require an ADR under `docs/adr/`.

**Applies to:** the platform Notifications capability (master guide
[§7.1](../ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md#71-capabilities)) and Major
Incident Management. This is the module that turns "an alert exists" into
"the right human, on the right device, in the right order, with automatic
escalation if nobody responds" — the piece ServiceNow's platform treats as a
first-class notification/on-call surface and xMatters treats as its entire
product.

**Relates to:** [`01-security-architecture.md`](01-security-architecture.md)
(security model reused, not replaced), master guide
[§4.4](../ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md#44-when-we-do-extract-a-real-microservice)
(Notification service is a named satellite-extraction candidate), and Wave 4
(Major Incident) / Wave 5 (Ops, integrations) in the wave plan.

---

## 1. Why a dedicated engine, not "just send an email"

ITSM notification requirements outgrow a simple mailer fast: an incident
needs to reach *whoever is on duty right now*, on *whatever device they'll
actually see it on*, and if they don't act, escalate automatically without a
human babysitting it. xMatters' entire product is built around this loop —
**Group → Shift → Escalation Timeline → Device fan-out → Response → (if no
response) escalate**. We adopt the same core loop rather than inventing a
weaker one, because these rules are already battle-tested against real
on-call failure modes (someone's phone is off, someone's on PTO, the primary
never acknowledges).

---

## 2. Core domain concepts

| Concept | Definition | ServiceNow/xMatters analog |
|---|---|---|
| **Recipient** | A user or a **Group** — notifications always target a recipient, never a raw phone number | xMatters Group/User |
| **Group** | A named set of people sharing responsibility (e.g. "Network On-Call") | xMatters Group |
| **Shift** | A time window defining who is on duty within a Group; a Group can have a default always-on shift or multiple named shifts | xMatters Shift |
| **Rotation** | A rule that periodically advances which member occupies each position in a shift's timeline, distributing primary-responder load evenly | xMatters Rotation |
| **Escalation Timeline** | Ordered list of timeline entries (members, sub-groups, or **escalation delays**) defining who is notified and in what sequence within a shift | xMatters Escalation |
| **Escalation Delay** | A wait period placed between timeline entries; delivery pauses until it elapses, then resumes if the alert is still unacknowledged | xMatters Escalation Delay |
| **Device** | A channel endpoint for a user: SMS, voice call, push (mobile app), email, Teams/Slack — each user can register multiple, ordered by preference | xMatters Device |
| **Alert / Event** | A single notification instance sent to one or more recipients, carrying a payload (incident number, priority, summary) and tracking delivery + response state per recipient | xMatters Alert |
| **Communication Plan / Form** | A template defining the message content, available response choices, and which recipients/groups it can target — reusable across triggers | xMatters Form / Communication Plan |
| **Response Action** | A recipient's reply (Accept/Acknowledge/Decline/custom) that can trigger a workflow outcome (assign the incident, stop escalation, notify next) | xMatters Response |

### 2.1 Where this sits relative to the platform Task engine

- An Alert is **not** a Task. It is a delivery artifact, always caused by
  something else: a Task engine event (incident assigned, SLA about to
  breach), a workflow activity ("notify CAB"), a Major Incident declaration,
  or an inbound integration/webhook from monitoring.
- Alerts reference their originating record (`SourceTable`, `SourceId`,
  `CorrelationId`) so the activity stream on that Task shows notification
  history, but the Notification engine owns its own delivery-state schema —
  it must not be modeled as more Task subtypes.

---

## 3. Escalation engine rules (locked, mirrors xMatters' proven model)

These are explicit **validation rules** on an Escalation Timeline definition
— reproduced from xMatters' documented behavior because they encode real
operational lessons, not just their product opinion:

1. **No terminal delay.** A timeline cannot end on an escalation delay — the
   last entry must be a notifiable recipient, otherwise an unacknowledged
   alert silently goes nowhere.
2. **No consecutive delays.** Two escalation delays cannot be adjacent;
   collapse them into one at validation time.
3. **A timeline may start with a delay** (e.g. "wait 2 minutes before paging
   anyone" for low-severity alerts).
4. **Repeat escalation** is configurable: after the timeline completes with
   no acknowledgment, repeat up to **9** times, with a configurable wait of
   **1–60 minutes** between repeats.
5. **End-of-sequence fallback.** If every repeat completes unacknowledged, an
   optional fallback notifies a designated user/group with the full alert
   context and a link — this is the "nobody answered the page" safety net
   and must never be silently dropped.
6. **Inactive-member skip.** A member who is inactive or has an unfilled
   temporary-absence gap is skipped automatically — the engine immediately
   notifies the next timeline entry and does not stall waiting for someone
   who isn't there. This is a fail-safe, not a fail-closed case (unlike
   security decisions in `01-security-architecture.md` — availability of the
   page matters more here than strict sequencing).
7. **Shift size limit.** Escalation and rotation logic is validated and
   supported for shifts of **≤200 members**; larger pools must be split into
   sub-groups. This is a modeling constraint to keep rotation math and UI
   comprehensible, not an arbitrary product cap — carry it forward.
8. **Device-timeout vs. shift-delay interaction.** If a single recipient's
   own per-device retry sequence (e.g. SMS → wait → call) takes longer than
   the shift's escalation delay, the shift can advance to the next member
   before that recipient's device fan-out finishes. This is intentional
   (speed of escalation wins) but must be visible in the UI so admins tune
   delays deliberately instead of being surprised by it.
9. **Escalation category is metadata only.** Tagging a step "Peer" or
   "Management" is for reporting/analytics and must **not** gate functional
   behavior — don't let a future engineer wire business logic to it.

---

## 4. Multi-channel delivery (device fan-out)

- A user's registered devices are ordered by personal preference (e.g.
  push → SMS → voice call → email), configurable per-user and overridable by
  alert priority (a P1 major incident may force voice-call-first regardless
  of preference).
- Each device attempt has its own **delivery status** (`Queued → Sent →
  Delivered → Failed`) and, for closed-loop channels (push, voice IVR, SMS
  reply), its own **response status** (`Acknowledged → Accepted / Declined`).
- **Closed-loop responses** update the Alert's aggregate state and can drive
  workflow outcomes (e.g. "Accepted" auto-assigns the Task to that user and
  halts further escalation for that alert).
- Delivery is **provider-abstracted**: SMS/voice/email/push go through a
  pluggable provider interface so a single vendor outage doesn't take down
  the whole channel — this is also the natural seam for provider failover
  (§7).

---

## 5. Integration architecture (inbound / outbound, closed-loop)

xMatters' pattern — Integration Builder + Flow/Workflow triggers + REST API,
inbound and outbound as symmetric halves of one closed loop — maps cleanly
onto this platform's existing integration fabric (master guide §7.1,
detailed further in `04-api-design.md`):

| xMatters concept | Our equivalent |
|---|---|
| Inbound integration (external webhook → creates an Alert) | Inbound webhook endpoint → transformation → Domain API call that creates an Alert/Task, same idempotent-inbound rule as any other integration (`01-security-architecture.md` §1.4, master guide principle 8) |
| Outbound integration (Alert/response state → external system) | Outbound webhook via the platform outbox pattern — never a synchronous call inside the request path |
| Flow Designer trigger | Workflow activity type `Notify` / `Escalate` inside the platform's own Workflow engine (`05-workflow-engine.md`) — not a second, parallel workflow system |
| REST API | Same Table/Domain API surface as the rest of the platform; Alerts and Groups are just more resources behind the same ACL/RLS wall |
| xMatters Agent (on-prem distributed integration runner) | Same role as the platform's later on-prem MID-analog agent (master guide Wave 5) |

**Design rule:** do **not** build a separate notification-specific
integration subsystem. Reuse the platform's one integration fabric, ACL
engine, and outbox — the only thing genuinely specific to this module is the
escalation/timeline domain model in §2–3.

---

## 6. Security model (inherits `01-security-architecture.md`)

No new security primitives — this module is a consumer of the platform's
existing controls, with module-specific application:

- **Tenant isolation:** Groups, Shifts, Devices, and Alerts are tenant-scoped
  rows like everything else; RLS applies identically.
- **ACL:** editing a Group's escalation timeline requires an explicit ACL
  grant (e.g. `group_admin` role) — matches xMatters' documented rule that
  only registered/authorized users may edit integrations or timelines,
  everyone else gets read-only.
- **PII on devices:** phone numbers and personal device identifiers are
  `Confidential`-classified fields (`01-security-architecture.md` §4.2) —
  masked in exports, never sent to AI providers.
- **MFA & SAML:** device-management screens (adding/removing a personal
  phone number) require a re-authenticated, MFA-verified session, matching
  xMatters' documented posture that systems touching client data require
  MFA + SAML federation.
- **Audit:** every escalation step, delivery attempt, and response is an
  audit-stream entry — this is also an operational necessity (a customer
  will ask "why didn't I get paged," and the audit trail is the answer, not
  a re-read of production logs).

---

## 7. Reliability & availability requirements

Notification delivery is the one platform capability where **being down is
itself the incident** — an ITSM tool that can't page anyone during an outage
has failed at its core job. Requirements, mirroring xMatters' SSDLC/trust
posture (SOC 2, redundant delivery):

- **Provider redundancy per channel.** At least two providers configured per
  channel type (SMS, voice, email) in production tenants above a size
  threshold, with automatic failover on provider error/timeout.
- **Delivery is asynchronous and durable.** Alert creation writes to the
  outbox before any provider call; a worker crash mid-delivery must resume,
  never silently drop, an alert.
- **Timeout and dead-letter handling.** A provider call that never responds
  must not stall the escalation timeline — a hard timeout forces the engine
  to treat that device attempt as failed and proceed.
- **Degraded-mode delivery.** If the primary notification worker pool is
  unhealthy, alerts still queue durably and drain once workers recover —
  never accept-then-lose.
- **SLOs:** define and monitor a delivery-latency SLO per channel (e.g. "95%
  of push notifications delivered within 10s of Alert creation") from the
  wave this module ships — see `07-devops-observability.md`.

---

## 8. Sketch data model

```
Group (TenantId, Name, DefaultShiftId)
Shift (TenantId, GroupId, Name, ScheduleDefinition, IsDefault)
ShiftMember (ShiftId, UserOrGroupId, TimelinePosition, InRotation)
EscalationTimeline (ShiftId, OrderedEntries: [Member | Delay(seconds) | RepeatConfig])
Device (UserId, Type[SMS|Voice|Push|Email|Chat], Address, PreferenceOrder, IsActive)
CommunicationPlan / Form (TenantId, Name, MessageTemplate, ResponseChoices[])
Alert (TenantId, SourceTable, SourceId, CorrelationId, CommunicationPlanId, Priority, State, CreatedAt)
AlertRecipient (AlertId, RecipientUserId, EscalationStepIndex, State[Pending|Notified|Acknowledged|Escalated])
DeliveryAttempt (AlertRecipientId, DeviceId, Provider, Status, SentAt, RespondedAt, ResponseValue)
```

All rows carry `TenantId` and are subject to the same RLS/ACL wall as every
other table in the platform (`02-tenancy-and-data-isolation.md`).

---

## 9. Checklist (maps to master guide waves)

**Wave 1 (platform kernel):**
- [ ] Basic in-app + email notification delivery for workflow events exists (already scoped in master guide Wave 1/2) — this is the minimum this module must support before anything richer.

**Wave 4 (Major Incident, per master guide):**
- [ ] Groups, Shifts, and a single-level Escalation Timeline (no repeats yet) support paging a Major Incident's response team.
- [ ] Closed-loop Accept/Decline via at least one channel (push or SMS) updates Task assignment.

**Wave 5 (ops/integrations, notification service extraction candidate per §4.4):**
- [ ] Full escalation rules (§3) implemented and validated (no terminal/consecutive delay, repeat up to 9x, inactive-member skip, 200-member shift limit).
- [ ] Rotations implemented and tested for even distribution.
- [ ] Multi-provider failover per channel (§7) proven in a chaos test (provider outage does not stop delivery).
- [ ] Inbound/outbound integrations for this module reuse the platform integration fabric (§5) — no parallel system.
- [ ] Extracted to a dedicated `Host.Workers`-adjacent notification service **only if** volume/provider-isolation metrics justify it (master guide §4.4) — not by default.

**Ongoing:**
- [ ] Delivery-latency SLOs monitored per channel (§7).
- [ ] Device PII handled per `01-security-architecture.md` §4.2 classification rules.

---

## 10. References

- [xMatters Help Center — Escalations](https://help.xmatters.com/ondemand/groups/groups-escalations.htm)
- [xMatters Help Center — Manage on-call schedules](https://help.xmatters.com/ondemand/groups/manage-on-call-schedules.htm)
- [xMatters Help Center — Rotations](https://help.xmatters.com/ondemand/groups/groups-rotations.htm)
- [xMatters Help Center — Integration Builder](https://help.xmatters.com/ondemand/integrationbuilder/build-integrations.htm)
- [xMatters Trust Center — Organizational Security](https://www.xmatters.com/trust/security/organizational-security)
- [xMatters Trust Center — SOC 3](https://www.xmatters.com/trust/security/soc-3)
- [Master guide §7.1 Platform capabilities](../ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md#71-capabilities)
- [Master guide §4.4 When we do extract a real microservice](../ENTERPRISE_ITSM_IMPLEMENTATION_GUIDE.md#44-when-we-do-extract-a-real-microservice)
- [`01-security-architecture.md`](01-security-architecture.md) — security model this module inherits

---

*Note: `support.xmatters.com` (Zendesk-hosted help center) returns HTTP 403 to automated fetches; the content above was sourced from `help.xmatters.com` (the primary xMatters product documentation site) and `xmatters.com/trust`, which cover the same material.*
