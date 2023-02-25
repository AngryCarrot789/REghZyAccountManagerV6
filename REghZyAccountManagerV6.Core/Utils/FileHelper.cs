using System.Collections.Generic;
using System.IO;
using System.Text;
using REghZyAccountManagerV6.Core.Accounting;

namespace REghZyAccountManagerV6.Core.Utils {
    public static class FileHelper {
        private static readonly char[] INVALID_FILENAME_CHARS_ARR = Path.GetInvalidFileNameChars();
        private static readonly char[] INVALID_PATH_CHARS_ARR = Path.GetInvalidPathChars();
        private static readonly HashSet<char> INVALID_FILENAME_CHARS;

        static FileHelper() {
            INVALID_FILENAME_CHARS = new HashSet<char>(INVALID_FILENAME_CHARS_ARR);
        }

        public static string GetCleanAccountFileName(AccountViewModel account) {
            return GetCleanAccountFileName(account.AccountName);
        }

        public static string GetCleanAccountFileName(string text) {
            if (IsFileNameValid(text)) {
                return text;
            }

            StringBuilder sb = new StringBuilder(text.Length);
            foreach (char ch in text) {
                sb.Append(INVALID_FILENAME_CHARS.Contains(ch) ? '_' : ch);
            }

            return Path.GetFileName(sb.ToString());
        }

        public static bool IsFileNameValid(string fileName) {
            return fileName.IndexOfAny(INVALID_FILENAME_CHARS_ARR) == -1;
        }

        public static bool IsPathValid(string path) {
            return path.IndexOfAny(INVALID_PATH_CHARS_ARR) == -1;
        }

        /// <summary>
        /// Returns the full path of the given path's parent directory
        /// </summary>
        public static string GetParent(string path) {
            return Path.GetDirectoryName(path);
            // int index = path.LastIndexOf(Path.DirectorySeparatorChar);
            // if (index == -1) {
            //     return path;
            // }
            // return path.Substring(0, index);
        }
    }
}