using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using REghZyAccountManagerV6.Accounting;
using REghZyAccountManagerV6.Core;
using REghZyAccountManagerV6.Core.Accounting;
using REghZyAccountManagerV6.Core.Config;
using REghZyAccountManagerV6.Core.Services;
using REghZyAccountManagerV6.Services;
using REghZyAccountManagerV6.Views.Dialogs.Login;
using REghZyAccountManagerV6.Views.Dialogs.Message;
using REghZyAccountManagerV6.Views.Dialogs.NewAccount;
using REghZyAccountManagerV6.Views.MainView;
using REghZyAccountManagerV6.Views.Progress;
using REghZyAccountManagerV6.Views.Settings;

namespace REghZyAccountManagerV6 {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IApplication {
        public static readonly string APP_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "REghZyAccountManager");
        public static readonly string DEFAULT_ACCOUNTS_FILE = Path.Combine(APP_DIR, "accounts.json");
        public static readonly string CONFIG_PATH = Path.Combine(APP_DIR, "configuration.txt");

        public string ConfigPath => CONFIG_PATH;

        public Configuration Configuration { get; set; }

        public static App Instance => (App) Current;

        private async void App_OnStartup(object sender, StartupEventArgs e) {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            IoC.Application = this;
            this.Configuration = new Configuration();
            if (File.Exists(this.ConfigPath)) {
                try {
                    this.Configuration.Load(this.ConfigPath);
                }
                catch (Exception ex) {
                    MessageBox.Show($"Failed to read config at {this.ConfigPath}, default values will be used\n{ex}", "Failed to read config");
                    this.Configuration.LoadDefaults();
                }
            }
            else {
                this.Configuration.LoadDefaults();
                try {
                    this.Configuration.Save(this.ConfigPath);
                }
                catch (Exception ex) {
                    MessageBox.Show($"Failed to create default config at {this.ConfigPath}\n{ex}", "Failed to save config");
                }
            }

            IoC.MessageDialogs = new MessageDialogService();
            IoC.Dispatcher = new DispatcherDelegate(this);
            IoC.Database = new SingularJsonFileDatabase();

            if (!string.IsNullOrEmpty(Environment.UserName) && !Debugger.IsAttached) {
                LoginDialog window = new LoginDialog();
                if (window.ShowDialog() != true) {
                    this.Shutdown();
                    return;
                }
            }

            // The login view model handles incorrect passwords, so we are logged in by now
            // Register core services
            IoC.Clipboard = new ClipboardService();
            IoC.NewAccountService = new NewUserDialogService();
            IoC.FilePicker = new FilePickDialogService();
            IoC.ConfigDialog = new ConfigDialogService();
            IoC.ProgressViewService = new ProgressViewService();

            this.MainWindow = new MainWindow();
            this.MainWindow.Show();

            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            await this.LoadAccounts();
        }

        private async Task LoadAccounts() {
            // ObservableCollection<AccountViewModel> list = ViewModelLocator.AccountCollection.Accounts;
            // IEnumerable<AccountModel> enumerable = new CrappyOriginalDatabase().ReadAccounts();
            // foreach (AccountModel model in enumerable.OrderBy(d => d.Position)) {
            //     list.Add(model.ToViewModel());
            // }

            if (string.IsNullOrEmpty(Configuration.AccountSaveFile) || !File.Exists(Configuration.AccountSaveFile)) {
                await IoC.MessageDialogs.ShowMessageAsync("Save file is non-existent", "The account save file does not exist or is invalid. Please select one in \"File > Preferences\", then click the \"File > Load\" button");
            }
            else {
                ObservableCollection<AccountViewModel> list = ViewModelLocator.AccountCollection.Accounts;
                IEnumerable<AccountModel> enumerable = IoC.Database.ReadAccounts();
                foreach (AccountModel model in enumerable.OrderBy(d => d.Position)) {
                    list.Add(model.ToViewModel());
                }
            }
        }

        private class DispatcherDelegate : IDispatcher {
            private readonly App app;

            public DispatcherDelegate(App app) {
                this.app = app;
            }

            public void Invoke(Action action) {
                this.app.Dispatcher.Invoke(action);
            }

            public T Invoke<T>(Func<T> function) {
                return this.app.Dispatcher.Invoke(function);
            }

            public async Task InvokeAsync(Action action) {
                await this.app.Dispatcher.InvokeAsync(action);
            }

            public async Task<T> InvokeAsync<T>(Func<T> function) {
                return await this.app.Dispatcher.InvokeAsync(function);
            }
        }
    }
}
