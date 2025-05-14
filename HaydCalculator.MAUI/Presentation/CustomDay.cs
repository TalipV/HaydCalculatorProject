using PropertyChanged;
using XCalendar.Core.Models;
using XCalendar.Maui.Views;

namespace MauiTestApp.Presentation
{
    [AddINotifyPropertyChangedInterface]
    public partial class CustomDay : CalendarDay
    {
        public Color MainColor { get; set; } = Colors.White;
        public Color MainTextColor { get; set; } = Colors.Black;
        public Style MainStyle { get; set; }

        public CustomDay()
        {
            MainStyle = new Style(typeof(DayView))
            {
                Setters = 
                {
                    new Setter()
                    {
                        Property = DayView.BackgroundColorProperty,
                        Value = MainColor
                    },                    
                    new Setter()
                    {
                        Property = DayView.TextColorProperty,
                        Value = MainTextColor
                    },
                }
            };
        }
    }

}
