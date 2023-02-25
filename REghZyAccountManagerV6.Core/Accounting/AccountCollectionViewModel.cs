using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using REghZyAccountManagerV6.Core.Utils;

namespace REghZyAccountManagerV6.Core.Accounting {
    public class AccountCollectionViewModel : BaseViewModel {
        private bool ignoreModifications;
        private AccountViewModel selectedAccount;
        private int selectedIndex;
        private bool canUndo;
        private int saveQueueCount;

        public AccountViewModel SelectedAccount {
            get => this.selectedAccount;
            set => this.RaisePropertyChanged(ref this.selectedAccount, value, IoC.AccountList.ScrollToAccount);
        }

        public int SelectedIndex {
            get => this.selectedIndex;
            set => this.RaisePropertyChanged(ref this.selectedIndex, value);
        }

        public bool CanUndo {
            get => this.canUndo;
            set => this.RaisePropertyChanged(ref this.canUndo, value);
        }

        public int SaveQueueCount {
            get => this.saveQueueCount;
            set => this.RaisePropertyChanged(ref this.saveQueueCount, value);
        }

        public ObservableCollection<AccountViewModel> Accounts { get; }

        public IEnumerable<AccountModel> AccountModels {
            get {
                int count = 0;
                return this.Accounts.Select(acc => new AccountModel(acc) {Position = count++});
            }
        }

        public Stack<DeletedAccount> DeletedAccounts { get; }

        public AccountCollectionViewModel() {
            this.Accounts = new ObservableCollection<AccountViewModel>();
            this.Accounts.CollectionChanged += this.Accounts_CollectionChanged;
            this.DeletedAccounts = new Stack<DeletedAccount>();
        }

        public void AddAccountAsync(AccountViewModel account) {
            IoC.Dispatcher.Invoke(() => this.AddNewAccount(account, false));
        }

        public int CalculateModifiedSize() {
            int count = 0;
            foreach (AccountViewModel account in this.Accounts) {
                if (account.HasBeenModified) {
                    count++;
                }
            }

            return count;
        }

        private void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems == null) {
                return;
            }

            this.SaveQueueCount = this.CalculateModifiedSize();
        }

        public void OnAccountModifiedChanged(AccountViewModel acc) {
            if (this.ignoreModifications) {
                return;
            }

            int count = 0;
            foreach (AccountViewModel account in this.Accounts) {
                if (account.HasBeenModified) {
                    count++;
                }
            }

            this.SaveQueueCount = count;
        }

        public void MarkAllModified() {
            foreach (AccountViewModel account in this.Accounts) {
                account.HasBeenModified = true;
            }

            this.SaveQueueCount = this.CalculateModifiedSize();
        }

        public void MarkNoneModified() {
            foreach (AccountViewModel account in this.Accounts) {
                account.HasBeenModified = false;
            }

            this.SaveQueueCount = 0;
        }

        public void MoveSelectedItemUp() {
            int index = this.SelectedIndex;
            if (index > 0) {
                this.Accounts[index].MarkModified();
                this.Accounts[index - 1].MarkModified();
                this.SelectedIndex = -1;
                this.Accounts.Move(index, index - 1);
                this.SelectedIndex = index - 1;
            }
        }

        public void MoveSelectedItemDown() {
            int index = this.SelectedIndex;
            if ((index + 1) < this.Accounts.Count) {
                this.Accounts[index].MarkModified();
                this.Accounts[index + 1].MarkModified();
                this.SelectedIndex = -1;
                this.Accounts.Move(index, index + 1);
                this.SelectedIndex = index + 1;
            }
        }

        public void MoveSelectedItemToBounds(bool top) {
            int index = this.SelectedIndex;
            if (top) {
                if (index > 0) {
                    for (int i = index; i >= 0; i--) {
                        this.Accounts[i].MarkModified();
                    }

                    this.SelectedIndex = -1;
                    this.Accounts.Move(index, 0);
                    this.SelectedIndex = 0;
                    IoC.AccountList.ScrollToAccount(this.SelectedAccount);
                }
            }
            else {
                if ((index + 1) < this.Accounts.Count) {
                    int end = this.Accounts.Count - 1;
                    for (int i = end; i >= index; i--) {
                        this.Accounts[i].MarkModified();
                    }

                    this.SelectedIndex = -1;
                    this.Accounts.Move(index, end);
                    this.SelectedIndex = end;
                    IoC.AccountList.ScrollToAccount(this.SelectedAccount);
                }
            }
        }

        public void AddNewAccount(AccountViewModel account) {
            this.AddNewAccount(account, true);
        }

        public void AddNewAccount(AccountViewModel account, bool select) {
            this.Accounts.Add(account);
            if (select) {
                this.SelectedAccount = account;
            }
        }

        public void DeleteSelectedAccount() {
            this.DeleteAccount(this.selectedAccount, this.selectedIndex);
        }

        public async void DeleteAccount(AccountViewModel account, int index = -1) {
            if (index == -1) {
                index = this.Accounts.IndexOf(account);
            }

            if (account == null || index < 0 || index >= this.Accounts.Count) {
                return;
            }

            int findIndex = ViewModelLocator.AccountPanel.Finder.FoundAccounts.IndexOf(account);
            if (findIndex != -1) {
                ViewModelLocator.AccountPanel.Finder.FoundAccounts.RemoveAt(findIndex);
            }

            if (this.Accounts.Remove(account)) {
                this.DeletedAccounts.Push(new DeletedAccount(account, index, findIndex, ViewModelLocator.AccountPanel.Finder.Editions));
            }

            account.MarkModified();
            this.UpdateCanUndo();
            try {
                IoC.Database.DeleteUser(account.AccountName);
            }
            catch (Exception e) {
                await IoC.MessageDialogs.ShowMessageAsync("Failed to delete user", $"Failed to delete {account.AccountName}: {e}");
            }
        }

        public void UndoLastDeletion() {
            if (this.DeletedAccounts.Count > 0) {
                DeletedAccount account = this.DeletedAccounts.Pop();
                this.Accounts.Insert(Math.Min(account.index, this.Accounts.Count), account.account);
                if (account.findIndex != -1 && ViewModelLocator.AccountPanel.Finder.Editions == account.historyEdition) {
                    ObservableCollection<AccountViewModel> find = ViewModelLocator.AccountPanel.Finder.FoundAccounts;
                    find.Insert(Math.Min(account.findIndex, find.Count), account.account);
                }

                account.account.MarkModified();
            }

            // just incase the button can still be pressed... somehow. This might disable it
            this.UpdateCanUndo();
        }

        private void UpdateCanUndo() {
            this.CanUndo = this.DeletedAccounts.Count > 0;
        }

        public class DeletedAccount {
            public readonly AccountViewModel account;
            public readonly int index;
            public readonly int findIndex;
            public int historyEdition;

            public DeletedAccount(AccountViewModel account, int index, int findIndex, int historyEdition) {
                this.account = account;
                this.index = index;
                this.findIndex = findIndex;
                this.historyEdition = historyEdition;
            }
        }
    }
}