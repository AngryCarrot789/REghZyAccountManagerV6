using System;
using System.Threading.Tasks;
using REghZyAccountManagerV6.Core;

namespace REghZyAccountManagerV6.Views.Dialogs.Login {
    public class LoginViewModel : BaseConfirmableDialogViewModel {
        public Predicate<string> PasswordVerifier { get; }

        public new ILoginDialog Dialog => (ILoginDialog) base.Dialog;

        public LoginViewModel(ILoginDialog dialog, Predicate<string> passwordVerifier) : base(dialog) {
            this.PasswordVerifier = passwordVerifier ?? throw new ArgumentNullException(nameof(passwordVerifier));
        }

        public override async Task<bool> CanConfirm() {
            if (this.PasswordVerifier(this.Dialog.Password)) {
                return true;
            }
            else {
                await IoC.MessageDialogs.ShowMessageAsync("Incorrect password", "You have entered an incorrect password. Try again");
                return false;
            }
        }
    }
}
