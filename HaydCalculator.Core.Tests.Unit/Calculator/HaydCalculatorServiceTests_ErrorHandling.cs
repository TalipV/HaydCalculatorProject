using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Calculator.Services;
using HaydCalculator.Core.Misc;

namespace HaydCalculator.Core.Tests.Unit.Calculator
{
    public class HaydCalculatorServiceTests_ErrorHandling
    {
        private readonly HaydCalculatorService haydCalculatorFactory = new();

        [Fact]
        public void ThrowExceptionWhenNoFlowDataIsHaydType()
        {
            // ARRANGE
            var date = new DateTime(year: 2000, month: 5, day: 20);

            List<FlowDataEntity> timeData =
            [
                new FlowDataEntity()
                {
                    FromDateTime = date,
                    ToDateTime = date.AddDays(5),
                    Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Clear },
                },
                new FlowDataEntity()
                {
                    FromDateTime = date.AddDays(10),
                    ToDateTime = date.AddDays(15),
                    Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Clear },
                },
            ];

            // ACT & ASSERT
            Exception exc = Assert.Throws<InfoException>(() => haydCalculatorFactory.Calculate(timeData));
            exc.Message.Should().Be(TextUtil.NO_HAYD_FLOW_DATA_PROVIDED);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void ThrowExceptionWhenFlowDataToDateIsEqualToOrSmallerThanFromDate(int dayDifference)
        {
            // ARRANGE
            var date = new DateTime(year: 2000, month: 5, day: 20);

            List<FlowDataEntity> timeData =
            [
                new FlowDataEntity()
                {
                    FromDateTime = date,
                    ToDateTime = date.AddDays(-dayDifference),
                    Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
                },
                new FlowDataEntity()
                {
                    FromDateTime = date.AddDays(dayDifference*2),
                    ToDateTime = date.AddDays(dayDifference*3),
                    Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
                },
            ];

            // ACT & ASSERT
            Exception exc = Assert.Throws<InfoException>(() => haydCalculatorFactory.Calculate(timeData));
            exc.Message.Should().Be(TextUtil.FLOW_DATA_WITH_INVALID_TIMES);
        }

        [Fact]
        public void ThrowExceptionWhenMultipleFlowDataOverlapEachOther()
        {
            // ARRANGE
            var date = new DateTime(year: 2000, month: 5, day: 20);

            List<FlowDataEntity> timeData =
            [
                new FlowDataEntity()
                {
                    FromDateTime = date.AddDays(3),
                    ToDateTime = date.AddDays(5),
                    Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
                },
                new FlowDataEntity()
                {
                    FromDateTime = date.AddDays(4),
                    ToDateTime = date.AddDays(6),
                    Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
                },
            ];

            // ACT & ASSERT
            Exception exc = Assert.Throws<InfoException>(() => haydCalculatorFactory.Calculate(timeData));
            exc.Message.Should().Be(TextUtil.FLOW_DATA_ENTRIES_WITH_OVERLAPPING_TIMES);
        }

        [Fact]
        public void ThrowExceptionWhenGapsBetweenFlowDataEntries()
        {
            // ARRANGE
            var date = new DateTime(year: 2000, month: 5, day: 20);

            List<FlowDataEntity> timeData =
            [
                new FlowDataEntity()
                {
                    FromDateTime = date.AddDays(3),
                    ToDateTime = date.AddDays(5),
                    Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
                },
                new FlowDataEntity()
                {
                    FromDateTime = date.AddDays(6),
                    ToDateTime = date.AddDays(7),
                    Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
                },
            ];

            // ACT & ASSERT
            Exception exc = Assert.Throws<InfoException>(() => haydCalculatorFactory.Calculate(timeData));
            exc.Message.Should().Be(TextUtil.FLOW_DATA_ENTRIES_WITH_TIME_GAPS);
        }
    }
}