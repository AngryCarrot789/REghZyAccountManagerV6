using System.Windows.Input;
using REghZyAccountManagerV6.Core.Views.Dialogs;

namespace REghZyAccountManagerV6.Core {
    public abstract class BaseConfirmableDialogViewModel : BaseDialogViewModel {
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        protected BaseConfirmableDialogViewModel(IDialog dialog) : base(dialog) {
            this.ConfirmCommand = new RelayCommand(this.ConfirmAction);
            this.CancelCommand = new RelayCommand(this.CancelAction);
        }

        public virtual void ConfirmAction() {
            this.Dialog.CloseDialog(true);
        }

        public virtual void CancelAction() {
            this.Dialog.CloseDialog(false);
        }
    }
}