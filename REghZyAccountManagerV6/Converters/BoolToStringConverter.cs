using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using REghZy.Utils;

namespace REghZyAccountManagerV6.Converters {
    public class BoolToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null || value == DependencyProperty.UnsetValue) {
                return "BINDING_ERROR_BTOS";
            }
            else if (value is bool boolean) {
                if (parameter == null) {
                    throw new Exception("Parameter is null");
                }
                else if (parameter == DependencyProperty.UnsetValue) {
                    return "(Unset Parameter)";
                }
                else {
                    string content = parameter.ToString();
                    int split = content.IndexOf('|');
                    if (split == -1) {
                        throw new Exception("Missing the '|' splitter char in the parameter's content");
                    }
                    else {
                        if (boolean) {
                            return content.JSubstring(0, split);
                        }
                        else {
                            return content.JSubstring(split + 1);
                        }
                    }
                }
            }
            else {
                throw new Exception("Value was not a boolean");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException("Cannot convert back a string to a boolean");
        }
    }
}