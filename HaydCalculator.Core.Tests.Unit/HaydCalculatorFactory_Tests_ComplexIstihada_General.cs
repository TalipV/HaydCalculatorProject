using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Calculator.Services;
using HaydCalculator.Core.Misc;
using NSubstitute.ExceptionExtensions;

namespace HaydCalculator.Core.Tests.Unit
{
    public class HaydCalculatorFactory_Tests_ComplexIstihada_General
    {
        private readonly HaydCalculatorService haydCalculatorFactory = new();

        [Theory]
        [InlineData(11.0, 1.0, 1.0, 5.0)]
        [InlineData(13.0, 1.0, 1.0, 5.0)]
        public void IdentifyComplexIstihadahCaseWithNotImplementedException(double redFlowDayCount1, double clearFlowDayCount, double redFlowDayCount2, double redFlowDayCount3)
        {
            // ARRANGE
            var beginningDate = new DateTime(DateTime.Now.Year, 1, 1);

            List<(EFlowAppearanceColor type, double dayCount)> tuple =
            [
                (EFlowAppearanceColor.Red, redFlowDayCount1),
                (EFlowAppearanceColor.Clear, clearFlowDayCount),
                (EFlowAppearanceColor.Red, redFlowDayCount2),
                (EFlowAppearanceColor.Red, redFlowDayCount3),
            ];

            List<FlowDataEntity> timeData = HaydCalculatorService.GetFlowDataList(beginningDate, tuple);

            // ACT & ASSERT
            Exception exc = Assert.Throws<InfoException>(() => haydCalculatorFactory.Calculate(timeData)); ;
            exc.Message.Should().Be(TextUtil.COMPLEX_ISTIHADA_NOT_IMPLEMENTED);
        }
    }
}
