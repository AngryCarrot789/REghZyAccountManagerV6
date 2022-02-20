using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using REghZy.MVVM.Commands;
using REghZy.MVVM.Views;

namespace REghZyAccountManagerV6.Views.Settings {
    public partial class UserSettingsWindow : Window, BaseView<UserSettingsViewModel>, ISettings {
        public static UserSettingsWindow Instance { get; private set; }

        public bool IsOpen {
            get => this.Visibility == Visibility.Visible;
        }

        public ICommand OpenViewCommand { get; }

        public ICommand CloseViewCommand { get; }

        public UserSettingsViewModel Settings {
            get => this.Model;
        }

        public UserSettingsViewModel Model {
            get => (UserSettingsViewModel) this.DataContext;
            set => this.DataContext = value;
        }

        public UserSettingsWindow() {
            InitializeComponent();
            Instance = this;
            this.Model = ViewModelLocator.Settings;
            this.OpenViewCommand = new RelayCommand(this.OpenView);
            this.CloseViewCommand = new RelayCommand(this.CloseView);
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            e.Cancel = true;
            CloseView();
        }

        public void OpenView() {
            this.Show();
        }

        public void CloseView() {
            this.Hide();
        }
    }
}