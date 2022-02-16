using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using REghZyAccountManagerV6.Accounting;

namespace REghZyAccountManagerV6.Converters {
    public class ModificationColourConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null || value == DependencyProperty.UnsetValue) {
                return Colors.Transparent;
            }
            else if (value is AccountViewModel account) {
                if (!account.HasBeenModified) {
                    return Colors.Transparent;
                }

                if (string.IsNullOrEmpty(account.FilePath) || !File.Exists(account.FilePath)) {
                    // brand new account, probably
                    return Colors.Peru;
                }
                else if (File.Exists(account.FilePath)) {
                    // existing account
                    return Colors.Transparent;
                }
                else {
                    return Colors.DarkRed;
                }
            }
            else {
                // debug/error :-)
                return Colors.Yellow;
                // throw new Exception("Type was not an AccountViewModel: " + value.GetType());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
