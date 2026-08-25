# ADR-019 — Object Storage (MinIO)

**Status:** Superseded (product) by `docs/adr/ADR-022-oss-self-hosted-infrastructure-stack.md`;
pipeline design below unchanged · **Full rationale:** `docs/platform/file-storage.md` (Phase 23)

> **Amendment:** self-hosted **MinIO** (S3-compatible) replaces Azure Blob
> Storage. Because the file-storage design already used signed-URL upload/
> download and a provider-abstracted metadata table rather than any
> Azure-SDK-specific call, this is a low-impact swap — the pipeline,
> quarantine-then-scan flow, and tenant-prefixed bucket layout below are
> otherwise unchanged. Antivirus is now explicitly **ClamAV**.

## Decision
Self-hosted MinIO for all attachments, with a quarantine-then-scan (ClamAV)
upload pipeline, direct-to-bucket presigned-URL upload/download,
tenant-prefixed buckets, and lifecycle-managed retention (MinIO's native
Object Lifecycle Management, the S3-API equivalent of Blob Storage's
tiering/deletion policies).

## Why
S3-compatible API with a broad tooling/SDK ecosystem, self-hostable per the
OSS stack direction (`ADR-022`), with native lifecycle management removing
the need for custom retention-job code — same reasoning as the original
decision, now against a self-hosted target.

## Trade-offs
Requires a virus-scan integration step (ClamAV, not built into MinIO
itself) between upload and availability; requires self-operating MinIO's
own durability/replication configuration rather than consuming a managed
99.999999999%-durability guarantee — accepted as part of the broader
self-hosting trade-off `ADR-022` documents.

## Consequences
No attachment bytes are ever stored in PostgreSQL — only metadata
(`attachment_metadata`) pointing to blob paths.

## Rejected alternative
Storing attachments as database BLOBs — rejected: bloats the operational
database, defeats efficient backup/restore, and loses native lifecycle
management.
