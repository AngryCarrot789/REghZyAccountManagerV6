using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace REghZyAccountManagerV6.Core.Accounting {
    public class AccountViewModel : BaseViewModel {
        private bool hasBeenModified;
        private string accountName;
        private string email;
        private string username;
        private string password;
        private string dateOfBirth;
        private string securityInfo;

        public bool HasBeenModified {
            get => this.hasBeenModified;
            set => this.RaisePropertyChanged(ref this.hasBeenModified, value);
        }

        public string AccountName {
            get => this.accountName;
            set => this.RaisePropertyChanged(ref this.accountName, value, this.MarkModified);
        }

        public string Email {
            get => this.email;
            set => this.RaisePropertyChanged(ref this.email, value, this.MarkModified);
        }

        public string Username {
            get => this.username;
            set => this.RaisePropertyChanged(ref this.username, value, this.MarkModified);
        }

        // tbh... who cares if its in plain text :-)
        // its gonna be stored in plain text in a password box so :)
        public string Password {
            get => this.password;
            set => this.RaisePropertyChanged(ref this.password, value, this.MarkModified);
        }

        public string DateOfBirth {
            get => this.dateOfBirth;
            set => this.RaisePropertyChanged(ref this.dateOfBirth, value, this.MarkModified);
        }

        public string SecurityInfo {
            get => this.securityInfo;
            set => this.RaisePropertyChanged(ref this.securityInfo, value, this.MarkModified);
        }

        private string customInfo;
        public string CustomInfo {
            get => this.customInfo;
            set => this.RaisePropertyChanged(ref this.customInfo, value, this.MarkModified);
        }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand PasteFromClipboardCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand SelectCommand { get; }

        public AccountViewModel() {
            this.CopyToClipboardCommand = new RelayCommandParam<string>(this.CopyClipboardData);
            this.PasteFromClipboardCommand = new RelayCommandParam<string>(this.PasteToClipboard);
            this.DeleteCommand = new RelayCommand(() => {
                ViewModelLocator.AccountCollection.DeleteAccount(this);
            });

            this.SelectCommand = new RelayCommand(() => {
                ViewModelLocator.AccountCollection.SelectedAccount = this;
                ViewModelLocator.AccountPanel.IsEditorOpen = true;
            });
        }

        public AccountViewModel(string accountName, string email, string username, string password, string dateOfBirth, string securityInfo, string customInfo) : this() {
            this.accountName = accountName;
            this.email = email;
            this.username = username;
            this.password = password;
            this.dateOfBirth = dateOfBirth;
            this.securityInfo = securityInfo;
            this.customInfo = customInfo;
        }

        /// <summary>
        /// Marks this account as being modified, meaning it requires saving
        /// </summary>
        public void MarkModified() {
            this.HasBeenModified = true;
            ViewModelLocator.AccountCollection.OnAccountModifiedChanged(this);
        }

        private void CopyClipboardData(string type) {
            if (type == null) {
                return;
            }

            string data;
            switch (type) {
                case "0": data = this.accountName;  break;
                case "1": data = this.email;        break;
                case "2": data = this.username;     break;
                case "3": data = this.password;     break;
                case "4": data = this.dateOfBirth;  break;
                case "5": data = this.securityInfo; break;
                case "6": data = this.customInfo;   break;
                default:
                    throw new Exception("Unknown type to copy to clipboard: " + type);
            }

            if (string.IsNullOrEmpty(data)) {
                return;
            }

            IoC.Clipboard.ReadableText = data;
        }

        private void PasteToClipboard(string type) {
            if (type == null) {
                return;
            }

            string data = IoC.Clipboard.ReadableText;
            if (string.IsNullOrEmpty(data)) {
                return;
            }

            switch (type) {
                case "0": this.AccountName  = data; break;
                case "1": this.Email        = data; break;
                case "2": this.Username     = data; break;
                case "3": this.Password     = data; break;
                case "4": this.DateOfBirth  = data; break;
                case "5": this.SecurityInfo = data; break;
                case "6": this.CustomInfo   = data; break;
                default:
                    throw new Exception("Unknown type to copy to clipboard: " + type);
            }
        }
    }
}