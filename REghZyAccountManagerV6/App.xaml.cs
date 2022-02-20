using System;
using System.Configuration;
using System.Windows;
using REghZyAccountManagerV6.Views.Login;
using REghZyAccountManagerV6.Views.MainView;
using REghZyAccountManagerV6.Views.Settings;

namespace REghZyAccountManagerV6 {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private bool hasInitialised = false;
        private UserSettingsWindow settingsWindow;

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
        }

        internal bool OnTryLogin() {
            if (ServiceLocator.Login.GetPassword() == Environment.UserName) {
                if (!this.hasInitialised) {
                    this.hasInitialised = true;
                    this.settingsWindow = new UserSettingsWindow();
                    ServiceLocator.Settings = this.settingsWindow;
                    this.MainWindow = new MainWindow();
                }

                LoginWindow.Instance.Hide();
                try {
                    this.MainWindow.Show();
                    LoginWindow.Instance.Close();
                }
                catch (Exception e) {
                    this.MainWindow = LoginWindow.Instance;
                    throw new Exception("Failed to load main window: " + e.Message);
                }

                return true;
            }

            throw new Exception("Password incorrect!");
        }
    }
}
