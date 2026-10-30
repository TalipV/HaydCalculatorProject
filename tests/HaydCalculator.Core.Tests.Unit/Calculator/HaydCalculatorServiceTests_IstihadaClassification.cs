using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Calculator.Services;
using HaydCalculator.Core.Misc;
using HaydCalculator.Core.Tests.Unit.Util;
using Microsoft.Extensions.Logging.Abstractions;

namespace HaydCalculator.Core.Tests.Unit.Calculator;

/// <summary>
/// Whether a course of bleeding is a mustahada case (blood beyond the maximum hayd of 15 days,
/// handled by the rules for the mustahada: tamyiz, 'ada and so on) or is settled by the basic
/// rules alone. These tests only check that classification, not which kind of mustahada it is
/// or which days are hayd. The classification belongs to each hayd cycle: in one course of
/// bleeding the first cycle can be a basic one and the next a mustahada case.
/// TODO: The calculator has no state for this yet; a mustahada case currently shows up as the
/// InfoException <see cref="TextUtil.ComplexIstihadaNotImplemented"/>. Once each hayd cycle
/// carries a bool for it (e.g. IsComplexIstihada), assert that bool in
/// <see cref="AssertMustahadaCase"/> and <see cref="AssertNoMustahadaCase"/> instead, and enable
/// the commented-out assertions in the tests with more than one cycle.
/// </summary>
public class HaydCalculatorServiceTests_IstihadaClassification
{
    private static readonly Instant s_start = Instant.FromUtc(2024, 1, 1, 0, 0);

    private readonly HaydCalculatorService _haydCalculatorService = new(NullLogger<HaydCalculatorService>.Instance);

    private HaydCalculationResultVO Calculate(params (EFlowAppearanceColor type, double dayCount)[] flows)
    {
        List<(EFlowAppearanceColor type, double dayCount)> flowList = [.. flows];
        return _haydCalculatorService.Calculate(flowList.GetFlowDataList(initialInstant: s_start));
    }

    // TODO: Assert a bool state of each hayd cycle (e.g. IsComplexIstihada == true) instead of the InfoException.
    private void AssertMustahadaCase(params (EFlowAppearanceColor type, double dayCount)[] flows)
    {
        InfoException exception = Assert.Throws<InfoException>(() => Calculate(flows));
        exception.Message.Should().Be(TextUtil.ComplexIstihadaNotImplemented);
    }

    // TODO: Assert a bool state of each hayd cycle (e.g. IsComplexIstihada == false) instead of only checking that nothing is thrown.
    private void AssertNoMustahadaCase(params (EFlowAppearanceColor type, double dayCount)[] flows)
    {
        Action act = () => Calculate(flows);
        act.Should().NotThrow();
    }

    #region Mustahada case

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] A woman whose blood exceeds 15 days is called mustahada; then follow her seven kinds. [IMPLICIT]
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 340; https://shamela.ws/book/3565/338] The same: blood that exceeds 15 days makes her a mustahada. [IMPLICIT]
    // KHILAF: The mu'tamad works agree on this point; no khilaf here.
    [FiqhTodo]
    [Fact]
    public void SixteenDaysRed_IsMustahadaCase()
    {
        AssertMustahadaCase(
            (EFlowAppearanceColor.Red, 16));
    }

    // Same rule as the previous test; here the blood after day 15 is entered as a separate flow that follows directly.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] A woman whose blood exceeds 15 days is called mustahada. [IMPLICIT]
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 340; https://shamela.ws/book/3565/338] The same. [IMPLICIT]
    // KHILAF: The mu'tamad works agree on this point; no khilaf here.
    [FiqhTodo]
    [Fact]
    public void FifteenDaysRedThenRedDirectlyAfter_IsMustahadaCase()
    {
        AssertMustahadaCase(
            (EFlowAppearanceColor.Red, 15),
            (EFlowAppearanceColor.Red, 5));
    }

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] 5 days black, then red: the red is hayd too; if it stops before 15 days that stays so, otherwise only the black is hayd.
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] Once the blood exceeds 15 days, a mubtada'a with strong and weak blood follows the tamyiz. [IMPLICIT]
    // KHILAF: The mu'tamad works agree on this point; no khilaf here.
    [FiqhTodo]
    [Fact]
    public void FiveDaysBlackThenFifteenDaysRed_IsMustahadaCase()
    {
        AssertMustahadaCase(
            (EFlowAppearanceColor.Black, 5),
            (EFlowAppearanceColor.Red, 15));
    }

    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 356; https://shamela.ws/book/3565/354] Purity between bloods is hayd only if the whole does not exceed 15 days; if the intermittent bleeding exceeds 15 days, the rules of the mustahada apply. [IMPLICIT]
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210] If the bloods together with the purity between them exceed 15 days, they are istihada. [IMPLICIT]
    // The blood here continues well beyond day 15 on purpose: whether intermittent blood that stops shortly after day 15 is a mustahada case is an open question, see HaydCalculatorServiceTests_OpenQuestions.ThirteenRedOnePurityFiveRed_DamFasadOrMustahada.
    // KHILAF: Purity between bloods is hayd (qawl as-sahb, azhar) or purity (qawl al-laqt, muqabil al-azhar, a strong view) (Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210). The sources state the limit of 15 days after both views, so both give a mustahada case here.
    [FiqhTodo]
    [Fact]
    public void TenDaysRedThreeDaysPurityThenTwentyDaysRed_IsMustahadaCase()
    {
        AssertMustahadaCase(
            (EFlowAppearanceColor.Red, 10),
            (EFlowAppearanceColor.Clear, 3),
            (EFlowAppearanceColor.Red, 20));
    }

    // Two cycles: the first 3 days are a basic hayd; the returning blood completes the purity and then exceeds 15 days, so the second cycle is a mustahada case.
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] 3 days blood, 12 days purity, 3 days blood, then it stops: the last 3 days are dam fasad; if instead the blood continues, a mubtada'a is ghayr mumayyiza and a mu'tada follows her 'ada.
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 340 (Hashiyat ash-Shabramallisi); https://shamela.ws/book/3565/338] The same restriction after Ibn Hajar: "then it stops" excludes the case where the blood continues.
    // [Ba 'Alawi, Bughyat al-Mustarshidin, Dar al-Minhaj ed., vol. 1, p. 277 (after Fatawa Ibn Yahya, p. 24)] The part of the returning blood that completes the purity is dam fasad; if the rest exceeds 15 days, the mubtada'a ghayr mumayyiza takes a day and a night from its start and the mu'tada her 'ada. Here the rest (days 19-35) is 17 days. [IMPLICIT]
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] Blood that exceeds 15 days makes her a mustahada. [IMPLICIT]
    // KHILAF: Whether the first 3 days already give her an 'ada (asahh: one occurrence is enough; muqabil al-asahh, a strong view: two or three occurrences, Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204) only decides which kind of mustahada she is; Ibn Hajar names both kinds, so both views give a mustahada case here.
    [FiqhTodo]
    [Fact]
    public void ThreeDaysRedTwelveDaysPurityThenTwentyDaysRed_FirstCycleNoMustahadaCaseSecondCycleMustahadaCase()
    {
        // TODO: Enable once each hayd cycle carries the bool state; the InfoException stops the whole calculation and cannot express a result per cycle.
        //HaydCalculationResultVO result = Calculate(
        //    (EFlowAppearanceColor.Red, 3),
        //    (EFlowAppearanceColor.Clear, 12),
        //    (EFlowAppearanceColor.Red, 20));
        //
        //result.HaydCycles[0].IsComplexIstihada.Should().BeFalse();
        //result.HaydCycles[1].IsComplexIstihada.Should().BeTrue();
        throw new NotImplementedException("The mustahada classification per hayd cycle is not implemented yet.");
    }

    // Two cycles: the first day is a basic hayd; the returning blood completes the purity with its first day and then exceeds 15 days, so the second cycle is a mustahada case.
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] 'Ada of 5 days, then 14 days purity, then the blood returns and continues: the first day and night of it complete the purity, then she follows her 'ada, so the rules of the mustahada apply. [IMPLICIT]
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 340 (Hashiyat ash-Shabramallisi); https://shamela.ws/book/3565/338] The same example after Ibn Hajar. [IMPLICIT]
    // [Ba 'Alawi, Bughyat al-Mustarshidin, Dar al-Minhaj ed., vol. 1, p. 277 (after Fatawa Ibn Yahya, p. 24)] The part of the returning blood that completes the purity is dam fasad; if the rest exceeds 15 days, the rules of the mustahada apply. Here the rest (days 17-35) is 19 days. Ba 'Alawi's case has a first blood of more than a day and a night; here it is exactly a day and a night. [IMPLICIT]
    // KHILAF: Whether the single day of blood gives her an 'ada (asahh: one occurrence is enough; muqabil al-asahh, a strong view: two or three occurrences, Mughni al-Muhtaj, vol. 1, p. 288; https://shamela.ws/book/11444/204) only decides which kind of mustahada she is (mu'tada or mubtada'a); both views give a mustahada case here.
    [FiqhTodo]
    [Fact]
    public void OneDayRedFourteenDaysPurityThenTwentyDaysRed_FirstCycleNoMustahadaCaseSecondCycleMustahadaCase()
    {
        // TODO: Enable once each hayd cycle carries the bool state; the InfoException stops the whole calculation and cannot express a result per cycle.
        //HaydCalculationResultVO result = Calculate(
        //    (EFlowAppearanceColor.Red, 1),
        //    (EFlowAppearanceColor.Clear, 14),
        //    (EFlowAppearanceColor.Red, 20));
        //
        //result.HaydCycles[0].IsComplexIstihada.Should().BeFalse();
        //result.HaydCycles[1].IsComplexIstihada.Should().BeTrue();
        throw new NotImplementedException("The mustahada classification per hayd cycle is not implemented yet.");
    }

    // Two cycles: the first 3 days are a basic hayd; the returning blood completes the purity on day 18, the rest (days 19-35) is 17 days, so the second cycle is a mustahada case.
    // [Ba 'Alawi, Bughyat al-Mustarshidin, Dar al-Minhaj ed., vol. 1, p. 277 (after Fatawa Ibn Yahya, p. 24)] The part of the returning blood that completes the purity is dam fasad; the rest is hayd if it meets its conditions and does not exceed 15 days, otherwise the rules of the mustahada apply. Here the rest is 17 days. [IMPLICIT]
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] If the returning blood continues, a mubtada'a is ghayr mumayyiza and a mu'tada follows her 'ada, i.e. the rules of the mustahada apply. [IMPLICIT]
    // KHILAF (mubtada'a): Ash-Shabramallisi, on exactly this kind of case (Ibn Qasim's "6 days of blood after the purity", blood that stops; Tuhfat al-Muhtaj, vol. 1, p. 399, hawashi; https://shamela.ws/book/9059/400), infers from Ibn Hajar's wording («يقتضي») that the rule holds for the mu'tada only and that the mubtada'a has a day and a night from the start of the month;
    // al-Bujayrimi's reasoning on the same page (the time of purity counts as weak blood and is under the minimal purity) points the same way. For the rule as tested: Ba 'Alawi (Bughya), as-Saqqaf (al-Ibana, p. 29-31), Ibn Qasim («لا يبعد»). Which reading the tests follow for a mubtada'a is open for the user's decision (open question 7).
    // (Correction 2026-10-30, found by an independent review: an earlier line said ash-Shabramallisi only remarked on continuous blood without holding a view.)
    [FiqhTodo]
    [Fact]
    public void ThreeDaysRedFourteenDaysPurityThenSeventeenDaysRed_FirstCycleNoMustahadaCaseSecondCycleMustahadaCase()
    {
        // TODO: Enable once each hayd cycle carries the bool state; the InfoException stops the whole calculation and cannot express a result per cycle.
        //HaydCalculationResultVO result = Calculate(
        //    (EFlowAppearanceColor.Red, 3),
        //    (EFlowAppearanceColor.Clear, 14),
        //    (EFlowAppearanceColor.Red, 17));
        //
        //result.HaydCycles[0].IsComplexIstihada.Should().BeFalse();
        //result.HaydCycles[1].IsComplexIstihada.Should().BeTrue();
        throw new NotImplementedException("The mustahada classification per hayd cycle is not implemented yet.");
    }

    // Three cycles of a mu'tada ('ada of 5 days): the returning blood completes the purity on day 50, the rest (days 51-66) is 16 days, so only the third cycle is a mustahada case.
    // [Ba 'Alawi, Bughyat al-Mustarshidin, Dar al-Minhaj ed., vol. 1, p. 277 (after Fatawa Ibn Yahya, p. 24)] The part of the returning blood that completes the purity is dam fasad; the rest is hayd if it meets its conditions and does not exceed 15 days, otherwise the rules of the mustahada apply. Here the rest is 16 days. [IMPLICIT]
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] If the returning blood continues, a mubtada'a is ghayr mumayyiza and a mu'tada follows her 'ada, i.e. the rules of the mustahada apply. [IMPLICIT]
    // KHILAF: The works checked give no other view; ash-Shabramallisi's remark (Nihayat al-Muhtaj, vol. 1, p. 340; https://shamela.ws/book/3565/338) only concerns the mubtada'a.
    [FiqhTodo]
    [Fact]
    public void MutadaFourteenDaysPurityThenSeventeenDaysRed_OnlyThirdCycleMustahadaCase()
    {
        // TODO: Enable once each hayd cycle carries the bool state; the InfoException stops the whole calculation and cannot express a result per cycle.
        //HaydCalculationResultVO result = Calculate(
        //    (EFlowAppearanceColor.Red, 5),
        //    (EFlowAppearanceColor.Clear, 25),
        //    (EFlowAppearanceColor.Red, 5),
        //    (EFlowAppearanceColor.Clear, 14),
        //    (EFlowAppearanceColor.Red, 17));
        //
        //result.HaydCycles[0].IsComplexIstihada.Should().BeFalse();
        //result.HaydCycles[1].IsComplexIstihada.Should().BeFalse();
        //result.HaydCycles[2].IsComplexIstihada.Should().BeTrue();
        throw new NotImplementedException("The mustahada classification per hayd cycle is not implemented yet.");
    }

    #endregion Mustahada case

    #region No mustahada case (basic rules)

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 284; https://shamela.ws/book/11444/200] Blood of at least a day and a night that does not exceed the maximum is all hayd, whatever its colour, for a mubtada'a and a mu'tada alike. [IMPLICIT]
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 339; https://shamela.ws/book/3565/337] The same. [IMPLICIT]
    // KHILAF: The mu'tamad works agree on this point; no khilaf here.
    [FiqhTodo]
    [Fact]
    public void FifteenDaysRedThenStop_IsNoMustahadaCase()
    {
        AssertNoMustahadaCase(
            (EFlowAppearanceColor.Red, 15));
    }

    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] 5 days black, then red: the red is hayd too, and if it stops before 15 days that stays so. [IMPLICIT]
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 339; https://shamela.ws/book/3565/337] Blood within the maximum is all hayd, whether of one kind or divided into strong and weak. [IMPLICIT]
    // KHILAF: The mu'tamad works agree on this point; no khilaf here.
    [FiqhTodo]
    [Fact]
    public void FiveDaysBlackFiveDaysRedThenStop_IsNoMustahadaCase()
    {
        AssertNoMustahadaCase(
            (EFlowAppearanceColor.Black, 5),
            (EFlowAppearanceColor.Red, 5));
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 284; https://shamela.ws/book/11444/200] 3 days blood, 12 days purity, 3 days blood, then it stops: the last 3 days are dam fasad, not hayd.
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 339f.; https://shamela.ws/book/3565/337] The same case, same result.
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] The same case, same result.
    // KHILAF: The mu'tamad works agree on this point; no khilaf here.
    [FiqhTodo]
    [Fact]
    public void ThreeDaysRedTwelveDaysPurityThreeDaysRedThenStop_IsNoMustahadaCase()
    {
        AssertNoMustahadaCase(
            (EFlowAppearanceColor.Red, 3),
            (EFlowAppearanceColor.Clear, 12),
            (EFlowAppearanceColor.Red, 3));
    }

    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 356; https://shamela.ws/book/3565/354] Purity between bloods is hayd if the whole does not exceed 15 days and the bloods together reach a day and a night. [IMPLICIT]
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210] The same; only bloods that exceed 15 days together with the purity are istihada. [IMPLICIT]
    // KHILAF: Purity between bloods is hayd (qawl as-sahb, azhar) or purity (qawl al-laqt, muqabil al-azhar, a strong view) (Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210). This only decides whether the 5 days of purity count as hayd; both views give no mustahada case here.
    [FiqhTodo]
    [Fact]
    public void ThreeDaysRedFiveDaysPurityThreeDaysRedThenStop_IsNoMustahadaCase()
    {
        AssertNoMustahadaCase(
            (EFlowAppearanceColor.Red, 3),
            (EFlowAppearanceColor.Clear, 5),
            (EFlowAppearanceColor.Red, 3));
    }

    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 340 (Hashiyat ash-Shabramallisi); https://shamela.ws/book/3565/338] If the intermittent blood stops at the end of the 15 days or within them, the blood together with the purity is hayd.
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 356; https://shamela.ws/book/3565/354] Purity between bloods is hayd if the whole does not exceed 15 days. [IMPLICIT]
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210] Only bloods that exceed 15 days together with the purity are istihada. [IMPLICIT]
    // KHILAF: Purity between bloods is hayd (qawl as-sahb, azhar) or purity (qawl al-laqt, muqabil al-azhar, a strong view) (Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210). This only decides whether the 4 days of purity count as hayd; both views give no mustahada case here.
    [FiqhTodo]
    [Fact]
    public void TenDaysRedFourDaysPurityOneDayRedEndingAtDayFifteen_IsNoMustahadaCase()
    {
        AssertNoMustahadaCase(
            (EFlowAppearanceColor.Red, 10),
            (EFlowAppearanceColor.Clear, 4),
            (EFlowAppearanceColor.Red, 1));
    }

    // Day 18 completes the purity (dam fasad), days 19-21 are a new hayd; see also HaydCalculation_CalculationWithPurityBetweenTwoSeparateMenstrualCyclesIncludingIstihada.
    // [Ba 'Alawi, Bughyat al-Mustarshidin, Dar al-Minhaj ed., vol. 1, p. 277 (after Fatawa Ibn Yahya, p. 24)] The part of the returning blood that completes the purity is dam fasad; the rest is hayd if it meets its conditions and does not exceed 15 days, otherwise the rules of the mustahada apply. Here the rest is 3 days. [IMPLICIT]
    // [Ibn Qasim al-'Abbadi, Hashiya on Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] The part of the returning blood beyond the completion of the purity: "it is not far-fetched" that it is hayd. [IMPLICIT]
    // KHILAF (mubtada'a): Ash-Shabramallisi, on exactly this kind of case (Ibn Qasim's "6 days of blood after the purity", blood that stops; Tuhfat al-Muhtaj, vol. 1, p. 399, hawashi; https://shamela.ws/book/9059/400), infers from Ibn Hajar's wording («يقتضي») that the rule holds for the mu'tada only and that the mubtada'a has a day and a night from the start of the month;
    // al-Bujayrimi's reasoning on the same page (the time of purity counts as weak blood and is under the minimal purity) points the same way. For the rule as tested: Ba 'Alawi (Bughya), as-Saqqaf (al-Ibana, p. 29-31), Ibn Qasim («لا يبعد»). Which reading the tests follow for a mubtada'a is open for the user's decision (open question 7).
    // (Correction 2026-10-30, found by an independent review: an earlier line said ash-Shabramallisi only remarked on continuous blood without holding a view.)
    [FiqhTodo]
    [Fact]
    public void ThreeDaysRedFourteenDaysPurityFourDaysRedThenStop_IsNoMustahadaCase()
    {
        AssertNoMustahadaCase(
            (EFlowAppearanceColor.Red, 3),
            (EFlowAppearanceColor.Clear, 14),
            (EFlowAppearanceColor.Red, 4));
    }

    // Days 19-25 complete the purity (dam fasad), days 26-30 are a new hayd; see also HaydCalculation_CalculationWithPartiallyIstihadaAndPartiallyMenstrualFlow.
    // [Ba 'Alawi, Bughyat al-Mustarshidin, Dar al-Minhaj ed., vol. 1, p. 277 (after Fatawa Ibn Yahya, p. 24)] The part of the returning blood that completes the purity is dam fasad; the rest is hayd if it meets its conditions and does not exceed 15 days, otherwise the rules of the mustahada apply. Here the rest is 5 days. [IMPLICIT]
    // [Ibn Qasim al-'Abbadi, Hashiya on Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] The part of the returning blood beyond the completion of the purity: "it is not far-fetched" that it is hayd. [IMPLICIT]
    // KHILAF (mubtada'a): Ash-Shabramallisi, on exactly this kind of case (Ibn Qasim's "6 days of blood after the purity", blood that stops; Tuhfat al-Muhtaj, vol. 1, p. 399, hawashi; https://shamela.ws/book/9059/400), infers from Ibn Hajar's wording («يقتضي») that the rule holds for the mu'tada only and that the mubtada'a has a day and a night from the start of the month;
    // al-Bujayrimi's reasoning on the same page (the time of purity counts as weak blood and is under the minimal purity) points the same way. For the rule as tested: Ba 'Alawi (Bughya), as-Saqqaf (al-Ibana, p. 29-31), Ibn Qasim («لا يبعد»). Which reading the tests follow for a mubtada'a is open for the user's decision (open question 7).
    // (Correction 2026-10-30, found by an independent review: an earlier line said ash-Shabramallisi only remarked on continuous blood without holding a view.)
    [FiqhTodo]
    [Fact]
    public void TenDaysRedEightDaysPurityTwelveDaysRedThenStop_IsNoMustahadaCase()
    {
        AssertNoMustahadaCase(
            (EFlowAppearanceColor.Red, 10),
            (EFlowAppearanceColor.Clear, 8),
            (EFlowAppearanceColor.Red, 12));
    }

    // Day 18 completes the purity (dam fasad), the rest (days 19-33) is exactly 15 days, so it is a new hayd without exceeding the maximum.
    // [Ba 'Alawi, Bughyat al-Mustarshidin, Dar al-Minhaj ed., vol. 1, p. 277 (after Fatawa Ibn Yahya, p. 24)] The part of the returning blood that completes the purity is dam fasad; the rest is hayd if it meets its conditions and does not exceed 15 days, otherwise the rules of the mustahada apply. Here the rest is exactly 15 days. [IMPLICIT]
    // [Ibn Qasim al-'Abbadi, Hashiya on Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] The part of the returning blood beyond the completion of the purity: "it is not far-fetched" that it is hayd. [IMPLICIT]
    // KHILAF (mubtada'a): Ash-Shabramallisi, on exactly this kind of case (Ibn Qasim's "6 days of blood after the purity", blood that stops; Tuhfat al-Muhtaj, vol. 1, p. 399, hawashi; https://shamela.ws/book/9059/400), infers from Ibn Hajar's wording («يقتضي») that the rule holds for the mu'tada only and that the mubtada'a has a day and a night from the start of the month;
    // al-Bujayrimi's reasoning on the same page (the time of purity counts as weak blood and is under the minimal purity) points the same way. For the rule as tested: Ba 'Alawi (Bughya), as-Saqqaf (al-Ibana, p. 29-31), Ibn Qasim («لا يبعد»). Which reading the tests follow for a mubtada'a is open for the user's decision (open question 7).
    // (Correction 2026-10-30, found by an independent review: an earlier line said ash-Shabramallisi only remarked on continuous blood without holding a view.)
    [FiqhTodo]
    [Fact]
    public void ThreeDaysRedFourteenDaysPuritySixteenDaysRedThenStop_IsNoMustahadaCase()
    {
        AssertNoMustahadaCase(
            (EFlowAppearanceColor.Red, 3),
            (EFlowAppearanceColor.Clear, 14),
            (EFlowAppearanceColor.Red, 16));
    }

    // Mu'tada with an 'ada of 5 days: day 50 completes the purity (dam fasad), the rest (days 51-65) is exactly 15 days, so it is a new hayd without exceeding the maximum.
    // [Ba 'Alawi, Bughyat al-Mustarshidin, Dar al-Minhaj ed., vol. 1, p. 277 (after Fatawa Ibn Yahya, p. 24)] The part of the returning blood that completes the purity is dam fasad; the rest is hayd if it meets its conditions and does not exceed 15 days, otherwise the rules of the mustahada apply. Here the rest is exactly 15 days. [IMPLICIT]
    // [Ibn Qasim al-'Abbadi, Hashiya on Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400] The part of the returning blood beyond the completion of the purity: "it is not far-fetched" that it is hayd. [IMPLICIT]
    // KHILAF: The works checked give no other view; ash-Shabramallisi's remark (Nihayat al-Muhtaj, vol. 1, p. 340; https://shamela.ws/book/3565/338) only concerns the mubtada'a.
    [FiqhTodo]
    [Fact]
    public void MutadaFourteenDaysPuritySixteenDaysRedThenStop_IsNoMustahadaCase()
    {
        AssertNoMustahadaCase(
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 25),
            (EFlowAppearanceColor.Red, 5),
            (EFlowAppearanceColor.Clear, 14),
            (EFlowAppearanceColor.Red, 16));
    }

    #endregion No mustahada case (basic rules)
}
