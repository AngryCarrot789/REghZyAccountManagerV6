using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace REghZyAccountManagerV6.Controls {
    public partial class NumberSelector : UserControl {
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum),
                typeof(double),
                typeof(NumberSelector),
                new FrameworkPropertyMetadata(
                    0.0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) => {
                        ((NumberSelector) d).ClampValueToNewMinimum((double) e.NewValue);
                    },
                    (d, value) => {
                        // if ((double) value > ((NumberSelector) d).Maximum) {
                        //     return ((NumberSelector) d).Maximum;
                        // }

                        return value;
                    }));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum),
                typeof(double),
                typeof(NumberSelector),
                new FrameworkPropertyMetadata(
                    10.0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) => {
                        ((NumberSelector) d).ClampValueToNewMaximum((double) e.NewValue);
                    },
                    (d, value) => {
                        // if ((double) value < ((NumberSelector) d).Minimum) {
                        //     return ((NumberSelector) d).Minimum;
                        // }

                        return value;
                    }));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(NumberSelector),
                new FrameworkPropertyMetadata(
                    5.0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValuePropertyChangedCallback,
                    OnValueCoerceValueCallback));

        public static readonly DependencyProperty NormalIncrementProperty =
            DependencyProperty.Register(
                nameof(NormalIncrement),
                typeof(double),
                typeof(NumberSelector),
                new FrameworkPropertyMetadata(1.0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ShiftIncrementProperty =
            DependencyProperty.Register(
                nameof(ShiftIncrement),
                typeof(double),
                typeof(NumberSelector),
                new FrameworkPropertyMetadata(0.2d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty RoundToProperty =
            DependencyProperty.Register(
                nameof(RoundTo),
                typeof(int),
                typeof(NumberSelector),
                new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private static void OnValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            NumberSelector selector = (NumberSelector) d;
            selector.NumberBox.Text = selector.Value.ToString();
        }

        private static object OnValueCoerceValueCallback(DependencyObject d, object val) {
            NumberSelector selector = (NumberSelector) d;
            return Math.Round(selector.GetNewValueClamped((double) val), selector.RoundTo);
        }

        /// <summary>
        /// The minimum allowed value of this number selector
        /// </summary>
        public double Minimum {
            get => (double) this.GetValue(MinimumProperty);
            set => this.SetValue(MinimumProperty, value);
        }

        /// <summary>
        /// The maximum allowed value of this number selector
        /// </summary>
        public double Maximum {
            get => (double) this.GetValue(MaximumProperty);
            set => this.SetValue(MaximumProperty, value);
        }

        /// <summary>
        /// The actual value of this number selector
        /// </summary>
        public double Value {
            get => (double) this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        /// <summary>
        /// The value to add or remove from the value when it is incremented or decremented (1.0 by default)
        /// </summary>
        public double NormalIncrement {
            get => (double) this.GetValue(NormalIncrementProperty);
            set => this.SetValue(NormalIncrementProperty, value);
        }

        /// <summary>
        /// The value to add or remove from the value when it is incremented or decremented while the shift key is pressed (0.2 by default)
        /// </summary>
        public double ShiftIncrement {
            get => (double) this.GetValue(ShiftIncrementProperty);
            set => this.SetValue(ShiftIncrementProperty, value);
        }

        /// <summary>
        /// The number of decimal places to round to (2 by default, giving a precision of 0.01)
        /// </summary>
        public int RoundTo {
            get => (int) this.GetValue(RoundToProperty);
            set => this.SetValue(RoundToProperty, value);
        }

        public NumberSelector() {
            this.InitializeComponent();
        }

        public void Increment() {
            this.Value += ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) ? this.ShiftIncrement : this.NormalIncrement;
        }

        public void Decrement() {
            this.Value -= ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) ? this.ShiftIncrement : this.NormalIncrement;
        }

        public double GetNewValueClamped(double value) {
            double min;
            double max;
            if (value > (max = this.Maximum)) {
                return max;
            }
            else if (value < (min = this.Minimum)) {
                return min;
            }
            else {
                return value;
            }
        }

        public void ClampValueToNewMaximum(double maximum) {
            if (this.Value > maximum) {
                this.Value = maximum;
            }
        }

        public void ClampValueToNewMinimum(double minimum) {
            if (this.Value < minimum) {
                this.Value = minimum;
            }
        }

        private void INCREMENT_Click(object sender, RoutedEventArgs e) {
            this.Increment();
        }

        private void DECREMENT_Click(object sender, RoutedEventArgs e) {
            this.Decrement();
        }

        private void NumberBox_MouseWheel(object sender, MouseWheelEventArgs e) {
            if (e.Delta > 0.0d) {
                this.Increment();
            }
            else if (e.Delta < 0.0d) {
                this.Decrement();
            }
        }
    }
}
