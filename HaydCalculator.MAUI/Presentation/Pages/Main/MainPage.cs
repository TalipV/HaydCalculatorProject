using CommunityToolkit.Maui.Markup;
using HaydCalculator.Presentation.Views.Calendar;
using XCalendar.Core.Models;
using XCalendar.Maui.Views;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace HaydCalculator.Presentation.Pages.Main;

public class MainPage : ContentPage
{
    private enum Row
    {
        Input,
        AddButton,
        Spacer,
        InputList,
        TopSeparator,
        Calendar,
        CalendarMiddle,
        CalendarBottom,
        BelowCalendar,
        BottomSeparator,
        LogHeader,
        Log,
    }

    private enum Column
    {
        Left,
        LeftMiddle,
        Gap,
        RightMiddle,
        Right,
    }

    public MainPage(MainPageViewModel viewModel)
    {
        BindingContext = viewModel;
        Title = "";

        Content = new ScrollView
        {
            Content = new Grid
            {
                RowDefinitions = Rows.Define(
                    (Row.Input, 40),
                    (Row.AddButton, 50),
                    (Row.Spacer, 30),
                    (Row.InputList, 120),
                    (Row.TopSeparator, 5),
                    (Row.Calendar, 25),
                    (Row.CalendarMiddle, 180),
                    (Row.CalendarBottom, 25),
                    (Row.BelowCalendar, 100),
                    (Row.BottomSeparator, 5),
                    (Row.LogHeader, 25),
                    (Row.Log, 170)),
                ColumnDefinitions = Columns.Define(
                    (Column.Left, Star),
                    (Column.LeftMiddle, Star),
                    (Column.Gap, 10),
                    (Column.RightMiddle, Star),
                    (Column.Right, Star)),
                Children =
                {
                    CreateDayCountInput()
                        .Row(Row.Input).Column(Column.LeftMiddle),

                    new Picker
                    {
                        MinimumWidthRequest = 45,
                        BackgroundColor = Colors.AntiqueWhite,
                        HorizontalTextAlignment = TextAlignment.Center,
                    }
                        .Start().CenterVertical()
                        .Bind(Picker.ItemsSourceProperty, nameof(MainPageViewModel.FlowAppearances))
                        .Bind(Picker.SelectedItemProperty, nameof(MainPageViewModel.SelectedFlowAppearance))
                        .Row(Row.Input).Column(Column.RightMiddle),

                    new Button { Text = "Add" }
                        .Margin(5).CenterHorizontal()
                        .BindCommand(nameof(MainPageViewModel.AddDataCommand))
                        .Row(Row.AddButton).Column(Column.LeftMiddle),

                    new CollectionView
                    {
                        BackgroundColor = Colors.AntiqueWhite,
                        ItemTemplate = new DataTemplate(() =>
                            new Label { TextColor = Colors.Blue, HeightRequest = 20 }.Bind(Label.TextProperty, ".")),
                    }
                        .Margin(10)
                        .Bind(ItemsView.ItemsSourceProperty, nameof(MainPageViewModel.InputData))
                        .Row(Row.InputList).Column(Column.Left).ColumnSpan(4),

                    new Button { Text = "Clear", HeightRequest = 40 }
                        .Margin(10)
                        .BindCommand(nameof(MainPageViewModel.ClearDataCommand))
                        .Row(Row.InputList).Column(Column.Right),

                    new BoxView { Color = Colors.BlueViolet }
                        .Row(Row.TopSeparator).ColumnSpan(5),

                    CreateCalendarView()
                        .Row(Row.Calendar).RowSpan(3).ColumnSpan(5),

                    new BoxView { Color = Colors.BlueViolet }
                        .Row(Row.BottomSeparator).ColumnSpan(5),

                    new Label { Text = "Log" }
                        .Padding(3).Font(bold: true)
                        .Row(Row.LogHeader),

                    new Label { Text = "Make up days: " }
                        .Padding(3)
                        .Bind(Label.TextProperty, nameof(MainPageViewModel.MakeUpDayCounterText))
                        .Row(Row.LogHeader).Column(Column.RightMiddle).ColumnSpan(3),

                    new Editor { BackgroundColor = Colors.AntiqueWhite, IsReadOnly = true }
                        .Margin(5)
                        .Bind(Editor.TextProperty, nameof(MainPageViewModel.FeedbackText))
                        .Row(Row.Log).ColumnSpan(5),
                },
            },
        };
    }

    private static Grid CreateDayCountInput()
    {
        return new Grid
        {
            ColumnDefinitions = Columns.Define(Auto, Star),
            Children =
            {
                new Label { Text = "Days:" }
                    .CenterVertical().Padding(5)
                    .Column(0),

                new Entry
                {
                    MaxLength = 5,
                    MinimumWidthRequest = 35,
                    BackgroundColor = Colors.AntiqueWhite,
                    HorizontalTextAlignment = TextAlignment.Center,
                }
                    .Center()
                    .Bind(Entry.TextProperty, nameof(MainPageViewModel.DayCountEntryText), BindingMode.TwoWay)
                    .Column(1),
            },
        };
    }

    private static CalendarView CreateCalendarView()
    {
        var calendarView = new CalendarView
        {
            BackgroundColor = Colors.White,
            DayNamesHeightRequest = 0,
            DaysViewHeightRequest = 500,
            IsVisible = true,
            NavigationViewTemplate = new ControlTemplate(() => new NavigationView { HeightRequest = 0 }),
            DayTemplate = new DataTemplate(CreateDayView),
        };

        return calendarView
            .Bind(CalendarView.DaysProperty, $"{nameof(MainPageViewModel.Calendar)}.{nameof(Calendar<CustomDay>.Days)}")
            .Bind(CalendarView.DaysOfWeekProperty, $"{nameof(MainPageViewModel.Calendar)}.{nameof(Calendar<CustomDay>.DayNamesOrder)}");
    }

    private static Border CreateDayView()
    {
        return new Border
        {
            Padding = 0,
            Content = new DayView
            {
                HeightRequest = 35,
                WidthRequest = 35,
            }
                .Margin(1).Center()
                .Bind(BackgroundColorProperty, nameof(CustomDay.MainColor))
                .Bind(DayView.TextColorProperty, nameof(CustomDay.MainTextColor))
                .Bind(DayView.DateTimeProperty, nameof(CustomDay.DateTime))
                .Bind(DayView.SelectedStyleProperty, nameof(CustomDay.MainStyle))
                .Bind(DayView.CurrentMonthStyleProperty, nameof(CustomDay.MainStyle))
                .Bind(DayView.OtherMonthStyleProperty, nameof(CustomDay.MainStyle)),
        }
            .Margin(3).Center()
            .Bind(BackgroundColorProperty, nameof(CustomDay.MainColor));
    }
}
