using System.Windows;
using REghZy.MVVM.Views;

namespace REghZyAccountManagerV6.Settings {
    public partial class UserSettingsWindow : Window, BaseView<UserSettingsViewModel> {
        public static UserSettingsWindow Instance { get; private set; }

        public UserSettingsViewModel Model {
            get => (UserSettingsViewModel) this.DataContext;
            set => this.DataContext = value;
        }

        public UserSettingsWindow() {
            InitializeComponent();
            Instance = this;
            ViewModelLocator.Settings = this.Model;
        }
    }
}