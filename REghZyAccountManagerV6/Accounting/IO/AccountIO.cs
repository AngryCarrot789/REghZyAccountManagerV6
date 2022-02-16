using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using REghZyAccountManagerV6.Utils;

namespace REghZyAccountManagerV6.Accounting.IO {
    public class AccountIO {

        public const string FILE_EXTENSION = "rzdat";
        public const string FILE_EXTENSION_DOT = ".rzdat";
        public const string FILE_EXTENSION_FILTER = "*.rzdat";

        public XmlSerializer XmlSerializer { get; }
        public string Directory { get; }

        public AccountIO(string directory) {
            this.Directory = directory;
            this.XmlSerializer = new XmlSerializer(typeof(AccountModel));
            EnsureDirectoriesExists(false);
        }

        public bool EnsureDirectoriesExists(bool force = false) {
            if (!System.IO.Directory.Exists(this.Directory)) {
                if (force || MessageBox.Show($"The main accounts directory doesn't exist. Do you want to create it?\nThe path will be: {this.Directory}",
                                             "Create directory?", MessageBoxButton.YesNo,
                                             MessageBoxImage.Exclamation,
                                             MessageBoxResult.Yes) == MessageBoxResult.Yes) {
                    System.IO.Directory.CreateDirectory(this.Directory);
                    return true;
                }
                else {
                    return false;
                }
            }

            return false;
        }

        public string GetAccountPath(string accountName, string defaultPath = null) {
            string path = defaultPath;
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) {
                path = Path.ChangeExtension(Path.Combine(this.Directory, FileHelper.GetValidAccountFileName(accountName)), FILE_EXTENSION);
            }

            return path;
        }

        public IEnumerable<AccountModel> ReadAccountsFromDisk(Action<string, Exception> errorHandler) {
            XmlSerializer deserialiser = this.XmlSerializer;
            foreach (string path in System.IO.Directory.EnumerateFiles(this.Directory, FILE_EXTENSION_FILTER)) {
                AccountModel item = new AccountModel();
                try {
                    // XML
                    // using (BufferedStream stream = new BufferedStream(File.OpenRead(path), 256)) {
                    //     model = (AccountModel) deserialiser.Deserialize(stream);
                    // }

                    // Mine
                    using (StreamReader reader = new StreamReader(new BufferedStream(File.OpenRead(path), 256))) {
                        item.Position = int.TryParse(reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read account position"), out int pos) ? pos : 0;
                        item.AccountName = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read account name");
                        item.Email = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read email");
                        item.Username = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read username");
                        item.Password = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read password");
                        item.DateOfBirth = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read date of birth");
                        item.SecurityInfo = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read security info");
                        List<string> data = new List<string>();
                        string line = reader.ReadLine();
                        while (!string.IsNullOrWhiteSpace(line)) {
                            data.Add(line);
                            line = reader.ReadLine();
                        }

                        item.Data = data;
                    }
                }
                catch (Exception e) {
                    errorHandler(path, e);
                }

                if (item == null) {
                    continue;
                }

                item.FilePath = path;
                yield return item;
            }
        }

        public void WriteAccountsToDisk(IEnumerable<AccountModel> accounts, Action<AccountModel, Exception> errorHandler) {
            XmlSerializer serializer = this.XmlSerializer;
            foreach (AccountModel item in accounts) {
                try {
                    string path = GetAccountPath(item.AccountName, item.FilePath);
                    // XML
                    // using (BufferedStream stream = new BufferedStream(File.OpenWrite(path), 1024)) {
                    //     serializer.Serialize(stream, item);
                    // }

                    // Mine
                    using (StreamWriter writer = new StreamWriter(new BufferedStream(File.OpenWrite(path), 1024))) {
                        writer.WriteLine(item.Position);
                        writer.WriteLine(item.AccountName);
                        writer.WriteLine(item.Email);
                        writer.WriteLine(item.Username);
                        writer.WriteLine(item.Password);
                        writer.WriteLine(item.DateOfBirth);
                        writer.WriteLine(item.SecurityInfo);
                        foreach (string line in item.Data) {
                            writer.WriteLine(line);
                        }
                    }
                }
                catch (Exception e) {
                    errorHandler(item, e);
                }
            }
        }

        public void SaveAccountXML(AccountViewModel account, int index) {
            AccountModel item = new AccountModel(account);
            string path = GetAccountPath(item.AccountName, item.FilePath);
            using (BufferedStream stream = new BufferedStream(File.OpenWrite(path), 1024)) {
                this.XmlSerializer.Serialize(stream, item);
            }
        }
    }
}