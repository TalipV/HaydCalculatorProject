using HaydCalculator.Core.Calculator.Models;

namespace HaydCalculator.Core.Tests.Unit.Util
{
    public class TestMethodsTests
    {
        [Fact]
        public void GetFlowDataList_ShouldCreateCorrectNumberOfFlows_WhenNonZeroDayCounts()
        {
            // ARRANGE
            var initialDateTime = new DateTime(2024, 4, 1);
            List<(EFlowAppearanceColor, double)> tupleList =
            [
                (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 2),
                (EFlowAppearanceColor.Clear, 0.5)
            ];

            // ACT
            var flows = TestMethods.GetFlowDataList(tupleList, initialDateTime);

            // ASSERT
            flows.Should().HaveCount(3);
        }

        [Fact]
        public void GetFlowDataList_ShouldIgnoreZeroDayCountEntries()
        {
            // ARRANGE
            var initialDateTime = new DateTime(2024, 4, 1);
            List<(EFlowAppearanceColor, double)> tupleList =
            [
                (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 0),
                (EFlowAppearanceColor.Clear, 1)
            ];

            // ACT
            var flows = TestMethods.GetFlowDataList(tupleList, initialDateTime);

            // ASSERT
            flows.Should().HaveCount(2);
            flows.Should().NotContain(f => f.ToDateTime == f.FromDateTime);
        }

        [Fact]
        public void GetFlowDataList_ShouldSetCorrectDatesForFlows()
        {
            // ARRANGE
            var initialDateTime = new DateTime(2024, 4, 1);
            List<(EFlowAppearanceColor, double)> tupleList =
            [
                (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 2)
            ];

            // ACT
            var flows = TestMethods.GetFlowDataList(tupleList, initialDateTime);

            // ASSERT
            flows[0].FromDateTime.Should().Be(initialDateTime);
            flows[0].ToDateTime.Should().Be(initialDateTime.AddDays(1));
            flows[1].FromDateTime.Should().Be(initialDateTime.AddDays(1));
            flows[1].ToDateTime.Should().Be(initialDateTime.AddDays(3));
        }

        [Fact]
        public void CheckFlowsInOrder_ShouldReturnTrue_WhenFlowsAreConsecutive()
        {
            // ARRANGE
            var initialDateTime = DateTime.Now;
            var flows = TestMethods.GetFlowDataList(
                new List<(EFlowAppearanceColor, double)>
                {
                    (EFlowAppearanceColor.Red, 1),
                    (EFlowAppearanceColor.Black, 1)
                },
                initialDateTime);

            // ACT
            var result = TestMethods.CheckFlowsInOrder(flows);

            // ASSERT
            result.Should().BeTrue();
        }

        [Fact]
        public void CheckFlowsInOrder_ShouldReturnFalse_WhenThereIsAGapBetweenFlows()
        {
            // ARRANGE
            var initialDateTime = DateTime.Now;
            List<FlowDataEntity> flows =
            [
                new FlowDataEntity { FromDateTime = initialDateTime, ToDateTime = initialDateTime.AddDays(1) },
                new FlowDataEntity { FromDateTime = initialDateTime.AddDays(2), ToDateTime = initialDateTime.AddDays(3) }
            ];

            // ACT
            var result = TestMethods.CheckFlowsInOrder(flows);

            // ASSERT
            result.Should().BeFalse();
        }

        [Fact]
        public void CheckFlowsInOrder_ShouldReturnTrue_WhenFlowsListIsEmpty()
        {
            // ARRANGE
            var flows = new List<FlowDataEntity>();

            // ACT
            var result = TestMethods.CheckFlowsInOrder(flows);

            // ASSERT
            result.Should().BeTrue();
        }

        [Fact]
        public void CheckFlowsInOrder_ShouldReturnTrue_WhenFlowsListHasOnlyOneElement()
        {
            // ARRANGE
            var initialDateTime = DateTime.Now;
            List<FlowDataEntity> flows =
            [
                new FlowDataEntity { FromDateTime = initialDateTime, ToDateTime = initialDateTime.AddDays(1) }
            ];

            // ACT
            var result = TestMethods.CheckFlowsInOrder(flows);

            // ASSERT
            result.Should().BeTrue();
        }
    }
}
