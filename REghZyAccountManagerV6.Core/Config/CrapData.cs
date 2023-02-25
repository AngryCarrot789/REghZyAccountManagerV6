using System.Collections.Generic;
using System.IO;

namespace REghZyAccountManagerV6.Core.Config {
    /// <summary>
    /// A key/value based config library that writes all entries to a file, splitting each entry with a unique character
    /// </summary>
    public class CrapData {
        public char Splitter { get; set; }

        private class Pair {
            public readonly string key;
            public string value;

            public Pair() {

            }

            public Pair(string key, string value) {
                this.key = key;
                this.value = value;
            }
        }

        private readonly List<object> lines;
        private readonly Dictionary<string, int> map;

        public CrapData() {
            this.lines = new List<object>();
            this.Splitter = ':';
            this.map = new Dictionary<string, int>();
        }

        public string this[string key] {
            get => this.Get(key);
            set => this.Set(key, value);
        }

        public string Get(string key) {
            return this.map.TryGetValue(key, out int index) ? ((Pair) this.lines[index]).value : null;
        }

        public List<string> GetListr(string key) {
            return this.map.TryGetValue(key, out int index) ? (List<string>) this.lines[index] : null;
        }

        public bool TryGet(string key, out string value) {
            if (this.map.TryGetValue(key, out int index)) {
                value = ((Pair) this.lines[index]).value;
                return true;
            }

            value = null;
            return false;
        }

        public void Set(string key, string value) {
            this.GetOrCreatePair(key).value = value;
        }

        private Pair GetOrCreatePair(string key) {
            if (this.map.TryGetValue(key, out int index)) {
                return (Pair) this.lines[index];
            }
            else {
                Pair pair = new Pair(key, null);
                this.map[key] = this.lines.Count;
                this.lines.Add(pair);
                return pair;
            }
        }

        public void Read(string file) {
            using (StreamReader reader = new StreamReader(File.OpenRead(file))) {
                this.Read(reader);
            }
        }

        public void Read(StreamReader reader) {
            this.lines.Clear();
            this.map.Clear();
            string line;
            while ((line = reader.ReadLine()) != null) {
                if (string.IsNullOrEmpty(line)) {
                    continue;
                }

                string realLine = line.TrimStart();
                if (realLine.StartsWith("#")) {
                    this.lines.Add(line);
                    continue;
                }

                int split = realLine.IndexOf(this.Splitter);
                if (split == -1) {
                    this.lines.Add(line);
                    continue;
                }

                string key = realLine.Substring(0, split);
                string val = realLine.Substring(split + 1);
                this.map[key] = this.lines.Count;
                this.lines.Add(new Pair(key, val));
            }
        }

        public void Save(string file) {
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(file))) {
                this.Save(writer);
            }
        }

        public void Save(StreamWriter writer) {
            foreach (object line in this.lines) {
                if (line is Pair pair) {
                    writer.WriteLine((pair.key ?? "") + this.Splitter + (pair.value ?? ""));
                }
                else {
                    writer.WriteLine(line ?? "");
                }
            }
        }
    }
}