using System;
using System.Windows;
using System.Windows.Controls;

namespace REghZyAccountManagerV6.AttachedProperties {
    public class PasswordAttachedProperty {
        public static readonly DependencyProperty ListenToLengthProperty =
            DependencyProperty.RegisterAttached(
                "ListenToLength",
                typeof(bool),
                typeof(PasswordAttachedProperty),
                new FrameworkPropertyMetadata(false, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue == e.OldValue) {
                return;
            }

            if (d is PasswordBox box) {
                if ((bool) e.NewValue) {
                    // just in case...
                    box.PasswordChanged -= BoxOnPasswordChanged;
                    box.PasswordChanged += BoxOnPasswordChanged;
                }
                else {
                    box.PasswordChanged -= BoxOnPasswordChanged;
                }
            }
            else {
                throw new Exception("DependencyObject is not a password box. It is '" + (d == null ? "null" : d.GetType().Name) + '\'');
            }
        }

        public static readonly DependencyProperty InputLengthProperty =
            DependencyProperty.RegisterAttached(
                "InputLength",
                typeof(int),
                typeof(PasswordAttachedProperty),
                new FrameworkPropertyMetadata(0));

        public static bool GetListenToLength(PasswordBox box) {
            return (bool) box.GetValue(ListenToLengthProperty);
        }

        public static void SetListenToLength(PasswordBox box, bool value) {
            box.SetValue(ListenToLengthProperty, value);
        }

        public static int GetInputLength(PasswordBox box) {
            return (int) box.GetValue(InputLengthProperty);
        }

        public static void SetInputLength(PasswordBox box, int value) {
            box.SetValue(InputLengthProperty, value);
        }

        private static void BoxOnPasswordChanged(object sender, RoutedEventArgs e) {
            SetInputLength((PasswordBox) sender, ((PasswordBox) sender).SecurePassword.Length);
        }
    }
}