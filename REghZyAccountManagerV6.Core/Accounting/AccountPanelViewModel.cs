using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using REghZyAccountManagerV6.Core.Accounting.Searching;
using REghZyAccountManagerV6.Core.Views.Dialogs.NewAccounts;

namespace REghZyAccountManagerV6.Core.Accounting {
    public class AccountPanelViewModel : BaseViewModel {
        private bool isEditorOpen;
        public bool IsEditorOpen {
            get => this.isEditorOpen;
            set => this.RaisePropertyChanged(ref this.isEditorOpen, value);
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

        public ICommand MoveItemToTop { get; }

        public ICommand MoveItemToBottom { get; }

        public AccountCollectionViewModel AccountCollection { get; }

        public AccountPanelViewModel(AccountCollectionViewModel accountCollection) {
            this.AccountCollection = accountCollection;
            this.Finder = new AccountFinderViewModel(accountCollection);
            this.CreateNewAccountCommand = new RelayCommand(async() => await this.CreateNewAccountAsync());
            this.ShowAccountEditorCommand = new RelayCommand(() => this.IsEditorOpen = true);
            this.HideAccountEditorCommand = new RelayCommand(() => this.IsEditorOpen = false);
            this.FlipAccountEditorCommand = new RelayCommand(() => this.IsEditorOpen = !this.IsEditorOpen);
            this.DeleteSelectedAccountCommand = new RelayCommand(this.DeleteSelectedAccount);
            this.UndoLastDeletionCommand = new RelayCommand(this.UndoLastDeletion);
            this.MoveItemUpCommand = new RelayCommand(() => this.AccountCollection.MoveSelectedItemUp());
            this.MoveItemDownCommand = new RelayCommand(() => this.AccountCollection.MoveSelectedItemDown());

            this.MoveItemToTop = new RelayCommand(() => {
                this.AccountCollection.MoveSelectedItemToBounds(true);
            });

            this.MoveItemToBottom = new RelayCommand(() => {
                this.AccountCollection.MoveSelectedItemToBounds(false);
            });
        }

        public async Task CreateNewAccountAsync() {
            INewAccountDialogService service = IoC.Instance.Provide<INewAccountDialogService>();
            NewAccountDialogResult result = service.ShowNewAccountDialog();
            if (!result.IsSuccess) {
                return;
            }

            if (this.AccountCollection.Accounts.Any(x => x.AccountName == result.AccountName)) {
                await IoC.MessageDialogs.ShowMessageAsync("Account already exists", $"An account named {result.AccountName} already exists");
                return;
            }

            AccountViewModel account = new AccountViewModel() {
                AccountName = result.AccountName,
                Email = result.Email,
                Username = result.Username,
                Password = result.Password,
                DateOfBirth = result.DateOfBirth,
                SecurityInfo = result.SecurityInfo
            };

            this.AccountCollection.AddNewAccount(account);
        }

        public void DeleteSelectedAccount() {
            this.AccountCollection.DeleteSelectedAccount();
        }

        public void UndoLastDeletion() {
            this.AccountCollection.UndoLastDeletion();
        }
    }
}