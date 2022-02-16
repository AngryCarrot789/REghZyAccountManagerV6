using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace REghZyAccountManagerV6.AttachedProperties {
    public static class SelectAllAttachedProperty {
        public static readonly DependencyProperty UseSelectAllProperty =
            DependencyProperty.RegisterAttached(
                "UseSelectAll",
                typeof(bool),
                typeof(SelectAllAttachedProperty),
                new FrameworkPropertyMetadata(false, (d, e) => {
                    if (d is ButtonBase button) {
                        button.Click -= DeselectAllClick;
                        button.Click -= SelectAllClick;
                        button.Click += SelectAllClick;
                    }
                }));

        public static readonly DependencyProperty UseDeselectAllProperty =
            DependencyProperty.RegisterAttached(
                "UseDeselectAll",
                typeof(bool),
                typeof(SelectAllAttachedProperty),
                new FrameworkPropertyMetadata(false, (d, e) => {
                    if (d is ButtonBase button) {
                        button.Click -= SelectAllClick;
                        button.Click -= DeselectAllClick;
                        button.Click += DeselectAllClick;
                    }
                }));

        public static bool GetUseSelectAll(ButtonBase button) {
            return (bool) button.GetValue(UseSelectAllProperty);
        }

        public static void SetUseSelectAll(ButtonBase button, bool value) {
            button.SetValue(UseSelectAllProperty, value);
        }

        public static bool GetUseDeselectAll(ButtonBase button) {
            return (bool) button.GetValue(UseDeselectAllProperty);
        }

        public static void SetUseDeselectAll(ButtonBase button, bool value) {
            button.SetValue(UseDeselectAllProperty, value);
        }

        private static void SelectAllClick(object sender, RoutedEventArgs e) {
            ButtonBase button = (ButtonBase) e.Source;
            if (button.Parent is Panel panel) {
                foreach (UIElement element in panel.Children) {
                    if (element is CheckBox checkBox) {
                        checkBox.IsChecked = true;
                    }
                }
            }
            else if (button.Parent is ItemsControl itemsControl) {
                ItemContainerGenerator generator = itemsControl.ItemContainerGenerator;
                foreach (object elementObj in itemsControl.Items) {
                    CheckBox checkBox;
                    if (elementObj is CheckBox check) {
                        checkBox = check;
                    }
                    else if (generator.ContainerFromItem(elementObj) is CheckBox generated) {
                        checkBox = generated;
                    }
                    else {
                        continue;
                    }

                    checkBox.IsChecked = true;
                }
            }
        }

        private static void DeselectAllClick(object sender, RoutedEventArgs e) {
            ButtonBase button = (ButtonBase) e.Source;
            if (button.Parent is Panel panel) {
                foreach (UIElement element in panel.Children) {
                    if (element is CheckBox checkBox) {
                        checkBox.IsChecked = false;
                    }
                }
            }
            else if (button.Parent is ItemsControl itemsControl) {
                ItemContainerGenerator generator = itemsControl.ItemContainerGenerator;
                foreach (object elementObj in itemsControl.Items) {
                    CheckBox checkBox;
                    if (elementObj is CheckBox check) {
                        checkBox = check;
                    }
                    else if (generator.ContainerFromItem(elementObj) is CheckBox generated) {
                        checkBox = generated;
                    }
                    else {
                        continue;
                    }

                    checkBox.IsChecked = false;
                }
            }
        }
    }
}
