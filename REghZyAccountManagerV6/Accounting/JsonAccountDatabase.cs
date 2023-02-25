using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REghZy.Utils;
using REghZyAccountManagerV6.Core;
using REghZyAccountManagerV6.Core.Accounting;
using REghZyAccountManagerV6.Core.Accounting.Storage;
using REghZyAccountManagerV6.Core.Config;
using REghZyAccountManagerV6.Core.Utils;

namespace REghZyAccountManagerV6.Accounting {
    public class JsonAccountDatabase : IAccountDatabase {
        public static readonly JsonSerializer serializer = new JsonSerializer();

        public static string AccountDirectory => IoC.Application.Configuration.Get(Configuration.AccountFilePathKey);

        static JsonAccountDatabase() {
            serializer.Formatting = Formatting.Indented;
        }

        public IEnumerable<AccountModel> ReadAccounts() {
            if (string.IsNullOrEmpty(AccountDirectory) || !Directory.Exists(AccountDirectory)) {
                throw new StorageUnavailableException("Account directory does not exist: " + AccountDirectory);
            }

            List<AccountModel> accounts = new List<AccountModel>();
            foreach (string file in Directory.EnumerateFiles(AccountDirectory)) {
                using (JsonReader reader = new JsonTextReader(new StreamReader(File.OpenRead(file)))) {
                    Dictionary<string, object> dictionary;
                    try {
                        dictionary = serializer.Deserialize<Dictionary<string, object>>(reader);
                    }
                    catch {
                        continue;
                    }

                    if (dictionary != null && dictionary.TryGetValue("IsRealAccount", out object isRealAccount) && "Yes!".Equals(isRealAccount)) {
                        AccountModel model = new AccountModel();
                        try {
                            model.Position = int.Parse(dictionary["Position"].ToString());
                            model.AccountName = dictionary["AccountName"].ToString();
                            model.Email = dictionary["Email"].ToString();
                            model.Username = dictionary["Username"].ToString();
                            model.Password = dictionary["Password"].ToString();
                            model.DateOfBirth = dictionary["DateOfBirth"].ToString();
                            model.SecurityInfo = dictionary["SecurityInfo"].ToString();
                            model.CreationTime = new DateTime(long.Parse(dictionary["CreationTime"].ToString()));
                            model.LastModifiedTime = new DateTime(long.Parse(dictionary["LastModifiedTime"].ToString()));
                            model.CustomInfo = string.Join("\n", ((JArray) dictionary["CustomInfo"]).ToObject<List<string>>() ?? throw new Exception());
                        }
                        catch {
                            continue;
                        }

                        accounts.Add(model);
                    }
                }
            }

            return accounts;
        }

        public void WriteAccounts(IEnumerable<AccountModel> models) {
            if (string.IsNullOrEmpty(AccountDirectory)) {
                throw new StorageUnavailableException("Account directory path is not set: " + AccountDirectory);
            }

            if (!Directory.Exists(AccountDirectory)) {
                try {
                    Directory.CreateDirectory(AccountDirectory);
                }
                catch (Exception e) {
                    throw new StorageUnavailableException("Failed to create account directory at: " + AccountDirectory, e);
                }
            }

            HashSet<string> usedPaths = new HashSet<string>();
            foreach (AccountModel account in models) {
                // string path = GetUniqueAccountPath(AccountDirectory, account.AccountName, account.FilePath, usedPaths);
                string path = GetUniqueAccountPath(account.AccountName, AccountDirectory, usedPaths);
                using (JsonWriter writer = new JsonTextWriter(new StreamWriter(File.OpenWrite(path)))) {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    try {
                        dictionary["IsRealAccount"] = "Yes!";
                        dictionary["Position"] = account.Position;
                        dictionary["AccountName"] = account.AccountName;
                        dictionary["Email"] = account.Email;
                        dictionary["Username"] = account.Username;
                        dictionary["Password"] = account.Password;
                        dictionary["DateOfBirth"] = account.DateOfBirth;
                        dictionary["SecurityInfo"] = account.SecurityInfo;
                        dictionary["CreationTime"] = account.CreationTime.Ticks;
                        dictionary["LastModifiedTime"] = account.LastModifiedTime.Ticks;
                        dictionary["CustomInfo"] = string.IsNullOrEmpty(account.CustomInfo) ? new List<string>() : account.CustomInfo.Split('\n').ToList();
                    }
                    catch {
                        continue;
                    }

                    try {
                        serializer.Serialize(writer, dictionary, typeof(Dictionary<string, object>));
                    }
                    catch {
                        // ignored
                    }
                }
            }
        }

        public bool DeleteUser(string accountName) {
            string path = Path.Combine(AccountDirectory, accountName) + ".json";
            if (File.Exists(path)) {
                File.Delete(path);
                return true;
            }

            return false;
        }

        // public static string GetUniqueAccountPath(string dir, string name, string defPath, ISet<string> visitedPaths) {
        //     string path = defPath;
        //     if (string.IsNullOrEmpty(defPath) || !File.Exists(defPath)) {
        //         path = Path.Combine(dir, FileHelper.GetValidAccountFileName(name));
        //     }
        //     if (visitedPaths != null) {
        //         path = Path.ChangeExtension(path, null);
        //         if (visitedPaths.Contains(path)) {
        //             while (true) {
        //                 path += '_';
        //                 if (!File.Exists(path + ".json")) {
        //                     break;
        //                 }
        //             }
        //         }
        //         visitedPaths.Add(path);
        //     }
        //     return Path.ChangeExtension(path, "json");
        // }

        public static string GetUniqueAccountPath(string name, string directory, ISet<string> usedPaths) {
            name = FileHelper.GetCleanAccountFileName(name);
            string path = Path.Combine(directory, name) + ".json";
            if (!usedPaths.Contains(path)) {
                // File may already exists, so overwrite
                usedPaths.Add(path);
                return path;
            }

            int count = 1;
            do {
                path = Path.Combine(directory, name + Repeat('_', count++)) + ".json";
            } while (usedPaths.Contains(path) && path.Length < 260);
            if (path.Length < 260) {
                usedPaths.Add(path);
                return path;
            }
            else {
                throw new StorageWriteException($"File path for {name} exceeded Max Path Lenght (260): {path}");
            }
        }

        public static string Repeat(char ch, int count) {
            char[] array = new char[count];
            Arrays.Fill(array, ch);
            return new string(array);
        }
    }
}