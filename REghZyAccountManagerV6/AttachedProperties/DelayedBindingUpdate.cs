using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace REghZyAccountManagerV6.AttachedProperties {
    public static class DelayedBindingUpdate {
        private static readonly Dictionary<TextBoxBase, Timer> TIMER_MAP = new Dictionary<TextBoxBase, Timer>();

        public static readonly DependencyProperty UpdateDelayForInputChangeProperty =
            DependencyProperty.RegisterAttached(
                "UpdateDelayForInputChange",
                typeof(TimeSpan),
                typeof(DelayedBindingUpdate),
                new FrameworkPropertyMetadata(TimeSpan.Zero, OnUpdateDelayForInputChangeChanged));

        public static void SetUpdateDelayForInputChange(TextBoxBase element, TimeSpan value) {
            element.SetValue(UpdateDelayForInputChangeProperty, value);
        }

        public static TimeSpan GetUpdateDelayForInputChange(TextBoxBase element) {
            return (TimeSpan) element.GetValue(UpdateDelayForInputChangeProperty);
        }

        private static void OnUpdateDelayForInputChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is TextBoxBase box) {
                box.TextChanged -= OnTextChanged;
                box.Unloaded -= TextBoxOnUnloaded;
                if (e.NewValue is TimeSpan delay && delay.Ticks > 0) {
                    box.Unloaded += TextBoxOnUnloaded;
                    box.TextChanged += OnTextChanged;
                    if (!TIMER_MAP.TryGetValue(box, out Timer timer)) {
                        TIMER_MAP[box] = timer = new Timer(delay.TotalMilliseconds);
                    }

                    timer.Elapsed += (x, args) => {
                        Application.Current.Dispatcher.Invoke(() => {
                            BindingExpression expression = box.GetBindingExpression(TextBox.TextProperty);
                            if (expression != null) {
                                expression.UpdateSource();
                            }
                        });

                        timer.Stop();
                    };
                }
                else {
                    if (TIMER_MAP.TryGetValue(box, out Timer timer)) {
                        timer.Dispose();
                        TIMER_MAP.Remove(box);
                    }
                }
            }
        }

        private static void TextBoxOnUnloaded(object sender, RoutedEventArgs e) {
            if (sender is TextBoxBase textBox && TIMER_MAP.TryGetValue(textBox, out Timer timer)) {
                timer.Dispose();
                TIMER_MAP.Remove(textBox);
            }
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e) {
            if (sender is TextBoxBase box) {
                if (TIMER_MAP.TryGetValue(box, out Timer timer)) {
                    timer.Stop();
                    timer.Start();
                }
            }
        }
    }
}