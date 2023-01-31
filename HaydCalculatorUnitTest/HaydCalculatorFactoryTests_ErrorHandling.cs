using HaydCalculator;
using HaydCalculator.Entities;
using HaydCalculator.Enums;
using NUnit.Framework;

namespace HaydCalculatorUnitTest
{
    public class HaydCalculatorFactoryTests_ErrorHandling
    {
        private HaydCalculatorFactory haydCalculatorFactory = new HaydCalculatorFactory();

        [Test]
        public void ThrowExceptionWhenNoFlowDataIsHaydType()
        {
            //ARRANGE
            DateTime date = new DateTime(year: 2000, month: 5, day: 20);

            List<FlowDataEntity> timeData = new List<FlowDataEntity>()
            {
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
            };

            haydCalculatorFactory.DataList = timeData.AsReadOnly();

            //ACT & ASSERT
            Exception exc = Assert.Throws<InfoException>(() => haydCalculatorFactory.Execute());
            Assert.That(exc.Message, Is.EqualTo(TextUtil.NO_HAYD_FLOW_DATA_PROVIDED));
        }

        [Test]
        [TestCase(0)]
        [TestCase(5)]
        public void ThrowExceptionWhenFlowDataToDateIsEqualToOrSmallerThanFromDate(int dayDifference)
        {
            //ARRANGE
            DateTime date = new DateTime(year: 2000, month: 5, day: 20);

            List<FlowDataEntity> timeData = new List<FlowDataEntity>()
            {
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
            };

            haydCalculatorFactory.DataList = timeData.AsReadOnly();

            //ACT & ASSERT
            Exception exc = Assert.Throws<InfoException>(() => haydCalculatorFactory.Execute());
            Assert.That(exc.Message, Is.EqualTo(TextUtil.FLOW_DATA_WITH_INVALID_TIMES));
        }

        [Test]
        public void ThrowExceptionWhenMultipleFlowDataOverlapEachOther()
        {
            //ARRANGE
            DateTime date = new DateTime(year: 2000, month: 5, day: 20);

            List<FlowDataEntity> timeData = new List<FlowDataEntity>()
            {
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
            };

            haydCalculatorFactory.DataList = timeData.AsReadOnly();

            //ACT & ASSERT
            Exception exc = Assert.Throws<InfoException>(() => haydCalculatorFactory.Execute());
            Assert.That(exc.Message, Is.EqualTo(TextUtil.FLOW_DATA_ENTRIES_WITH_OVERLAPPING_TIMES));
        }
        [Test]
        public void ThrowExceptionWhenGapsBetweenFlowDataEntries()
        {
            //ARRANGE
            DateTime date = new DateTime(year: 2000, month: 5, day: 20);

            List<FlowDataEntity> timeData = new List<FlowDataEntity>()
            {
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
            };

            haydCalculatorFactory.DataList = timeData.AsReadOnly();

            //ACT & ASSERT
            Exception exc = Assert.Throws<InfoException>(() => haydCalculatorFactory.Execute());
            Assert.That(exc.Message, Is.EqualTo(TextUtil.FLOW_DATA_ENTRIES_WITH_TIME_GAPS));
        }
    }
}