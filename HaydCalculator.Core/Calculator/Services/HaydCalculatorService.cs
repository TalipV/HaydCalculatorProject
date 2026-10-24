using HaydCalculator.Core.Calculator.Models;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace HaydCalculator.Core.Calculator.Services;

/// <summary>
/// Entry point of the calculation. Stateless: every <see cref="Calculate"/> call works on its own
/// <see cref="HaydCalculation"/>, so one instance can be shared (registered as singleton).
/// </summary>
public class HaydCalculatorService(ILogger<HaydCalculatorService> logger)
{
    public static readonly Duration MinimumHaydDuration = Duration.FromDays(1);
    public static readonly Duration MaximumHaydDuration = Duration.FromDays(15);
    public static readonly Duration MinimalTuhurDuration = Duration.FromDays(15);

    public HaydCalculationResultVO Calculate(IReadOnlyList<FlowDataEntity> flowDataEntities)
    {
        ArgumentNullException.ThrowIfNull(flowDataEntities);

        logger.LogDebug("Calculating {FlowDataCount} flow data entries", flowDataEntities.Count);

        HaydCalculationResultVO result = new HaydCalculation(flowDataEntities).Run();

        logger.LogDebug(
            "Calculation result: {HaydCycleCount} hayd cycles, {IstihadaFlowCount} istihada flows, {MakeUpDays} make up days",
            result.HaydCycles.Count,
            result.IstihadaFlows.Count,
            result.MakeUpDuration.TotalDays);

        return result;
    }
}
