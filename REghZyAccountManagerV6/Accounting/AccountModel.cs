using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace REghZyAccountManagerV6.Accounting {
    public class AccountModel {
        // Not using XML anymore, because it's very dodgy
        // keeping the XML attributes just incase i add it back some day...
        // and so that i remember how to actually use XML serialisation :-)

        public bool HasBeenModified { get; set; }

        [XmlIgnore]
        public string FilePath { get; set; }

        [XmlAttribute("Position")] // position within the listbox. used to order during loading
        public int Position { get; set; }

        [XmlAttribute("AccountName")]
        public string AccountName { get; set; }

        [XmlAttribute("Email")]
        public string Email { get; set; }

        [XmlAttribute("Username")]
        public string Username { get; set; }

        [XmlAttribute("Password")]
        public string Password { get; set; }

        [XmlAttribute("DateOfBirth")]
        public string DateOfBirth { get; set; }

        [XmlAttribute("SecurityInfo")]
        public string SecurityInfo { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Item")]
        public List<string> Data { get; set; }

        public AccountModel() {

        }

        public AccountModel(string filePath, string accountName, string email, string username, string password, string dateOfBirth, string securityInfo, bool hasBeenModified, List<string> extraData) {
            this.FilePath = filePath;
            this.AccountName = accountName;
            this.Email = email;
            this.Username = username;
            this.Password = password;
            this.DateOfBirth = dateOfBirth;
            this.SecurityInfo = securityInfo;
            this.Data = extraData;
            this.HasBeenModified = hasBeenModified;
        }

        public AccountModel(AccountViewModel account) {
            this.FilePath = account.FilePath;
            this.AccountName = account.AccountName;
            this.Email = account.Email;
            this.Username = account.Username;
            this.Password = account.Password;
            this.DateOfBirth = account.DateOfBirth;
            this.SecurityInfo = account.SecurityInfo;
            this.HasBeenModified = account.HasBeenModified;
            this.Data = account.ExtraInfo.ExtraInformation.Select(d => d.Value).ToList();
        }

        public AccountViewModel ToViewModel() {
            return new AccountViewModel(this.FilePath, this.AccountName, this.Email, this.Username, this.Password, this.DateOfBirth, this.SecurityInfo, this.Data) {
                HasBeenModified = this.HasBeenModified
            };
        }
    }
}