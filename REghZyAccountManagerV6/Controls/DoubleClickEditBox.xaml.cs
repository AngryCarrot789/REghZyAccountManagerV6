using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using REghZy.MVVM.Commands;

namespace REghZyAccountManagerV6.Controls {
    public partial class DoubleClickEditBox : UserControl {
        private bool ignoreLostFocus;
        private string preEditText;

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(
                nameof(IsEditing),
                typeof(bool),
                typeof(DoubleClickEditBox),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) => {
                        if (e.OldValue == e.NewValue) {
                            return;
                        }

                        ((DoubleClickEditBox) d).OnIsEditingChanged((bool)e.OldValue, (bool)e.NewValue);
                    },
                    (obj, value) => value,
                    true,
                    UpdateSourceTrigger.PropertyChanged));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(DoubleClickEditBox),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) => { },
                    (obj, value) => value ?? "",
                    true,
                    UpdateSourceTrigger.PropertyChanged));

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(
                nameof(TextWrapping),
                typeof(object),
                typeof(DoubleClickEditBox),
                new PropertyMetadata(TextWrapping.NoWrap));

        private void OnIsEditingChanged(bool oldValue, bool newValue) {
            if (newValue) {
                this.PART_Preview.Visibility = Visibility.Collapsed;
                this.PART_Editor.Visibility = Visibility.Visible;
                this.PART_Editor.Focus();
                this.PART_Editor.SelectAll();
                this.preEditText = this.Text;
            }
            else {
                this.Focus();
                this.PART_Editor.Visibility = Visibility.Collapsed;
                this.PART_Preview.Visibility = Visibility.Visible;
            }
        }

        public bool IsEditing {
            get => (bool) GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public string Text {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public TextWrapping TextWrapping {
            get => (TextWrapping) GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        public ICommand EnableEditing { get; set; }

        public ICommand DisableEditing { get; set; }

        public DoubleClickEditBox() {
            InitializeComponent();
            this.EnableEditing = new RelayCommand(()=> {
                if (this.IsEditing) {
                    return;
                }

                this.IsEditing = true;
            });

            this.DisableEditing = new RelayCommand(()=> {
                if (this.IsEditing) {
                    this.IsEditing = false;
                }
            });
        }

        private void Editor_OnLostFocus(object sender, RoutedEventArgs e) {
            if (this.ignoreLostFocus) {
                return;
            }

            this.ignoreLostFocus = true;
            this.IsEditing = false;
            this.ignoreLostFocus = false;
        }

        private void Editor_OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter || e.Key == Key.Escape) {
                if (e.Key == Key.Escape) {
                    if (this.preEditText != null && !this.preEditText.Equals(this.Text)) {
                        this.Text = this.preEditText;
                    }
                }

                this.preEditText = null;
                this.IsEditing = false;
            }
        }

        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e) {
            base.OnPreviewMouseDoubleClick(e);
            if (e.ChangedButton != MouseButton.Left) {
                return;
            }

            if (this.IsEditing) {
                return;
            }

            this.IsEditing = true;
            e.Handled = true;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.F2 || (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)) {
                this.IsEditing = true;
                e.Handled = true;
            }
        }
    }
}