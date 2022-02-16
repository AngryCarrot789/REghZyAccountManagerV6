using REghZyAccountManagerV6.Accounting;

namespace REghZyAccountManagerV6.Views {
    public interface INewAccount : IView {
        /// <summary>
        /// The account model that the new account view has created
        /// </summary>
        AccountViewModel Account { get; set; }

        bool IsAccountValid { get; }
    }
}