namespace REghZyAccountManagerV6.Views.Dialogs.NewAccount {
    /// <summary>
    /// Interaction logic for NewAccountWindow.xaml
    /// </summary>
    public partial class NewAccountDialog : BaseDialog {
        public NewAccountViewModel ViewModel => (NewAccountViewModel) this.DataContext;

        public NewAccountDialog() {
            this.InitializeComponent();
            this.DataContext = new NewAccountViewModel(this);
        }
    }
}
