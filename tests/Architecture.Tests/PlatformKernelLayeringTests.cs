using ITSM.BuildingBlocks.Domain;
using NetArchTest.Rules;
using Xunit;

namespace Architecture.Tests;

/// <summary>
/// Enforces docs/backend/dotnet-architecture.md's Clean Architecture rule:
/// Domain has zero outward dependencies; Application depends only on
/// Domain; Infrastructure/Api depend inward. Also enforces
/// docs/rules/coding-rules.md's "no EF Core / ASP.NET Core in Domain"
/// rule. This is the first of the "isolation tests" the master guide's
/// Wave 0 exit criteria requires — a build-blocking check, not a
/// suggestion.
/// </summary>
public class PlatformKernelLayeringTests
{
    private static readonly System.Reflection.Assembly DomainAssembly = typeof(Entity).Assembly;

    [Fact]
    public void Platform_Domain_should_not_depend_on_Platform_Infrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("Platform.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, DescribeFailures(result));
    }

    [Fact]
    public void Platform_Domain_should_not_depend_on_EntityFrameworkCore()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful, DescribeFailures(result));
    }

    [Fact]
    public void Platform_Domain_should_not_depend_on_AspNetCore()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.AspNetCore")
            .GetResult();

        Assert.True(result.IsSuccessful, DescribeFailures(result));
    }

    private static string DescribeFailures(TestResult result) =>
        result.FailingTypes is null
            ? "Architecture rule violated."
            : "Architecture rule violated by: " + string.Join(", ", result.FailingTypes.Select(t => t.FullName));
}
