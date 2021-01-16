using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
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
            { Settings2.TestPath.Key, Directory.GetCurrentDirectory() },
            { Settings2.Retries.Key, "22" },
            { Settings2.Sections.Key, "4" },
        };

        private readonly NameValueCollection appConfig1 = new NameValueCollection
        {
            { Settings.Path.Key, BAD_VAL },
            { Settings.Retries.Key, "0" },
            { Settings.Author.Key, "Sam" },
            { Settings.Created.Key, GOOD_DATE },
            { Settings.DefaultStatus.Key, StatusEnum.Unknown.ToString() },
        };

        private readonly NameValueCollection appConfig2Bad = new NameValueCollection
        {
            { Settings2.TestPath.Key, BAD_VAL }, { Settings2.Retries.Key, "-1" },
        };

        [TestInitialize]
        public void Settings_InitTest()
        {
            Settings.SetAppSettings(appConfig1);
            Settings2.SetAppSettings(appConfig2);

            settings = new Settings();
            settings2 = new Settings2();
        }

        [TestCleanup]
        public void Settings_CleanTest() => Settings.SetAppSettings(ConfigurationManager.AppSettings);

        [TestMethod]
        public void InstanceTests_Type()
        {
            settings.GetType().Name.Should().Be("Settings");
            settings2.GetType().Name.Should().Be("Settings2");
        }

        [TestMethod]
        public void InstanceTests_CompareWithBackup()
        {
            settings[nameof(Settings.Author)].Should().Be(Settings.Author.Get());
            settings[nameof(Settings.Created)].Should().Be(Settings.Created.Get());
            settings[nameof(Settings.DefaultStatus)].Should().Be(Settings.DefaultStatus.Get());
            settings[nameof(Settings.Path)].Should().Be(Settings.Path.Get());
            settings[nameof(Settings.Retries)].Should().Be(Settings.Retries.Get());
        }

        [TestMethod]
        public void InstanceTests_CompareWithoutBackup()
        {
            settings[nameof(Settings.Author)].Should().Be(Settings.Author.Get(false));
            settings[nameof(Settings.Created)].Should().Be(Settings.Created.Get(false));
            settings[nameof(Settings.DefaultStatus)].Should().Be(Settings.DefaultStatus.Get(false));

            settings[nameof(Settings.Path)].Should().NotBe(Settings.Path.Get(false));
            settings[nameof(Settings.Retries)].Should().NotBe(Settings.Retries.Get(false));

            settings[nameof(Settings.Path)].Should().Be(Settings2.TestPath.Get());
            settings[nameof(Settings.Retries)].Should().Be(Settings2.Retries.Get());
        }

        [TestMethod]
        public void EnvironmentVariables()
        {
            Settings2.Public.Get().Should().Be(Environment.GetEnvironmentVariable("Public") ?? string.Empty);
            settings2[Settings2.Public.Key].Should().Be(Settings2.Public.Get());
        }
    }
}
