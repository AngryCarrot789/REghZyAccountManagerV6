using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using REghZyAccountManagerV6.Core;
using REghZyAccountManagerV6.Core.Views.Dialogs;

namespace REghZyAccountManagerV6.Views.Dialogs.ErrorInfo {
    public class ErrorInfoViewModel : BaseDialogViewModel {
        public ObservableCollection<Tuple<string, string>> Errors { get; }

        public ICommand CloseDialogCommand { get; }

        public ErrorInfoViewModel(IDialog dialog, IEnumerable<Tuple<string, string>> errors) : base(dialog) {
            this.Errors = new ObservableCollection<Tuple<string, string>>();
            foreach (Tuple<string,string> error in errors) {
                this.Errors.Add(error);
            }

            this.CloseDialogCommand = new RelayCommand(this.CloseDialogAction);
        }

        public void CloseDialogAction() {
            this.Dialog.CloseDialog(true);
        }
    }
}