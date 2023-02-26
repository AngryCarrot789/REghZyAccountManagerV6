using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace REghZyAccountManagerV6.AttachedProperties {
    public static class AutoSelect {
        public static readonly DependencyProperty FocusAndSelectAllOnLoadedProperty = DependencyProperty.RegisterAttached("FocusAndSelectAllOnLoaded", typeof(bool), typeof(AutoSelect), new PropertyMetadata(false, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is UIElement element) {
                element.Dispatcher.Invoke(() => {
                    element.Focus();
                    if (element is TextBoxBase textbox) {
                        textbox.SelectAll();
                    }
                }, DispatcherPriority.Loaded);
            }
        }

        public static void SetFocusAndSelectAllOnLoaded(DependencyObject element, bool value) {
            element.SetValue(FocusAndSelectAllOnLoadedProperty, value);
        }

        public static bool GetFocusAndSelectAllOnLoaded(DependencyObject element) {
            return (bool) element.GetValue(FocusAndSelectAllOnLoadedProperty);
        }
    }
}