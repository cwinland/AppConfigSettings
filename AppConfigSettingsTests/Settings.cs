using System;
using System.IO;
using AppConfigSettings;

namespace AppConfigSettingsTests
{
    public class Settings : SettingsBase<Settings>
    {
        public static readonly ConfigSetting<string> Path =
            new ConfigSetting<string>("DefaultPath", @"C:\", Directory.Exists, false, Settings2.TestPath);

        public static readonly ConfigSetting<int> Retries =
            new ConfigSetting<int>("Retries", 2, i => i > 0, false, Settings2.Retries);

        public static readonly ConfigSetting<StatusEnum> DefaultStatus =
            new ConfigSetting<StatusEnum>("DefaultStatus", StatusEnum.Open, status => status != StatusEnum.Closed);

        public static readonly ConfigSetting<string> Author = new ConfigSetting<string>("Author", "Chris");

        public static readonly ConfigSetting<DateTime> Created =
            new ConfigSetting<DateTime>("Created", DateTime.Today, t => t > DateTime.Parse("1/1/1998"));
    }
}
