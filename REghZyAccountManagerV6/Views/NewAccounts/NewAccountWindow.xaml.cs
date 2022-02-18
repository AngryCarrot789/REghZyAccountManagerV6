using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using REghZyAccountManagerV6.Accounting;
using REghZyAccountManagerV6.Utils;

namespace REghZyAccountManagerV6.Views.NewAccounts {
    public partial class NewAccountWindow : Window, INewAccount {
        private bool confirmed;

        public bool IsAccountValid { get; private set; }

        public AccountViewModel Model {
            get => (AccountViewModel) this.DataContext;
            set => this.DataContext = value;
        }

        public NewAccountWindow() {
            InitializeComponent();
        }

        public bool IsOpen => this.Visibility == Visibility.Visible;

        public void OpenView() {
            this.IsAccountValid = false;
            this.confirmed = false;
            this.Model = new AccountViewModel();
            this.Model.AccountName = "New Account";
            this.selectionIndex = 0;
            SelectNext();

            // the functionality relies on a dialog blocking the rest of the app
            this.ShowDialog();
        }

        private int selectionIndex = 0;

        private void SelectNext() {
            if (this.selectionIndex == 0) {
                this.BoxAccountName.Focus();
                this.BoxAccountName.SelectAll();
            }
            else if (this.selectionIndex == 1) {
                this.BoxEmail.Focus();
                this.BoxEmail.SelectAll();
            }
            else if (this.selectionIndex == 2) {
                this.BoxUsername.Focus();
                this.BoxUsername.SelectAll();
            }
            else if (this.selectionIndex == 3) {
                this.BoxPassword.Focus();
                this.BoxPassword.SelectAll();
            }
            else if (this.selectionIndex == 4) {
                this.BoxDOB.Focus();
                this.BoxDOB.SelectAll();
            }
            else if (this.selectionIndex == 5) {
                this.BoxSecurityInfo.Focus();
                this.BoxSecurityInfo.SelectAll();
            }
            else {
                this.selectionIndex = 0;
                SelectNext();
                return;
            }

            selectionIndex++;
        }

        public void CloseView() {
            this.Hide();
        }

        public AccountViewModel Account {
            get => this.Model;
            set => this.Model = value;
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            // base.OnKeyDown(e);
            switch (e.Key) {
                case Key.Tab:
                    SelectNext();
                    e.Handled = true;
                    return;
                case Key.Return: this.confirmed = true; break;
                case Key.Escape: this.confirmed = false; break;
                default: return;
            }

            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e) {
            // This will be true if the application is shutting down... at least that's the hope
            if (!this.IsOpen || this.Model == null) {
                base.OnClosing(e);
                return;
            }

            base.OnClosing(e);
            if (this.confirmed) {
                try {
                    VerifyAccount();
                }
                catch (Exception ex) {
                    MessageBox.Show("The account cannot be created. Reason: " + ex.Message);
                    this.confirmed = false;
                }

                try {
                    this.Model.FilePath = FileHelper.GetValidAccountFileName(this.Model);
                }
                catch (Exception ex) {
                    MessageBox.Show("Failed to get a valid file name for the account. Reason: " + ex.Message);
                    this.confirmed = false;
                }
            }

            this.IsAccountValid = this.confirmed;
            e.Cancel = true;
            Hide();
        }

        private void VerifyAccount() {
            if (this.Model == null) {
                throw new Exception("Error: 0x0001 (Invalid Model)");
            }

            if (string.IsNullOrWhiteSpace(this.Model.AccountName)) {
                throw new Exception("The account name cannot be empty or consist of only whitespaces");
            }

            // chosen to hopefully prevent over-sized file paths
            if (this.Model.AccountName.Length > 60) {
                throw new Exception("The account name cannot be more than 60 characters");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            this.confirmed = false;
            Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e) {
            this.confirmed = true;
            Close();
        }
    }
}