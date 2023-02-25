using System.Threading.Tasks;
using System.Windows;
using REghZyAccountManagerV6.Core.Views.Dialogs.Progress;

namespace REghZyAccountManagerV6.Views.Progress {
    public class ProgressViewService : IProgressViewService {
        public IProgressWindow ShowIndeterminateProgress(string title, string message) {
            ProgressWindow window = new ProgressWindow {
                Title = title,
                Message = message
            };

            window.Show();
            return window;
        }

        public async Task<IProgressWindow> ShowIndeterminateProgressAsync(string title, string message) {
            return await Application.Current.Dispatcher.InvokeAsync(() => this.ShowIndeterminateProgress(title, message));
        }
    }
}