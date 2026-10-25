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

    [FiqhTodo]
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

    [FiqhTodo]
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

    [FiqhTodo]
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

    [FiqhTodo]
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

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 385; https://shamela.ws/book/9059/386] The minimum of hayd is a day and a night (24 hours); if the bloods together stay under it, there is no hayd (Hashiyat ash-Sharwani there).
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 326; https://shamela.ws/book/3565/324] Minimum of hayd a day and a night, maximum 15 days with their nights.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210] If the bloods together stay under the minimum duration, they are dam fasad (not hayd). [IMPLICIT]
    [FiqhTodo]
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

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 385; https://shamela.ws/book/9059/386] The minimum of hayd is a day and a night (24 hours); if the bloods together stay under it, there is no hayd (Hashiyat ash-Sharwani there).
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 326; https://shamela.ws/book/3565/324] Minimum of hayd a day and a night, maximum 15 days with their nights.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 278; https://shamela.ws/book/11444/194] A day and a night are 24 hours.
    [FiqhTodo]
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

    [FiqhTodo]
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

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 412; https://shamela.ws/book/9059/413] According to the azhar the purity between bloods that can be hayd is hayd, if purity and blood together do not exceed 15 days and the purity is enclosed by blood.
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 356; https://shamela.ws/book/3565/354] The purity between bloods of (at least) the minimum of hayd is hayd, following them.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 293; https://shamela.ws/book/11444/209] Conditions: everything together not over 15 days, the bloods together at least a day and night, the purity enclosed by blood. [IMPLICIT]
    // KHILAF: Counting the purity between the bloods as hayd is the stronger view (qawl as-sahb); the other view counts it as purity (qawl al-laqt / at-talfiq) (Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210). This test follows qawl as-sahb.
    [FiqhTodo]
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
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 284; https://shamela.ws/book/11444/200] Blood within the remaining minimal purity that then stops is dam fasad (example 3 blood, 12 purity, 3 blood). [IMPLICIT]
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 339f.; https://shamela.ws/book/3565/337] The same example. [IMPLICIT]
    [FiqhTodo]
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

    // No text source found yet: whether blood past the completion of the minimal purity is a new hayd is an open question, see HaydCalculatorServiceTests_OpenQuestions.BloodContinuingPastCompletionOfMinimalPurityThenStopping_IsTheRestHayd.
    [FiqhTodo]
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

    // No text source found yet: whether blood past the completion of the minimal purity is a new hayd is an open question, see HaydCalculatorServiceTests_OpenQuestions.BloodContinuingPastCompletionOfMinimalPurityThenStopping_IsTheRestHayd.
    [FiqhTodo]
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


    // Whether intermittent blood that exceeds 15 days together with the purity is a mustahada case is partly an open question, see HaydCalculatorServiceTests_OpenQuestions.ThirteenRedOnePurityFiveRed_DamFasadOrMustahada.
    [FiqhTodo]
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
        return CalculateSourceExample(HaydCalculatorSettings.Default, flows);
    }

    private HaydCalculationResultVO CalculateSourceExample(HaydCalculatorSettings settings, params (EFlowAppearanceColor type, double dayCount)[] flows)
    {
        List<(EFlowAppearanceColor type, double dayCount)> flowList = [.. flows];
        return _haydCalculatorService.Calculate(flowList.GetFlowDataList(initialInstant: s_sourceExampleStart), settings);
    }

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 385; https://shamela.ws/book/9059/386] The maximum of hayd is 15 days with their nights.
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 339; https://shamela.ws/book/3565/337] Blood of at least the minimum that does not exceed the maximum is all hayd, whether she is a mubtada'a or a mu'tada.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 284; https://shamela.ws/book/11444/200] The same, no matter whether she is a mubtada'a or a mu'tada. [IMPLICIT]
    [FiqhTodo]
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
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 402; https://shamela.ws/book/9059/403] A day and a night black, then red: if it stops before 15 days, everything is hayd (no khilaf here).
    [FiqhTodo]
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

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 385; https://shamela.ws/book/9059/386] The minimal purity between two hayds is 15 days with their nights.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 278; https://shamela.ws/book/11444/194] The minimal purity between two hayds is 15 days.
    [FiqhTodo]
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
    [FiqhTodo]
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
    // KHILAF: Counting the purity between the bloods as hayd is the stronger view (qawl as-sahb); the other view counts it as purity (qawl al-laqt / at-talfiq) (Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210). This test follows qawl as-sahb.
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 412; https://shamela.ws/book/9059/413] The purity between bloods that can be hayd is hayd (azhar), if purity and blood together do not exceed 15 days and the purity is enclosed by blood. [IMPLICIT]
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 356; https://shamela.ws/book/3565/354] The purity between bloods of (at least) the minimum of hayd is hayd, following them. [IMPLICIT]
    [FiqhTodo]
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
    // KHILAF: Counting the purity between the bloods as hayd is the stronger view (qawl as-sahb); the other view counts it as purity (qawl al-laqt / at-talfiq) (Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210). This test follows qawl as-sahb.
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 385; https://shamela.ws/book/9059/386] Hashiyat ash-Sharwani: with intermittent blood, if the bloods together reach a day and a night, everything is hayd (qawl as-sahb, the mu'tamad), otherwise there is no hayd. [IMPLICIT]
    [FiqhTodo]
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

    // [Ibn Hajar al-Haytami, al-Fatawa al-Kubra al-Fiqhiyya, vol. 1, p. 111; https://shamela.ws/book/21628/110] 'Ada of 5 days at the start of the month; if she is pure for 20 days after them and the blood returns in the last five days of the month, her hayd has moved earlier and her cycle is 25 days.
    [FiqhTodo]
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

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 284; https://shamela.ws/book/11444/200] Blood of at least the minimum that does not exceed 15 days is all hayd, whether it is black or not. [IMPLICIT]
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 339; https://shamela.ws/book/3565/337] All hayd, whether the blood has one quality or is split into strong and weak. [IMPLICIT]
    [FiqhTodo]
    [Fact]
    public void HaydCalculation_BlackRedBlackWithinFifteenDays_AllHayd()
    {
        // ACT
        HaydCalculationResultVO result = CalculateSourceExample(
            (EFlowAppearanceColor.Black, 5),
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Black, 5));

        // ASSERT
        result.GetHaydDayRanges(s_sourceExampleStart).Should().Equal((0.0, 15.0));
        result.GetIstihadaDayRanges(s_sourceExampleStart).Should().BeEmpty();
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 284; https://shamela.ws/book/11444/200] Blood that falls into the remaining minimal purity and then stops is dam fasad (example 3 blood, 12 purity, 3 blood). [IMPLICIT]
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] The same example. [IMPLICIT]
    [FiqhTodo]
    [Fact]
    public void HaydCalculation_OneDayOfBloodWithinRemainingMinimalTuhur_IsDamFasad()
    {
        // ACT
        HaydCalculationResultVO result = CalculateSourceExample(
            (EFlowAppearanceColor.Red, 10),
            (EFlowAppearanceColor.Clear, 10),
            (EFlowAppearanceColor.Red, 1));

        // ASSERT
        result.GetHaydDayRanges(s_sourceExampleStart).Should().Equal((0.0, 10.0));
        result.GetIstihadaDayRanges(s_sourceExampleStart).Should().Equal((20.0, 21.0));
    }

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 400; https://shamela.ws/book/9059/401] Yellow and turbid blood are hayd according to the asahh. [IMPLICIT]
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 340; https://shamela.ws/book/3565/338] Each of them is hayd, whether she is a mubtada'a or not and whether it contradicts her 'ada or not. [IMPLICIT]
    // KHILAF: Yellow and turbid blood are hayd according to the asahh; the other view says they are not hayd because they do not have the color of blood (Mughni al-Muhtaj, vol. 1, p. 284f.; https://shamela.ws/book/11444/200). This test follows the asahh.
    [FiqhTodo]
    [Fact]
    public void HaydCalculation_OnlyYellowBloodWithinTheLimits_IsHayd()
    {
        // ACT
        HaydCalculationResultVO result = CalculateSourceExample((EFlowAppearanceColor.Yellow, 5));

        // ASSERT
        result.GetHaydDayRanges(s_sourceExampleStart).Should().Equal((0.0, 5.0));
        result.GetIstihadaDayRanges(s_sourceExampleStart).Should().BeEmpty();
    }

    #endregion Shafi'i sources (blood within the limits, no complex istihada)

    #region Other views (khilaf), selected through HaydCalculatorSettings

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210] qawl al-laqt / at-talfiq: the purity between the bloods counts as purity, only the bloods are hayd. [IMPLICIT]
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 385; https://shamela.ws/book/9059/386] Hashiyat ash-Sharwani: under qawl al-laqt (not qawl as-sahb) only the bloods count. [IMPLICIT]
    [FiqhTodo]
    [Fact]
    public void HaydCalculation_PurityInBetweenTwoMenstrualFlows_PurityCountsAsTuhr()
    {
        // ACT
        HaydCalculationResultVO result = CalculateSourceExample(
            HaydCalculatorSettings.Default with { PurityBetweenBloods = EPurityBetweenBloods.Tuhr },
            (EFlowAppearanceColor.Red, 3),
            (EFlowAppearanceColor.Clear, 7),
            (EFlowAppearanceColor.Red, 1));

        // ASSERT
        result.GetHaydFlowDayRanges(s_sourceExampleStart).Should().Equal((0.0, 3.0), (10.0, 11.0));
        result.GetIstihadaDayRanges(s_sourceExampleStart).Should().BeEmpty();
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 284f.; https://shamela.ws/book/11444/200] Other view: yellow and turbid blood are not hayd, because they do not have the color of blood. [IMPLICIT]
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 340; https://shamela.ws/book/3565/338] The view opposite to the asahh: yellow and turbid blood are not hayd, because they do not have the color of blood. [IMPLICIT]
    [FiqhTodo]
    [Fact]
    public void HaydCalculation_OnlyYellowBlood_YellowNotHayd_NoHaydAtAll()
    {
        // ACT & ASSERT
        Exception exc = Assert.Throws<InfoException>(() => CalculateSourceExample(
            HaydCalculatorSettings.Default with { YellowAndTurbidAreHayd = false },
            (EFlowAppearanceColor.Yellow, 5)));
        exc.Message.Should().Be(TextUtil.NoHaydFlowDataProvided);
    }

    #endregion Other views (khilaf), selected through HaydCalculatorSettings
}