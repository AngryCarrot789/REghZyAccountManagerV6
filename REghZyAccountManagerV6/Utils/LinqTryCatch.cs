using System;
using System.Collections.Generic;

namespace REghZyAccountManagerV6.Utils {
    public static class LinqTryCatch {
        /// <summary>
        /// A delegate for the TryCatch linq thing
        /// </summary>
        /// <typeparam name="T">The type of element that was enumerated</typeparam>
        public delegate void ExceptionHandler(Exception exception);

        public static IEnumerable<T> TryCatch<T>(this IEnumerable<T> source, ExceptionHandler exceptionHandler) {
            using (IEnumerator<T> enumerator = source.GetEnumerator()) {
                bool hasNext = true;
                while (hasNext) {
                    try {
                        hasNext = enumerator.MoveNext();
                    }
                    catch (Exception ex) {
                        exceptionHandler?.Invoke(ex);
                        continue;
                    }

                    if (hasNext) {
                        yield return enumerator.Current;
                    }
                }
            }
        }
    }
}