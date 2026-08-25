# Diagrams: Request, Event, Workflow, Audit, Notification Lifecycles

**Phase:** 42 · Diagrams 8–12.

## 8. Request lifecycle (a single API call, full path)

```mermaid
sequenceDiagram
    participant U as User
    participant CH as Channel/BFF
    participant ACL as ACL Engine
    participant BR as Business Rules
    participant TBL as Table
    participant AUD as Audit
    participant EVT as Event Bus

    U->>CH: Mutating request
    CH->>ACL: Authorize
    ACL-->>CH: Allow
    CH->>BR: Validate/default
    BR->>TBL: Persist
    TBL->>AUD: Write audit entry (same transaction)
    TBL->>EVT: Emit event (via outbox, after commit)
```
Source: `docs/architecture/servicenow-layered-architecture.md` (Phase 3),
applied to our stack in `docs/api/api-design-rules.md` (Phase 16).

## 9. Event lifecycle (outbox → RabbitMQ → satellite)

```mermaid
flowchart LR
  TX[Business transaction commits] --> OUTBOX[Outbox row written, same transaction]
  OUTBOX --> RELAY[Outbox relay worker polls]
  RELAY --> MQ[RabbitMQ topic exchange]
  MQ --> CONSUMER[Satellite consumer]
  CONSUMER --> INBOX[Inbox check: already processed?]
  INBOX -->|yes| SKIP[Skip - idempotent]
  INBOX -->|no| ACT[Process + record in inbox]
  ACT -->|failure, retries exhausted| DLQ[Dead Letter Queue]
```
Source: `docs/integration/event-driven-architecture.md` (Phase 14).

## 10. Workflow execution

```mermaid
stateDiagram-v2
    [*] --> Pending
    Pending --> Running
    Running --> Completed
    Running --> Waiting: Approval/Wait activity
    Waiting --> Running: response/timer
    Running --> Failed: retries exhausted
    Failed --> Compensated
    Completed --> [*]
    Compensated --> [*]
```
Source: `docs/workflow/workflow-engine-architecture.md` (Phase 12).

## 11. Audit flow

```mermaid
flowchart LR
  CHANGE[Entity change tracked] --> INTERCEPTOR[SaveChanges interceptor]
  INTERCEPTOR --> BUILD[Build audit row: who/what/when/old/new]
  BUILD --> COMMIT[Same transaction commit: business change + audit row]
  COMMIT --> PARTITION[(audit_log, partitioned by month)]
```
Source: `docs/audit/audit-architecture.md` (Phase 19).

## 12. Notification flow

```mermaid
flowchart TD
  EVT[Triggering event] --> RESOLVE[Resolve Group/Shift/on-duty members]
  RESOLVE --> TIMELINE[Walk Escalation Timeline]
  TIMELINE --> FANOUT[Fan out per recipient device]
  FANOUT --> PROVIDER[Provider call - SMS/voice/push/email]
  PROVIDER -->|no ack within delay| ESCALATE[Escalate to next timeline entry]
  PROVIDER -->|acknowledged| DONE[Alert closed]
  ESCALATE --> TIMELINE
```
Source: `docs/architecture/09-notification-oncall-escalation-engine.md` §3,
`docs/platform/notification-service.md` (Phase 20).

---

*Next: `cicd-and-infrastructure.md`.*
