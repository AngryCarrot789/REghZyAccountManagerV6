using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using REghZyAccountManagerV6.Accounting;
using REghZyAccountManagerV6.Accounting.IO;
using REghZyAccountManagerV6.Views.NewAccounts;

namespace REghZyAccountManagerV6.Views.MainView {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private bool isEditorOpen;
        private const double EDITOR_WIDTH_CLOSE = 0;
        private const double EDITOR_WIDTH_OPEN = 350;

        public MainWindow() {
            InitializeComponent();
            ServiceLocator.Editor = new EditorViewWrapper(this);
            ServiceLocator.NewAccount = new NewAccountWindow();
            ViewModelLocator.AccountPanel.IsEditorOpen = this.isEditorOpen = true;
            ServiceLocator.AccountIO = new AccountIO("C:\\Users\\kettl\\Desktop\\ElloThere");

            ObservableCollection<AccountViewModel> accounts = ViewModelLocator.AccountCollection.Accounts;
            IEnumerable<AccountModel> enumerable = ServiceLocator.AccountIO.ReadAccountsFromDisk((path, exception) => {
                MessageBox.Show($"Failed to read account (at path {path}) to disk. Reason: {exception.Message}", "Error");
            });

            foreach(AccountModel model in enumerable.OrderBy(d => d.Position)) {
                accounts.Add(model.ToViewModel());
            }

            // Window window = new Window();
            // Ellipse ellipse = new Ellipse();
            // ellipse.Width = 220;
            // ellipse.Height = 100;
            // ellipse.Fill = new SolidColorBrush(Colors.Orange);
            // window.Content = ellipse;
            // window.ShowDialog();
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            if (e.Cancel) {
                return;
            }

            int a = 0;
            ServiceLocator.AccountIO.WriteAccountsToDisk(
                ViewModelLocator.AccountCollection.Accounts.Select(acc => new AccountModel(acc) { Position = a++ }),
                (model, exception) => {
                    MessageBox.Show($"Failed to write account (named {model.AccountName}) to disk. Reason: {exception.Message}", "Error");
                    e.Cancel = true;
                });

            Application.Current.Shutdown(0);
        }

        private void DoEditorAnimation(double from, double to) {
            DoubleAnimation animation = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(150)) {
                AccelerationRatio = 0.05,
                DecelerationRatio = 0.95
            };

            this.EditorView.BeginAnimation(WidthProperty, animation);
        }

        private class EditorViewWrapper : IAccountEditor {
            private readonly MainWindow window;

            public EditorViewWrapper(MainWindow window) {
                this.window = window;
            }

            public bool IsOpen => this.window.isEditorOpen;

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
    }
}
