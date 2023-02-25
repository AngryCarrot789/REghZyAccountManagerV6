using REghZyAccountManagerV6.Core.Views.Dialogs.Progress;

namespace REghZyAccountManagerV6.Views.Progress {
    public partial class ProgressWindow : BaseWindow, IProgressWindow {
        public ProgressViewModel ViewModel => (ProgressViewModel) this.DataContext;

        public string Message {
            get => this.Dispatcher.Invoke(() => this.ViewModel.Message);
            set => this.Dispatcher.Invoke(() => this.ViewModel.Message = value);
        }

        public ProgressWindow() {
            this.InitializeComponent();
            this.DataContext = new ProgressViewModel(this);
        }
    }
}