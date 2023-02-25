using System;

namespace REghZyAccountManagerV6.Core.Config {
    public abstract class ConfigKey {
        public string Key { get; }

        protected ConfigKey(string key) {
            this.Key = key;
        }
    }

    public class ConfigKey<T> : ConfigKey {
        public Type Type => typeof(T);

        public T Default { get; }

        public ConfigKey(string key, T def = default) : base(key) {
            this.Default = def;
        }
    }
}