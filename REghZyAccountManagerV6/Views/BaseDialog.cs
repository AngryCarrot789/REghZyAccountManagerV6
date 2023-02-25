using System.Windows;
using REghZyAccountManagerV6.Core.Views.Dialogs;

namespace REghZyAccountManagerV6.Views {
    public class BaseDialog : Window, IDialog {
        public void CloseDialog(bool result) {
            this.DialogResult = result;
            Application.Current.Dispatcher.Invoke(this.Close);
        }
    }
}