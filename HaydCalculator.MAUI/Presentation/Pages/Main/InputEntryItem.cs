namespace HaydCalculator.Presentation.Pages.Main;

/// <summary>
/// One row of the input list: the flow's appearance with its swatch, its length and its dates.
/// </summary>
public record InputEntryItem(Color SwatchColor, string Name, string DaysText, string RangeText);
