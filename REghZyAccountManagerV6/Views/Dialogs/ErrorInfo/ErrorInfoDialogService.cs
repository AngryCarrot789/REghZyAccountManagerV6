using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using REghZyAccountManagerV6.Core.Views.Dialogs;

namespace REghZyAccountManagerV6.Views.Dialogs.ErrorInfo {
    public class ErrorInfoDialogService : IErrorInfoDialogService {
        public async Task ShowDialogAsync(IEnumerable<Tuple<string, string>> errors) {
            ErrorInfoDialog dialog = new ErrorInfoDialog(errors);
            await Application.Current.Dispatcher.InvokeAsync(() => dialog.ShowDialog());
        }
    }
}