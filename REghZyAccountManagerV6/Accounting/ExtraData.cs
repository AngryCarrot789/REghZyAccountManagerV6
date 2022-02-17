using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Converters;
using REghZy.MVVM.Commands;
using REghZy.MVVM.ViewModels;

namespace REghZyAccountManagerV6.Accounting {
    public class ExtraData : BaseViewModel {
        public bool a;
        public bool b;
        public bool c;


        private string value;

        public string Value {
            get => this.value;
            set {
                RaisePropertyChanged(ref this.value, value);
                if (this.ExtraInfoView != null) {
                    this.ExtraInfoView.Account.MarkModified();
                }
                else {
                    Debug.WriteLine($"{a} {b} {c}");
                }
            }
        }

        public ICommand CopyToClipboard { get; }

        public ICommand DeleteItemCommand { get; }

        public ExtraInfoViewModel ExtraInfoView { get; set; }

        public ExtraData() {
            if (this.value == null) {
                this.value = "";
            }

            this.CopyToClipboard = new RelayCommand(()=> {
                if (string.IsNullOrEmpty(this.value)) {
                    return;
                }

                try {
                    Clipboard.SetText(this.value);
                }
                catch(Exception e) {
                    MessageBox.Show("Failed to set clipboard text: " + e.Message, "Clipboard error");
                }
            });

            this.DeleteItemCommand = new RelayCommand(() => {
                Remove();
            });
        }

        public ExtraData(ExtraInfoViewModel extraInfoView) : this() {
            this.value = "";
            this.ExtraInfoView = extraInfoView;
            this.a = true;
        }

        public ExtraData(string value) : this() {
            this.value = value;
            this.b = true;
        }

        public ExtraData(string value, ExtraInfoViewModel extraInfoView) : this() {
            this.value = value;
            this.ExtraInfoView = extraInfoView;
            this.c = true;
        }

        public bool Remove() {
            if (this.ExtraInfoView != null) {
                return this.ExtraInfoView.RemoveItem(this);
            }

            return false;
        }
    }
}
