using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPlatformSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateTable(
                name: "tenant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    isolation_tier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    connection_string_secret_ref = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_platform = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant", x => x.id);
                    table.CheckConstraint("ck_tenant_isolation_tier", "isolation_tier IN ('shared', 'dedicated')");
                    table.CheckConstraint("ck_tenant_status", "status IN ('provisioning', 'active', 'suspended', 'deprovisioned')");
                });

            migrationBuilder.CreateTable(
                name: "identity_role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_system_role = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_role", x => x.id);
                    table.ForeignKey(
                        name: "fk_identity_role_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "organization",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_organization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization", x => x.id);
                    table.CheckConstraint("ck_organization_status", "status IN ('active', 'retired')");
                    table.ForeignKey(
                        name: "fk_organization_organization_parent_organization_id",
                        column: x => x.parent_organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_organization_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "acl_rule",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    table_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    operation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    required_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    condition_expression = table.Column<string>(type: "jsonb", nullable: true),
                    effect = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_acl_rule", x => x.id);
                    table.CheckConstraint("ck_acl_rule_effect", "effect IN ('allow', 'deny')");
                    table.CheckConstraint("ck_acl_rule_operation", "operation IN ('read', 'write', 'create', 'delete')");
                    table.ForeignKey(
                        name: "fk_acl_rule_identity_roles_required_role_id",
                        column: x => x.required_role_id,
                        principalTable: "identity_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_acl_rule_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "identity_group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_group", x => x.id);
                    table.ForeignKey(
                        name: "fk_identity_group_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_identity_group_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "identity_user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    external_subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "citext", maxLength: 320, nullable: false),
                    display_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_local_identity = table.Column<bool>(type: "boolean", nullable: false),
                    mfa_enrolled = table.Column<bool>(type: "boolean", nullable: false),
                    last_login_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user", x => x.id);
                    table.CheckConstraint("ck_identity_user_status", "status IN ('invited', 'active', 'suspended', 'deprovisioned')");
                    table.ForeignKey(
                        name: "fk_identity_user_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_identity_user_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "identity_group_member",
                columns: table => new
                {
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_group_member", x => new { x.group_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_identity_group_member_identity_group_group_id",
                        column: x => x.group_id,
                        principalTable: "identity_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_identity_group_member_identity_users_user_id",
                        column: x => x.user_id,
                        principalTable: "identity_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_identity_group_member_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "identity_user_role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    granted_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    granted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user_role", x => x.id);
                    table.ForeignKey(
                        name: "fk_identity_user_role_identity_role_role_id",
                        column: x => x.role_id,
                        principalTable: "identity_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_identity_user_role_identity_user_granted_by_user_id",
                        column: x => x.granted_by_user_id,
                        principalTable: "identity_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_identity_user_role_identity_user_user_id",
                        column: x => x.user_id,
                        principalTable: "identity_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_identity_user_role_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_acl_rule_required_role_id",
                table: "acl_rule",
                column: "required_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_acl_rule_tenant_id_table_name_operation",
                table: "acl_rule",
                columns: new[] { "tenant_id", "table_name", "operation" });

            migrationBuilder.CreateIndex(
                name: "ix_identity_group_organization_id",
                table: "identity_group",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_group_tenant_id_name",
                table: "identity_group",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_identity_group_member_tenant_id",
                table: "identity_group_member",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_group_member_user_id",
                table: "identity_group_member",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_role_tenant_id_name",
                table: "identity_role",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_organization_id",
                table: "identity_user",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_tenant_id_email",
                table: "identity_user",
                columns: new[] { "tenant_id", "email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_tenant_id_external_subject",
                table: "identity_user",
                columns: new[] { "tenant_id", "external_subject" },
                unique: true,
                filter: "external_subject IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_tenant_id_username",
                table: "identity_user",
                columns: new[] { "tenant_id", "username" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_role_granted_by_user_id",
                table: "identity_user_role",
                column: "granted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_role_role_id",
                table: "identity_user_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_role_tenant_id",
                table: "identity_user_role",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_role_user_id_role_id",
                table: "identity_user_role",
                columns: new[] { "user_id", "role_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organization_parent_organization_id",
                table: "organization",
                column: "parent_organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_tenant_id",
                table: "organization",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_tenant_id_code",
                table: "organization",
                columns: new[] { "tenant_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenant_is_platform_singleton",
                table: "tenant",
                column: "is_platform",
                unique: true,
                filter: "is_platform = true");

            migrationBuilder.CreateIndex(
                name: "ix_tenant_slug",
                table: "tenant",
                column: "slug",
                unique: true);

            // Row-Level Security — one policy per tenant-scoped table,
            // required in the same migration that introduces its tenant_id
            // column (docs/rules/database-rules.md), enforced by
            // MigrationRlsPairingTests. WITH CHECK (not just USING) so a
            // buggy write can never insert a row under the wrong tenant_id
            // and have it silently vanish behind the filter instead of
            // being rejected — see docs/architecture/multi-tenancy.md.
            foreach (var table in new[]
                     {
                         "identity_role",
                         "organization",
                         "acl_rule",
                         "identity_group",
                         "identity_user",
                         "identity_group_member",
                         "identity_user_role",
                     })
            {
                migrationBuilder.Sql($"ALTER TABLE {table} ENABLE ROW LEVEL SECURITY;");
                migrationBuilder.Sql($"""
                    CREATE POLICY tenant_isolation_policy ON {table}
                        USING (tenant_id = current_setting('app.tenant_id')::uuid)
                        WITH CHECK (tenant_id = current_setting('app.tenant_id')::uuid);
                    """);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acl_rule");

            migrationBuilder.DropTable(
                name: "identity_group_member");

            migrationBuilder.DropTable(
                name: "identity_user_role");

            migrationBuilder.DropTable(
                name: "identity_group");

            migrationBuilder.DropTable(
                name: "identity_role");

            migrationBuilder.DropTable(
                name: "identity_user");

            migrationBuilder.DropTable(
                name: "organization");

            migrationBuilder.DropTable(
                name: "tenant");
        }
    }
}
