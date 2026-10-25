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
        return Calculate(HaydCalculatorSettings.Default, flows);
    }

    private HaydCalculationResultVO Calculate(HaydCalculatorSettings settings, params (EFlowAppearanceColor type, double dayCount)[] flows)
    {
        List<(EFlowAppearanceColor type, double dayCount)> flowList = [.. flows];
        return _haydCalculatorService.Calculate(flowList.GetFlowDataList(initialInstant: s_start), settings);
    }

    #region Mubtada'a ghayr mumayyiza (without distinguishable blood)

    // [an-Nawawi, Minhaj at-Talibin, p. 19f.; https://shamela.ws/book/12096/19] Mubtada'a ghayr mumayyiza (without distinguishable blood): according to the stronger view her hayd is a day and a night, her purity 29 days.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] The same, counted from the start of the blood; the 29 days complete the month to a 30-day cycle.
    // KHILAF: Hayd of a day and a night is the stronger view (azhar); the other view gives her the usual hayd of 6 or 7 days and the rest of the month as purity (Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202). This test follows the azhar.
    [FiqhTodo]
    [Fact]
    public void MubtadaaGhayrMumayyiza_ContinuousRedBlood_OneDayHaydAndTwentyNineDaysTuhur()
    {
        // ACT
        HaydCalculationResultVO result = Calculate((EFlowAppearanceColor.Red, 45));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 1.0), (30.0, 31.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((1.0, 30.0), (31.0, 45.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 287; https://shamela.ws/book/11444/203] A mubtada'a sees 15 days red, then 15 days black; if the black continues she has no distinction, her hayd is a day and a night from the start of each month.
    // KHILAF: Hayd of a day and a night is the stronger view (azhar); the other view gives her the usual hayd of 6 or 7 days and the rest of the month as purity (Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202). This test follows the azhar.
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 402f.; https://shamela.ws/book/9059/403] Same case and result (no khilaf with Mughni al-Muhtaj here).
    [FiqhTodo]
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

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] If she sees 16 days black, a condition of the distinction is missing and she counts as a mubtada'a ghayr mumayyiza.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] Then her hayd is a day and a night, her purity 29 days.
    // KHILAF: Hayd of a day and a night is the stronger view (azhar); the other view gives her the usual hayd of 6 or 7 days and the rest of the month as purity (Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202). This test follows the azhar.
    [FiqhTodo]
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

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] If she permanently sees one day black and two days red, a condition of the distinction is missing and she counts as a mubtada'a ghayr mumayyiza.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] Then her hayd is a day and a night, her purity 29 days.
    // KHILAF: Hayd of a day and a night is the stronger view (azhar); the other view gives her the usual hayd of 6 or 7 days and the rest of the month as purity (Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202). This test follows the azhar.
    [FiqhTodo]
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

    #endregion Mubtada'a ghayr mumayyiza (without distinguishable blood)

    #region Mubtada'a mumayyiza (with distinguishable blood)

    // [an-Nawawi, Minhaj at-Talibin, p. 19; https://shamela.ws/book/12096/19] Mubtada'a mumayyiza (with distinguishable blood): the weak blood is istihada, the strong is hayd if it lasts at least a day and night and at most 15 days and the weak does not stay under 15 days.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] If she sees a day and a night black and then weak blood permanently, the weak blood is purity. (The source says the weak blood continues "for years"; 40 days fulfil the explicit conditions of al-Minhaj above.)
    [FiqhTodo]
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
    [FiqhTodo]
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
    // KHILAF: [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 401; https://shamela.ws/book/9059/402] For 7 days black, 7 days red, 3 days black (then it stops) her hayd is only the first black, as the mu'tamad that an-Nawawi corrected in at-Tahqiq. Hashiyat ash-Shirwani there: Nihayat al-Muhtaj and Mughni al-Muhtaj disagree with him in that case, and Ibn Qasim notes that at-Tahqiq has 7/7/7 with black and red as hayd.
    // KHILAF: [Rulings of Haydh and Nifas in the Shafi'i school (8); https://youtu.be/WEabG3nBuEM?t=2342] 7 days each of black, red, black: according to Ibn Hajar al-Haytami only the first black is hayd; according to Shams ad-Din ar-Ramli and Ibn Surayj the first 14 days are hayd.
    // [an-Nawawi, al-Majmu', vol. 2, p. 409; https://shamela.ws/book/2186/937] 7/7/7: Ibn Surayj: her hayd is the first black with the red, the second black is purity. Abu Ishaq: both blacks are hayd and the red between them purity; an-Nawawi calls this very weak, even a mistake, because the blood exceeds 15 days.
    // This test follows ar-Ramli / al-Khatib (and al-Majmu'/ar-Rawda).
    [FiqhTodo]
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

    // [an-Nawawi, al-Majmu', vol. 2, p. 409; https://shamela.ws/book/2186/937] 8 days black, then 8 days red, then 8 days black: her hayd is the first black, by agreement (bi-l-ittifaq).
    // [Ibn Hajar al-Haytami, al-Fatawa al-Kubra al-Fiqhiyya, vol. 1, p. 86; https://shamela.ws/book/21628/85] Same case: her hayd is the first black, by agreement (no khilaf here).
    // [Rulings of Haydh and Nifas in the Shafi'i school (8); https://youtu.be/WEabG3nBuEM?t=2342] Same case: the first eight days are hayd, the other days are not.
    // Note: the last day of the second black (day 23-24) lies after a full minimal purity of 15 days following the first black; the sources still give only the first black as hayd and do not discuss that day.
    [FiqhTodo]
    [Fact]
    public void MubtadaaMumayyiza_EightBlackEightRedEightBlack_OnlyFirstBlackIsHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Black, 8),
            (EFlowAppearanceColor.Red, 8),
            (EFlowAppearanceColor.Black, 8));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 8.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((8.0, 24.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] Strong, weak and weaker blood: 5 days black, then 5 days red, then the yellow continues: the first two are hayd, because they are strong compared to what follows (the strong comes first, the weak follows directly, and together they do not exceed the maximum).
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 403f.; https://shamela.ws/book/9059/404] Same case and result: the first ten days are hayd (no khilaf here).
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 342; https://shamela.ws/book/3565/340] Same case and result.
    [FiqhTodo]
    [Fact]
    public void MubtadaaMumayyiza_FiveBlackFiveRedThenYellow_BlackAndRedAreHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Black, 5),
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Yellow, 20));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 10.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((10.0, 30.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] 10 days black, then 6 days red, then the yellow continues: black and red together exceed the maximum, so her hayd is only the black.
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 342; https://shamela.ws/book/3565/340] Same case and result.
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 404; https://shamela.ws/book/9059/405] Same principle: if the red cannot be joined to the black (e.g. 11 days red after 5 black), it is joined to the yellow instead (no khilaf here).
    // [Rulings of Haydh and Nifas in the Shafi'i school (8); https://youtu.be/WEabG3nBuEM?t=1961] 10 days black, 6 days red, then yellow: the hayd is the 10 days, because adding the red would exceed the maximum.
    [FiqhTodo]
    [Fact]
    public void MubtadaaMumayyiza_TenBlackSixRedThenYellow_OnlyBlackIsHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Black, 10),
            (EFlowAppearanceColor.Red, 6),
            (EFlowAppearanceColor.Yellow, 20));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 10.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((10.0, 36.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] 5 days red, then 5 days black, then the yellow continues: the weak comes first, so her hayd is only the black.
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 342; https://shamela.ws/book/3565/340] Same case and result (the strong blood that comes later is hayd according to the asahh).
    [FiqhTodo]
    [Fact]
    public void MubtadaaMumayyiza_FiveRedFiveBlackThenYellow_OnlyBlackIsHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Black, 5),
            (EFlowAppearanceColor.Yellow, 20));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((5.0, 10.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((0.0, 5.0), (10.0, 30.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] 5 days black, then 5 days yellow, then the red continues: the weak (red) does not follow the strong directly, so her hayd is only the black.
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 342; https://shamela.ws/book/3565/340] Same case and result, as stated by ar-Ruyani and the commentators of al-Hawi as-Saghir and corrected by an-Nawawi in at-Tahqiq.
    // KHILAF: [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 403f.; https://shamela.ws/book/9059/404] 5 days black, then 5 days yellow, then the red continues: the first ten days (black and yellow) are hayd.
    // KHILAF: [Rulings of Haydh and Nifas in the Shafi'i school (8); https://youtu.be/WEabG3nBuEM?t=1961] Black, yellow, then red: according to ar-Ramli the strong blood and the stronger of the two weak ones must be connected, so black and red cannot be combined; according to Ibn Hajar al-Haytami this condition is not needed and the black is combined with the yellow.
    // KHILAF: al-Majmu' and ar-Rawda treat this case like red between two blacks (reported in Mughni al-Muhtaj, vol. 1, p. 286 and Nihayat al-Muhtaj, vol. 1, p. 342). This test follows Nihayat al-Muhtaj and Mughni al-Muhtaj.
    [FiqhTodo]
    [Fact]
    public void MubtadaaMumayyiza_FiveBlackFiveYellowThenRed_OnlyBlackIsHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            (EFlowAppearanceColor.Black, 5),
            (EFlowAppearanceColor.Yellow, 5),
            (EFlowAppearanceColor.Red, 20));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((5.0, 30.0));
    }

    #endregion Mubtada'a mumayyiza (with distinguishable blood)

    #region Mu'tada (woman with an 'ada)

    // [an-Nawawi, Minhaj at-Talibin, p. 20; https://shamela.ws/book/12096/20] Mu'tada (woman with an 'ada): she returns to the hayd and purity of her 'ada in duration and timing; according to the sounder view the 'ada is established by a single occurrence.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 287f.; https://shamela.ws/book/11444/203] Example 5 days every month; if she had 5 days of hayd in one month and then became a mustahada, she returns to the 5 days.
    // KHILAF: The 'ada is established by a single occurrence according to the sounder view (asahh); other views require two or three occurrences (Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204). This test follows the asahh.
    [FiqhTodo]
    [Fact]
    public void Mutada_AdaOfFiveDays_ReturnsToAda()
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

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204] Hayd of 5 days in one month, then 6, then 7, then istihada: according to the sounder view ('ada by a single occurrence) she returns to the 7.
    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 148; https://shamela.ws/book/499/146] 'Ada of 5 days at the start of each month, then 6 in one cycle, 7 in the next, then istihada: if the 'ada is established by a single occurrence, she returns to the 7.
    // KHILAF: With an 'ada established by two occurrences she returns to the 6, with three occurrences to the 5 (Mughni al-Muhtaj, vol. 1, p. 288; ar-Rawda, vol. 1, p. 148). This test follows the asahh (single occurrence).
    [FiqhTodo]
    [Fact]
    public void Mutada_ChangingAdaFiveSixSeven_ReturnsToLastAda()
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

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] 'Ada of 5 days at the start of the month, then 14 days purity, then the blood returns and continues: the first day and night of the returning blood are purity, then 5 days hayd, and her cycle stays 20 days.
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 340 (Hashiyat ash-Shabramallisi); https://shamela.ws/book/3565/338] The same example after Ibn Hajar.
    // KHILAF: The 'ada is established by a single occurrence according to the sounder view (asahh); other views require two or three occurrences (Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204). This test follows the asahh.
    [FiqhTodo]
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

    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 149; https://shamela.ws/book/499/147] 'Ada of 5 days at the start of the month; after 20 days of purity the blood returns in the last 5 days and continues: according to the soundest view she has 5 days of hayd from its start and 20 days of purity, and so on.
    // KHILAF: ar-Rawda gives four views (awjuh) for this case: (1, the soundest) 5 days hayd from its start and 20 days purity, and so on; (2) 5 days hayd and 25 days purity; (3) 10 days hayd from it and 25 days purity, then the old cycle; (4) further view in ar-Rawda, vol. 1, p. 149. This test follows the soundest view.
    [FiqhTodo]
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

    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 148; https://shamela.ws/book/499/146] 'Ada of 5 days at the start of each month, the rest pure; in one cycle she has only 4 of the 5 days, then istihada: if the 'ada is established by a single occurrence (the sounder view), she returns to what came right before the istihada (4 days).
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204] The 'ada is established by a single occurrence according to the sounder view.
    // KHILAF: If the 'ada is not established by a single occurrence, she returns to her old 'ada of 5 days (ar-Rawda, vol. 1, p. 148). This test follows the asahh.
    [FiqhTodo]
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

    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 149; https://shamela.ws/book/499/147] 'Ada of 5 days at the start of the month; she sees the second five days as blood, it stops, she is pure the rest of the month (20 days), and the blood returns at the start of the month and continues: the first 5 days are hayd without dispute, and her purity is 20 days if the 'ada is established by a single occurrence.
    // KHILAF: If the 'ada is not established by a single occurrence, her purity is 25 days instead of 20 (ar-Rawda, vol. 1, p. 149). This test follows the asahh.
    [FiqhTodo]
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

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 287; https://shamela.ws/book/11444/203] 'Ada of 15 days at the start of each month; she sees 15 days red from the start of a month, then black continues: as the black goes on, it becomes clear that she returns to her 'ada (after al-Barizi).
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 403; https://shamela.ws/book/9059/404] Same case and result (no khilaf with Mughni al-Muhtaj here).
    // KHILAF: The 'ada is established by a single occurrence according to the sounder view (asahh); other views require two or three occurrences (Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204). This test follows the asahh.
    [FiqhTodo]
    [Fact]
    public void Mutada_AdaOfFifteenDays_RedThenContinuousBlack_ReturnsToAda()
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

    #endregion Mu'tada (woman with an 'ada)

    #region Mu'tada mumayyiza (woman with an 'ada and distinguishable blood)

    // [an-Nawawi, Minhaj at-Talibin, p. 20; https://shamela.ws/book/12096/20] For a woman with an 'ada and distinguishable blood, the distinction decides according to the sounder view, not the 'ada.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204] 'Ada of 5 days at the start of the month, the rest pure; then 10 days black from the start of the month and the rest red: her hayd are the 10 black days.
    // KHILAF: The other view (not the asahh) decides by the 'ada: her hayd would be the first 5 days (Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204). This test follows the asahh (distinction).
    [FiqhTodo]
    [Fact]
    public void MutadaMumayyiza_TenDaysBlackAgainstAdaOfFive_TamyizWins()
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

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 288f.; https://shamela.ws/book/11444/204] If a full minimal purity lies between 'ada and distinction, both are used: after her 5 'ada days at the start of the month she sees 20 days red, then 5 days black, then red; according to the soundest view both (the 5 'ada days and the 5 black days) are hayd.
    // [an-Nawawi, Minhaj at-Talibin, p. 19; https://shamela.ws/book/12096/19] The minimal purity between two hayds is 15 days, so the red right after the black cannot be hayd.
    // KHILAF: Another view (wa qila) applies the khilaf between distinction and 'ada here as well (Mughni al-Muhtaj, vol. 1, p. 289; https://shamela.ws/book/11444/205). This test follows the asahh (both are hayd).
    [FiqhTodo]
    [Fact]
    public void MutadaMumayyiza_FullTuhurBetweenAdaAndBlack_BothAreHayd()
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

    #endregion Mu'tada mumayyiza (woman with an 'ada and distinguishable blood)

    #region Other views (khilaf), selected through HaydCalculatorSettings

    // [Rulings of Haydh and Nifas in the Shafi'i school (8); https://youtu.be/WEabG3nBuEM?t=2342] 7 days each of black, red, black: according to Ibn Hajar al-Haytami only the first black is hayd.
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 401; https://shamela.ws/book/9059/402] For 7 black, 7 red, 3 black (then it stops) only the first black is hayd. [IMPLICIT] (for 7/7/7)
    [FiqhTodo]
    [Fact]
    public void MubtadaaMumayyiza_SevenDaysEachBlackRedBlack_IbnHajar_OnlyFirstBlackIsHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            HaydCalculatorSettings.IbnHajar,
            (EFlowAppearanceColor.Black, 7),
            (EFlowAppearanceColor.Red, 7),
            (EFlowAppearanceColor.Black, 7));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 7.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((7.0, 21.0));
    }

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 403f.; https://shamela.ws/book/9059/404] 5 days black, then 5 days yellow, then the red continues: the first ten days are hayd.
    // [Rulings of Haydh and Nifas in the Shafi'i school (8); https://youtu.be/WEabG3nBuEM?t=1961] According to Ibn Hajar al-Haytami the connection condition is not needed and the black is combined with the yellow.
    [FiqhTodo]
    [Fact]
    public void MubtadaaMumayyiza_FiveBlackFiveYellowThenRed_IbnHajar_BlackAndYellowAreHayd()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            HaydCalculatorSettings.IbnHajar,
            (EFlowAppearanceColor.Black, 5),
            (EFlowAppearanceColor.Yellow, 5),
            (EFlowAppearanceColor.Red, 20));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 10.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((10.0, 30.0));
    }

    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 148; https://shamela.ws/book/499/146] 'Ada of 5 days, then 6, then 7, then istihada: if the 'ada is established by two occurrences, she returns to the 6 (the soundest view on that basis).
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204] 5, then 6, then 7: with two occurrences she returns to the 6.
    [FiqhTodo]
    [Fact]
    public void Mutada_ChangingAdaFiveSixSeven_AdaByTwoOccurrences_ReturnsToSix()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            HaydCalculatorSettings.Default with { OccurrencesToEstablishAda = 2 },
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 6),
            (EFlowAppearanceColor.Clear, 24),
            (EFlowAppearanceColor.Red, 7),
            (EFlowAppearanceColor.Clear, 23),
            (EFlowAppearanceColor.Red, 30));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 36.0), (60.0, 67.0), (90.0, 96.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((96.0, 120.0));
    }

    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 148; https://shamela.ws/book/499/146] 'Ada of 5 days, then 6, then 7, then istihada: if the 'ada is only established by three occurrences, she returns to the 5.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204] 5, then 6, then 7: with three occurrences she returns to the 5.
    [FiqhTodo]
    [Fact]
    public void Mutada_ChangingAdaFiveSixSeven_AdaByThreeOccurrences_ReturnsToFive()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            HaydCalculatorSettings.Default with { OccurrencesToEstablishAda = 3 },
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 6),
            (EFlowAppearanceColor.Clear, 24),
            (EFlowAppearanceColor.Red, 7),
            (EFlowAppearanceColor.Clear, 23),
            (EFlowAppearanceColor.Red, 30));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 36.0), (60.0, 67.0), (90.0, 95.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((95.0, 120.0));
    }

    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 149; https://shamela.ws/book/499/147] Second view for the 'ada that came earlier with continuous returning blood: 5 days hayd, then 25 days purity.
    [FiqhTodo]
    [Fact]
    public void Mutada_BloodReturnsEarlierAndContinues_OldTuhurLength_FiveDaysHaydTwentyFiveDaysTuhur()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            HaydCalculatorSettings.Default with { ShiftedAdaWithContinuousBlood = EShiftedAdaWithContinuousBlood.OldTuhurLength },
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 20),
            (EFlowAppearanceColor.Red, 30));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 35.0), (55.0, 60.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((60.0, 85.0));
    }

    // [an-Nawawi, Rawdat at-Talibin, vol. 1, p. 149; https://shamela.ws/book/499/147] Third view for the 'ada that came earlier with continuous returning blood: 10 days hayd from it, then 25 days purity, then she keeps to the old cycle.
    [FiqhTodo]
    [Fact]
    public void Mutada_BloodReturnsEarlierAndContinues_TenDaysThenOldCycle()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            HaydCalculatorSettings.Default with { ShiftedAdaWithContinuousBlood = EShiftedAdaWithContinuousBlood.TenDaysThenOldCycle },
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 20),
            (EFlowAppearanceColor.Red, 30));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 35.0), (55.0, 65.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((65.0, 85.0));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204] Other view for the mu'tada mumayyiza: the 'ada decides, so with an 'ada of 5 days and then 10 days black from the start of the month her hayd is the first 5 days.
    [FiqhTodo]
    [Fact]
    public void MutadaMumayyiza_TenDaysBlackAgainstAdaOfFive_FollowAda_AdaWins()
    {
        // ACT
        HaydCalculationResultVO result = Calculate(
            HaydCalculatorSettings.Default with { MutadaMumayyizaFollows = EMutadaMumayyizaFollows.Ada },
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Black, 10),
            (EFlowAppearanceColor.Red, 20));

        // ASSERT
        result.GetHaydDayRanges(s_start).Should().Equal((0.0, 5.0), (30.0, 35.0));
        result.GetIstihadaDayRanges(s_start).Should().Equal((35.0, 60.0));
    }

    #endregion Other views (khilaf), selected through HaydCalculatorSettings
}
