using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using REghZy.MVVM.Commands;
using REghZy.MVVM.ViewModels;
using REghZyAccountManagerV6.Accounting;
using REghZyAccountManagerV6.Finding;

namespace REghZyAccountManagerV6.ViewModels {
    public class AccountPanelViewModel : BaseViewModel {
        private bool isEditorOpen;

        public bool IsEditorOpen {
            get => this.isEditorOpen;
            set => RaisePropertyChangedWithCallback(ref this.isEditorOpen, value, (newOpenStatus) => {
                if (newOpenStatus) {
                    ServiceLocator.Editor.OpenView();
                }
                else {
                    ServiceLocator.Editor.CloseView();
                }
            });
        }

        public AccountFinderViewModel Finder { get; }

        public ICommand CreateNewAccountCommand { get; }

        public ICommand ShowAccountEditorCommand { get; }

        public ICommand HideAccountEditorCommand { get; }

        public ICommand FlipAccountEditorCommand { get; }

        public ICommand DeleteSelectedAccountCommand { get; }

        public ICommand UndoLastDeletionCommand { get; }

        public ICommand MoveItemUpCommand { get; }

        public ICommand MoveItemDownCommand { get; }

        public AccountPanelViewModel() {
            this.Finder = new AccountFinderViewModel();
            this.CreateNewAccountCommand = new RelayCommand(this.CreateNewAccount);
            this.ShowAccountEditorCommand = new RelayCommand(() => this.IsEditorOpen = true);
            this.HideAccountEditorCommand = new RelayCommand(() => this.IsEditorOpen = false);
            this.FlipAccountEditorCommand = new RelayCommand(() => this.IsEditorOpen = !this.IsEditorOpen);
            this.DeleteSelectedAccountCommand = new RelayCommand(this.DeleteSelectedAccount);
            this.UndoLastDeletionCommand = new RelayCommand(this.UndoLastDeletion);
            this.MoveItemUpCommand = new RelayCommand(() => ViewModelLocator.AccountCollection.MoveSelectedItemUp());
            this.MoveItemDownCommand = new RelayCommand(() => ViewModelLocator.AccountCollection.MoveSelectedItemDown());
        }

        public void CreateNewAccount() {
            // this should always be false, according to the logic
            if (ServiceLocator.NewAccount.IsOpen) {
                return;
            }

            ServiceLocator.NewAccount.OpenView();
            if (!ServiceLocator.NewAccount.IsAccountValid) {
                return;
            }

            AccountViewModel account = ServiceLocator.NewAccount.Account;
            ServiceLocator.NewAccount.Account = null;
            ViewModelLocator.AccountCollection.AddNewAccount(account);
        }

        public void DeleteSelectedAccount() {
            ViewModelLocator.AccountCollection.DeleteSelectedAccount();
        }

        public void UndoLastDeletion() {
            ViewModelLocator.AccountCollection.UndoLastDeletion();
        }
    }
}