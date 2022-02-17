using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace REghZyAccountManagerV6.Utils {
    public static class HorizontalScrolling {
        public static readonly DependencyProperty UseHorizontalScrollingProperty =
            DependencyProperty.RegisterAttached(
                "UseHorizontalScrolling",
                typeof(bool),
                typeof(HorizontalScrolling),
                new PropertyMetadata(false, UseHorizontalScrollingChangedCallback));

        public static readonly DependencyProperty HorizontalScrollAmountProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalScrollAmount",
                typeof(int),
                typeof(HorizontalScrolling),
                new PropertyMetadata(1, (d, e) => { }, HorizontalScrollCoerceValueCallback));

        private static object HorizontalScrollCoerceValueCallback(DependencyObject d, object value) {
            if (value == null || value == DependencyProperty.UnsetValue) {
                return 1;
            }
            else if (value is int i) {
                return Math.Max(0, i);
            }
            else {
                return 1;
            }
        }

        private static void UseHorizontalScrollingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is UIElement element) {
                if ((bool) e.NewValue)
                    element.PreviewMouseWheel += OnPreviewMouseWheel;
                else
                    element.PreviewMouseWheel -= OnPreviewMouseWheel;
            }
            else {
                throw new Exception("Attached property must be used with UIElement.");
            }
        }

        public static void SetUseHorizontalScrolling(DependencyObject element, bool value) => element.SetValue(UseHorizontalScrollingProperty, value);

        public static bool GetUseHorizontalScrolling(DependencyObject element) => (bool) element.GetValue(UseHorizontalScrollingProperty);

        public static void SetHorizontalScrollAmount(DependencyObject element, int value) => element.SetValue(HorizontalScrollAmountProperty, value);

        public static int GetHorizontalScrollAmount(DependencyObject element) => (int) element.GetValue(HorizontalScrollAmountProperty);

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args) {
            if (Keyboard.Modifiers != ModifierKeys.Shift) {
                return;
            }

            ScrollViewer scrollViewer = ((UIElement) sender).FindDescendant<ScrollViewer>();
            if (scrollViewer == null) {
                throw new Exception($"Type '{sender.GetType()}' does not have a ScrollViewer in it's descendants");
            }

            for (int i = 0, count = GetHorizontalScrollAmount(scrollViewer); i < count; i++) {
                if (args.Delta < 0) {
                    scrollViewer.LineRight();
                }
                else {
                    scrollViewer.LineLeft();
                }
            }

            args.Handled = true;
        }

        private static T FindDescendant<T>(this DependencyObject d) where T : DependencyObject {
            if (d == null) {
                return null;
            }

            int childCount = VisualTreeHelper.GetChildrenCount(d);
            for (int i = 0; i < childCount; i++) {
                DependencyObject child = VisualTreeHelper.GetChild(d, i);
                T result = child as T ?? FindDescendant<T>(child);
                if (result != null) {
                    return result;
                }
            }

            return null;
        }
    }
}