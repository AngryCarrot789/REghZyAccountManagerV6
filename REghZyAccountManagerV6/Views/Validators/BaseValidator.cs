using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace REghZyAccountManagerV6.Views.Validators {
    public abstract class BaseValidator<T> : ValidationRule {
        public static ValidationResult Valid => ValidationResult.ValidResult;

        public bool IgnoreInvalidType { get; set; }
        public bool ThrowForInvalidType { get; set; }

        public object InvalidTypeMessage { get; set; }

        public bool ProcessValueConvertersFirst { get; set; }

        public ICollection<IValueConverter> ValueConverters { get; set; }

        public object ConverterParameter { get; set; }

        protected BaseValidator() {
            this.ProcessValueConvertersFirst = true;
            this.ValueConverters = new List<IValueConverter>();
        }

        public override ValidationResult Validate(object value, CultureInfo culture) {
            if (this.ValueConverters != null && this.ValueConverters.Count > 0) {
                foreach (IValueConverter converter in this.ValueConverters) {
                    try {
                        value = converter.Convert(value, typeof(T), this.ConverterParameter, culture);
                    }
                    catch (Exception e) {
                        return new ValidationResult(false, e.Message);
                    }
                }
            }

            if (value == null || value is T) {
                // could just cast directly to T maybe?
                return this.ValidateValue(value == null ? default : (T) value, culture);
            }

            if (this.IgnoreInvalidType) {
                return Valid;
            }
            else if (this.ThrowForInvalidType) {
                throw new Exception($"Validation expected value of type {typeof(T)}");
            }
            else {
                return new ValidationResult(false, this.InvalidTypeMessage);
            }
        }

        public abstract ValidationResult ValidateValue(T value, CultureInfo culture);
    }
}