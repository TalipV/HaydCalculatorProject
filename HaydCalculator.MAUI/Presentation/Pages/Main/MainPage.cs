using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using HaydCalculator.Presentation.Views.Calendar;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using XCalendar.Core.Models;
using XCalendar.Maui.Views;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace HaydCalculator.Presentation.Pages.Main;

public class MainPage : ContentPage
{
    private const double DayCellSize = 40;
    private const double DayCellMargin = 2;
    private const double DayNamesHeight = 24;
    private const int CalendarRows = 7;

    private readonly MainPageViewModel _viewModel;
    private readonly StatusBarBehavior _statusBarBehavior = new();

    public MainPage(MainPageViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = viewModel;
        Title = "";

        // .NET 10 lays pages out edge to edge by default; keep the content clear of the
        // status bar (and the navigation bar at the bottom) instead of drawing under it.
        SafeAreaEdges = new(SafeAreaRegions.Container);
        Shell.SetNavBarIsVisible(this, false);
        Behaviors.Add(_statusBarBehavior);

        ApplyTheme();

        // the colors below are picked once per build of the page, so a theme switch rebuilds it
        if (Application.Current is Application application)
        {
            application.RequestedThemeChanged += (_, _) => ApplyTheme();
        }
    }

    private static Color PageBackground => DayColors.Pick("#F5F5F7", "#121212");
    private static Color CardBackground => DayColors.Pick("#FFFFFF", "#1E1E1E");
    private static Color CardStroke => DayColors.Pick("#E0E0E0", "#333333");
    private static Color SecondaryText => DayColors.Pick("#757575", "#A0A0A0");
    private static Color Accent => Color.FromArgb("#512BD4");
    private static Color AccentText => DayColors.Pick("#512BD4", "#B39DFF");
    private static Color InfoBackground => DayColors.Pick("#FFF8E1", "#3A2E00");
    private static Color InfoStroke => Color.FromArgb("#FFB300");
    private static Color ErrorBackground => DayColors.Pick("#FFEBEE", "#3B1416");
    private static Color ErrorStroke => DayColors.Pick("#E53935", "#FF6B6B");

    private void ApplyTheme()
    {
        BackgroundColor = PageBackground;

        // status bar icons that stay visible on the page background
        _statusBarBehavior.StatusBarColor = PageBackground;
        _statusBarBehavior.StatusBarStyle = DayColors.IsDark ? StatusBarStyle.LightContent : StatusBarStyle.DarkContent;

        Content = CreateContent();
    }

    private ScrollView CreateContent()
    {
        return new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(12, 8),
                Spacing = 10,
                Children =
                {
                    CreateCard(CreateInputRow()),
                    CreateCard(CreateEntriesSection()),
                    CreateFeedbackBanner(),
                    CreateCard(CreateResultSection())
                        .Bind(IsVisibleProperty, nameof(MainPageViewModel.HasResult)),
                    CreateCard(CreateCalendarSection()),
                },
            },
        };
    }

    private static Border CreateCard(View content)
    {
        return new Border
        {
            BackgroundColor = CardBackground,
            Stroke = CardStroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Padding = 12,
            Content = content,
        };
    }

    private static Label CreateSectionTitle(string text)
    {
        return new Label { Text = text, FontSize = 15 }.Font(bold: true).CenterVertical();
    }

    private static Grid CreateInputRow()
    {
        return new Grid
        {
            ColumnDefinitions = Columns.Define(Auto, 70, Star, Auto),
            ColumnSpacing = 8,
            Children =
            {
                new Label { Text = "Days" }
                    .CenterVertical()
                    .Column(0),

                new Entry
                {
                    MaxLength = 5,
                    Keyboard = Keyboard.Numeric,
                    HorizontalTextAlignment = TextAlignment.Center,
                }
                    .Bind(Entry.TextProperty, nameof(MainPageViewModel.DayCountEntryText), BindingMode.TwoWay)
                    .Column(1),

                new Picker
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                }
                    .Bind(Picker.ItemsSourceProperty, nameof(MainPageViewModel.FlowAppearances))
                    .Bind(Picker.SelectedItemProperty, nameof(MainPageViewModel.SelectedFlowAppearance), BindingMode.TwoWay)
                    .Column(2),

                new Button { Text = "Add", BackgroundColor = Accent, TextColor = Colors.White }
                    .BindCommand(nameof(MainPageViewModel.AddDataCommand))
                    .Column(3),
            },
        };
    }

    private View CreateEntriesSection()
    {
        var clearButton = new Button
        {
            Text = "Clear all",
            BackgroundColor = Colors.Transparent,
            TextColor = ErrorStroke,
            Padding = new Thickness(8, 0),
        };
        clearButton.Clicked += OnClearClicked;

        return new VerticalStackLayout
        {
            Spacing = 6,
            Children =
            {
                new Grid
                {
                    ColumnDefinitions = Columns.Define(Star, Auto, Auto),
                    Children =
                    {
                        CreateSectionTitle("Entries").Column(0),

                        new Button
                        {
                            Text = "Undo last",
                            BackgroundColor = Colors.Transparent,
                            TextColor = AccentText,
                            Padding = new Thickness(8, 0),
                        }
                            .BindCommand(nameof(MainPageViewModel.UndoLastCommand))
                            .Bind(IsEnabledProperty, nameof(MainPageViewModel.HasInput))
                            .Column(1),

                        clearButton
                            .Bind(IsEnabledProperty, nameof(MainPageViewModel.HasInput))
                            .Column(2),
                    },
                },

                new CollectionView
                {
                    HeightRequest = 130,
                    ItemTemplate = new DataTemplate(CreateEntryRow),
                    EmptyView = new Label
                    {
                        Text = "No entries yet. Add the days of each blood color (or clean days) in order.",
                        TextColor = SecondaryText,
                        FontSize = 13,
                    }.Paddings(4, 8, 4, 8),
                }
                    .Bind(CollectionView.ItemsSourceProperty, nameof(MainPageViewModel.InputEntries)),
            },
        };
    }

    private static Grid CreateEntryRow()
    {
        return new Grid
        {
            ColumnDefinitions = Columns.Define(Auto, Star, Auto),
            ColumnSpacing = 10,
            Padding = new Thickness(2, 4),
            Children =
            {
                CreateSwatch()
                    .Bind(BackgroundColorProperty, nameof(InputEntryItem.SwatchColor))
                    .Column(0),

                new Label().CenterVertical()
                    .Bind(Label.TextProperty, nameof(InputEntryItem.Name))
                    .Column(1),

                new HorizontalStackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        new Label { TextColor = SecondaryText, FontSize = 13 }.CenterVertical()
                            .Bind(Label.TextProperty, nameof(InputEntryItem.RangeText)),
                        new Label { WidthRequest = 60, HorizontalTextAlignment = TextAlignment.End }
                            .Font(bold: true).CenterVertical()
                            .Bind(Label.TextProperty, nameof(InputEntryItem.DaysText)),
                    },
                }
                    .Column(2),
            },
        };
    }

    private static Border CreateSwatch(Color? color = null)
    {
        return new Border
        {
            WidthRequest = 16,
            HeightRequest = 16,
            BackgroundColor = color ?? Colors.White,
            Stroke = CardStroke,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 4 },
        }.CenterVertical();
    }

    private async void OnClearClicked(object? sender, EventArgs e)
    {
        if (await DisplayAlertAsync("Clear all entries?", "All entries and the result are removed.", "Clear", "Cancel"))
        {
            _viewModel.ClearDataCommand.Execute(null);
        }
    }

    private static Border CreateFeedbackBanner()
    {
        return new Border
        {
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Padding = 12,
            Content = new Grid
            {
                ColumnDefinitions = Columns.Define(Auto, Star),
                RowDefinitions = Rows.Define(Auto, Auto),
                ColumnSpacing = 10,
                Children =
                {
                    new Label { FontSize = 18 }
                        .Bind(Label.TextProperty, nameof(MainPageViewModel.IsFeedbackError), convert: (bool isError) => isError ? "⛔" : "⚠")
                        .Row(0).Column(0),

                    new Label().Font(bold: true).CenterVertical()
                        .Bind(Label.TextProperty, nameof(MainPageViewModel.FeedbackText))
                        .Row(0).Column(1),

                    new Label { FontSize = 11, TextColor = SecondaryText, MaxLines = 12 }
                        .Bind(Label.TextProperty, nameof(MainPageViewModel.FeedbackDetailsText))
                        .Bind(IsVisibleProperty, nameof(MainPageViewModel.HasFeedbackDetails))
                        .Row(1).Column(1),
                },
            },
        }
            .Bind(BackgroundColorProperty, nameof(MainPageViewModel.IsFeedbackError), convert: (bool isError) => isError ? ErrorBackground : InfoBackground)
            .Bind(Border.StrokeProperty, nameof(MainPageViewModel.IsFeedbackError), convert: (bool isError) => (Brush)(isError ? ErrorStroke : InfoStroke))
            .Bind(IsVisibleProperty, nameof(MainPageViewModel.HasFeedback));
    }

    private static View CreateResultSection()
    {
        return new VerticalStackLayout
        {
            Spacing = 10,
            Children =
            {
                CreateSectionTitle("Result"),

                new Grid
                {
                    ColumnDefinitions = Columns.Define(Star, Star, Star),
                    ColumnSpacing = 8,
                    Children =
                    {
                        CreateStatTile("Hayd", DayColors.Hayd, nameof(MainPageViewModel.HaydDaysText)).Column(0),
                        CreateStatTile("Istihada", DayColors.Istihada, nameof(MainPageViewModel.IstihadaDaysText)).Column(1),
                        CreateStatTile("Make up", AccentText, nameof(MainPageViewModel.MakeUpDaysText)).Column(2),
                    },
                },

                new Label { FontSize = 13, TextColor = SecondaryText }
                    .Bind(Label.TextProperty, nameof(MainPageViewModel.HaydPeriodsText)),
            },
        };
    }

    private static Border CreateStatTile(string caption, Color accent, string valuePath)
    {
        return new Border
        {
            BackgroundColor = PageBackground,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Padding = new Thickness(8, 10),
            Content = new VerticalStackLayout
            {
                Spacing = 2,
                Children =
                {
                    new Label { FontSize = 18, TextColor = accent, HorizontalTextAlignment = TextAlignment.Center }
                        .Font(bold: true)
                        .Bind(Label.TextProperty, valuePath),
                    new Label { Text = caption, FontSize = 12, TextColor = SecondaryText, HorizontalTextAlignment = TextAlignment.Center },
                },
            },
        };
    }

    private static View CreateCalendarSection()
    {
        return new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                CreateSectionTitle("")
                    .Bind(Label.TextProperty, nameof(MainPageViewModel.CalendarTitleText)),
                CreateLegend(),
                CreateDayNamesRow(),
                CreateCalendarView(),
            },
        };
    }

    private static FlexLayout CreateLegend()
    {
        var legend = new FlexLayout { Wrap = FlexWrap.Wrap };

        foreach ((string text, Color color) in new[]
        {
            ("Hayd", DayColors.Hayd),
            ("Clean within hayd", DayColors.HaydNaqa),
            ("Istihada", DayColors.Istihada),
            ("Tuhr", DayColors.Tuhr),
            ("Not evaluated", DayColors.NotEvaluated),
        })
        {
            legend.Children.Add(new HorizontalStackLayout
            {
                Spacing = 4,
                Margin = new Thickness(0, 2, 12, 2),
                Children =
                {
                    CreateSwatch(color),
                    new Label { Text = text, FontSize = 12, TextColor = SecondaryText }.CenterVertical(),
                },
            });
        }

        return legend;
    }

    private static CalendarView CreateCalendarView()
    {
        var calendarView = new CalendarView
        {
            BackgroundColor = CardBackground,
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

    /// <summary>
    /// The weekday header above the calendar, starting on Sunday like the calendar. XCalendar's own
    /// header follows the system culture and overlaps the first row of days, so it is hidden.
    /// </summary>
    private static Grid CreateDayNamesRow()
    {
        var row = new Grid
        {
            ColumnDefinitions = Columns.Define(Star, Star, Star, Star, Star, Star, Star),
            HeightRequest = DayNamesHeight,
        };

        for (int i = 0; i < 7; i++)
        {
            var day = (DayOfWeek)i;
            row.Children.Add(
                new Label
                {
                    Text = day.ToString()[..3],
                    FontSize = 12,
                    TextColor = SecondaryText,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                }
                    .Column(i));
        }

        return row;
    }

    private static Border CreateDayView()
    {
        return new Border
        {
            Padding = 0,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 6 },
            HeightRequest = DayCellSize,
            WidthRequest = DayCellSize,
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
