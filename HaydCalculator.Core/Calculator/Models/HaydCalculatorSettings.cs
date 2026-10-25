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
