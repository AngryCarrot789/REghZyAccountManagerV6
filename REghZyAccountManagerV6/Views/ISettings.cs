using REghZyAccountManagerV6.Views.Settings;

namespace REghZyAccountManagerV6.Views {
    public interface ISettings : IView {
        UserSettingsViewModel Settings { get; }
    }
}