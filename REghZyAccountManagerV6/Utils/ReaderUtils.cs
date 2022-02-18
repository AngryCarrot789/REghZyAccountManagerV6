using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static string ReadBlock(this TextReader reader, int count, int offsetIndex = 0) {
            char[] chars = new char[count];
            int read = reader.ReadBlock(chars, offsetIndex, count);
            if (count != read) {
                return new string(chars, 0, read);
            }
            else {
                return new string(chars);
            }
        }
    }
}
