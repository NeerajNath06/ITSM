# API Design Rules

**Phase:** 16 · Normative rules for every REST surface (Table API, Domain
APIs, BFF). Companion to `docs/rules/api-rules.md` (Phase 34), which is the
condensed developer checklist version of this document.

## API surfaces (three-tier, carried forward from earlier architecture work)

| Surface | Purpose | Example |
|---|---|---|
| **Table API** | Generic, metadata-driven CRUD for any table, ACL-enforced | `GET /api/v1/table/incident?sysparm_query=state=1` |
| **Domain API** | Purpose-built endpoints for business outcomes, hides schema, orchestrates | `POST /api/v1/incidents/{id}/resolve` |
| **BFF** | React-only aggregation/shaping layer, not a public contract | `GET /bff/agent-workspace/incidents/{id}/record-page` |

## Conventions

| Concern | Rule |
|---|---|
| **Resource naming** | Plural nouns, kebab-case path segments: `/api/v1/configuration-items`, not `/api/v1/getConfigItems` |
| **Versioning** | URL-segment versioning (`/api/v1/...`). A breaking change ships as `/api/v2/...`; `v1` stays live per the deprecation window in the API governance policy (`docs/governance/architecture-governance.md`), never silently altered in place |
| **Pagination** | Cursor-based (`sysparm_cursor` + `sysparm_limit`), not raw offset — offset pagination degrades badly on large, frequently-mutated tables (a classic ITSM ticket-list problem) |
| **Filtering** | A typed, server-parsed query expression (mirroring the original `sysparm_query`-style approach), never raw client-supplied SQL/predicate strings |
| **Sorting** | `sysparm_order_by=field:asc,field2:desc` — explicit, whitelisted sortable fields per table (prevents accidental full-table-scan sorts on unindexed columns) |
| **Validation** | FluentValidation at the Application layer boundary; validation errors return `400` with a field-level error map, never a generic 500 |
| **Error model** | RFC 7807 **ProblemDetails** for every error response — `type`, `title`, `status`, `detail`, `instance`, plus a `traceId` extension member tying the error to an OpenTelemetry trace |
| **Correlation IDs** | Every response echoes `X-Correlation-Id` (accepted from the request if present, generated if not) — required for support/debugging across a distributed request |
| **Idempotency** | Mutating POSTs that could be retried by a client (e.g. "create incident from email") accept an `Idempotency-Key` header; the server deduplicates using the same inbox mechanism as `event-driven-architecture.md` |
| **Rate limiting** | Enforced at the YARP gateway layer, per-tenant and per-API-key, with `429` + `Retry-After` on limit — never silently dropped requests |
| **Authentication** | Bearer JWT (OAuth2/OIDC) on every Table/Domain API call; BFF uses an HttpOnly session cookie, never a JWT exposed to browser JS |
| **Authorization** | ACL evaluated per the model in `docs/security/security-architecture.md` on every call — no endpoint is ACL-exempt by virtue of being "internal to the BFF" |

## Request/response shape example

```http
GET /api/v1/table/incident/3f6a...?sysparm_fields=number,state,priority
Authorization: Bearer <jwt>
X-Correlation-Id: 8b1e2c...

200 OK
{
  "result": {
    "number": "INC0010045",
    "state": "InProgress",
    "priority": 2
  }
}
```

```http
POST /api/v1/incidents/3f6a.../resolve
Content-Type: application/json
Idempotency-Key: 91c4...

{ "resolutionNotes": "Restarted the app pool" }

200 OK
{ "incidentId": "3f6a...", "state": "Resolved", "resolvedAt": "2026-08-25T10:03:11Z" }
```

Error:

```http
400 Bad Request
Content-Type: application/problem+json

{
  "type": "https://errors.example.com/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-4bf9...-01",
  "errors": {
    "resolutionNotes": ["Resolution notes are required to resolve an incident."]
  }
}
```

## Discoverability and documentation

- Every endpoint is documented via OpenAPI/Swagger, generated from the code
  (attributes/annotations), never hand-maintained separately — hand-written
  docs drift from the real contract.
- A Postman collection is exported/published alongside each API version for
  partner/integration teams.
- Table API is documented **generically** (the shape of the query language,
  not a per-table endpoint list); Domain APIs are documented **per
  resource**, matching how ServiceNow documents its equivalent split
  (Phase 1 finding).

## What the BFF is allowed and not allowed to do

- **Allowed:** aggregate multiple Table/Domain API calls into one response
  shaped for a specific React screen; hold the session cookie; apply
  UI-specific field shaping.
- **Not allowed:** contain business logic, bypass ACL (it calls the same
  ACL-enforced Table/Domain APIs internally, it does not have a privileged
  side door), or be treated as a stable public contract other teams build
  against.

---

*Next: `docs/security/security-architecture.md` (Phase 17).*
