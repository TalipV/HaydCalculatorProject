using HaydCalculator.Core.Entities;
using HaydCalculator.Core.Enums;
using System.Collections.ObjectModel;

namespace HaydCalculator.Core
{
    public class HaydCalculatorFactory
    {
        #region static fields

        public static readonly int MINIMUM_HAYD_DURATION_DAYS = 1;
        public static readonly int MAXIMUM_HAYD_DURATION_DAYS = 15;
        public static readonly int MINIMAL_TUHUR_DURATION_DAYS = 15;

        #endregion  static fields

        #region constructors
        public HaydCalculatorFactory() { }

        #endregion  constructors

        /// <summary>
        /// Data input which will then be calculated.
        /// </summary>
        public ReadOnlyCollection<FlowDataEntity> DataList { get; set; } = new List<FlowDataEntity>().AsReadOnly();

        /// <summary>
        /// Calculated hayd cycles from <see cref="DataList"/>.
        /// </summary>
        public List<HaydCycleEntity> HaydCycleLst { get; set; } = [];

        /// <summary>
        /// Entries from <see cref="DataList"/> which have been determined to be istihada entries.
        /// </summary>
        public List<FlowDataEntity> IstihadaLst { get; set; } = [];

        /// <summary>
        /// Based on <see cref="DataList"/> and constitutes unhandled <see cref="FlowDataEntity"/> entries.
        /// </summary>
        private List<FlowDataEntity> _workList { get; set; } = null;

        private HaydCycleEntity _currentIncompleteHaydCycle = null;

        public double MakeUpDayCount = 0;

        public bool ConsiderYellowAsHayd { get; set; } = true;

        public void Execute()
        {
            checkPrerequesites();
            ResetCalculationData();

            _workList = DataList.OrderBy(x => x.FromDateTime).ToList();

            while (_workList.FirstOrDefault() is FlowDataEntity currentData)
            {
                _workList.Remove(currentData);

                if (currentData.Description.IsHaydType)
                    handleHaydFlowData(currentData);
            }

            if (_currentIncompleteHaydCycle != null)
                completeCurrentHaydData();
        }

        private void checkPrerequesites()
        {
            if (DataList.None(dataEntry => dataEntry.Description.IsHaydType))
                throw new InfoException(TextUtil.NO_HAYD_FLOW_DATA_PROVIDED);

            if (DataList.Any(x => x.FromDateTime >= x.ToDateTime))
                throw new InfoException(TextUtil.FLOW_DATA_WITH_INVALID_TIMES);

            if (DataList.HasOverlaps())
                throw new InfoException(TextUtil.FLOW_DATA_ENTRIES_WITH_OVERLAPPING_TIMES);

            if (DataList.HasGaps())
                throw new InfoException(TextUtil.FLOW_DATA_ENTRIES_WITH_TIME_GAPS);
        }

        public void ResetCalculationData()
        {
            MakeUpDayCount = 0;
            _currentIncompleteHaydCycle = null;
            _workList = null;
            HaydCycleLst.Clear();
            IstihadaLst.Clear();
        }

        public void AddToHaydDataLst(FlowDataEntity newFlowTimeData)
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

        public void AddToIstihadaLst(FlowDataEntity newFlowTimeData)
        {
            if (IstihadaLst.LastOrDefault() is FlowDataEntity lastData
                && lastData.ToDateTime >= newFlowTimeData.FromDateTime)
            {
                lastData.ToDateTime = newFlowTimeData.ToDateTime;
            }
            else
            {
                IstihadaLst.Add(newFlowTimeData);
            }
        }

        private void considerFlowDataAsIstihadaBecauseOfTuhur(FlowDataEntity currentData)
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
                    throw new Exception();

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
            if (IsMinimalTuhurFulfilledByDate(currentData.FromDateTime)
                && passedDaysFromCurrentHaydBeginning + currentData.DayCount <= MAXIMUM_HAYD_DURATION_DAYS)
            {
                AddToHaydDataLst(currentData);
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
            if (passedDaysFromCurrentHaydBeginning + currentData.DayCount <= MAXIMUM_HAYD_DURATION_DAYS && IsMinimalTuhurFulfilledByDate(currentData.FromDateTime))
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
            if (HaydCycleLst.None())
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
                IstihadaLst.AddRange(haydFlowWithoutNaqa);
                MakeUpDayCount += haydFlowWithoutNaqa.Sum(x => x.DayCount);
            }
            else
                HaydCycleLst.Add(_currentIncompleteHaydCycle);

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

            DataList
                .Where(x => x.FromDateTime >= lastHaydData.ToDateTime)
                .Where(x => x.ToDateTime <= currentData.FromDateTime)
                .OrderBy(x => x.FromDateTime).ToList()
                .ForEach(x =>
                {
                    AddToHaydDataLst(x);
                    x.IsNaqa = true;
                    MakeUpDayCount += x.DayCount;
                });
        }

        public static List<FlowDataEntity> TransformHaydTupleListToFlowTimeDataList(DateTime initialDateTime, List<(EFlowAppearanceColor type, double dayCount)> tupleLst)
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

        public double GetRemainingMinimalTuhurDayCountByDate(DateTime dateTime)
        {
            DateTime? endOfLastCompletedHaydCycle = HaydCycleLst.LastOrDefault()?.HaydEnd;

            if (endOfLastCompletedHaydCycle == null || dateTime < endOfLastCompletedHaydCycle)
                return 0;

            double result = MINIMAL_TUHUR_DURATION_DAYS - (dateTime - endOfLastCompletedHaydCycle.Value).TotalDays;
            return Math.Max(result, 0);
        }

        public bool IsMinimalTuhurFulfilledByDate(DateTime dateTime)
        {
            return GetRemainingMinimalTuhurDayCountByDate(dateTime) == 0;
        }
    }
}
