using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using REghZyAccountManagerV6.Core;
using REghZyAccountManagerV6.Core.Config;
using REghZyAccountManagerV6.Core.Views.Dialogs;

namespace REghZyAccountManagerV6.Views.Settings {
    public class UserSettingsViewModel : BaseConfirmableDialogViewModel {
        private string saveDirectory;
        private int windowWidth;
        private int windowHeight;
        private bool doubleClickSelectAll;

        public string SaveDirectory {
            get => this.saveDirectory;
            set => this.RaisePropertyChanged(ref this.saveDirectory, value);
        }

        public int WindowWidth {
            get => this.windowWidth;
            set => this.RaisePropertyChanged(ref this.windowWidth, value);
        }

        public int WindowHeight {
            get => this.windowHeight;
            set => this.RaisePropertyChanged(ref this.windowHeight, value);
        }

        public bool DoubleClickBoxSelectsAll {
            get => this.doubleClickSelectAll;
            set => this.RaisePropertyChanged(ref this.doubleClickSelectAll, value);
        }

        public ICommand SelectDirectoryCommand { get; }

        public ICommand ApplyCommand { get; }

        /// <summary>
        /// A copy of the configuration that will be used for modifying
        /// </summary>
        public Configuration ModifiedConfiguration { get; }

        public UserSettingsViewModel(IDialog dialog, Configuration configuration) : base(dialog) {
            this.ModifiedConfiguration = configuration.Clone();
            this.SelectDirectoryCommand = new RelayCommand(this.SelectDirectoryDialog);
            this.SaveDirectory = this.ModifiedConfiguration.Get(Configuration.AccountFilePathKey);
            this.WindowWidth = this.ModifiedConfiguration.Get(Configuration.MainWindowWidthKey);
            this.WindowHeight = this.ModifiedConfiguration.Get(Configuration.MainWindowHeightKey);
            this.DoubleClickBoxSelectsAll = this.ModifiedConfiguration.Get(Configuration.DoubleClickSelectAllTextKey);
        }

        public void SelectDirectoryDialog() {
            DialogResult<string> result = IoC.FilePicker.ShowFolderPickerDialogAsync(titleBar: "Select a directory, which is where accounts are saved to");
            if (result.IsSuccess) {
                this.SaveDirectory = result.Value;
            }
        }

        public override async void ConfirmAction() {
            if (await this.ApplySettings()) {
                base.ConfirmAction();
            }
        }

        public async Task<bool> ApplySettings() {
            if (string.IsNullOrEmpty(this.SaveDirectory)) {
                bool revert = await IoC.MessageDialogs.ShowYesNoDialogAsync("No save directory chosen", "You have not chosen a directory to save to. Do you want to revert back to the previous directory?", true);
                if (!revert) {
                    return false;
                }

                this.SaveDirectory = this.ModifiedConfiguration.Get(Configuration.AccountFilePathKey);
            }

            if (this.windowWidth < 750) {
                if (!await IoC.MessageDialogs.ShowYesNoDialogAsync("Invalid window width", $"The window width is too small ({this.windowWidth} < 750). Do you want to revert back to the previous width?", true)) {
                    return false;
                }

                this.WindowWidth = this.ModifiedConfiguration.Get(Configuration.MainWindowWidthKey);
            }

            if (this.windowHeight < 525) {
                if (!await IoC.MessageDialogs.ShowYesNoDialogAsync("Invalid window height", $"The window height is too small ({this.windowHeight} < 525). Do you want to revert back to the previous height?", true)) {
                    return false;
                }

                this.WindowHeight = this.ModifiedConfiguration.Get(Configuration.MainWindowHeightKey);
            }

            this.ModifiedConfiguration.Set(Configuration.AccountFilePathKey, this.SaveDirectory);
            this.ModifiedConfiguration.Set(Configuration.MainWindowWidthKey, this.WindowWidth);
            this.ModifiedConfiguration.Set(Configuration.MainWindowHeightKey, this.WindowHeight);
            this.ModifiedConfiguration.Set(Configuration.DoubleClickSelectAllTextKey, this.DoubleClickBoxSelectsAll);
            return true;
        }
    }
}