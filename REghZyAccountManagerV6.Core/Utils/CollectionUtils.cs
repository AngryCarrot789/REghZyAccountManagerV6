using System.Collections.Generic;

namespace REghZyAccountManagerV6.Core.Utils {
    public static class CollectionUtils {
        public static void AddAll<T>(this ICollection<T> destination, IEnumerable<T> source) {
            foreach (T value in source) {
                destination.Add(value);
            }
        }
    }
}