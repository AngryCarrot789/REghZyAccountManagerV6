using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using REghZyAccountManagerV6.Core.Config;
using REghZyAccountManagerV6.Core.Utils;

namespace REghZyAccountManagerV6.Core.Accounting {
    public static class AccountIO {
        private static readonly ConfigKey<int> AccountPositionKey = new ConfigKey<int>("pos");
        private static readonly ConfigKey<string> AccountNameKey = new ConfigKey<string>("accname");
        private static readonly ConfigKey<string> EmailKey = new ConfigKey<string>("email");
        private static readonly ConfigKey<string> UsernameKey = new ConfigKey<string>("usrnm");
        private static readonly ConfigKey<string> PasswordKey = new ConfigKey<string>("pswrd");
        private static readonly ConfigKey<string> DateOfBirthKey = new ConfigKey<string>("dob");
        private static readonly ConfigKey<string> SecurityInfoKey = new ConfigKey<string>("secinfo");
        private static readonly ConfigKey<DateTime> DateCreatedKey = new ConfigKey<DateTime>("secinfo");
        private static readonly ConfigKey<DateTime> DateModifiedKey = new ConfigKey<DateTime>("secinfo");
        private static readonly ConfigKey<string> CustomInfoKey = new ConfigKey<string>("custom");

        public const string PREAMBLE_OLD = "ThisIsAnActualAccount";
        public const string PREAMBLE = "#ThisIsAnActualAccount";
        public const string FILE_EXTENSION = "txt";
        public const string FILE_EXTENSION_DOT = ".txt";
        public const string FILE_EXTENSION_FILTER = "*.txt";

        public static IEnumerable<AccountModel> ReadAccountsFromDisk_OLD(Action<string, Exception> errorHandler, int firstPosition = 0) {
            foreach (string path in Directory.EnumerateFiles(Configuration.AccountSaveFile, FILE_EXTENSION_FILTER)) {
                AccountModel item = new AccountModel();
                try {
                    using (StreamReader reader = new StreamReader(new BufferedStream(File.OpenRead(path), 256))) {
                        ReadAccountFromReader_OLD(ref item, reader);
                    }
                }
                catch (Exception e) {
                    errorHandler(path, e);
                    item = null;
                }

                if (item != null) {
                    if (item.Position == -1) {
                        item.Position = firstPosition;
                    }

                    firstPosition++;
                    yield return item;
                }
            }
        }

        public static IEnumerable<AccountModel> ReadAccountsFromDisk(Action<string, Exception> errorHandler, int firstPosition = 0) {
            foreach (string path in Directory.EnumerateFiles(Configuration.AccountSaveFile, FILE_EXTENSION_FILTER)) {
                AccountModel item = new AccountModel();
                try {
                    using (StreamReader reader = new StreamReader(new BufferedStream(File.OpenRead(path), 256))) {
                        ReadAccountFromReader_OLD(ref item, reader);
                    }
                }
                catch (Exception e) {
                    errorHandler(path, e);
                    item = null;
                }

                if (item != null) {
                    if (item.Position == -1) {
                        item.Position = firstPosition;
                    }

                    firstPosition++;
                    yield return item;
                }
            }
        }

        public static async Task WriteAccountsToDiskAsync(IEnumerable<AccountModel> accounts, Func<AccountModel, Exception, Task> errorHandler) {
            foreach (AccountModel item in accounts) {
                try {
                    string path = Path.Combine(Configuration.AccountSaveFile, item.AccountName) + ".txt";
                    using (StreamWriter writer = new StreamWriter(new BufferedStream(File.OpenWrite(path), 1024))) {
                        WriteAccountToWriter(item, writer);
                    }
                }
                catch (Exception e) {
                    await errorHandler(item, e);
                }
            }
        }

        public static void WriteAccountToWriter(AccountModel account, TextWriter writer) {
            writer.WriteLine(PREAMBLE_OLD);
            writer.WriteLine(account.Position);
            writer.WriteLine(account.AccountName);
            writer.WriteLine(account.Email);
            writer.WriteLine(account.Username);
            writer.WriteLine(account.Password);
            writer.WriteLine(account.DateOfBirth);
            writer.WriteLine(account.SecurityInfo);
            if (!string.IsNullOrEmpty(account.CustomInfo)) {
                foreach (string line in account.CustomInfo.Split('\n')) {
                    writer.WriteLine(line);
                }
            }
        }

        public static void ReadAccountFromReader_OLD(ref AccountModel account, TextReader reader, bool readPreamble = true) {
            // ReadLine() returns null if there's no more data. It returns an empty
            // string if there is more data, but the line is just empty
            if (readPreamble) {
                string preamble = reader.ReadLine();
                if (preamble == null) {
                    throw new EndOfStreamException("Not enough data to read preamble");
                }
                else if (preamble != PREAMBLE_OLD) {
                    throw new Exception("Preamble was invalid (it was '{preamble}')");
                }
            }

            account.Position = int.TryParse(reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read account position"), out int pos) ? pos : -1;
            account.AccountName = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read account name");
            account.Email = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read email");
            account.Username = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read username");
            account.Password = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read password");
            account.DateOfBirth = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read date of birth");
            account.SecurityInfo = reader.ReadLine() ?? throw new EndOfStreamException("Not enough data to read security info");
            account.CustomInfo = ReadLinesUntilEndOfFile(reader) ?? "";
        }

        public static void ReadAccountFromReader(ref AccountModel account, StreamReader reader, bool readPreamble = true) {
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

            CrapData data = new CrapData();
            data.Read(reader);
            account.Position = int.TryParse(data.Get(AccountPositionKey.Key) ?? throw new EndOfStreamException("Not enough data to read account position"), out int pos) ? pos : -1;
            account.AccountName = data.Get(AccountNameKey.Key) ?? throw new EndOfStreamException("Not enough data to read account name");
            account.Email = data.Get(EmailKey.Key) ?? throw new EndOfStreamException("Not enough data to read email");
            account.Username = data.Get(UsernameKey.Key) ?? throw new EndOfStreamException("Not enough data to read username");
            account.Password = data.Get(PasswordKey.Key) ?? throw new EndOfStreamException("Not enough data to read password");
            account.DateOfBirth = data.Get(DateOfBirthKey.Key) ?? throw new EndOfStreamException("Not enough data to read date of birth");
            account.SecurityInfo = data.Get(SecurityInfoKey.Key) ?? throw new EndOfStreamException("Not enough data to read security info");
            if (!data.TryGet(DateCreatedKey.Key, out string val1) || !long.TryParse(val1, out long creation))
                throw new EndOfStreamException("Not enough data to read creation date");
            account.CreationTime = new DateTime(creation);

            if (!data.TryGet(DateModifiedKey.Key, out string val2) || !long.TryParse(val2, out long modified))
                throw new EndOfStreamException("Not enough data to read last modified date");
            account.CreationTime = new DateTime(modified);

            account.CustomInfo = data.Get(CustomInfoKey.Key);
        }

        public static string ReadLinesUntilEndOfFile(TextReader reader) {
            StringBuilder sb = new StringBuilder(128);
            string line = reader.ReadLine();
            if (line != null) {
                sb.Append(line);
            }

            while ((line = reader.ReadLine()) != null) {
                sb.Append("\n").Append(line);
            }

            return sb.ToString();
        }
    }
}