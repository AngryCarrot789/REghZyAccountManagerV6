using System;
using REghZy.MVVM.IoC;
using REghZyAccountManagerV6.Accounting.IO;
using REghZyAccountManagerV6.Views;
using REghZyAccountManagerV6.Views.MainView;

namespace REghZyAccountManagerV6 {
    public static class ServiceLocator {
        private static readonly SimpleIoC IoC = new SimpleIoC();

        public static IAccountEditor Editor {
            get => IoC.GetService<IAccountEditor>();
            set {
                if (value == null) {
                    throw new ArgumentNullException("value", "Account editor cannot be set to null");
                }

                IoC.SetService(value);
            }
        }

        public static INewAccount NewAccount {
            get => IoC.GetService<INewAccount>();
            set {
                if (value == null) {
                    throw new ArgumentNullException("value", "New Account view cannot be set to null");
                }

                IoC.SetService(value);
            }
        }

        public static IFindView FindView {
            get => IoC.GetService<IFindView>();
            set {
                if (value == null) {
                    throw new ArgumentNullException("value", "New find view cannot be set to null");
                }

                IoC.SetService(value);
            }
        }

        public static AccountIO AccountIO {
            get => IoC.GetService<AccountIO>();
            set {
                if (IoC.HasService<AccountIO>()) {
                    throw new ArgumentNullException("value", "AccountIO has already been set, it cannot be set again");
                }

                IoC.SetService(value);
            }
        }

        public static void InitCTOR() { }
    }
}