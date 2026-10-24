using HaydCalculator.Core.Calculator.Models;
using NodaTime;

namespace HaydCalculator.Core.Misc;

public static class ExtensionMethods
{
    public static bool None<TSource>(this IEnumerable<TSource> source)
    {
        return !source.Any();
    }

    public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        return !source.Any(predicate);
    }

    public static Duration Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Duration> selector)
    {
        return source.Aggregate(Duration.Zero, (sum, item) => sum + selector(item));
    }

    public static bool HasOverlaps(this IEnumerable<FlowDataEntity> haydTimeData)
    {
        foreach (FlowDataEntity dataEntryA in haydTimeData)
        {
            foreach (FlowDataEntity dataEntryB in haydTimeData)
            {
                if (dataEntryA == dataEntryB)
                {
                    continue;
                }

                if (dataEntryA.From < dataEntryB.To
                    && dataEntryA.To > dataEntryB.From)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool HasGaps(this IEnumerable<FlowDataEntity> flowDataLst)
    {
        if (flowDataLst.None())
        {
            return false;
        }

        Instant minValue = flowDataLst.Select(x => x.From).Min();
        Instant maxValue = flowDataLst.Select(x => x.To).Max();

        foreach (FlowDataEntity flowData in flowDataLst)
        {
            // the current one isn't the last one and no other entry starts where it ends
            if (flowData.To != maxValue && flowDataLst.Where(x => flowData != x).None(x => x.From == flowData.To))
            {
                return true;
            }

            // the current one isn't the first one and no other entry ends where it starts
            if (flowData.From != minValue && flowDataLst.Where(x => flowData != x).None(x => x.To == flowData.From))
            {
                return true;
            }
        }

        return false;
    }
}
