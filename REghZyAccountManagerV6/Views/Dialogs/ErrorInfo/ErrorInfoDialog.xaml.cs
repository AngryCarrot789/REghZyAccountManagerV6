using System;
using System.Collections.Generic;

namespace REghZyAccountManagerV6.Views.Dialogs.ErrorInfo {
    public partial class ErrorInfoDialog : BaseDialog {
        public ErrorInfoDialog(IEnumerable<Tuple<string,string>> errors) {
            this.InitializeComponent();
            this.DataContext = new ErrorInfoViewModel(this, errors);
        }
    }
}