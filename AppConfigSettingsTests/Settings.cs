using System;
using System.IO;
using AppConfigSettings;
using AppConfigSettings.Enum;

namespace AppConfigSettingsTests
{
    public class Settings : SettingsBase<Settings>
    {
        public static readonly ConfigSetting<string> Path =
            new ConfigSetting<string>("DefaultPath",
                                      @"C:\",
                                      SettingScopes.Any,
                                      Directory.Exists,
                                      false,
                                      Settings2.TestPath);

        public static readonly ConfigSetting<int> Retries =
            new ConfigSetting<int>("Retries", 2, SettingScopes.Any, i => i > 0, false, Settings2.Retries);

        public static readonly ConfigSetting<StatusEnum> DefaultStatus =
            new ConfigSetting<StatusEnum>("DefaultStatus",
                                          StatusEnum.Open,
                                          SettingScopes.Any,
                                          status => status != StatusEnum.Closed);

        public static readonly ConfigSetting<string> Author =
            new ConfigSetting<string>("Author", "Chris", SettingScopes.Json | SettingScopes.AppSettings);

        public static readonly ConfigSetting<DateTime> Created =
            new ConfigSetting<DateTime>("Created",
                                        DateTime.Today,
                                        SettingScopes.Any,
                                        t => t > DateTime.Parse("1/1/1998"));

        public static readonly ConfigSetting<string> HomePath =
            new ConfigSetting<string>("HomePath", string.Empty, SettingScopes.Environment, Path);

        public static readonly ConfigSetting<string> LogLevelDefault =
            new ConfigSetting<string>("Logging:LogLevel:Default", "DefaultDefault", SettingScopes.Json);

        public static readonly ConfigSetting<string> LogLevelLife =
            new ConfigSetting<string>("Logging:LogLevel:Microsoft.Hosting.Lifetime", "LifeDefault", SettingScopes.Json);
    }
}
