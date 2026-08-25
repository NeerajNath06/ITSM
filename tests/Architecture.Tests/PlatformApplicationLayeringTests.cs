using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace Architecture.Tests;

/// <summary>
/// Platform.Application has no real types yet as of Milestone 1 (it's
/// populated in Milestone 2), so this anchors on the assembly by name
/// rather than a marker type — the checks are still meaningful today
/// (an empty assembly trivially has no such dependency) and start
/// exercising real code the moment Milestone 2 lands, the same way
/// PlatformKernelLayeringTests worked before any real Domain type existed.
/// </summary>
public class PlatformApplicationLayeringTests
{
    private static readonly Assembly ApplicationAssembly = Assembly.Load("Platform.Application");

    [Fact]
    public void Platform_Application_should_not_depend_on_Platform_Infrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn("Platform.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, DescribeFailures(result));
    }

    [Fact]
    public void Platform_Application_should_not_depend_on_EntityFrameworkCore()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful, DescribeFailures(result));
    }

    [Fact]
    public void Platform_Application_should_not_depend_on_AspNetCore()
    {
        var result = Types.InAssembly(ApplicationAssembly)
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
