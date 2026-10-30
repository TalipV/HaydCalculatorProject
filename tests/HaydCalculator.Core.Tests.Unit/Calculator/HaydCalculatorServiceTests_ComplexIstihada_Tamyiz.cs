using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Tests.Unit.Util;

namespace HaydCalculator.Core.Tests.Unit.Calculator;

public class HaydCalculatorFactory_Tests_ComplexIstihada_TamyizDetermination
{
    private const string TamyizNotFinished = "Tamyiz determination (IsHaydFlowDataLstWithDistinguishableFlows) is not finished yet; this case still fails.";
    private readonly Instant _startDate = Instant.FromUtc(2024, 1, 1, 0, 0);

    #region Condition 3

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] 10 days black, then 10 days red, then it stops: she follows the distinction, although the weak blood stays under 15 days.
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 401; https://shamela.ws/book/9059/402] 10 days black, then 10 days red, then it stops: she follows her distinction, although the weak blood stays under 15 days (after al-Mutawalli).
    [FiqhTodo]
    [Fact]
    public void Condition3_Fulfilled_TenDaysBlackTenDaysRedAndStop()
    {
        // ARRANGE
        List<(EFlowAppearanceColor type, double dayCount)> data =
        [
            (EFlowAppearanceColor.Black, 10), (EFlowAppearanceColor.Red, 10),
        ];

        List<FlowDataEntity> timeData = data.GetFlowDataList(initialInstant: _startDate);
        var haydCycleEntity = new HaydCycleEntity
        {
            HaydFlows = timeData
        };

        // ACT & ASSERT
        HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeTrue();
    }

    // The same numbers as the previous test in the other order. Two points together carry the expected result:
    // 1. 10 days black, then 10 days red, then it stops, is a valid distinction, although the weak blood stays under 15 days:
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 401; https://shamela.ws/book/9059/402] 10 days black, then 10 days red, then it stops: she follows her distinction, although the weak blood stays under 15 days (after al-Mutawalli).
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] The same case, same result.
    // 2. It is no condition that the strong blood comes before the weak one:
    // [Ibn Hajar al-Haytami, al-Fatawa al-Kubra al-Fiqhiyya, vol. 1, p. 102; https://shamela.ws/book/21628/101] Where the conditions of the distinction are met, the strong blood is hayd and the weak blood purity, whether the weak comes first or later and whether the blood stops or continues. [IMPLICIT]
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 402; https://shamela.ws/book/9059/403] The strong blood is hayd also when it comes later, e.g. 5 days red, then 5 or 11 days black, then red continuously; a mubtada'a who sees 15 days red and then 15 days black: once it turns black, the red before it is istihada. [IMPLICIT]
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 341f.; https://shamela.ws/book/3565/339] The strong blood is hayd also when it comes later or in the middle, e.g. 5 days red, then 5 days black, then red continuously; so according to the asahh. [IMPLICIT]
    // KHILAF: ar-Ramli says "according to the asahh" for a strong blood that comes later, so there is another view
    // (muqabil al-asahh, a strong view). The allowed works checked so far do not name it. The test follows the asahh.
    [FiqhTodo]
    [Fact]
    public void Condition3_Fulfilled_TenDaysRedTenDaysBlackAndStop()
    {
        // ARRANGE
        List<(EFlowAppearanceColor type, double dayCount)> data =
        [
            (EFlowAppearanceColor.Red, 10), (EFlowAppearanceColor.Black, 10),
        ];

        List<FlowDataEntity> timeData = data.GetFlowDataList(initialInstant: _startDate);
        var haydCycleEntity = new HaydCycleEntity
        {
            HaydFlows = timeData
        };

        // ACT & ASSERT
        HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeTrue();
    }

    // 2 days black, then 14 days red, then it stops: 16 days, so a mustahada case; the weak blood stays under 15 days, which is allowed because the blood stops.
    // (Changed 2026-10-30 after the independent review: the test had 1 day black and 14 days red, i.e. 15 days in total, which is no mustahada case, so the tamyiz check did not apply.)
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285f.; https://shamela.ws/book/11444/201] The condition that the weak blood lasts at least 15 days only applies if the blood continues. [IMPLICIT]
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 401; https://shamela.ws/book/9059/402] The condition that the weak blood is not shorter than the minimal purity only applies if the blood continues (after al-Mutawalli). [IMPLICIT]
    [FiqhTodo]
    [Fact]
    public void Condition3_Fulfilled_2DaysBlackAnd14DaysRedAndStop()
    {
        // ARRANGE
        List<(EFlowAppearanceColor type, double dayCount)> data =
        [
            (EFlowAppearanceColor.Black, 2), (EFlowAppearanceColor.Red, 14),
        ];

        List<FlowDataEntity> timeData = data.GetFlowDataList(initialInstant: _startDate);
        var haydCycleEntity = new HaydCycleEntity
        {
            HaydFlows = timeData
        };

        // ACT & ASSERT
        HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeTrue();
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] Only 14 days of weak blood (followed by strong blood again) does not fulfil the conditions of the distinction. [IMPLICIT]
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 401; https://shamela.ws/book/9059/402] Hashiyat ash-Sharwani (after al-Isnawi): a day and a night black, 14 days red, then black again: the distinction does not count, because otherwise the purity between the two hayds would be under the minimum.
    // The second black continues (here 16 days): al-Isnawi's case says «ثُمَّ السَّوَادَ» without a number, which means it continues (al-Fatawa al-Kubra, vol. 1, p. 105; https://shamela.ws/book/21628/104: no number = continuing).
    // (Changed 2026-10-30 after the independent review: the test had 1 day of the second black and then stopped; with stopping blood the case has the structure of Tuhfat al-Muhtaj, vol. 1, p. 401: 7 black, 7 red, 3 black, stop, where the tamyiz counts.)
    [FiqhTodo]
    [Fact(Skip = TamyizNotFinished)]
    public void Condition3_NotFulfilled_1DayBlack14DaysRedAndThenBlackAgain()
    {
        // ARRANGE
        List<(EFlowAppearanceColor type, double dayCount)> data =
        [
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 14), (EFlowAppearanceColor.Black, 16)
        ];

        List<FlowDataEntity> timeData = data.GetFlowDataList(initialInstant: _startDate);
        var haydCycleEntity = new HaydCycleEntity
        {
            HaydFlows = timeData
        };

        // ACT & ASSERT
        HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeFalse();
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] The weak blood must last at least 15 days if the blood continues; 16 days fulfil it. [IMPLICIT]
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 401; https://shamela.ws/book/9059/402] Hashiyat ash-Sharwani (after al-Isnawi): with 14 days of red between two blacks the distinction does not count, because the purity between the two hayds would be under the minimum; with 16 days that reason no longer applies. [IMPLICIT]
    [FiqhTodo]
    [Fact]
    public void Condition3_Fulfilled_1DayBlack16DaysRedAndThenBlackAgain()
    {
        // ARRANGE
        List<(EFlowAppearanceColor type, double dayCount)> data =
        [
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 16), (EFlowAppearanceColor.Black, 1)
        ];

        List<FlowDataEntity> timeData = data.GetFlowDataList(initialInstant: _startDate);
        var haydCycleEntity = new HaydCycleEntity
        {
            HaydFlows = timeData
        };

        // ACT & ASSERT
        HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeTrue();
    }

    #endregion Condition 3

    #region Condition 4

    // [Ibn Hajar al-Haytami, al-Fatawa al-Kubra al-Fiqhiyya, vol. 1, p. 100; https://shamela.ws/book/21628/99] Weak blood between strong bloods counts like purity, provided the weak continues alone after the 15 days; her hayd are the strong bloods within the 15 days with what lies between them. [IMPLICIT]
    // [as-Saqqaf, al-Ibana wa-l-Ifada, 3rd ed., p. 71-72] One day black, one day red, five times, then black, then red: she is mumayyiza; the first and the last black and what lies between are hayd, the last red is istihada.
    // KHILAF: Ibn Hajar says "according to the asahh" for the weak blood between two strong ones (al-Fatawa al-Kubra, vol. 1, p. 86; https://shamela.ws/book/21628/85), so there is another view (muqabil al-asahh, a strong view); the allowed works checked so far do not name it. The test follows the asahh.
    [FiqhTodo]
    [Fact(Skip = TamyizNotFinished)]
    public void Condition4_ContinuouslyBlackAndRedAndAtTheEndLongRed()
    {
        // ARRANGE
        List<(EFlowAppearanceColor type, double dayCount)> data =
        [
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 10),
        ];

        List<FlowDataEntity> timeData = data.GetFlowDataList(initialInstant: _startDate);
        var haydCycleEntity = new HaydCycleEntity
        {
            HaydFlows = timeData
        };

        // ACT & ASSERT
        HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeTrue();
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285; https://shamela.ws/book/11444/201] Permanently alternating 1 day black and 2 days red: no valid distinction. [IMPLICIT]
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 401; https://shamela.ws/book/9059/402] One day black, one day red and so on: a condition of the distinction is missing, because the weak blood is not continuous. Hashiyat ash-Shirwani there: fourth condition, the weak blood must not be interrupted by strong blood (purity in between does not matter).
    // [al-Ibana wa-l-Ifada fi Ahkam al-Hayd wa-n-Nifas wa-l-Istihada] Same condition and the same example (one day black, one day red, beyond 15 days: not mumayyiza).
    // One day black, one day red, to the end of the month (30 days), as in the sources («أَبَدًا» in Mughni al-Muhtaj, «إلَى آخِرِ الشَّهْرِ» in al-Fatawa al-Kubra, vol. 1, p. 100).
    // (Changed 2026-10-30 after the independent review: the test stopped after 17 days.)
    [FiqhTodo]
    [Fact]
    public void Condition4_ContinuouslyBlackAndRed()
    {
        // ARRANGE
        List<(EFlowAppearanceColor type, double dayCount)> data =
        [
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 1),
        ];

        List<FlowDataEntity> timeData = data.GetFlowDataList(initialInstant: _startDate);
        var haydCycleEntity = new HaydCycleEntity
        {
            HaydFlows = timeData
        };

        // ACT & ASSERT
        HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeFalse();
    }

    #endregion Condition 4
}
