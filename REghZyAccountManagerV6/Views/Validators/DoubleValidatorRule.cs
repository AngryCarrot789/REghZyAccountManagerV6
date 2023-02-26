using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using REghZyAccountManagerV6.Core.Utils;

namespace REghZyAccountManagerV6.Views.Validators {
    public class IsNumberValidator : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            if (value == null) {
                return new ValidationResult(false, "No value provided");
            }

            if (value == DependencyProperty.UnsetValue) {
                throw new Exception("Did not expect null or unset");
            }

            if (value is double || double.TryParse(ObjUtils.ToString(value, ""), out _)) {
                return ValidationResult.ValidResult;
            }

            throw new Exception($"Value is not a number: {value}");
        }
    }
}