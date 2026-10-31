using System.Collections.ObjectModel;
using System.Globalization;
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
        CalendarTitleText = s_startDate.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

        // strongest first, as the user usually starts with the strongest blood
        FlowAppearances = [.. Enum.GetValues<EFlowAppearanceColor>().Reverse().Select(x => new FlowAppearanceOption(x))];
        AddDataCommand = new Command(() => RunCalculation(addCurrentInput: true));
        UndoLastCommand = new Command(UndoLast);
        ClearDataCommand = new Command(ClearData);
        AssignDefaultValuesToViews();
        ApplyDataToViews();

        // the day colors depend on the light or dark theme
        if (Application.Current is Application application)
        {
            application.RequestedThemeChanged += (_, _) => SetCalendarColors();
        }
    }

    public Calendar<CustomDay> Calendar { get; set; }
    public string CalendarTitleText { get; set; }

    public List<FlowAppearanceOption> FlowAppearances { get; set; } = [];
    public FlowAppearanceOption? SelectedFlowAppearance { get; set; }
    public ObservableCollection<FlowDataEntity> InputData { get; set; } = [];
    public ObservableCollection<InputEntryItem> InputEntries { get; set; } = [];
    public bool HasInput { get; set; }

    public string DayCountEntryText { get; set; } = "";

    public string FeedbackText { get; set; } = "";
    public string FeedbackDetailsText { get; set; } = "";
    public bool IsFeedbackError { get; set; }
    public bool HasFeedback => FeedbackText.Length > 0;
    public bool HasFeedbackDetails => FeedbackDetailsText.Length > 0;

    public bool HasResult { get; set; }
    public string HaydDaysText { get; set; } = "";
    public string IstihadaDaysText { get; set; } = "";
    public string MakeUpDaysText { get; set; } = "";
    public string HaydPeriodsText { get; set; } = "";

    public ICommand AddDataCommand { get; }
    public ICommand UndoLastCommand { get; }
    public ICommand ClearDataCommand { get; }

    // XCalendar's API is the only place with BCL date types; they are converted to and from
    // LocalDate right here (ToDateTimeUnspecified / LocalDate.FromDateTime) and nowhere else.
    private static Calendar<CustomDay> CreateCalendar(LocalDate today)
    {
        return new Calendar<CustomDay>()
        {
            NavigatedDate = s_startDate.ToDateTimeUnspecified(),
            NavigationLowerBound = LocalDate.Min(s_startDate, today.PlusYears(-2)).ToDateTimeUnspecified(),
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

    private static string FormatDays(Duration duration)
    {
        double days = duration.TotalDays;
        return days == 1 ? "1 day" : $"{days:0.##} days";
    }

    /// <summary>
    /// The calendar days a range touches; <paramref name="to"/> is exclusive.
    /// </summary>
    private static string FormatRange(Instant from, Instant to)
    {
        LocalDate firstDay = from.InUtc().Date;
        LocalDate lastDay = (to - Duration.Epsilon).InUtc().Date;

        string first = firstDay.ToString("d MMM", CultureInfo.InvariantCulture);
        return firstDay == lastDay ? first : $"{first} – {lastDay.ToString("d MMM", CultureInfo.InvariantCulture)}";
    }

    private void ClearData()
    {
        InputData.Clear();
        ClearFeedback();
        _result = null;

        AssignDefaultValuesToViews();
        ApplyDataToViews();
    }

    private void UndoLast()
    {
        if (InputData.Count == 0)
        {
            return;
        }

        InputData.RemoveAt(InputData.Count - 1);

        if (InputData.Count == 0)
        {
            ClearData();
            return;
        }

        RunCalculation(addCurrentInput: false);
    }

    private void ClearFeedback()
    {
        FeedbackText = "";
        FeedbackDetailsText = "";
        IsFeedbackError = false;
    }

    private void AssignDefaultValuesToViews()
    {
        SelectedFlowAppearance = FlowAppearances.First(x => x.Value == EFlowAppearanceColor.Red);
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
            Description = new FlowDataDescriptionEntity()
            {
                FlowAppearanceColorEnum = SelectedFlowAppearance?.Value ?? EFlowAppearanceColor.Red,
            },
        };
    }

    [Time]
    private void RunCalculation(bool addCurrentInput)
    {
        try
        {
            ClearFeedback();

            if (addCurrentInput)
            {
                AddInputData(GetHaydTimeDataFromInput());
            }

            _result = _haydCalculatorService.Calculate([.. InputData.Select(x => new FlowDataEntity(x))]);
        }
        catch (InfoException ex)
        {
            _logger.LogInformation("Calculation stopped: {Reason}", ex.Message);

            // a result of the previous input would not match the current input
            _result = null;
            FeedbackText = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Calculation failed for {FlowDataCount} input entries", InputData.Count);
            _result = null;
            IsFeedbackError = true;
            FeedbackText = $"Error: {ex.Message}";
            FeedbackDetailsText = ex.StackTrace ?? "";
        }
        finally
        {
            ApplyDataToViews();
        }
    }

    private void ApplyDataToViews()
    {
        InputEntries.Clear();
        foreach (FlowDataEntity data in InputData)
        {
            EFlowAppearanceColor appearance = data.Description.FlowAppearanceColorEnum;
            InputEntries.Add(new InputEntryItem(
                DayColors.ForAppearance(appearance),
                DayColors.NameOf(appearance),
                FormatDays(data.Duration),
                FormatRange(data.From, data.To)));
        }

        HasInput = InputData.Count > 0;
        ApplyResultToViews();
        SetCalendarColors();
    }

    private void ApplyResultToViews()
    {
        HasResult = _result is not null;
        if (_result is null)
        {
            HaydDaysText = IstihadaDaysText = MakeUpDaysText = HaydPeriodsText = "";
            return;
        }

        List<FlowDataEntity> haydFlows = [.. _result.HaydCycles.SelectMany(x => x.HaydFlows)];
        HaydDaysText = FormatDays(Duration.FromNanoseconds(haydFlows.Sum(x => x.Duration.TotalNanoseconds)));
        IstihadaDaysText = FormatDays(Duration.FromNanoseconds(_result.IstihadaFlows.Sum(x => x.Duration.TotalNanoseconds)));
        MakeUpDaysText = FormatDays(_result.MakeUpDuration);

        HaydPeriodsText = string.Join(
            Environment.NewLine,
            _result.HaydCycles
                .Where(x => x.HaydFlows.Count > 0)
                .Select((cycle, index) =>
                {
                    Instant from = cycle.HaydFlows.Min(x => x.From);
                    Instant to = cycle.HaydFlows.Max(x => x.To);
                    return $"Hayd {index + 1}: {FormatRange(from, to)} ({FormatDays(to - from)})";
                }));
    }

    public void SetCalendarColors()
    {
        Instant? minInstant = InputData.Count == 0 ? null : InputData.Min(x => x.From);
        Instant? maxInstant = InputData.Count == 0 ? null : InputData.Max(x => x.To);

        foreach (CustomDay day in Calendar.Days)
        {
            Instant dayInstant = ToInstant(LocalDate.FromDateTime(day.DateTime));

            // basically days for which there is no data
            if (minInstant is null || dayInstant < minInstant || maxInstant <= dayInstant)
            {
                day.MainColor = DayColors.NoData;
                day.MainTextColor = day.IsCurrentMonth ? DayColors.Text : DayColors.OtherMonthText;
            }
            else if (_result is null)
            {
                day.MainColor = DayColors.NotEvaluated;
                day.MainTextColor = DayColors.Text;
            }
            else if (IsHaydDay(dayInstant))
            {
                day.MainColor = IsNaqaDay(dayInstant) ? DayColors.HaydNaqa : DayColors.Hayd;
                day.MainTextColor = Colors.White;
            }
            else if (IsIstihadaDay(dayInstant))
            {
                day.MainColor = DayColors.Istihada;
                day.MainTextColor = Colors.White;
            }
            // days for which there is data but it is not hayd or istihada
            else
            {
                day.MainColor = DayColors.Tuhr;
                day.MainTextColor = DayColors.Text;
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
