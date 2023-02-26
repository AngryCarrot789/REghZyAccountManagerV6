namespace REghZyAccountManagerV6.Core.Utils {
    public static class ObjUtils {
        public static string ToString(object value, string def = "null") {
            return value != null ? value.ToString() : def;
        }
    }
}