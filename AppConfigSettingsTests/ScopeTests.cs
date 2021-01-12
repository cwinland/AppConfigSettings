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

        private const string Test1Key = "Test1";
        private const string Test2Key = "Test2";

        private const string JSON_CONFIG =
            "{\r\n  \"Test1\": \"FromJson\",\r\n  \"Test2\": \"FromJson\"\r\n}";

        private static ConfigSetting<string> test1 = new ConfigSetting<string>("Test1", default);
        private static ConfigSetting<string> test2 = new ConfigSetting<string>("Test2", default);

        private readonly NameValueCollection appConfig = new NameValueCollection
        {
            { Test1Key, APP }, { Test2Key, APP },
        };

        private readonly List<string> filePaths = new List<string>();

        [TestInitialize]
        public void Settings_InitTest()
        {
            Environment.SetEnvironmentVariable(Test1Key, ENV);
            Environment.SetEnvironmentVariable(Test2Key, ENV);

            filePaths.Add(CreateFile($"{APP_SETTINGS_NAME}.{APP_SETTINGS_EXT}",
                                     JSON_CONFIG,
                                     Directory.GetCurrentDirectory()));
        }

        [TestCleanup]
        public void Settings_CleanTest()
        {
            Environment.SetEnvironmentVariable(Test1Key, null);
            Environment.SetEnvironmentVariable(Test2Key, null);

            Settings.SetAppSettings(ConfigurationManager.AppSettings);
            filePaths.ForEach(DeleteFile);
        }

        [TestMethod]
        public void Scope_ShouldBe_JSON_APP_ENV()
        {
            test1 = new ConfigSetting<string>(Test1Key, default) { AppConfig = appConfig, };
            test2 = new ConfigSetting<string>(Test2Key, default) { AppConfig = appConfig, };

            test1.Get().Should().Be(JSON);
            test2.Get().Should().Be(JSON);

            test1 = new ConfigSetting<string>(Test1Key,
                                              default,
                                              SettingScopes.AppSettings |
                                              SettingScopes.Environment |
                                              SettingScopes.Json) { AppConfig = appConfig, };
            test2 = new ConfigSetting<string>(Test2Key, default) { AppConfig = appConfig, };

            test1.Get().Should().Be(JSON);
            test2.Get().Should().Be(JSON);

            test1 = new ConfigSetting<string>(Test1Key, default, SettingScopes.AppSettings | SettingScopes.Environment)
            {
                AppConfig = appConfig,
            };
            test2 = new ConfigSetting<string>(Test2Key, default, SettingScopes.AppSettings | SettingScopes.Environment)
            {
                AppConfig = appConfig,
            };

            test1.Get().Should().Be(APP);
            test2.Get().Should().Be(APP);

            test1 = new ConfigSetting<string>(Test1Key, default, SettingScopes.Environment) { AppConfig = appConfig, };
            test2 = new ConfigSetting<string>(Test2Key, default, SettingScopes.Environment) { AppConfig = appConfig, };

            test1.Get().Should().Be(ENV);
            test2.Get().Should().Be(ENV);
        }

        [TestMethod]
        public void Scope_ShouldBe_AppSettings()
        {
            test1 = new ConfigSetting<string>(Test1Key, default, SettingScopes.AppSettings) { AppConfig = appConfig, };
            test2 = new ConfigSetting<string>(Test2Key, default, SettingScopes.AppSettings) { AppConfig = appConfig, };

            test1.Get().Should().Be(APP);
            test2.Get().Should().Be(APP);
        }

        [TestMethod]
        public void Scope_ShouldBe_Json()
        {
            test1 = new ConfigSetting<string>(Test1Key, default, SettingScopes.Json) { AppConfig = appConfig, };
            test2 = new ConfigSetting<string>(Test2Key, default, SettingScopes.Json) { AppConfig = appConfig, };

            test1.Get().Should().Be(JSON);
            test2.Get().Should().Be(JSON);
        }

        [TestMethod]
        public void Scope_ShouldBe_AppEnvironment()
        {
            test1 = new ConfigSetting<string>(Test1Key, default, SettingScopes.Environment) { AppConfig = appConfig, };
            test2 = new ConfigSetting<string>(Test2Key, default, SettingScopes.Environment) { AppConfig = appConfig, };

            test1.Get().Should().Be(ENV);
            test2.Get().Should().Be(ENV);
        }
    }
}
