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

    // No open question is covered by a test at the moment; see docs/fiqh/open-questions.md.
}
