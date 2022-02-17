using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Input;
using REghZy.MVVM.Commands;
using REghZy.MVVM.ViewModels;

namespace REghZyAccountManagerV6.Accounting {
    public class ExtraInfoViewModel : BaseViewModel {
        private ExtraData selectedItem;

        public ExtraData SelectedItem {
            get => this.selectedItem;
            set => RaisePropertyChanged(ref this.selectedItem, value);
        }

        private string toAdd;

        public string ToAdd {
            get => this.toAdd;
            set => RaisePropertyChanged(ref this.toAdd, value);
        }

        public ObservableCollection<ExtraData> ExtraInformation { get; }

        public ICommand AddItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        public ICommand PasteItemCommand { get; }

        public AccountViewModel Account { get; }

        public ExtraInfoViewModel(AccountViewModel account, IEnumerable<ExtraData> extraData) {
            this.Account = account;
            this.AddItemCommand = new RelayCommand(AddItem);
            this.RemoveItemCommand = new RelayCommand(RemoveItem);
            this.PasteItemCommand = new RelayCommand(PasteItem);

            List<ExtraData> data = extraData.ToList();
            foreach (ExtraData item in data) {
                item.ExtraInfoView = this;
            }

            this.ExtraInformation = new ObservableCollection<ExtraData>(data);
            this.ExtraInformation.CollectionChanged += (sender, args) => this.Account.MarkModified();
        }

        public void SplitSelectedInto2(int caretIndex) {
            ExtraData selected = this.selectedItem;
            int index = this.ExtraInformation.IndexOf(selected);
            if (index == -1) {
                return;
            }

            string splitText = selected.Value.Substring(caretIndex);
            string prevText = selected.Value.Substring(0, caretIndex);
            selected.Value = prevText;
            this.ExtraInformation.Insert(index + 1, selected = new ExtraData(splitText, this));
            this.SelectedItem = selected;
        }

        public bool RemoveItem(ExtraData extraData) {
            return this.ExtraInformation.Remove(extraData);
        }

        private void AddItem() {
            if (string.IsNullOrWhiteSpace(this.toAdd)) {
                return;
            }

            AddItem(new ExtraData(this.toAdd, this));
            this.ToAdd = "";
        }

        private void AddItem(ExtraData data) {
            data.ExtraInfoView = this;
            this.ExtraInformation.Add(data);
        }

        private void RemoveItem() {
            if (this.selectedItem == null) {
                return;
            }

            this.ExtraInformation.Remove(this.selectedItem);
        }

        private void PasteItem() {
            try {
                string clip = Clipboard.GetText();
                if (string.IsNullOrEmpty(clip)) {
                    return;
                }

                AddItem(new ExtraData(clip));
            }
            catch(Exception e) {
                MessageBox.Show("Failed to get clipboard text: " + e.Message, "Clipboard Error");
            }
        }
    }
}
