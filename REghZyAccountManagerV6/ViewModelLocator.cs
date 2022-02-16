using REghZy.MVVM.IoC;
using REghZyAccountManagerV6.ViewModels;
using REghZyAccountManagerV6.Views.MainView;

namespace REghZyAccountManagerV6 {
    public static class ViewModelLocator {
        private static readonly SimpleIoC IoC = new SimpleIoC();

        public static MainViewModel MainView {
            get => IoC.GetViewModel<MainViewModel>();
            private set {
                IoC.SetViewModel(value);
                AccountPanel = value.Panel;
                AccountCollection = value.Collection;
            }
        }

        public static AccountCollectionViewModel AccountCollection {
            get => IoC.GetViewModel<AccountCollectionViewModel>();
            private set => IoC.SetViewModel(value);
        }

        public static AccountPanelViewModel AccountPanel {
            get => IoC.GetViewModel<AccountPanelViewModel>();
            private set => IoC.SetViewModel(value);
        }

        static ViewModelLocator() {
            MainView = new MainViewModel();
        }
    }
}