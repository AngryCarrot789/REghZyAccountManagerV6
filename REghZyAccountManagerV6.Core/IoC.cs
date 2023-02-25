using System;
using REghZyAccountManagerV6.Core.Accounting;
using REghZyAccountManagerV6.Core.Accounting.Storage;
using REghZyAccountManagerV6.Core.Config;
using REghZyAccountManagerV6.Core.Services;
using REghZyAccountManagerV6.Core.Views;
using REghZyAccountManagerV6.Core.Views.Dialogs.FilePicking;
using REghZyAccountManagerV6.Core.Views.Dialogs.Message;
using REghZyAccountManagerV6.Core.Views.Dialogs.NewAccounts;
using REghZyAccountManagerV6.Core.Views.Dialogs.Progress;

namespace REghZyAccountManagerV6.Core {
    public static class IoC {
        public static SimpleIoC Instance { get; } = new SimpleIoC();

        public static IApplication Application {
            get => Instance.Provide<IApplication>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IAccountDatabase Database {
            get => Instance.Provide<IAccountDatabase>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IDispatcher Dispatcher {
            get => Instance.Provide<IDispatcher>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static INewAccountDialogService NewAccountService {
            get => Instance.Provide<INewAccountDialogService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IClipboardService Clipboard {
            get => Instance.Provide<IClipboardService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IMessageDialogService MessageDialogs {
            get => Instance.Provide<IMessageDialogService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IFilePickDialogService FilePicker {
            get => Instance.Provide<IFilePickDialogService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IConfigDialogService ConfigDialog {
            get => Instance.Provide<IConfigDialogService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IFindView FindView {
            get => Instance.Provide<IFindView>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IAccountList AccountList {
            get => Instance.Provide<IAccountList>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IProgressViewService ProgressViewService {
            get => Instance.Provide<IProgressViewService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IAccountActivityView AccountActivity {
            get => Instance.Provide<IAccountActivityView>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }
    }
}
