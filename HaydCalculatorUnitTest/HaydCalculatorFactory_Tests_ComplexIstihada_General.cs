using HaydCalculator;
using HaydCalculator.Entities;
using HaydCalculator.Enums;
using NUnit.Framework;

namespace HaydCalculatorUnitTest
{
    public class HaydCalculatorFactory_Tests_ComplexIstihada_General
    {
        private HaydCalculatorFactory haydCalculatorFactory = new HaydCalculatorFactory();

        [Test]
        [TestCase(11.0, 1.0, 1.0, 5.0)]
        [TestCase(13.0, 1.0, 1.0, 5.0)]
        public void IdentifyComplexIstihadahCaseWithNotImplementedException(double redFlowDayCount1, double clearFlowDayCount, double redFlowDayCount2, double redFlowDayCount3)
        {
            //ARRANGE
            DateTime beginningDate = new DateTime(DateTime.Now.Year, 1, 1);

            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Red, redFlowDayCount1),
                (EFlowAppearanceColor.Clear, clearFlowDayCount),
                (EFlowAppearanceColor.Red, redFlowDayCount2),
                (EFlowAppearanceColor.Red, redFlowDayCount3),
            };

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(beginningDate, tuple);
            haydCalculatorFactory.DataList = timeData.AsReadOnly();

            //ACT & ASSERT
            Exception exc = Assert.Throws<InfoException>(() => haydCalculatorFactory.Execute()); ;
            Assert.That(exc.Message, Is.EqualTo(TextUtil.COMPLEX_ISTIHADA_NOT_IMPLEMENTED));
        }
    }
}
