using System;
using System.Windows;
using System.Windows.Input;
using REghZy.MVVM.Commands;
using REghZy.MVVM.ViewModels;

namespace REghZyAccountManagerV6.Views.Login {
    public class LoginViewModel : BaseViewModel {
        public ICommand LoginCommand { get; }

        public LoginViewModel() {
            this.LoginCommand = new RelayCommand(this.TryLogin);
        }

        private void TryLogin() {
            try {
                ((App) Application.Current).OnTryLogin();
            }
            catch(Exception e) {
                MessageBox.Show(e.Message, "Error logging in");
            }
        }
    }
}
