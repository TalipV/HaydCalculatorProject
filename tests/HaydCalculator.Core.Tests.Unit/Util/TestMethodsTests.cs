using HaydCalculator.Core.Calculator.Models;

namespace HaydCalculator.Core.Tests.Unit.Util;

public class TestMethodsTests
{
    [Fact]
    public void GetFlowDataList_ShouldCreateCorrectNumberOfFlows_WhenNonZeroDayCounts()
    {
        // ARRANGE
        var initialInstant = Instant.FromUtc(2024, 4, 1, 0, 0);
        List<(EFlowAppearanceColor, double)> tupleList =
        [
            (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 2),
            (EFlowAppearanceColor.Clear, 0.5)
        ];

        // ACT
        List<FlowDataEntity> flows = TestMethods.GetFlowDataList(tupleList, initialInstant);

        // ASSERT
        flows.Should().HaveCount(3);
    }

    [Fact]
    public void GetFlowDataList_ShouldIgnoreZeroDayCountEntries()
    {
        // ARRANGE
        var initialInstant = Instant.FromUtc(2024, 4, 1, 0, 0);
        List<(EFlowAppearanceColor, double)> tupleList =
        [
            (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 0),
            (EFlowAppearanceColor.Clear, 1)
        ];

        // ACT
        List<FlowDataEntity> flows = TestMethods.GetFlowDataList(tupleList, initialInstant);

        // ASSERT
        flows.Should().HaveCount(2);
        flows.Should().NotContain(f => f.To == f.From);
    }

    [Fact]
    public void GetFlowDataList_ShouldSetCorrectDatesForFlows()
    {
        // ARRANGE
        var initialInstant = Instant.FromUtc(2024, 4, 1, 0, 0);
        List<(EFlowAppearanceColor, double)> tupleList =
        [
            (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 2)
        ];

        // ACT
        List<FlowDataEntity> flows = TestMethods.GetFlowDataList(tupleList, initialInstant);

        // ASSERT
        flows[0].From.Should().Be(initialInstant);
        flows[0].To.Should().Be(initialInstant + Duration.FromDays(1));
        flows[1].From.Should().Be(initialInstant + Duration.FromDays(1));
        flows[1].To.Should().Be(initialInstant + Duration.FromDays(3));
    }

    [Fact]
    public void CheckFlowsInOrder_ShouldReturnTrue_WhenFlowsAreConsecutive()
    {
        // ARRANGE
        var initialInstant = Instant.FromUtc(2024, 4, 1, 0, 0);
        List<FlowDataEntity> flows = TestMethods.GetFlowDataList(
            [
                (EFlowAppearanceColor.Red, 1),
                (EFlowAppearanceColor.Black, 1)
            ],
            initialInstant);

        // ACT
        bool result = TestMethods.CheckFlowsInOrder(flows);

        // ASSERT
        result.Should().BeTrue();
    }

    [Fact]
    public void CheckFlowsInOrder_ShouldReturnFalse_WhenThereIsAGapBetweenFlows()
    {
        // ARRANGE
        var initialInstant = Instant.FromUtc(2024, 4, 1, 0, 0);
        List<FlowDataEntity> flows =
        [
            new FlowDataEntity { From = initialInstant, To = initialInstant + Duration.FromDays(1) },
            new FlowDataEntity { From = initialInstant + Duration.FromDays(2), To = initialInstant + Duration.FromDays(3) }
        ];

        // ACT
        bool result = TestMethods.CheckFlowsInOrder(flows);

        // ASSERT
        result.Should().BeFalse();
    }

    [Fact]
    public void CheckFlowsInOrder_ShouldReturnTrue_WhenFlowsListIsEmpty()
    {
        // ARRANGE
        var flows = new List<FlowDataEntity>();

        // ACT
        bool result = TestMethods.CheckFlowsInOrder(flows);

        // ASSERT
        result.Should().BeTrue();
    }

    [Fact]
    public void CheckFlowsInOrder_ShouldReturnTrue_WhenFlowsListHasOnlyOneElement()
    {
        // ARRANGE
        var initialInstant = Instant.FromUtc(2024, 4, 1, 0, 0);
        List<FlowDataEntity> flows =
        [
            new FlowDataEntity { From = initialInstant, To = initialInstant + Duration.FromDays(1) }
        ];

        // ACT
        bool result = TestMethods.CheckFlowsInOrder(flows);

        // ASSERT
        result.Should().BeTrue();
    }
}
