using Microsoft.Win32;
using REghZyAccountManagerV6.Core.Views.Dialogs;
using REghZyAccountManagerV6.Core.Views.Dialogs.FilePicking;
using REghZyAccountManagerV6.Utils;

namespace REghZyAccountManagerV6.Services {
    public class FilePickDialogService : IFilePickDialogService {
        public DialogResult<string[]> ShowFilePickerDialogAsync(string filter, string defaultPath = null, string titleBar = null, bool multiSelect = false) {
            OpenFileDialog dialog = new OpenFileDialog {
                Filter = filter,
                Multiselect = multiSelect,
                Title = titleBar ?? "Select a file"
            };

            if (defaultPath != null) {
                dialog.InitialDirectory = defaultPath;
            }

            return dialog.ShowDialog() == true ? new DialogResult<string[]>(dialog.FileNames) : new DialogResult<string[]>(false);
        }

        public DialogResult<string> ShowFolderPickerDialogAsync(string defaultPath = null, string titleBar = null) {
            FolderPicker picker = new FolderPicker {
                Title = titleBar ?? "Select a folder"
            };

            if (defaultPath != null) {
                picker.InputPath = defaultPath;
            }

            return picker.ShowDialog() == true ? new DialogResult<string>(picker.ResultPath) : new DialogResult<string>(false);
        }
    }
}