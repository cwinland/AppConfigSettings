using System.Collections.Specialized;
using System.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppConfigSettingsTests
{
    [TestClass]
    public class AppConfigFallbackTests : TestBase
    {
        private readonly NameValueCollection appConfig2 = new NameValueCollection
        {
            { Settings2.TestPath.Key, @"C:\" }, { Settings2.Retries.Key, "22" }, { Settings2.Sections.Key, "4" },
        };

        private readonly NameValueCollection appConfig1Bad = new NameValueCollection
        {
            { Settings.Path.Key, BadVal }, { Settings.Retries.Key, "0" },
        };

        private readonly NameValueCollection appConfig2Bad = new NameValueCollection
        {
            { Settings2.TestPath.Key, BadVal }, { Settings2.Retries.Key, "-1" },
        };

        [TestInitialize]
        public void InitTest()
        {
            Settings.SetAppSettings(appConfig1Bad);
            Settings2.SetAppSettings(appConfig2);
        }

        [TestCleanup]
        public void CleanTest() => Settings.SetAppSettings(ConfigurationManager.AppSettings);

        [TestMethod]
        public void GetSettings()
        {
            Settings.Path.Get(false).Should().Be(Settings.Path.DefaultValue);
            Settings.Retries.Get(false).Should().Be(Settings.Retries.DefaultValue);
        }

        [TestMethod]
        public void GetSettings2()
        {
            Settings2.Sections.Get().Should().Be(4);
            Settings2.TestPath.Get().Should().Be(@"C:\");
            Settings2.Retries.Get().Should().Be(22);
        }

        [TestMethod]
        public void GetSettingsBackupImplicit()
        {
            Settings.Path.Get().Should().Be(@"C:\");
            Settings.Retries.Get().Should().Be(22);
        }

        [TestMethod]
        public void GetSettingsBackupExplicit()
        {
            Settings.Path.Get(Settings2.TestPath).Should().Be(@"C:\");
            Settings.Retries.Get(Settings2.Retries).Should().Be(22);
        }

        [TestMethod]
        public void GetSettingsBackup_Fail()
        {
            Settings2.SetAppSettings(appConfig2Bad);
            Settings.Path.Get(Settings2.TestPath).Should().Be(Settings.Path.DefaultValue);
            Settings.Retries.Get(Settings2.Retries).Should().Be(Settings.Retries.DefaultValue);
        }
    }
}
