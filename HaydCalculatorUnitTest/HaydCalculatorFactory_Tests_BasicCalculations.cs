using HaydCalculator;
using HaydCalculator.Entities;
using HaydCalculator.Enums;
using NUnit.Framework;

namespace HaydCalculatorUnitTest
{
    public class HaydCalculatorFactoryTests_BasicCalculations
    {
        private DateTime _beginningDate = new DateTime(DateTime.Now.Year, 1, 1);
        private HaydCalculatorFactory _haydCalculatorFactory = new HaydCalculatorFactory();

        [Test] // Self explanatory.
        public void HaydCalculation_MinimalHaydDurationNotFulfilled()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Red, 0.4),
                (EFlowAppearanceColor.Clear, 2.0),
                (EFlowAppearanceColor.Red, 0.1),
            };

            _haydCalculatorFactory.DataList = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple).AsReadOnly();

            //ACT
            _haydCalculatorFactory.Execute();

            //ASSERT
            Assert.IsEmpty(_haydCalculatorFactory.HaydCycleLst);
            Assert.IsTrue(_haydCalculatorFactory.IstihadaLst.Count == 2);
            Assert.IsTrue(_haydCalculatorFactory.IstihadaLst.First().DayCount == 0.5);
        }

        [Test] // Self explanatory.
        public void HaydCalculation_MinimalHaydDurationBarelyFulfilled()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Red, 1.0),
            };

            _haydCalculatorFactory.DataList = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple).AsReadOnly();

            //ACT
            _haydCalculatorFactory.Execute();

            //ASSERT
            Assert.IsEmpty(_haydCalculatorFactory.IstihadaLst);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst.Count == 1);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count == 1);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[0].DayCount == 1.0);
        }

        [Test] // Program specific test.
        public void HaydCalculation_ConnectSuccessiveSameFlows()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Red, 1.0),
                (EFlowAppearanceColor.Black, 2.0),
                (EFlowAppearanceColor.Black, 3.0),
            };

            _haydCalculatorFactory.DataList = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple).AsReadOnly();

            //ACT
            _haydCalculatorFactory.Execute();

            //ASSERT
            Assert.IsEmpty(_haydCalculatorFactory.IstihadaLst);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst.Count == 1);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count == 2);

            FlowDataEntity firstFlowData = _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[0];
            FlowDataEntity secondFlowData = _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[1];

            Assert.IsTrue(firstFlowData.DayCount == 1.0);
            Assert.IsTrue(firstFlowData.Description.FlowAppearanceColorEnum == EFlowAppearanceColor.Red);
            Assert.IsTrue(secondFlowData.DayCount == 5.0);
            Assert.IsTrue(secondFlowData.Description.FlowAppearanceColorEnum == EFlowAppearanceColor.Black);
        }

        [Test]  // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1591]
        public void HaydCalculation_CalculationWithPurityInBetweenTwoMenstrualFlows()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Red, 3.0),
                (EFlowAppearanceColor.Clear, 7.0),
                (EFlowAppearanceColor.Red, 1.0),
            };

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple);
            _haydCalculatorFactory.DataList = timeData.AsReadOnly();

            //ACT
            _haydCalculatorFactory.Execute();

            //ASSERT
            Assert.IsEmpty(_haydCalculatorFactory.IstihadaLst);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst.Count == 1);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count == 3);

            for (int i = 0; i < tuple.Count; i++)
            {
                Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.ElementAt(i) == timeData.ElementAt(i));
            }
        }

        [Test]  // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1750]
        public void HaydCalculation_CalculationWithPurityBetweenMenstrualFlowAndIstihadaFlow()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Red, 7.0),
                (EFlowAppearanceColor.Clear, 8.0),

                // intentionally split up into two because this tests another potential bug

                // TODO: Separater Test? Problem war, dass nachdem ein Überschuss als Istihada festgestellt wurde,
                // der nächste Überschuss nicht korrekt verarbeitet wurde.
                // UND gleiche aufeinanderfolgende Istihada-Einträge wurden nicht korrekt zu einem Eintrag vereinigt.
                (EFlowAppearanceColor.Red, 1.0),
                (EFlowAppearanceColor.Red, 1.0),
            };

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple);
            _haydCalculatorFactory.DataList = timeData.AsReadOnly();

            //ACT
            _haydCalculatorFactory.Execute();

            //ASSERT
            Assert.IsTrue(_haydCalculatorFactory.IstihadaLst.Count == 1);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst.Count == 1);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count == 1);

            FlowDataEntity menstruation = _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[0];
            FlowDataEntity istihada = _haydCalculatorFactory.IstihadaLst[0];

            Assert.IsTrue(menstruation == timeData.First());
            //Assert.IsTrue(istihada == timeData.Last());

            Assert.That((EFlowAppearanceColor.Red, 7.0).ToFlowTimeData(_beginningDate), Is.EqualTo(menstruation));
            Assert.That((EFlowAppearanceColor.Red, 2.0).ToFlowTimeData(_beginningDate.AddDays(15)), Is.EqualTo(istihada));
        }

        [Test]  // [Rulings of Haydh and Nifas in the Shafi'i school (8); https://youtu.be/WEabG3nBuEM?t=200; Seite 61-62 der PDF]
        public void HaydCalculation_CalculationWithPurityBetweenTwoSeparateMenstrualCyclesIncludingIstihada()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Red, 3.0),
                (EFlowAppearanceColor.Clear, 14.0),
                (EFlowAppearanceColor.Red, 4.0),
            };

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple);
            _haydCalculatorFactory.DataList = timeData.AsReadOnly();

            //ACT
            _haydCalculatorFactory.Execute();

            //ASSERT
            Assert.IsTrue(_haydCalculatorFactory.IstihadaLst.Count == 1);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst.Count == 2);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count == 1);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst[1].HaydDataLst.Count == 1);

            FlowDataEntity firstMenstruation = _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[0];
            FlowDataEntity secondMenstruation = _haydCalculatorFactory.HaydCycleLst[1].HaydDataLst[0];
            FlowDataEntity istihada = _haydCalculatorFactory.IstihadaLst[0];

            Assert.That((EFlowAppearanceColor.Red, 3.0).ToFlowTimeData(_beginningDate), Is.EqualTo(firstMenstruation));
            Assert.That((EFlowAppearanceColor.Red, 1.0).ToFlowTimeData(_beginningDate.AddDays(17)), Is.EqualTo(istihada));
            Assert.That((EFlowAppearanceColor.Red, 3.0).ToFlowTimeData(_beginningDate.AddDays(18)), Is.EqualTo(secondMenstruation));
        }

        [Test] // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1910]
        public void HaydCalculation_CalculationWithPartiallyIstihadaAndPartiallyMenstrualFlow()
        {
            //ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple = new List<(EFlowAppearanceColor type, double dayCount)>()
            {
                (EFlowAppearanceColor.Red, 10.0),
                (EFlowAppearanceColor.Clear, 8.0),
                (EFlowAppearanceColor.Red, 12.0),
            };

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple);
            _haydCalculatorFactory.DataList = timeData.AsReadOnly();

            //ACT
            _haydCalculatorFactory.Execute();

            //ASSERT
            Assert.IsTrue(_haydCalculatorFactory.IstihadaLst.Count == 1);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst.Count == 2);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count == 1);
            Assert.IsTrue(_haydCalculatorFactory.HaydCycleLst[1].HaydDataLst.Count == 1);

            FlowDataEntity firstMenstruation = _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[0];
            FlowDataEntity secondMenstruation = _haydCalculatorFactory.HaydCycleLst[1].HaydDataLst[0];
            FlowDataEntity istihada = _haydCalculatorFactory.IstihadaLst[0];

            Assert.That((EFlowAppearanceColor.Red, 10.0).ToFlowTimeData(_beginningDate), Is.EqualTo(firstMenstruation));
            Assert.That((EFlowAppearanceColor.Red, 7.0).ToFlowTimeData(_beginningDate.AddDays(18)), Is.EqualTo(istihada));
            Assert.That((EFlowAppearanceColor.Red, 5.0).ToFlowTimeData(_beginningDate.AddDays(25)), Is.EqualTo(secondMenstruation));
        }
    }
}