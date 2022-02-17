using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using REghZy.MVVM.Commands;
using REghZy.MVVM.ViewModels;
using REghZyAccountManagerV6.Utils;

namespace REghZyAccountManagerV6.Settings {
    public class UserSettingsViewModel : BaseViewModel {
        private string saveDirectory;
        private int windowWidth;
        private int windowHeight;

        [XmlElement]
        public string SaveDirectory {
            get => this.saveDirectory;
            set {
                if (FileHelper.IsPathValid(value)) {
                    if (!Directory.Exists(value)) {
                        if (MessageBox.Show($"The directory '{value}' does not exist. Do you want to create it?", "Create directory?", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes) == MessageBoxResult.Yes) {
                            try {
                                Directory.CreateDirectory(value);
                            }
                            catch (Exception e) {
                                MessageBox.Show("Failed to create directory: " + e.Message, "Invalid directory");
                                return;
                            }
                        }

                        RaisePropertyChanged(ref this.saveDirectory, value);
                    }
                }
                else {
                    MessageBox.Show("Invalid path: " + value, "Invalid directory");
                }
            }
        }

        [XmlElement]
        public int WindowWidth {
            get => this.windowWidth;
            set => RaisePropertyChanged(ref this.windowWidth, value);
        }

        [XmlElement]
        public int WindowHeight {
            get => this.windowHeight;
            set => RaisePropertyChanged(ref this.windowHeight, value);
        }
    }
}