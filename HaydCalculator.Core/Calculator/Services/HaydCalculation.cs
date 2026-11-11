using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Misc;
using NodaTime;

using static HaydCalculator.Core.Calculator.Services.HaydCalculatorService;

namespace HaydCalculator.Core.Calculator.Services;

/// <summary>
/// One single run of the calculation over one set of flow data. Holds all the intermediate
/// state, so it must not be reused: create a new instance per calculation.
/// </summary>
/// <remarks>
/// The flows are cut into segments at every point where a ruling changes. The run then goes from
/// one blood to the next (docs/fiqh, rest-of-tuhr.md and mustahada.md):
/// <list type="bullet">
/// <item>blood inside the remaining minimal tuhr of the last hayd is dam fasad;</item>
/// <item>otherwise a hayd starts; if blood at the end of its day 15 runs on into day 16, the course is a
/// mustahada case and is resolved by tamyiz or 'ada, else the bloods within the 15 days are one hayd.</item>
/// </list>
/// </remarks>
internal sealed class HaydCalculation(IReadOnlyList<FlowDataEntity> flowData, HaydCalculatorSettings settings)
{
    private static readonly Duration s_mustahadaCycleDuration = Duration.FromDays(30);

    private readonly IReadOnlyList<FlowDataEntity> _flowData = flowData;
    private readonly HaydCalculatorSettings _settings = settings;
    private readonly List<Segment> _segments = [];
    private readonly List<CycleRange> _cycles = [];
    private Duration _makeUpDuration = Duration.Zero;

    public HaydCalculationResultVO Run()
    {
        CheckPrerequisites();

        _segments.AddRange(_flowData
            .OrderBy(x => x.From)
            .Select(x => new Segment(x.From, x.To, x.Description, IsHaydBlood(x.Description))));

        Instant cursor = _segments[0].From;

        while (FindNextBloodStart(cursor) is Instant bloodStart)
        {
            Instant next = ProcessBloodFrom(bloodStart);

            if (next <= cursor)
            {
                throw new HaydCalculationStateException($"The calculation did not advance beyond {cursor}.");
            }

            cursor = next;
        }

        return BuildResult();
    }

    #region steps

    private Instant ProcessBloodFrom(Instant bloodStart)
    {
        if (LastCycle is CycleRange lastCycle && bloodStart < lastCycle.End + MinimalTuhurDuration)
        {
            return ProcessBloodInRestOfTuhr(bloodStart, lastCycle);
        }

        Instant windowEnd = bloodStart + MaximumHaydDuration;
        SplitAt(windowEnd);

        if (IsBloodConnectedAcross(windowEnd))
        {
            return ProcessMustahadaCourse(bloodStart);
        }

        return ProcessBasicHayd(bloodStart, windowEnd);
    }

    /// <summary>
    /// Blood before the minimal tuhr after the last hayd is complete: up to its completion dam fasad;
    /// what goes on beyond it starts again as a new blood (rest-of-tuhr.md, the Bughya rule).
    /// </summary>
    private Instant ProcessBloodInRestOfTuhr(Instant bloodStart, CycleRange lastCycle)
    {
        Instant tuhrEnd = lastCycle.End + MinimalTuhurDuration;
        SplitAt(tuhrEnd);

        // the other reading for a mubtada'a (al-Bujayrimi, ash-Shabramallisi on Tuhfat al-Muhtaj 1/399):
        // the purity counts like weak blood, so she is a mustahada from her first blood on
        if (_settings.MubtadaaBloodInRestOfTuhr == EMubtadaaBloodInRestOfTuhr.MustahadaFromFirstBlood
            && _cycles.Count == 1
            && !lastCycle.IsMustahadaCase
            && IsBloodConnectedAcross(tuhrEnd))
        {
            _cycles.Clear();
            Instant courseEnd = GetCourseEnd(bloodStart);
            AddCycles(GetMubtadaaGhayrMumayyizaRanges(lastCycle.Start, courseEnd), isMustahadaCase: true);
            return courseEnd;
        }

        return tuhrEnd;
    }

    private Instant ProcessBasicHayd(Instant bloodStart, Instant windowEnd)
    {
        List<Segment> bloods = [.. _segments.Where(x => x.IsBlood && x.From >= bloodStart && x.To <= windowEnd)];
        Duration bloodDuration = bloods.Sum(x => x.Duration);

        // bloods that together do not reach a day and a night are dam fasad: drop the first one and look again
        if (bloodDuration < MinimumHaydDuration)
        {
            Instant firstBloodEnd = GetBloodRunEnd(bloodStart);
            _makeUpDuration += firstBloodEnd - bloodStart;
            return firstBloodEnd;
        }

        Instant haydEnd = bloods.Max(x => x.To);
        _cycles.Add(new CycleRange(bloodStart, haydEnd, IsMustahadaCase: false));
        return haydEnd;
    }

    /// <summary>
    /// Blood beyond 15 days: the seven kinds of mustahada (mustahada.md), so far without the mutahayyira.
    /// </summary>
    private Instant ProcessMustahadaCourse(Instant courseStart)
    {
        Instant courseEnd = GetCourseEnd(courseStart);
        List<(Instant Start, Instant End)>? tamyizRanges = GetTamyizRanges(courseStart, courseEnd);

        if (GetAda() is Ada ada)
        {
            List<(Instant Start, Instant End)> adaRanges = GetAdaRanges(ada, courseStart, courseEnd);

            if (tamyizRanges != null && _settings.MutadaMumayyizaFollows == EMutadaMumayyizaFollows.Tamyiz)
            {
                // a full minimal tuhr between her 'ada and the strong blood: both are hayd (Mughni al-Muhtaj 1/288f.)
                (Instant Start, Instant End) firstAdaRange = adaRanges[0];

                if (tamyizRanges.All(x => x.Start - firstAdaRange.End >= MinimalTuhurDuration || firstAdaRange.Start - x.End >= MinimalTuhurDuration))
                {
                    tamyizRanges.Add(firstAdaRange);
                }

                AddCycles(tamyizRanges, isMustahadaCase: true);
            }
            else
            {
                AddCycles(adaRanges, isMustahadaCase: true);
            }
        }
        else
        {
            AddCycles(tamyizRanges ?? GetMubtadaaGhayrMumayyizaRanges(courseStart, courseEnd), isMustahadaCase: true);
        }

        return courseEnd;
    }

    #endregion steps

    #region mustahada

    private List<(Instant Start, Instant End)> GetMubtadaaGhayrMumayyizaRanges(Instant courseStart, Instant courseEnd)
    {
        // a day and a night (azhar) or the usual 6 days (the calculator takes the lower of "6 or 7"), in a cycle of 30 days
        Duration haydDuration = _settings.MubtadaaGhayrMumayyizaHayd == EMubtadaaGhayrMumayyizaHayd.DayAndNight
            ? MinimumHaydDuration
            : Duration.FromDays(6);

        List<(Instant Start, Instant End)> ranges = [];

        for (Instant start = courseStart; start < courseEnd; start += s_mustahadaCycleDuration)
        {
            ranges.Add((start, start + haydDuration));
        }

        return ranges;
    }

    private List<(Instant Start, Instant End)> GetAdaRanges(Ada ada, Instant courseStart, Instant courseEnd)
    {
        List<(Instant Start, Instant End)> ranges = [];

        void AddRepeated(Instant firstStart, Duration cycleDuration)
        {
            for (Instant start = firstStart; start < courseEnd; start += cycleDuration)
            {
                ranges.Add((start, start + ada.HaydDuration));
            }
        }

        // the blood came on time or later, or the earlier blood starts a new cycle (asahh): her cycle is from the
        // start of her last hayd to the start of this blood
        if (courseStart >= ada.ExpectedStart || _settings.ShiftedCycle == EShiftedCycle.NewCycle)
        {
            AddRepeated(courseStart, courseStart - ada.LastHaydStart);
            return ranges;
        }

        // the other views on a cycle that came earlier than her 'ada (al-Fatawa al-Kubra 1/78, 1/91)
        switch (_settings.ShiftedCycle)
        {
            case EShiftedCycle.HaydExtendedThenOldCycle:
                Instant extendedEnd = ada.ExpectedStart + ada.HaydDuration;
                ranges.Add((courseStart, extendedEnd < courseStart + MaximumHaydDuration ? extendedEnd : courseStart + MaximumHaydDuration));
                AddRepeated(ada.ExpectedStart + ada.CycleDuration, ada.CycleDuration);
                break;
            case EShiftedCycle.AdaThenOldPurity:
                AddRepeated(courseStart, ada.HaydDuration + (ada.ExpectedStart - ada.LastHaydEnd));
                break;
            case EShiftedCycle.OldCycleOnly:
                AddRepeated(ada.ExpectedStart, ada.CycleDuration);
                break;
            default:
                throw new HaydCalculationStateException($"Unknown shifted cycle view {_settings.ShiftedCycle}.");
        }

        return ranges;
    }

    private Ada? GetAda()
    {
        int occurrences = Math.Max(1, _settings.OccurrencesToEstablishAda);

        if (_cycles.Count < occurrences)
        {
            return null;
        }

        CycleRange lastCycle = _cycles[^1];

        // her 'ada is what repeated as often as required: 5, 6, 7 gives 7 (once), 6 (twice), 5 (three times)
        Duration haydDuration = _cycles.TakeLast(occurrences).Min(x => x.End - x.Start);

        if (_cycles.Count < 2)
        {
            return new Ada(haydDuration, lastCycle.Start, lastCycle.End, ExpectedStart: Instant.MinValue, CycleDuration: Duration.Zero);
        }

        Duration cycleDuration = lastCycle.Start - _cycles[^2].Start;
        return new Ada(haydDuration, lastCycle.Start, lastCycle.End, lastCycle.Start + cycleDuration, cycleDuration);
    }

    /// <summary>
    /// The hayd ranges by tamyiz, or <see langword="null"/> if a condition of tamyiz fails (tamyiz.md).
    /// </summary>
    private List<(Instant Start, Instant End)>? GetTamyizRanges(Instant courseStart, Instant courseEnd)
    {
        List<BloodRun> runs = GetBloodRuns(courseStart, courseEnd);

        if (runs.Select(x => x.Strength).Distinct().Count() < 2)
        {
            return null;
        }

        int strongest = runs.Max(x => x.Strength);
        List<BloodRun> strongRuns = [.. runs.Where(x => x.Strength == strongest)];

        // condition 2: the strong blood does not exceed the maximum of hayd
        if (strongRuns.Any(x => x.End - x.Start > MaximumHaydDuration))
        {
            return null;
        }

        List<(Instant Start, Instant End)> ranges = [];
        int index = 0;

        while (index < strongRuns.Count)
        {
            Instant haydStart = strongRuns[index].Start;
            Instant haydEnd = strongRuns[index].End;

            // strong bloods within 15 days with the weak between them are one hayd (al-Fatawa al-Kubra 1/100)
            while (index + 1 < strongRuns.Count && strongRuns[index + 1].End - haydStart <= MaximumHaydDuration)
            {
                index++;
                haydEnd = strongRuns[index].End;
            }

            // condition 1: at least a day and a night of strong blood
            if (strongRuns.Where(x => x.Start >= haydStart && x.End <= haydEnd).Sum(x => x.End - x.Start) < MinimumHaydDuration)
            {
                return null;
            }

            BloodRun? nextStrong = index + 1 < strongRuns.Count ? strongRuns[index + 1] : null;

            // strong, weak, strong beyond 15 days (7/7/7): the first strong with the weak (Ibn Surayj, ar-Ramli, al-Khatib)
            if (nextStrong != null
                && _settings.WeakBetweenStrongBeyondMaximum == EWeakBetweenStrongBeyondMaximum.FirstStrongWithWeakIsHayd
                && nextStrong.Start - haydStart <= MaximumHaydDuration)
            {
                haydEnd = nextStrong.Start;
            }

            haydEnd = JoinStrongerWeak(runs, haydStart, haydEnd);
            ranges.Add((haydStart, haydEnd));

            // the strong bloods that follow: istihada unless a full minimal tuhr lies before them
            index++;

            while (index < strongRuns.Count && strongRuns[index].Start - haydEnd < MinimalTuhurDuration)
            {
                BloodRun laterStrong = strongRuns[index];

                // condition 4: the weak blood that is to be tuhr is not interrupted by strong blood
                if (runs.Any(x => x.Strength < strongest && x.Start >= laterStrong.End))
                {
                    return null;
                }

                // condition 3: the weak blood is at least the minimal tuhr if the strong blood after it goes on
                // (as-Saqqaf, lessons on al-Ibana: if the later strong blood lasts 15 days or more)
                if (laterStrong.End - laterStrong.Start >= MaximumHaydDuration)
                {
                    return null;
                }

                index++;
            }
        }

        return ranges;
    }

    /// <summary>
    /// Strong, weaker, weak (Nihayat al-Muhtaj 1/342, Tuhfat al-Muhtaj 1/403f.): the weak blood right after the
    /// strong joins it if both stay within 15 days and, by default, the blood after it is weaker still; on Ibn Hajar's
    /// view it joins whenever another weak blood of 15 days or more follows it.
    /// </summary>
    private Instant JoinStrongerWeak(List<BloodRun> runs, Instant haydStart, Instant haydEnd)
    {
        int firstWeakIndex = runs.FindIndex(x => x.Start == haydEnd);

        if (firstWeakIndex < 0 || firstWeakIndex + 1 >= runs.Count)
        {
            return haydEnd;
        }

        BloodRun firstWeak = runs[firstWeakIndex];
        BloodRun secondWeak = runs[firstWeakIndex + 1];
        int strongest = runs.Max(x => x.Strength);

        if (firstWeak.Strength >= strongest
            || secondWeak.Strength >= strongest
            || secondWeak.Strength == firstWeak.Strength
            || firstWeak.End - haydStart > MaximumHaydDuration)
        {
            return haydEnd;
        }

        bool joins = _settings.StrongerWeakMustFollowStrongDirectly
            ? secondWeak.Strength < firstWeak.Strength
            : secondWeak.End - secondWeak.Start >= MinimalTuhurDuration;

        return joins ? firstWeak.End : haydEnd;
    }

    private List<BloodRun> GetBloodRuns(Instant from, Instant to)
    {
        List<BloodRun> runs = [];

        foreach (Segment segment in _segments.Where(x => x.IsBlood && x.From >= from && x.To <= to))
        {
            int strength = GetStrength(segment.Description);

            if (runs.Count > 0 && runs[^1].End == segment.From && runs[^1].Strength == strength)
            {
                runs[^1] = runs[^1] with { End = segment.To };
            }
            else
            {
                runs.Add(new BloodRun(segment.From, segment.To, strength));
            }
        }

        return runs;
    }

    /// <summary>
    /// Colour from yellow (1) to black (3), one more for thickness and one for a bad smell
    /// (Tuhfat al-Muhtaj 1/402: thick or smelly red is as strong as plain black).
    /// </summary>
    private static int GetStrength(FlowDataDescriptionEntity description)
    {
        return (int)description.FlowAppearanceColorEnum + (description.IsThick ? 1 : 0) + (description.IsOdorStingy ? 1 : 0);
    }

    #endregion mustahada

    #region segments

    private Instant? FindNextBloodStart(Instant cursor)
    {
        foreach (Segment segment in _segments.Where(x => x.To > cursor))
        {
            if (!segment.IsBlood && IsYellowInsideAda(segment, cursor))
            {
                SplitAt(segment.From + GetAda()!.HaydDuration);
                segment.IsBlood = true;
            }

            if (segment.IsBlood)
            {
                return segment.From > cursor ? segment.From : cursor;
            }
        }

        return null;
    }

    /// <summary>
    /// If yellow is not hayd: inside the days of her 'ada it is hayd anyway (al-Khatib after ar-Rawda), unless the
    /// khilaf applies there too (Ibn Hajar).
    /// </summary>
    private bool IsYellowInsideAda(Segment segment, Instant cursor)
    {
        return !_settings.YellowAndTurbidAreHayd
            && _settings.YellowNotHaydScope == EYellowNotHaydScope.OnlyOutsideAda
            && segment.Description.FlowAppearanceColorEnum == EFlowAppearanceColor.Yellow
            && segment.From >= cursor
            && LastCycle is CycleRange lastCycle
            && segment.From >= lastCycle.End + MinimalTuhurDuration
            && GetAda() != null;
    }

    private Instant GetBloodRunEnd(Instant from)
    {
        Instant end = from;

        foreach (Segment segment in _segments.Where(x => x.From >= from))
        {
            if (!segment.IsBlood || segment.From != end)
            {
                break;
            }

            end = segment.To;
        }

        return end;
    }

    /// <summary>
    /// The end of a course of bleeding: it goes on as long as blood returns before 15 days of purity.
    /// </summary>
    private Instant GetCourseEnd(Instant courseStart)
    {
        Instant end = courseStart;

        foreach (Segment segment in _segments.Where(x => x.IsBlood && x.To > courseStart))
        {
            if (segment.From - end >= MinimalTuhurDuration)
            {
                break;
            }

            end = segment.To;
        }

        return end;
    }

    /// <summary>
    /// Whether there is blood at the last moment before <paramref name="instant"/> that goes on after it.
    /// </summary>
    private bool IsBloodConnectedAcross(Instant instant)
    {
        return _segments.Any(x => x.IsBlood && x.To == instant)
            && _segments.Any(x => x.IsBlood && x.From == instant);
    }

    private void SplitAt(Instant instant)
    {
        int index = _segments.FindIndex(x => x.From < instant && instant < x.To);

        if (index < 0)
        {
            return;
        }

        Segment segment = _segments[index];
        _segments[index] = segment with { To = instant };
        _segments.Insert(index + 1, segment with { From = instant });
    }

    #endregion segments

    #region result

    private void AddCycles(IEnumerable<(Instant Start, Instant End)> ranges, bool isMustahadaCase)
    {
        foreach ((Instant start, Instant end) in ranges.OrderBy(x => x.Start))
        {
            SplitAt(start);
            SplitAt(end);

            List<Segment> bloods = [.. _segments.Where(x => x.IsBlood && x.From >= start && x.To <= end)];

            if (bloods.Count > 0)
            {
                _cycles.Add(new CycleRange(bloods.Min(x => x.From), bloods.Max(x => x.To), isMustahadaCase));
            }
        }
    }

    private HaydCalculationResultVO BuildResult()
    {
        List<HaydCycleEntity> haydCycles = [];

        foreach (CycleRange cycle in _cycles)
        {
            var haydCycle = new HaydCycleEntity { IsMustahadaCase = cycle.IsMustahadaCase };

            foreach (Segment segment in _segments.Where(x => x.From >= cycle.Start && x.To <= cycle.End))
            {
                if (segment.IsBlood)
                {
                    AddMerged(haydCycle.HaydFlows, segment, isNaqa: false);
                }
                // the purity between the bloods is hayd (qawl as-sahb), not under qawl al-laqt
                else if (_settings.PurityBetweenBloods == EPurityBetweenBloods.Hayd)
                {
                    AddMerged(haydCycle.HaydFlows, segment, isNaqa: true);
                    _makeUpDuration += segment.Duration;
                }
            }

            haydCycles.Add(haydCycle);
        }

        List<FlowDataEntity> istihadaFlows = [];

        foreach (Segment segment in _segments.Where(x => x.IsBlood && _cycles.None(c => x.From >= c.Start && x.To <= c.End)))
        {
            AddMerged(istihadaFlows, segment, isNaqa: false);
        }

        return new HaydCalculationResultVO
        {
            HaydCycles = haydCycles.AsReadOnly(),
            IstihadaFlows = istihadaFlows.AsReadOnly(),
            MakeUpDuration = _makeUpDuration,
        };
    }

    private static void AddMerged(List<FlowDataEntity> flows, Segment segment, bool isNaqa)
    {
        if (flows.LastOrDefault() is FlowDataEntity last
            && last.To == segment.From
            && last.IsNaqa == isNaqa
            && last.Description.Equals(segment.Description))
        {
            last.To = segment.To;
            return;
        }

        flows.Add(new FlowDataEntity
        {
            From = segment.From,
            To = segment.To,
            Description = new FlowDataDescriptionEntity(segment.Description),
            IsNaqa = isNaqa,
        });
    }

    #endregion result

    private CycleRange? LastCycle => _cycles.Count > 0 ? _cycles[^1] : null;

    private void CheckPrerequisites()
    {
        if (_flowData.None(x => IsHaydBlood(x.Description)))
        {
            throw new InfoException(TextUtil.NoHaydFlowDataProvided);
        }

        if (_flowData.Any(x => x.From >= x.To))
        {
            throw new InfoException(TextUtil.FlowDataWithInvalidTimes);
        }

        if (_flowData.HasOverlaps())
        {
            throw new InfoException(TextUtil.FlowDataEntriesWithOverlappingTimes);
        }

        if (_flowData.HasGaps())
        {
            throw new InfoException(TextUtil.FlowDataEntriesWithTimeGaps);
        }
    }

    private bool IsHaydBlood(FlowDataDescriptionEntity description)
    {
        return description.IsHaydType
            && (_settings.YellowAndTurbidAreHayd || description.FlowAppearanceColorEnum != EFlowAppearanceColor.Yellow);
    }

    private sealed record Segment(Instant From, Instant To, FlowDataDescriptionEntity Description, bool IsBlood)
    {
        public bool IsBlood { get; set; } = IsBlood;

        public Duration Duration => To - From;
    }

    private sealed record CycleRange(Instant Start, Instant End, bool IsMustahadaCase);

    private sealed record BloodRun(Instant Start, Instant End, int Strength);

    /// <param name="HaydDuration">Her 'ada of hayd.</param>
    /// <param name="LastHaydStart">Start of her last hayd.</param>
    /// <param name="LastHaydEnd">End of her last hayd.</param>
    /// <param name="ExpectedStart">When her next hayd was due by her old cycle; <see cref="Instant.MinValue"/> if only one hayd is known.</param>
    /// <param name="CycleDuration">Her old cycle (hayd and tuhr).</param>
    private sealed record Ada(Duration HaydDuration, Instant LastHaydStart, Instant LastHaydEnd, Instant ExpectedStart, Duration CycleDuration);
}
