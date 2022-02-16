namespace REghZyAccountManagerV6.Views {
    public interface IView {
        /// <summary>
        /// Whether the view is open or not
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Opens the view. If it's already open, nothing happens
        /// </summary>
        void OpenView();

        /// <summary>
        /// Closes the view. If it's already closed, nothing happens
        /// </summary>
        void CloseView();
    }
}