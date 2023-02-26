using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using REghZyAccountManagerV6.Core.Accounting;

namespace REghZyAccountManagerV6.Core.Utils {
    public static class FileHelper {
        private static readonly char[] INVALID_FILENAME_CHARS_ARR = Path.GetInvalidFileNameChars();
        private static readonly char[] INVALID_PATH_CHARS_ARR = Path.GetInvalidPathChars();
        private static readonly char[] INVALID_CHARS_SPECIAL;
        private static readonly HashSet<char> INVALID_FILENAME_CHARS;

        static FileHelper() {
            INVALID_FILENAME_CHARS = new HashSet<char>(INVALID_FILENAME_CHARS_ARR);
            INVALID_CHARS_SPECIAL = CombineArrays(INVALID_FILENAME_CHARS_ARR, INVALID_PATH_CHARS_ARR);
        }

        private static char[] CombineArrays(params char[][] arrays) {
            int length = arrays.Length, elements = 0;
            for (int index = 0; index < length; index++) {
                elements += arrays[index].Length;
            }

            int i = 0;
            char[] dest = new char[elements];
            for (int index = 0; index < length; index++) {
                char[] src = arrays[index];
                Array.ConstrainedCopy(src, 0, dest, i, src.Length);
                i += src.Length;
            }

            return dest;
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

        public static bool IsPathInvalid(string path, out List<Tuple<int, char>> illegal) {
            illegal = new List<Tuple<int, char>>();
            for (int j = -1; (j = path.IndexOfAny(INVALID_FILENAME_CHARS_ARR, j + 1)) != -1;) {
                // cba to implement root detection
                if (path[j] != '\\' && path[j] != '/' && path[j] != ':') {
                    illegal.Add(new Tuple<int, char>(j, path[j]));
                }
            }

            return illegal.Count > 0;
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