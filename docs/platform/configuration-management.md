# Configuration Architecture

**Phase:** 28 · Where each kind of "settable thing" belongs — the single
most common source of architectural drift if left implicit.

## The separation

```
Code                  →  Compiled, versioned in Git, deployed via CI/CD
Configuration         →  Environment-scoped settings (non-secret), Kubernetes ConfigMaps
Secrets                →  OpenBao, referenced (not embedded) by configuration
Feature Flags          →  See feature-flags.md (Phase 29) — a distinct concept from static config
Tenant Configuration   →  Database rows, owned by the Tenant bounded context, editable via Admin console
Business Rules         →  Code (logic) + metadata (thresholds/parameters) — see business-rule-engine.md
Workflow Definitions    →  Metadata (versioned graph in the database) — see workflow-engine-architecture.md
```

## Decision table: where does a given setting belong?

| Question | If yes → |
|---|---|
| Does it differ between `dev`/`staging`/`prod` but is the same for every tenant? | **Configuration** (Kubernetes ConfigMap, per environment) |
| Is it a credential, key, or connection secret? | **Secret** (OpenBao, injected via the OpenBao Kubernetes auth method/agent injector, referenced from Configuration, never inlined) |
| Does it differ per tenant and is business-meaningful (SLA default hours, branding, enabled modules)? | **Tenant Configuration** (database row) |
| Does it gate a specific capability on/off, possibly per-tenant/per-user, with a rollback need? | **Feature Flag** |
| Is it "how does the system behave" logic rather than a value? | **Code** (or Business Rule / Workflow Definition if it's the kind of logic those systems own) |

## Why this separation matters

Conflating these is a recurring, costly failure mode:

- **Config-as-code** (a setting hardcoded and requiring a redeploy to
  change) forces unnecessary release cycles for values that should be
  operational knobs.
- **Secrets-as-config** (a connection string with an embedded password in
  a ConfigMap instead of OpenBao) defeats rotation and audit.
- **Tenant data treated as global config** (e.g. a single shared SLA default
  "constant" instead of a per-tenant row) breaks the moment a second tenant
  needs a different value — exactly the kind of retrofit this program is
  trying to avoid by deciding the shape up front.

## Implementation

- **Kubernetes ConfigMaps**: environment-scoped, non-secret settings, with
  OpenBao Agent Injector sidecars supplying anything secret as mounted
  files/environment variables at pod startup — a ConfigMap update triggers
  a rolling restart (or is polled/hot-reloaded via `IOptionsMonitor` for
  values that must update without a restart).
- **Tenant Configuration table**: `tenant_configuration(tenant_id,
  key, value_jsonb)`, cached in Valkey with an event-driven invalidation on
  update (`TenantConfigChanged` event, per `event-driven-architecture.md`)
  — never re-read from PostgreSQL on every request.
- Configuration changes to shared (non-tenant) settings follow the same
  change-management discipline as code (reviewed, logged) since a bad
  global config change has blast radius across every tenant.

---

*Next: `docs/platform/feature-flags.md` (Phase 29).*
