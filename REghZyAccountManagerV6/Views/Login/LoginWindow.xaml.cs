using System;
using System.Windows;

namespace REghZyAccountManagerV6.Views.Login {
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, ILoginView {
        public static LoginWindow Instance { get; private set; }

        public LoginWindow() {
            InitializeComponent();
            Instance = this;
            ServiceLocator.Login = this;
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            this.InputBox.Focus();
            this.InputBox.SelectAll();
        }

        public string GetPassword() {
            return this.InputBox.Password;
        }

        public void ClearPassword() {
            this.InputBox.Password = "";
        }
    }
}
