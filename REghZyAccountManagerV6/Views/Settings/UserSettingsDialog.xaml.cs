using REghZyAccountManagerV6.Core.Config;

namespace REghZyAccountManagerV6.Views.Settings {
    public partial class UserSettingsDialog : BaseDialog {
        public UserSettingsViewModel ViewModel => (UserSettingsViewModel) this.DataContext;

        public UserSettingsDialog(Configuration configuration) {
            this.InitializeComponent();
            this.DataContext = new UserSettingsViewModel(this, configuration);
        }
    }
}