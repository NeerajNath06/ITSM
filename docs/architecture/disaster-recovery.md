# Disaster Recovery

**Phase:** 26

## RTO / RPO targets by tenant tier

| Tenant tier | RTO (max downtime) | RPO (max data loss) | Rationale |
|---|---|---|---|
| Standard (shared DB) | 4 hours | 15 minutes | Balances cost against typical ITSM customer expectations; shared infrastructure recovers as one unit |
| Enterprise / regulated (dedicated DB) | 1 hour | 5 minutes | Contractually tighter; justifies dedicated infrastructure's added cost |
| Notification service specifically | 15 minutes | Best-effort (delivery history may lag; in-flight alerts must not silently vanish) | Alerting is the one capability where "the tool is down" is itself the incident (carried from the earlier notification document) |

These are **targets that drive architecture decisions below**, not
aspirational marketing numbers — every mechanism in this document exists to
make one of these numbers achievable and testable.

## Backup

| Data store | Mechanism | Frequency |
|---|---|---|
| PostgreSQL (core + each satellite's own DB) | Automated backups + WAL/point-in-time restore via **pgBackRest** (or WAL-G), backup targets written to MinIO | Continuous WAL archiving, daily full snapshot |
| Valkey | Not backed up as a source of truth (cache/session only) — designed to be safely rebuilt from PostgreSQL/re-authentication on loss | N/A |
| MinIO | Bucket replication to a secondary MinIO cluster/site for production tenants (MinIO's built-in server-side or site replication); lifecycle-managed | Continuous (storage-native replication) |
| RabbitMQ | Not a durable system of record — messages in flight are replayed from the Outbox (`event-driven-architecture.md`) if lost, never treated as the sole copy of a fact | N/A (outbox is the durable source) |

## Restore

- **PostgreSQL**: point-in-time restore drilled quarterly against a
  non-production environment, timed against the RTO/RPO targets above —
  an untested backup is not a backup.
- **MinIO**: versioning enabled on buckets, allowing restore of an
  accidentally deleted/overwritten attachment within the retention window.
- **Message recovery**: since the outbox table is the durable source of
  truth for "what happened," recovering from a RabbitMQ outage means the
  outbox relay simply resumes publishing unpublished rows — no message loss
  as long as PostgreSQL itself is intact.

## Site redundancy and failover

- **Standard tier:** single-site Kubernetes/K3s deployment with off-site
  replicated backups (PostgreSQL WAL to a secondary MinIO/object store,
  MinIO bucket replication — can be restored to a secondary site on a site
  outage, within the 4-hour RTO) — **active-passive**, not active-active,
  because active-active multi-site for a modular monolith with RLS-based
  tenancy would require either cross-site synchronous replication
  (unacceptable write latency) or accepting eventual consistency on the
  primary transactional store (unacceptable for ticket/CMDB data) — the
  cost/complexity is not justified at this tier.
- **Enterprise tier:** dedicated database with zone-redundant HA
  (synchronous PostgreSQL streaming replica within-cluster, across nodes/
  availability zones if the underlying infrastructure offers them) as the
  default, cross-site active-passive available as a contractual option for
  tenants whose RTO demands it.

## Disaster scenarios and response

| Scenario | Response |
|---|---|
| Single node/zone failure | Kubernetes reschedules pods to healthy nodes; PostgreSQL streaming replica promotes if the primary's node is lost — absorbed with no manual failover |
| Primary site outage | Manual (initially) failover: restore latest PostgreSQL backup + redeploy the Kubernetes workload set in the secondary site via OpenTofu; DNS cutover at the Caddy/Nginx edge | RTO target 4h standard / 1h enterprise |
| Accidental data deletion (application bug or human error) | Point-in-time restore to a scratch environment, selective data recovery — never a blind full-database rollback that would lose unrelated tenants' interim data |
| RabbitMQ cluster failure | Outbox relay pauses, resumes automatically on recovery — no data loss, bounded delivery delay |
| Compromised credential / key | OpenBao key rotation runbook (Phase 17), forced session revocation via Identity/Keycloak, audit review of actions taken with the compromised credential |
| Tenant-specific data corruption | Restore that tenant's data only, from the dedicated-DB backup (enterprise) or a filtered point-in-time restore replayed against production (standard, more involved — this is a real operational cost of the shared-DB tier worth naming honestly) |

## Failback

- After a site failover, failback to the primary site is a **planned,
  scheduled operation** (not automatic), executed once the primary site's
  health is confirmed stable, with a maintenance window communicated to
  affected tenants — an automatic failback risking a second disruptive cutover
  during an unstable recovery is explicitly avoided.

---

*Next: `docs/governance/data-governance.md` (Phase 27).*
