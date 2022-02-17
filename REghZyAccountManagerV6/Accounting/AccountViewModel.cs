using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using REghZy.MVVM.Commands;
using REghZy.MVVM.ViewModels;

namespace REghZyAccountManagerV6.Accounting {
    public class AccountViewModel : BaseViewModel {
        private bool hasBeenModified;
        private string filePath;
        private string accountName;
        private string email;
        private string username;
        private string password;
        private string dateOfBirth;
        private string securityInfo;

        public bool HasBeenModified {
            get => this.hasBeenModified;
            set => RaisePropertyChanged(ref this.hasBeenModified, value);
        }

        public string FilePath {
            get => this.filePath;
            set => RaisePropertyChangedWithCallback(ref this.filePath, value, MarkModified);
        }

        public string AccountName {
            get => this.accountName;
            set => RaisePropertyChangedWithCallback(ref this.accountName, value, MarkModified);
        }

        public string Email {
            get => this.email;
            set => RaisePropertyChangedWithCallback(ref this.email, value, MarkModified);
        }

        public string Username {
            get => this.username;
            set => RaisePropertyChangedWithCallback(ref this.username, value, MarkModified);
        }

        // tbh... who cares if its in plain text :-)
        // its gonna be stored in plain text in a password box so :)
        public string Password {
            get => this.password;
            set => RaisePropertyChangedWithCallback(ref this.password, value, MarkModified);
        }

        public string DateOfBirth {
            get => this.dateOfBirth;
            set => RaisePropertyChangedWithCallback(ref this.dateOfBirth, value, MarkModified);
        }

        public string SecurityInfo {
            get => this.securityInfo;
            set => RaisePropertyChangedWithCallback(ref this.securityInfo, value, MarkModified);
        }

        public ExtraInfoViewModel ExtraInfo { get; }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand PasteFromClipboardCommand { get; }


        public ICommand DeleteCommand { get; }

        public ICommand SelectCommand { get; }

        public AccountViewModel(IEnumerable<ExtraData> extraInfo) {
            this.ExtraInfo = new ExtraInfoViewModel(this, extraInfo);
            this.CopyToClipboardCommand = new RelayCommandParam<string>(CopyClipboardData);
            this.PasteFromClipboardCommand = new RelayCommandParam<string>(PasteToClipboard);
            this.DeleteCommand = new RelayCommand(() => {
                ViewModelLocator.AccountCollection.DeleteAccount(this);
            });

            this.SelectCommand = new RelayCommand(() => {
                ViewModelLocator.AccountCollection.SelectedAccount = this;
                ViewModelLocator.AccountPanel.IsEditorOpen = true;
            });
        }

        public AccountViewModel() : this(new List<ExtraData>()) {

        }

        public AccountViewModel(IEnumerable<string> list) : this(list == null ? new List<ExtraData>() : list.Select(d => new ExtraData(d))) {

        }

        public AccountViewModel(string filePath, string accountName, string email, string username, string password, string dateOfBirth, string securityInfo, IEnumerable<string> extraData) : this(extraData ?? new List<string>()) {
            this.filePath = filePath;
            this.accountName = accountName;
            this.email = email;
            this.username = username;
            this.password = password;
            this.dateOfBirth = dateOfBirth;
            this.securityInfo = securityInfo;
        }

        /// <summary>
        /// Marks this account as being modified, meaning it requires saving
        /// </summary>
        public void MarkModified() {
            this.HasBeenModified = true;
        }

        public FileInfo GetFileInfo() {
            return new FileInfo(this.filePath);
        }

        private void CopyClipboardData(string type) {
            if (type == null) {
                return;
            }

            string data;
            switch (type) {
                case "an":
                    data = this.accountName;
                    break;
                case "e":
                    data = this.email;
                    break;
                case "u":
                    data = this.username;
                    break;
                case "p":
                    data = this.password;
                    break;
                case "dob":
                    data = this.dateOfBirth;
                    break;
                case "si":
                    data = this.securityInfo;
                    break;
                default:
                    throw new Exception("Unknown type to copy to clipboard: " + type);
            }

            if (string.IsNullOrEmpty(data)) {
                return;
            }

            Clipboard.SetText(data);
        }

        private void PasteToClipboard(string type) {
            if (type == null) {
                return;
            }

            string data = Clipboard.GetText();
            if (string.IsNullOrEmpty(data)) {
                return;
            }

            switch (type) {
                case "an":
                    this.AccountName = data;
                    break;
                case "e":
                    this.Email = data;
                    break;
                case "u":
                    this.Username = data;
                    break;
                case "p":
                    this.Password = data;
                    break;
                case "dob":
                    this.DateOfBirth = data;
                    break;
                case "si":
                    this.SecurityInfo = data;
                    break;
                default:
                    throw new Exception("Unknown type to copy to clipboard: " + type);
            }
        }
    }
}