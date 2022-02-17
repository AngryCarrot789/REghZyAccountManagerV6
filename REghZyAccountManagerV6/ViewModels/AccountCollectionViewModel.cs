using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Principal;
using System.Windows;
using REghZy.MVVM.ViewModels;
using REghZyAccountManagerV6.Accounting;

namespace REghZyAccountManagerV6.ViewModels {
    public class AccountCollectionViewModel : BaseViewModel {
        private AccountViewModel selectedAccount;
        private int selectedIndex;
        private bool canUndo;

        public AccountViewModel SelectedAccount {
            get => this.selectedAccount;
            set => RaisePropertyChanged(ref this.selectedAccount, value);
        }

        public int SelectedIndex {
            get => this.selectedIndex;
            set => RaisePropertyChanged(ref this.selectedIndex, value);
        }

        public bool CanUndo {
            get => this.canUndo;
            set => RaisePropertyChanged(ref this.canUndo, value);
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
            this.DeletedAccounts = new Stack<DeletedAccount>();
        }

        public void MoveSelectedItemUp() {
            int index = this.SelectedIndex;
            if (index > 0) {
                this.SelectedIndex = -1;
                this.Accounts.Move(index, index - 1);
                this.SelectedIndex = index - 1;
            }
        }

        public void MoveSelectedItemDown() {
            int index = this.SelectedIndex;
            if ((index + 1) < this.Accounts.Count) {
                this.SelectedIndex = -1;
                this.Accounts.Move(index, index + 1);
                this.SelectedIndex = index + 1;
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
                    File.Delete(account.FilePath);
                }
                catch {
                    MessageBox.Show("Failed to delete the file on disk. You should manually do it. Path: " + account.FilePath, "Error deleting");
                }
            }
        }

        public void UndoLastDeletion() {
            if (this.DeletedAccounts.Count > 0) {
                DeletedAccount account = this.DeletedAccounts.Pop();
                this.Accounts.Insert(Math.Min(account.index, this.Accounts.Count), account.account);
                if (account.findIndex != -1 && ViewModelLocator.AccountPanel.Finder.Editions == account.historyEdition) {
                    ObservableCollection<AccountViewModel> find = ViewModelLocator.AccountPanel.Finder.FoundAccounts;
                    find.Insert(Math.Min(account.index, find.Count), account.account);
                }
            }

            UpdateCanUndo();
        }

        private void UpdateCanUndo() {
            this.CanUndo = this.DeletedAccounts.Count > 0;
        }
    }
}