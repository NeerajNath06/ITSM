using NetArchTest.Rules;
using Platform.Domain.Tenancy;
using Xunit;

namespace Architecture.Tests;

/// <summary>
/// Same rule as PlatformKernelLayeringTests, anchored on a real
/// Platform.Domain type now that Phase 2 has populated one — the kernel
/// test's anchor (typeof(Entity)) only ever exercised BuildingBlocks, not
/// this project.
/// </summary>
public class PlatformDomainLayeringTests
{
    private static readonly System.Reflection.Assembly DomainAssembly = typeof(Tenant).Assembly;

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
