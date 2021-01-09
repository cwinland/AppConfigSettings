using System.Collections.Specialized;
using System.Configuration;
using AppConfigSettings;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppConfigSettingsTests
{
    [TestClass]
    public class InstanceTests : TestBase
    {
        private Settings settings;
        private Settings2 settings2;

        private readonly NameValueCollection appConfig2 = new NameValueCollection
        {
            { Settings2.TestPath.Key, @"C:\windows" },
            { Settings2.Retries.Key, "22" },
            { Settings2.Sections.Key, "4" },
        };

        private readonly NameValueCollection appConfig1 = new NameValueCollection
        {
            { Settings.Path.Key, BadVal },
            { Settings.Retries.Key, "0" },
            { Settings.Author.Key, "Sam" },
            { Settings.Created.Key, GoodDate },
            { Settings.DefaultStatus.Key, StatusEnum.Unknown.ToString() },
        };

        private readonly NameValueCollection appConfig2Bad = new NameValueCollection
        {
            { Settings2.TestPath.Key, BadVal }, { Settings2.Retries.Key, "-1" },
        };

        [TestInitialize]
        public void Settings_InitTest()
        {
            ConfigSetting<Settings>.SetAppSettings(appConfig1);
            ConfigSetting<Settings2>.SetAppSettings(appConfig2);

            settings = new Settings();
            settings2 = new Settings2();
        }

        [TestCleanup]
        public void Settings_CleanTest() => ConfigSetting<Settings>.SetAppSettings(ConfigurationManager.AppSettings);

        [TestMethod]
        public void InstanceTests_Type()
        {
            settings.GetType().Name.Should().Be("Settings");
            settings2.GetType().Name.Should().Be("Settings2");
        }

        [TestMethod]
        public void InstanceTests_CompareWithBackup()
        {
            settings[Settings.Author.Key].Should().Be(Settings.Author.Get());
            settings[Settings.Created.Key].Should().Be(Settings.Created.Get());
            settings[Settings.DefaultStatus.Key].Should().Be(Settings.DefaultStatus.Get());
            settings[Settings.Path.Key].Should().Be(Settings.Path.Get());
            settings[Settings.Retries.Key].Should().Be(Settings.Retries.Get());
        }

        [TestMethod]
        public void InstanceTests_CompareWithoutBackup()
        {
            settings[Settings.Author.Key].Should().Be(Settings.Author.Get(false));
            settings[Settings.Created.Key].Should().Be(Settings.Created.Get(false));
            settings[Settings.DefaultStatus.Key].Should().Be(Settings.DefaultStatus.Get(false));

            settings[Settings.Path.Key].Should().NotBe(Settings.Path.Get(false));
            settings[Settings.Retries.Key].Should().NotBe(Settings.Retries.Get(false));

            settings[Settings.Path.Key].Should().Be(Settings2.TestPath.Get());
            settings[Settings.Retries.Key].Should().Be(Settings2.Retries.Get());
        }
    }
}
