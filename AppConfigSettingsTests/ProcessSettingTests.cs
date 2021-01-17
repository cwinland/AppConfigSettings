using System.Collections.Specialized;
using System.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppConfigSettingsTests
{
    [TestClass]
    public class ProcessSettingTests : TestBase
    {
        private const string INIT_AUTHOR = "Chris";
        private const string NEW_AUTHOR = "Tom";
        private const string NEW_TEST_KEY = "TESTNUM";
        private const int NEW_TEST_VAL = 45;

        private readonly NameValueCollection appConfig = new NameValueCollection
        {
            { Settings.Retries.Key, GOOD_INT.ToString() },
            { Settings.Author.Key, INIT_AUTHOR },
            { NEW_TEST_KEY, NEW_TEST_VAL.ToString() },
        };

        [TestInitialize]
        public void InitTest() => Settings.SetAppSettings(appConfig);

        [TestCleanup]
        public void CleanTest() => Settings.SetAppSettings(ConfigurationManager.AppSettings);

        [TestMethod]
        public void ProcessSetting_Author()
        {
            var author = Settings.Author;
            var authorValue = string.Empty;
            author.ProcessSettingValue = selectedSetting =>
                                         {
                                             authorValue = NEW_AUTHOR;

                                             return true;
                                         };
            authorValue = author.Get();
            authorValue.Should().Be(INIT_AUTHOR);
            authorValue.Should().NotBe(NEW_AUTHOR); // Changed from ProcessSettingValue
        }

        [TestMethod]
        public void ProcessSetting_Int()
        {
            var testInt = 0;
            var testKey = "";
            var retrySetting = Settings.Retries;
            retrySetting.ProcessSettingValue = selectedSetting =>
                                               {
                                                   testInt = selectedSetting.Value + 3;
                                                   testKey = selectedSetting.Key;

                                                   return true;
                                               };
            retrySetting.Get(); // ProcessSettingValue ran after getting this value
            testInt.Should().Be(GOOD_INT + 3);
            testKey.Should().Be(Settings.Retries.Key);
        }
    }
}
