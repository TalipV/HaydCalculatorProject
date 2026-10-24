using HaydCalculator.Core.Calculator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;

namespace HaydCalculator.Core.DependencyInjection;

/// <summary>
/// Composition root of the Core project: registers its services so the app only has to
/// call <see cref="AddCore"/> instead of knowing every single type.
/// </summary>
public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        // the only source of "now": inject IClock instead of reading the system time directly,
        // so tests can replace it (e.g. with NodaTime.Testing's FakeClock)
#pragma warning disable RS0030 // the one place that may touch the real clock
        services.TryAddSingleton<IClock>(SystemClock.Instance);
#pragma warning restore RS0030

        // stateless, every calculation runs on its own HaydCalculation instance
        services.AddSingleton<HaydCalculatorService>();

        return services;
    }
}
