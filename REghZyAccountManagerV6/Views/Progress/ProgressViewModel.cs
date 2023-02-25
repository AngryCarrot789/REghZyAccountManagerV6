using REghZyAccountManagerV6.Core.Views;
using REghZyAccountManagerV6.Core.Views.Dialogs.Progress;

namespace REghZyAccountManagerV6.Views.Progress {
    public class ProgressViewModel : BaseWindowViewModel {
        private string message;
        public string Message {
            get => this.message;
            set => this.RaisePropertyChanged(ref this.message, value);
        }

        public new IProgressWindow Window => (IProgressWindow) base.Window;

        public ProgressViewModel(IProgressWindow window) : base(window) {

        }
    }
}