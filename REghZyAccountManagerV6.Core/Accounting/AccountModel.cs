using System;

namespace REghZyAccountManagerV6.Core.Accounting {
    public class AccountModel {
        public bool HasBeenModified { get; set; }
        public int Position { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DateOfBirth { get; set; }
        public string SecurityInfo { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string CustomInfo { get; set; }

        public AccountModel() {

        }

        public AccountModel(AccountViewModel account) {
            this.AccountName = account.AccountName;
            this.Email = account.Email;
            this.Username = account.Username;
            this.Password = account.Password;
            this.DateOfBirth = account.DateOfBirth;
            this.SecurityInfo = account.SecurityInfo;
            this.HasBeenModified = account.HasBeenModified;
            this.CustomInfo = account.CustomInfo;
        }

        public AccountViewModel ToViewModel() {
            return new AccountViewModel(this.AccountName, this.Email, this.Username, this.Password, this.DateOfBirth, this.SecurityInfo, this.CustomInfo);
        }
    }
}