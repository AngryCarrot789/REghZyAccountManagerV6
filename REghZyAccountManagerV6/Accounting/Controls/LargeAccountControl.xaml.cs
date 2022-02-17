using System;
using System.Windows;
using System.Windows.Controls;

namespace REghZyAccountManagerV6.Accounting.Controls {
    public partial class LargeAccountControl : UserControl {
        private const int MOVE_THRESHOLD_X = 3;
        private const int MOVE_THRESHOLD_Y = 3;

        private bool mouseDown = false;
        private bool isDragDropping = false;
        private Point click;

        public LargeAccountControl() {
            InitializeComponent();
        }

        private void Rectangle_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            if (!this.mouseDown) {
                return;
            }

            Point mouse = e.GetPosition(this);
            if (Math.Abs(mouse.X - this.click.X) > MOVE_THRESHOLD_X && Math.Abs(mouse.Y - this.click.Y) > MOVE_THRESHOLD_Y) {
                this.isDragDropping = true;
                // do drag drop

                this.mouseDown = false;
            }
        }

        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.click = e.GetPosition(this);
            this.mouseDown = true;
        }

        private void Rectangle_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.mouseDown = false;
        }
    }
}