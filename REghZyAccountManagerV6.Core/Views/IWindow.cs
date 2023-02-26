using System.Threading.Tasks;

namespace REghZyAccountManagerV6.Core.Views {
    public interface IWindow : IViewBase {
        void CloseWindow();

        Task CloseWindowAsync();
    }
}