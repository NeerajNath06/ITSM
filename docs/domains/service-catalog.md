# Domain: Service Catalog

**Phase:** 40 · MVP scope. Schema/UX detail in `docs/platform/service-catalog.md`
(Phase 48); this document covers the domain profile.

| | |
|---|---|
| **Purpose** | Present a structured, browsable set of things employees can request, each with a defined fulfillment path. |
| **Business Capability** | Service Offering Presentation |
| **Actors** | Catalog Admin (author), Requester (browser/orderer) |

**Use cases:** define a catalog item with variables and a fulfillment
workflow; organize into categories; publish; requester browses/searches and
orders (handled by Request Management, `request-management.md`).

**Entities/Aggregates:** `CatalogItem`, `Category`, `VariableSet`
(reusable variable groups across items).

**Commands:** `CreateCatalogItem`, `PublishCatalogItem`, `RetireCatalogItem`.

**Queries:** `BrowseCatalog`, `SearchCatalogItems`, `GetCatalogItemById`.

**Events:** `CatalogItemPublished`, `CatalogItemRetired`.

**APIs:** `GET /api/v1/catalog/items`, `POST /api/v1/catalog/items` (admin).

**Database tables:** `catalog_item`, `category`, `variable_set`,
`catalog_item_variable`.

**Security:** Catalog visibility can be role/group-restricted per item
(e.g. an executive-only hardware tier) — same ACL mechanism as everything
else, applied at the catalog-browse query level (not filtered client-side).

**Audit:** Publish/retire actions, price/entitlement-affecting field changes.

**Workflow:** none owned directly — items *reference* a fulfillment
Workflow Definition, consumed by Request Management at checkout.

**Notifications:** none owned directly.

**Dependencies:** Request Management (consumer), Workflow (fulfillment
definitions referenced), Search (browse/find).

**Failure scenarios:** a published item references a Workflow Definition
that's later deactivated — checkout must fail loudly (validation at
publish-time and at checkout-time), never silently create an
unfulfillable Request.

**KPIs:** Catalog adoption rate, item order volume, time-to-publish for
new items.

---

*Next: `docs/domains/knowledge.md`.*
