using System.Globalization;
using System.Windows.Controls;

namespace REghZyAccountManagerV6.Views.Validators {
    public abstract class RangeValidator<T> : BaseValidator<T> {
        public bool AllowInclusiveMin { get; set; }
        public bool AllowInclusiveMax { get; set; }

        public object ValueTooSmall { get; set; }
        public object ValueTooLarge { get; set; }

        public T Min { get; set; }
        public T Max { get; set; }

        protected RangeValidator() {
            this.AllowInclusiveMin = true;
            this.AllowInclusiveMax = true;
            this.ValueTooSmall = "Value is too small";
            this.ValueTooLarge = "Value is too large";
        }

        public override ValidationResult ValidateValue(T value, CultureInfo culture) {
            if (this.Min != null && this.IsTooSmall(value, this.Min, this.AllowInclusiveMin)) {
                return new ValidationResult(false, this.ValueTooSmall);
            }

            if (this.Max != null && this.IsTooLarge(value, this.Max, this.AllowInclusiveMax)) {
                return new ValidationResult(false, this.ValueTooLarge);
            }

            return Valid;
        }

        public abstract bool IsTooSmall(T value, T min, bool canBeInclusive);
        public abstract bool IsTooLarge(T value, T max, bool canBeInclusive);
    }

    public class DoubleRangeValidator : RangeValidator<double> {
        public override bool IsTooSmall(double value, double min, bool canBeInclusive) {
            return canBeInclusive ? (value < min) : (value <= min);
        }

        public override bool IsTooLarge(double value, double max, bool canBeInclusive) {
            return canBeInclusive ? (value > max) : (value >= max);
        }
    }

    public class IntRangeValidator : RangeValidator<int> {
        public override bool IsTooSmall(int value, int min, bool canBeInclusive) {
            return canBeInclusive ? (value < min) : (value <= min);
        }

        public override bool IsTooLarge(int value, int max, bool canBeInclusive) {
            return canBeInclusive ? (value > max) : (value >= max);
        }
    }
}