using System;
using System.IO;

namespace REghZyAccountManagerV6.Utils {
    public static class ReaderUtils {
        /// <summary>
        /// Reads the given number of characters
        /// </summary>
        /// <param name="reader">The reader to read from</param>
        /// <param name="count">The number of characters to read</param>
        /// <returns>
        /// A string containing the read characters. If the reader could not read the entire string, 
        /// whatever was read will be returned; simply check the compare string's length. 
        /// Null will be returned if no characters could be read (end of stream)
        /// </returns>
        public static string ReadBlock(this TextReader reader, int count) {
            char[] chars = new char[count];
            int read = reader.ReadBlock(chars, 0, count);
            if (count == read) {
                return new string(chars);
            }
            else {
                // just in case read somehow reads more than count... which it really shouldn't because that would
                // overflow the char buffer and throw an exception... but just in case it somehow all works out...
                return new string(chars, 0, Math.Min(read, count));
            }
        }
    }
}
