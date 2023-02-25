using System.Windows;
using REghZyAccountManagerV6.Core.Services;

namespace REghZyAccountManagerV6.Services {
    public class ClipboardService : IClipboardService {
        public string ReadableText {
            get => Clipboard.GetText(TextDataFormat.UnicodeText);
            set => Clipboard.SetText(value, TextDataFormat.UnicodeText);
        }
    }
}