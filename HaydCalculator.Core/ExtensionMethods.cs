using HaydCalculator.Entities;
using HaydCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaydCalculator
{
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

        public static bool HasOverlaps(this IEnumerable<FlowDataEntity> haydTimeData)
        {
            foreach(FlowDataEntity dataEntryA in haydTimeData)
            {
                foreach (FlowDataEntity dataEntryB in haydTimeData)
                {
                    if (dataEntryA == dataEntryB)
                    {
                        continue;
                    }

                    if (dataEntryA.FromDateTime < dataEntryB.ToDateTime
                        && dataEntryA.ToDateTime > dataEntryB.FromDateTime)
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

            DateTime minValue = flowDataLst.Select(x => x.FromDateTime).Min();
            DateTime maxValue = flowDataLst.Select(x => x.ToDateTime).Max();

            foreach (FlowDataEntity flowData in flowDataLst)
            {
                // the current one isn't the last one and no other entry starts where it ends
                if (flowData.ToDateTime != maxValue && flowDataLst.Where(x => flowData != x).None(x => x.FromDateTime == flowData.ToDateTime))
                    return true;

                // the current one isn't the first one and no other entry ends where it starts
                if (flowData.FromDateTime != minValue && flowDataLst.Where(x => flowData != x).None(x => x.ToDateTime == flowData.FromDateTime))
                    return true;
            }

            return false;
        }

        public static FlowDataEntity ToFlowTimeData(this (EFlowAppearanceColor haydDataType, double dayCount) tuple, DateTime initialDateTime)
        {
            return
                new FlowDataEntity()
                {
                    FromDateTime = initialDateTime,
                    ToDateTime = initialDateTime.AddDays(tuple.dayCount),
                    Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = tuple.haydDataType },
                };
        }
    }
}
