using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using REghZy.MVVM.Views;
using REghZyAccountManagerV6.Accounting.IO;
using REghZyAccountManagerV6.Utils;

namespace REghZyAccountManagerV6.Accounting.Controls {
    public partial class LargeAccountControl : UserControl, BaseView<AccountViewModel> {
        private const int MOVE_THRESHOLD_X = 2;
        private const int MOVE_THRESHOLD_Y = 2;

        private bool mouseDown = false;
        private bool isDragDropping = false;
        private Point click;

        public AccountViewModel Model {
            get => (AccountViewModel) this.DataContext;
            set => this.DataContext = value;
        }

        public LargeAccountControl() {
            InitializeComponent();
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e) {
            if (!this.mouseDown || this.isDragDropping) {
                return;
            }

            if (this.DataContext is AccountViewModel account) {
                // do drag drop
                Point mouse = e.GetPosition(this);
                if (Math.Abs(mouse.X - this.click.X) >= MOVE_THRESHOLD_X || Math.Abs(mouse.Y - this.click.Y) >= MOVE_THRESHOLD_Y) {
                    string temp = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(FileHelper.GetValidAccountFileName(account), AccountIO.FILE_EXTENSION));
                    using (StreamWriter stream = new StreamWriter(new BufferedStream(File.OpenWrite(temp)))) {
                        AccountIO.WriteAccountToWriter(account, stream);
                        stream.Flush();
                    }

                    this.isDragDropping = true;
                    DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, new string[] { temp }), DragDropEffects.Copy);
                    this.mouseDown = false;
                    this.isDragDropping = false;

                    if (File.Exists(temp)) {
                        File.Delete(temp);
                    }
                }
            }
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton != MouseButton.Left) {
                return;
            }

            this.click = e.GetPosition(this);
            this.mouseDown = true;
        }

        private void Rectangle_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.mouseDown = false;
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e) {
            this.mouseDown = false;
        }
    }
}