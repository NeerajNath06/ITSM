# AI Agent Governance

**Phase:** 47 · The binding rule set every AI feature must satisfy before
shipping — the governance layer over `ai-architecture.md`'s mechanisms.

## Rules (verbatim from the Master AI Command, each with its enforcement mechanism)

| Rule | Enforcement mechanism |
|---|---|
| **AI can read only authorized data.** | Context assembly happens after ACL/tenant filtering (`ai-architecture.md` §Context Engine) — the model never receives data the requesting user couldn't already see via a normal API call. |
| **AI can execute only authorized actions.** | AI actions call the same Domain API commands a human UI action would, subject to the same authorization policy (`ai-architecture.md` §Tool calling). |
| **High-risk actions require human approval.** | Routed through the standard Workflow `Approval` activity (Phase 12) — no bespoke "AI approval" mechanism, no auto-apply for the high-risk category list maintained per feature. |
| **Every AI action is auditable.** | AI Gateway audit entry per call (`ai-architecture.md` §Audit), same immutable `audit_log` mechanism as every other mutation. |
| **Prompts and tool calls are observable.** | Logged (with `Restricted`-classified content excluded) and traceable via the same OpenTelemetry correlation ID as the rest of the request (Phase 24). |
| **Sensitive data must be protected.** | `Confidential`/`Restricted` fields excluded from prompts/context by construction, never sent to an external provider (`docs/governance/data-governance.md`). |
| **AI output must not automatically become trusted business data.** | AI-drafted content (e.g. a Knowledge article draft, a suggested resolution summary) is created in `Draft` state and requires the same human publish/approval step as human-authored content — never auto-published, auto-applied, or treated as verified fact. |

## Additional governance requirements

- **Model change control**: switching or upgrading the underlying model
  provider/version is a reviewed change (like a dependency upgrade), with
  regression testing against a held-out set of representative prompts
  before rollout — not a silent provider-side model swap.
- **Feature-level kill switch**: every AI feature is gated by a Feature
  Flag (Phase 29) supporting instant global disable — matches the general
  "capability with a plausible failure mode needs a kill switch" rule.
- **Bias/quality review**: AI-suggested classifications/routing
  (e.g. incident category suggestion) are monitored for systematic bias
  (e.g. consistently misrouting a category) via the same observability
  dashboards used for any other automated decision — not assumed correct
  because it's AI-generated.
- **Red-team testing for RAG**: adversarial testing specifically targeting
  cross-tenant retrieval and prompt-injection-driven data exposure is a
  release gate for any RAG feature (`docs/testing/testing-strategy.md`
  Phase 38's AI stage) — not an optional nice-to-have.
- **Vendor data-use terms reviewed**: before any external model provider is
  used with tenant data, its data-retention/training-use terms are reviewed
  and documented (e.g. confirming prompts are not used for provider model
  training) — a compliance/legal step, not purely a technical one.

## What disqualifies an AI feature from shipping

An AI feature does **not** ship if any of the following is true:

1. It cannot cite which Domain API command it calls for any mutating
   action (i.e., it has its own privileged write path).
2. It sends `Confidential`/`Restricted`-classified data to a model
   provider.
3. It has no audit trail for its suggestions/actions.
4. It auto-applies a high-risk action without a human approval step.
5. It has not passed the cross-tenant adversarial retrieval test (if it
   uses RAG).

---

*Next: `docs/platform/service-catalog.md` (Phase 48).*
