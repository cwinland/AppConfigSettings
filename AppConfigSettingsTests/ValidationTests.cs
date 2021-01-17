using System;
using System.Collections.Specialized;
using System.IO;
using AppConfigSettings;
using AppConfigSettings.Enum;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppConfigSettingsTests
{
    [TestClass]
    public class ValidationTests : TestBase
    {
        private const string KEY = "TEST";

        private readonly NameValueCollection appConfig = new NameValueCollection { { KEY, GOOD_INT.ToString() }, };

        private readonly ConfigSetting<int> setting =
            new ConfigSetting<int>(KEY, 21, SettingScopes.Any, i => i > GOOD_INT);

        private readonly ConfigSetting<int> setting2 =
            new ConfigSetting<int>(KEY, 19, SettingScopes.Any, i => i > GOOD_INT);

        private readonly ConfigSetting<int> setting3 =
            new ConfigSetting<int>(BAD_VAL, 19, SettingScopes.Any, i => i > GOOD_INT);

        [TestInitialize]
        public void InitTests()
        {
            setting.SetAppSettings(appConfig);
            setting2.SetAppSettings(appConfig);
            setting3.SetAppSettings(appConfig);
        }

        [TestMethod]
        public void TestException_False()
        {
            Func<int> a1 = () => setting.Get();
            a1.Should().NotThrow();

            Func<int> a2 = () => setting2.Get();
            a2.Should().NotThrow();

            Func<int> a4 = () => setting3.Get();
            a4.Should().NotThrow();
        }

        [TestMethod]
        public void TestException_True()
        {
            setting2.ThrowOnException = true;
            Func<int> a2 = () => setting2.Get();
            a2.Should().Throw<InvalidDataException>();

            setting3.ThrowOnException = true;
            Func<int> a4 = () => setting3.Get();
            a4.Should().Throw<InvalidCastException>();
        }
    }
}
