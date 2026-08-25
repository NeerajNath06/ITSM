# Diagrams: Authentication & Authorization Flow

**Phase:** 42 · Diagrams 6–7.

## 6. Authentication flow

```mermaid
sequenceDiagram
    participant User
    participant React
    participant IdP as Enterprise IdP
    participant TokenSvc as Platform Token Service
    participant API as Host.Api

    User->>React: Login
    React->>IdP: OIDC Authorization Code + PKCE
    IdP-->>React: Authorization code
    React->>TokenSvc: Exchange code
    TokenSvc->>IdP: Validate + fetch claims
    TokenSvc-->>React: Platform JWT (short-lived) + refresh token, set as BFF session cookie
    React->>API: API call with session cookie
    API->>API: Validate session, resolve JWT server-side
    API-->>React: Response
```
Source: `docs/security/security-architecture.md` (Phase 17).

## 7. Authorization flow

```mermaid
flowchart TD
  REQ[Incoming request] --> AUTHN[Authenticate: resolve session/JWT]
  AUTHN --> TENANT[Resolve tenant from tid claim]
  TENANT --> RBAC[RBAC: does role exist?]
  RBAC -->|no matching role| DENY[Deny - fail closed]
  RBAC -->|role present| ABAC[ABAC: evaluate table/op/field/condition policy]
  ABAC -->|explicit Deny matches| DENY
  ABAC -->|no matching Allow| DENY
  ABAC -->|Allow matches, no Deny| EFQUERY[EF Core tenant query filter]
  EFQUERY --> RLS[PostgreSQL Row-Level Security]
  RLS --> DATA[(Data)]
```
Source: `docs/security/security-architecture.md` §Authorization (Phase 17).

---

*Next: `lifecycle-flows.md`.*
