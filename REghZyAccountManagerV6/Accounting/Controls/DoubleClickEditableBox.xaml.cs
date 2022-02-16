using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace REghZyAccountManagerV6.Accounting.Controls {
    /// <summary>
    /// Interaction logic for DoubleClickEditableBox.xaml
    /// </summary>
    public partial class DoubleClickEditableBox : UserControl {
        private bool isHandlingClick;
        private bool ignoreLostFocus;

        private const int FLAG_EDITOR = 0b0001;
        private const int FLAG_PREVIEW = 0b0010;

        private bool flag1;
        private bool flag2;

        public bool EditMode {
            get => this.Editor.IsFocused;
            set {
                if (value) {
                    this.Preview.Visibility = Visibility.Collapsed;
                    this.Editor.Visibility = Visibility.Visible;
                    this.Editor.Focus();
                    // this.Editor.SelectAll();
                }
                else {
                    this.Focus();
                    this.Editor.Visibility = Visibility.Collapsed;
                    this.Preview.Visibility = Visibility.Visible;
                }
            }
        }

        public DoubleClickEditableBox() {
            InitializeComponent();
        }

        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e) {
            base.OnPreviewMouseDoubleClick(e);
            if (this.EditMode) {
                return;
            }

            this.EditMode = true;
            e.Handled = true;
        }

        private void Editor_LostFocus(object sender, RoutedEventArgs e) {
            if (this.ignoreLostFocus) {
                return;
            }

            this.ignoreLostFocus = true;
            this.EditMode = false;
            this.ignoreLostFocus = false;
        }

        private bool lastKeyWasEnter;

        private void Editor_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                if (this.lastKeyWasEnter) {
                    this.lastKeyWasEnter = false;
                    // SplitInto2();
                }
                else {
                    this.lastKeyWasEnter = true;
                }
            }
            else {
                if (e.Key == Key.Escape) {
                    this.EditMode = false;
                }
            }
        }

        private void SplitInto2() {
            if (ViewModelLocator.AccountCollection.SelectedAccount != null) {
                ViewModelLocator.AccountCollection.SelectedAccount.ExtraInfo.SplitSelectedInto2(this.Editor.CaretIndex);
            }
        }
    }
}
