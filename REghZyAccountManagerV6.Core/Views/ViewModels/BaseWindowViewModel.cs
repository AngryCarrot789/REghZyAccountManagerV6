namespace REghZyAccountManagerV6.Core.Views.ViewModels {
    public abstract class BaseWindowViewModel : BaseViewModel {
        public IWindow Window { get; }

        protected BaseWindowViewModel(IWindow window) {
            this.Window = window;
        }
    }
}