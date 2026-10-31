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
}
