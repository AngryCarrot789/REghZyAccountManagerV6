using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private static string ToString(object obj, string def) {
            return obj != null ? obj.ToString() : "";
        }

        public static AccountModel ReadAccount(string filePath) {
            using (JsonReader reader = new JsonTextReader(new StreamReader(File.OpenRead(filePath)))) {
                Dictionary<string, object> dictionary;
                try {
                    dictionary = serializer.Deserialize<Dictionary<string, object>>(reader);
                }
                catch (Exception e) {
                    throw new StorageReadException("Failed to deserialise json to a dictionary", e);
                }

                if (dictionary != null && dictionary.TryGetValue("IsRealAccount", out object isRealAccount) && "Yes!".Equals(isRealAccount)) {
                    AccountModel model = new AccountModel();
                    try {
                        model.Position = int.Parse(ToString(dictionary["Position"], ""));
                        model.AccountName = ToString(dictionary["AccountName"], "");
                        model.Email = ToString(dictionary["Email"], "");
                        model.Username = ToString(dictionary["Username"], "");
                        model.Password = ToString(dictionary["Password"], "");
                        model.DateOfBirth = ToString(dictionary["DateOfBirth"], "");
                        model.SecurityInfo = ToString(dictionary["SecurityInfo"], "");
                        model.CreationTime = new DateTime(long.Parse(ToString(dictionary["CreationTime"], "")));
                        model.LastModifiedTime = new DateTime(long.Parse(ToString(dictionary["LastModifiedTime"], "")));
                        model.CustomInfo = string.Join("\n", ((JArray) dictionary["CustomInfo"]).ToObject<List<string>>() ?? throw new Exception());
                    }
                    catch (Exception e) {
                        throw new StorageReadException("Failed to parse deserialised json into AccountModel", e);
                    }

                    return model;
                }
                else {
                    throw new StorageReadException("Invalid json account file");
                }
            }
        }

        public IEnumerable<AccountModel> ReadAccounts() {
            if (string.IsNullOrEmpty(AccountDirectory) || !Directory.Exists(AccountDirectory)) {
                throw new StorageUnavailableException("Account directory does not exist: " + AccountDirectory);
            }

            List<AccountModel> accounts = new List<AccountModel>();
            foreach (string file in Directory.EnumerateFiles(AccountDirectory)) {
                try {
                    accounts.Add(ReadAccount(file));
                }
                catch { }
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
                        dictionary["AccountName"] = account.AccountName ?? "";
                        dictionary["Email"] = account.Email ?? "";
                        dictionary["Username"] = account.Username ?? "";
                        dictionary["Password"] = account.Password ?? "";
                        dictionary["DateOfBirth"] = account.DateOfBirth ?? "";
                        dictionary["SecurityInfo"] = account.SecurityInfo ?? "";
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
            for (int i = 0; i < count; i++)
                array[i] = ch;
            return new string(array);
        }
    }
}