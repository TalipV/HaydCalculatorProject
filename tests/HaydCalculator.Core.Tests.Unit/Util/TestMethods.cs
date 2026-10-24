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
