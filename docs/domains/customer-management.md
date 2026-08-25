# Domain: Customer Management

**Phase:** 40 · **Out of MVP scope** — documented for pattern completeness.

| | |
|---|---|
| **Purpose** | Represent external customer accounts/contacts and their entitlements, supporting CSM-family products. |
| **Business Capability** | Customer & Entitlement Master Data |
| **Actors** | Account Manager, CSM Agent |

**Entities/Aggregates:** `Account`, `Contact`, `Entitlement` (references a
`Contract` and a `ServiceOffering`, per `csdm-inspired-service-model.md`).

**Commands/Queries/Events:** `CreateAccount`, `GrantEntitlement`;
`GetAccountById`, `CheckEntitlement`; `AccountCreated`,
`EntitlementGranted`.

**APIs:** `POST /api/v1/customers/accounts`, `GET
/api/v1/customers/{id}/entitlements`.

**Database tables:** `customer_account`, `customer_contact`, `entitlement`.

**Security:** Customer PII is `Confidential`-classified; account data is
tenant-scoped like everything else, plus potentially account-team-scoped
within a tenant for large B2B customers (a finer ACL condition, not a new
isolation mechanism).

**Audit:** Entitlement grants/revocations (billing-adjacent, compliance-relevant).

**Workflow:** entitlement approval reuses the shared `Approval` activity.

**Dependencies:** Contract (Vendor/Contract domain), Case Management
(consumer of entitlement checks).

**Failure scenarios:** entitlement check against stale contract data lets
an expired customer open cases — mitigated by entitlement checks querying
live contract status, not a cached "is a customer" boolean.

**KPIs:** Entitlement utilization, account health score (future — depends
on Case/CSAT data existing).

**Build trigger:** only alongside a future CSM product line, per
`docs/roadmap/implementation-roadmap.md` Phase 9 (Customer).

---

*Next: `docs/domains/employee-management.md`.*
