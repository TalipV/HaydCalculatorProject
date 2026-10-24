using PropertyChanged;
using XCalendar.Core.Models;
using XCalendar.Maui.Views;

namespace HaydCalculator.Presentation.Views.Calendar;

[AddINotifyPropertyChangedInterface]
public partial class CustomDay : CalendarDay
{
    public CustomDay()
    {
        MainStyle = CreateStyle(MainColor, MainTextColor);
    }

    // DayView takes its colors from MainStyle (bound to its Selected/CurrentMonth/OtherMonth styles),
    // so the style has to be rebuilt whenever a color changes; a Style cannot be changed once applied.
    public Color MainColor
    {
        get;
        set
        {
            field = value;
            MainStyle = CreateStyle(value, MainTextColor);
        }
    } = Colors.White;

    public Color MainTextColor
    {
        get;
        set
        {
            field = value;
            MainStyle = CreateStyle(MainColor, value);
        }
    } = Colors.Black;

    public Style MainStyle { get; set; }

    private static Style CreateStyle(Color backgroundColor, Color textColor)
    {
        return new Style(typeof(DayView))
        {
            Setters =
            {
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = backgroundColor },
                new Setter { Property = DayView.TextColorProperty, Value = textColor },
            },
        };
    }
}
