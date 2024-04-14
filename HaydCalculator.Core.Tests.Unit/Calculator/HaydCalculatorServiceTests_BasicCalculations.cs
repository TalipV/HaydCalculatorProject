using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Calculator.Services;
using HaydCalculator.Core.Tests.Unit.Util;

namespace HaydCalculator.Core.Tests.Unit.Calculator
{
    public class HaydCalculatorFactoryTests_BasicCalculations
    {
        private readonly HaydCalculatorService _haydCalculatorService;

        public HaydCalculatorFactoryTests_BasicCalculations()
        {
            _haydCalculatorService = new HaydCalculatorService();
        }

        [Fact] // Self explanatory.
        public void HaydCalculation_MinimalHaydDurationNotFulfilled()
        {
            // ARRANGE
            var data = new List<(EFlowAppearanceColor, double)>
            {
                (EFlowAppearanceColor.Red, 0.4),
                (EFlowAppearanceColor.Clear, 2.0),
                (EFlowAppearanceColor.Red, 0.59),
            }.GetFlowDataList(initialDateTime: new DateTime(2024, 1, 1));

            // ACT
            var calculationResult = _haydCalculatorService.Calculate(data);

            // ASSERT
            calculationResult.HaydCycles.Should().BeEmpty();
            calculationResult.IstihadaFlows.Count.Should().Be(2);

            FlowDataEntity firstIstihadaData = calculationResult.IstihadaFlows.First();
            FlowDataEntity secondIstihadaData = calculationResult.IstihadaFlows.Last();

            firstIstihadaData.DayCount.Should().Be(0.4);
            firstIstihadaData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);
            secondIstihadaData.DayCount.Should().Be(0.59);
            secondIstihadaData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

            TestMethods.CheckFlowsInOrder([firstIstihadaData, secondIstihadaData]);
        }

        [Fact] // Self explanatory.
        public void HaydCalculation_MinimalHaydDurationBarelyFulfilled()
        {
            // ARRANGE
            var data = new List<(EFlowAppearanceColor, double)>
            {
                (EFlowAppearanceColor.Red, 1.0),
            }.GetFlowDataList(initialDateTime: new DateTime(2024, 1, 1));

            // ACT
            var calculationResult = _haydCalculatorService.Calculate(data);

            // ASSERT
            calculationResult.IstihadaFlows.Should().BeEmpty();
            calculationResult.HaydCycles.Count.Should().Be(1);
            calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(1);
            calculationResult.HaydCycles[0].HaydFlows[0].DayCount.Should().Be(1.0);
            calculationResult.HaydCycles[0].HaydFlows[0].Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);
        }

        [Fact] // Program specific test.
        public void HaydCalculation_ConnectSuccessiveSameFlows()
        {
            // ARRANGE
            var data = new List<(EFlowAppearanceColor, double)>
            {
                (EFlowAppearanceColor.Red, 1.0),
                (EFlowAppearanceColor.Black, 2.0),
                (EFlowAppearanceColor.Black, 3.0),
            }.GetFlowDataList(initialDateTime: new DateTime(2024, 1, 1));

            // ACT
            var calculationResult = _haydCalculatorService.Calculate(data);

            // ASSERT
            calculationResult.IstihadaFlows.Should().BeEmpty();
            calculationResult.HaydCycles.Count.Should().Be(1);
            calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(2);

            FlowDataEntity firstFlowData = calculationResult.HaydCycles[0].HaydFlows[0];
            firstFlowData.DayCount.Should().Be(1.0);
            firstFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

            FlowDataEntity secondFlowData = calculationResult.HaydCycles[0].HaydFlows[1];
            secondFlowData.DayCount.Should().Be(5.0);
            secondFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Black);

            TestMethods.CheckFlowsInOrder([firstFlowData, secondFlowData]);
        }

        [Fact]  // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1591]
        public void HaydCalculation_CalculationWithPurityInBetweenTwoMenstrualFlows()
        {
            // ARRANGE
            var data = new List<(EFlowAppearanceColor, double)>
            {
                (EFlowAppearanceColor.Red, 3.0),
                (EFlowAppearanceColor.Clear, 7.0),
                (EFlowAppearanceColor.Red, 1.0),
            }.GetFlowDataList(initialDateTime: new DateTime(2024, 1, 1));

            // ACT
            var calculationResult = _haydCalculatorService.Calculate(data);

            // ASSERT
            calculationResult.IstihadaFlows.Should().BeEmpty();
            calculationResult.HaydCycles.Count.Should().Be(1);
            calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(3);

            FlowDataEntity firstFlowData = calculationResult.HaydCycles[0].HaydFlows[0];
            firstFlowData.DayCount.Should().Be(3.0);
            firstFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);
            firstFlowData.IsNaqa.Should().Be(false);

            FlowDataEntity secondFlowData = calculationResult.HaydCycles[0].HaydFlows[1];
            secondFlowData.DayCount.Should().Be(7.0);
            secondFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Clear);
            secondFlowData.IsNaqa.Should().Be(true);

            FlowDataEntity thirdFlowData = calculationResult.HaydCycles[0].HaydFlows[2];
            thirdFlowData.DayCount.Should().Be(1.0);
            thirdFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);
            thirdFlowData.IsNaqa.Should().Be(false);

            TestMethods.CheckFlowsInOrder([firstFlowData, secondFlowData, thirdFlowData]);
        }

        [Fact]  // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1750]
        public void HaydCalculation_CalculationWithPurityBetweenMenstrualFlowAndIstihadaFlow()
        {
            // ARRANGE
            var data = new List<(EFlowAppearanceColor, double)>
            {
                (EFlowAppearanceColor.Red, 7.0),
                (EFlowAppearanceColor.Clear, 8.0),

                // intentionally split up into two to capture a now fixed bug
                (EFlowAppearanceColor.Red, 1.0),
                (EFlowAppearanceColor.Red, 1.0),
            }.GetFlowDataList(initialDateTime: new DateTime(2024, 1, 1));

            // ACT
            var calculationResult = _haydCalculatorService.Calculate(data);

            // ASSERT
            calculationResult.IstihadaFlows.Count.Should().Be(1);
            calculationResult.HaydCycles.Count.Should().Be(1);
            calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(1);

            FlowDataEntity firstFlowData = calculationResult.HaydCycles[0].HaydFlows[0];
            firstFlowData.DayCount.Should().Be(7.0);
            firstFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

            FlowDataEntity firstIstihadaData = calculationResult.IstihadaFlows[0];
            firstIstihadaData.DayCount.Should().Be(2.0);
            firstIstihadaData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

            TestMethods.CheckFlowsInOrder([firstFlowData, firstIstihadaData]);
        }

        [Fact]  // [Rulings of Haydh and Nifas in the Shafi'i school (8); https://youtu.be/WEabG3nBuEM?t=200; Pages 61-62 of the pdf]
        public void HaydCalculation_CalculationWithPurityBetweenTwoSeparateMenstrualCyclesIncludingIstihada()
        {
            // ARRANGE
            var data = new List<(EFlowAppearanceColor, double)>
            {
                (EFlowAppearanceColor.Red, 3.0),
                (EFlowAppearanceColor.Clear, 14.0),
                (EFlowAppearanceColor.Red, 4.0),
            }.GetFlowDataList(initialDateTime: new DateTime(2024, 1, 1));

            // ACT
            var calculationResult = _haydCalculatorService.Calculate(data);

            // ASSERT
            calculationResult.IstihadaFlows.Count.Should().Be(1);
            calculationResult.HaydCycles.Count.Should().Be(2);
            calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(1);
            calculationResult.HaydCycles[1].HaydFlows.Count.Should().Be(1);

            FlowDataEntity firstFlowData = calculationResult.HaydCycles[0].HaydFlows[0];
            firstFlowData.DayCount.Should().Be(3.0);
            firstFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

            FlowDataEntity firstIstihadaData = calculationResult.IstihadaFlows[0];
            firstIstihadaData.DayCount.Should().Be(1.0);
            firstIstihadaData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

            FlowDataEntity secondFlowData = calculationResult.HaydCycles[1].HaydFlows[0];
            secondFlowData.DayCount.Should().Be(3.0);
            secondFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

            TestMethods.CheckFlowsInOrder([firstFlowData, firstIstihadaData, secondFlowData]);
        }

        [Fact] // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1910]
        public void HaydCalculation_CalculationWithPartiallyIstihadaAndPartiallyMenstrualFlow()
        {
            // ARRANGE
            var data = new List<(EFlowAppearanceColor, double)>
            {
                (EFlowAppearanceColor.Red, 10.0),
                (EFlowAppearanceColor.Clear, 8.0),
                (EFlowAppearanceColor.Red, 12.0),
            }.GetFlowDataList(initialDateTime: new DateTime(2024, 1, 1));

            // ACT
            var calculationResult = _haydCalculatorService.Calculate(data);

            // ASSERT
            calculationResult.IstihadaFlows.Count.Should().Be(1);
            calculationResult.HaydCycles.Count.Should().Be(2);
            calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(1);
            calculationResult.HaydCycles[1].HaydFlows.Count.Should().Be(1);

            FlowDataEntity firstFlowData = calculationResult.HaydCycles[0].HaydFlows[0];
            firstFlowData.DayCount.Should().Be(10.0);
            firstFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

            FlowDataEntity firstIstihadaData = calculationResult.IstihadaFlows[0];
            firstIstihadaData.DayCount.Should().Be(7.0);
            firstIstihadaData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

            FlowDataEntity secondFlowData = calculationResult.HaydCycles[1].HaydFlows[0];
            secondFlowData.DayCount.Should().Be(5.0);
            secondFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

            TestMethods.CheckFlowsInOrder([firstFlowData, firstIstihadaData, secondFlowData]);
        }
    }
}