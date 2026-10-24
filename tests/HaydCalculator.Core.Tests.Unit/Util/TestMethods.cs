using HaydCalculator.Core.Calculator.Models;

namespace HaydCalculator.Core.Tests.Unit.Util;

internal static class TestMethods
{
    public static List<FlowDataEntity> GetFlowDataList(this List<(EFlowAppearanceColor type, double dayCount)> tupleLst, Instant initialInstant)
    {
        List<FlowDataEntity> list = new(tupleLst.Count);
        Instant lastBeginningInstant = initialInstant;

        foreach ((EFlowAppearanceColor type, double dayCount) in tupleLst.Where(x => x.dayCount != 0))
        {
            var flowDataEntity = new FlowDataEntity()
            {
                From = lastBeginningInstant,
                To = lastBeginningInstant + Duration.FromDays(dayCount),
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = type },
            };
            list.Add(flowDataEntity);
            lastBeginningInstant = flowDataEntity.To;
        }

        return list;
    }

    /// <summary>
    /// The hayd cycles of a result as day ranges [from, to) relative to <paramref name="start"/>,
    /// purity in between included (one range per cycle).
    /// </summary>
    public static List<(double FromDay, double ToDay)> GetHaydDayRanges(this HaydCalculationResultVO result, Instant start)
    {
        return [.. result.HaydCycles.Select(cycle => ToDayRange(start, cycle.HaydBeginning!.Value, cycle.HaydEnd!.Value))];
    }

    /// <summary>
    /// The istihada flows (including dam fasad) of a result as day ranges [from, to) relative to
    /// <paramref name="start"/>. Touching flows are merged and empty ones dropped, so the ranges
    /// only describe what is istihada, not how the calculator happens to split it up.
    /// </summary>
    public static List<(double FromDay, double ToDay)> GetIstihadaDayRanges(this HaydCalculationResultVO result, Instant start)
    {
        List<(double FromDay, double ToDay)> merged = [];

        foreach (FlowDataEntity flow in result.IstihadaFlows.Where(x => x.Duration > Duration.Zero).OrderBy(x => x.From))
        {
            (double fromDay, double toDay) = ToDayRange(start, flow.From, flow.To);

            if (merged.Count > 0 && merged[^1].ToDay == fromDay)
            {
                merged[^1] = (merged[^1].FromDay, toDay);
            }
            else
            {
                merged.Add((fromDay, toDay));
            }
        }

        return merged;
    }

    private static (double FromDay, double ToDay) ToDayRange(Instant start, Instant from, Instant to)
    {
        return ((from - start).TotalDays, (to - start).TotalDays);
    }

    public static bool CheckFlowsInOrder(List<FlowDataEntity> flows)
    {
        ArgumentNullException.ThrowIfNull(flows);

        if (flows.Count < 2)
        {
            return true;
        }

        for (int i = 1; i < flows.Count; i++)
        {
            if (flows[i - 1].To != flows[i].From)
            {
                return false;
            }
        }

        return true;
    }
}
