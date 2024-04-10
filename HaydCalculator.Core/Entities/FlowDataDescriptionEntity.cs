using HaydCalculator.Core.Enums;

namespace HaydCalculator.Core.Entities
{
    public class FlowDataDescriptionEntity : IComparable
    {
        #region constructors

        public FlowDataDescriptionEntity() { }

        public FlowDataDescriptionEntity(FlowDataDescriptionEntity copyFrom)
        {
            FlowAppearanceColorEnum = copyFrom.FlowAppearanceColorEnum;
            IsThick = copyFrom.IsThick;
            IsOdorStingy = copyFrom.IsOdorStingy;
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
                    FlowAppearanceColorEnum != EFlowAppearanceColor.Clear;
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
                FlowAppearanceColorEnum == compareToObject.FlowAppearanceColorEnum
                && IsOdorStingy == compareToObject.IsOdorStingy
                && IsThick == compareToObject.IsThick;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Type: {FlowAppearanceColorEnum}";//; Stingy odor: {this.IsOdorStingy}; Thick: {this.IsThick}]";
        }

        public int CompareTo(object compareToObj)
        {
            if (compareToObj is not FlowDataDescriptionEntity compareToDescription)
                throw new ArgumentException($"Can not compare {nameof(FlowDataDescriptionEntity)} with an object of a different type.");

            if (this == compareToDescription)
                return 0;

            if (!IsHaydType && !compareToDescription.IsHaydType)
                return -1;
            if (IsHaydType && !compareToDescription.IsHaydType)
                return 0;
            if (!IsHaydType && compareToDescription.IsHaydType)
                return 1;

            int thisStrongQualityCount = 0;
            int compareToStrongQualityCount = 0;

            if (FlowAppearanceColorEnum < compareToDescription.FlowAppearanceColorEnum)
                compareToStrongQualityCount++;
            else if (FlowAppearanceColorEnum > compareToDescription.FlowAppearanceColorEnum)
                thisStrongQualityCount++;

            if (!IsOdorStingy && compareToDescription.IsOdorStingy)
                compareToStrongQualityCount++;
            else if (IsOdorStingy && !compareToDescription.IsOdorStingy)
                thisStrongQualityCount++;

            if (!IsThick && compareToDescription.IsThick)
                compareToStrongQualityCount++;
            else if (IsThick && !compareToDescription.IsThick)
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
