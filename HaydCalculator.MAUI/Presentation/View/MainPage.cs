using HaydCalculator.Core;
using HaydCalculator.Core.Entities;
using HaydCalculator.Core.Enums;
using MauiTestApp.Presentation;
using MauiTestApp.Presentation.ViewModel;
using System.Xml.Linq;
using System;
using XCalendar.Core.Models;
using XCalendar.Maui.Views;
using Microsoft.Maui.Graphics.Text;

namespace HaydCalculator
{
    public class MainPage : ContentPage
    {
        private static readonly DateTime START_DATE_TIME = new(2023, 1, 1);
        private readonly HaydCalculatorFactory _haydCalculatorFactory = new();
        private readonly List<FlowDataEntity> _inputData = [];

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
            this.BindingContext = viewModel;

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
                ItemsSource = Enum.GetValues(typeof(EFlowAppearanceColor))
            };

            grid.Children.Add(_flowAppearanceSelectorView);
            Grid.SetColumn(_flowAppearanceSelectorView, 3);

            var addDataButton = new Button
            {
                Text = "Add",
                Margin = 5,
                HorizontalOptions = LayoutOptions.Center
            };
            addDataButton.Clicked += addDataButton_Clicked;
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
            _inputListView.ItemSelected += (sender, e) => { (sender as ListView).SelectedItem = null; };

            grid.Children.Add(_inputListView);
            Grid.SetColumnSpan(_inputListView, 4);
            Grid.SetColumn(_inputListView, 0);
            Grid.SetRow(_inputListView, 3);

            var clearDataButton = new Button { Text = "Clear", Margin = 10, HeightRequest = 40 };
            clearDataButton.Clicked += clearDataButton_Clicked;
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
            grid.Children.Add(_makeUpDayCounter);
            Grid.SetColumnSpan(_makeUpDayCounter, 3);
            Grid.SetColumn(_makeUpDayCounter, 3);
            Grid.SetRow(_makeUpDayCounter, 10);

            _feedbackEditor = new Editor { BackgroundColor = Colors.AntiqueWhite, Margin = 5, IsReadOnly = true };
            grid.Children.Add(_feedbackEditor);
            Grid.SetColumnSpan(_feedbackEditor, 5);
            Grid.SetRow(_feedbackEditor, 11);

            scrollView.Content = grid;
            Content = scrollView;

            assignDefaultValuesToViews();
            addCalendarStuff(grid);
        }

        private CalendarView _calendarView;

        private void addCalendarStuff(Grid grid)
        {
            var fancyCalendarView = _calendarView = new CalendarView
            {
                BackgroundColor = Colors.White,
                DayNamesHeightRequest = 0,
                DaysViewHeightRequest = 500,
                IsVisible = true
            };
            grid.Children.Add(fancyCalendarView);
            Grid.SetRow(fancyCalendarView, 5);
            Grid.SetColumnSpan(fancyCalendarView, 5);
            Grid.SetRowSpan(fancyCalendarView, 3);

            fancyCalendarView.SetBinding(CalendarView.DaysProperty, $"{nameof(Calendar)}.{nameof(Calendar.Days)}");
            fancyCalendarView.SetBinding(CalendarView.DaysOfWeekProperty, $"{nameof(Calendar)}.{nameof(Calendar.DayNamesOrder)}");
            fancyCalendarView.NavigationViewTemplate = new ControlTemplate(() => new NavigationView { HeightRequest = 0 });
            
            return;

            fancyCalendarView.DayTemplate = new DataTemplate(() =>
            {
                var frame = new Frame
                {
                    Margin = 3,
                    Padding = 0,
                    HasShadow = false,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };

                var dayView = new DayView
                {
                    Margin = 1,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HeightRequest = 35,
                    WidthRequest = 35
                };

                frame.Content = dayView;

                // Assuming CustomDay is your data class, you would bind properties like MainColor, MainTextColor etc.
                frame.SetBinding(BackgroundColorProperty, $"{nameof(CustomDay.MainColor)}");
                dayView.SetBinding(BackgroundColorProperty, $"{nameof(CustomDay.MainColor)}");

                dayView.SetBinding(DayView.BackgroundColorProperty, $"{nameof(CustomDay.MainColor)}");
                dayView.SetBinding(DayView.TextColorProperty, $"{nameof(CustomDay.MainTextColor)}");
                dayView.SetBinding(DayView.DateTimeProperty, $"{nameof(CustomDay.DateTime)}");

                dayView.SetBinding(DayView.SelectedBackgroundColorProperty, $"{nameof(CustomDay.MainColor)}");
                dayView.SetBinding(DayView.CurrentMonthBackgroundColorProperty, $"{nameof(CustomDay.MainColor)}");
                dayView.SetBinding(DayView.OtherMonthBackgroundColorProperty, $"{nameof(CustomDay.MainColor)}");
                dayView.SetBinding(DayView.SelectedTextColorProperty, $"{nameof(CustomDay.MainTextColor)}");
                dayView.SetBinding(DayView.CurrentMonthTextColorProperty, $"{nameof(CustomDay.MainTextColor)}");
                dayView.SetBinding(DayView.OtherMonthTextColorProperty, $"{nameof(CustomDay.MainTextColor)}");

                return new ViewCell { View = frame };
            });
        }

        public Calendar<CustomDay> Calendar => (this.BindingContext as MainPageViewModel)?.Calendar;

        public bool IsDateWithinDateRange(DateTime targetDate, DateTime fromDate, DateTime toDate)
        {
            return fromDate <= targetDate && targetDate < toDate;
        }

        // TODO: Refactoring

        public bool IsHaydDay(DateTime date)
        {
            return _haydCalculatorFactory?.HaydCycleLst.SelectMany(x => x.HaydDataLst).Any(x => IsDateWithinDateRange(date, x.FromDateTime, x.ToDateTime)) == true;
        }

        public bool IsNaqaDay(DateTime date)
        {
            return _haydCalculatorFactory?.HaydCycleLst.SelectMany(x => x.HaydDataLst.Where(x => x.IsNaqa)).Any(x => IsDateWithinDateRange(date, x.FromDateTime, x.ToDateTime)) == true;
        }

        public bool IsIstihadaDay(DateTime date)
        {
            return _haydCalculatorFactory?.IstihadaLst.Any(x => IsDateWithinDateRange(date, x.FromDateTime, x.ToDateTime)) == true;
        }

        public void setCalendarColors()
        {
            if (this._inputData.Count == 0)
            {
                this.Calendar.Days.ToList().ForEach(day =>
                {
                    day.MainColor = Colors.White;
                    day.MainTextColor = Colors.Black;
                });

                return;
            }

            DateTime minDate = this._inputData.MinBy(x => x.FromDateTime).FromDateTime;
            DateTime maxDate = this._inputData.MaxBy(x => x.ToDateTime).ToDateTime;

            foreach (CustomDay day in Calendar.Days)
            {
                if (day.DateTime < minDate || day.DateTime > maxDate)
                {
                    day.MainColor = Colors.White;
                    day.MainTextColor = Colors.Black;
                }

                if (this.IsHaydDay(day.DateTime))
                {
                    if (this.IsNaqaDay(day.DateTime))
                        day.MainColor = Colors.IndianRed;
                    else
                        day.MainColor = Colors.Red;

                    day.MainTextColor = Colors.White;
                }
                else if (this.IsIstihadaDay(day.DateTime))
                {
                    day.MainColor = Colors.Black;
                    day.MainTextColor = Colors.White;
                }
                else
                {
                    day.MainColor = Colors.White;
                    day.MainTextColor = Colors.Black;
                }
            }
        }

        private void assignDefaultValuesToViews()
        {
            this._flowAppearanceSelectorView.SelectedItem = EFlowAppearanceColor.Red;
            this._dayCountEntryView.Text = "3";
        }

        private FlowDataEntity getHaydTimeDataFromInput()
        {
            EFlowAppearanceColor haydDataType = (EFlowAppearanceColor)this._flowAppearanceSelectorView.SelectedItem;
            DateTime dateTime = _inputData.LastOrDefault()?.ToDateTime ?? START_DATE_TIME;

            if (!double.TryParse(this._dayCountEntryView.Text, out double dayCount) || dayCount <= 0)
            {
                this._dayCountEntryView.Text = "";
                throw new InfoException("Invalid input!");
            }

            return new FlowDataEntity()
            {
                FromDateTime = dateTime,
                ToDateTime = dateTime.AddDays(dayCount),
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = haydDataType }
            };
        }

        private void applyDataToViews()
        {
            this._inputListView.ItemsSource = _inputData.ToList();
            this._makeUpDayCounter.Text = $"Make up days: {_haydCalculatorFactory.MakeUpDayCount}";
            this.setCalendarColors();
        }

        private void addInputData(FlowDataEntity newInputData)
        {
            if (_inputData.LastOrDefault() is FlowDataEntity lastData)
            {
                if (lastData.FromDateTime == newInputData.FromDateTime)
                {
                    _inputData.Remove(lastData);
                }
                else
                {
                    lastData.ToDateTime = newInputData.FromDateTime;
                }
            }

            _inputData.Add(getHaydTimeDataFromInput());
        }

        private void runCalculation(bool addCurrentInput)
        {
            try
            {
                _feedbackEditor.Text = "";
                _haydCalculatorFactory.ResetCalculationData();

                if (addCurrentInput)
                    addInputData(getHaydTimeDataFromInput());

                _haydCalculatorFactory.DataList = _inputData.Select(x => new FlowDataEntity(x)).ToList().AsReadOnly();
                _haydCalculatorFactory.Execute();
            }
            catch (InfoException ex)
            {
                _feedbackEditor.Text = ex.Message;
            }
            catch (Exception ex)
            {
                _feedbackEditor.Text = $"""
                    ERROR: {ex.Message}
                    
                    Stacktrace: 
                    {ex.StackTrace}
                    """;
            }
            finally
            {
                applyDataToViews();
            }
        }

        private void addDataButton_Clicked(object sender, EventArgs e)
        {
            runCalculation(addCurrentInput: true);
        }

        private void clearDataButton_Clicked(object sender, EventArgs e)
        {
            this._inputData.Clear();
            this._haydCalculatorFactory.ResetCalculationData();
            this._feedbackEditor.Text = "";

            this.assignDefaultValuesToViews();
            this.applyDataToViews();
        }
    }
}