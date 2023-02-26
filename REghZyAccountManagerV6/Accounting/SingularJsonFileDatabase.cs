using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REghZyAccountManagerV6.Core.Accounting;
using REghZyAccountManagerV6.Core.Accounting.Storage;
using REghZyAccountManagerV6.Core.Config;

namespace REghZyAccountManagerV6.Accounting {
    public class SingularJsonFileDatabase : IAccountDatabase {
        public static readonly JsonSerializer serializer = new JsonSerializer();

        public string FilePath => Configuration.AccountSaveFile;

        static SingularJsonFileDatabase() {
            serializer.Formatting = Formatting.Indented;
            serializer.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
        }

        public IEnumerable<AccountModel> ReadAccounts() {
            string path = this.FilePath;
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) {
                throw new StorageUnavailableException("Account directory does not exist: " + path);
            }

            List<AccountModel> accounts = new List<AccountModel>();
            using (JsonReader reader = new JsonTextReader(new StreamReader(File.OpenRead(path)))) {
                Dictionary<string, object> root;
                try {
                    root = serializer.Deserialize<Dictionary<string, object>>(reader) ?? throw new Exception("Json File could not be deserialized to a dictionary");
                }
                catch (Exception e) {
                    throw new StorageReadException("Failed to deserialize json", e);
                }

                foreach (KeyValuePair<string, object> pair in root) {
                    Dictionary<string, object> userData = ((JObject) pair.Value).ToObject<Dictionary<string, object>>();
                    if (userData != null) {
                        AccountModel model = new AccountModel();
                        model.Position = int.Parse(ToString(userData["Position"], ""));
                        model.AccountName = ToString(userData["AccountName"], "");
                        model.Email = ToString(userData["Email"], "");
                        model.Username = ToString(userData["Username"], "");
                        model.Password = ToString(userData["Password"], "");
                        model.DateOfBirth = ToString(userData["DateOfBirth"], "");
                        model.SecurityInfo = ToString(userData["SecurityInfo"], "");
                        model.CreationTime = new DateTime(long.Parse(ToString(userData["CreationTime"], "").ToString()));
                        model.LastModifiedTime = new DateTime(long.Parse(ToString(userData["LastModifiedTime"], "").ToString()));
                        model.CustomInfo = string.Join("\n", ((JArray) userData["CustomInfo"]).ToObject<List<string>>() ?? throw new Exception());

                        accounts.Add(model);
                    }
                }
            }

            return accounts;
        }

        public static string ToString(object obj, string def) {
            return obj != null ? obj.ToString() : def;
        }

        public void WriteAccounts(IEnumerable<AccountModel> models) {
            string path = this.FilePath;
            if (string.IsNullOrEmpty(path)) {
                throw new StorageUnavailableException("Account directory path is not set: " + path);
            }

            if (File.Exists(path)) {
                if (File.Exists(path + "_backup")) {
                    File.Delete(path + "_backup");
                    File.Move(path, path + "_backup");
                }
                else {
                    File.Move(path, path + "_backup");
                }
            }

            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(File.OpenWrite(path)))) {
                Dictionary<string, object> root = new Dictionary<string, object>();
                int i = 0;
                foreach (AccountModel account in models) {
                    Dictionary<string, object> userData = new Dictionary<string, object>();
                    try {
                        userData["Position"] = account.Position;
                        userData["AccountName"] = account.AccountName ?? "";
                        userData["Email"] = account.Email ?? "";
                        userData["Username"] = account.Username ?? "";
                        userData["Password"] = account.Password ?? "";
                        userData["DateOfBirth"] = account.DateOfBirth ?? "";
                        userData["SecurityInfo"] = account.SecurityInfo ?? "";
                        userData["CreationTime"] = account.CreationTime.Ticks;
                        userData["LastModifiedTime"] = account.LastModifiedTime.Ticks;
                        userData["CustomInfo"] = string.IsNullOrEmpty(account.CustomInfo) ? new List<string>() : account.CustomInfo.Split('\n').ToList();
                    }
                    catch {
                        continue;
                    }

                    root[(i++).ToString()] = userData;
                }

                try {
                    serializer.Serialize(writer, root, typeof(Dictionary<string, object>));
                }
                catch {
                    // ignored
                }
            }
        }

        public bool DeleteUser(string accountName) {
            return true;
        }
    }
}