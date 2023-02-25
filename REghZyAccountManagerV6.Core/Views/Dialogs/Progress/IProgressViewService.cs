using System.Threading.Tasks;

namespace REghZyAccountManagerV6.Core.Views.Dialogs.Progress {
    public interface IProgressViewService {
        IProgressWindow ShowIndeterminateProgress(string title, string message);

        Task<IProgressWindow> ShowIndeterminateProgressAsync(string title, string message);
    }
}