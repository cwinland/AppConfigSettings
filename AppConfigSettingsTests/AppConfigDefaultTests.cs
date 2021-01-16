using System;
using System.Collections.Specialized;
using System.Configuration;
using AppConfigSettings;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppConfigSettingsTests
{
    [TestClass]
    public class AppConfigDefaultTests : TestBase
    {
        [TestInitialize]
        public void Settings_InitTest() => Settings.SetAppSettings(new NameValueCollection());

        [TestCleanup]
        public void Settings_CleanTest() => Settings.SetAppSettings(ConfigurationManager.AppSettings);

        [TestMethod]
        public void AppConfig_Defaults_Static()
        {
            Settings.Author.Get().Should().Be("Chris");
            Settings.Created.Get().Should().Be(DateTime.Today);
            Settings.DefaultStatus.Get().Should().Be(StatusEnum.Open);
            Settings.Path.Get().Should().Be(@"C:\");
            Settings.Retries.Get().Should().Be(2);
        }

        [TestMethod]
        public void AppConfig_Defaults_Instance()
        {
            new ConfigSetting<int>(BAD_VAL).Get().Should().Be(default);
            new ConfigSetting<int>(BAD_VAL, GOOD_INT).Get().Should().Be(GOOD_INT);

            new ConfigSetting<string>(BAD_VAL).Get().Should().Be(default);
            new ConfigSetting<string>(BAD_VAL, GOOD_INT.ToString()).Get().Should().Be(GOOD_INT.ToString());

            new ConfigSetting<DateTime>(BAD_VAL).Get().Should().Be(default);
            new ConfigSetting<DateTime>(BAD_VAL, DateTime.MinValue).Get().Should().Be(DateTime.MinValue);

            new ConfigSetting<StatusEnum>(BAD_VAL).Get().Should().Be(StatusEnum.Open);
            new ConfigSetting<StatusEnum>(BAD_VAL, StatusEnum.Closed).Get().Should().Be(StatusEnum.Closed);
        }
    }
}
