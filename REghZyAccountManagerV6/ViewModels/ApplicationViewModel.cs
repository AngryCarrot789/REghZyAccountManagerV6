using System.IO;
using System.Threading;
using System.Windows.Input;
using REghZy.MVVM.Commands;
using REghZy.MVVM.ViewModels;
using REghZy.Utils;
using REghZyAccountManagerV6.Views.MainView;
using REghZyAccountManagerV6.Views.Settings;

namespace REghZyAccountManagerV6.ViewModels {
    public class ApplicationViewModel : BaseViewModel {
        public MainViewModel MainView { get; }
        public UserSettingsViewModel UserSettings { get; }

        public delegate void ConfigEventArgs(UserSettingsViewModel settings);
        public event ConfigEventArgs OnConfigSaving;
        public event ConfigEventArgs OnConfigLoaded;

        public ICommand SaveConfigToDiskCommand { get; }
        public ICommand LoadConfigFromDiskCommand { get; }

        public ApplicationViewModel() {
            this.MainView = new MainViewModel();
            this.UserSettings = new UserSettingsViewModel();
            this.SaveConfigToDiskCommand = new RelayCommand(SaveToDisk);
            this.LoadConfigFromDiskCommand = new RelayCommand(LoadFromDisk);
        }

        public void SaveToDisk() {
            this.OnConfigSaving?.Invoke(this.UserSettings);
            this.UserSettings.SaveToDisk();
        }

        public void LoadFromDisk() {
            this.UserSettings.LoadFromDisk();
            this.OnConfigLoaded?.Invoke(this.UserSettings);
        }
    }
}