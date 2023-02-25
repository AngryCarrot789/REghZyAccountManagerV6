using REghZyAccountManagerV6.Core.Config;

namespace REghZyAccountManagerV6.Core {
    public interface IApplication {
        string ConfigPath { get; }

        Configuration Configuration { get; set; }
    }
}