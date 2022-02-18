using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using REghZyAccountManagerV6.Utils;

namespace REghZyAccountManagerV6.Accounting.IO {
    public class AccountIO {
        public const string PREAMBLE = "ThisIsAnActualAccount";
        public const string FILE_EXTENSION = "txt";
        public const string FILE_EXTENSION_DOT = ".txt";
        public const string FILE_EXTENSION_FILTER = "*.txt";

        public XmlSerializer XmlSerializer { get; }
        public string Directory { get; set; }

        private ICollection<string> visitedPathsNoExtension;

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

        public string GetUniqueAccountPath(string accountName, string defaultPath) {
            string path = defaultPath;
            if (string.IsNullOrEmpty(defaultPath) || !File.Exists(defaultPath)) {
                path = Path.Combine(this.Directory, FileHelper.GetValidAccountFileName(accountName));
            }

            ICollection<string> visited = this.visitedPathsNoExtension;
            if (visited != null) {
                path = Path.ChangeExtension(path, null);
                if (visited.Contains(path)) {
                    while (true) {
                        path += '_';
                        if (!File.Exists(path + FILE_EXTENSION_DOT)) {
                            break;
                        }
                    }
                }

                visited.Add(path);
            }

            return Path.ChangeExtension(path, FILE_EXTENSION);
        }

        public IEnumerable<AccountModel> ReadAccountsFromDisk(Action<string, Exception> errorHandler) {
            foreach (string path in System.IO.Directory.EnumerateFiles(this.Directory, FILE_EXTENSION_FILTER)) {
                AccountModel item = new AccountModel();
                try {
                    // XML
                    // using (BufferedStream stream = new BufferedStream(File.OpenRead(path), 256)) {
                    //     model = (AccountModel) deserialiser.Deserialize(stream);
                    // }

                    // Mine
                    using (StreamReader reader = new StreamReader(new BufferedStream(File.OpenRead(path), 256))) {
                        ReadAccountFromReader(ref item, reader);
                    }
                }
                catch (Exception e) {
                    errorHandler(path, e);
                    item = null;
                }

                if (item == null) {
                    continue;
                }

                item.FilePath = path;
                yield return item;
            }
        }

        public void WriteAccountsToDisk(IEnumerable<AccountModel> accounts, Action<AccountModel, Exception> errorHandler) {
            this.visitedPathsNoExtension = new List<string>();
            foreach (AccountModel item in accounts) {
                try {
                    string path = GetUniqueAccountPath(item.AccountName, item.FilePath);
                    // XML
                    // using (BufferedStream stream = new BufferedStream(File.OpenWrite(path), 1024)) {
                    //     serializer.Serialize(stream, item);
                    // }

                    // Mine
                    using (StreamWriter writer = new StreamWriter(new BufferedStream(File.OpenWrite(path), 1024))) {
                        WriteAccountToWriter(item, writer);
                    }
                }
                catch (Exception e) {
                    errorHandler(item, e);
                }
            }

            this.visitedPathsNoExtension = null;
        }

        public static void WriteAccountToWriter(AccountViewModel account, TextWriter stream) {
            WriteAccountToWriter(new AccountModel(account), stream);
        }

        public static void WriteAccountToWriter(AccountModel account, TextWriter writer) {
            writer.WriteLine(PREAMBLE);
            writer.WriteLine(account.Position);
            writer.WriteLine(account.AccountName);
            writer.WriteLine(account.Email);
            writer.WriteLine(account.Username);
            writer.WriteLine(account.Password);
            writer.WriteLine(account.DateOfBirth);
            writer.WriteLine(account.SecurityInfo);
            foreach (string line in account.Data) {
                writer.WriteLine(line);
            }
        }

        public static void ReadAccountFromReader(out AccountViewModel account, TextReader reader, bool readPreamble) {
            AccountModel model = new AccountModel();
            ReadAccountFromReader(ref model, reader);
            account = model.ToViewModel();
        }

        public static void ReadAccountFromReader(ref AccountModel account, TextReader reader, bool readPreamble = true) {
            // ReadLine() returns null if there's no more data. It returns an empty
            // string if there is more data, but the line is just empty
            if (readPreamble) {
                string preamble = reader.ReadLine();
                if (preamble == null) {
                    throw new EndOfStreamException("Not enough data to read preamble");
                }
                else if (preamble != PREAMBLE) {
                    throw new Exception("Preamble was invalid (it was '{preamble}')");
                }
            }

            account.Position = int.TryParse(reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read account position"), out int pos) ? pos : 0;
            account.AccountName = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read account name");
            account.Email = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read email");
            account.Username = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read username");
            account.Password = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read password");
            account.DateOfBirth = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read date of birth");
            account.SecurityInfo = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read security info");
            List<string> data = new List<string>();
            string line = reader.ReadLine();
            while (!string.IsNullOrWhiteSpace(line)) {
                data.Add(line);
                line = reader.ReadLine();
            }

            account.Data = data;
        }
    }
}