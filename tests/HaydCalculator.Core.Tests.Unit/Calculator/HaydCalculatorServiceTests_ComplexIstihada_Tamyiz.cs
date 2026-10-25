using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Tests.Unit.Util;

namespace HaydCalculator.Core.Tests.Unit.Calculator;

public class HaydCalculatorFactory_Tests_ComplexIstihada_TamyizDetermination
{
    private const string TamyizNotFinished = "Tamyiz determination (IsHaydFlowDataLstWithDistinguishableFlows) is not finished yet; this case still fails.";
    private readonly Instant _startDate = Instant.FromUtc(2024, 1, 1, 0, 0);

    #region Condition 3

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 286; https://shamela.ws/book/11444/202] 10 days black, then 10 days red, then it stops: she follows the distinction, although the weak blood stays under 15 days.
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

    [Fact(Skip = TamyizNotFinished)]
    public void Condition3_NotFulfilled_TenDaysRedTenDaysBlackAndStop()
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
        HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeFalse();
    }

    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 285f.; https://shamela.ws/book/11444/201] The condition that the weak blood lasts at least 15 days only applies if the blood continues. [IMPLICIT]
    [Fact]
    public void Condition3_Fulfilled_1DayBlackAnd14DaysRed()
    {
        // ARRANGE
        List<(EFlowAppearanceColor type, double dayCount)> data =
        [
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 14),
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
    [Fact(Skip = TamyizNotFinished)]
    public void Condition3_NotFulfilled_1DayBlack14DaysRedAndThenBlackAgain()
    {
        // ARRANGE
        List<(EFlowAppearanceColor type, double dayCount)> data =
        [
            (EFlowAppearanceColor.Black, 1), (EFlowAppearanceColor.Red, 14), (EFlowAppearanceColor.Black, 1)
        ];

        List<FlowDataEntity> timeData = data.GetFlowDataList(initialInstant: _startDate);
        var haydCycleEntity = new HaydCycleEntity
        {
            HaydFlows = timeData
        };

        // ACT & ASSERT
        HaydCycleEntity.IsHaydFlowDataLstWithDistinguishableFlows(haydCycleEntity.HaydFlows).Should().BeFalse();
    }

    #endregion Condition 3

    #region Condition 4

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
    // [al-Juwayni, Nihayat al-Matlab, vol. 1, p. 435; https://shamela.ws/book/9851/843] Permanently alternating 1 day black and 1 day light red: the pillars of the distinction are missing.
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 401; https://shamela.ws/book/9059/402] One day black, one day red and so on: a condition of the distinction is missing, because the weak blood is not continuous. Hashiyat ash-Shirwani there: fourth condition, the weak blood must not be interrupted by strong blood (purity in between does not matter).
    // [al-Ibana wa-l-Ifada fi Ahkam al-Hayd wa-n-Nifas wa-l-Istihada] Same condition and the same example (one day black, one day red, beyond 15 days: not mumayyiza).
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
            (EFlowAppearanceColor.Black, 1),
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
