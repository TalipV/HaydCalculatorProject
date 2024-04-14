using HaydCalculator.Core.Calculator.Services;
using HaydCalculator.Core.Misc;

namespace HaydCalculator.Core.Calculator.Models
{
    public class HaydCycleEntity
    {
        #region constructors

        #endregion constructors

        #region properties

        public List<FlowDataEntity> HaydFlows { get; set; } = [];

        #endregion properties

        #region calculated properties

        public DateTime? HaydBeginning
        {
            get
            {
                return HaydFlows.MinBy(x => x.FromDateTime)?.FromDateTime;
            }
        }

        public DateTime? HaydEnd
        {
            get
            {
                return HaydFlows.MaxBy(x => x.ToDateTime)?.ToDateTime;
            }
        }

        public double HaydDurationDays
        {
            get
            {
                if (HaydBeginning == null || HaydEnd == null)
                {
                    return 0;
                }

                return (HaydEnd.Value - HaydBeginning.Value).TotalDays;
            }
        }

        public FlowDataEntity LastHaydData
        {
            get
            {
                return HaydFlows.LastOrDefault();
            }
        }

        #endregion calculated properties

        #region methods

        public double GetDaysSinceHaydBeginningUntilSpecificDate(DateTime dateTime)
        {
            if (HaydBeginning == null)
            {
                return 0;
            }

            return (dateTime - HaydBeginning.Value).TotalDays;
        }

        public double GetDaysSinceHaydEndUntilSpecificDate(DateTime dateTime)
        {
            if (HaydEnd == null)
            {
                return 0;
            }

            return (dateTime - HaydEnd.Value).TotalDays;
        }

        public static bool IsHaydFlowDataLstWithDistinguishableFlows(List<FlowDataEntity> flowDataLst)
        {
            flowDataLst.RemoveAll(x => !x.Description.IsHaydType);

            // at least two different hayd flows necessary, of course
            if (flowDataLst.GroupBy(x => x.Description).Count() < 2)
                return false;

            List<FlowDataEntity> strongHaydFlowDataLst = extractHeavyHaydFlowData(flowDataLst).OrderBy(x => x.FromDateTime).ToList();
            List<FlowDataEntity> lighHaydFlowDataLst = flowDataLst.Except(strongHaydFlowDataLst).OrderBy(x => x.FromDateTime).ToList();

            double strongHaydFlowDayDuration = strongHaydFlowDataLst.Sum(x => x.DayCount);
            double lightHaydFlowDayDuration = lighHaydFlowDataLst.Sum(x => x.DayCount);

            // FIRST CONDITION: The duration of the heavy flow has to be AT LEAST the minimum hayd duration
            if (strongHaydFlowDayDuration < HaydCalculatorService.MINIMUM_HAYD_DURATION_DAYS)
                return false;
            // SECOND CONDITION: The duration of the heavy flow has to LESS THAN the maximum hayd duration
            if (strongHaydFlowDayDuration > HaydCalculatorService.MAXIMUM_HAYD_DURATION_DAYS)
                return false;

            DateTime latestLightFlow = lighHaydFlowDataLst.Select(x => x.ToDateTime).Max();
            List<FlowDataEntity> heavyFlowAfterLight = strongHaydFlowDataLst.Where(x => x.FromDateTime >= latestLightFlow).ToList();

            // THIRD CONDITION: The duration of the light flow has to be at least the minimal tuhur (i.e. 15 days)
            // if the (heavy) blood continuous after that for at least a maximum hayd (i.e. 15 days)
            if (lightHaydFlowDayDuration < HaydCalculatorService.MINIMAL_TUHUR_DURATION_DAYS
                && heavyFlowAfterLight.Sum(x => x.DayCount) >= HaydCalculatorService.MAXIMUM_HAYD_DURATION_DAYS)
            {
                return false;
            }

            // FOURTH CONDITION: The light flows must not be separated by a heavy flow.
            if (isLightFlowInterruptedByHeavyFlow(lighHaydFlowDataLst, strongHaydFlowDataLst))
            {
                return false;
            }

            return true;
        }

        private static bool isLightFlowInterruptedByHeavyFlow(
            List<FlowDataEntity> lighHaydFlowDataLst,
            List<FlowDataEntity> strongHaydFlowDataLst)
        {
            foreach (FlowDataEntity strongFlow in strongHaydFlowDataLst)
            {
                FlowDataEntity latestLightFlowEndingBeforeStrongFlow = lighHaydFlowDataLst.LastOrDefault(a => a.ToDateTime < strongFlow.FromDateTime);
                FlowDataEntity earliestLightFlowBeginningAfterStrongFlow = lighHaydFlowDataLst.FirstOrDefault(a => a.FromDateTime > strongFlow.ToDateTime);

                if (latestLightFlowEndingBeforeStrongFlow != null && earliestLightFlowBeginningAfterStrongFlow != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static List<FlowDataEntity> extractHeavyHaydFlowData(List<FlowDataEntity> haydDataWithHaydTypeLst)
        {
            if (haydDataWithHaydTypeLst.None())
                throw new ArgumentException("Empty list");

            FlowDataEntity initialHeavyFlowData = haydDataWithHaydTypeLst
                    .OrderByDescending(x => x.Description)
                    .ThenByDescending(x => x.FromDateTime)
                    .FirstOrDefault();

            // based on that heaviest/earliest flow,
            // consider all the other ones with the same description too
            List<FlowDataEntity> resultLst =
                haydDataWithHaydTypeLst
                    .Where(x => x.Description == initialHeavyFlowData.Description)
                    .ToList();

            // join different description but same strength, if ...
            if (true)
            {
                // based on that heaviest/earliest flow,
                // consider all the other ones with a differing description
                // but with the same degree of strength as well
                resultLst.AddRange(
                    haydDataWithHaydTypeLst
                        .Where(x => x.Description != initialHeavyFlowData.Description)
                        .Where(x => x.Description.CompareTo(initialHeavyFlowData.Description) == 0)
                        .ToList());
            }

            // join different description and different strengths,
            // if there are more than two strength levels and other conditions
            if (true)
            {
                resultLst.AddRange(
                    haydDataWithHaydTypeLst
                        .Where(x => x.Description != initialHeavyFlowData.Description)
                        .Where(x => x.Description.CompareTo(initialHeavyFlowData.Description) < 0)
                        // aber es sind doch mehrere heavies?
                        .Where(x => x.FromDateTime == initialHeavyFlowData.FromDateTime)
                        .ToList());
            }

            return resultLst;
        }

        #endregion methods

        #region override methods

        #endregion override methods
    }
}