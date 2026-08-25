using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Entirely hand-written raw SQL — audit_log's PARTITION BY RANGE shape
    /// and its REVOKE can't be expressed via EF's declarative migration API
    /// (see AuditLogEntryConfiguration's ExcludeFromMigrations). Schema is
    /// the exact audit_log definition from docs/audit/audit-architecture.md.
    /// Seeds a handful of near-term monthly partitions; ongoing partition
    /// creation is a follow-up background job (Host.Workers), not yet built.
    /// </summary>
    public partial class AddAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // id alone can't be PRIMARY KEY here: PostgreSQL requires every
            // unique/primary key constraint on a partitioned table to
            // include the partitioning column, so this is PRIMARY KEY
            // (id, occurred_at) rather than the single-column PK shown in
            // docs/audit/audit-architecture.md's illustrative (non-partitioned)
            // SQL — a real id collision would need a matching occurred_at
            // too to violate it, which is an acceptable trade-off standard
            // for Postgres partitioned tables, not a weakening anyone chose.
            migrationBuilder.Sql("""
                CREATE TABLE audit_log (
                    id UUID NOT NULL DEFAULT gen_random_uuid(),
                    tenant_id UUID NOT NULL,
                    actor_user_id UUID,
                    impersonated_by_user_id UUID,
                    action TEXT NOT NULL,
                    entity_type TEXT NOT NULL,
                    entity_id UUID,
                    old_value JSONB,
                    new_value JSONB,
                    reason_code TEXT,
                    correlation_id UUID,
                    source_ip INET,
                    channel TEXT NOT NULL,
                    service TEXT NOT NULL,
                    occurred_at TIMESTAMPTZ NOT NULL DEFAULT now(),
                    PRIMARY KEY (id, occurred_at)
                ) PARTITION BY RANGE (occurred_at);
                """);

            migrationBuilder.Sql("CREATE INDEX ix_audit_log_tenant_id_occurred_at ON audit_log (tenant_id, occurred_at);");

            // Seed the current month plus two on either side so writes don't
            // fail immediately after this migration runs.
            foreach (var (name, from, to) in new[]
                     {
                         ("audit_log_2026_06", "2026-06-01", "2026-07-01"),
                         ("audit_log_2026_07", "2026-07-01", "2026-08-01"),
                         ("audit_log_2026_08", "2026-08-01", "2026-09-01"),
                         ("audit_log_2026_09", "2026-09-01", "2026-10-01"),
                         ("audit_log_2026_10", "2026-10-01", "2026-11-01"),
                     })
            {
                migrationBuilder.Sql(
                    $"CREATE TABLE {name} PARTITION OF audit_log FOR VALUES FROM ('{from}') TO ('{to}');");
            }

            // docs/audit/audit-architecture.md calls for "no UPDATE/DELETE
            // grant on this table for any application role," enforced at
            // the PostgreSQL level. There is deliberately no REVOKE
            // statement here yet: deploy/docker-compose.yml runs migrations
            // and the application as the same single Postgres role, and in
            // PostgreSQL a REVOKE against a table's own owning role is a
            // silent no-op (ownership bypasses the grant/revoke system
            // entirely) — so shipping one now would look like an
            // enforced restriction while doing nothing. This needs a
            // genuinely separate, least-privileged runtime role, which is
            // tied to deciding Host.Api's real connection string
            // (a later milestone), not invented here in isolation.
            migrationBuilder.Sql("ALTER TABLE audit_log ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("""
                CREATE POLICY tenant_isolation_policy ON audit_log
                    USING (tenant_id = current_setting('app.tenant_id')::uuid)
                    WITH CHECK (tenant_id = current_setting('app.tenant_id')::uuid);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS audit_log;");
        }
    }
}
