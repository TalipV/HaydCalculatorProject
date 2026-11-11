using HaydCalculator.Core.Calculator.Models;

namespace HaydCalculator.Presentation.Views.Calendar;

/// <summary>
/// The colors of the calendar days and the input swatches, shared by the calendar, its legend
/// and the input list, so they always mean the same thing. Each color follows the current
/// light or dark theme; whoever uses them redraws when the theme changes.
/// </summary>
public static class DayColors
{
    public static bool IsDark => Application.Current?.RequestedTheme == AppTheme.Dark;

    public static Color Hayd => Color.FromArgb("#D32F2F");
    public static Color HaydNaqa => Pick("#EF9A9A", "#8E4A4A");
    public static Color Istihada => Pick("#212121", "#6E6E6E");
    public static Color Tuhr => Pick("#C8E6C9", "#2E5E33");
    public static Color NotEvaluated => Pick("#E0E0E0", "#3A3A3A");
    public static Color NoData => Pick("#FFFFFF", "#1E1E1E");

    public static Color Text => IsDark ? Colors.White : Colors.Black;
    public static Color OtherMonthText => Pick("#BDBDBD", "#5A5A5A");

    public static Color Pick(string light, string dark)
    {
        return Color.FromArgb(IsDark ? dark : light);
    }

    public static Color ForAppearance(EFlowAppearanceColor appearance)
    {
        return appearance switch
        {
            EFlowAppearanceColor.Black => Color.FromArgb("#212121"),
            EFlowAppearanceColor.Red => Color.FromArgb("#D32F2F"),
            EFlowAppearanceColor.Yellow => Color.FromArgb("#FBC02D"),
            _ => Colors.White,
        };
    }

    public static string NameOf(EFlowAppearanceColor appearance)
    {
        return appearance switch
        {
            EFlowAppearanceColor.Clear => "Clean",
            _ => appearance.ToString(),
        };
    }
}
