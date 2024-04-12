namespace HaydCalculator.Core.Calculator.Models
{
    public class FlowDataEntity
    {
        #region properties

        public bool IsNaqa { get; set; } = false;

        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }

        public FlowDataDescriptionEntity Description { get; set; } = new FlowDataDescriptionEntity();

        #endregion properties

        #region calculated properties

        public double DayCount
        {
            get
            {
                return (ToDateTime - FromDateTime).TotalDays;
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
            FromDateTime = copiedFromData.FromDateTime;
            ToDateTime = copiedFromData.ToDateTime;
        }

        #endregion constructors

        #region override methods

        public override bool Equals(object obj)
        {
            if (obj is not FlowDataEntity compareToObject)
                return false;

            return
                FromDateTime == compareToObject.FromDateTime
                && ToDateTime == compareToObject.ToDateTime
                && Description.FlowAppearanceColorEnum == compareToObject.Description.FlowAppearanceColorEnum;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            //return $"{this.Description.FlowAppearanceColorEnum}: {FromDateTime.ToString(TextUtil.DATE_FORMAT)} - {ToDateTime.ToString(TextUtil.DATE_FORMAT)} [{DayCount.ToString("N2")} days]";
            return $"{Description.FlowAppearanceColorEnum}: {DayCount:0.##} days";
        }

        #endregion override methods

        #region methods


        #endregion methods
    }
}