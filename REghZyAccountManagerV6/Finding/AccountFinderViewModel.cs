using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using REghZy.MVVM.Commands;
using REghZy.MVVM.ViewModels;
using REghZyAccountManagerV6.Accounting;

namespace REghZyAccountManagerV6.Finding {
    public class AccountFinderViewModel : BaseViewModel {
        private bool checkContains;
        private bool searchAccountName;
        private bool searchEmail;
        private bool searchUsername;
        private bool searchPassword;
        private bool searchDateOfBirth;
        private bool searchSecurityInfo;
        private bool searchExtraInfo;

        private string input;

        public bool CheckContains {
            get => this.checkContains;
            set => RaisePropertyChanged(ref this.checkContains, value);
        }

        public bool SearchAccountName {
            get => this.searchAccountName;
            set => RaisePropertyChanged(ref this.searchAccountName, value);
        }

        public bool SearchEmail {
            get => this.searchEmail;
            set => RaisePropertyChanged(ref this.searchEmail, value);
        }

        public bool SearchUsername {
            get => this.searchUsername;
            set => RaisePropertyChanged(ref this.searchUsername, value);
        }

        public bool SearchPassword {
            get => this.searchPassword;
            set => RaisePropertyChanged(ref this.searchPassword, value);
        }

        public bool SearchDateOfBirth {
            get => this.searchDateOfBirth;
            set => RaisePropertyChanged(ref this.searchDateOfBirth, value);
        }

        public bool SearchSecurityInfo {
            get => this.searchSecurityInfo;
            set => RaisePropertyChanged(ref this.searchSecurityInfo, value);
        }

        public bool SearchExtraInfo {
            get => this.searchExtraInfo;
            set => RaisePropertyChanged(ref this.searchExtraInfo, value);
        }

        public string Input {
            get => this.input;
            set => RaisePropertyChangedWithCallback(ref this.input, value.ToLower(), DoSearch);
        }

        public ObservableCollection<AccountViewModel> FoundAccounts { get; }

        private AccountViewModel selectedAccount;
        public AccountViewModel SelectedAccount { 
            get => this.selectedAccount; 
            set => RaisePropertyChanged(ref this.selectedAccount, value);
        }

        public ICommand DoSearchCommand { get; }

        public ICommand ClearResultsCommand { get; }

        public ICommand SelectAllCommand { get; }

        public ICommand DeselectAllCommand { get; }

        public ICommand FocusListCommand { get; }

        public int Editions { get; private set; }

        public AccountFinderViewModel() {
            this.FoundAccounts = new ObservableCollection<AccountViewModel>();
            this.DoSearchCommand = new RelayCommand(DoSearch);
            this.ClearResultsCommand = new RelayCommand(this.FoundAccounts.Clear);
            this.SearchAccountName = true;

            this.SelectAllCommand = new RelayCommand(() => {
                SearchAccountName = true;
                SearchEmail = true;
                SearchUsername = true;
                SearchPassword = true;
                SearchDateOfBirth = true;
                SearchSecurityInfo = true;
                SearchExtraInfo = true;
            });

            this.DeselectAllCommand = new RelayCommand(() => {
                SearchAccountName = false;
                SearchEmail = false;
                SearchUsername = false;
                SearchPassword = false;
                SearchDateOfBirth = false;
                SearchSecurityInfo = false;
                SearchExtraInfo = false;
            });

            this.FocusListCommand = new RelayCommand(() => {
                if (this.FoundAccounts.Count > 0) {
                    ServiceLocator.FindView.FocusList();
                    this.SelectedAccount = this.FoundAccounts[0];
                }
            });
        }

        public void DoSearch() {
            this.Editions++;
            if (string.IsNullOrEmpty(this.input)) {
                this.FoundAccounts.Clear();
                return;
            }

            ICollection<AccountViewModel> accounts = this.FoundAccounts;
            accounts.Clear();
            foreach (AccountViewModel account in ViewModelLocator.AccountCollection.Accounts) {
                if (this.MatchAccount(account)) {
                    accounts.Add(account);
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
                foreach (ExtraData value in account.ExtraInfo.ExtraInformation) {
                    if (this.MatchString(value.Value)) {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool MatchString(string value) {
            if (string.IsNullOrEmpty(value)) {
                return false;
            }

            if (this.checkContains) {
                return value.ToLower().Contains(this.input);
            }
            else {
                return value.ToLower().StartsWith(this.input);
            }
        }
    }
}