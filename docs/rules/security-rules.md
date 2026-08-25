# Security Rules

**Phase:** 36 · The binding checklist form of
`docs/security/security-architecture.md` (Phase 17). Every rule here is
review-blocking.

## Authentication

1. Every API call (except the token-issuance endpoint itself) requires a
   valid, unexpired JWT or an authenticated BFF session cookie — no
   anonymous endpoint ships without an explicit, reviewed exception (e.g.
   a public catalog browse page, documented as such).
2. No JWT is ever exposed to React JavaScript — BFF holds the session,
   React holds an HttpOnly cookie.
3. MFA is required for: local-identity admin accounts, impersonation,
   break-glass access, and API key generation — no exceptions without a
   documented, time-boxed, security-owner-approved waiver.

## Authorization

4. Every table/field access is ACL-checked — "internal" or "trusted caller"
   is not a valid reason to skip it (Master AI Command Rule 4 restated: no
   endpoint, module, or internal gRPC call bypasses authorization).
5. Explicit `Deny` always wins over any `Allow`.
6. Missing tenant context, missing role, or an ACL evaluation error is a
   **deny**, never a default-allow.
7. Editing an ACL/permission rule itself requires `security_admin` — a
   lower-privileged role must never be able to grant itself access.

## Tenant isolation

8. Every tenant-scoped table has both an EF Core query filter and a
   PostgreSQL RLS policy — CI-enforced, not optional.
9. No background job scans across tenants in one unscoped query — fan-out
   per tenant explicitly.
10. Valkey keys, RabbitMQ message payloads, object storage paths, and
    search index entries are all tenant-scoped/prefixed.

## Secrets and encryption

11. No secret, key, or credential in source control, `appsettings.json`, or
    CI logs, ever — OpenBao references only.
12. TLS 1.2+ everywhere; mTLS for internal gRPC.
13. `Confidential`/`Restricted`-classified fields (`docs/governance/data-governance.md`)
    are never logged, never sent to an AI provider, and masked in exports by
    default.

## Audit

14. Every mutating operation on Task-family, CMDB, ACL, Role, or Workflow
    Definition data writes an audit entry in the **same transaction** as the
    change.
15. The audit log has no application-level update/delete code path, and no
    database role used by the application has `UPDATE`/`DELETE` grants on
    it.

## Input handling

16. Parameterized queries / EF Core LINQ only — no string-concatenated SQL,
    ever, without an explicit exception review and proven parameterization.
17. User-supplied filter/sort expressions are parsed into a typed,
    whitelisted expression — never concatenated into a query string.
18. Rich-text/HTML rendering surfaces (Knowledge articles, comments) are
    sanitized with an allow-list, not a blocklist.

## Rate limiting and abuse

19. Every public API surface has per-tenant and per-API-key rate limits
    enforced at the gateway.
20. Attachment uploads are content-type-sniffed and virus-scanned before
    being retrievable by anyone.

## Dependencies

21. SAST, dependency vulnerability scanning, secret scanning, and container
    image scanning run on every PR and block merge on high/critical
    findings.
22. Base container images are pinned and patched on a scheduled cadence,
    not left floating on `latest`.

## AI (forward reference to Phase 46/47 — restated here as a security rule)

23. AI must never bypass ACL — a model call only sees data the requesting
    user is already authorized to see, filtered server-side before it
    reaches the prompt.
24. Every AI-suggested action that mutates data requires the same
    authorization check as if a human had triggered it directly, and is
    audited identically.

---

*Next: `docs/rules/git-rules.md` (Phase 37).*
