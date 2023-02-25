using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace REghZyAccountManagerV6.Controls {
    public class EditableTextBlock : Control {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(EditableTextBlock),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged, (d,x) => x ?? ""));

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(
                "IsEditing",
                typeof(bool),
                typeof(EditableTextBlock),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsEditingChanged));

        public string Text {
            get => (string) this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        public bool IsEditing {
            get => (bool) this.GetValue(IsEditingProperty);
            set => this.SetValue(IsEditingProperty, value);
        }

        private TextBlock textBlock;
        private TextBox textBox;

        public EditableTextBlock() {
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (ReferenceEquals(e.NewValue, e.OldValue) || e.NewValue == e.OldValue) {
                return;
            }

            if (d is EditableTextBlock block) {

            }
        }

        private static void OnIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (ReferenceEquals(e.NewValue, e.OldValue) || e.NewValue == e.OldValue) {
                return;
            }

            if (d is EditableTextBlock block) {

            }
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            if (this.textBlock != null) {
                this.textBlock.MouseLeftButtonDown -= this.OnTextBlockOnMouseLeftButtonDown;
            }

            this.textBlock = this.GetTemplateChild("PART_TextBlock") as TextBlock ?? throw new Exception("Missing PART_TextBlock from template");
            this.textBox = this.GetTemplateChild("PART_TextBox") as TextBox ?? throw new Exception("Missing PART_TextBox from template");

            if (this.textBlock != null) {
                this.textBlock.MouseLeftButtonDown += this.OnTextBlockOnMouseLeftButtonDown;
            }
        }

        private void OnTextBlockOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            
        }
    }
}