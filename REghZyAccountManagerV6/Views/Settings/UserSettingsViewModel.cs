using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using REghZy.MVVM.Commands;
using REghZy.MVVM.ViewModels;
using REghZyAccountManagerV6.Utils;

namespace REghZyAccountManagerV6.Views.Settings {
    public class UserSettingsViewModel : BaseViewModel {
        private string prevDirectory;
        private string saveDirectory;
        private int windowWidth;
        private int windowHeight;
        private bool doubleClickSelectAll;

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

        public bool DoubleClickBoxSelectsAll {
            get => this.doubleClickSelectAll;
            set => RaisePropertyChanged(ref this.doubleClickSelectAll, value);
        }

        public ICommand SaveToDiskCommand { get; }
        public ICommand LoadFromDiskCommand { get; }

        public ICommand SelectDirectoryCommand { get; }


        public static readonly string AppDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "REghZyAccountManager");
        public static readonly string DefaultSaveDirectory = Path.Combine(AppDirectory, "Accounts");
        public static readonly string ConfigPath = Path.Combine(AppDirectory, "config.txt");

        public UserSettingsViewModel() {
            EnsureAppDirectoryExists();
            this.SaveDirectory = DefaultSaveDirectory;
            this.SaveToDiskCommand = new RelayCommand(SaveToDisk);
            this.LoadFromDiskCommand = new RelayCommand(LoadFromDisk);
            this.SelectDirectoryCommand = new RelayCommand(SelectDirectoryDialog);

            if (!File.Exists(ConfigPath)) {
                // default settings
                // this.SaveDirectory = "C:\\Users\\kettl\\Documents\\IISExtended\\0x0000004TEMP\\NULL_ACCESS\\v6";
                this.WindowWidth = 1280;
                this.WindowHeight = 760;
                this.DoubleClickBoxSelectsAll = false;
                this.SaveToDisk();
            }
            else {
                this.LoadFromDisk();
            }
        }

        private void SelectDirectoryDialog() {
            FolderPicker picker = new FolderPicker();
            if (picker.ShowDialog() == true) {
                this.prevDirectory = this.SaveDirectory;
                this.SaveDirectory = picker.ResultPath;
                VerifyDirectory();
            }
        }

        public void LoadFromDisk() {
            EnsureAppDirectoryExists();
            if (!File.Exists(ConfigPath)) {
                return;
            }

            try {
                using (StreamReader reader = new StreamReader(new BufferedStream(File.OpenRead(ConfigPath)))) {
                    this.SaveDirectory = reader.ReadLine();
                    this.WindowWidth = int.Parse(reader.ReadLine());
                    this.WindowHeight = int.Parse(reader.ReadLine());
                }
            }
            catch(Exception e) {
                MessageBox.Show("Failed to read config: " + e.Message);
            }
        }

        public void SaveToDisk() {
            EnsureAppDirectoryExists();
            using(StreamWriter writer = new StreamWriter(new BufferedStream(File.OpenWrite(ConfigPath)))) {
                writer.WriteLine(this.SaveDirectory);
                writer.WriteLine(this.WindowWidth);
                writer.WriteLine(this.WindowHeight);
            }
        }

        public void EnsureAppDirectoryExists() {
            Directory.CreateDirectory(AppDirectory);
        }

        public void VerifyDirectory() {
            if (FileHelper.IsPathValid(this.saveDirectory)) {
                if (!Directory.Exists(this.saveDirectory)) {
                    try {
                        Directory.CreateDirectory(this.saveDirectory);
                    }
                    catch (Exception e) {
                        MessageBox.Show("Failed to create directory: " + e.Message, "Invalid directory");
                        this.SaveDirectory = this.prevDirectory;
                    }
                }
            }
            else {
                MessageBox.Show("Invalid path: " + this.SaveDirectory, "Invalid directory");
                this.SaveDirectory = this.prevDirectory;
            }
        }
    }
}