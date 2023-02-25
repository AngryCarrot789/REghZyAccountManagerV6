using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using REghZyAccountManagerV6.Accounting;
using REghZyAccountManagerV6.Core;
using REghZyAccountManagerV6.Core.Accounting;
using REghZyAccountManagerV6.Core.Config;
using REghZyAccountManagerV6.Core.Views.Dialogs;
using REghZyAccountManagerV6.Core.Views.Dialogs.Message;
using REghZyAccountManagerV6.Core.Views.Dialogs.Progress;

namespace REghZyAccountManagerV6.Views.MainView {
    public class MainViewModel : BaseViewModel {
        /// <summary>
        /// The panel, on the left. Responsible for handling adding/removing/searching/etc
        /// </summary>
        public AccountPanelViewModel Panel { get; }

        /// <summary>
        /// The view model that handles the collections of items
        /// </summary>
        public AccountCollectionViewModel Collection { get; }

        public ICommand FocusFindCommand { get; }

        public ICommand OpenPreferencesCommand { get; }
        public ICommand LoadAccountsCommand { get; }
        public ICommand SaveAccountsCommand { get; }

        public MainViewModel() {
            ViewModelLocator.AccountCollection = this.Collection = new AccountCollectionViewModel();
            ViewModelLocator.AccountPanel = this.Panel = new AccountPanelViewModel(this.Collection);
            this.FocusFindCommand = new RelayCommand(()=> {
                IoC.FindView.FocusInput();
            });

            this.OpenPreferencesCommand = new RelayCommand(async () => await this.OpenPreferencesAction());
            this.LoadAccountsCommand = new RelayCommand(async () => await this.LoadAccountsAction());
            this.SaveAccountsCommand = new RelayCommand(async () => await this.SaveAccountsAction());
        }

        public async Task LoadAccountsAction() {
            if (string.IsNullOrEmpty(Configuration.AccountSaveFile) || !File.Exists(Configuration.AccountSaveFile)) {
                await IoC.MessageDialogs.ShowMessageAsync("Save dir does not exist", "The account save directory does not exist or is invalid. Please select one in \"File > Preferences\"");
                return;
            }

            try {
                IoC.AccountActivity.IsVisible = true;
                IoC.AccountActivity.Message = "Loading accounts..";
                ObservableCollection<AccountViewModel> list = ViewModelLocator.AccountCollection.Accounts;
                IEnumerable<AccountModel> enumerable = IoC.Database.ReadAccounts();
                list.Clear();
                foreach (AccountModel model in enumerable.OrderBy(d => d.Position)) {
                    list.Add(model.ToViewModel());
                }
            }
            catch (Exception e) {
                Debug.WriteLine(e);
                await IoC.MessageDialogs.ShowMessageAsync("Failed to read accounts", e.Message);
            }
            finally {
                IoC.AccountActivity.IsVisible = false;
                IoC.AccountActivity.Message = "";
            }
        }

        public async Task SaveAccountsAction() {
            if (string.IsNullOrEmpty(Configuration.AccountSaveFile)) {
                MsgDialogResult output = await IoC.MessageDialogs.ShowDialogAsync(
                    "Save to default file?",
                    $"The account save file was not set. Do you want to set it to ({App.DEFAULT_ACCOUNTS_FILE}) and save there?",
                    MsgDialogType.OKCancel);
                if (output == MsgDialogResult.OK) {
                    try {
                        Configuration.AccountSaveFile = App.DEFAULT_ACCOUNTS_FILE;
                    }
                    catch (Exception ex) {
                        await IoC.MessageDialogs.ShowMessageAsync("Failed to create dir", $"Failed to create directory at {Configuration.AccountSaveFile}. The app will not close, so write down your stuff!\n{ex.Message}");
                        return;
                    }
                }
                else {
                    return;
                }
            }

            IEnumerable<AccountModel> modified = ViewModelLocator.AccountCollection.AccountModels;
            try {
                IoC.AccountActivity.IsVisible = true;
                IoC.AccountActivity.Message = "Saving accounts";
                IoC.Database.WriteAccounts(modified);
                ViewModelLocator.AccountCollection.MarkNoneModified();
            }
            catch (Exception e) {
                Debug.WriteLine(e);
                await IoC.MessageDialogs.ShowMessageAsync("Failed to write accounts", e.ToString());
            }
            finally {
                IoC.AccountActivity.IsVisible = false;
                IoC.AccountActivity.Message = "";
            }
        }

        public async Task OpenPreferencesAction() {
            DialogResult<Configuration> result = await IoC.ConfigDialog.ShowEditConfigDialogAsync(App.Instance.Configuration);
            if (result.IsSuccess) {
                IoC.Application.Configuration = result.Value;
                try {
                    result.Value.Save(IoC.Application.ConfigPath);
                }
                catch (Exception ex) {
                    await IoC.MessageDialogs.ShowMessageAsync("Failed to save config", $"Failed to save config to {IoC.Application.ConfigPath}\n{ex}");
                }

                this.Collection.MarkAllModified();
            }
        }
    }
}