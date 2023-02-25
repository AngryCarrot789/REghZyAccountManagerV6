using System.Threading.Tasks;
using System.Windows;
using REghZyAccountManagerV6.Core.Config;
using REghZyAccountManagerV6.Core.Views.Dialogs;

namespace REghZyAccountManagerV6.Views.Settings {
    public class ConfigDialogService : IConfigDialogService {
        public async Task<DialogResult<Configuration>> ShowEditConfigDialogAsync(Configuration configuration) {
            return await Application.Current.Dispatcher.InvokeAsync(() => {
                UserSettingsDialog window = new UserSettingsDialog(configuration);
                if (window.ShowDialog() == true) {
                    return new DialogResult<Configuration>(window.ViewModel.ModifiedConfiguration);
                }

                return new DialogResult<Configuration>(false);
            });
        }
    }
}