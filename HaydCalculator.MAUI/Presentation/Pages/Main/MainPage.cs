using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using HaydCalculator.Presentation.Views.Calendar;
using XCalendar.Core.Models;
using XCalendar.Maui.Views;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace HaydCalculator.Presentation.Pages.Main;

public class MainPage : ContentPage
{
    private const double DayCellSize = 40;
    private const double DayCellMargin = 2;
    private const int CalendarRows = 7;

    public MainPage(MainPageViewModel viewModel)
    {
        BindingContext = viewModel;
        Title = "";
        BackgroundColor = Colors.White;

        // .NET 10 lays pages out edge to edge by default; keep the content clear of the
        // status bar (and the navigation bar at the bottom) instead of drawing under it.
        SafeAreaEdges = new(SafeAreaRegions.Container);
        Shell.SetNavBarIsVisible(this, false);

        // dark status bar icons, so they stay visible on the white page
        Behaviors.Add(new StatusBarBehavior
        {
            StatusBarColor = Colors.White,
            StatusBarStyle = StatusBarStyle.DarkContent,
        });

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(12, 8),
                Spacing = 8,
                Children =
                {
                    CreateInputRow(),

                    new CollectionView
                    {
                        BackgroundColor = Colors.AntiqueWhite,
                        HeightRequest = 120,
                        ItemTemplate = new DataTemplate(() =>
                            new Label { TextColor = Colors.Blue }
                                .Paddings(8, 2, 8, 2)
                                .Bind(Label.TextProperty, ".")),
                    }
                        .Bind(CollectionView.ItemsSourceProperty, nameof(MainPageViewModel.InputData)),

                    CreateSeparator(),

                    CreateCalendarView(),

                    CreateSeparator(),

                    new Grid
                    {
                        ColumnDefinitions = Columns.Define(Auto, Star),
                        Children =
                        {
                            new Label { Text = "Log" }
                                .Font(bold: true).CenterVertical()
                                .Column(0),

                            new Label { HorizontalTextAlignment = TextAlignment.End }
                                .CenterVertical()
                                .Bind(Label.TextProperty, nameof(MainPageViewModel.MakeUpDayCounterText))
                                .Column(1),
                        },
                    },

                    new Editor { BackgroundColor = Colors.AntiqueWhite, IsReadOnly = true, HeightRequest = 170 }
                        .Bind(Editor.TextProperty, nameof(MainPageViewModel.FeedbackText)),
                },
            },
        };
    }

    private static Grid CreateInputRow()
    {
        return new Grid
        {
            ColumnDefinitions = Columns.Define(Auto, Star, Star, Auto, Auto),
            ColumnSpacing = 8,
            Children =
            {
                new Label { Text = "Days:" }
                    .CenterVertical()
                    .Column(0),

                new Entry
                {
                    MaxLength = 5,
                    Keyboard = Keyboard.Numeric,
                    BackgroundColor = Colors.AntiqueWhite,
                    HorizontalTextAlignment = TextAlignment.Center,
                }
                    .Bind(Entry.TextProperty, nameof(MainPageViewModel.DayCountEntryText), BindingMode.TwoWay)
                    .Column(1),

                new Picker
                {
                    BackgroundColor = Colors.AntiqueWhite,
                    HorizontalTextAlignment = TextAlignment.Center,
                }
                    .Bind(Picker.ItemsSourceProperty, nameof(MainPageViewModel.FlowAppearances))
                    .Bind(Picker.SelectedItemProperty, nameof(MainPageViewModel.SelectedFlowAppearance))
                    .Column(2),

                new Button { Text = "Add" }
                    .BindCommand(nameof(MainPageViewModel.AddDataCommand))
                    .Column(3),

                new Button { Text = "Clear" }
                    .BindCommand(nameof(MainPageViewModel.ClearDataCommand))
                    .Column(4),
            },
        };
    }

    private static BoxView CreateSeparator()
    {
        return new BoxView { Color = Colors.BlueViolet, HeightRequest = 3 };
    }

    private static CalendarView CreateCalendarView()
    {
        var calendarView = new CalendarView
        {
            BackgroundColor = Colors.White,
            DayNamesHeightRequest = 0,
            // exactly the space of the day rows, so the calendar neither overlaps nor leaves a gap
            DaysViewHeightRequest = CalendarRows * (DayCellSize + (2 * DayCellMargin)),
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
            StrokeThickness = 0,
            Content = new DayView
            {
                HeightRequest = DayCellSize,
                WidthRequest = DayCellSize,
            }
                .Center()
                .Bind(BackgroundColorProperty, nameof(CustomDay.MainColor))
                .Bind(DayView.TextColorProperty, nameof(CustomDay.MainTextColor))
                .Bind(DayView.DateTimeProperty, nameof(CustomDay.DateTime))
                .Bind(DayView.SelectedStyleProperty, nameof(CustomDay.MainStyle))
                .Bind(DayView.CurrentMonthStyleProperty, nameof(CustomDay.MainStyle))
                .Bind(DayView.OtherMonthStyleProperty, nameof(CustomDay.MainStyle)),
        }
            .Margin(DayCellMargin).Center()
            .Bind(BackgroundColorProperty, nameof(CustomDay.MainColor));
    }
}
