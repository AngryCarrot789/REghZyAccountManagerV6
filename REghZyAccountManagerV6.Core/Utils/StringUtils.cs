using System;

namespace REghZyAccountManagerV6.Core.Utils {
    public static class StringUtils {
        public static int Count(this string str, char character, int startIndex = 0) {
            int i = startIndex - 1, count = 0;
            while ((i = str.IndexOf(character, i + 1)) != -1)
                count++;
            return count;
        }

        public static int Count(this string str, string text, int startIndex = 0) {
            int len = text.Length, i = startIndex - len, count = 0;
            while ((i = str.IndexOf(text, i + len)) != -1)
                count++;
            return count;
        }

        // It doesn't get much more efficient than this
        public static string Format(string format, object[] args) {
            int i, j, k, num;
            if (string.IsNullOrEmpty(format) || (i = format.IndexOf('{', j = 0)) == -1)
                return format;
            // buffer of 2x format is typically the best result
            FastStringBuf sb = new FastStringBuf(format.Length * 2);
            do {
                if (i == 0 || format[i - 1] != '\\') { // check escape char
                    sb.Append(format, j, i); // append text between j and before '{' char
                    if ((k = format.IndexOf('}', i)) != -1) { // get closing char index
                        j = k + 1; // set last char to after closing char
                        if ((num = parseIntSigned(format, i + 1, k, 10)) >= 0 && num < args.Length) {
                            sb.Append(args[num]); // append content
                        }
                        else { // OLD: sb.append('{').append(format, i + 1, k).append('}');
                            sb.Append(format, i, j); // append values between { and }
                        }
                        i = k; // set next search index to the '}' char
                    }
                    else {
                        j = i; // set last char to the last '{' char
                    }
                }
                else { // remove escape char
                    sb.Append(format, j, i - 1); // append text between j and before the escape char
                    j = i; // set last index to the '{' char, which was originally escaped
                }
            } while ((i = format.IndexOf('{', i + 1)) != -1);
            sb.Append(format, j, format.Length); // append remainder of string
            return sb.ToString();
        }

        public static int parseIntSigned(string input, int startIndex, int endIndex, int radix) {
            if (startIndex < 0 || endIndex <= startIndex) {
                return -1;
            }

            char first = input[startIndex];
            if (first < '0') { // Possible leading "+"
                if (first != '+' || (endIndex - startIndex) == 1)
                    return -1; // Cannot have lone "+"
                startIndex++;
            }

            int result = 0;
            const int limit = -int.MaxValue; // Integer.MIN_VALUE + 1
            int radixMinLimit = limit / radix;
            while (startIndex < endIndex) {
                // Accumulating negatively avoids surprises near MAX_VALUE
                int digit = Digit(input[startIndex++], radix);
                if (digit < 0 || result < radixMinLimit)
                    return -1;
                if ((result *= radix) < limit + digit)
                    return -1;
                result -= digit;
            }
            return -result;
        }

        public static int Digit(char value, int radix) {
            if (radix > 0 && radix <= 36) {
                if (radix <= 10) {
                    if (value >= '0' && value < '0' + radix) {
                        return value - '0';
                    }
                    else {
                        return -1;
                    }
                }
                else if (value >= '0' && value <= '9') {
                    return value - '0';
                }
                else if (value >= 'a' && value < 'a' + radix - 10) {
                    return value - 'a' + 10;
                }
                else if (value >= 'A' && value < 'A' + radix - 10) {
                    return value - 'A' + 10;
                }

                return -1;
            }

            throw new ArgumentOutOfRangeException(nameof(radix), "Invalid radix value: " + radix);
        }

        private sealed class FastStringBuf {
            private char[] data;
            private int count;

            public FastStringBuf(int initialCapacity) {
                this.data = new char[initialCapacity];
            }

            public void Append(string str, int startIndex, int endIndex) {
                int len = endIndex - startIndex;
                if (len > 0) {
                    this.EnsureCapacityForAddition(len);
                    str.CopyTo(startIndex, this.data, this.count, len);
                    this.count += len;
                }
            }

            public void Append(char[] array, int startIndex, int endIndex) {
                int len = endIndex - startIndex;
                if (len > 0) {
                    this.EnsureCapacityForAddition(len);
                    Array.ConstrainedCopy(array, startIndex, this.data, this.count, len);
                    this.count += len;
                }
            }

            public void Append(string str) {
                this.Append(str, 0, str.Length);
            }

            public void Append(object value) {
                this.Append(value == null ? "null" : value.ToString());
            }

            private void EnsureCapacityForAddition(int additional) {
                if (((this.count + additional) - this.data.Length) > 0) {
                    this.Grow(this.count + additional);
                }
            }

            private void Grow(int minCapacity) {
                int oldCapacity = this.data.Length;
                int newCapacity = oldCapacity + (oldCapacity >> 1);
                if (newCapacity - minCapacity < 0)
                    newCapacity = minCapacity;
                Array.Resize(ref this.data, newCapacity);
            }

            public override String ToString() {
                return new String(this.data, 0, this.count);
            }
        }
    }
}