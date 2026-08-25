# MASTER AI COMMAND
## ServiceNow-Inspired Enterprise Architecture & Implementation Documentation

You are acting as a **Principal Enterprise Architect, Solution Architect, Software Architect, Database Architect, Cloud Architect, Security Architect, DevOps Architect, and Service Management Architect with 20+ years of enterprise experience**.

Your task is to perform a **deep architecture research and documentation project inspired by ServiceNow**, using the official ServiceNow documentation as the primary reference:

https://www.servicenow.com/docs

You must NOT simply summarize ServiceNow.

The goal is to understand the architectural principles, product domains, data models, workflow concepts, governance, security, integration patterns, service management concepts, CMDB/CSDM concepts, AI/workflow architecture, and enterprise operating model used by ServiceNow, and then derive an **original implementation architecture** suitable for our own enterprise application platform.

The target implementation technology is:

- Backend: ASP.NET Core / .NET 10 Web API
- Frontend: Angular + TypeScript
- Database: PostgreSQL
- Cache: Redis
- Messaging: RabbitMQ
- API communication: REST + gRPC where appropriate
- Authentication/Authorization: OAuth2/OIDC + JWT
- Observability: OpenTelemetry
- Containerization: Docker
- Cloud: Microsoft Azure
- CI/CD: Azure DevOps or GitHub Actions
- Infrastructure: Infrastructure as Code
- Architecture: Modular Monolith where appropriate + Microservices where justified
- Deployment: Azure production architecture
- API documentation: OpenAPI/Swagger & Postman

Do not blindly copy ServiceNow architecture.

Use ServiceNow as an architectural reference and derive a maintainable, scalable, secure and cost-effective architecture for our own system.

---

# PHASE 0 — RESEARCH RULES

Before creating any documentation, inspect the official ServiceNow documentation thoroughly.

Primary source:

https://www.servicenow.com/docs

Use official ServiceNow documentation wherever possible.

Secondary sources may only be used when official documentation does not provide enough information.

For every important architectural conclusion, record:

1. Source
2. ServiceNow concept
3. What the concept means
4. Why ServiceNow uses it
5. Architectural lesson
6. How we should adapt it
7. What we should NOT copy
8. Our implementation decision

Do not make assumptions when official documentation is available.

If information is uncertain, explicitly mark:

`UNKNOWN`

or

`REQUIRES_VALIDATION`

Never fabricate ServiceNow capabilities.

---

# PHASE 1 — SERVICE NOW PRODUCT LANDSCAPE

Create a complete product/capability inventory from the current ServiceNow documentation.

Research and document at minimum:

## Platform

- ServiceNow AI Platform
- Now Platform concepts
- Application Engine
- App Engine Studio
- Workflow Automation
- IntegrationHub
- Automation Engine
- Flow Designer
- Business Rules
- Scripted automation
- APIs
- Data model
- Security model
- AI capabilities
- AI Agents
- Knowledge
- Service Catalog
- Employee Center
- Mobile
- Reporting
- Performance Analytics
- Notifications
- Events
- Approvals

## IT

- ITSM
- ITOM
- ITAM
- IT Asset Management
- Hardware Asset Management
- Software Asset Management
- CMDB
- Discovery
- Service Mapping
- Event Management
- AIOps
- Cloud Observability
- Service Operations
- DevOps
- Strategic Portfolio Management
- Project Portfolio Management

## Customer

- Customer Service Management
- Field Service Management
- Sales and CRM
- Customer workflows
- Industry workflows

## Employee

- HR Service Delivery
- Employee Experience
- Employee Center
- Employee workflows

## Security

- Security Operations
- Security Incident Response
- Vulnerability Response
- Security Exposure Management
- Security Posture Control
- Risk
- Compliance
- Governance
- Privacy
- Integrated Risk Management
- Third-party risk

## Enterprise

Research additional current product families available in ServiceNow documentation, including where applicable:

- Finance
- Procurement
- Legal
- Workplace
- Telecommunications
- Manufacturing
- Healthcare
- Public Sector
- Banking / Financial Services
- Retail
- Technology
- Industry workflows

Do NOT assume the above list is exhaustive.

Discover the current official product taxonomy.

Create:

`docs/servicenow-research/product-landscape.md`

---

# PHASE 2 — PRODUCT CAPABILITY MATRIX

Create a matrix:

| Product | Business Problem | Primary Users | Core Entities | Core Workflows | Dependencies | Integrations | AI Usage | Security Concerns | KPIs |

For every major ServiceNow product/capability.

Also document:

- What problem it solves
- Why it exists
- Where it fits in enterprise architecture
- What data it owns
- What data it consumes
- What workflows it creates
- What other products depend on it

Create:

`docs/servicenow-research/product-capability-matrix.md`

---

# PHASE 3 — SERVICE NOW REFERENCE ARCHITECTURE

Reverse-engineer the conceptual architecture.

Document:

```text
User Experience
        |
        v
Channels / Portals / Mobile / Workspace
        |
        v
Workflow & Experience Layer
        |
        v
Business Application Layer
        |
        v
Shared Foundation / Data Model
        |
        v
Platform Services
        |
        v
Integration Layer
        |
        v
Enterprise Systems
```

Explain every layer.

Create:

`docs/architecture/servicenow-reference-architecture.md`

Also create:

`docs/architecture/servicenow-layered-architecture.md`

---

# PHASE 4 — OUR TARGET ARCHITECTURE

Now derive our own architecture.

Do NOT create a ServiceNow clone.

Design an architecture that gives us:

- Shared platform capabilities
- Domain isolation
- Reusable workflows
- Unified identity
- Unified audit
- Unified notification
- Unified authorization
- Unified search
- Unified configuration
- Unified tenant/company model
- Unified service catalog
- Unified knowledge
- Unified reporting
- Unified integration
- Unified observability

Create:

`docs/architecture/target-enterprise-architecture.md`

---

# PHASE 5 — DOMAIN ARCHITECTURE

Define bounded contexts.

At minimum evaluate:

```text
Identity
Organization
Tenant
User
Role
Permission
Service Catalog
Workflow
Task
Incident
Problem
Change
Request
Approval
Knowledge
Notification
Audit
CMDB
Asset
Configuration
Integration
Event
Monitoring
Reporting
Search
Document
Case
Customer
Employee
Vendor
Contract
Risk
Compliance
Security
Portfolio
Project
```

For every domain explain:

- Responsibility
- Owned data
- Read data
- Write data
- APIs
- Events
- Dependencies
- Transaction boundaries
- Security boundaries

Create:

`docs/architecture/bounded-contexts.md`

---

# PHASE 6 — MICROSERVICE VS MODULAR MONOLITH

This is extremely important.

Do not automatically create a microservice for every domain.

Evaluate each domain using:

- Business ownership
- Data ownership
- Deployment independence
- Scaling requirements
- Failure isolation
- Security isolation
- Team ownership
- Change frequency
- Transaction requirements
- Communication requirements

Create a decision matrix.

Example:

| Domain | Modular Monolith | Microservice | Decision | Reason |

Create:

`docs/architecture/microservice-boundary-rules.md`

---

# PHASE 7 — DATABASE ARCHITECTURE

Design PostgreSQL architecture.

Evaluate:

1. Shared database
2. Database per service
3. Schema per service
4. Hybrid approach

Do NOT simply state "database per microservice".

Explain:

- Why
- When
- Cost
- Operational complexity
- Transactions
- Reporting
- Referential integrity
- Data duplication
- Eventual consistency
- Backup
- Migration
- Disaster recovery

Create:

`docs/database/database-architecture.md`

---

# PHASE 8 — SERVICE NOW DATA MODEL INSIGHTS

Study concepts such as:

- Task hierarchy
- User
- Company
- Service
- Service Offering
- Configuration Item
- CMDB
- Relationships
- Case
- Incident
- Request
- Problem
- Change
- SLA
- Contract
- Entitlement
- Asset

Explain how ServiceNow avoids duplicating common entities.

Then derive our own reusable foundation.

Create:

`docs/database/shared-enterprise-data-model.md`

---

# PHASE 9 — TASK MODEL

Design a reusable Task abstraction.

Evaluate:

```text
Task
 ├── Incident
 ├── Problem
 ├── Change
 ├── Request
 ├── Case
 ├── Approval
 ├── WorkOrder
 └── Other domain tasks
```

Explain:

- inheritance vs composition
- PostgreSQL strategy
- EF Core mapping
- status
- priority
- assignment
- SLA
- audit
- comments
- attachments
- relationships

Create:

`docs/domain/task-model.md`

---

# PHASE 10 — CSDM-INSPIRED SERVICE MODEL

Research ServiceNow CSDM.

Understand:

- Business capability
- Business application
- Business service
- Technology service
- Service offering
- Service instance
- Application
- Infrastructure
- Configuration item
- Relationship

Then design our own simplified CSDM-inspired model.

Do not copy proprietary implementation details.

Create:

`docs/cmdb/csdm-inspired-service-model.md`

---

# PHASE 11 — CMDB ARCHITECTURE

Design:

- CI
- CI types
- CI relationships
- Service relationships
- Discovery
- Identification
- Reconciliation
- Dependency mapping
- Ownership
- Lifecycle
- Data quality

Create:

`docs/cmdb/cmdb-architecture.md`

Also create:

`docs/cmdb/cmdb-data-quality-rules.md`

---

# PHASE 12 — WORKFLOW ENGINE

Study ServiceNow workflow/Flow Designer concepts.

Design our own workflow engine.

It must support:

```text
Trigger
   |
Condition
   |
Action
   |
Approval
   |
Wait
   |
Event
   |
Branch
   |
Subworkflow
```

Define:

- Workflow definition
- Workflow version
- Workflow instance
- Workflow step
- Execution state
- Retry
- Timeout
- Compensation
- Human approval
- Escalation
- Scheduling
- Idempotency

Create:

`docs/workflow/workflow-engine-architecture.md`

---

# PHASE 13 — RULE ENGINE

Design business rules separately from workflow.

Support:

- validation rules
- authorization rules
- routing rules
- SLA rules
- escalation rules
- assignment rules
- notification rules
- automation rules

Explain when to use:

- application code
- business rule
- workflow
- event handler
- background job

Create:

`docs/workflow/business-rule-engine.md`

---

# PHASE 14 — EVENT-DRIVEN ARCHITECTURE

Design RabbitMQ architecture.

Define:

- Domain events
- Integration events
- Commands
- Events
- Event handlers
- Retry
- Dead-letter queue
- Idempotency
- Outbox pattern
- Inbox pattern
- Ordering
- Correlation ID
- Causation ID

Create:

`docs/integration/event-driven-architecture.md`

---

# PHASE 15 — REST + gRPC

Define clear rules.

REST for:

- external clients
- Angular
- public APIs
- partner integrations

gRPC for:

- internal service-to-service communication
- high-throughput calls
- strongly typed contracts

Do not use gRPC everywhere.

Create:

`docs/api/rest-grpc-rules.md`

---

# PHASE 16 — API ARCHITECTURE

Define:

- REST conventions
- versioning
- pagination
- filtering
- sorting
- validation
- error model
- ProblemDetails
- correlation IDs
- idempotency
- rate limiting
- authentication
- authorization

Create:

`docs/api/api-design-rules.md`

---

# PHASE 17 — SECURITY ARCHITECTURE

Design Zero Trust architecture.

Cover:

- OAuth2
- OIDC
- JWT
- refresh tokens
- service identities
- RBAC
- ABAC
- policy-based authorization
- tenant isolation
- encryption
- secrets
- Key Vault
- data protection
- audit
- privileged access
- least privilege
- defense in depth

Create:

`docs/security/security-architecture.md`

---

# PHASE 18 — MULTI-TENANCY

Design tenant architecture.

Compare:

1. Database per tenant
2. Schema per tenant
3. Shared tables with TenantId
4. Hybrid

Define:

- tenant isolation
- authorization
- row-level security
- configuration
- branding
- quotas
- tenant-specific workflows
- tenant-specific rules

Create:

`docs/architecture/multi-tenancy.md`

---

# PHASE 19 — AUDIT ARCHITECTURE

Design centralized audit service.

Audit:

- who
- what
- when
- where
- why
- old value
- new value
- IP
- user agent
- correlation ID
- tenant
- service
- entity

Audit must be immutable.

Create:

`docs/audit/audit-architecture.md`

---

# PHASE 20 — NOTIFICATION PLATFORM

Design centralized notification service.

Support:

- Email
- SMS
- Push
- In-app
- Webhook
- Teams/Slack where applicable

Define:

- templates
- preferences
- localization
- retries
- provider abstraction
- delivery status
- dead letter
- rate limits

Create:

`docs/platform/notification-service.md`

---

# PHASE 21 — SEARCH ARCHITECTURE

Design enterprise search.

Evaluate:

- PostgreSQL full text
- Elasticsearch/OpenSearch
- Azure AI Search

Support:

- global search
- domain search
- security trimming
- tenant isolation
- indexing
- reindexing

Create:

`docs/platform/search-architecture.md`

---

# PHASE 22 — KNOWLEDGE MANAGEMENT

Design:

- articles
- categories
- versioning
- approval
- publication
- expiry
- feedback
- search
- permissions

Create:

`docs/platform/knowledge-management.md`

---

# PHASE 23 — FILE / ATTACHMENT ARCHITECTURE

Use Azure Blob Storage.

Define:

- metadata
- blob storage
- virus scanning
- access control
- signed URLs
- retention
- lifecycle policies
- encryption

Create:

`docs/platform/file-storage.md`

---

# PHASE 24 — OBSERVABILITY

Use OpenTelemetry.

Design:

```text
Logs
Metrics
Traces
   |
OpenTelemetry
   |
Azure Monitor / Application Insights
```

Define:

- trace ID
- span ID
- correlation ID
- structured logging
- dashboards
- alerts
- SLO
- SLA
- health checks

Create:

`docs/observability/observability.md`

---

# PHASE 25 — AZURE PRODUCTION ARCHITECTURE

Design production deployment.

Evaluate:

- Azure Front Door
- WAF
- API Management
- Azure Container Apps vs AKS
- PostgreSQL Flexible Server
- Azure Cache for Redis
- Azure Service Bus vs RabbitMQ
- Azure Blob Storage
- Azure Key Vault
- Azure Monitor
- Application Insights
- Microsoft Entra ID
- Private Endpoints
- Virtual Network
- Private DNS
- Azure Firewall
- Azure Container Registry

Do not assume every service is necessary.

For every Azure component explain:

- Why
- When
- Cost impact
- Alternative
- Security
- Scalability
- Operational complexity

Create:

`docs/azure/production-architecture.md`

---

# PHASE 26 — DISASTER RECOVERY

Define:

- RTO
- RPO
- Backup
- Restore
- Geo-redundancy
- Database recovery
- Blob recovery
- Message recovery
- Disaster scenarios
- Failover
- Failback

Create:

`docs/architecture/disaster-recovery.md`

---

# PHASE 27 — DATA GOVERNANCE

Define:

- Data ownership
- Data classification
- PII
- Sensitive data
- Retention
- Archival
- Deletion
- Legal hold
- Encryption
- Access reviews
- Data lineage

Create:

`docs/governance/data-governance.md`

---

# PHASE 28 — CONFIGURATION ARCHITECTURE

Create centralized configuration principles.

Separate:

```text
Code
Configuration
Secrets
Feature Flags
Tenant Configuration
Business Rules
Workflow Definitions
```

Define where each belongs.

Create:

`docs/platform/configuration-management.md`

---

# PHASE 29 — FEATURE FLAGS

Design:

- global flags
- environment flags
- tenant flags
- user/role flags
- gradual rollout
- kill switch

Create:

`docs/platform/feature-flags.md`

---

# PHASE 30 — FRONTEND ARCHITECTURE

Design Angular architecture.

Cover:

- shell
- feature modules
- standalone components
- lazy loading
- state management
- API clients
- authentication
- authorization
- route guards
- permissions
- reusable components
- design system
- accessibility
- error handling
- telemetry

Create:

`docs/frontend/angular-architecture.md`

---

# PHASE 31 — .NET ARCHITECTURE

Design .NET 10 architecture.

Cover:

- Clean Architecture
- Vertical Slice Architecture
- CQRS
- MediatR or alternative
- EF Core
- repositories
- Unit of Work
- domain events
- integration events
- validation
- middleware
- exception handling
- ProblemDetails
- dependency injection
- background workers

Do not add abstractions merely because they are popular.

Create:

`docs/backend/dotnet-architecture.md`

---

# PHASE 32 — CODING PRINCIPLES

Create strict coding rules.

Include:

- SOLID
- DRY
- KISS
- YAGNI
- separation of concerns
- dependency inversion
- immutability where useful
- async/await
- cancellation tokens
- nullability
- naming
- exception handling
- logging
- testing
- performance
- security

Create:

`docs/rules/coding-rules.md`

---

# PHASE 33 — DATABASE RULES

Create:

`docs/rules/database-rules.md`

Include:

- naming
- indexes
- primary keys
- foreign keys
- GUID strategy
- timestamps
- soft delete
- audit
- migrations
- transactions
- concurrency
- partitioning
- JSONB usage
- normalization
- denormalization
- query optimization

---

# PHASE 34 — API RULES

Create:

`docs/rules/api-rules.md`

Include strict rules that developers must follow.

---

# PHASE 35 — MICROSERVICE RULES

Create:

`docs/rules/microservice-rules.md`

Include:

- service ownership
- database ownership
- contracts
- events
- retries
- timeouts
- circuit breakers
- observability
- deployment
- versioning

---

# PHASE 36 — SECURITY RULES

Create:

`docs/rules/security-rules.md`

---

# PHASE 37 — GIT RULES

Create:

`docs/rules/git-rules.md`

Include:

- branch strategy
- commit conventions
- PR rules
- code review
- merge strategy
- release tags
- hotfixes

---

# PHASE 38 — TESTING STRATEGY

Design:

```text
Unit Tests
Integration Tests
Contract Tests
API Tests
Component Tests
E2E Tests
Performance Tests
Security Tests
Chaos Tests
```

Create:

`docs/testing/testing-strategy.md`

---

# PHASE 39 — CI/CD

Design complete pipeline.

```text
Developer
   |
Git
   |
PR
   |
Build
   |
Test
   |
Security Scan
   |
Container Build
   |
Registry
   |
Deploy Dev
   |
Integration Tests
   |
Deploy QA
   |
Approval
   |
Production
```

Create:

`docs/devops/cicd.md`

---

# PHASE 40 — DOMAIN MODULE DOCUMENTATION

For every major domain create:

```text
Purpose
Business Capability
Actors
Use Cases
Entities
Value Objects
Aggregates
Commands
Queries
Events
APIs
Database Tables
Security
Audit
Workflow
Notifications
Dependencies
Failure Scenarios
KPIs
```

Example:

`docs/domains/incident-management.md`

Also create equivalent documentation for:

- Problem Management
- Change Management
- Request Management
- Case Management
- Service Catalog
- Knowledge
- CMDB
- Asset Management
- Customer Management
- Employee Management
- Security
- Risk
- Compliance
- Project
- Portfolio
- Vendor
- Contract

---

# PHASE 41 — ARCHITECTURE DECISION RECORDS

Create ADRs for important decisions.

At minimum:

```text
ADR-001 Modular Monolith vs Microservices
ADR-002 Database-per-Service Decision
ADR-003 PostgreSQL Decision
ADR-004 RabbitMQ Decision
ADR-005 REST vs gRPC
ADR-006 CQRS
ADR-007 Event-Driven Architecture
ADR-008 Multi-Tenancy
ADR-009 Authentication
ADR-010 Authorization
ADR-011 Audit Architecture
ADR-012 CMDB
ADR-013 Workflow Engine
ADR-014 Search Engine
ADR-015 Azure Container Apps vs AKS
ADR-016 Azure Service Bus vs RabbitMQ
ADR-017 Redis
ADR-018 OpenTelemetry
ADR-019 Blob Storage
ADR-020 API Gateway
```

Each ADR must contain:

```text
Context
Problem
Options
Decision
Why
Trade-offs
Consequences
Rejected Alternatives
```

---

# PHASE 42 — ARCHITECTURE DIAGRAMS

Create Mermaid diagrams wherever possible.

Required diagrams:

1. Enterprise architecture
2. Platform architecture
3. Domain architecture
4. Microservices architecture
5. Database architecture
6. Authentication flow
7. Authorization flow
8. Request lifecycle
9. Event lifecycle
10. Workflow execution
11. Audit flow
12. Notification flow
13. CMDB architecture
14. CSDM-inspired architecture
15. CI/CD
16. Azure production architecture
17. Disaster recovery
18. Observability
19. Angular architecture
20. .NET architecture

Store diagrams in:

`docs/diagrams/`

Use Mermaid syntax.

---

# PHASE 43 — IMPLEMENTATION ROADMAP

Create an implementation roadmap.

Do NOT implement everything simultaneously.

Define:

## Phase 1
Foundation

## Phase 2
Identity and Organization

## Phase 3
Platform Services

## Phase 4
Workflow

## Phase 5
Service Management

## Phase 6
CMDB

## Phase 7
IT Operations

## Phase 8
Security

## Phase 9
Customer

## Phase 10
Employee

## Phase 11
Analytics and AI

For every phase define:

- objective
- dependencies
- deliverables
- database changes
- APIs
- frontend
- tests
- deployment
- risks

Create:

`docs/roadmap/implementation-roadmap.md`

---

# PHASE 44 — MVP DEFINITION

Do not attempt to build the entire ServiceNow ecosystem initially.

Define the smallest production-capable platform.

Recommend:

```text
Identity
Organization
Tenant
RBAC
Audit
Notification
Task
Workflow
Service Catalog
Knowledge
Incident
Request
Change
CMDB foundation
Observability
Integration
```

Explain why these should come first.

Create:

`docs/roadmap/mvp.md`

---

# PHASE 45 — PROJECT FOLDER STRUCTURE

Create the recommended repository structure.

Example:

```text
src/
  BuildingBlocks/
  Platform/
  Modules/
  Services/
  Gateway/
  Web/

tests/
  Unit/
  Integration/
  Contract/
  E2E/

docs/
  architecture/
  database/
  domains/
  workflow/
  security/
  platform/
  integration/
  azure/
  observability/
  rules/
  testing/
  devops/
  roadmap/
  adr/
  diagrams/
  servicenow-research/
```

Do not blindly copy this structure.

Adapt it based on the final architecture.

Create:

`docs/architecture/repository-structure.md`

---

# PHASE 46 — AI ARCHITECTURE

Research ServiceNow AI Platform and AI Agent concepts.

Then design our own AI architecture.

Include:

- AI Gateway
- Model abstraction
- Azure OpenAI
- prompt management
- RAG
- embeddings
- vector search
- tool calling
- agent orchestration
- permissions
- audit
- AI safety
- prompt injection protection
- PII handling
- human approval
- AI action boundaries

Critical principle:

AI must NEVER bypass existing authorization.

Create:

`docs/ai/ai-architecture.md`

---

# PHASE 47 — AI AGENT GOVERNANCE

Define rules:

```text
AI can read only authorized data.
AI can execute only authorized actions.
High-risk actions require human approval.
Every AI action is auditable.
Prompts and tool calls are observable.
Sensitive data must be protected.
AI output must not automatically become trusted business data.
```

Create:

`docs/ai/ai-governance.md`

---

# PHASE 48 — SERVICE CATALOG

Design a Service Catalog similar conceptually to ServiceNow.

Support:

- service
- offering
- catalog item
- category
- form
- variables
- approvals
- fulfillment workflow
- SLA
- entitlement
- pricing if required

Create:

`docs/platform/service-catalog.md`

---

# PHASE 49 — SLA ENGINE

Design reusable SLA engine.

Support:

- SLA definition
- start condition
- pause condition
- stop condition
- business calendar
- holidays
- escalation
- breach
- percentage consumed
- notifications

Create:

`docs/platform/sla-engine.md`

---

# PHASE 50 — ENTERPRISE GOVERNANCE

Create architecture governance.

Define:

- Architecture Review Board
- coding standards
- API governance
- database governance
- security review
- dependency review
- ADR process
- release governance
- observability requirements
- technical debt management

Create:

`docs/governance/architecture-governance.md`

---

# PHASE 51 — FINAL RULES FILE

Create a single master file:

`CLAUDE.md`

It must contain strict rules for AI coding agents.

It must tell the AI:

1. Never violate architecture.
2. Never create unnecessary microservices.
3. Never duplicate shared entities.
4. Never bypass authorization.
5. Never store secrets in source code.
6. Never directly modify production data.
7. Never create database changes without migrations.
8. Never create APIs without documentation.
9. Never create business logic in Angular.
10. Never create business logic inside controllers.
11. Never call another service database directly.
12. Never ignore tenant boundaries.
13. Never ignore audit requirements.
14. Never ignore observability.
15. Never introduce dependencies without justification.
16. Never change architecture without ADR.
17. Never create duplicate workflow engines.
18. Never duplicate notification logic.
19. Never duplicate identity logic.
20. Never bypass domain boundaries.

Add all additional rules discovered during the research.

---

# PHASE 52 — AI CODING AGENT RULES

Create:

`docs/rules/ai-agent-rules.md`

The AI coding agent must follow:

```text
Understand
   ↓
Inspect
   ↓
Plan
   ↓
Validate architecture
   ↓
Implement
   ↓
Test
   ↓
Review
   ↓
Document
```

The AI must NOT immediately start coding.

Before every significant implementation:

1. Inspect existing code.
2. Identify affected modules.
3. Check architecture documentation.
4. Check ADRs.
5. Check database ownership.
6. Check API contracts.
7. Check security.
8. Check audit.
9. Check observability.
10. Plan changes.
11. Implement.
12. Test.
13. Review.
14. Update documentation.

---

# PHASE 53 — ANTI-PATTERNS

Create:

`docs/rules/anti-patterns.md`

Document things we must NOT do.

Examples:

- God Service
- God Controller
- God Database
- Shared Database Across Independent Services
- Distributed Monolith
- Chatty Services
- Circular Dependencies
- Anemic Domain Model where inappropriate
- Over-engineered Repository Pattern
- Generic Repository everywhere
- Business logic in controllers
- Business logic in Angular
- Service-to-service database access
- Hard-coded configuration
- Secrets in Git
- Synchronous distributed transactions
- Unlimited retries
- Missing idempotency
- Missing correlation IDs
- Missing audit
- Missing authorization
- Duplicate master data

---

# PHASE 54 — QUALITY GATES

Create quality gates.

A feature is NOT complete until:

```text
Architecture reviewed
Database migration created
API documented
Authorization implemented
Audit implemented
Logging implemented
Tracing implemented
Metrics implemented
Unit tests implemented
Integration tests implemented
Security reviewed
Performance considered
Documentation updated
ADR created if architecture changed
```

Create:

`docs/governance/definition-of-done.md`

---

# PHASE 55 — FINAL MASTER ARCHITECTURE DOCUMENT

Create:

`docs/MASTER-ARCHITECTURE.md`

It must summarize:

1. Vision
2. Goals
3. Non-goals
4. Architecture principles
5. Product domains
6. Platform architecture
7. Domain architecture
8. Data architecture
9. Workflow
10. Integration
11. Security
12. Multi-tenancy
13. Audit
14. Observability
15. AI
16. Azure
17. CI/CD
18. Disaster recovery
19. Governance
20. Development rules
21. Roadmap

---

# CRITICAL RESEARCH RULES

## Rule 1

Do not assume that every ServiceNow product must become our microservice.

## Rule 2

Do not copy ServiceNow proprietary implementation.

## Rule 3

Use ServiceNow concepts only as architectural inspiration.

## Rule 4

Prefer simple architecture when complexity does not create business value.

## Rule 5

Every architecture decision must have a reason.

## Rule 6

Every important decision must document trade-offs.

## Rule 7

Prefer domain ownership over technical layering.

## Rule 8

Prefer asynchronous communication for long-running workflows.

## Rule 9

Use synchronous communication only when immediate response is required.

## Rule 10

Every distributed operation must consider failure.

## Rule 11

Every event consumer must be idempotent.

## Rule 12

Every external API must have authentication, authorization, validation, rate limiting and observability.

## Rule 13

Every database change must use EF Core migrations.

## Rule 14

Every production component must have monitoring and health checks.

## Rule 15

Every business-critical operation must be auditable.

---

# OUTPUT REQUIREMENTS

At the end of the work, the repository must contain a complete documentation system.

Minimum structure:

```text
docs/
├── MASTER-ARCHITECTURE.md
│
├── architecture/
├── database/
├── backend/
├── frontend/
├── domains/
├── workflow/
├── cmdb/
├── security/
├── audit/
├── platform/
├── integration/
├── ai/
├── azure/
├── observability/
├── testing/
├── devops/
├── governance/
├── roadmap/
├── adr/
├── diagrams/
├── rules/
└── servicenow-research/
```

Also create:

```text
CLAUDE.md
```

---

# FINAL VALIDATION

After creating all documentation, perform a consistency review.

Check:

### Architecture
- Are bounded contexts consistent?
- Are service boundaries justified?
- Are there circular dependencies?

### Database
- Does every service own its data?
- Are shared entities correctly modeled?
- Are migrations defined?

### APIs
- Are API ownership and contracts clear?
- Are REST/gRPC decisions justified?

### Events
- Are events idempotent?
- Are retries and DLQs defined?
- Is the Outbox pattern defined?

### Security
- Is authorization enforced everywhere?
- Is tenant isolation defined?
- Is audit defined?

### Operations
- Are logs, metrics and traces defined?
- Are alerts and SLOs defined?

### Azure
- Is every Azure service justified?
- Are alternatives documented?

### AI
- Can AI bypass authorization?
- Are AI actions auditable?
- Are high-risk actions protected?

### Development
- Can an AI coding agent understand the architecture?
- Are coding rules explicit?
- Are anti-patterns documented?

---

# IMPORTANT EXECUTION BEHAVIOR

Do not rush.

Do not create shallow documentation.

Do not create generic textbook architecture.

Do not simply copy ServiceNow terminology.

Use the ServiceNow documentation to understand WHY their architecture is designed the way it is.

For each major concept answer:

```text
WHAT?
WHY?
WHEN?
WHEN NOT?
HOW?
TRADE-OFF?
SECURITY?
SCALABILITY?
FAILURE MODE?
OUR IMPLEMENTATION?
```

The final result must be practical enough that another AI coding agent can read:

`CLAUDE.md`

and the `docs/` directory and then implement the system consistently without inventing architecture.

Before modifying existing source code, inspect the repository completely and respect existing architectural decisions.

If an existing implementation conflicts with the new architecture, document the conflict first instead of silently rewriting it.

Do not implement application features in this task unless explicitly instructed.

The primary deliverable of this task is the **complete architecture/documentation/rules system**.