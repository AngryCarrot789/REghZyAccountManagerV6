using REghZy.MVVM.ViewModels;

namespace REghZyAccountManagerV6.ViewModels {
    public class AccountEditorViewModel : BaseViewModel {
        private double editorWidth;

        public double EditorWidth {
            get => this.editorWidth;
            set => RaisePropertyChanged(ref this.editorWidth, value, true);
        }
    }
}