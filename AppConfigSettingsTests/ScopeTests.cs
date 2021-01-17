using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using AppConfigSettings;
using AppConfigSettings.Enum;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppConfigSettingsTests
{
    [TestClass]
    public class ScopeTests : TestBase
    {
        private const string JSON = "FromJson";
        private const string APP = "FromApp";
        private const string ENV = "FromEnv";

        private const string TEST1_KEY = "Test1";
        private const string TEST2_KEY = "Test2";

        private const string JSON_CONFIG =
            "{\r\n  \"Test1\": \"FromJson\",\r\n  \"Test2\": \"FromJson\"\r\n}";

        private static ConfigSetting<string> test1 = new ConfigSetting<string>("Test1");
        private static ConfigSetting<string> test2 = new ConfigSetting<string>("Test2");

        private readonly NameValueCollection appConfig = new NameValueCollection
        {
            { TEST1_KEY, APP }, { TEST2_KEY, APP },
        };

        private readonly List<string> filePaths = new List<string>();

        [TestInitialize]
        public void Settings_InitTest()
        {
            Environment.SetEnvironmentVariable(TEST1_KEY, ENV);
            Environment.SetEnvironmentVariable(TEST2_KEY, ENV);

            filePaths.Add(CreateFile($"{APP_SETTINGS_NAME}.{APP_SETTINGS_EXT}",
                                     JSON_CONFIG,
                                     Directory.GetCurrentDirectory()));
        }

        [TestCleanup]
        public void Settings_CleanTest()
        {
            Environment.SetEnvironmentVariable(TEST1_KEY, null);
            Environment.SetEnvironmentVariable(TEST2_KEY, null);

            Settings.SetAppSettings(ConfigurationManager.AppSettings);
            filePaths.ForEach(DeleteFile);
        }

        [TestMethod]
        public void Scope_ShouldBe_JSON_APP_ENV()
        {
            test1 = new ConfigSetting<string>(TEST1_KEY);
            test1.SetAppSettings(appConfig);
            test2 = new ConfigSetting<string>(TEST2_KEY);
            test2.SetAppSettings(appConfig);

            test1.Get().Should().Be(JSON);
            test2.Get().Should().Be(JSON);

            test1 = new ConfigSetting<string>(TEST1_KEY,
                                              default,
                                              SettingScopes.AppSettings |
                                              SettingScopes.Environment |
                                              SettingScopes.Json);
            test1.SetAppSettings(appConfig);

            test2 = new ConfigSetting<string>(TEST2_KEY);
            test2.SetAppSettings(appConfig);

            test1.Get().Should().Be(JSON);
            test2.Get().Should().Be(JSON);

            test1 = new ConfigSetting<string>(TEST1_KEY,
                                              default,
                                              SettingScopes.AppSettings | SettingScopes.Environment);
            test1.SetAppSettings(appConfig);

            test2 = new ConfigSetting<string>(TEST2_KEY,
                                              default,
                                              SettingScopes.AppSettings | SettingScopes.Environment);
            test2.SetAppSettings(appConfig);

            test1.Get().Should().Be(APP);
            test2.Get().Should().Be(APP);

            test1 = new ConfigSetting<string>(TEST1_KEY, default, SettingScopes.Environment);
            test1.SetAppSettings(appConfig);

            test2 = new ConfigSetting<string>(TEST2_KEY, default, SettingScopes.Environment);
            test2.SetAppSettings(appConfig);

            test1.Get().Should().Be(ENV);
            test2.Get().Should().Be(ENV);
        }

        [TestMethod]
        public void Scope_ShouldBe_AppSettings()
        {
            test1 = new ConfigSetting<string>(TEST1_KEY, default, SettingScopes.AppSettings);
            test1.SetAppSettings(appConfig);

            test2 = new ConfigSetting<string>(TEST2_KEY, default, SettingScopes.AppSettings);
            test2.SetAppSettings(appConfig);

            test1.Get().Should().Be(APP);
            test2.Get().Should().Be(APP);
        }

        [TestMethod]
        public void Scope_ShouldBe_Json()
        {
            test1 = new ConfigSetting<string>(TEST1_KEY, default, SettingScopes.Json);
            test1.SetAppSettings(appConfig);

            test2 = new ConfigSetting<string>(TEST2_KEY, default, SettingScopes.Json);
            test2.SetAppSettings(appConfig);

            test1.Get().Should().Be(JSON);
            test2.Get().Should().Be(JSON);
        }

        [TestMethod]
        public void Scope_ShouldBe_AppEnvironment()
        {
            test1 = new ConfigSetting<string>(TEST1_KEY, default, SettingScopes.Environment);
            test1.SetAppSettings(appConfig);

            test2 = new ConfigSetting<string>(TEST2_KEY, default, SettingScopes.Environment);
            test2.SetAppSettings(appConfig);

            test1.Get().Should().Be(ENV);
            test2.Get().Should().Be(ENV);
        }
    }
}
