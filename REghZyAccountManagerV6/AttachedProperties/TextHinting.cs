using System.Windows;
using System.Windows.Controls;

namespace REghZyAccountManagerV6.AttachedProperties {
    public static class TextBoxHinting {
        public static readonly DependencyProperty ShowWhenFocusedProperty =
            DependencyProperty.RegisterAttached(
                "ShowWhenFocused",
                typeof(bool),
                typeof(TextBoxHinting),
                new FrameworkPropertyMetadata(false));

        public static void SetShowWhenFocused(TextBox textBox, bool value) {
            textBox.SetValue(ShowWhenFocusedProperty, value);
        }

        public static bool GetShowWhenFocused(TextBox textBox) {
            return (bool) textBox.GetValue(ShowWhenFocusedProperty);
        }
    }
}
