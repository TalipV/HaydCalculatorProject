using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Presentation.Views.Calendar;

namespace HaydCalculator.Presentation.Pages.Main;

/// <summary>
/// An entry of the appearance picker; the picker shows <see cref="ToString"/>.
/// </summary>
public record FlowAppearanceOption(EFlowAppearanceColor Value)
{
    public override string ToString()
    {
        return DayColors.NameOf(Value);
    }
}
