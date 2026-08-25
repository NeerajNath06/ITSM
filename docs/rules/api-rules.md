# API Rules

**Phase:** 34 · The developer-facing checklist form of
`docs/api/api-design-rules.md` (Phase 16) and `docs/api/rest-grpc-rules.md`
(Phase 15) — read those for rationale; this is the enforced checklist.

## Every new endpoint MUST

- [ ] Live under the correct surface: Table API (generic), Domain API
      (business outcome), or BFF (React-only aggregation) — never a
      Domain-API-shaped endpoint added to the BFF "because it was
      convenient this once."
- [ ] Be versioned (`/api/v1/...`); a breaking change is a new version, not
      an in-place change.
- [ ] Have an ACL policy applied — no endpoint ships without an explicit
      authorization check, including ones that "only return data the user
      should already see" (field-level ACL still applies).
- [ ] Validate input via FluentValidation, returning `ProblemDetails` `400`
      on failure.
- [ ] Return `ProblemDetails` for every non-2xx response, with a `traceId`.
- [ ] Support cursor-based pagination if it returns a list.
- [ ] Whitelist sortable/filterable fields explicitly — never expose
      raw column names as an implicit filter surface.
- [ ] Accept and propagate `X-Correlation-Id`.
- [ ] Be documented in the generated OpenAPI spec (no undocumented
      endpoints in `prod`).
- [ ] Have rate limiting applied at the gateway tier for anything
      public-facing.
- [ ] Support `Idempotency-Key` if it's a mutating POST that a client could
      plausibly retry.

## Every new Domain API MUST additionally

- [ ] Represent a real business outcome (`resolve`, `checkout`,
      `declare-major-incident`) — not a thin wrapper that just proxies to
      Table API with extra steps.
- [ ] Emit the relevant domain/integration event on success, via the outbox.
- [ ] Write an audit entry for the outcome (in the same transaction).

## Every new gRPC internal contract MUST

- [ ] Pass the decision checklist in `docs/api/rest-grpc-rules.md` before
      being added — reviewers reject a gRPC addition that doesn't cite
      which checklist condition justified it.
- [ ] Be versioned in its `.proto` definition with the same deprecation
      discipline as a public REST contract.
- [ ] Use mTLS (Phase 17) — no internal gRPC call skips mutual
      authentication "because it's internal."

## Forbidden

- A "god endpoint" accepting an arbitrary action-type parameter and
  branching internally (`POST /api/v1/incidents/{id}/action` with a body
  `{ "type": "resolve" }`) — each business outcome gets its own explicit
  endpoint (`/resolve`, `/reassign`), for discoverability, contract clarity,
  and per-action ACL granularity.
- Returning different response *shapes* for the same endpoint based on
  role — field-level ACL strips fields, it does not change the response's
  structural shape (clients should be able to rely on a stable shape with
  some fields possibly absent/null, not a shape that varies by caller).
- Synchronous fan-out to more than one other bounded context inside a
  single request handler where an event-driven flow would do — check
  `docs/integration/event-driven-architecture.md` before adding a second
  synchronous call chain inside a handler.

---

*Next: `docs/rules/microservice-rules.md` (Phase 35).*
