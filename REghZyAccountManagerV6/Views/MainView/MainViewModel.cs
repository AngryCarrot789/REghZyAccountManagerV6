using System.Windows.Input;
using REghZy.MVVM.Commands;
using REghZy.MVVM.ViewModels;
using REghZyAccountManagerV6.ViewModels;

namespace REghZyAccountManagerV6.Views.MainView {
    public class MainViewModel : BaseViewModel {
        /// <summary>
        /// The panel, on the left. Responsible for handling adding/removing/searching/etc
        /// </summary>
        public AccountPanelViewModel Panel { get; }

        /// <summary>
        /// The view model that handles the collections of items
        /// </summary>
        public AccountCollectionViewModel Collection { get; }

        /// <summary>
        /// The account editor itself. This is only really used for animations, because of the grid splitter and stuff
        /// </summary>
        public AccountEditorViewModel Editor { get; }

        public ICommand FocusFindCommand { get; }

        public MainViewModel() {
            this.Panel = new AccountPanelViewModel();
            this.Collection = new AccountCollectionViewModel();
            this.Editor = new AccountEditorViewModel();
            this.Editor.EditorWidth = 375.0d;

            this.FocusFindCommand = new RelayCommand(()=> {
                ServiceLocator.FindView.FocusInput();
            });

            // this.Collection.Accounts.Add(new AccountViewModel(new List<ExtraData> { new ExtraData("hi 1"), new ExtraData("hi 2") }) { AccountName = "My acc name 1", Email = "Google@google.co.uk" });
            // this.Collection.Accounts.Add(new AccountViewModel(new List<ExtraData> { new ExtraData("ello 1111111") }) { AccountName = "ACc 2 lol mi", Email = "No thx@google.co.uk" });
            // this.Collection.Accounts.Add(new AccountViewModel(new List<ExtraData> { new ExtraData("ello 11123542351111") }) { AccountName = "ACc 3 lol mi", Email = "No thx@google.co.uk" });
            // this.Collection.Accounts.Add(new AccountViewModel(new List<ExtraData> { new ExtraData("ello 1123511111") }) { AccountName = "ACc 4 lol mi", Email = "No thx@google.co.uk" });
            // this.Collection.Accounts.Add(new AccountViewModel(new List<ExtraData> { new ExtraData("ello 135111111") }) { AccountName = "ACc 555 lol mi", Email = "No thx@google.co.uk" });

            // foreach(AccountViewModel account in this.Collection.Accounts) {
            //     account.HasBeenModified = false;
            // }
        }
    }
}