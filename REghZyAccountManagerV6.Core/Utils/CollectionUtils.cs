using System.Collections.Generic;

namespace REghZyAccountManagerV6.Core.Utils {
    public static class CollectionUtils {
        public static void AddAll<T>(this ICollection<T> destination, IEnumerable<T> source) {
            foreach (T value in source) {
                destination.Add(value);
            }
        }

        public static void AddAll<K, V>(this IDictionary<K, V> destination, IDictionary<K, V> source) {
            foreach (KeyValuePair<K, V> value in source) {
                destination.Add(value);
            }
        }
    }
}