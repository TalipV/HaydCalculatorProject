using System.Reflection;
using Microsoft.Extensions.Logging;

namespace HaydCalculator;

/// <summary>
/// Picked up by MethodTimer.Fody: every method marked with <c>[Time]</c> reports its duration here.
/// </summary>
public static class MethodTimeLogger
{
    public static ILogger? Logger { get; set; }

#pragma warning disable IDE0060 // Remove unused parameter
    public static void Log(MethodBase methodBase, TimeSpan timeSpan, string message)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        Logger?.LogInformation(
            "TIME-LOGGER: {DeclaringType}.{MethodName}, {Milliseconds} ms",
            methodBase.DeclaringType,
            methodBase.Name,
            timeSpan.TotalMilliseconds.ToString("N0"));
    }
}
