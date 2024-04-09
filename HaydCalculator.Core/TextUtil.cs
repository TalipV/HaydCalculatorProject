using HaydCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaydCalculator
{
    public static class TextUtil
    {
        public const string DATE_FORMAT = "HH:mm dd.MM";

        public const string NO_HAYD_FLOW_DATA_PROVIDED = "No hayd flow!";
        public const string FLOW_DATA_WITH_INVALID_TIMES = "Time spans with 'end' time before or equal to 'from' time!";
        public const string FLOW_DATA_ENTRIES_WITH_OVERLAPPING_TIMES = "Time overlaps are not permitted!";
        public const string FLOW_DATA_ENTRIES_WITH_TIME_GAPS = "Gaps between times are not permitted!";
        public const string COMPLEX_ISTIHADA_NOT_IMPLEMENTED = "Complex istihada cases not implemented!";
    }
}
