using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using REghZy.MVVM.Commands;
using REghZyAccountManagerV6.Accounting;
using REghZyAccountManagerV6.Accounting.IO;
using REghZyAccountManagerV6.Utils;
using REghZyAccountManagerV6.ViewModels;
using REghZyAccountManagerV6.Views.NewAccounts;
using REghZyAccountManagerV6.Views.Settings;

namespace REghZyAccountManagerV6.Views.MainView {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private bool isEditorOpen;
        private const double EDITOR_WIDTH_CLOSE = 0;
        private const double EDITOR_WIDTH_OPEN = 375;

        public MainWindow() {
            InitializeComponent();
            this.isEditorOpen = true;

            ServiceLocator.InitCTOR();
            ServiceLocator.Editor = new EditorViewWrapper(this);
            ServiceLocator.FindView = new FindViewWrapper(this);
            ServiceLocator.AccountList = new AccountListWrapper(this);
            ServiceLocator.NewAccount = new NewAccountWindow();

            ViewModelLocator.InitCTOR();
            ViewModelLocator.AccountPanel.IsEditorOpen = false;

            // ViewModelLocator.Application.OnConfigLoaded += this.Application_OnConfigLoaded;
            // ViewModelLocator.Application.LoadFromDisk();

            // string directory = ViewModelLocator.Application.UserSettings.SaveDirectory;
            // if (string.IsNullOrEmpty(directory)) {
            //    directory = PATH;
            // }

            ServiceLocator.AccountIO = new AccountIO(ViewModelLocator.Application.UserSettings.SaveDirectory);
            ViewModelLocator.Application.OnConfigLoaded += ApplicationOnOnConfigLoaded;

            ObservableCollection<AccountViewModel> acc = ViewModelLocator.AccountCollection.Accounts;
            IEnumerable<AccountModel> enumerable = ServiceLocator.AccountIO.ReadAccountsFromDisk((path, exception) => {
                MessageBox.Show($"Failed to read account (at path {path}) to disk. Reason: {exception.Message}", "Error");
            });

            foreach(AccountModel model in enumerable.OrderBy(d => d.Position)) {
                acc.Add(model.ToViewModel());
            }

            // List<AccountModel> load = new List<AccountModel>();
            // string dir = "C:\\Users\\kettl\\Documents\\IISExtended\\0x0000004TEMP\\NULL_ACCESS\\main";
            // string[] list_0 = File.ReadAllLines(Path.Combine(dir, "accName.txt"));
            // string[] list_1 = File.ReadAllLines(Path.Combine(dir, "email.txt"));
            // string[] list_2 = File.ReadAllLines(Path.Combine(dir, "usrName.txt"));
            // string[] list_3 = File.ReadAllLines(Path.Combine(dir, "pssWrd.txt"));
            // string[] list_4 = File.ReadAllLines(Path.Combine(dir, "DtoBrth.txt"));
            // string[] list_5 = File.ReadAllLines(Path.Combine(dir, "ScrtyInfo.txt"));
            // string[] list_6 = File.ReadAllLines(Path.Combine(dir, "ExtInf1.txt"));
            // string[] list_7 = File.ReadAllLines(Path.Combine(dir, "ExtInf2.txt"));
            // string[] list_8 = File.ReadAllLines(Path.Combine(dir, "ExtInf3.txt"));
            // string[] list_9 = File.ReadAllLines(Path.Combine(dir, "ExtInf4.txt"));
            // string[] list_10 = File.ReadAllLines(Path.Combine(dir, "ExtInf5.txt"));
            //
            // void doAdd(ref AccountModel model, string element) {
            //     if (!string.IsNullOrEmpty(element)) {
            //         model.Data.Add(element);
            //     }
            // }
            //
            // for(int i = 0; i < list_0.Length; i++) {
            //     AccountModel model = new AccountModel();
            //     model.Position = i;
            //     model.AccountName = list_0[i];
            //     model.Email = list_1[i];
            //     model.Username = list_2[i];
            //     model.Password = list_3[i];
            //     model.DateOfBirth = list_4[i];
            //     model.SecurityInfo = list_5[i];
            //     model.Data = new List<string>();
            //     doAdd(ref model, list_6[i]);
            //     doAdd(ref model, list_7[i]);
            //     doAdd(ref model, list_8[i]);
            //     doAdd(ref model, list_9[i]);
            //     doAdd(ref model, list_10[i]);
            //     load.Add(model);
            // }
            //
            // foreach (AccountModel model in load.OrderBy(d => d.Position)) {
            //     accounts.Add(model.ToViewModel());
            // }

            // Window window = new Window();
            // Ellipse ellipse = new Ellipse();
            // ellipse.Width = 220;
            // ellipse.Height = 100;
            // ellipse.Fill = new SolidColorBrush(Colors.Orange);
            // window.Content = ellipse;
            // window.ShowDialog();
        }

        private void ApplicationOnOnConfigLoaded(UserSettingsViewModel settings) {
            string directory = ViewModelLocator.Application.UserSettings.SaveDirectory;
            if (directory == null) {
                throw new NullReferenceException("Directory was null!");
            }

            ServiceLocator.AccountIO.Directory = directory;
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            if (e.Cancel) {
                return;
            }

            int count = 0;
            ServiceLocator.AccountIO.WriteAccountsToDisk(
                ViewModelLocator.AccountCollection.Accounts.Select(acc => new AccountModel(acc) { Position = count++ }).Where(d => d.HasBeenModified),
                (model, exception) => {
                    MessageBox.Show($"Failed to write account (named {model.AccountName}) to disk. Reason: {exception.Message}", "Error");
                    e.Cancel = true;
                });

            // unnecessary tbh... but oh wel
            Application.Current.Shutdown(0);
        }

        private void DoEditorAnimation(double from, double to) {
            // DoubleAnimation animation = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(150)) {
            //     AccelerationRatio = 0.05,
            //     DecelerationRatio = 0.95
            // };

            this.EditorColumn.Width = new GridLength(to);
            // this.EditorColumn.BeginAnimation(WidthProperty, animation);
            // this.EditorColumn.BeginAnimation(ColumnDefinitionAttachedProperties.AnimatableGridWidthProperty, animation);
        }

        private class EditorViewWrapper : IAccountEditor {
            private readonly MainWindow window;

            public EditorViewWrapper(MainWindow window) {
                this.window = window;
            }

            public bool IsOpen => this.window.isEditorOpen;
            
            public ICommand OpenViewCommand { get; }

            public ICommand CloseViewCommand { get; }

            public EditorViewWrapper() {
                this.OpenViewCommand = new RelayCommand(this.OpenView);
                this.CloseViewCommand = new RelayCommand(this.CloseView);
            }

            public void CloseView() {
                if (this.window.isEditorOpen) {
                    this.window.DoEditorAnimation(EDITOR_WIDTH_OPEN, EDITOR_WIDTH_CLOSE);
                    this.window.isEditorOpen = false;
                }
            }

            public void OpenView() {
                if (this.window.isEditorOpen) {
                    return;
                }

                this.window.DoEditorAnimation(EDITOR_WIDTH_CLOSE, EDITOR_WIDTH_OPEN);
                this.window.isEditorOpen = true;
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

            public void ScrollToAccount(AccountViewModel account) {
                this.window.AccountListBox.ScrollIntoView(account);
            }
        }

        private List<AccountModel> accounts;
        private void ListBox_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] paths) {
                this.accounts = new List<AccountModel>(paths.Length);
                foreach(string path in paths) {
                    // if it doesn't exist, or is bigger than 64K, then ignore it
                    if (!File.Exists(path) || new FileInfo(path).Length > 65535) {
                        continue;
                    }

                    try {
                        AccountModel model = new AccountModel();
                        using (FileStream stream = File.OpenRead(path)) {
                            stream.Seek(0, SeekOrigin.Begin);
                            // inefficient reading, but only for the preamble
                            StreamReader reader = new StreamReader(stream);
                            string preamble = reader.ReadBlock(AccountIO.PREAMBLE.Length);
                            if (preamble == null || preamble != AccountIO.PREAMBLE) {
                                continue;
                            }

                            // switch to buffered reading to read blocks of memory
                            BufferedStream buffered = new BufferedStream(stream, 1024);
                            reader = new StreamReader(buffered);
                            buffered.Seek(0, SeekOrigin.Begin);

                            try {
                                AccountIO.ReadAccountFromReader(ref model, reader);
                            }
                            catch(Exception fail) { // may not be a valid account file, so ignore it
                                continue;
                            }

                            this.accounts.Add(model);
                        }
                    }
                    catch(Exception ee) {
                        MessageBox.Show($"Failed to read file at path '{path}'. Reason: {ee.Message}", "Error reading file");
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
                foreach (AccountModel account in this.accounts) {
                    AccountViewModel acc = account.ToViewModel();
                    acc.MarkModified();
                    vm.AddNewAccount(acc, false);
                }

                this.accounts = null;
            }
        }

        private void GridSplitter_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            ViewModelLocator.AccountPanel.IsEditorOpen = !ViewModelLocator.AccountPanel.IsEditorOpen;
        }
    }
}
