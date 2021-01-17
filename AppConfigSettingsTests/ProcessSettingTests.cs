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
        private const int NEW_INT = 15;

        private readonly NameValueCollection appConfig = new NameValueCollection
        {
            { Settings.Retries.Key, GOOD_INT.ToString() }, { Settings.Author.Key, INIT_AUTHOR },
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
                                             appConfig[selectedSetting.Key] =
                                                 NEW_INT.ToString(); // This will update the NEXT call.

                                             return true;
                                         };
            authorValue = author.Get();
            authorValue.Should().Be(INIT_AUTHOR);
            authorValue.Should().NotBe(NEW_AUTHOR); // Changed from ProcessSettingValue
            author.Get().Should().Be(NEW_INT.ToString()); // Changed from ProcessSettingValue
        }

        [TestMethod]
        public void ProcessSetting_Int()
        {
            var testInt = 0;
            var retrySetting = Settings.Retries;
            retrySetting.ProcessSettingValue = selectedSetting =>
                                               {
                                                   testInt = selectedSetting.Value + 3;
                                                   appConfig[selectedSetting.Key] =
                                                       NEW_INT.ToString(); // This will update the NEXT call.

                                                   return true;
                                               };
            var val = retrySetting.Get(); // ProcessSettingValue ran after getting this value
            testInt.Should().Be(GOOD_INT + 3);
            val.Should().NotBe(GOOD_INT + 3);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ProcessSetting_TrueFalse(bool isValid)
        {
            var retrySetting = Settings.Retries;
            retrySetting.ProcessSettingValue = selectedSetting =>
                                               {
                                                   appConfig[selectedSetting.Key] =
                                                       NEW_INT.ToString(); // This will update the NEXT call.

                                                   return isValid;
                                               };
            var val = retrySetting.Get(); // ProcessSettingValue ran after getting this value

            if (isValid)
            {
                val.Should().Be(GOOD_INT);
                retrySetting.Get().Should().Be(NEW_INT); // New Value
            }
            else
            {
                val.Should().Be(retrySetting.DefaultValue);
                retrySetting.Get().Should().Be(retrySetting.DefaultValue); // New Value
            }
        }
    }
}
