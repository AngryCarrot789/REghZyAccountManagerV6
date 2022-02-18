using System;
using System.Runtime.CompilerServices;
using REghZy.MVVM.IoC;
using REghZyAccountManagerV6.ViewModels;
using REghZyAccountManagerV6.Views.MainView;

namespace REghZyAccountManagerV6 {
    public static class ViewModelLocator {
        private static readonly SimpleIoC IoC = new SimpleIoC();

        public static ApplicationViewModel Application {
            get => IoC.GetViewModel<ApplicationViewModel>();
            private set {
                if (IoC.HasViewModel<ApplicationViewModel>()) {
                    throw new Exception("ApplicationViewModel cannot be assigned more than once");
                }

                IoC.SetViewModel(value);
            }
        }

        public static MainViewModel MainView {
            get => IoC.GetViewModel<MainViewModel>();
            private set {
                if (IoC.HasViewModel<MainViewModel>()) {
                    throw new Exception("MainViewModel cannot be assigned more than once");
                }

                IoC.SetViewModel(value);
                AccountPanel = value.Panel;
                AccountCollection = value.Collection;
                AccountEditor = value.Editor;
            }
        }

        public static AccountPanelViewModel AccountPanel {
            get => IoC.GetViewModel<AccountPanelViewModel>();
            private set => IoC.SetViewModel(value);
        }

        public static AccountCollectionViewModel AccountCollection {
            get => IoC.GetViewModel<AccountCollectionViewModel>();
            private set => IoC.SetViewModel(value);
        }

        public static AccountEditorViewModel AccountEditor {
            get => IoC.GetViewModel<AccountEditorViewModel>();
            private set => IoC.SetViewModel(value);
        }

        static ViewModelLocator() {
            Application = new ApplicationViewModel();
            MainView = Application.MainView;
        }

        // forces this class to be loaded, ensuring the application and main view models are loaded
        public static void InitCTOR() { }
    }
}