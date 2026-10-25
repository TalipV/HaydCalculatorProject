using HaydCalculator.Core.Tests.Unit.Util;

namespace HaydCalculator.Core.Tests.Unit.Calculator;

/// <summary>
/// Cases whose Shafi'i mu'tamad result is not settled by the sources found so far. Each test
/// documents the open question and the conflicting sources and fails on purpose with
/// <see cref="OpenFiqhQuestionException"/> until the question is resolved and the test can state
/// the expected result.
/// </summary>
public class HaydCalculatorServiceTests_OpenQuestions
{
    public class OpenFiqhQuestionException(string question) : Exception($"Open fiqh question, to be resolved from the sources: {question}")
    {
    }

    // Case: 13 days red, 1 day purity, 5 days red, then the blood stops (19 days in total).
    //
    // Two rules from the sources collide:
    //
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 294; https://shamela.ws/book/11444/210]
    //   If the bloods together with the purity between them exceed 15 days, they are istihada.
    //   Read on its own, this makes the case a mustahada case (for a mubtada'a ghayr mumayyiza:
    //   only a day and a night of hayd).
    //
    // [al-Khatib ash-Shirbini, Mughni al-Muhtaj, vol. 1, p. 284; https://shamela.ws/book/11444/200]
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400]
    //   3 days blood, 12 days purity, 3 days blood, then it stops (18 days in total): the first 3 days
    //   are hayd and only the last 3 days are dam fasad. Applied here: the 13 days are hayd and the
    //   5 days (within the remaining minimal purity) are dam fasad.
    //
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 340 (Hashiyat ash-Shabramallisi); https://shamela.ws/book/3565/338]
    //   Notes that the general rule is restricted by the 3/12/3 case, but does not say where exactly
    //   the line between the two runs.
    //
    // To resolve: the passages on intermittent blood (talfiq) in Tuhfat al-Muhtaj, Nihayat al-Muhtaj, Mughni al-Muhtaj and their hawashi.
    [FiqhTodo]
    [Fact]
    public void ThirteenRedOnePurityFiveRed_DamFasadOrMustahada()
    {
        throw new OpenFiqhQuestionException(
            "13 days red, 1 day purity, 5 days red: are the last 5 days dam fasad (as in the 3/12/3 example), "
            + "or is it a mustahada case because blood and purity together exceed 15 days?");
    }

    // Case: blood that continues beyond the completion of the remaining minimal purity, e.g. 3 days red,
    // 14 days purity, 4 days red, then it stops. The existing tests
    // HaydCalculation_CalculationWithPurityBetweenTwoSeparateMenstrualCyclesIncludingIstihada (3/14/4)
    // and HaydCalculation_CalculationWithPartiallyIstihadaAndPartiallyMenstrualFlow (10/8/12) expect:
    // the part up to the completion of the minimal purity is dam fasad, the rest is a new hayd.
    // Their only sources are the lecture videos.
    //
    // [ar-Ramli, Nihayat al-Muhtaj, vol. 1, p. 340 (Hashiyat ash-Shabramallisi, quoting Ibn Qasim); https://shamela.ws/book/3565/338]
    //   Asks whether, if the blood after the purity lasts e.g. 6 days, the part beyond the completion of
    //   the purity is hayd, and only answers "it is not far-fetched" (la yab'ud), i.e. tentatively.
    //   He also notes that Ibn Hajar's wording suggests a difference between mubtada'a and mu'tada.
    //
    // [Ibn Hajar al-Haytami, Tuhfat al-Muhtaj, vol. 1, p. 399; https://shamela.ws/book/9059/400]
    //   For a mu'tada (5 days 'ada, 14 days purity, then continuous blood) the first day and night of the
    //   returning blood complete the purity and the following 5 days are hayd. That case is continuous
    //   blood (mustahada), not blood that stops.
    //
    // To resolve: an explicit text for blood that stops after it has passed the completion of the
    // minimal purity, for mubtada'a and mu'tada.
    [FiqhTodo]
    [Fact]
    public void BloodContinuingPastCompletionOfMinimalPurityThenStopping_IsTheRestHayd()
    {
        throw new OpenFiqhQuestionException(
            "3 days red, 14 days purity, 4 days red: is the part after the completion of the minimal purity a new hayd "
            + "(as the lecture videos say), and does the answer differ between mubtada'a and mu'tada?");
    }
}
