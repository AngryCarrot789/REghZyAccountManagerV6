using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace REghZyAccountManagerV6.Core.Config {
    /// <summary>
    /// A configuration contains Key/Value entries
    /// </summary>
    public class Configuration {
        private static readonly List<ConfigKey> ConfigKeys = new List<ConfigKey>();

        public static readonly ConfigKey<string> AccountFilePathKey = CreateKey<string>("AccountsFilePath");
        public static readonly ConfigKey<int> MainWindowWidthKey = CreateKey("WindowWidth", 1280);
        public static readonly ConfigKey<int> MainWindowHeightKey = CreateKey("WindowHeight", 760);
        public static readonly ConfigKey<bool> DoubleClickSelectAllTextKey = CreateKey("DoubleClickSelectAllText", false);

        private readonly Dictionary<ConfigKey, object> map;

        public Configuration() : this(new Dictionary<ConfigKey, object>()) {

        }

        private Configuration(Dictionary<ConfigKey, object> dictionary) {
            this.map = dictionary;
        }

        public void LoadDefaults() {
            this.Set(AccountFilePathKey, "");
            this.Set(MainWindowWidthKey, 1280);
            this.Set(MainWindowHeightKey, 760);
            this.Set(DoubleClickSelectAllTextKey, false);
        }

        public T Get<T>(ConfigKey<T> key) {
            return this.map.TryGetValue(key, out object value) ? (T) value : key.Default;
        }

        public bool TryGet<T>(ConfigKey<T> key, out T value) {
            if (this.map.TryGetValue(key, out object val)) {
                value = (T) val;
                return true;
            }

            value = default;
            return false;
        }

        public void Set<T>(ConfigKey<T> key, T value) {
            this.map[key] = value;
        }

        public bool Contains(ConfigKey key) {
            return this.map.ContainsKey(key);
        }

        private static ConfigKey<T> CreateKey<T>(string key, T def = default) {
            ConfigKey<T> entry = new ConfigKey<T>(key, def);
            ConfigKeys.Add(entry);
            return entry;
        }

        public Configuration Clone() {
            return new Configuration(new Dictionary<ConfigKey, object>(this.map));
        }

        public void LoadFrom(CrapData data) {
            this.Set(AccountFilePathKey, data[AccountFilePathKey.Key]);
            if (int.TryParse(data[MainWindowWidthKey.Key], out int width))
                this.Set(MainWindowWidthKey, width);
            if (int.TryParse(data[MainWindowHeightKey.Key], out int height))
                this.Set(MainWindowHeightKey, height);
            if (bool.TryParse(data[DoubleClickSelectAllTextKey.Key], out bool state))
                this.Set(DoubleClickSelectAllTextKey, state);
        }

        public void SaveTo(CrapData data) {
            this.TrySet(data, AccountFilePathKey);
            this.TrySet(data, MainWindowWidthKey);
            this.TrySet(data, MainWindowHeightKey);
            this.TrySet(data, DoubleClickSelectAllTextKey);
        }

        public void Load(string file) {
            CrapData data = new CrapData();
            data.Read(file);
            this.LoadFrom(data);
        }

        public void Save(string file) {
            CrapData data = new CrapData();
            this.SaveTo(data);
            data.Save(file);
        }

        private void TrySet(CrapData data, ConfigKey key) {
            if (this.map.TryGetValue(key, out object value) && value != null) {
                data[key.Key] = value.ToString();
            }
        }

        public static string AccountSaveFile {
            get => IoC.Application.Configuration.Get(AccountFilePathKey);
            set => IoC.Application.Configuration.Set(AccountFilePathKey, value);
        }
    }
}