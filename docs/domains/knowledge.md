# Domain: Knowledge

**Phase:** 40 · MVP scope. Full lifecycle/schema in
`docs/platform/knowledge-management.md` (Phase 22); this document is the
domain profile.

| | |
|---|---|
| **Purpose** | Make institutional knowledge findable and self-service-able, reducing ticket volume. |
| **Business Capability** | Self-Service Knowledge |
| **Actors** | Author, Reviewer, Reader (any employee/customer per visibility) |

**Use cases:** draft an article, submit for review, publish; attach an
article to a resolved Incident (institutionalizing tribal knowledge);
reader searches/browses; reader gives helpful/not-helpful feedback.

**Entities/Aggregates:** `KnowledgeArticle`, `KnowledgeArticleVersion`,
`KnowledgeFeedback`.

**Commands:** `DraftArticle`, `SubmitForReview`, `PublishArticle`,
`RetireArticle`, `SubmitFeedback`.

**Queries:** `SearchKnowledge`, `GetArticleById`, `ListArticlesByCategory`.

**Events:** `ArticlePublished`, `ArticleRetired`.

**APIs:** `GET /api/v1/knowledge/search`, `POST /api/v1/knowledge/articles`.

**Database tables:** `knowledge_article`, `knowledge_article_version`,
`knowledge_feedback`, `category`.

**Security:** Visibility tiers (Internal/CustomerFacing/RoleRestricted);
`REQUIRES_VALIDATION` for cross-tenant shared knowledge base scenarios (not
in MVP scope — every tenant's knowledge base is fully isolated by default).

**Audit:** Publish/retire, especially for compliance-relevant articles
(e.g. security runbooks).

**Workflow:** publish approval reuses the shared `Approval` activity.

**Notifications:** review-request notification to designated reviewers.

**Dependencies:** Search (indexing/discovery), Incident/Request (attach-to),
AI/RAG grounding (Phase 46/47 — published, ACL-visible content only).

**Failure scenarios:** an article is published with sensitive content
mistakenly marked `CustomerFacing` — mitigated by classification review as
part of the publish-approval workflow step, not left to author judgment
alone for `Confidential`/`Restricted`-adjacent content.

**KPIs:** Deflection rate (searches resolved without a ticket), article
feedback score, publish cycle time.

---

*Next: `docs/domains/cmdb.md`.*
