using System.Threading.Tasks;
using REghZyAccountManagerV6.Core.Views;

namespace REghZyAccountManagerV6.Views {
    public class BaseWindow : BaseWindowCore, IWindow {
        public void CloseWindow() {
            this.Close();
        }

        public async Task CloseWindowAsync() {
            await this.Dispatcher.InvokeAsync(this.CloseWindow);
        }
    }
}