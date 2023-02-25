using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using REghZyAccountManagerV6.Core.IdleEvents;

namespace REghZyAccountManagerV6.Core.Accounting.Searching {
    public class AccountFinderViewModel : TimedInputBaseViewModel {
        private bool checkContains;
        private bool searchAccountName;
        private bool searchEmail;
        private bool searchUsername;
        private bool searchPassword;
        private bool searchDateOfBirth;
        private bool searchSecurityInfo;
        private bool searchExtraInfo;

        public bool CheckContains {
            get => this.checkContains;
            set => this.RaisePropertyChanged(ref this.checkContains, value);
        }

        public bool SearchAccountName {
            get => this.searchAccountName;
            set => this.RaisePropertyChanged(ref this.searchAccountName, value);
        }

        public bool SearchEmail {
            get => this.searchEmail;
            set => this.RaisePropertyChanged(ref this.searchEmail, value);
        }

        public bool SearchUsername {
            get => this.searchUsername;
            set => this.RaisePropertyChanged(ref this.searchUsername, value);
        }

        public bool SearchPassword {
            get => this.searchPassword;
            set => this.RaisePropertyChanged(ref this.searchPassword, value);
        }

        public bool SearchDateOfBirth {
            get => this.searchDateOfBirth;
            set => this.RaisePropertyChanged(ref this.searchDateOfBirth, value);
        }

        public bool SearchSecurityInfo {
            get => this.searchSecurityInfo;
            set => this.RaisePropertyChanged(ref this.searchSecurityInfo, value);
        }

        public bool SearchExtraInfo {
            get => this.searchExtraInfo;
            set => this.RaisePropertyChanged(ref this.searchExtraInfo, value);
        }

        public ObservableCollection<AccountViewModel> FoundAccounts { get; }

        private AccountViewModel selectedAccount;
        public AccountViewModel SelectedAccount { 
            get => this.selectedAccount; 
            set => this.RaisePropertyChanged(ref this.selectedAccount, value);
        }

        public ICommand ForceSearchCommand => base.triggerCommand;

        public ICommand ClearResultsCommand { get; }

        public ICommand SelectAllCommand { get; }

        public ICommand DeselectAllCommand { get; }

        public ICommand FocusListCommand { get; }

        public int Editions { get; private set; }

        public AccountCollectionViewModel AccountCollection { get; }

        public AccountFinderViewModel(AccountCollectionViewModel accountCollection) {
            this.AccountCollection = accountCollection;
            base.IdleEventService.MinimumTimeSinceInput = TimeSpan.FromMilliseconds(200);
            this.IdleEventService.OnIdle += this.DoSearch;
            this.FoundAccounts = new ObservableCollection<AccountViewModel>();
            this.ClearResultsCommand = new RelayCommand(() => {
                this.FoundAccounts.Clear();
                this.InputText = "";
            });

            this.SearchAccountName = true;
            this.SearchEmail = true;
            this.SearchUsername = true;

            this.SelectAllCommand = new RelayCommand(() => {
                this.SearchAccountName = true;
                this.SearchEmail = true;
                this.SearchUsername = true;
                this.SearchPassword = true;
                this.SearchDateOfBirth = true;
                this.SearchSecurityInfo = true;
                this.SearchExtraInfo = true;
            });

            this.DeselectAllCommand = new RelayCommand(() => {
                this.SearchAccountName = false;
                this.SearchEmail = false;
                this.SearchUsername = false;
                this.SearchPassword = false;
                this.SearchDateOfBirth = false;
                this.SearchSecurityInfo = false;
                this.SearchExtraInfo = false;
            });

            this.FocusListCommand = new RelayCommand(() => {
                if (this.FoundAccounts.Count > 0) {
                    IoC.FindView.FocusList();
                    this.SelectedAccount = this.FoundAccounts[0];
                }
            });
        }

        public override bool CanSearchForInput() {
            return true;
        }

        public void DoSearch() {
            this.Editions++;
            if (string.IsNullOrEmpty(this.InputText)) {
                this.FoundAccounts.Clear();
                return;
            }

            this.FoundAccounts.Clear();
            foreach (AccountViewModel account in this.AccountCollection.Accounts) {
                if (this.MatchAccount(account)) {
                    this.FoundAccounts.Add(account);
                }
            }
        }

        private bool MatchAccount(AccountViewModel account) {
            if (this.searchAccountName)
                if (this.MatchString(account.AccountName))
                    return true;

            if (this.searchEmail)
                if (this.MatchString(account.Email))
                    return true;

            if (this.searchUsername)
                if (this.MatchString(account.Username))
                    return true;

            if (this.searchPassword)
                if (this.MatchString(account.Password))
                    return true;

            if (this.searchDateOfBirth)
                if (this.MatchString(account.DateOfBirth))
                    return true;

            if (this.searchSecurityInfo)
                if (this.MatchString(account.SecurityInfo))
                    return true;

            if (this.searchExtraInfo) {
                if (this.MatchString(account.CustomInfo))
                    return true;
            }

            return false;
        }

        private bool MatchString(string value) {
            if (string.IsNullOrEmpty(value)) {
                return false;
            }

            if (this.checkContains) {
                return value.ToLower().Contains(this.InputText);
            }
            else {
                return value.ToLower().StartsWith(this.InputText);
            }
        }
    }
}