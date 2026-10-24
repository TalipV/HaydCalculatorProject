using System.Collections.ObjectModel;
using System.Windows.Input;
using HaydCalculator.Core.Calculator.Models;
using HaydCalculator.Core.Calculator.Services;
using HaydCalculator.Core.Misc;
using HaydCalculator.Presentation.Views.Calendar;
using MethodTimer;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Extensions;
using PropertyChanged;
using XCalendar.Core.Enums;
using XCalendar.Core.Models;

namespace HaydCalculator.Presentation.Pages.Main;

[AddINotifyPropertyChangedInterface]
public class MainPageViewModel
{
    private static readonly LocalDate s_startDate = new(2023, 1, 1);

    private readonly HaydCalculatorService _haydCalculatorService;
    private readonly ILogger<MainPageViewModel> _logger;
    private readonly IClock _clock;
    private HaydCalculationResultVO? _result = null;

    public MainPageViewModel(HaydCalculatorService haydCalculatorService, ILogger<MainPageViewModel> logger, IClock clock)
    {
        _haydCalculatorService = haydCalculatorService;
        _logger = logger;
        _clock = clock;
        Calendar = CreateCalendar(_clock.InTzdbSystemDefaultZone().GetCurrentDate());
        FlowAppearances = [.. Enum.GetValues<EFlowAppearanceColor>()];
        AddDataCommand = new Command(() => RunCalculation(addCurrentInput: true));
        ClearDataCommand = new Command(ClearData);
        AssignDefaultValuesToViews();
    }

    public Calendar<CustomDay> Calendar { get; set; }

    public List<EFlowAppearanceColor> FlowAppearances { get; set; } = [];
    public EFlowAppearanceColor SelectedFlowAppearance { get; set; } = EFlowAppearanceColor.Red;
    public ObservableCollection<FlowDataEntity> InputData { get; set; } = [];

    public string MakeUpDayCounterText { get; set; } = "";
    public string DayCountEntryText { get; set; } = "";
    public string FeedbackText { get; set; } = "";

    public ICommand AddDataCommand { get; }
    public ICommand ClearDataCommand { get; }

    // XCalendar's API is the only place with BCL date types; they are converted to and from
    // LocalDate right here (ToDateTimeUnspecified / LocalDate.FromDateTime) and nowhere else.
    private static Calendar<CustomDay> CreateCalendar(LocalDate today)
    {
        return new Calendar<CustomDay>()
        {
            NavigatedDate = s_startDate.ToDateTimeUnspecified(),
            NavigationLowerBound = today.PlusYears(-2).ToDateTimeUnspecified(),
            NavigationUpperBound = today.PlusYears(2).ToDateTimeUnspecified(),
            StartOfWeek = DayOfWeek.Sunday,
            SelectionAction = SelectionAction.Modify,
            NavigationLoopMode = NavigationLoopMode.LoopMinimumAndMaximum,
            SelectionType = SelectionType.Single,
            PageStartMode = PageStartMode.FirstDayOfYear,
            AutoRows = false,
            AutoRowsIsConsistent = true,
            Rows = 7,
            TodayDate = today.ToDateTimeUnspecified(),
        };
    }

    /// <summary>
    /// The calendar works with plain dates, the calculation with instants. Calendar days are
    /// mapped to midnight UTC, so every day is exactly 24 hours long (no daylight saving shifts).
    /// </summary>
    private static Instant ToInstant(LocalDate date)
    {
        return date.AtMidnight().InUtc().ToInstant();
    }

    private void ClearData()
    {
        InputData.Clear();
        FeedbackText = "";
        _result = null;

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
            if (lastData.From == newInputData.From)
            {
                InputData.Remove(lastData);
            }
            else
            {
                lastData.To = newInputData.From;
            }
        }

        InputData.Add(newInputData);
    }

    private FlowDataEntity GetHaydTimeDataFromInput()
    {
        Instant from = InputData.LastOrDefault()?.To ?? ToInstant(s_startDate);

        if (!double.TryParse(DayCountEntryText, out double dayCount) || dayCount <= 0)
        {
            DayCountEntryText = "";
            throw new InfoException("Invalid input!");
        }

        return new FlowDataEntity()
        {
            From = from,
            To = from + Duration.FromDays(dayCount),
            Description = new FlowDataDescriptionEntity() { FlowAppearanceColorEnum = SelectedFlowAppearance }
        };
    }

    [Time]
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
            _logger.LogInformation("Calculation stopped: {Reason}", ex.Message);
            FeedbackText = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Calculation failed for {FlowDataCount} input entries", InputData.Count);
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
        Duration makeUpDuration = _result?.MakeUpDuration ?? Duration.Zero;
        MakeUpDayCounterText = $"Make up days: {makeUpDuration.TotalDays:0.##}";
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

            return;
        }

        Instant minInstant = InputData.Min(x => x.From);
        Instant maxInstant = InputData.Max(x => x.To);

        foreach (CustomDay day in Calendar.Days)
        {
            Instant dayInstant = ToInstant(LocalDate.FromDateTime(day.DateTime));

            // basically days for which there is no data
            if (dayInstant < minInstant || maxInstant <= dayInstant)
            {
                day.MainColor = Colors.White;
                day.MainTextColor = Colors.Black;
            }
            else if (IsHaydDay(dayInstant))
            {
                if (IsNaqaDay(dayInstant))
                {
                    day.MainColor = Colors.IndianRed;
                }
                else
                {
                    day.MainColor = Colors.Red;
                }

                day.MainTextColor = Colors.White;
            }
            else if (IsIstihadaDay(dayInstant))
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

    public static bool IsWithinRange(Instant target, Instant from, Instant to)
    {
        return from <= target && target < to;
    }

    public bool IsHaydDay(Instant day)
    {
        return _result?.HaydCycles.SelectMany(x => x.HaydFlows).Any(x => IsWithinRange(day, x.From, x.To)) == true;
    }

    public bool IsNaqaDay(Instant day)
    {
        return _result?.HaydCycles.SelectMany(x => x.HaydFlows.Where(x => x.IsNaqa)).Any(x => IsWithinRange(day, x.From, x.To)) == true;
    }

    public bool IsIstihadaDay(Instant day)
    {
        return _result?.IstihadaFlows.Any(x => IsWithinRange(day, x.From, x.To)) == true;
    }
}
