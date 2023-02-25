using REghZyAccountManagerV6.Core.Views.Dialogs.NewAccounts;

namespace REghZyAccountManagerV6.Views.Dialogs.NewAccount {
    public class NewUserDialogService : INewAccountDialogService {
        public NewAccountDialogResult ShowNewAccountDialog() {
            NewAccountDialog window = new NewAccountDialog();
            if (window.ShowDialog() != true) {
                return new NewAccountDialogResult(false);
            }

            NewAccountViewModel vm = window.ViewModel;
            return new NewAccountDialogResult() {
                AccountName = vm.AccountName,
                Email = vm.Email,
                Username = vm.Username,
                Password = vm.Password,
                DateOfBirth = vm.DateOfBirth,
                SecurityInfo = vm.SecurityInfo
            };
        }
    }
}