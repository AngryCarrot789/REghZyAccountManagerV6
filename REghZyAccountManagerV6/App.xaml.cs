using System;
using System.Windows;
using REghZyAccountManagerV6.Views.Login;
using REghZyAccountManagerV6.Views.MainView;

namespace REghZyAccountManagerV6 {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
        }

        internal bool OnTryLogin() {
            if (ServiceLocator.Login.GetPassword() == Environment.UserName) {
                this.MainWindow = new MainWindow();
                LoginWindow.INSTANCE.Hide();
                try {
                    this.MainWindow.Show();
                }
                catch(Exception e) {
                    this.MainWindow = LoginWindow.INSTANCE;
                    throw new Exception("Failed to load main window: " + e.Message);
                }
                finally {
                    LoginWindow.INSTANCE.Close();
                }

                return true;
            }

            throw new Exception("Password incorrect!");
        }
    }
}
