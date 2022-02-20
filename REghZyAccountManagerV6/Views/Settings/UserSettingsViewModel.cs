using System;
using System.IO;
using System.Windows;
using REghZy.MVVM.ViewModels;
using REghZyAccountManagerV6.Utils;

namespace REghZyAccountManagerV6.Settings {
    public class UserSettingsViewModel : BaseViewModel {
        private string saveDirectory;
        private int windowWidth;
        private int windowHeight;

        public string SaveDirectory {
            get => this.saveDirectory;
            set => RaisePropertyChanged(ref this.saveDirectory, value);
        }

        public int WindowWidth {
            get => this.windowWidth;
            set => RaisePropertyChanged(ref this.windowWidth, value);
        }

        public int WindowHeight {
            get => this.windowHeight;
            set => RaisePropertyChanged(ref this.windowHeight, value);
        }

        public void VerifyDirectory() {
            if (FileHelper.IsPathValid(this.saveDirectory)) {
                if (!Directory.Exists(this.saveDirectory)) {
                    if (MessageBox.Show($"The directory '{this.saveDirectory}' does not exist. Do you want to create it?", "Create directory?", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes) == MessageBoxResult.Yes) {
                        try {
                            Directory.CreateDirectory(this.saveDirectory);
                        }
                        catch (Exception e) {
                            MessageBox.Show("Failed to create directory: " + e.Message, "Invalid directory");
                            return;
                        }
                    }

                    RaisePropertyChanged(ref this.saveDirectory, this.saveDirectory);
                }
            }
            else {
                MessageBox.Show("Invalid path: " + this.saveDirectory, "Invalid directory");
            }
        }
    }
}