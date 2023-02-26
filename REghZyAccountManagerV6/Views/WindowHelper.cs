using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace REghZyAccountManagerV6.Views {
    public static class WindowHelper {
        public static void GetValidationErrors(DependencyObject parent, Dictionary<string, string> errors) {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++) {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                ICollection<ValidationError> errorList = Validation.GetErrors(child);
                if (errorList != null && errorList.Count > 0) {
                    foreach (ValidationError error in errorList) {
                        if (error.BindingInError is BindingExpression expression && error.ErrorContent is string message && message.Length > 0) {
                            string property = expression.ResolvedSourcePropertyName;
                            if (property != null) {
                                errors[property] = message;
                            }
                        }
                    }
                }

                GetValidationErrors(child, errors);
            }
        }
    }
}