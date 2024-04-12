using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaydCalculator.Core.Entities
{
    public class HaydCalculationResultVO
    {
        public ReadOnlyCollection<HaydCycleEntity> HaydFlows { get; set; }
        public ReadOnlyCollection<FlowDataEntity> IstihadaFlows { get; set; }
        public double MakeUpDayCount { get; set; }
    }
}
