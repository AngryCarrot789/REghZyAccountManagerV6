using System.IO;
using System.Windows.Input;
using REghZy.MVVM.Commands;
using REghZy.MVVM.ViewModels;
using REghZy.Utils;
using REghZyAccountManagerV6.Settings;
using REghZyAccountManagerV6.Views.MainView;

namespace REghZyAccountManagerV6.ViewModels {
    public class ApplicationViewModel : BaseViewModel {
        private readonly XmlUserSettings<UserSettingsViewModel> settings;

        private UserSettingsViewModel userSettings;
        public UserSettingsViewModel UserSettings {
            get => this.userSettings;
            set => RaisePropertyChanged(ref this.userSettings, value);
        }

        public MainViewModel MainView { get; }

        public delegate void ConfigEventArgs(UserSettingsViewModel settings);
        public event ConfigEventArgs OnConfigSaving;
        public event ConfigEventArgs OnConfigLoaded;

        public ICommand SaveToDiskCommand { get; }

        public ICommand LoadFromDiskCommand { get; }

        public ApplicationViewModel() {
            this.MainView = new MainViewModel();
            // this.settings = new XmlUserSettings<UserSettingsViewModel>(Path.Combine("AccountManager", "config"));
            this.SaveToDiskCommand = new RelayCommand(SaveToDisk);
            this.LoadFromDiskCommand = new RelayCommand(LoadFromDisk);
        }

        public void SaveToDisk() {
            this.OnConfigSaving?.Invoke(this.userSettings);
            this.settings.Save(this.userSettings);
        }

        public void LoadFromDisk() {
            this.UserSettings = this.settings.Load();
            this.OnConfigLoaded?.Invoke(this.UserSettings);
        }
    }
}