using System.Reflection;
using HaydCalculator.Core.DependencyInjection;
using NetArchTest.Rules;

namespace HaydCalculator.Core.Tests.Unit.Architecture;

/// <summary>
/// Guards the split between the calculation logic and the app: Core must stay free of MAUI and
/// the UI libraries, so it can be tested (and reused) without any UI framework. The project
/// references already prevent Core from referencing the MAUI project; these tests additionally
/// catch a stray UI package leaking into Core.
/// </summary>
public class ArchitectureTests
{
    private static readonly Assembly s_coreAssembly = typeof(CoreServiceCollectionExtensions).Assembly;

    private static readonly string[] s_uiFrameworks =
    [
        "Microsoft.Maui",
        "CommunityToolkit.Maui",
        "UraniumUI",
        "XCalendar",
        "MetroLog",
        "PropertyChanged",
    ];

    [Fact]
    public void Core_should_not_depend_on_ui_frameworks()
    {
        TestResult result = Types.InAssembly(s_coreAssembly)
            .ShouldNot().HaveDependencyOnAny(s_uiFrameworks)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: "Core holds only the calculation logic, but these types leak a UI framework: {0}",
            becauseArgs: string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Core_should_not_depend_on_the_app()
    {
        TestResult result = Types.InAssembly(s_coreAssembly)
            .ShouldNot().HaveDependencyOn("HaydCalculator.Presentation")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: "dependencies point from the app to Core, never back, but these types use the app: {0}",
            becauseArgs: string.Join(", ", result.FailingTypeNames ?? []));
    }
}
