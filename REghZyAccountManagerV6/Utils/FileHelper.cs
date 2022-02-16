using System.Collections.Generic;
using System.IO;
using System.Text;
using REghZyAccountManagerV6.Accounting;

namespace REghZyAccountManagerV6.Utils {
    public class FileHelper {
        private static char[] INVALID_FILENAME_CHARS_ARR = Path.GetInvalidFileNameChars();
        private static readonly HashSet<char> INVALID_FILENAME_CHARS;

        static FileHelper() {
            INVALID_FILENAME_CHARS = new HashSet<char>(INVALID_FILENAME_CHARS_ARR);
        }

        public static string GetValidAccountFileName(AccountViewModel account) {
            return GetValidAccountFileName(account.AccountName);
        }

        public static string GetValidAccountFileName(string accountName) {
            if (IsFileNameValid(accountName)) {
                return accountName;
            }

            HashSet<char> invalidChars = INVALID_FILENAME_CHARS;
            StringBuilder sb = new StringBuilder(accountName.Length);
            foreach (char c in accountName) {
                sb.Append(invalidChars.Contains(c) ? '_' : c);
            }

            return Path.GetFileName(sb.ToString());
        }

        public static bool IsFileNameValid(string fileName) {
            if (fileName.IndexOfAny(INVALID_FILENAME_CHARS_ARR) == -1) {
                return true;
            }

            return false;
        }
    }
}