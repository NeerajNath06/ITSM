using System.Runtime.CompilerServices;
using Xunit;

namespace Architecture.Tests;

/// <summary>
/// Enforces docs/rules/database-rules.md's binding convention: "every
/// migration that adds a tenant_id column includes the corresponding
/// ENABLE ROW LEVEL SECURITY + CREATE POLICY statements in the same
/// migration — CI fails the build otherwise." Scans migration source text
/// directly rather than the compiled EF model, since RLS is raw SQL
/// (migrationBuilder.Sql) that leaves no trace in EF's model metadata for
/// NetArchTest-style reflection checks to see.
/// </summary>
public class MigrationRlsPairingTests
{
    [Fact]
    public void Every_migration_adding_tenant_id_also_adds_its_RLS_policy_in_the_same_file()
    {
        var migrationFiles = Directory.GetFiles(MigrationsDirectory(), "*.cs")
            .Where(f => !f.EndsWith(".Designer.cs", StringComparison.Ordinal))
            .Where(f => !Path.GetFileName(f).Equals("PlatformDbContextModelSnapshot.cs", StringComparison.Ordinal))
            .ToList();

        Assert.NotEmpty(migrationFiles);

        var violations = migrationFiles
            .Select(file => (File: Path.GetFileName(file), Text: File.ReadAllText(file)))
            .Where(m => m.Text.Contains("\"tenant_id\"", StringComparison.Ordinal))
            .Where(m => !m.Text.Contains("ENABLE ROW LEVEL SECURITY", StringComparison.Ordinal) ||
                        !m.Text.Contains("CREATE POLICY", StringComparison.Ordinal))
            .Select(m => m.File)
            .ToList();

        Assert.True(
            violations.Count == 0,
            "Migration(s) reference a tenant_id column without a matching RLS policy in the same file: "
                + string.Join(", ", violations));
    }

    private static string MigrationsDirectory([CallerFilePath] string thisFilePath = "") => Path.GetFullPath(
        Path.Combine(Path.GetDirectoryName(thisFilePath)!, "..", "..", "src", "Platform", "Platform.Infrastructure", "Persistence", "Migrations"));
}
