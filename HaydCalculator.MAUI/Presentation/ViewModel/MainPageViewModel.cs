using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Calculator.Services;
using HaydCalculator.Core.Misc;
using PropertyChanged;
using XCalendar.Core.Enums;
using XCalendar.Core.Models;

namespace MauiTestApp.Presentation.ViewModel;

[AddINotifyPropertyChangedInterface]
public class MainPageViewModel
{
    private static readonly DateTime s_startDateTime = new(2023, 1, 1);
    private readonly HaydCalculatorService _haydCalculatorService;
    private HaydCalculationResultVO _result = null;

    public MainPageViewModel(HaydCalculatorService haydCalculatorService)
    {
        _haydCalculatorService = haydCalculatorService;
        FlowAppearances = [.. Enum.GetValues<EFlowAppearanceColor>()];
        AssignDefaultValuesToViews();
    }

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

    public List<EFlowAppearanceColor> FlowAppearances { get; set; } = [];
    public EFlowAppearanceColor SelectedFlowAppearance { get; set; } = EFlowAppearanceColor.Red;
    public List<FlowDataEntity> InputData { get; set; } = [];

    public string MakeUpDayCounterText { get; set; }
    public string DayCountEntryText { get; set; }
    public string FeedbackText { get; set; }

    public void AddDataButton_Clicked(object sender, EventArgs e)
    {
        RunCalculation(addCurrentInput: true);
    }

    public void ClearDataButton_Clicked(object sender, EventArgs e)
    {
        InputData.Clear();
        FeedbackText = "";

        AssignDefaultValuesToViews();
        ApplyDataToViews();
    }

    private void AssignDefaultValuesToViews()
    {
        SelectedFlowAppearance = EFlowAppearanceColor.Red;
        DayCountEntryText = "3";
    }

    private void AddInputData(FlowDataEntity newInputData)
    {
        if (InputData.LastOrDefault() is FlowDataEntity lastData)
        {
            if (lastData.FromDateTime == newInputData.FromDateTime)
            {
                InputData.Remove(lastData);
            }
            else
            {
                lastData.ToDateTime = newInputData.FromDateTime;
            }
        }

        InputData.Add(newInputData);
    }

    private FlowDataEntity GetHaydTimeDataFromInput()
    {
        DateTime dateTime = InputData.LastOrDefault()?.ToDateTime ?? s_startDateTime;

        if (!double.TryParse(DayCountEntryText, out double dayCount) || dayCount <= 0)
        {
            DayCountEntryText = "";
            throw new InfoException("Invalid input!");
        }

        return new FlowDataEntity()
        {
            FromDateTime = dateTime,
            ToDateTime = dateTime.AddDays(dayCount),
            Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = SelectedFlowAppearance }
        };
    }

    private void RunCalculation(bool addCurrentInput)
    {
        try
        {
            FeedbackText = "";

            if (addCurrentInput)
            {
                AddInputData(GetHaydTimeDataFromInput());
            }

            _result = _haydCalculatorService.Calculate([.. InputData.Select(x => new FlowDataEntity(x))]);
        }
        catch (InfoException ex)
        {
            FeedbackText = ex.Message;
        }
        catch (Exception ex)
        {
            FeedbackText = $"""
                ERROR: {ex.Message}
                
                Stacktrace: 
                {ex.StackTrace}
                """;
        }
        finally
        {
            ApplyDataToViews();
        }
    }

    private void ApplyDataToViews()
    {
        MakeUpDayCounterText = $"Make up days: {_result?.MakeUpDayCount ?? 0}";
        SetCalendarColors();
    }

    public void SetCalendarColors()
    {
        if (InputData.Count == 0)
        {
            foreach (CustomDay day in Calendar.Days)
            {
                day.MainColor = Colors.White;
                day.MainTextColor = Colors.Black;
            }
            ;

            return;
        }

        DateTime minDate = InputData.MinBy(x => x.FromDateTime).FromDateTime;
        DateTime maxDate = InputData.MaxBy(x => x.ToDateTime).ToDateTime;

        foreach (CustomDay day in Calendar.Days)
        {
            // basically days for which there is no data
            if (day.DateTime < minDate || maxDate <= day.DateTime)
            {
                day.MainColor = Colors.White;
                day.MainTextColor = Colors.Black;
            }
            else if (IsHaydDay(day.DateTime))
            {
                if (IsNaqaDay(day.DateTime))
                {
                    day.MainColor = Colors.IndianRed;
                }
                else
                {
                    day.MainColor = Colors.Red;
                }

                day.MainTextColor = Colors.White;
            }
            else if (IsIstihadaDay(day.DateTime))
            {
                day.MainColor = Colors.Black;
                day.MainTextColor = Colors.White;
            }
            // days for which there is data but it is not hayd or istihada
            else
            {
                day.MainColor = Colors.LightGray;
                day.MainTextColor = Colors.Black;
            }
        }
    }
    public static bool IsDateWithinDateRange(DateTime targetDate, DateTime fromDate, DateTime toDate)
    {
        return fromDate <= targetDate && targetDate < toDate;
    }

    public bool IsHaydDay(DateTime date)
    {
        return _result?.HaydCycles.SelectMany(x => x.HaydFlows).Any(x => IsDateWithinDateRange(date, x.FromDateTime, x.ToDateTime)) == true;
    }

    public bool IsNaqaDay(DateTime date)
    {
        return _result?.HaydCycles.SelectMany(x => x.HaydFlows.Where(x => x.IsNaqa)).Any(x => IsDateWithinDateRange(date, x.FromDateTime, x.ToDateTime)) == true;
    }

    public bool IsIstihadaDay(DateTime date)
    {
        return _result?.IstihadaFlows.Any(x => IsDateWithinDateRange(date, x.FromDateTime, x.ToDateTime)) == true;
    }
}
