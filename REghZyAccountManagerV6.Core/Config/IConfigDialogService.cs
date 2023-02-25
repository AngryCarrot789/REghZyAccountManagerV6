using System.Threading.Tasks;
using REghZyAccountManagerV6.Core.Views.Dialogs;

namespace REghZyAccountManagerV6.Core.Config {
    public interface IConfigDialogService {
        /// <summary>
        /// Takes a configuration, shows the UI, and returns a newly modified config (containing new values)
        /// </summary>
        Task<DialogResult<Configuration>> ShowEditConfigDialogAsync(Configuration configuration);
    }
}