using XCalendar.Core.Enums;
using XCalendar.Core.Models;

namespace HaydCalculator
{
    public class MainPageViewModel
    {
        public Calendar<CustomDay> Calendar { get; set; } = new Calendar<CustomDay>()
        {
            NavigatedDate = new DateTime(2023, 1, 1),
            NavigationLowerBound = DateTime.Today.AddYears(-2),
            NavigationUpperBound = DateTime.Today.AddYears(2),
            StartOfWeek = DayOfWeek.Sunday,
            SelectionAction = SelectionAction.Modify,
            NavigationLoopMode = NavigationLoopMode.LoopMinimumAndMaximum,
            SelectionType = SelectionType.Single,
            PageStartMode = PageStartMode.FirstDayOfYear,
            AutoRows = false,
            AutoRowsIsConsistent = true,
            Rows = 7,
            TodayDate = DateTime.Today,
        };
    }
}
