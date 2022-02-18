using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using REghZy.MVVM.ViewModels;
using REghZyAccountManagerV6.Accounting;

namespace REghZyAccountManagerV6.ViewModels {
    public class AccountCollectionViewModel : BaseViewModel {
        private AccountViewModel selectedAccount;
        private int selectedIndex;
        private bool canUndo;
        private int saveQueueCount;
        private readonly HashSet<AccountViewModel> toSave = new HashSet<AccountViewModel>();

        public AccountViewModel SelectedAccount {
            get => this.selectedAccount;
            set => RaisePropertyChangedWithCallback(ref this.selectedAccount, value, ServiceLocator.AccountList.ScrollToAccount);
        }

        public int SelectedIndex {
            get => this.selectedIndex;
            set => RaisePropertyChanged(ref this.selectedIndex, value);
        }

        public void MarkAccountModifed(AccountViewModel acc) {
            if (this.Accounts.Contains(acc)) {
                if (acc.HasBeenModified) {
                    this.toSave.Add(acc);
                }
                else {
                    this.toSave.Remove(acc);
                }
            }
            else {
                this.toSave.Remove(acc);
            }

            this.SaveQueueCount = this.toSave.Count;
        }

        public bool CanUndo {
            get => this.canUndo;
            set => RaisePropertyChanged(ref this.canUndo, value);
        }

        public int SaveQueueCount {
            get => this.saveQueueCount;
            set => RaisePropertyChanged(ref this.saveQueueCount, value);
        }

        public ObservableCollection<AccountViewModel> Accounts { get; }

        public Stack<DeletedAccount> DeletedAccounts { get; }

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

        public AccountCollectionViewModel() {
            this.Accounts = new ObservableCollection<AccountViewModel>();
            this.Accounts.CollectionChanged += this.Accounts_CollectionChanged;
            this.DeletedAccounts = new Stack<DeletedAccount>();
        }

        private void Accounts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.NewItems == null) {
                return;
            }

            foreach (object obj in e.NewItems) {
                if (obj is AccountViewModel account) {
                    MarkAccountModifed(account);
                }
            }
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
                    for(int i = index; i >= 0; i--) {
                        this.Accounts[i].MarkModified();
                    }

                    this.SelectedIndex = -1;
                    this.Accounts.Move(index, 0);
                    this.SelectedIndex = 0;
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
                }
            }
        }

        public void AddNewAccount(AccountViewModel account) {
            AddNewAccount(account, true);
        }

        public void AddNewAccount(AccountViewModel account, bool select) {
            this.Accounts.Add(account);
            if (select) {
                this.SelectedAccount = account;
            }
        }

        public void DeleteSelectedAccount() {
            DeleteAccount(this.selectedAccount, this.selectedIndex);
        }

        public void DeleteAccount(AccountViewModel account, int index = -1) {
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
            UpdateCanUndo();
            if (!string.IsNullOrEmpty(account.FilePath) && File.Exists(account.FilePath)) {
                try {
                    File.Move(account.FilePath, Path.Combine(Path.GetTempPath(), "RZ_ACCOUNT_BACKUP_" + Path.GetFileName(account.FilePath)));
                }
                catch(Exception e) {
                    MessageBox.Show($"Failed to delete the file on disk: {e.Message}. You should manually do it. Path: " + account.FilePath, "Error deleting");
                }
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
            UpdateCanUndo();
        }

        private void UpdateCanUndo() {
            this.CanUndo = this.DeletedAccounts.Count > 0;
        }
    }
}