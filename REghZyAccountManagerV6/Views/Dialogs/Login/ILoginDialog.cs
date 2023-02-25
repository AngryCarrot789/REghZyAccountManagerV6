using REghZyAccountManagerV6.Core.Views.Dialogs;

namespace REghZyAccountManagerV6.Views.Dialogs.Login {
    public interface ILoginDialog : IDialog {
        string Password { get; }
    }
}