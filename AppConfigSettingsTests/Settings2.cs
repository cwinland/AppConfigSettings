using System.IO;
using AppConfigSettings;
using AppConfigSettings.Enum;

namespace AppConfigSettingsTests
{
    public class Settings2 : SettingsBase<Settings2>
    {
        public static readonly ConfigSetting<string> TestPath =
            new ConfigSetting<string>("TestPath", @"C:\windows", SettingScopes.Any, Directory.Exists);

        public static readonly ConfigSetting<int> Retries =
            new ConfigSetting<int>("Retries", 55, SettingScopes.Any, i => i > 1);

        public static readonly ConfigSetting<int> Sections = new ConfigSetting<int>("Sections", 3);

        public static readonly ConfigSetting<string> Public =
            new ConfigSetting<string>("Public", string.Empty, SettingScopes.Environment);
    }
}
