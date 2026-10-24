namespace HaydCalculator.Core.Misc;

/// <summary>
/// Expected outcome the user should see as a plain message: invalid input or a case the
/// calculator does not support yet. Unexpected states inside the calculation throw
/// <see cref="HaydCalculationStateException"/> instead.
/// </summary>
public class InfoException(string message) : Exception(message)
{
}
