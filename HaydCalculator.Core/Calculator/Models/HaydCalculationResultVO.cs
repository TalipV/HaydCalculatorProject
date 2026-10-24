using System.Collections.ObjectModel;
using NodaTime;

namespace HaydCalculator.Core.Calculator.Models;

public class HaydCalculationResultVO
{
    public required ReadOnlyCollection<HaydCycleEntity> HaydCycles { get; init; }
    public required ReadOnlyCollection<FlowDataEntity> IstihadaFlows { get; init; }
    public required Duration MakeUpDuration { get; init; }
}
