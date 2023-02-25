using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using REghZyAccountManagerV6.Accounting;
using REghZyAccountManagerV6.Core;
using REghZyAccountManagerV6.Core.Accounting;
using REghZyAccountManagerV6.Core.Config;
using REghZyAccountManagerV6.Core.Views;
using REghZyAccountManagerV6.Core.Views.Dialogs.Message;
using REghZyAccountManagerV6.Utils;

namespace REghZyAccountManagerV6.Views.MainView {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            this.DataContext = new MainViewModel();
            this.InitializeComponent();
            IoC.FindView = new FindViewWrapper(this);
            IoC.AccountList = new AccountListWrapper(this);
            IoC.AccountActivity = new ActivityStatusWrapper(this);
            ViewModelLocator.AccountPanel.IsEditorOpen = false;
        }

        protected override async void OnClosing(CancelEventArgs e) {
            if (string.IsNullOrEmpty(Configuration.AccountSaveFile)) {
                await IoC.MessageDialogs.ShowMessageAsync("Could not save files", "Accounts could not be saved, because the target account file directory was not set (File > Preferences)");
                e.Cancel = true;
                return;
            }

            List<AccountModel> modified = ViewModelLocator.AccountCollection.AccountModels.ToList();
            IoC.Database.WriteAccounts(modified);
            if (!e.Cancel) {
                base.OnClosing(e);
            }
        }

        private class ActivityStatusWrapper : IAccountActivityView {
            private readonly MainWindow window;

            public ActivityStatusWrapper(MainWindow window) {
                this.window = window;
            }

            public string Message {
                get => this.window.Dispatcher.Invoke(() => this.window.ActivityStatus.Text);
                set => this.window.Dispatcher.Invoke(() => { this.window.ActivityStatus.Text = value; });
            }

            public bool IsVisible {
                get => this.window.Dispatcher.Invoke(() => this.window.ActivityStatus.Visibility == Visibility.Visible);
                set => this.window.Dispatcher.Invoke(() => { this.window.ActivityStatus.Visibility = value ? Visibility.Visible : Visibility.Collapsed; });
            }
        }

        private class FindViewWrapper : IFindView {
            private readonly MainWindow window;

            public FindViewWrapper(MainWindow window) {
                this.window = window;
            }

            public void FocusInput() {
                this.window.FindInputBox.Focus();
                this.window.FindInputBox.SelectAll();
            }

            public void FocusList() {
                this.window.FindList.Focus();
            }
        }

        public class AccountListWrapper : IAccountList {
            private readonly MainWindow window;

            public AccountListWrapper(MainWindow window) {
                this.window = window;
            }

            public void ScrollToAccount(object account) {
                this.window.AccountListBox.ScrollIntoView(account);
            }
        }

        private List<AccountModel> accounts;
        private void ListBox_DragEnter(object sender, DragEventArgs e) {
            if (IoC.Database is JsonAccountDatabase database) {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] paths) {
                    this.accounts = new List<AccountModel>(paths.Length);
                    foreach (string path in paths) {
                        // if it doesn't exist, or is bigger than 64K, then ignore it
                        if (File.Exists(path) && Path.GetExtension(path) == ".json" && new FileInfo(path).Length <= 65535) {
                            try {
                                this.accounts.Add(JsonAccountDatabase.ReadAccount(path));
                            }
                            catch (Exception ee) {
                                MessageBox.Show($"Failed to read file at path '{path}'. Reason: {ee.Message}", "Error reading file");
                            }
                        }
                    }
                }
            }
        }

        private void ListBox_DragLeave(object sender, DragEventArgs e) {
            this.accounts = null;
        }

        private void ListBox_Drop(object sender, DragEventArgs e) {
            if (this.accounts != null) {
                AccountCollectionViewModel vm = ViewModelLocator.AccountCollection;
                foreach (AccountViewModel acc in this.accounts.Select(account => account.ToViewModel())) {
                    acc.MarkModified();
                    vm.AddNewAccount(acc, false);
                }

                this.accounts = null;
            }
        }
    }
}
