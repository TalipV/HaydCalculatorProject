using HaydCalculator;
using HaydCalculator.Entities;
using HaydCalculator.Enums;
using System.IO;
using XCalendar.Core.Enums;
using XCalendar.Core.Models;

namespace MauiTestApp
{
    public partial class MainPage : ContentPage
    {
        #region fields 

        private static readonly DateTime BEGINNING_DATE_TIME = new DateTime(2023, 1, 1);
        private HaydCalculatorFactory _haydCalculatorFactory = new HaydCalculatorFactory();
        private List<FlowDataEntity> _inputData = new List<FlowDataEntity>();

        #endregion fields

        /// <summary>
        /// Comment
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new MainPageViewModel();

            this.flowAppearanceSelectorView.ItemsSource = Enum.GetValues(typeof(EFlowAppearanceColor));
            this.inputListView.ItemSelected += (sender, e) => { (sender as ListView).SelectedItem = null; };

            assignDefaultValuesToViews();
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

        #region methods

        private void assignDefaultValuesToViews()
        {
            this.flowAppearanceSelectorView.SelectedItem = EFlowAppearanceColor.Red;
            this.dayCountEntryView.Text = "3";
        }

        private FlowDataEntity getHaydTimeDataFromInput()
        {
            EFlowAppearanceColor haydDataType = (EFlowAppearanceColor)this.flowAppearanceSelectorView.SelectedItem;
            DateTime fromDateTime = _inputData.LastOrDefault()?.ToDateTime ?? BEGINNING_DATE_TIME;
            DateTime toDateTime = fromDateTime;

            double dayCount = 0;

            if (double.TryParse(this.dayCountEntryView.Text, out dayCount) && dayCount > 0)
            {
                toDateTime = fromDateTime.AddDays(dayCount);
            }
            else
            {
                this.dayCountEntryView.Text = "";
                throw new InfoException("Invalid input!");
            }

            return new FlowDataEntity()
            {
                FromDateTime = fromDateTime,
                ToDateTime = toDateTime,
                Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = haydDataType }
            };
        }

        private void applyDataToViews()
        {
            this.inputListView.ItemsSource = _inputData.ToList();
            this.makeUpDayCounter.Text = "Make up days: " + _haydCalculatorFactory.MakeUpDayCount;
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
                feedbackEditor.Text = "";
                _haydCalculatorFactory.ResetCalculationData();

                if (addCurrentInput)
                    addInputData(getHaydTimeDataFromInput());

                _haydCalculatorFactory.DataList = _inputData.Select(x => new FlowDataEntity(x)).ToList().AsReadOnly();
                _haydCalculatorFactory.Execute();
            }
            catch (InfoException ex)
            {
                feedbackEditor.Text = ex.Message;
            }
            catch (Exception ex)
            {
                feedbackEditor.Text = $"ERROR: {ex.Message}{Environment.NewLine}{Environment.NewLine}Stacktrace: {Environment.NewLine}{ex.StackTrace}";
            }
            finally
            {
                applyDataToViews();
            }
        }

        #endregion methods

        #region events

        private void addDataButton_Clicked(object sender, EventArgs e)
        {
            runCalculation(addCurrentInput: true);
        }

        private void clearDataButton_Clicked(object sender, EventArgs e)
        {
            this._inputData.Clear();
            this._haydCalculatorFactory.ResetCalculationData();
            this.feedbackEditor.Text = "";

            this.assignDefaultValuesToViews();
            this.applyDataToViews();
        }

        #endregion events
    }
}