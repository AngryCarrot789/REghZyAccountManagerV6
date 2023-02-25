using REghZyAccountManagerV6.Core;
using REghZyAccountManagerV6.Core.Views.Dialogs;

namespace REghZyAccountManagerV6.Views.Dialogs.NewAccount {
    public class NewAccountViewModel : BaseConfirmableDialogViewModel {
        private string accountName;
        private string email;
        private string username;
        private string password;
        private string dateOfBirth;
        private string securityInfo;

        public string AccountName {
            get => this.accountName;
            set => this.RaisePropertyChanged(ref this.accountName, value);
        }

        public string Email {
            get => this.email;
            set => this.RaisePropertyChanged(ref this.email, value);
        }

        public string Username {
            get => this.username;
            set => this.RaisePropertyChanged(ref this.username, value);
        }

        public string Password {
            get => this.password;
            set => this.RaisePropertyChanged(ref this.password, value);
        }

        public string DateOfBirth {
            get => this.dateOfBirth;
            set => this.RaisePropertyChanged(ref this.dateOfBirth, value);
        }

        public string SecurityInfo {
            get => this.securityInfo;
            set => this.RaisePropertyChanged(ref this.securityInfo, value);
        }

        public RelayCommandParam<string> CopyToClipboardCommand { get; }
        public RelayCommandParam<string> PasteFromClipboardCommand { get; }

        public NewAccountViewModel(IDialog dialog) : base(dialog) {
            this.CopyToClipboardCommand = new RelayCommandParam<string>(this.CopyToClipboardAction);
            this.PasteFromClipboardCommand = new RelayCommandParam<string>(this.PasteFromClipboardAction);
        }

        private void CopyToClipboardAction(string type) {
            string clipboard = null;
            switch (type) {
                case "0": clipboard = this.AccountName; break;
                case "1": clipboard = this.Email; break;
                case "2": clipboard = this.Username; break;
                case "3": clipboard = this.Password; break;
                case "4": clipboard = this.DateOfBirth; break;
                case "5": clipboard = this.SecurityInfo; break;
            }

            if (clipboard != null) {
                IoC.Clipboard.ReadableText = clipboard;
            }
        }

        private void PasteFromClipboardAction(string type) {
            string clipboard = IoC.Clipboard.ReadableText;
            if (clipboard == null) {
                return;
            }

            switch (type) {
                case "0": this.AccountName = clipboard; break;
                case "1": this.Email = clipboard; break;
                case "2": this.Username = clipboard; break;
                case "3": this.Password = clipboard; break;
                case "4": this.DateOfBirth = clipboard; break;
                case "5": this.SecurityInfo = clipboard; break;
            }
        }
    }
}
