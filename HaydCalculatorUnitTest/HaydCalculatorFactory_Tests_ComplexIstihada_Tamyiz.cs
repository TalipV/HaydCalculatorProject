using HaydCalculator;
using HaydCalculator.Enums;
using NUnit.Framework;

namespace HaydCalculatorUnitTest
{
    public class HaydCalculatorFactory_Tests_ComplexIstihada_TamyizDetermination
    {
        DateTime beginningDate = new DateTime(DateTime.Now.Year, 1, 1);

        #region Condition 3

        [Test]
        public void Condition3_Fulfilled_TenDaysBlackTenDaysRedAndStop()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Black, 10), (EFlowAppearanceColor.Red, 10),
            };

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(beginningDate, tuple);
            HaydCycleEntity haydCycleEntity = new HaydCycleEntity();
            haydCycleEntity.HaydDataLst = timeData;

            //ACT & ASSERT
            Assert.IsTrue(HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydDataLst));
        }

        [Test]
        public void Condition3_NotFulfilled_TenDaysRedTenDaysBlackAndStop()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Red, 10), (EFlowAppearanceColor.Black, 10),
            };

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(beginningDate, tuple);
            HaydCycleEntity haydCycleEntity = new HaydCycleEntity();
            haydCycleEntity.HaydDataLst = timeData;

            //ACT & ASSERT
            Assert.IsFalse(HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydDataLst));
        }

        [Test]
        public void Condition3_Fulfilled_1DayBlackAnd14DaysRed()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 14),
            };

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(beginningDate, tuple);
            HaydCycleEntity haydCycleEntity = new HaydCycleEntity();
            haydCycleEntity.HaydDataLst = timeData;

            //ACT & ASSERT
            Assert.IsTrue(HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydDataLst));
        }

        [Test]
        public void Condition3_NotFulfilled_1DayBlack14DaysRedAndThenBlackAgain()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 14), (EFlowAppearanceColor.Black, 1)
            };

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(beginningDate, tuple);
            HaydCycleEntity haydCycleEntity = new HaydCycleEntity();
            haydCycleEntity.HaydDataLst = timeData;

            //ACT & ASSERT
            Assert.IsFalse(HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydDataLst));
        }

        #endregion Condition 3

        #region Condition 4

        [Test]
        public void Condition4_ContinuouslyBlackAndRedAndAtTheEndLongRed()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 10),
            };

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(beginningDate, tuple);
            HaydCycleEntity haydCycleEntity = new HaydCycleEntity();
            haydCycleEntity.HaydDataLst = timeData;

            //ACT & ASSERT
            Assert.IsTrue(HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydDataLst));
        }

        [Test]
        public void Condition4_ContinuouslyBlackAndRed()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1),
            };

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(beginningDate, tuple);
            HaydCycleEntity haydCycleEntity = new HaydCycleEntity();
            haydCycleEntity.HaydDataLst = timeData;

            //ACT & ASSERT
            Assert.IsFalse(HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydDataLst));
        }

        #endregion Condition 4
    }
}
