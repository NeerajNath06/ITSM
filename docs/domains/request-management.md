# Domain: Request Management

**Phase:** 40 · MVP scope. Tightly paired with Service Catalog
(`service-catalog.md`) — this document covers the fulfillment side; the
catalog document covers what can be ordered.

| | |
|---|---|
| **Purpose** | Standardize how employees ask for something (access, hardware, software) and how it gets fulfilled. |
| **Business Capability** | Request Fulfillment |
| **Actors** | Requester, Approver, Fulfiller (agent or automated integration) |

**Use cases:** browse catalog, configure item variables, add to cart,
checkout (creates `Request` + one `RequestedItem` per line); approval if
required by the item's policy; fulfillment task(s) dispatched to the
appropriate group/integration; requester notified on completion.

**Entities/Aggregates:** `Request` (extends `TaskBase`, the "order"),
`RequestedItem` (extends `TaskBase`, one per catalog line, its own
fulfillment lifecycle), `Cart` (transient, pre-checkout only — not
persisted as a Task).

**Commands:** `AddToCart`, `Checkout`, `ApproveRequestedItem`,
`FulfillRequestedItem`, `CancelRequest`.

**Queries:** `GetRequestById`, `ListMyRequests`, `ListPendingApprovals`.

**Events:** `RequestSubmitted`, `RequestedItemApproved`,
`RequestedItemFulfilled`, `RequestCancelled`.

**APIs:** `POST /api/v1/catalog/cart/checkout` (Domain API — the real
business outcome endpoint, not a raw Table API insert), `POST
/api/v1/requested-items/{id}/fulfill`.

**Database tables:** `task`, `request`, `requested_item`, `cart_item`
(transient/session-scoped, not audited like a Task), variables stored as
JSONB on `requested_item` per the catalog item's variable schema.

**Security:** Standard ACL/RLS; entitlement check at checkout (can this
requester actually order this item, per their role/group) — enforced
server-side at `Checkout`, not just hidden in the UI.

**Audit:** Checkout, approval decisions, fulfillment completion.

**Workflow:** Approval (shared activity), fulfillment routing — often a
Subworkflow per catalog item type (hardware request subworkflow differs
from access-request subworkflow) composed under one parent Request
workflow.

**Notifications:** Approval requests, fulfillment status updates to
requester.

**Dependencies:** Service Catalog (item definitions), Workflow (approval +
fulfillment), Identity (entitlement checks), provisioning integrations
(future — Integration fabric).

**Failure scenarios:** checkout succeeds but downstream provisioning
integration fails silently — mitigated by the Workflow engine's
compensation mechanism (Phase 12) and by fulfillment status being visible
to the requester rather than assumed-complete on checkout.

**KPIs:** Cycle time (submit → fulfilled), catalog adoption rate,
approval turnaround time.

---

*Next: `docs/domains/case-management.md`.*
