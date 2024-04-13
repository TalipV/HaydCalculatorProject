using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Calculator.Services;
using HaydCalculator.Core.Misc;
using MauiTestApp.Presentation;
using MauiTestApp.Presentation.ViewModel;
using XCalendar.Core.Models;
using XCalendar.Maui.Views;

namespace MauiTestApp.Presentation.View
{
    public class MainPage : ContentPage
    {
        private readonly Picker _flowAppearanceSelectorView;
        private readonly Editor _feedbackEditor;
        private readonly ListView _inputListView;
        private readonly Entry _dayCountEntryView;
        private readonly Label _makeUpDayCounter;

        /// <summary>
        /// Comment
        /// </summary>
        public MainPage(MainPageViewModel viewModel)
        {
            BindingContext = viewModel;

            Title = "";
            var scrollView = new ScrollView();
            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(40) },
                    new RowDefinition { Height = new GridLength(50) },
                    new RowDefinition { Height = new GridLength(30) },
                    new RowDefinition { Height = new GridLength(120) },
                    new RowDefinition { Height = new GridLength(5) },
                    new RowDefinition { Height = new GridLength(25) },
                    new RowDefinition { Height = new GridLength(180) },
                    new RowDefinition { Height = new GridLength(25) },
                    new RowDefinition { Height = new GridLength(100) },
                    new RowDefinition { Height = new GridLength(5) },
                    new RowDefinition { Height = new GridLength(25) },
                    new RowDefinition { Height = new GridLength(170) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition(),
                    new ColumnDefinition(),
                    new ColumnDefinition { Width = new GridLength(10) },
                    new ColumnDefinition(),
                    new ColumnDefinition()
                }
            };

            _dayCountEntryView = new Entry
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                MaxLength = 5,
                MinimumWidthRequest = 35,
                BackgroundColor = Colors.AntiqueWhite,
                HorizontalTextAlignment = TextAlignment.Center
            };
            _dayCountEntryView.SetBinding(Entry.TextProperty, nameof(MainPageViewModel.DayCountEntryText), BindingMode.TwoWay);
            var hStackGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            hStackGrid.Children.Add(new Label
            {
                Text = "Days:",
                VerticalOptions = LayoutOptions.Center,
                Padding = 5
            });
            hStackGrid.Children.Add(_dayCountEntryView);
            Grid.SetColumn(_dayCountEntryView, 1);
            grid.Children.Add(hStackGrid);
            Grid.SetColumn(hStackGrid, 1);

            _flowAppearanceSelectorView = new Picker
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                MinimumWidthRequest = 45,
                BackgroundColor = Colors.AntiqueWhite,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            _flowAppearanceSelectorView.SetBinding(Picker.ItemsSourceProperty, nameof(MainPageViewModel.FlowAppearances));
            _flowAppearanceSelectorView.SetBinding(Picker.SelectedItemProperty, nameof(MainPageViewModel.SelectedFlowAppearance));
            grid.Children.Add(_flowAppearanceSelectorView);
            Grid.SetColumn(_flowAppearanceSelectorView, 3);

            var addDataButton = new Button
            {
                Text = "Add",
                Margin = 5,
                HorizontalOptions = LayoutOptions.Center
            };
            addDataButton.Clicked += viewModel.addDataButton_Clicked;
            grid.Children.Add(addDataButton);
            Grid.SetColumn(addDataButton, 1);
            Grid.SetRow(addDataButton, 1);

            _inputListView = new ListView
            {
                BackgroundColor = Colors.AntiqueWhite,
                Margin = 10,
                RowHeight = 20,
                ItemTemplate = new DataTemplate(() =>
                {
                    var label = new Label { TextColor = Colors.Blue };
                    label.SetBinding(Label.TextProperty, ".");
                    return new ViewCell { View = label };
                })
            };
            _inputListView.SetBinding(ListView.ItemsSourceProperty, nameof(MainPageViewModel.InputData));
            grid.Children.Add(_inputListView);
            Grid.SetColumnSpan(_inputListView, 4);
            Grid.SetColumn(_inputListView, 0);
            Grid.SetRow(_inputListView, 3);

            var clearDataButton = new Button { Text = "Clear", Margin = 10, HeightRequest = 40 };
            clearDataButton.Clicked += viewModel.clearDataButton_Clicked;
            grid.Children.Add(clearDataButton);
            Grid.SetColumn(clearDataButton, 4);
            Grid.SetRow(clearDataButton, 3);

            var boxView1 = new BoxView { Color = Colors.BlueViolet };
            grid.Children.Add(boxView1);
            Grid.SetColumnSpan(boxView1, 5);
            Grid.SetRow(boxView1, 4);

            var boxView2 = new BoxView { Color = Colors.BlueViolet };
            grid.Children.Add(boxView2);
            Grid.SetColumnSpan(boxView2, 5);
            Grid.SetRow(boxView2, 9);

            var feedbackLabel = new Label { Text = "Log", Padding = 3, FontAttributes = FontAttributes.Bold };
            grid.Children.Add(feedbackLabel);
            Grid.SetRow(feedbackLabel, 10);

            _makeUpDayCounter = new Label { Text = "Make up days: ", Padding = 3 };
            _makeUpDayCounter.SetBinding(Label.TextProperty, nameof(MainPageViewModel.MakeUpDayCounterText));
            grid.Children.Add(_makeUpDayCounter);
            Grid.SetColumnSpan(_makeUpDayCounter, 3);
            Grid.SetColumn(_makeUpDayCounter, 3);
            Grid.SetRow(_makeUpDayCounter, 10);

            _feedbackEditor = new Editor { BackgroundColor = Colors.AntiqueWhite, Margin = 5, IsReadOnly = true };
            _feedbackEditor.SetBinding(Editor.TextProperty, nameof(MainPageViewModel.FeedbackText));
            grid.Children.Add(_feedbackEditor);
            Grid.SetColumnSpan(_feedbackEditor, 5);
            Grid.SetRow(_feedbackEditor, 11);

            scrollView.Content = grid;
            Content = scrollView;

            var calendarView = new CalendarView
            {
                BackgroundColor = Colors.White,
                DayNamesHeightRequest = 0,
                DaysViewHeightRequest = 500,
                IsVisible = true
            };
            grid.Children.Add(calendarView);
            Grid.SetRow(calendarView, 5);
            Grid.SetColumnSpan(calendarView, 5);
            Grid.SetRowSpan(calendarView, 3);
            calendarView.SetBinding(CalendarView.DaysProperty, $"{nameof(Calendar)}.{nameof(Calendar.Days)}");
            calendarView.SetBinding(CalendarView.DaysOfWeekProperty, $"{nameof(Calendar)}.{nameof(Calendar.DayNamesOrder)}");
            calendarView.NavigationViewTemplate = new ControlTemplate(() => new NavigationView { HeightRequest = 0 });
            calendarView.DayTemplate = new DataTemplate(() =>
            {
                var frame = new Frame
                {
                    Margin = 3,
                    Padding = 0,
                    HasShadow = false,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };
                frame.SetBinding(BackgroundColorProperty, $"{nameof(CustomDay.MainColor)}");

                var dayView = frame.Content = new DayView
                {
                    Margin = 1,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HeightRequest = 35,
                    WidthRequest = 35
                };

                dayView.SetBinding(BackgroundColorProperty, nameof(CustomDay.MainColor));
                dayView.SetBinding(DayView.TextColorProperty, nameof(CustomDay.MainTextColor));
                dayView.SetBinding(DayView.DateTimeProperty, nameof(CustomDay.DateTime));
                dayView.SetBinding(DayView.SelectedStyleProperty, nameof(CustomDay.MainStyle));
                dayView.SetBinding(DayView.CurrentMonthStyleProperty, nameof(CustomDay.MainStyle));
                dayView.SetBinding(DayView.OtherMonthStyleProperty, nameof(CustomDay.MainStyle));

                return frame;
            });
        }
    }
}