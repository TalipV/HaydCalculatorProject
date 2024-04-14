using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Tests.Unit.Util;

namespace HaydCalculator.Core.Tests.Unit.Calculator
{
    public class HaydCalculatorFactory_Tests_ComplexIstihada_TamyizDetermination
    {
        private readonly DateTime startDate = new DateTime(2024, 1, 1);

        #region Condition 3

        [Fact]
        public void Condition3_Fulfilled_TenDaysBlackTenDaysRedAndStop()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> data =
            [
                (EFlowAppearanceColor.Black, 10), (EFlowAppearanceColor.Red, 10),
            ];

            List<FlowDataEntity> timeData = data.GetFlowDataList(initialDateTime: startDate);
            var haydCycleEntity = new HaydCycleEntity
            {
                HaydFlows = timeData
            };

            // ACT & ASSERT
            HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeTrue();
        }

        [Fact]
        public void Condition3_NotFulfilled_TenDaysRedTenDaysBlackAndStop()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> data =
            [
                (EFlowAppearanceColor.Red, 10), (EFlowAppearanceColor.Black, 10),
            ];

            List<FlowDataEntity> timeData = data.GetFlowDataList(initialDateTime: startDate);
            var haydCycleEntity = new HaydCycleEntity
            {
                HaydFlows = timeData
            };

            // ACT & ASSERT
            HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeFalse();
        }

        [Fact]
        public void Condition3_Fulfilled_1DayBlackAnd14DaysRed()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> data =
            [
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 14),
            ];

            List<FlowDataEntity> timeData = data.GetFlowDataList(initialDateTime: startDate);
            var haydCycleEntity = new HaydCycleEntity
            {
                HaydFlows = timeData
            };

            // ACT & ASSERT
            HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeTrue();
        }

        [Fact]
        public void Condition3_NotFulfilled_1DayBlack14DaysRedAndThenBlackAgain()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> data =
            [
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 14), (EFlowAppearanceColor.Black, 1)
            ];

            List<FlowDataEntity> timeData = data.GetFlowDataList(initialDateTime: startDate);
            var haydCycleEntity = new HaydCycleEntity
            {
                HaydFlows = timeData
            };

            // ACT & ASSERT
            HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeFalse();
        }

        #endregion Condition 3

        #region Condition 4

        [Fact]
        public void Condition4_ContinuouslyBlackAndRedAndAtTheEndLongRed()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> data =
            [
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 10),
            ];

            List<FlowDataEntity> timeData = data.GetFlowDataList(initialDateTime: startDate);
            var haydCycleEntity = new HaydCycleEntity
            {
                HaydFlows = timeData
            };

            // ACT & ASSERT
            HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeTrue();
        }

        [Fact]
        public void Condition4_ContinuouslyBlackAndRed()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> data =
            [
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1),
            ];

            List<FlowDataEntity> timeData = data.GetFlowDataList(initialDateTime: startDate);
            var haydCycleEntity = new HaydCycleEntity
            {
                HaydFlows = timeData
            };

            // ACT & ASSERT
            HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeFalse();
        }

        #endregion Condition 4
    }
}
