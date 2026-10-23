using System.Collections.ObjectModel;
using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Misc;

namespace HaydCalculator.Core.Calculator.Services;

public class HaydCalculatorService
{
    public static readonly int MINIMUM_HAYD_DURATION_DAYS = 1;
    public static readonly int MAXIMUM_HAYD_DURATION_DAYS = 15;
    public static readonly int MINIMAL_TUHUR_DURATION_DAYS = 15;

    private ReadOnlyCollection<FlowDataEntity> _flowData;
    private readonly List<HaydCycleEntity> _haydCycleLst = [];
    private readonly List<FlowDataEntity> _istihadaLst = [];
    private List<FlowDataEntity> _workList = [];

    private HaydCycleEntity _currentIncompleteHaydCycle = null;
    private double _makeUpDayCount = 0;

    private void ResetCalculationData()
    {
        _makeUpDayCount = 0;
        _currentIncompleteHaydCycle = null;
        _workList.Clear();
        _haydCycleLst.Clear();
        _istihadaLst.Clear();
    }

    private void CheckPrerequisites()
    {
        if (_flowData.None(dataEntry => dataEntry.Description.IsHaydType))
        {
            throw new InfoException(TextUtil.NO_HAYD_FLOW_DATA_PROVIDED);
        }

        if (_flowData.Any(x => x.FromDateTime >= x.ToDateTime))
        {
            throw new InfoException(TextUtil.FLOW_DATA_WITH_INVALID_TIMES);
        }

        if (_flowData.HasOverlaps())
        {
            throw new InfoException(TextUtil.FLOW_DATA_ENTRIES_WITH_OVERLAPPING_TIMES);
        }

        if (_flowData.HasGaps())
        {
            throw new InfoException(TextUtil.FLOW_DATA_ENTRIES_WITH_TIME_GAPS);
        }
    }

    public HaydCalculationResultVO Calculate(List<FlowDataEntity> flowDataEntities)
    {
        _flowData = flowDataEntities.AsReadOnly();

        CheckPrerequisites();
        ResetCalculationData();

        _workList = [.. _flowData.OrderBy(x => x.FromDateTime)];

        while (_workList.FirstOrDefault() is FlowDataEntity currentData)
        {
            _workList.Remove(currentData);

            if (currentData.Description.IsHaydType)
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
            MakeUpDayCount = _makeUpDayCount,
        };
    }

    private void AddToHaydDataLst(FlowDataEntity newFlowTimeData)
    {
        // if last hayd data is of the same type and could just be merged with the new one
        if (_currentIncompleteHaydCycle.LastHaydData is FlowDataEntity lastData
            && lastData.ToDateTime == newFlowTimeData.FromDateTime
            && lastData.Description.Equals(newFlowTimeData.Description))
        {
            lastData.ToDateTime = newFlowTimeData.ToDateTime;
        }
        else
        {
            ConditionallyAddInBetweenPurityToMenstruation(newFlowTimeData);
            _currentIncompleteHaydCycle.HaydFlows.Add(newFlowTimeData);
        }
    }

    private void AddToIstihadaLst(FlowDataEntity newFlowTimeData)
    {
        if (_istihadaLst.LastOrDefault() is FlowDataEntity lastData
            && lastData.ToDateTime >= newFlowTimeData.FromDateTime)
        {
            lastData.ToDateTime = newFlowTimeData.ToDateTime;
        }
        else
        {
            _istihadaLst.Add(newFlowTimeData);
        }
    }

    private void CompleteCurrentHaydCycleIfTooLongWithProvidedDate(DateTime dateTime)
    {
        // the current hayd is completed and there needs to be a tuhur now
        if (_currentIncompleteHaydCycle.GetDaysSinceHaydBeginningUntilSpecificDate(dateTime) >= MAXIMUM_HAYD_DURATION_DAYS)
        {
            CompleteCurrentHaydData();
        }
    }

    private void ConsiderFlowDataAsIstihadaBecauseOfTuhur(FlowDataEntity currentData)
    {
        // istihada
        if (!IsMinimalTuhurFulfilledByDate(currentData.FromDateTime)
            // the minimal conceivable time is added to the from date if the ToDateTime is not yet known
            && !IsMinimalTuhurFulfilledByDate(currentData.ToDateTime))
        {
            AddToIstihadaLst(currentData);
        }
        // first part istihada
        // second part menstruation
        else
        {
            double remainingTuhurDayCount = GetRemainingMinimalTuhurDayCountByDate(currentData.FromDateTime);

            if (remainingTuhurDayCount == 0)
            {
                throw new Exception();
            }

            var normalIstihadaPartFlowTimeData = new FlowDataEntity(currentData)
            {
                ToDateTime = currentData.FromDateTime.AddDays(GetRemainingMinimalTuhurDayCountByDate(currentData.FromDateTime)),
            };

            AddToIstihadaLst(normalIstihadaPartFlowTimeData);

            var excessMenstruationPartFlowTimeData = new FlowDataEntity(currentData)
            {
                FromDateTime = normalIstihadaPartFlowTimeData.ToDateTime
            };

            _workList.Insert(0, excessMenstruationPartFlowTimeData);
        }
    }

    private void HandleHaydFlowData(FlowDataEntity currentData)
    {
        if (!currentData.Description.IsHaydType)
        {
            return;
        }

        _currentIncompleteHaydCycle ??= new HaydCycleEntity();

        // 0 if the hayd has not even begun
        double passedDaysFromCurrentHaydBeginning =
            _currentIncompleteHaydCycle
                .GetDaysSinceHaydBeginningUntilSpecificDate(currentData.FromDateTime);

        // new flow data would fully fit within the maximum possible hayd duration
        // --> fully hayd
        if (IsMinimalTuhurFulfilledByDate(currentData.FromDateTime)
            && passedDaysFromCurrentHaydBeginning + currentData.DayCount <= MAXIMUM_HAYD_DURATION_DAYS)
        {
            AddToHaydDataLst(currentData);
        }
        // new flow data would make the hayd go beyond the maximum
        // --> basic or complex istihada
        else
        {
            HandleFlowDataAsIstihada(currentData, passedDaysFromCurrentHaydBeginning);
        }
    }

    private void HandleFlowDataAsIstihada(FlowDataEntity currentData, double passedDaysFromCurrentHaydBeginning)
    {
        if (passedDaysFromCurrentHaydBeginning + currentData.DayCount <= MAXIMUM_HAYD_DURATION_DAYS && IsMinimalTuhurFulfilledByDate(currentData.FromDateTime))
        {
            throw new Exception();
        }

        FlowDataEntity lastFlowData = _currentIncompleteHaydCycle.LastHaydData;

        // if the new flow starts BEFORE the maximum and then would make the hayd surpass the maximum
        // --> complex istihada
        if (lastFlowData == null)
        {
            ConsiderFlowDataAsIstihadaBecauseOfTuhur(currentData);
        }
        else if (passedDaysFromCurrentHaydBeginning < MAXIMUM_HAYD_DURATION_DAYS)
        {
            DoComplexIstihada(currentData);
        }
        else
        {
            // if thew new flow starts AT the maximum
            // --> basic istihada if there was non hayd immediately before and complex istihada if there was
            if (passedDaysFromCurrentHaydBeginning == MAXIMUM_HAYD_DURATION_DAYS)
            {
                // complex
                if (lastFlowData.ToDateTime == currentData.FromDateTime && lastFlowData.Description.IsHaydType)
                {
                    DoComplexIstihada(currentData);
                }
                // basic
                else
                {
                    CompleteCurrentHaydCycleIfTooLongWithProvidedDate(currentData.FromDateTime);
                    ConsiderFlowDataAsIstihadaBecauseOfTuhur(currentData);
                }
            }
            // if the new flow starts AFTER the maximum
            // --> basic istihada because the previous flow could not have been of hayd type
            else
            {
                // how could the previous flow surpass the maximum hayd without requiring the tuhur?
                if (lastFlowData.ToDateTime == currentData.FromDateTime && lastFlowData.Description.IsHaydType)
                {
                    throw new Exception();
                }

                CompleteCurrentHaydCycleIfTooLongWithProvidedDate(currentData.FromDateTime);
                ConsiderFlowDataAsIstihadaBecauseOfTuhur(currentData);
            }
        }
    }

    private void DoComplexIstihada(FlowDataEntity currentData)
    {
        ArgumentNullException.ThrowIfNull(currentData, nameof(currentData));

        if (_currentIncompleteHaydCycle == null)
        {
            throw new Exception("No hayd");
        }

        throw new InfoException(TextUtil.COMPLEX_ISTIHADA_NOT_IMPLEMENTED);

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

    //private void determineIfTuhurIsNecessary(FlowDataEntity currentData)
    //{
    //    // the current hayd is completed and there needs to be a tuhur now
    //    if (this._currentIncompleteHaydCycle.GetDaysSinceHaydBeginningUntilSpecificDate(currentData.ToDateTime) >= MAXIMUM_HAYD_DURATION_DAYS)
    //    {
    //        this.CompleteCurrentHaydData();
    //    }
    //}

    private void CompleteCurrentHaydData()
    {
        if (_currentIncompleteHaydCycle == null)
        {
            return;
        }

        var haydFlowWithoutNaqa =
            _currentIncompleteHaydCycle.HaydFlows.Where(x => !x.IsNaqa).ToList();

        if (haydFlowWithoutNaqa.Sum(x => x.DayCount) < MINIMUM_HAYD_DURATION_DAYS)
        {
            _istihadaLst.AddRange(haydFlowWithoutNaqa);
            _makeUpDayCount += haydFlowWithoutNaqa.Sum(x => x.DayCount);
        }
        else
        {
            _haydCycleLst.Add(_currentIncompleteHaydCycle);
        }

        _currentIncompleteHaydCycle = null;
    }

    private void ConditionallyAddInBetweenPurityToMenstruation(FlowDataEntity currentData)
    {
        if (_currentIncompleteHaydCycle.LastHaydData is not FlowDataEntity lastHaydData || !currentData.Description.IsHaydType)
        {
            return;
        }

        double dayGap = (currentData.FromDateTime - lastHaydData.ToDateTime).TotalDays;

        if (dayGap < 0)
        {
            throw new Exception();
        }

        // no gap
        if (dayGap == 0)
        {
            return;
        }

        _flowData
            .Where(x => x.FromDateTime >= lastHaydData.ToDateTime)
            .Where(x => x.ToDateTime <= currentData.FromDateTime)
            .OrderBy(x => x.FromDateTime).ToList()
            .ForEach(x =>
            {
                AddToHaydDataLst(x);
                x.IsNaqa = true;
                _makeUpDayCount += x.DayCount;
            });
    }

    private double GetRemainingMinimalTuhurDayCountByDate(DateTime dateTime)
    {
        DateTime? endOfLastCompletedHaydCycle = _haydCycleLst.LastOrDefault()?.HaydEnd;

        if (endOfLastCompletedHaydCycle == null || dateTime < endOfLastCompletedHaydCycle)
        {
            return 0;
        }

        double result = MINIMAL_TUHUR_DURATION_DAYS - (dateTime - endOfLastCompletedHaydCycle.Value).TotalDays;
        return Math.Max(result, 0);
    }

    private bool IsMinimalTuhurFulfilledByDate(DateTime dateTime)
    {
        return GetRemainingMinimalTuhurDayCountByDate(dateTime) == 0;
    }
}
