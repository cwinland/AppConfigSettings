using System.Collections.Generic;
using AppConfigSettings;

namespace TestSettingsConsole
{
    public class Settings : SettingsBase<Settings>
    {
        public static readonly ConfigSetting<string> DefaultRunbookFolder =
            new ConfigSetting<string>("DefaultRunbookFolder", "");

        public static readonly ConfigSetting<int> MaxRetries = new ConfigSetting<int>("MaxRetries", 0);

        public static readonly ConfigSetting<string> AppLoggingLevel =
            new ConfigSetting<string>("AppLoggingLevel", "None");

        public static readonly ConfigSetting<string> LogLevel =
            new ConfigSetting<string>("Logging:LogLevel:Default", "Information", AppLoggingLevel);

        public static readonly ConfigSetting<string> AllowedHosts = new ConfigSetting<string>("AllowedHosts", "None");

        public static readonly ConfigSetting<string> TestSetting = new ConfigSetting<string>("TestSetting", "None");

        public static readonly ConfigSetting<string> SystemRoot =
            new ConfigSetting<string>("SystemRoot", "None", null, false, null, new List<string>(), false);

        public static readonly ConfigSetting<string> SystemRoot2 =
            new ConfigSetting<string>("SystemRoot", "None");
    }
}
