using HaydCalculator.Enums;

namespace HaydCalculator.Entities
{
    public class FlowDataDescriptionEntity : IComparable
    {
        #region constructors

        public FlowDataDescriptionEntity() { }

        public FlowDataDescriptionEntity(FlowDataDescriptionEntity copyFrom)
        {
            this.FlowAppearanceColorEnum = copyFrom.FlowAppearanceColorEnum;
            this.IsThick = copyFrom.IsThick;
            this.IsOdorStingy = copyFrom.IsOdorStingy;
        }

        #endregion constructors

        #region properties

        public EFlowAppearanceColor FlowAppearanceColorEnum { get; set; } = EFlowAppearanceColor.Clear;
        public bool IsThick { get; set; } = false;
        public bool IsOdorStingy { get; set; } = false;

        #endregion properties

        #region calculated properties

        public bool IsHaydType
        {
            get
            {
                return 
                    this.FlowAppearanceColorEnum != EFlowAppearanceColor.Clear;
            }
        }

        #endregion calculated properties

        #region methods

        #endregion methods

        #region override methods

        public override bool Equals(object obj)
        {
            if (obj is not FlowDataDescriptionEntity compareToObject)
                return false;

            return
                this.FlowAppearanceColorEnum == compareToObject.FlowAppearanceColorEnum
                && this.IsOdorStingy == compareToObject.IsOdorStingy
                && this.IsThick == compareToObject.IsThick;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Type: {this.FlowAppearanceColorEnum}";//; Stingy odor: {this.IsOdorStingy}; Thick: {this.IsThick}]";
        }

        public int CompareTo(object compareToObj)
        {
            if (compareToObj is not FlowDataDescriptionEntity compareToDescription)
                throw new ArgumentException($"Can not compare {nameof(FlowDataDescriptionEntity)} with an object of a different type.");

            if (this == compareToDescription)
                return 0;

            if (!this.IsHaydType && !compareToDescription.IsHaydType)
                return -1;
            if (this.IsHaydType && !compareToDescription.IsHaydType)
                return 0;
            if (!this.IsHaydType && compareToDescription.IsHaydType)
                return 1;

            int thisStrongQualityCount = 0;
            int compareToStrongQualityCount = 0;

            if (this.FlowAppearanceColorEnum < compareToDescription.FlowAppearanceColorEnum)
                compareToStrongQualityCount++;
            else if (this.FlowAppearanceColorEnum > compareToDescription.FlowAppearanceColorEnum)
                thisStrongQualityCount++;

            if (!this.IsOdorStingy && compareToDescription.IsOdorStingy)
                compareToStrongQualityCount++;
            else if (this.IsOdorStingy && !compareToDescription.IsOdorStingy)
                thisStrongQualityCount++;

            if (!this.IsThick && compareToDescription.IsThick)
                compareToStrongQualityCount++;
            else if (this.IsThick && !compareToDescription.IsThick)
                thisStrongQualityCount++;

            if (thisStrongQualityCount < compareToStrongQualityCount)
                return -1;
            else if (thisStrongQualityCount > compareToStrongQualityCount)
                return 1;
            else
                return 0;
        }

        #endregion override methods
    }
}
