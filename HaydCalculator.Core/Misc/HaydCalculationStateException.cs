namespace HaydCalculator.Core.Misc;

/// <summary>
/// The calculation reached a state that its own rules should have made impossible,
/// i.e. a bug in the calculator rather than a problem with the user's input.
/// </summary>
public class HaydCalculationStateException(string message) : InvalidOperationException(message)
{
}
