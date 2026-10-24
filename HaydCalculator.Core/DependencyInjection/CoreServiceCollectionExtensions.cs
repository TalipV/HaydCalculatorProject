using HaydCalculator.Core.Calculator.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HaydCalculator.Core.DependencyInjection;

/// <summary>
/// Composition root of the Core project: registers its services so the app only has to
/// call <see cref="AddCore"/> instead of knowing every single type.
/// </summary>
public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        // stateless, every calculation runs on its own HaydCalculation instance
        services.AddSingleton<HaydCalculatorService>();

        return services;
    }
}
