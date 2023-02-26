using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using REghZyAccountManagerV6.Core.Utils;
using REghZyAccountManagerV6.Views.Validators;

namespace REghZyAccountManagerV6.Views.Settings {
    public class SaveDirectoryValidationRule : BaseValidator<string> {
        public override ValidationResult ValidateValue(string value, CultureInfo culture) {
            if (string.IsNullOrEmpty(value)) {
                return new ValidationResult(false, "A non-empty save directory must be provided");
            }
            else if (FileHelper.IsPathInvalid(value, out List<Tuple<int, char>> illegal)) {
                return new ValidationResult(false, $"Invalid file path character{(illegal.Count == 1 ? "" : "s")}: {string.Join(", ", illegal.Select(a => $"'{a.Item2}'"))}");
            }
            else {
                return Valid;
            }
        }
    }
}