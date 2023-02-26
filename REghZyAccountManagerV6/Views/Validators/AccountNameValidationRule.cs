using System.Globalization;
using System.Windows.Controls;
using REghZyAccountManagerV6.Core;

namespace REghZyAccountManagerV6.Views.Validators {
    public class AccountNameValidationRule : BaseValidator<string> {
        public override ValidationResult ValidateValue(string value, CultureInfo culture) {
            if (string.IsNullOrEmpty(value)) {
                return new ValidationResult(false, "A non-empty account name must be provided");
            }
            else if (ViewModelLocator.AccountCollection.Exists(value)) {
                return new ValidationResult(false, $"The account '{value}' already exists");
            }
            else {
                return Valid;
            }
        }
    }
}