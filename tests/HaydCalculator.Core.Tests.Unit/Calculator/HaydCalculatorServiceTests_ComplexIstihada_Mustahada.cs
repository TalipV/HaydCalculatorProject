using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Calculator.Services;
using HaydCalculator.Core.Tests.Unit.Util;
using Microsoft.Extensions.Logging.Abstractions;

namespace HaydCalculator.Core.Tests.Unit.Calculator;

/// <summary>
/// Blood beyond the maximum hayd duration (al-mustahada): the expected results of the Shafi'i
/// mu'tamad for each kind of mustahada, taken from explicit examples in the sources. The
/// calculator does not support these cases yet, so these tests are expected to fail until it
/// does (TDD). Only sources that state the case with explicit numbers are used here.
/// </summary>
public class HaydCalculatorServiceTests_ComplexIstihada_Mustahada
{
    private static readonly Instant s_start = Instant.FromUtc(2024, 1, 1, 0, 0);

    private readonly HaydCalculatorService _haydCalculatorService = new(NullLogger<HaydCalculatorService>.Instance);

    private HaydCalculationResultVO Calculate(params (EFlowAppearanceColor type, double dayCount)[] flows)
    {
        List<(EFlowAppearanceColor type, double dayCount)> flowList = [.. flows];
        return _haydCalculatorService.Calculate(flowList.GetFlowDataList(initialInstant: s_start));
    }

    #region Mubtada'a ghayr mumayyiza (beginner without distinguishable blood)

    // [an-Nawawi, Minhaj at-Talibin, p. 19f.; https://shamela.ws/book/12096/19] Beginner without distinguishable blood: according to the stronger view her hayd is a day and a night, her purity 29 days.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] The same, counted from the start of the blood; the 29 days complete the month to a 30-day cycle.
    [Fact]
    public void MubtadaaGhayrMumayyiza_ContinuousRedBlood_OneDayHaydAndTwentyNineDaysTuhur()
    {
        // ACT
        HaydCalculationResultVO result = Calculate((EFlowAppearanceColor.Red, 45));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 1.0), (30.0, 31.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((1.0, 30.0), (31.0, 45.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 287; https://shamela.ws/book/11444/203] A beginner sees 15 days red, then 15 days black; if the black continues she has no distinction, her hayd is a day and a night from the start of each month.
    [Fact]
    public void MubtadaaGhayrMumayyiza_FifteenDaysRedThenBlackContinues_OneDayHaydPerMonth()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Red, 15),
            (EFlowAppearanceColor.Black, 16));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 1.0), (30.0, 31.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((1.0, 30.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] If she sees 16 days black, a condition of the distinction is missing and she counts as a beginner without distinguishable blood.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] Then her hayd is a day and a night, her purity 29 days.
    [Fact]
    public void MubtadaaGhayrMumayyiza_SixteenDaysBlack_TamyizFailsSoOneDayHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Black, 16),
            (EFlowAppearanceColor.Red, 14));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 1.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((1.0, 30.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] If she permanently sees one day black and two days red, a condition of the distinction is missing and she counts as a beginner without distinguishable blood.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] Then her hayd is a day and a night, her purity 29 days.
    [Fact]
    public void MubtadaaGhayrMumayyiza_AlternatingOneDayBlackTwoDaysRed_TamyizFailsSoOneDayHayd()
    {
        // ARRANGE
        List<(EFlowAppearanceColor type, double dayCount)> flows = [];

        for (int i = 0; i < 10; i++)
        {
            flows.Add((EFlowAppearanceColor.Black, 1));
            flows.Add((EFlowAppearanceColor.Red, 2));
        }

        // ACT
        HaydCalculationResultVO result = Calculate([.. flows]);

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 1.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((1.0, 30.0));
    }

    #endregion Mubtada'a ghayr mumayyiza (beginner without distinguishable blood)

    #region Mubtada'a mumayyiza (beginner with distinguishable blood)

    // [an-Nawawi, Minhaj at-Talibin, p. 19; https://shamela.ws/book/12096/19] Beginner with distinguishable blood: the weak blood is istihada, the strong is hayd if it lasts at least a day and night and at most 15 days and the weak does not stay under 15 days.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] If she sees a day and a night black and then weak blood permanently, the weak blood is purity. (The source says the weak blood continues "for years"; 40 days fulfil the explicit conditions of al-Minhaj above.)
    [Fact]
    public void MubtadaaMumayyiza_OneDayBlackThenLongRed_BlackIsHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Black, 1),
            (EFlowAppearanceColor.Red, 40));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 1.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((1.0, 41.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] 10 days black, then 10 days red, then it stops: she follows the distinction, although the weak blood stays under 15 days.
    [Fact]
    public void MubtadaaMumayyiza_TenDaysBlackTenDaysRedAndStop_BlackIsHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Black, 10),
            (EFlowAppearanceColor.Red, 10));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 10.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((10.0, 20.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] 7 days each of black, red, black: her hayd is the first black together with the red (after al-Majmu' and ar-Rawda).
    [Fact]
    public void MubtadaaMumayyiza_SevenDaysEachBlackRedBlack_FirstBlackAndRedAreHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Black, 7),
            (EFlowAppearanceColor.Red, 7),
            (EFlowAppearanceColor.Black, 7));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 14.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((14.0, 21.0));
    }

    #endregion Mubtada'a mumayyiza (beginner with distinguishable blood)

    #region Mu'tada (woman with a habit)

    // [an-Nawawi, Minhaj at-Talibin, p. 20; https://shamela.ws/book/12096/20] Woman with a habit: she returns to the hayd and purity of her habit in duration and timing; according to the sounder view the habit is established by a single occurrence.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 287f.; https://shamela.ws/book/11444/203] Example 5 days every month; if she had 5 days of hayd in one month and then became a mustahada, she returns to the 5 days.
    [Fact]
    public void Mutada_HabitOfFiveDays_ReturnsToHabit()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 30));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 35.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((35.0, 60.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204] Hayd of 5 days in one month, then 6, then 7, then istihada: according to the sounder view (habit by a single occurrence) she returns to the 7.
    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 148; https://shamela.ws/book/499/146] Habit of 5 days at the start of each month, then 6 in one cycle, 7 in the next, then istihada: if the habit is established by a single occurrence, she returns to the 7.
    [Fact]
    public void Mutada_ChangingHabitFiveSixSeven_ReturnsToLastHabit()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 6),
            (EFlowAppearanceColor.Clear, 24),
            (EFlowAppearanceColor.Red, 7),
            (EFlowAppearanceColor.Clear, 23),
            (EFlowAppearanceColor.Red, 30));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 36.0), (60.0, 67.0), (90.0, 97.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((97.0, 120.0));
    }

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] Habit of 5 days at the start of the month, then 14 days purity, then the blood returns and continues: the first day and night of the returning blood are purity, then 5 days hayd, and her cycle stays 20 days.
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 340 (Hashiyat ash-Shabramallisi); https://shamela.ws/book/3565/338] The same example after Ibn Hajar.
    [Fact]
    public void Mutada_FourteenDaysPurityThenContinuousBlood_FirstDayCompletesTuhur()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 14),
            (EFlowAppearanceColor.Red, 21));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 35.0), (50.0, 55.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((49.0, 50.0), (55.0, 70.0));
    }

    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 149; https://shamela.ws/book/499/147] Habit of 5 days at the start of the month; after 20 days of purity the blood returns in the last 5 days and continues: according to the soundest view she has 5 days of hayd from its start and 20 days of purity, and so on.
    [Fact]
    public void Mutada_BloodReturnsEarlierAndContinues_FiveDaysHaydTwentyDaysTuhur()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 20),
            (EFlowAppearanceColor.Red, 30));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 35.0), (55.0, 60.0), (80.0, 85.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((60.0, 80.0));
    }

    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 148; https://shamela.ws/book/499/146] Habit of 5 days at the start of each month, the rest pure; in one cycle she has only 4 of the 5 days, then istihada: if the habit is established by a single occurrence (the sounder view), she returns to what came right before the istihada (4 days).
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204] The habit is established by a single occurrence according to the sounder view.
    [Fact]
    public void Mutada_LastCycleOnlyFourDays_ReturnsToFourDays()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 4),
            (EFlowAppearanceColor.Clear, 26),
            (EFlowAppearanceColor.Red, 30));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 34.0), (60.0, 64.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((64.0, 90.0));
    }

    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 149; https://shamela.ws/book/499/147] Habit of 5 days at the start of the month; she sees the second five days as blood, it stops, she is pure the rest of the month (20 days), and the blood returns at the start of the month and continues: the first 5 days are hayd without dispute, and her purity is 20 days if the habit is established by a single occurrence.
    [Fact]
    public void Mutada_HaydMovedToSecondFiveThenContinuousBlood_FiveDaysHaydTwentyDaysTuhur()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 30),
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 20),
            (EFlowAppearanceColor.Red, 30));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (35.0, 40.0), (60.0, 65.0), (85.0, 90.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((65.0, 85.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 287; https://shamela.ws/book/11444/203] Habit of 15 days at the start of each month; she sees 15 days red from the start of a month, then black continues: as the black goes on, it becomes clear that she returns to her habit (after al-Barizi).
    [Fact]
    public void Mutada_HabitOfFifteenDays_RedThenContinuousBlack_ReturnsToHabit()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Red, 15),
            (EFlowAppearanceColor.Clear, 15),
            (EFlowAppearanceColor.Red, 15),
            (EFlowAppearanceColor.Black, 30));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 15.0), (30.0, 45.0), (60.0, 75.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((45.0, 60.0));
    }

    #endregion Mu'tada (woman with a habit)

    #region Mu'tada mumayyiza (woman with a habit and distinguishable blood)

    // [an-Nawawi, Minhaj at-Talibin, p. 20; https://shamela.ws/book/12096/20] For a woman with a habit and distinguishable blood, the distinction decides according to the sounder view, not the habit.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204] Habit of 5 days at the start of the month, the rest pure; then 10 days black from the start of the month and the rest red: her hayd are the 10 black days.
    [Fact]
    public void MutadaMumayyiza_TenDaysBlackAgainstHabitOfFive_TamyizWins()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Black, 10),
            (EFlowAppearanceColor.Red, 20));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 40.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((40.0, 60.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 288f.; https://shamela.ws/book/11444/204] If a full minimal purity lies between habit and distinction, both are used: after her habitual 5 days at the start of the month she sees 20 days red, then 5 days black, then red; according to the soundest view both (the 5 habit days and the 5 black days) are hayd.
    // [an-Nawawi, Minhaj at-Talibin, p. 19; https://shamela.ws/book/12096/19] The minimal purity between two hayds is 15 days, so the red right after the black cannot be hayd.
    [Fact]
    public void MutadaMumayyiza_FullTuhurBetweenHabitAndBlack_BothAreHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Red, 20),
            (EFlowAppearanceColor.Black, 5),
            (EFlowAppearanceColor.Red, 5));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 35.0), (55.0, 60.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((35.0, 55.0), (60.0, 65.0));
    }

    #endregion Mu'tada mumayyiza (woman with a habit and distinguishable blood)
}
