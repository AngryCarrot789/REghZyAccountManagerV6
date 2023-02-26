using System.Threading.Tasks;
using REghZyAccountManagerV6.Core.Views.Dialogs;

namespace REghZyAccountManagerV6.Views {
    public class BaseDialog : BaseWindowCore, IDialog {
        public void CloseDialog(bool result) {
            this.DialogResult = result;
            this.Close();
        }

        public async Task CloseDialogAsync(bool result) {
            await this.Dispatcher.InvokeAsync(() => this.CloseDialog(result));
        }
    }
}