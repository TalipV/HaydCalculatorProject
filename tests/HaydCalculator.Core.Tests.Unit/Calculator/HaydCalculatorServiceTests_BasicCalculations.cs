using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Calculator.Services;
using HaydCalculator.Core.Misc;
using HaydCalculator.Core.Tests.Unit.Util;
using Microsoft.Extensions.Logging.Abstractions;

namespace HaydCalculator.Core.Tests.Unit.Calculator;

public class HaydCalculatorFactoryTests_BasicCalculations
{
    private readonly HaydCalculatorService _haydCalculatorService;

    public HaydCalculatorFactoryTests_BasicCalculations()
    {
        _haydCalculatorService = new HaydCalculatorService(NullLogger<HaydCalculatorService>.Instance);
    }

    [Fact]
    public void ThrowExceptionWhenNoFlowDataIsHaydType()
    {
        // ARRANGE
        var date = Instant.FromUtc(2000, 5, 20, 0, 0);

        List<FlowDataEntity> timeData =
        [
            new FlowDataEntity()
            {
                From = date,
                To = date + Duration.FromDays(5),
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Clear },
            },
            new FlowDataEntity()
            {
                From = date + Duration.FromDays(10),
                To = date + Duration.FromDays(15),
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Clear },
            },
        ];

        // ACT & ASSERT
        Exception exc = Assert.Throws<InfoException>(() => _haydCalculatorService.Calculate(timeData));
        exc.Message.Should().Be(TextUtil.NoHaydFlowDataProvided);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void ThrowExceptionWhenFlowDataToDateIsEqualToOrSmallerThanFromDate(int dayDifference)
    {
        // ARRANGE
        var date = Instant.FromUtc(2000, 5, 20, 0, 0);

        List<FlowDataEntity> timeData =
        [
            new FlowDataEntity()
            {
                From = date,
                To = date + Duration.FromDays(-dayDifference),
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
            },
            new FlowDataEntity()
            {
                From = date + Duration.FromDays(dayDifference*2),
                To = date + Duration.FromDays(dayDifference*3),
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
            },
        ];

        // ACT & ASSERT
        Exception exc = Assert.Throws<InfoException>(() => _haydCalculatorService.Calculate(timeData));
        exc.Message.Should().Be(TextUtil.FlowDataWithInvalidTimes);
    }

    [Fact]
    public void ThrowExceptionWhenMultipleFlowDataOverlapEachOther()
    {
        // ARRANGE
        var date = Instant.FromUtc(2000, 5, 20, 0, 0);

        List<FlowDataEntity> timeData =
        [
            new FlowDataEntity()
            {
                From = date + Duration.FromDays(3),
                To = date + Duration.FromDays(5),
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
            },
            new FlowDataEntity()
            {
                From = date + Duration.FromDays(4),
                To = date + Duration.FromDays(6),
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
            },
        ];

        // ACT & ASSERT
        Exception exc = Assert.Throws<InfoException>(() => _haydCalculatorService.Calculate(timeData));
        exc.Message.Should().Be(TextUtil.FlowDataEntriesWithOverlappingTimes);
    }

    [Fact]
    public void ThrowExceptionWhenGapsBetweenFlowDataEntries()
    {
        // ARRANGE
        var date = Instant.FromUtc(2000, 5, 20, 0, 0);

        List<FlowDataEntity> timeData =
        [
            new FlowDataEntity()
            {
                From = date + Duration.FromDays(3),
                To = date + Duration.FromDays(5),
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
            },
            new FlowDataEntity()
            {
                From = date + Duration.FromDays(6),
                To = date + Duration.FromDays(7),
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = EFlowAppearanceColor.Red},
            },
        ];

        // ACT & ASSERT
        Exception exc = Assert.Throws<InfoException>(() => _haydCalculatorService.Calculate(timeData));
        exc.Message.Should().Be(TextUtil.FlowDataEntriesWithTimeGaps);
    }

    // [an-Nawawi, Minhaj at-Talibin, p. 19; https://shamela.ws/book/12096/19] The minimum duration of hayd is a day and a night.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210] If the bloods together stay under the minimum duration, they are dam fasad (not hayd). [IMPLICIT]
    [Fact] // Self explanatory.
    public void HaydCalculation_MinimalHaydDurationNotFulfilled()
    {
        // ARRANGE
        List<FlowDataEntity> data = new List<(EFlowAppearanceColor, double)>
        {
            (EFlowAppearanceColor.Red, 0.4),
            (EFlowAppearanceColor.Clear, 2.0),
            (EFlowAppearanceColor.Red, 0.59),
        }.GetFlowDataList(initialInstant: Instant.FromUtc(2024, 1, 1, 0, 0));

        // ACT
        HaydCalculationResultVO calculationResult = _haydCalculatorService.Calculate(data);

        // ASSERT
        calculationResult.HaydCycles.Should().BeEmpty();
        calculationResult.IstihadaFlows.Count.Should().Be(2);

        FlowDataEntity firstIstihadaData = calculationResult.IstihadaFlows.First();
        FlowDataEntity secondIstihadaData = calculationResult.IstihadaFlows.Last();

        firstIstihadaData.Duration.Should().Be(Duration.FromDays(0.4));
        firstIstihadaData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);
        secondIstihadaData.Duration.Should().Be(Duration.FromDays(0.59));
        secondIstihadaData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

        TestMethods.CheckFlowsInOrder([firstIstihadaData, secondIstihadaData]);
    }

    // [an-Nawawi, Minhaj at-Talibin, p. 19; https://shamela.ws/book/12096/19] The minimum duration of hayd is a day and a night.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 278; https://shamela.ws/book/11444/194] A day and a night are 24 hours.
    [Fact] // Self explanatory.
    public void HaydCalculation_MinimalHaydDurationBarelyFulfilled()
    {
        // ARRANGE
        List<FlowDataEntity> data = new List<(EFlowAppearanceColor, double)>
        {
            (EFlowAppearanceColor.Red, 1.0),
        }.GetFlowDataList(initialInstant: Instant.FromUtc(2024, 1, 1, 0, 0));

        // ACT
        HaydCalculationResultVO calculationResult = _haydCalculatorService.Calculate(data);

        // ASSERT
        calculationResult.IstihadaFlows.Should().BeEmpty();
        calculationResult.HaydCycles.Count.Should().Be(1);
        calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(1);
        calculationResult.HaydCycles[0].HaydFlows[0].Duration.Should().Be(Duration.FromDays(1.0));
        calculationResult.HaydCycles[0].HaydFlows[0].Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);
    }

    [Fact] // Program specific test.
    public void HaydCalculation_ConnectSuccessiveSameFlows()
    {
        // ARRANGE
        List<FlowDataEntity> data = new List<(EFlowAppearanceColor, double)>
        {
            (EFlowAppearanceColor.Red, 1.0),
            (EFlowAppearanceColor.Black, 2.0),
            (EFlowAppearanceColor.Black, 3.0),
        }.GetFlowDataList(initialInstant: Instant.FromUtc(2024, 1, 1, 0, 0));

        // ACT
        HaydCalculationResultVO calculationResult = _haydCalculatorService.Calculate(data);

        // ASSERT
        calculationResult.IstihadaFlows.Should().BeEmpty();
        calculationResult.HaydCycles.Count.Should().Be(1);
        calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(2);

        FlowDataEntity firstFlowData = calculationResult.HaydCycles[0].HaydFlows[0];
        firstFlowData.Duration.Should().Be(Duration.FromDays(1.0));
        firstFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

        FlowDataEntity secondFlowData = calculationResult.HaydCycles[0].HaydFlows[1];
        secondFlowData.Duration.Should().Be(Duration.FromDays(5.0));
        secondFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Black);

        TestMethods.CheckFlowsInOrder([firstFlowData, secondFlowData]);
    }

    // [an-Nawawi, Minhaj at-Talibin, p. 20; https://shamela.ws/book/12096/20] The purity between bloods that together reach the minimum duration is hayd (qawl as-sahb). [IMPLICIT]
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 293; https://shamela.ws/book/11444/209] Conditions: everything together not over 15 days, the bloods together at least a day and night, the purity enclosed by blood. [IMPLICIT]
    [Fact]  // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1591]
    public void HaydCalculation_CalculationWithPurityInBetweenTwoMenstrualFlows()
    {
        // ARRANGE
        List<FlowDataEntity> data = new List<(EFlowAppearanceColor, double)>
        {
            (EFlowAppearanceColor.Red, 3.0),
            (EFlowAppearanceColor.Clear, 7.0),
            (EFlowAppearanceColor.Red, 1.0),
        }.GetFlowDataList(initialInstant: Instant.FromUtc(2024, 1, 1, 0, 0));

        // ACT
        HaydCalculationResultVO calculationResult = _haydCalculatorService.Calculate(data);

        // ASSERT
        calculationResult.IstihadaFlows.Should().BeEmpty();
        calculationResult.HaydCycles.Count.Should().Be(1);
        calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(3);

        FlowDataEntity firstFlowData = calculationResult.HaydCycles[0].HaydFlows[0];
        firstFlowData.Duration.Should().Be(Duration.FromDays(3.0));
        firstFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);
        firstFlowData.IsNaqa.Should().Be(false);

        FlowDataEntity secondFlowData = calculationResult.HaydCycles[0].HaydFlows[1];
        secondFlowData.Duration.Should().Be(Duration.FromDays(7.0));
        secondFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Clear);
        secondFlowData.IsNaqa.Should().Be(true);

        FlowDataEntity thirdFlowData = calculationResult.HaydCycles[0].HaydFlows[2];
        thirdFlowData.Duration.Should().Be(Duration.FromDays(1.0));
        thirdFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);
        thirdFlowData.IsNaqa.Should().Be(false);

        TestMethods.CheckFlowsInOrder([firstFlowData, secondFlowData, thirdFlowData]);
    }

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] Blood that still falls into the remainder of the minimal purity and then stops is dam fasad (example 3 blood, 12 purity, 3 blood). [IMPLICIT]
    [Fact]  // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1750]
    public void HaydCalculation_CalculationWithPurityBetweenMenstrualFlowAndIstihadaFlow()
    {
        // ARRANGE
        List<FlowDataEntity> data = new List<(EFlowAppearanceColor, double)>
        {
            (EFlowAppearanceColor.Red, 7.0),
            (EFlowAppearanceColor.Clear, 8.0),

            // intentionally split up into two to capture a now fixed bug
            (EFlowAppearanceColor.Red, 1.0),
            (EFlowAppearanceColor.Red, 1.0),
        }.GetFlowDataList(initialInstant: Instant.FromUtc(2024, 1, 1, 0, 0));

        // ACT
        HaydCalculationResultVO calculationResult = _haydCalculatorService.Calculate(data);

        // ASSERT
        calculationResult.IstihadaFlows.Count.Should().Be(1);
        calculationResult.HaydCycles.Count.Should().Be(1);
        calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(1);

        FlowDataEntity firstFlowData = calculationResult.HaydCycles[0].HaydFlows[0];
        firstFlowData.Duration.Should().Be(Duration.FromDays(7.0));
        firstFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

        FlowDataEntity firstIstihadaData = calculationResult.IstihadaFlows[0];
        firstIstihadaData.Duration.Should().Be(Duration.FromDays(2.0));
        firstIstihadaData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

        TestMethods.CheckFlowsInOrder([firstFlowData, firstIstihadaData]);
    }

    [Fact]  // [Rulings of Haydh and Nifas in the Shafi'i school (8); https://youtu.be/WEabG3nBuEM?t=200; Pages 61-62 of the pdf]
    public void HaydCalculation_CalculationWithPurityBetweenTwoSeparateMenstrualCyclesIncludingIstihada()
    {
        // ARRANGE
        List<FlowDataEntity> data = new List<(EFlowAppearanceColor, double)>
        {
            (EFlowAppearanceColor.Red, 3.0),
            (EFlowAppearanceColor.Clear, 14.0),
            (EFlowAppearanceColor.Red, 4.0),
        }.GetFlowDataList(initialInstant: Instant.FromUtc(2024, 1, 1, 0, 0));

        // ACT
        HaydCalculationResultVO calculationResult = _haydCalculatorService.Calculate(data);

        // ASSERT
        calculationResult.IstihadaFlows.Count.Should().Be(1);
        calculationResult.HaydCycles.Count.Should().Be(2);
        calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(1);
        calculationResult.HaydCycles[1].HaydFlows.Count.Should().Be(1);

        FlowDataEntity firstFlowData = calculationResult.HaydCycles[0].HaydFlows[0];
        firstFlowData.Duration.Should().Be(Duration.FromDays(3.0));
        firstFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

        FlowDataEntity firstIstihadaData = calculationResult.IstihadaFlows[0];
        firstIstihadaData.Duration.Should().Be(Duration.FromDays(1.0));
        firstIstihadaData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

        FlowDataEntity secondFlowData = calculationResult.HaydCycles[1].HaydFlows[0];
        secondFlowData.Duration.Should().Be(Duration.FromDays(3.0));
        secondFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

        TestMethods.CheckFlowsInOrder([firstFlowData, firstIstihadaData, secondFlowData]);
    }

    [Fact] // [Rulings of Haydh and Nifas in the Shafi'i school (3); https://youtu.be/fKriEgAo50o?t=1910]
    public void HaydCalculation_CalculationWithPartiallyIstihadaAndPartiallyMenstrualFlow()
    {
        // ARRANGE
        List<FlowDataEntity> data = new List<(EFlowAppearanceColor, double)>
        {
            (EFlowAppearanceColor.Red, 10.0),
            (EFlowAppearanceColor.Clear, 8.0),
            (EFlowAppearanceColor.Red, 12.0),
        }.GetFlowDataList(initialInstant: Instant.FromUtc(2024, 1, 1, 0, 0));

        // ACT
        HaydCalculationResultVO calculationResult = _haydCalculatorService.Calculate(data);

        // ASSERT
        calculationResult.IstihadaFlows.Count.Should().Be(1);
        calculationResult.HaydCycles.Count.Should().Be(2);
        calculationResult.HaydCycles[0].HaydFlows.Count.Should().Be(1);
        calculationResult.HaydCycles[1].HaydFlows.Count.Should().Be(1);

        FlowDataEntity firstFlowData = calculationResult.HaydCycles[0].HaydFlows[0];
        firstFlowData.Duration.Should().Be(Duration.FromDays(10.0));
        firstFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

        FlowDataEntity firstIstihadaData = calculationResult.IstihadaFlows[0];
        firstIstihadaData.Duration.Should().Be(Duration.FromDays(7.0));
        firstIstihadaData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

        FlowDataEntity secondFlowData = calculationResult.HaydCycles[1].HaydFlows[0];
        secondFlowData.Duration.Should().Be(Duration.FromDays(5.0));
        secondFlowData.Description.FlowAppearanceColorEnum.Should().Be(EFlowAppearanceColor.Red);

        TestMethods.CheckFlowsInOrder([firstFlowData, firstIstihadaData, secondFlowData]);
    }


    [Theory]
    [InlineData(11.0, 1.0, 1.0, 5.0)]
    [InlineData(13.0, 1.0, 1.0, 5.0)]
    public void IdentifyComplexIstihadahCaseWithNotImplementedException(double redFlowDayCount1, double clearFlowDayCount, double redFlowDayCount2, double redFlowDayCount3)
    {
        // ARRANGE
        var startDate = Instant.FromUtc(2024, 1, 1, 0, 0);

        List<(EFlowAppearanceColor type, double dayCount)> data =
        [
            (EFlowAppearanceColor.Red, redFlowDayCount1),
            (EFlowAppearanceColor.Clear, clearFlowDayCount),
            (EFlowAppearanceColor.Red, redFlowDayCount2),
            (EFlowAppearanceColor.Red, redFlowDayCount3),
        ];

        List<FlowDataEntity> timeData = data.GetFlowDataList(initialInstant: startDate);

        // ACT & ASSERT
        Exception exc = Assert.Throws<InfoException>(() => _haydCalculatorService.Calculate(timeData)); ;
        exc.Message.Should().Be(TextUtil.ComplexIstihadaNotImplemented);
    }

    #region Shafi'i sources (blood within the limits, no complex istihada)

    private static readonly Instant s_sourceExampleStart = Instant.FromUtc(2024, 1, 1, 0, 0);

    private HaydCalculationResultVO CalculateSourceExample(params (EFlowAppearanceColor type, double dayCount)[] flows)
    {
        List<(EFlowAppearanceColor type, double dayCount)> flowList = [.. flows];
        return _haydCalculatorService.Calculate(flowList.GetFlowDataList(initialInstant: s_sourceExampleStart));
    }

    // [an-Nawawi, Minhaj at-Talibin, p. 19; https://shamela.ws/book/12096/19] The maximum duration of hayd is 15 days with their nights; blood of at least a day and night that does not exceed the maximum is all hayd.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 284; https://shamela.ws/book/11444/200] The same, no matter whether she is a beginner or has a habit. [IMPLICIT]
    [Fact]
    public void HaydCalculation_FifteenDaysOfBlood_AllHayd()
    {
        // ACT
        HaydCalculationResultVO result = CalculateSourceExample((EFlowAppearanceColor.Red, 15));

        // ASSERT
        result.GetHaydDayRanges(s_sourceExampleStart).Should().Equal((0.0, 15.0));
        result.GetIstihadaDayRanges(s_sourceExampleStart).Should().BeEmpty();
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 284; https://shamela.ws/book/11444/200] Within 15 days everything is hayd, whether the blood is black or not. [IMPLICIT]
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] If red blood follows black blood and stops before 15 days are over, the red is hayd as well. [IMPLICIT]
    [Fact]
    public void HaydCalculation_BlackThenRedEndingWithinFifteenDays_AllHayd()
    {
        // ACT
        HaydCalculationResultVO result = CalculateSourceExample(
            (EFlowAppearanceColor.Black, 10),
            (EFlowAppearanceColor.Red, 5));

        // ASSERT
        result.GetHaydDayRanges(s_sourceExampleStart).Should().Equal((0.0, 15.0));
        result.GetIstihadaDayRanges(s_sourceExampleStart).Should().BeEmpty();
    }

    // [an-Nawawi, Minhaj at-Talibin, p. 19; https://shamela.ws/book/12096/19] The minimal purity between two hayds is 15 days.
    [Fact]
    public void HaydCalculation_ExactlyMinimalTuhurBetweenTwoBloods_TwoHaydCycles()
    {
        // ACT
        HaydCalculationResultVO result = CalculateSourceExample(
            (EFlowAppearanceColor.Red, 3),
            (EFlowAppearanceColor.Clear, 15),
            (EFlowAppearanceColor.Red, 3));

        // ASSERT
        result.GetHaydDayRanges(s_sourceExampleStart).Should().Equal((0.0, 3.0), (18.0, 21.0));
        result.GetIstihadaDayRanges(s_sourceExampleStart).Should().BeEmpty();
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 284; https://shamela.ws/book/11444/200] 3 days blood, 12 days purity, 3 days blood, then it stops: the last 3 days are dam fasad, not hayd (after al-Majmu').
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] The same example: the last 3 days are dam fasad.
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 339f.; https://shamela.ws/book/3565/337] The same example: the last 3 days are dam fasad.
    [Fact]
    public void HaydCalculation_BloodWithinRemainingMinimalTuhur_IsDamFasad()
    {
        // ACT
        HaydCalculationResultVO result = CalculateSourceExample(
            (EFlowAppearanceColor.Red, 3),
            (EFlowAppearanceColor.Clear, 12),
            (EFlowAppearanceColor.Red, 3));

        // ASSERT
        result.GetHaydDayRanges(s_sourceExampleStart).Should().Equal((0.0, 3.0));
        result.GetIstihadaDayRanges(s_sourceExampleStart).Should().Equal((15.0, 18.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 293f.; https://shamela.ws/book/11444/209] If she sees blood and purity alternately, everything is hayd when the conditions are met (qawl as-sahb, the mu'tamad). [IMPLICIT]
    // [ash-Shashi, Hilyat al-'Ulama', vol. 1, p. 228; https://shamela.ws/book/6131/176] If she sees a day of blood and a day of purity without exceeding 15 days, everything is hayd according to ash-Shafi'i's text.
    [Fact]
    public void HaydCalculation_AlternatingDayOfBloodAndDayOfPurityWithinFifteenDays_AllHayd()
    {
        // ARRANGE
        List<(EFlowAppearanceColor type, double dayCount)> flows = [];

        for (int i = 0; i < 7; i++)
        {
            flows.Add((EFlowAppearanceColor.Red, 1));
            flows.Add((EFlowAppearanceColor.Clear, 1));
        }

        flows.Add((EFlowAppearanceColor.Red, 1));

        // ACT
        HaydCalculationResultVO result = CalculateSourceExample([.. flows]);

        // ASSERT
        result.GetHaydDayRanges(s_sourceExampleStart).Should().Equal((0.0, 15.0));
        result.GetIstihadaDayRanges(s_sourceExampleStart).Should().BeEmpty();
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 293; https://shamela.ws/book/11444/209] Condition of sahb: the bloods together reach the minimum duration (a day and a night). [IMPLICIT]
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210] According to the mu'tamad the purity only counts as hayd if the bloods together reach the minimum duration. [IMPLICIT]
    [Fact]
    public void HaydCalculation_BloodsReachMinimumOnlyTogether_PurityInBetweenIsHayd()
    {
        // ACT
        HaydCalculationResultVO result = CalculateSourceExample(
            (EFlowAppearanceColor.Red, 0.5),
            (EFlowAppearanceColor.Clear, 1),
            (EFlowAppearanceColor.Red, 0.5));

        // ASSERT
        result.GetHaydDayRanges(s_sourceExampleStart).Should().Equal((0.0, 2.0));
        result.GetIstihadaDayRanges(s_sourceExampleStart).Should().BeEmpty();
    }

    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 149; https://shamela.ws/book/499/147] Habit of 5 days at the start of the month; if she is pure for 20 days after her 5 days and the blood returns in the last 5 days, her hayd has moved earlier and her cycle is 25 days.
    [Fact]
    public void HaydCalculation_BloodAfterTwentyDaysOfPurity_IsEarlierHayd()
    {
        // ACT
        HaydCalculationResultVO result = CalculateSourceExample(
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 20),
            (EFlowAppearanceColor.Red, 5));

        // ASSERT
        result.GetHaydDayRanges(s_sourceExampleStart).Should().Equal((0.0, 5.0), (30.0, 35.0), (55.0, 60.0));
        result.GetIstihadaDayRanges(s_sourceExampleStart).Should().BeEmpty();
    }

    #endregion Shafi'i sources (blood within the limits, no complex istihada)
}