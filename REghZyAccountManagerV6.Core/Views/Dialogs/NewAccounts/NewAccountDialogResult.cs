namespace REghZyAccountManagerV6.Core.Views.Dialogs.NewAccounts {
    public class NewAccountDialogResult : BaseDialogResult {
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DateOfBirth { get; set; }
        public string SecurityInfo { get; set; }

        public NewAccountDialogResult() {
        }

        public NewAccountDialogResult(bool isSuccess) : base(isSuccess) {
        }
    }
}