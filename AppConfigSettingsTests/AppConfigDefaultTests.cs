using System;
using System.Collections.Specialized;
using System.Configuration;
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
        public void AppConfig_Defaults()
        {
            Settings.Author.Get().Should().Be("Chris");
            Settings.Created.Get().Should().Be(DateTime.Today);
            Settings.DefaultStatus.Get().Should().Be(StatusEnum.Open);
            Settings.Path.Get().Should().Be(@"C:\");
            Settings.Retries.Get().Should().Be(2);
        }
    }
}
