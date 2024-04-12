using HaydCalculator.Core.Entities;
using HaydCalculator.Core.Enums;
using System.Collections.ObjectModel;

namespace HaydCalculator.Core
{
    public class HaydCalculatorService
    {
        #region static fields

        public static readonly int MINIMUM_HAYD_DURATION_DAYS = 1;
        public static readonly int MAXIMUM_HAYD_DURATION_DAYS = 15;
        public static readonly int MINIMAL_TUHUR_DURATION_DAYS = 15;

        #endregion  static fields

        private ReadOnlyCollection<FlowDataEntity> _flowData;
        private readonly List<HaydCycleEntity> _haydCycleLst = [];
        private readonly List<FlowDataEntity> _istihadaLst = [];
        private List<FlowDataEntity> _workList = [];

        private HaydCycleEntity _currentIncompleteHaydCycle = null;
        private double _makeUpDayCount = 0;

        public HaydCalculationResultVO Calculate(List<FlowDataEntity> flowDataEntities)
        {
            this._flowData = flowDataEntities.AsReadOnly();

            checkPrerequesites();
            resetCalculationData();

            _workList = _flowData.OrderBy(x => x.FromDateTime).ToList();

            while (_workList.FirstOrDefault() is FlowDataEntity currentData)
            {
                _workList.Remove(currentData);

                if (currentData.Description.IsHaydType)
                    handleHaydFlowData(currentData);
            }

            if (_currentIncompleteHaydCycle != null)
                completeCurrentHaydData();

            return new HaydCalculationResultVO
            {
                HaydFlows = this._haydCycleLst.AsReadOnly(),
                IstihadaFlows = this._istihadaLst.AsReadOnly(),
                MakeUpDayCount = _makeUpDayCount,
            };
        }

        private void checkPrerequesites()
        {
            if (_flowData.None(dataEntry => dataEntry.Description.IsHaydType))
                throw new InfoException(TextUtil.NO_HAYD_FLOW_DATA_PROVIDED);

            if (_flowData.Any(x => x.FromDateTime >= x.ToDateTime))
                throw new InfoException(TextUtil.FLOW_DATA_WITH_INVALID_TIMES);

            if (_flowData.HasOverlaps())
                throw new InfoException(TextUtil.FLOW_DATA_ENTRIES_WITH_OVERLAPPING_TIMES);

            if (_flowData.HasGaps())
                throw new InfoException(TextUtil.FLOW_DATA_ENTRIES_WITH_TIME_GAPS);
        }

        private void resetCalculationData()
        {
            _makeUpDayCount = 0;
            _currentIncompleteHaydCycle = null;
            _workList.Clear();
            _haydCycleLst.Clear();
            _istihadaLst.Clear();
        }

        private void addToHaydDataLst(FlowDataEntity newFlowTimeData)
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
                conditionallyAddInBetweenPurityToMenstruation(newFlowTimeData);
                _currentIncompleteHaydCycle.HaydDataLst.Add(newFlowTimeData);
            }
        }

        private void completeCurrentHaydCycleIfTooLongWithProvidedDate(DateTime dateTime)
        {
            // the current hayd is completed and there needs to be a tuhur now
            if (_currentIncompleteHaydCycle.GetDaysSinceHaydBeginningUntilSpecificDate(dateTime) >= MAXIMUM_HAYD_DURATION_DAYS)
            {
                completeCurrentHaydData();
            }
        }

        private void addToIstihadaLst(FlowDataEntity newFlowTimeData)
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

        private void considerFlowDataAsIstihadaBecauseOfTuhur(FlowDataEntity currentData)
        {
            // istihada
            if (!isMinimalTuhurFulfilledByDate(currentData.FromDateTime)
                // the minimal conceivable time is added to the from date if the ToDateTime is not yet known
                && !isMinimalTuhurFulfilledByDate(currentData.ToDateTime))
            {
                addToIstihadaLst(currentData);
            }
            // first part istihada
            // second part menstruation
            else
            {
                double remainingTuhurDayCount = getRemainingMinimalTuhurDayCountByDate(currentData.FromDateTime);

                if (remainingTuhurDayCount == 0)
                    throw new Exception();

                var normalIstihadaPartFlowTimeData = new FlowDataEntity(currentData)
                {
                    ToDateTime = currentData.FromDateTime.AddDays(getRemainingMinimalTuhurDayCountByDate(currentData.FromDateTime)),
                };

                addToIstihadaLst(normalIstihadaPartFlowTimeData);

                var excessMenstruationPartFlowTimeData = new FlowDataEntity(currentData)
                {
                    FromDateTime = normalIstihadaPartFlowTimeData.ToDateTime
                };

                _workList.Insert(0, excessMenstruationPartFlowTimeData);
            }
        }

        private void handleHaydFlowData(FlowDataEntity currentData)
        {
            if (!currentData.Description.IsHaydType)
                return;

            createCurrentHaydDataIfNotExists();

            // 0 if the hayd has not even begun
            double passedDaysFromCurrentHaydBeginning =
                _currentIncompleteHaydCycle
                    .GetDaysSinceHaydBeginningUntilSpecificDate(currentData.FromDateTime);

            // new flow data would fully fit within the maximum possible hayd duration
            // --> fully hayd
            if (isMinimalTuhurFulfilledByDate(currentData.FromDateTime)
                && passedDaysFromCurrentHaydBeginning + currentData.DayCount <= MAXIMUM_HAYD_DURATION_DAYS)
            {
                addToHaydDataLst(currentData);
            }
            // new flow data would make the hayd go beyond the maximum
            // --> basic or complex istihada
            else
            {
                handleFlowDataAsIstihada(currentData, passedDaysFromCurrentHaydBeginning);
            }
        }

        private void handleFlowDataAsIstihada(FlowDataEntity currentData, double passedDaysFromCurrentHaydBeginning)
        {
            if (passedDaysFromCurrentHaydBeginning + currentData.DayCount <= MAXIMUM_HAYD_DURATION_DAYS && isMinimalTuhurFulfilledByDate(currentData.FromDateTime))
                throw new Exception();

            FlowDataEntity lastFlowData = _currentIncompleteHaydCycle.LastHaydData;

            // if the new flow starts BEFORE the maximum and then would make the hayd surpass the maximum
            // --> complex istihada
            if (lastFlowData == null)
            {
                considerFlowDataAsIstihadaBecauseOfTuhur(currentData);
            }
            else if (passedDaysFromCurrentHaydBeginning < MAXIMUM_HAYD_DURATION_DAYS)
            {
                doComplexIstihada(currentData);
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
                        doComplexIstihada(currentData);
                    }
                    // basic
                    else
                    {
                        completeCurrentHaydCycleIfTooLongWithProvidedDate(currentData.FromDateTime);
                        considerFlowDataAsIstihadaBecauseOfTuhur(currentData);
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

                    completeCurrentHaydCycleIfTooLongWithProvidedDate(currentData.FromDateTime);
                    considerFlowDataAsIstihadaBecauseOfTuhur(currentData);
                }
            }
        }

        private EMustahadaType getCurrentMustahadaType()
        {
            if (_haydCycleLst.None())
                return EMustahadaType.Mubtada_ah;
            else
                return EMustahadaType.Mu3taadah;
        }

        private void doComplexIstihada(FlowDataEntity currentData)
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
            //        doComplexIstihadaForMubtada_ah(toEvaluate);
            //        break;
            //    case EMustahadaType.Mu3taadah:
            //        doComplexIstihadaForMu3taadah(toEvaluate);
            //        break;
            //    case EMustahadaType.Mutahayyirah:
            //        break;

            //    default:
            //        throw new NotImplementedException();
            //}
        }

        private void doComplexIstihadaForMubtada_ah(List<FlowDataEntity> toEvaluate)
        {
            ArgumentNullException.ThrowIfNull(toEvaluate, nameof(toEvaluate));

            if (HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(toEvaluate))
            {

            }
        }

        private void doComplexIstihadaForMu3taadah(List<FlowDataEntity> toEvaluate)
        {
            ArgumentNullException.ThrowIfNull(toEvaluate, nameof(toEvaluate));

            if (HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(toEvaluate))
            {

            }
        }

        //private void determineIfTuhurIsNecessary(FlowDataEntity currentData)
        //{
        //    // the current hayd is completed and there needs to be a tuhur now
        //    if (this._currentIncompleteHaydCycle.GetDaysSinceHaydBeginningUntilSpecificDate(currentData.ToDateTime) >= MAXIMUM_HAYD_DURATION_DAYS)
        //    {
        //        this.completeCurrentHaydData();
        //    }
        //}

        private void createCurrentHaydDataIfNotExists()
        {
            _currentIncompleteHaydCycle ??= new HaydCycleEntity();
        }

        private void completeCurrentHaydData()
        {
            if (_currentIncompleteHaydCycle == null)
                return;

            List<FlowDataEntity> haydFlowWithoutNaqa =
                _currentIncompleteHaydCycle.HaydDataLst.Where(x => !x.IsNaqa).ToList();

            if (haydFlowWithoutNaqa.Sum(x => x.DayCount) < MINIMUM_HAYD_DURATION_DAYS)
            {
                _istihadaLst.AddRange(haydFlowWithoutNaqa);
                _makeUpDayCount += haydFlowWithoutNaqa.Sum(x => x.DayCount);
            }
            else
                _haydCycleLst.Add(_currentIncompleteHaydCycle);

            _currentIncompleteHaydCycle = null;
        }

        private void conditionallyAddInBetweenPurityToMenstruation(FlowDataEntity currentData)
        {
            if (_currentIncompleteHaydCycle.LastHaydData is not FlowDataEntity lastHaydData || !currentData.Description.IsHaydType)
                return;

            double dayGap = (currentData.FromDateTime - lastHaydData.ToDateTime).TotalDays;

            if (dayGap < 0)
                throw new Exception();

            // no gap
            if (dayGap == 0)
                return;

            _flowData
                .Where(x => x.FromDateTime >= lastHaydData.ToDateTime)
                .Where(x => x.ToDateTime <= currentData.FromDateTime)
                .OrderBy(x => x.FromDateTime).ToList()
                .ForEach(x =>
                {
                    addToHaydDataLst(x);
                    x.IsNaqa = true;
                    _makeUpDayCount += x.DayCount;
                });
        }

        public static List<FlowDataEntity> GetFlowDataList(DateTime initialDateTime, List<(EFlowAppearanceColor type, double dayCount)> tupleLst)
        {
            List<FlowDataEntity> list = new(tupleLst.Count);
            DateTime lastBeginningDateTime = initialDateTime;

            foreach (var tupleEntry in tupleLst.Where(x => x.dayCount != 0))
            {
                list.Add(tupleEntry.ToFlowTimeData(lastBeginningDateTime));
                lastBeginningDateTime = list.Last().ToDateTime;
            }

            return list;
        }

        private double getRemainingMinimalTuhurDayCountByDate(DateTime dateTime)
        {
            DateTime? endOfLastCompletedHaydCycle = _haydCycleLst.LastOrDefault()?.HaydEnd;

            if (endOfLastCompletedHaydCycle == null || dateTime < endOfLastCompletedHaydCycle)
                return 0;

            double result = MINIMAL_TUHUR_DURATION_DAYS - (dateTime - endOfLastCompletedHaydCycle.Value).TotalDays;
            return Math.Max(result, 0);
        }

        private bool isMinimalTuhurFulfilledByDate(DateTime dateTime)
        {
            return getRemainingMinimalTuhurDayCountByDate(dateTime) == 0;
        }
    }
}
