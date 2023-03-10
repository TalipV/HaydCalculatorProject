using HaydCalculator;
using HaydCalculator.Enums;
using System;

public class HaydCycleEntity
{
    #region constructors

    #endregion constructors

    #region properties

    public List<FlowDataEntity> HaydDataLst { get; set; } = new List<FlowDataEntity>();

    #endregion properties

    #region calculated properties

    public DateTime? HaydBeginning
    {
        get
        {
            return this.HaydDataLst.MinBy(x => x.FromDateTime)?.FromDateTime;
        }
    }

    public DateTime? HaydEnd
    {
        get
        {
            return this.HaydDataLst.MaxBy(x => x.ToDateTime)?.ToDateTime;
        }
    }

    public double HaydDurationDays
    {
        get
        {
            if (this.HaydBeginning == null || this.HaydEnd == null)
            {
                return 0;
            }

            return (this.HaydEnd.Value - this.HaydBeginning.Value).TotalDays;
        }
    }

    public FlowDataEntity LastHaydData
    {
        get
        {
            return this.HaydDataLst.LastOrDefault();
        }
    }

    #endregion calculated properties

    #region methods

    public double GetDaysSinceHaydBeginningUntilSpecificDate(DateTime dateTime)
    {
        if (this.HaydBeginning == null)
        {
            return 0;
        }

        return (dateTime - this.HaydBeginning.Value).TotalDays;
    }

    public double GetDaysSinceHaydEndUntilSpecificDate(DateTime dateTime)
    {
        if (this.HaydEnd == null)
        {
            return 0;
        }

        return (dateTime - this.HaydEnd.Value).TotalDays;
    }

    public static bool IsHaydFlowDataLstWithDistinguishableFlows(List<FlowDataEntity> flowDataLst)
    {
        flowDataLst.RemoveAll(x => !x.Description.IsHaydType);

        // at least two different hayd flows necessary, of course
        if (flowDataLst.GroupBy(x => x.Description).Count() < 2)
            return false;

        List<FlowDataEntity> strongHaydFlowDataLst = extractHeavyHaydFlowData(flowDataLst);
        List<FlowDataEntity> lighHaydFlowDataLst = flowDataLst.Except(strongHaydFlowDataLst).ToList();

        double strongHaydFlowDayDuration = strongHaydFlowDataLst.Sum(x => x.DayCount);

        // FIRST CONDITION: The duration of the heavy flow has to be AT LEAST the minimum hayd duration
        if (strongHaydFlowDayDuration < HaydCalculatorFactory.MINIMUM_HAYD_DURATION_DAYS)
            return false;
        // SECOND CONDITION: The duration of the heavy flow has to LESS THAN the maximum hayd duration
        if (strongHaydFlowDayDuration > HaydCalculatorFactory.MAXIMUM_HAYD_DURATION_DAYS)
            return false;

        DateTime strongFlowBeginningDateTime = strongHaydFlowDataLst.Select(x => x.ToDateTime).Min();
        DateTime strongFlowEndDateTime = strongHaydFlowDataLst.Select(x => x.ToDateTime).Max();

        List<FlowDataEntity> lighHaydFlowDataWithoutHeavyBeforeAndAfterLst = 
            lighHaydFlowDataLst.Where(x => !(x.FromDateTime > strongFlowBeginningDateTime && x.FromDateTime < strongFlowEndDateTime)).ToList();

        DateTime lightHaydFlowEndDateTime = lighHaydFlowDataLst.Select(x => x.ToDateTime).Max();

        // THIRD CONDITION: The duration of the light flow has to be AT LEAST the minimum tuhur duration
        // or else there must be no heavy flow right after the light one.
        if (lighHaydFlowDataWithoutHeavyBeforeAndAfterLst.Sum(x => x.DayCount) < HaydCalculatorFactory.MINIMAL_TUHUR_DURATION_DAYS
            && flowDataLst.Any(x => x.FromDateTime >= lightHaydFlowEndDateTime))
            return false;

        // FOURTH CONDITION: There must not be a heavy flow in between two light flows. (?)
        if (lighHaydFlowDataWithoutHeavyBeforeAndAfterLst.None())
            return false;

        return true;
    }

    private static bool isLightFlowInterruptedByHeavyFlow(List<FlowDataEntity> lighHaydFlowDataLst, List<FlowDataEntity> strongHaydFlowDataLst)
    {
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
