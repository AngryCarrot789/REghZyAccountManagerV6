using System;

namespace REghZyAccountManagerV6.Views.Dialogs.Login {
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginDialog : BaseDialog, ILoginDialog {
        public string Password => this.InputBox.Password;

        public LoginViewModel ViewModel => (LoginViewModel) this.DataContext;

        public LoginDialog() {
            this.InitializeComponent();
            // cheap ass password :-)
            this.DataContext = new LoginViewModel(this, x => x == Environment.UserName);
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            this.InputBox.Focus();
            this.InputBox.SelectAll();
        }
    }
}
