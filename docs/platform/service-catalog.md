# Service Catalog

**Phase:** 48 · Full architecture (schema depth, variable system,
fulfillment integration) — domain profile lives in
`docs/domains/service-catalog.md` (Phase 40); this document is the detailed
platform-capability design.

## Structure

```
Category
 └── CatalogItem
      ├── VariableSet (reusable across items)
      │    └── Variable (type, label, validation, default)
      ├── Form layout (which variables, in what order/grouping)
      └── FulfillmentWorkflowDefinitionId (reference to Phase 12 Workflow)
```

## Data model

```sql
CREATE TABLE catalog_item (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    category_id UUID NOT NULL,
    name TEXT NOT NULL,
    description TEXT,
    status TEXT NOT NULL,                    -- Draft, Published, Retired
    fulfillment_workflow_id UUID NOT NULL,
    entitlement_role_ids UUID[],              -- which roles/groups may order this
    price NUMERIC(12,2),                      -- nullable; only if pricing required
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE variable_set (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    name TEXT NOT NULL
);

CREATE TABLE catalog_item_variable (
    id UUID PRIMARY KEY,
    catalog_item_id UUID REFERENCES catalog_item(id),
    variable_set_id UUID REFERENCES variable_set(id),  -- one of these two, not both
    name TEXT NOT NULL,
    label TEXT NOT NULL,
    variable_type TEXT NOT NULL,              -- String, Choice, Boolean, Reference, ...
    validation_rule JSONB,
    "order" INT NOT NULL
);
```

## Variables

- Class-schema-driven exactly like CMDB's CI attributes (Phase 11) —
  `variable_type` determines both the React form control rendered
  (via the shared metadata-driven form renderer, Phase 30) and the
  server-side validation applied at checkout.
- `Reference` type variables (e.g. "select the CI this software will run
  on") integrate with the metadata Table API's reference-field lookup
  mechanism, not a bespoke autocomplete per catalog item.

## Fulfillment

- Every `CatalogItem` references exactly one Workflow Definition —
  fulfillment logic (approval, provisioning steps, task assignment) lives
  entirely in Workflow (Phase 12), never hardcoded per catalog item in
  application code. Adding a new catalog item with a new fulfillment shape
  is a metadata + Workflow Definition change, not a code deploy.
- Common fulfillment subworkflows (e.g. "standard approval + provisioning
  ticket") are built once and referenced as Subworkflow steps
  (Phase 12 §Subworkflow) across many catalog items, avoiding duplicated
  workflow graphs.

## Entitlement

- `entitlement_role_ids` is checked server-side at both **browse time**
  (item hidden from users without entitlement — a UX nicety) and
  **checkout time** (hard-enforced — a UI-hidden item is not a security
  boundary on its own, matching the general rule that client-side hiding is
  never the authorization mechanism, Phase 30).

## SLA and pricing

- SLA Definitions attach at the `ServiceOffering` level
  (`csdm-inspired-service-model.md`), not the raw `CatalogItem` — a catalog
  item may represent multiple service offering tiers (e.g. "Standard
  Laptop" vs. "Premium Laptop" with different fulfillment SLAs), modeled as
  separate catalog items each referencing its own Service Offering, rather
  than SLA logic branching inside one item.
- Pricing (`price` column) is optional and populated only where the tenant
  needs cost visibility/chargeback — not required for pure IT service
  requests.

---

*Next: `docs/platform/sla-engine.md` (Phase 49).*
