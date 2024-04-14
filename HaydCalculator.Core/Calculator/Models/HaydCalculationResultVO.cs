using System.Collections.ObjectModel;

namespace HaydCalculator.Core.Calculator.Models
{
    public class HaydCalculationResultVO
    {
        public ReadOnlyCollection<HaydCycleEntity> HaydCycles { get; set; }
        public ReadOnlyCollection<FlowDataEntity> IstihadaFlows { get; set; }
        public double MakeUpDayCount { get; set; }
    }
}
