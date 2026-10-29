using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Misc;
using NodaTime;

using static HaydCalculator.Core.Calculator.Services.HaydCalculatorService;

namespace HaydCalculator.Core.Calculator.Services;

/// <summary>
/// One single run of the calculation over one set of flow data. Holds all the intermediate
/// state, so it must not be reused: create a new instance per calculation.
/// </summary>
internal sealed class HaydCalculation(IReadOnlyList<FlowDataEntity> flowData, HaydCalculatorSettings settings)
{
    private readonly IReadOnlyList<FlowDataEntity> _flowData = flowData;
    private readonly HaydCalculatorSettings _settings = settings;
    private readonly List<HaydCycleEntity> _haydCycleLst = [];
    private readonly List<FlowDataEntity> _istihadaLst = [];
    private List<FlowDataEntity> _workList = [];

    private HaydCycleEntity? _currentIncompleteHaydCycle = null;
    private Duration _makeUpDuration = Duration.Zero;

    private HaydCycleEntity CurrentIncompleteHaydCycle =>
        _currentIncompleteHaydCycle
        ?? throw new HaydCalculationStateException("There is no hayd cycle in progress.");

    public HaydCalculationResultVO Run()
    {
        CheckPrerequisites();

        _workList = [.. _flowData.OrderBy(x => x.From)];

        while (_workList.FirstOrDefault() is FlowDataEntity currentData)
        {
            _workList.Remove(currentData);

            if (IsHaydBlood(currentData))
            {
                HandleHaydFlowData(currentData);
            }
        }

        if (_currentIncompleteHaydCycle != null)
        {
            CompleteCurrentHaydData();
        }

        return new HaydCalculationResultVO
        {
            HaydCycles = _haydCycleLst.AsReadOnly(),
            IstihadaFlows = _istihadaLst.AsReadOnly(),
            MakeUpDuration = _makeUpDuration,
        };
    }

    private void CheckPrerequisites()
    {
        if (_flowData.None(IsHaydBlood))
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

    private void AddToHaydDataLst(FlowDataEntity newFlowTimeData)
    {
        // if last hayd data is of the same type and could just be merged with the new one
        if (CurrentIncompleteHaydCycle.LastHaydData is FlowDataEntity lastData
            && lastData.To == newFlowTimeData.From
            && lastData.Description.Equals(newFlowTimeData.Description))
        {
            lastData.To = newFlowTimeData.To;
        }
        else
        {
            ConditionallyAddInBetweenPurityToMenstruation(newFlowTimeData);
            CurrentIncompleteHaydCycle.HaydFlows.Add(newFlowTimeData);
        }
    }

    private void AddToIstihadaLst(FlowDataEntity newFlowTimeData)
    {
        if (_istihadaLst.LastOrDefault() is FlowDataEntity lastData
            && lastData.To >= newFlowTimeData.From)
        {
            lastData.To = newFlowTimeData.To;
        }
        else
        {
            _istihadaLst.Add(newFlowTimeData);
        }
    }

    private void CompleteCurrentHaydCycleIfTooLongWithProvidedDate(Instant instant)
    {
        // the current hayd is completed and there needs to be a tuhur now
        if (CurrentIncompleteHaydCycle.GetDurationSinceHaydBeginningUntil(instant) >= MaximumHaydDuration)
        {
            CompleteCurrentHaydData();
        }
    }

    private void ConsiderFlowDataAsIstihadaBecauseOfTuhur(FlowDataEntity currentData)
    {
        // istihada
        if (!IsMinimalTuhurFulfilledBy(currentData.From)
            // the minimal conceivable time is added to the from date if the To is not yet known
            && !IsMinimalTuhurFulfilledBy(currentData.To))
        {
            AddToIstihadaLst(currentData);
        }
        // first part istihada
        // second part menstruation
        else
        {
            Duration remainingTuhurDuration = GetRemainingMinimalTuhurDurationBy(currentData.From);

            if (remainingTuhurDuration == Duration.Zero)
            {
                throw new HaydCalculationStateException(
                    $"Flow from {currentData.From} was to be split into istihada and hayd, but the minimal tuhur is already fulfilled at its beginning.");
            }

            var normalIstihadaPartFlowTimeData = new FlowDataEntity(currentData)
            {
                To = currentData.From + remainingTuhurDuration,
            };

            AddToIstihadaLst(normalIstihadaPartFlowTimeData);

            var excessMenstruationPartFlowTimeData = new FlowDataEntity(currentData)
            {
                From = normalIstihadaPartFlowTimeData.To
            };

            _workList.Insert(0, excessMenstruationPartFlowTimeData);
        }
    }

    private void HandleHaydFlowData(FlowDataEntity currentData)
    {
        if (!IsHaydBlood(currentData))
        {
            return;
        }

        _currentIncompleteHaydCycle ??= new HaydCycleEntity();

        // zero if the hayd has not even begun
        Duration passedDurationFromCurrentHaydBeginning =
            CurrentIncompleteHaydCycle.GetDurationSinceHaydBeginningUntil(currentData.From);

        // new flow data would fully fit within the maximum possible hayd duration
        // --> fully hayd
        if (IsMinimalTuhurFulfilledBy(currentData.From)
            && passedDurationFromCurrentHaydBeginning + currentData.Duration <= MaximumHaydDuration)
        {
            AddToHaydDataLst(currentData);
        }
        // new flow data would make the hayd go beyond the maximum
        // --> basic or complex istihada
        else
        {
            HandleFlowDataAsIstihada(currentData, passedDurationFromCurrentHaydBeginning);
        }
    }

    private void HandleFlowDataAsIstihada(FlowDataEntity currentData, Duration passedDurationFromCurrentHaydBeginning)
    {
        if (passedDurationFromCurrentHaydBeginning + currentData.Duration <= MaximumHaydDuration && IsMinimalTuhurFulfilledBy(currentData.From))
        {
            throw new HaydCalculationStateException(
                $"Flow from {currentData.From} fits into the current hayd and should not be handled as istihada.");
        }

        FlowDataEntity? lastFlowData = CurrentIncompleteHaydCycle.LastHaydData;

        // if the new flow starts BEFORE the maximum and then would make the hayd surpass the maximum
        // --> complex istihada
        if (lastFlowData == null)
        {
            ConsiderFlowDataAsIstihadaBecauseOfTuhur(currentData);
        }
        else if (passedDurationFromCurrentHaydBeginning < MaximumHaydDuration)
        {
            DoComplexIstihada(currentData);
        }
        else
        {
            // if thew new flow starts AT the maximum
            // --> basic istihada if there was non hayd immediately before and complex istihada if there was
            if (passedDurationFromCurrentHaydBeginning == MaximumHaydDuration)
            {
                // complex
                if (lastFlowData.To == currentData.From && IsHaydBlood(lastFlowData))
                {
                    DoComplexIstihada(currentData);
                }
                // basic
                else
                {
                    CompleteCurrentHaydCycleIfTooLongWithProvidedDate(currentData.From);
                    ConsiderFlowDataAsIstihadaBecauseOfTuhur(currentData);
                }
            }
            // if the new flow starts AFTER the maximum
            // --> basic istihada because the previous flow could not have been of hayd type
            else
            {
                // how could the previous flow surpass the maximum hayd without requiring the tuhur?
                if (lastFlowData.To == currentData.From && IsHaydBlood(lastFlowData))
                {
                    throw new HaydCalculationStateException(
                        $"Hayd flow directly before {currentData.From} already exceeds the maximum hayd duration.");
                }

                CompleteCurrentHaydCycleIfTooLongWithProvidedDate(currentData.From);
                ConsiderFlowDataAsIstihadaBecauseOfTuhur(currentData);
            }
        }
    }

    private void DoComplexIstihada(FlowDataEntity currentData)
    {
        ArgumentNullException.ThrowIfNull(currentData, nameof(currentData));

        if (_currentIncompleteHaydCycle == null)
        {
            throw new HaydCalculationStateException("Complex istihada requires a hayd cycle in progress.");
        }

        // TODO: Instead of signalling a mustahada case through this InfoException, set a bool state on the
        // hayd cycle (e.g. IsComplexIstihada) that callers and tests can read
        // (see HaydCalculatorServiceTests_IstihadaClassification).
        throw new InfoException(TextUtil.ComplexIstihadaNotImplemented);

        //List<FlowDataEntity> toEvaluate = this._currentIncompleteHaydCycle.HaydDataLst;
        //toEvaluate.Add(currentData);

        //switch (GetCurrentMustahadaType())
        //{
        //    case EMustahadaType.Mubtada_ah:
        //        DoComplexIstihadaForMubtada_ah(toEvaluate);
        //        break;
        //    case EMustahadaType.Mu3taadah:
        //        DoComplexIstihadaForMu3taadah(toEvaluate);
        //        break;
        //    case EMustahadaType.Mutahayyirah:
        //        break;

        //    default:
        //        throw new NotImplementedException();
        //}
    }

    // Not called yet: groundwork for the complex istihada handling in DoComplexIstihada (see the commented-out switch there).
#pragma warning disable IDE0051 // Remove unused private members
    private EMustahadaType GetCurrentMustahadaType()
    {
        return _haydCycleLst.None() ? EMustahadaType.Mubtada_ah : EMustahadaType.Mu3taadah;
    }

    private static void DoComplexIstihadaForMubtada_ah(List<FlowDataEntity> toEvaluate)
    {
        ArgumentNullException.ThrowIfNull(toEvaluate, nameof(toEvaluate));

        if (HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(toEvaluate))
        {

        }
    }

    private static void DoComplexIstihadaForMu3taadah(List<FlowDataEntity> toEvaluate)
    {
        ArgumentNullException.ThrowIfNull(toEvaluate, nameof(toEvaluate));

        if (HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(toEvaluate))
        {

        }
    }
#pragma warning restore IDE0051 // Remove unused private members

    private void CompleteCurrentHaydData()
    {
        if (_currentIncompleteHaydCycle == null)
        {
            return;
        }

        var haydFlowWithoutNaqa =
            _currentIncompleteHaydCycle.HaydFlows.Where(x => !x.IsNaqa).ToList();

        Duration haydFlowWithoutNaqaDuration = haydFlowWithoutNaqa.Sum(x => x.Duration);

        if (haydFlowWithoutNaqaDuration < MinimumHaydDuration)
        {
            _istihadaLst.AddRange(haydFlowWithoutNaqa);
            _makeUpDuration += haydFlowWithoutNaqaDuration;
        }
        else
        {
            _haydCycleLst.Add(_currentIncompleteHaydCycle);
        }

        _currentIncompleteHaydCycle = null;
    }

    private void ConditionallyAddInBetweenPurityToMenstruation(FlowDataEntity currentData)
    {
        // the purity between the bloods counts as tuhr (qawl al-laqt) and is not added to the hayd
        if (_settings.PurityBetweenBloods == EPurityBetweenBloods.Tuhr)
        {
            return;
        }

        if (CurrentIncompleteHaydCycle.LastHaydData is not FlowDataEntity lastHaydData || !IsHaydBlood(currentData))
        {
            return;
        }

        Duration gap = currentData.From - lastHaydData.To;

        if (gap < Duration.Zero)
        {
            throw new HaydCalculationStateException(
                $"Flow from {currentData.From} starts before the last hayd flow ends ({lastHaydData.To}).");
        }

        // no gap
        if (gap == Duration.Zero)
        {
            return;
        }

        _flowData
            .Where(x => x.From >= lastHaydData.To)
            .Where(x => x.To <= currentData.From)
            .OrderBy(x => x.From).ToList()
            .ForEach(x =>
            {
                AddToHaydDataLst(x);
                x.IsNaqa = true;
                _makeUpDuration += x.Duration;
            });
    }

    private Duration GetRemainingMinimalTuhurDurationBy(Instant instant)
    {
        if (_haydCycleLst.LastOrDefault()?.HaydEnd is not Instant endOfLastCompletedHaydCycle
            || instant < endOfLastCompletedHaydCycle)
        {
            return Duration.Zero;
        }

        Duration result = MinimalTuhurDuration - (instant - endOfLastCompletedHaydCycle);
        return result > Duration.Zero ? result : Duration.Zero;
    }

    private bool IsHaydBlood(FlowDataEntity flowData)
    {
        return flowData.Description.IsHaydType
            && (_settings.YellowAndTurbidAreHayd || flowData.Description.FlowAppearanceColorEnum != EFlowAppearanceColor.Yellow);
    }

    private bool IsMinimalTuhurFulfilledBy(Instant instant)
    {
        return GetRemainingMinimalTuhurDurationBy(instant) == Duration.Zero;
    }
}
