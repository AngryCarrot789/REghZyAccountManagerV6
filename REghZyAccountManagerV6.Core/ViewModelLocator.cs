using REghZyAccountManagerV6.Core.Accounting;

namespace REghZyAccountManagerV6.Core {
    public static class ViewModelLocator {
        public static AccountPanelViewModel AccountPanel {
            get => IoC.Instance.Provide<AccountPanelViewModel>();
            set => IoC.Instance.Register(value);
        }

        public static AccountCollectionViewModel AccountCollection {
            get => IoC.Instance.Provide<AccountCollectionViewModel>();
            set => IoC.Instance.Register(value);
        }
    }
}