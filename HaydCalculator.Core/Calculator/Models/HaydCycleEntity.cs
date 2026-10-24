using HaydCalculator.Core.Calculator.Services;
using HaydCalculator.Core.Misc;
using NodaTime;

namespace HaydCalculator.Core.Calculator.Models;

public class HaydCycleEntity
{
    #region properties

    public List<FlowDataEntity> HaydFlows { get; set; } = [];

    #endregion properties

    #region calculated properties

    public Instant? HaydBeginning
    {
        get
        {
            return HaydFlows.MinBy(x => x.From)?.From;
        }
    }

    public Instant? HaydEnd
    {
        get
        {
            return HaydFlows.MaxBy(x => x.To)?.To;
        }
    }

    public Duration HaydDuration
    {
        get
        {
            if (HaydBeginning is not Instant haydBeginning || HaydEnd is not Instant haydEnd)
            {
                return Duration.Zero;
            }

            return haydEnd - haydBeginning;
        }
    }

    public FlowDataEntity? LastHaydData
    {
        get
        {
            return HaydFlows.LastOrDefault();
        }
    }

    #endregion calculated properties

    #region methods

    public Duration GetDurationSinceHaydBeginningUntil(Instant instant)
    {
        if (HaydBeginning is not Instant haydBeginning)
        {
            return Duration.Zero;
        }

        return instant - haydBeginning;
    }

    public Duration GetDurationSinceHaydEndUntil(Instant instant)
    {
        if (HaydEnd is not Instant haydEnd)
        {
            return Duration.Zero;
        }

        return instant - haydEnd;
    }

    public static bool IsHaydFlowDataLstWithDistinguishableFlows(List<FlowDataEntity> flowDataLst)
    {
        flowDataLst.RemoveAll(x => !x.Description.IsHaydType);

        // at least two different hayd flows necessary, of course
        if (flowDataLst.GroupBy(x => x.Description).Count() < 2)
        {
            return false;
        }

        var strongHaydFlowDataLst = ExtractHeavyHaydFlowData(flowDataLst).OrderBy(x => x.From).ToList();
        var lighHaydFlowDataLst = flowDataLst.Except(strongHaydFlowDataLst).OrderBy(x => x.From).ToList();

        Duration strongHaydFlowDuration = strongHaydFlowDataLst.Sum(x => x.Duration);
        Duration lightHaydFlowDuration = lighHaydFlowDataLst.Sum(x => x.Duration);

        // FIRST CONDITION: The duration of the heavy flow has to be AT LEAST the minimum hayd duration
        if (strongHaydFlowDuration < HaydCalculatorService.MinimumHaydDuration)
        {
            return false;
        }
        // SECOND CONDITION: The duration of the heavy flow has to LESS THAN the maximum hayd duration
        if (strongHaydFlowDuration > HaydCalculatorService.MaximumHaydDuration)
        {
            return false;
        }

        Instant latestLightFlow = lighHaydFlowDataLst.Select(x => x.To).Max();
        var heavyFlowAfterLight = strongHaydFlowDataLst.Where(x => x.From >= latestLightFlow).ToList();
        Duration heavyFlowAfterLightDuration = heavyFlowAfterLight.Sum(x => x.Duration);

        // THIRD CONDITION: The duration of the light flow has to be at least the minimal tuhur (i.e. 15 days)
        // if the (heavy) blood continuous after that for at least a maximum hayd (i.e. 15 days)
        if (lightHaydFlowDuration < HaydCalculatorService.MinimalTuhurDuration
            && heavyFlowAfterLightDuration >= HaydCalculatorService.MaximumHaydDuration)
        {
            return false;
        }

        // FOURTH CONDITION: The light flows must not be separated by a heavy flow.
        if (IsLightFlowInterruptedByHeavyFlow(lighHaydFlowDataLst, strongHaydFlowDataLst))
        {
            return false;
        }

        return true;
    }

    private static bool IsLightFlowInterruptedByHeavyFlow(
        List<FlowDataEntity> lighHaydFlowDataLst,
        List<FlowDataEntity> strongHaydFlowDataLst)
    {
        foreach (FlowDataEntity strongFlow in strongHaydFlowDataLst)
        {
            FlowDataEntity? latestLightFlowEndingBeforeStrongFlow = lighHaydFlowDataLst.LastOrDefault(a => a.To < strongFlow.From);
            FlowDataEntity? earliestLightFlowBeginningAfterStrongFlow = lighHaydFlowDataLst.FirstOrDefault(a => a.From > strongFlow.To);

            if (latestLightFlowEndingBeforeStrongFlow != null && earliestLightFlowBeginningAfterStrongFlow != null)
            {
                return true;
            }
        }

        return false;
    }

    private static List<FlowDataEntity> ExtractHeavyHaydFlowData(List<FlowDataEntity> haydDataWithHaydTypeLst)
    {
        if (haydDataWithHaydTypeLst.None())
        {
            throw new ArgumentException("Empty list", nameof(haydDataWithHaydTypeLst));
        }

        FlowDataEntity initialHeavyFlowData = haydDataWithHaydTypeLst
                .OrderByDescending(x => x.Description)
                .ThenByDescending(x => x.From)
                .First();

        // based on that heaviest/earliest flow,
        // consider all the other ones with the same description too
        var resultLst =
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
                [.. haydDataWithHaydTypeLst
                    .Where(x => x.Description != initialHeavyFlowData.Description)
                    .Where(x => x.Description.CompareTo(initialHeavyFlowData.Description) == 0)]);
        }

        // join different description and different strengths,
        // if there are more than two strength levels and other conditions
        if (true)
        {
            resultLst.AddRange(
                [.. haydDataWithHaydTypeLst
                    .Where(x => x.Description != initialHeavyFlowData.Description)
                    .Where(x => x.Description.CompareTo(initialHeavyFlowData.Description) < 0)
                    // aber es sind doch mehrere heavies?
                    .Where(x => x.From == initialHeavyFlowData.From)]);
        }

        return resultLst;
    }

    #endregion methods
}
