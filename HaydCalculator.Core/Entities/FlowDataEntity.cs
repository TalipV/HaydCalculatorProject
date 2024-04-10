namespace HaydCalculator.Core.Entities
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
            this.Description = new FlowDataDescriptionEntity(copiedFromData.Description);
            this.FromDateTime = copiedFromData.FromDateTime;
            this.ToDateTime = copiedFromData.ToDateTime;
        }

        #endregion constructors

        #region override methods

        public override bool Equals(object obj)
        {
            if (obj is not FlowDataEntity compareToObject)
                return false;

            return
                this.FromDateTime == compareToObject.FromDateTime
                && this.ToDateTime == compareToObject.ToDateTime
                && this.Description.FlowAppearanceColorEnum == compareToObject.Description.FlowAppearanceColorEnum;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            //return $"{this.Description.FlowAppearanceColorEnum}: {FromDateTime.ToString(TextUtil.DATE_FORMAT)} - {ToDateTime.ToString(TextUtil.DATE_FORMAT)} [{DayCount.ToString("N2")} days]";
            return $"{this.Description.FlowAppearanceColorEnum}: {DayCount:0.##} days";
        }

        #endregion override methods

        #region methods


        #endregion methods
    }
}