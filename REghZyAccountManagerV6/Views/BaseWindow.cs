using System.Windows;
using REghZyAccountManagerV6.Core.Views;

namespace REghZyAccountManagerV6.Views {
    public class BaseWindow : Window, IWindow {
        public void CloseWindow() {
            this.Dispatcher.Invoke(this.Close);
        }
    }
}