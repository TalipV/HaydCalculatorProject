using HaydCalculator.Entities;
using HaydCalculator.Enums;
using System.Collections.ObjectModel;

namespace HaydCalculator
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
        public List<HaydCycleEntity> HaydCycleLst { get; set; } = new List<HaydCycleEntity>();

        /// <summary>
        /// Entries from <see cref="DataList"/> which have been determined to be istihada entries.
        /// </summary>
        public List<FlowDataEntity> IstihadaLst { get; set; } = new List<FlowDataEntity>();

        /// <summary>
        /// Based on <see cref="DataList"/> and constitutes unhandled <see cref="FlowDataEntity"/> entries.
        /// </summary>
        private List<FlowDataEntity> _workList { get; set; } = null;

        private HaydCycleEntity _currentIncompleteHaydCycle = null;

        public double MakeUpDayCount = 0;

        public bool ConsiderYellowAsHayd { get; set; } = true;

        public void Execute()
        {
            this.checkPrerequesites();
            this.ResetCalculationData();

            this._workList =
                this.DataList
                    .OrderBy(x => x.FromDateTime)
                    .ToList();

            while (this._workList.FirstOrDefault() is FlowDataEntity currentData)
            {
                this._workList.Remove(currentData);

                if (currentData.Description.IsHaydType)
                    handleHaydFlowData(currentData);
            }

            if (_currentIncompleteHaydCycle != null)
                this.completeCurrentHaydData();
        }

        private void checkPrerequesites()
        {
            if (this.DataList.None(dataEntry => dataEntry.Description.IsHaydType))
                throw new InfoException(TextUtil.NO_HAYD_FLOW_DATA_PROVIDED);

            if (this.DataList.Any(x => x.FromDateTime >= x.ToDateTime))
                throw new InfoException(TextUtil.FLOW_DATA_WITH_INVALID_TIMES);

            if (this.DataList.HasOverlaps())
                throw new InfoException(TextUtil.FLOW_DATA_ENTRIES_WITH_OVERLAPPING_TIMES);

            if (this.DataList.HasGaps())
                throw new InfoException(TextUtil.FLOW_DATA_ENTRIES_WITH_TIME_GAPS);
        }

        public void ResetCalculationData()
        {
            this.MakeUpDayCount = 0;
            this._currentIncompleteHaydCycle = null;
            this._workList = null;
            this.HaydCycleLst.Clear();
            this.IstihadaLst.Clear();
        }

        public void AddToHaydDataLst(FlowDataEntity newFlowTimeData)
        {
            // if last hayd data is of the same type and could just be merged with the new one
            if (this._currentIncompleteHaydCycle.LastHaydData is FlowDataEntity lastData
                && lastData.ToDateTime == newFlowTimeData.FromDateTime
                && lastData.Description.Equals(newFlowTimeData.Description))
            {
                lastData.ToDateTime = newFlowTimeData.ToDateTime;
            }
            else
            {
                this.conditionallyAddInBetweenPurityToMenstruation(newFlowTimeData);
                this._currentIncompleteHaydCycle.HaydDataLst.Add(newFlowTimeData);
            }
        }

        private void completeCurrentHaydCycleIfTooLongWithProvidedDate(DateTime dateTime)
        {
            // the current hayd is completed and there needs to be a tuhur now
            if (this._currentIncompleteHaydCycle.GetDaysSinceHaydBeginningUntilSpecificDate(dateTime) >= MAXIMUM_HAYD_DURATION_DAYS)
            {
                this.completeCurrentHaydData();
            }
        }

        public void AddToIstihadaLst(FlowDataEntity newFlowTimeData)
        {
            if (this.IstihadaLst.LastOrDefault() is FlowDataEntity lastData
                && lastData.ToDateTime >= newFlowTimeData.FromDateTime)
            {
                lastData.ToDateTime = newFlowTimeData.ToDateTime;
            }
            else
            {
                this.IstihadaLst.Add(newFlowTimeData);
            }
        }

        private void considerFlowDataAsIstihadaBecauseOfTuhur(FlowDataEntity currentData)
        {
            // istihada
            if (!this.IsMinimalTuhurFulfilledByDate(currentData.FromDateTime) 
                // the minimal conceivable time is added to the from date if the ToDateTime is not yet known
                && !this.IsMinimalTuhurFulfilledByDate(currentData.ToDateTime))
            {
                this.AddToIstihadaLst(currentData);
            }
            // first part istihada
            // second part menstruation
            else
            {
                double remainingTuhurDayCount = this.GetRemainingMinimalTuhurDayCountByDate(currentData.FromDateTime);

                if (remainingTuhurDayCount == 0)
                    throw new Exception();

                FlowDataEntity normalIstihadaPartFlowTimeData = new FlowDataEntity(currentData)
                {
                    ToDateTime = currentData.FromDateTime.AddDays(this.GetRemainingMinimalTuhurDayCountByDate(currentData.FromDateTime)),
                };

                this.AddToIstihadaLst(normalIstihadaPartFlowTimeData);

                FlowDataEntity excessMenstruationPartFlowTimeData = new FlowDataEntity(currentData)
                {
                    FromDateTime = normalIstihadaPartFlowTimeData.ToDateTime
                };

                this._workList.Insert(0, excessMenstruationPartFlowTimeData);
            }
        }

        private void handleHaydFlowData(FlowDataEntity currentData)
        {
            if (!currentData.Description.IsHaydType)
                return;

            this.createCurrentHaydDataIfNotExists();

            // 0 if the hayd has not even begun
            double passedDaysFromCurrentHaydBeginning = 
                this._currentIncompleteHaydCycle
                    .GetDaysSinceHaydBeginningUntilSpecificDate(currentData.FromDateTime);

            // new flow data would fully fit within the maximum possible hayd duration
            // --> fully hayd
            if (this.IsMinimalTuhurFulfilledByDate(currentData.FromDateTime)
                && (passedDaysFromCurrentHaydBeginning + currentData.DayCount) <= MAXIMUM_HAYD_DURATION_DAYS)
            {
                this.AddToHaydDataLst(currentData);
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
            if ((passedDaysFromCurrentHaydBeginning + currentData.DayCount) <= MAXIMUM_HAYD_DURATION_DAYS && this.IsMinimalTuhurFulfilledByDate(currentData.FromDateTime))
                throw new Exception();

            FlowDataEntity lastFlowData = this._currentIncompleteHaydCycle.LastHaydData;

            // if the new flow starts BEFORE the maximum and then would make the hayd surpass the maximum
            // --> complex istihada
            if (lastFlowData == null)
            {
                this.considerFlowDataAsIstihadaBecauseOfTuhur(currentData);
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
                        this.completeCurrentHaydCycleIfTooLongWithProvidedDate(currentData.FromDateTime);
                        this.considerFlowDataAsIstihadaBecauseOfTuhur(currentData);
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

                    this.completeCurrentHaydCycleIfTooLongWithProvidedDate(currentData.FromDateTime);
                    this.considerFlowDataAsIstihadaBecauseOfTuhur(currentData);
                }
            }
        }

        private EMustahadaType GetCurrentMustahadaType()
        {
            if (this.HaydCycleLst.None())
                return EMustahadaType.Mubtada_ah;
            else
                return EMustahadaType.Mu3taadah;
        }

        private void doComplexIstihada(FlowDataEntity currentData)
        {
            if (currentData == null)
                throw new ArgumentException();
            if (this._currentIncompleteHaydCycle == null)
                throw new ArgumentException();

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
            if (toEvaluate == null)
                throw new ArgumentException();

            if (HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(toEvaluate))
            {

            }
        }

        private void doComplexIstihadaForMu3taadah(List<FlowDataEntity> toEvaluate)
        {
            if (toEvaluate == null)
                throw new ArgumentException();

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
            if (this._currentIncompleteHaydCycle == null)
                this._currentIncompleteHaydCycle = new HaydCycleEntity();
        }

        private void completeCurrentHaydData()
        {
            if (this._currentIncompleteHaydCycle == null)
                return;

            if (this._currentIncompleteHaydCycle.HaydDurationDays < MINIMUM_HAYD_DURATION_DAYS)
            {
                this.IstihadaLst.AddRange(this._currentIncompleteHaydCycle.HaydDataLst);
                this._currentIncompleteHaydCycle.HaydDataLst.ForEach(x => x.IsNaqa = true);
                this.MakeUpDayCount += (this._currentIncompleteHaydCycle.HaydDataLst).Sum(x => x.DayCount);
            }
            else
                this.HaydCycleLst.Add(this._currentIncompleteHaydCycle);

            this._currentIncompleteHaydCycle = null;
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

            this.DataList
                .Where(x => x.FromDateTime >= lastHaydData.ToDateTime)
                .Where(x => x.ToDateTime <= currentData.FromDateTime)
                .OrderBy(x => x.FromDateTime).ToList()
                .ForEach(x =>
                {
                    this.AddToHaydDataLst(x);
                    x.IsNaqa = true;
                    this.MakeUpDayCount += x.DayCount;
                });
        }

        public static List<FlowDataEntity> TransformHaydTupleListToFlowTimeDataList(DateTime initialDateTime, List<(EFlowAppearanceColor type, double dayCount)> tupleLst)
        {
            List<FlowDataEntity> list = new List<FlowDataEntity>(tupleLst.Count);
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
            DateTime? endOfLastCompletedHaydCycle = this.HaydCycleLst.LastOrDefault()?.HaydEnd;

            if (endOfLastCompletedHaydCycle == null || dateTime < endOfLastCompletedHaydCycle)
                return 0;

            double result = MINIMAL_TUHUR_DURATION_DAYS - (dateTime - endOfLastCompletedHaydCycle.Value).TotalDays;
            return Math.Max(result, 0);
        }

        public bool IsMinimalTuhurFulfilledByDate(DateTime dateTime)
        {
            return this.GetRemainingMinimalTuhurDayCountByDate(dateTime) == 0;
        }
    }
}
