using System;
using System.Globalization;
using System.Windows.Controls;

namespace REghZyAccountManagerV6.Views {
    public abstract class BaseValidator<T> : ValidationRule {
        public static ValidationResult Valid => ValidationResult.ValidResult;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            if (value == null || value is T) {
                // could just cast directly to T maybe?
                return this.ValidateValue(value == null ? default : (T) value, cultureInfo);
            }

            throw new Exception($"Validation expected value of type {typeof(T)}");
        }

        public abstract ValidationResult ValidateValue(T value, CultureInfo culture);
    }
}