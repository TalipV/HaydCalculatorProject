using HaydCalculator.Core.Entities;
using HaydCalculator.Core.Enums;

namespace HaydCalculator.Core.Tests.Unit
{
    public class HaydCalculatorFactoryTests_BasicCalculations
    {
        private readonly DateTime _beginningDate = new(DateTime.Now.Year, 1, 1);
        private readonly HaydCalculatorFactory _haydCalculatorFactory = new();

        [Fact] // Self explanatory.
        public void HaydCalculation_MinimalHaydDurationNotFulfilled()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple =
            [
                (EFlowAppearanceColor.Red, 0.4),
                (EFlowAppearanceColor.Clear, 2.0),
                (EFlowAppearanceColor.Red, 0.1),
            ];

            _haydCalculatorFactory.DataList = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple).AsReadOnly();

            // ACT
            _haydCalculatorFactory.Execute();

            // ASSERT
            _haydCalculatorFactory.HaydCycleLst.Should().BeEmpty();
            _haydCalculatorFactory.IstihadaLst.Count.Should().Be(2);
            _haydCalculatorFactory.IstihadaLst.First().DayCount.Should().Be(0.4);
            _haydCalculatorFactory.IstihadaLst.Last().DayCount.Should().Be(0.1);
        }

        [Fact] // Self explanatory.
        public void HaydCalculation_MinimalHaydDurationBarelyFulfilled()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple =
            [
                (EFlowAppearanceColor.Red, 1.0),
            ];

            _haydCalculatorFactory.DataList = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple).AsReadOnly();

            // ACT
            _haydCalculatorFactory.Execute();

            // ASSERT
            _haydCalculatorFactory.IstihadaLst.Should().BeEmpty();
            _haydCalculatorFactory.HaydCycleLst.Count.Should().Be(1);
            _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count.Should().Be(1);
            _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[0].DayCount.Should().Be(1.0);
        }

        [Fact] // Program specific test.
        public void HaydCalculation_ConnectSuccessiveSameFlows()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple =
            [
                (EFlowAppearanceColor.Red, 1.0),
                (EFlowAppearanceColor.Black, 2.0),
                (EFlowAppearanceColor.Black, 3.0),
            ];

            _haydCalculatorFactory.DataList = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple).AsReadOnly();

            // ACT
            _haydCalculatorFactory.Execute();

            // ASSERT
            _haydCalculatorFactory.IstihadaLst.Should().BeEmpty();
            _haydCalculatorFactory.HaydCycleLst.Count.Should().Be(1);
            _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count.Should().Be(2);

            FlowDataEntity firstFlowData = _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[0];
            FlowDataEntity secondFlowData = _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[1];

            firstFlowData.DayCount.Should().Be(1.0);
            firstFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);
            secondFlowData.DayCount.Should().Be(5.0);
            secondFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Black);
        }

        [Fact]  // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1591]
        public void HaydCalculation_CalculationWithPurityInBetweenTwoMenstrualFlows()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple =
            [
                (EFlowAppearanceColor.Red, 3.0),
                (EFlowAppearanceColor.Clear, 7.0),
                (EFlowAppearanceColor.Red, 1.0),
            ];

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple);
            _haydCalculatorFactory.DataList = timeData.AsReadOnly();

            // ACT
            _haydCalculatorFactory.Execute();

            // ASSERT
            _haydCalculatorFactory.IstihadaLst.Should().BeEmpty();
            _haydCalculatorFactory.HaydCycleLst.Count.Should().Be(1);
            _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count.Should().Be(3);

            for (int i = 0; i < tuple.Count; i++)
            {
                _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.ElementAt(i).Should().Be(timeData.ElementAt(i));
            }
        }

        [Fact]  // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1750]
        public void HaydCalculation_CalculationWithPurityBetweenMenstrualFlowAndIstihadaFlow()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple =
            [
                (EFlowAppearanceColor.Red, 7.0),
                (EFlowAppearanceColor.Clear, 8.0),

                // intentionally split up into two because this tests another potential bug

                // TODO: Separater Test? Problem war, dass nachdem ein Überschuss als Istihada festgestellt wurde,
                // der nächste Überschuss nicht korrekt verarbeitet wurde.
                // UND gleiche aufeinanderfolgende Istihada-Einträge wurden nicht korrekt zu einem Eintrag vereinigt.
                (EFlowAppearanceColor.Red, 1.0),
                (EFlowAppearanceColor.Red, 1.0),
            ];

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple);
            _haydCalculatorFactory.DataList = timeData.AsReadOnly();

            // ACT
            _haydCalculatorFactory.Execute();

            // ASSERT
            _haydCalculatorFactory.IstihadaLst.Count.Should().Be(1);
            _haydCalculatorFactory.HaydCycleLst.Count.Should().Be(1);
            _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count.Should().Be(1);

            FlowDataEntity menstruation = _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[0];
            FlowDataEntity istihada = _haydCalculatorFactory.IstihadaLst[0];

            menstruation.Should().Be(timeData.First());
            //istihada.Should().Be(timeData.Last());

            (EFlowAppearanceColor.Red, 7.0).ToFlowTimeData(_beginningDate).Should().Be(menstruation);
            (EFlowAppearanceColor.Red, 2.0).ToFlowTimeData(_beginningDate.AddDays(15)).Should().Be(istihada);
        }

        [Fact]  // [Rulings of Haydh and Nifas in the Shafi'i school (8); https://youtu.be/WEabG3nBuEM?t=200; Seite 61-62 der PDF]
        public void HaydCalculation_CalculationWithPurityBetweenTwoSeparateMenstrualCyclesIncludingIstihada()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple =
            [
                (EFlowAppearanceColor.Red, 3.0),
                (EFlowAppearanceColor.Clear, 14.0),
                (EFlowAppearanceColor.Red, 4.0),
            ];

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple);
            _haydCalculatorFactory.DataList = timeData.AsReadOnly();

            // ACT
            _haydCalculatorFactory.Execute();

            // ASSERT
            _haydCalculatorFactory.IstihadaLst.Count.Should().Be(1);
            _haydCalculatorFactory.HaydCycleLst.Count.Should().Be(2);
            _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count.Should().Be(1);
            _haydCalculatorFactory.HaydCycleLst[1].HaydDataLst.Count.Should().Be(1);

            FlowDataEntity firstMenstruation = _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[0];
            FlowDataEntity secondMenstruation = _haydCalculatorFactory.HaydCycleLst[1].HaydDataLst[0];
            FlowDataEntity istihada = _haydCalculatorFactory.IstihadaLst[0];

            (EFlowAppearanceColor.Red, 3.0).ToFlowTimeData(_beginningDate).Should().Be(firstMenstruation);
            (EFlowAppearanceColor.Red, 1.0).ToFlowTimeData(_beginningDate.AddDays(17)).Should().Be(istihada);
            (EFlowAppearanceColor.Red, 3.0).ToFlowTimeData(_beginningDate.AddDays(18)).Should().Be(secondMenstruation);
        }

        [Fact] // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1910]
        public void HaydCalculation_CalculationWithPartiallyIstihadaAndPartiallyMenstrualFlow()
        {
            // ARRANGE
            List<(EFlowAppearanceColor type, double dayCount)> tuple =
            [
                (EFlowAppearanceColor.Red, 10.0),
                (EFlowAppearanceColor.Clear, 8.0),
                (EFlowAppearanceColor.Red, 12.0),
            ];

            List<FlowDataEntity> timeData = HaydCalculatorFactory.TransformHaydTupleListToFlowTimeDataList(_beginningDate, tuple);
            _haydCalculatorFactory.DataList = timeData.AsReadOnly();

            // ACT
            _haydCalculatorFactory.Execute();

            // ASSERT
            _haydCalculatorFactory.IstihadaLst.Count.Should().Be(1);
            _haydCalculatorFactory.HaydCycleLst.Count.Should().Be(2);
            _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst.Count.Should().Be(1);
            _haydCalculatorFactory.HaydCycleLst[1].HaydDataLst.Count.Should().Be(1);

            FlowDataEntity firstMenstruation = _haydCalculatorFactory.HaydCycleLst[0].HaydDataLst[0];
            FlowDataEntity secondMenstruation = _haydCalculatorFactory.HaydCycleLst[1].HaydDataLst[0];
            FlowDataEntity istihada = _haydCalculatorFactory.IstihadaLst[0];

            (EFlowAppearanceColor.Red, 10.0).ToFlowTimeData(_beginningDate).Should().Be(firstMenstruation);
            (EFlowAppearanceColor.Red, 7.0).ToFlowTimeData(_beginningDate.AddDays(18)).Should().Be(istihada);
            (EFlowAppearanceColor.Red, 5.0).ToFlowTimeData(_beginningDate.AddDays(25)).Should().Be(secondMenstruation);
        }
    }
}