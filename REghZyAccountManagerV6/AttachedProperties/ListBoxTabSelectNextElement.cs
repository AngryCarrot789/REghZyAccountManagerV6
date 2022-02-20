using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace REghZyAccountManagerV6.AttachedProperties {
    public static class ListBoxTabSelectNextElement {
        public static readonly DependencyProperty UseProperty =
            DependencyProperty.RegisterAttached(
                "Use", 
                typeof(bool), 
                typeof(ListBoxTabSelectNextElement), 
                new FrameworkPropertyMetadata(
                    false, 
                    (d, e) => {
                        if (d is Selector listBox) {
                            listBox.PreviewKeyDown += ListBox_PreviewKeyDown;
                        }
                    }));

        public static readonly DependencyProperty ReverseProperty =
            DependencyProperty.RegisterAttached(
                "Reverse", 
                typeof(bool), 
                typeof(ListBoxTabSelectNextElement), 
                new FrameworkPropertyMetadata(false));

        public static bool GetUse(Selector control) {
            return (bool) control.GetValue(UseProperty);
        }

        public static void SetUse(Selector control, bool value) {
            control.SetValue(UseProperty, value);
        }

        public static bool GetReverse(Selector control) {
            return (bool) control.GetValue(ReverseProperty);
        }

        public static void SetReverse(Selector control, bool value) {
            control.SetValue(ReverseProperty, value);
        }

        private static void ListBox_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Tab) {
                return;
            }

            if (sender is Selector control) {
                int index = control.SelectedIndex;
                if (GetReverse(control)) {
                    index--;
                }
                else {
                    index++;
                }

                if (index >= control.Items.Count) {
                    index = 0;
                }
                else if (index < 0) {
                    index = control.Items.Count - 1;
                }

                control.SelectedIndex = index;
                e.Handled = true;
            }
        }
    }
}
