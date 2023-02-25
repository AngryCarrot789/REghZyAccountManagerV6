using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using REghZyAccountManagerV6.Core.Accounting;
using REghZyAccountManagerV6.Core.Accounting.Storage;
using REghZyAccountManagerV6.Core.Config;

namespace REghZyAccountManagerV6.Accounting {
    public class CrappyOriginalDatabase : IAccountDatabase {
        public IEnumerable<AccountModel> ReadAccounts() {
            int i = 0;
            foreach (string path in Directory.EnumerateFiles("OLD_PATH_HERE")) {
                AccountModel item = new AccountModel();
                try {
                    using (StreamReader reader = new StreamReader(new BufferedStream(File.OpenRead(path), 256))) {
                        ReadAccountFromReader_OLD(ref item, reader);
                    }
                }
                catch (Exception e) {
                    item = null;
                }

                if (item != null) {
                    if (item.Position == -1) {
                        item.Position = i;
                    }

                    i++;
                    yield return item;
                }
            }
        }

        public void WriteAccounts(IEnumerable<AccountModel> models) {
            foreach (AccountModel item in models) {
                try {
                    string path = Path.Combine(Configuration.AccountSaveFile, item.AccountName) + ".txt";
                    using (StreamWriter writer = new StreamWriter(new BufferedStream(File.OpenWrite(path), 1024))) {
                        WriteAccountToWriter(item, writer);
                    }
                }
                catch (Exception e) {

                }
            }
        }

        public bool DeleteUser(string accountName) {
            return true;
        }

        public static void WriteAccountToWriter(AccountModel account, TextWriter writer) {
            writer.WriteLine("ThisIsAnActualAccount");
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
                else if (preamble != "ThisIsAnActualAccount") {
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