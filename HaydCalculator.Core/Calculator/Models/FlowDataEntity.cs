using NodaTime;

namespace HaydCalculator.Core.Calculator.Models;

public class FlowDataEntity
{
    #region properties

    public bool IsNaqa { get; set; } = false;

    public Instant From { get; set; }
    public Instant To { get; set; }

    public FlowDataDescriptionEntity Description { get; set; } = new FlowDataDescriptionEntity();

    #endregion properties

    #region calculated properties

    public Duration Duration
    {
        get
        {
            return To - From;
        }
    }

    #endregion calculated properties

    #region constructors

    public FlowDataEntity()
    {
    }

    public FlowDataEntity(FlowDataEntity copiedFromData)
    {
        Description = new FlowDataDescriptionEntity(copiedFromData.Description);
        From = copiedFromData.From;
        To = copiedFromData.To;
    }

    #endregion constructors

    #region override methods

    public override bool Equals(object? obj)
    {
        if (obj is not FlowDataEntity compareToObject)
        {
            return false;
        }

        return
            From == compareToObject.From
            && To == compareToObject.To
            && Description.FlowAppearanceColorEnum == compareToObject.Description.FlowAppearanceColorEnum;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(From, To, Description.FlowAppearanceColorEnum);
    }

    public override string ToString()
    {
        return $"{Description.FlowAppearanceColorEnum}: {Duration.TotalDays:0.##} days";
    }

    #endregion override methods
}
