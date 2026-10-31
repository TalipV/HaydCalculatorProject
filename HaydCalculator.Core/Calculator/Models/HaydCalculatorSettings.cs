namespace HaydCalculator.Core.Calculator.Models;

/// <summary>
/// Which view the calculation follows where the Shafi'i school has a khilaf. Only the mu'tamad
/// authorities (Ibn Hajar al-Haytami, ar-Ramli, al-Khatib ash-Shirbini) and views that are at least
/// strong inside the school are offered; the default is the asahh / azhar and, where the mu'tamad
/// authorities differ, ar-Ramli and al-Khatib ash-Shirbini.
/// Sources for every point are in the fiqh tests (KHILAF lines).
/// </summary>
public sealed record HaydCalculatorSettings
{
    public static HaydCalculatorSettings Default { get; } = new();

    /// <summary>
    /// Tuhfat al-Muhtaj where it differs from Nihayat al-Muhtaj and Mughni al-Muhtaj.
    /// </summary>
    public static HaydCalculatorSettings IbnHajar { get; } = new()
    {
        WeakBetweenStrongBeyondMaximum = EWeakBetweenStrongBeyondMaximum.OnlyFirstStrongIsHayd,
        StrongerWeakMustFollowStrongDirectly = false,
        YellowNotHaydScope = EYellowNotHaydScope.InsideAndOutsideAda,
    };

    /// <summary>
    /// Nihayat al-Muhtaj (same as <see cref="Default"/>).
    /// </summary>
    public static HaydCalculatorSettings Ramli => Default;

    /// <summary>
    /// Mughni al-Muhtaj (same as <see cref="Default"/>; no difference to ar-Ramli in the covered points).
    /// </summary>
    public static HaydCalculatorSettings Shirbini => Default;

    /// <summary>
    /// Purity between bloods that together reach the minimum (within 15 days): counts as hayd (azhar) or as tuhr.
    /// </summary>
    public EPurityBetweenBloods PurityBetweenBloods { get; init; } = EPurityBetweenBloods.Hayd;

    /// <summary>
    /// Yellow and turbid blood are hayd (asahh) or not.
    /// </summary>
    public bool YellowAndTurbidAreHayd { get; init; } = true;

    /// <summary>
    /// Only used when <see cref="YellowAndTurbidAreHayd"/> is false: whether yellow and turbid blood inside the days
    /// of her 'ada are hayd anyway (al-Khatib after ar-Rawda) or not hayd either (Ibn Hajar).
    /// </summary>
    public EYellowNotHaydScope YellowNotHaydScope { get; init; } = EYellowNotHaydScope.OnlyOutsideAda;

    /// <summary>
    /// Mubtada'a whose returning blood falls into the rest of the minimal purity and runs past its completion
    /// (e.g. 3 days blood, 14 days purity, then blood): the first blood is hayd and the returning blood completes
    /// the purity (Bughyat al-Mustarshidin, al-Ibana, Ibn Qasim) or she is a mustahada from the first blood on
    /// (ash-Shabramallisi and al-Bujayrimi reading Tuhfat al-Muhtaj 1/399).
    /// </summary>
    public EMubtadaaBloodInRestOfTuhr MubtadaaBloodInRestOfTuhr { get; init; } = EMubtadaaBloodInRestOfTuhr.CompletesTuhrThenNewHayd;

    /// <summary>
    /// Hayd of a mubtada'a ghayr mumayyiza: a day and a night (azhar) or the usual 6 or 7 days.
    /// </summary>
    public EMubtadaaGhayrMumayyizaHayd MubtadaaGhayrMumayyizaHayd { get; init; } = EMubtadaaGhayrMumayyizaHayd.DayAndNight;

    /// <summary>
    /// How many occurrences establish an 'ada: one (asahh), two or three.
    /// </summary>
    public int OccurrencesToEstablishAda { get; init; } = 1;

    /// <summary>
    /// A mu'tada mumayyiza follows her distinction (asahh) or her 'ada.
    /// </summary>
    public EMutadaMumayyizaFollows MutadaMumayyizaFollows { get; init; } = EMutadaMumayyizaFollows.Tamyiz;

    /// <summary>
    /// Strong, weak, strong beyond 15 days (e.g. 7/7/7): the first strong with the weak (Ibn Surayj, ar-Ramli, al-Khatib) or only the first strong (Ibn Hajar).
    /// </summary>
    public EWeakBetweenStrongBeyondMaximum WeakBetweenStrongBeyondMaximum { get; init; } = EWeakBetweenStrongBeyondMaximum.FirstStrongWithWeakIsHayd;

    /// <summary>
    /// Strong, weaker, weak (e.g. black, yellow, red): the stronger weak blood must follow the strong directly to be joined to it (ar-Ramli, al-Khatib) or not (Ibn Hajar).
    /// </summary>
    public bool StrongerWeakMustFollowStrongDirectly { get; init; } = true;

    /// <summary>
    /// Mu'tada whose blood returns before her usual time and then continues (shifted cycle, e.g. 'ada of 5,
    /// then 14 or 20 days of purity, then continuous blood): the four awjuh reported in al-Fatawa al-Kubra 1/78 and 1/91.
    /// </summary>
    public EShiftedCycle ShiftedCycle { get; init; } = EShiftedCycle.NewCycle;
}

public enum EShiftedCycle
{
    /// <summary>
    /// the earlier blood starts a new cycle: her 'ada of hayd from the start of the possible hayd, then the purity of the new cycle (asahh)
    /// </summary>
    NewCycle,
    /// <summary>
    /// the returning blood is hayd up to the end of her old hayd in the next month, then she keeps her old cycle
    /// (14 days of purity: 10 + 5 days; 20 days of purity: 10 days, then 25 days purity)
    /// </summary>
    HaydExtendedThenOldCycle,
    /// <summary>
    /// her 'ada of hayd from the start of the possible hayd, then her old purity (here 25 days), always
    /// </summary>
    AdaThenOldPurity,
    /// <summary>
    /// the returning blood up to the end of the month is istihada, her old cycle continues from the next month
    /// </summary>
    OldCycleOnly,
}

public enum EYellowNotHaydScope
{
    /// <summary>
    /// yellow and turbid blood are not hayd only outside the days of her 'ada; inside them they are hayd (al-Khatib after ar-Rawda)
    /// </summary>
    OnlyOutsideAda,
    /// <summary>
    /// yellow and turbid blood are not hayd, also inside the days of her 'ada (Ibn Hajar)
    /// </summary>
    InsideAndOutsideAda,
}

public enum EMubtadaaBloodInRestOfTuhr
{
    /// <summary>
    /// the first blood is hayd, the returning blood completes the minimal purity (dam fasad) and the rest is a new hayd
    /// or, if it exceeds 15 days, a mustahada case of its own (Bughyat al-Mustarshidin, al-Ibana, Ibn Qasim)
    /// </summary>
    CompletesTuhrThenNewHayd,
    /// <summary>
    /// the purity counts like weak blood under the minimal purity, so she is a mustahada from the first blood on,
    /// with a day and a night of hayd from the start of the month (ash-Shabramallisi, al-Bujayrimi)
    /// </summary>
    MustahadaFromFirstBlood,
}

public enum EPurityBetweenBloods
{
    /// <summary>
    /// the purity counts as hayd (azhar; called qawl as-sahb in the sources)
    /// </summary>
    Hayd,
    /// <summary>
    /// the purity counts as tuhr (called qawl al-laqt or at-talfiq in the sources)
    /// </summary>
    Tuhr,
}

public enum EMubtadaaGhayrMumayyizaHayd
{
    /// <summary>
    /// a day and a night of hayd, 29 days of purity (azhar)
    /// </summary>
    DayAndNight,
    /// <summary>
    /// the usual hayd of 6 or 7 days, the rest of the month purity
    /// </summary>
    UsualSixOrSevenDays,
}

public enum EMutadaMumayyizaFollows
{
    /// <summary>
    /// the distinction decides (asahh)
    /// </summary>
    Tamyiz,
    /// <summary>
    /// the 'ada decides
    /// </summary>
    Ada,
}

public enum EWeakBetweenStrongBeyondMaximum
{
    /// <summary>
    /// the first strong blood together with the weak one is hayd (Ibn Surayj, ar-Ramli, al-Khatib)
    /// </summary>
    FirstStrongWithWeakIsHayd,
    /// <summary>
    /// only the first strong blood is hayd (Ibn Hajar)
    /// </summary>
    OnlyFirstStrongIsHayd,
}
