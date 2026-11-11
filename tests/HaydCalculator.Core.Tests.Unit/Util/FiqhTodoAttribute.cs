using Xunit.Abstractions;
using Xunit.Sdk;

namespace HaydCalculator.Core.Tests.Unit.Util;

/// <summary>
/// Marks a test whose expected result still has to be verified from the fiqh point of view.
/// Shows up as the trait <c>Fiqh = ToDo</c>, so the Visual Studio Test Explorer can group and
/// filter by it (and <c>dotnet test --filter "Fiqh=ToDo"</c> runs only these tests).
/// Only the user removes it, once the expected result is checked.
/// </summary>
[TraitDiscoverer("HaydCalculator.Core.Tests.Unit.Util.FiqhTodoTraitDiscoverer", "HaydCalculator.Core.Tests.Unit")]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public sealed class FiqhTodoAttribute : Attribute, ITraitAttribute
{
}

public sealed class FiqhTodoTraitDiscoverer : ITraitDiscoverer
{
    public const string TraitName = "Fiqh";
    public const string TraitValue = "ToDo";

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        yield return new KeyValuePair<string, string>(TraitName, TraitValue);
    }
}
