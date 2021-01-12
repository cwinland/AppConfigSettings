using System.Collections.Generic;
using System.IO;
using AppConfigSettings;
using AppConfigSettings.Enum;

namespace TestSettingsConsole
{
    public class Settings : SettingsBase<Settings>
    {
        public static readonly ConfigSetting<string> DefaultRunbookFolder =
            new ConfigSetting<string>("DefaultRunbookFolder",
                                      Directory.GetCurrentDirectory(),
                                      SettingScopes.Any,
                                      Directory.Exists);

        public static readonly ConfigSetting<int> MaxRetries =
            new ConfigSetting<int>("MaxRetries", 2, SettingScopes.Any, i => i > 0);

        public static readonly ConfigSetting<LoggingLevels> AppLoggingLevel =
            new ConfigSetting<LoggingLevels>("AppLoggingLevel", LoggingLevels.None);

        public static readonly ConfigSetting<LoggingLevels> LogLevel =
            new ConfigSetting<LoggingLevels>("Logging:LogLevel:Default",
                                             LoggingLevels.Information,
                                             SettingScopes.Any,
                                             AppLoggingLevel);

        public static readonly ConfigSetting<string> AllowedHosts = new ConfigSetting<string>("AllowedHosts", "None");

        public static readonly ConfigSetting<string> TestSetting = new ConfigSetting<string>("TestSetting", "None");

        public static readonly ConfigSetting<string> SystemRoot =
            new ConfigSetting<string>("SystemRoot",
                                      "None",
                                      SettingScopes.AppSettings,
                                      null,
                                      false,
                                      null,
                                      new List<string>(),
                                      false);

        public static readonly ConfigSetting<string> SystemRoot2 =
            new ConfigSetting<string>("SystemRoot", "None", SettingScopes.AppSettings | SettingScopes.Environment);
    }
}
