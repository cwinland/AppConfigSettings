﻿using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppConfigSettingsTests
{
    [TestClass]
    public class JsonTests : TestBase
    {
        private readonly string jsonConfig =
            "{\r\n  \"Logging\": {\r\n    \"LogLevel\": {\r\n      \"Default\": \"Information\",\r\n      \"Microsoft\": \"Warning\",\r\n      \"Microsoft.Hosting.Lifetime\": \"Warning\"\r\n    }\r\n  },\r\n  \"AllowedHosts\": \"*\",\r\n  \"MyCustomKey\": \"MyCustomKey Value coming from appsettings.json\"\r\n}";

        private readonly string jsonConfigDev =
            "{\r\n  \"Logging\": {\r\n    \"LogLevel\": {\r\n      \"Default\": \"Verbose\",\r\n      \"Microsoft\": \"Warning\",\r\n      \"Microsoft.Hosting.Lifetime\": \"Warning\"\r\n    }\r\n  },\r\n  \"AllowedHosts\": \"*\",\r\n  \"MyCustomKey\": \"MyCustomKey Value coming from appsettings.dev.json\"\r\n}";

        private readonly string jsonConfigTest =
            "{\r\n  \"Logging\": {\r\n    \"LogLevel\": {\r\n      \"Default\": \"Warning\",\r\n      \"Microsoft\": \"Warning\",\r\n      \"Microsoft.Hosting.Lifetime\": \"Warning\"\r\n    }\r\n  },\r\n  \"AllowedHosts\": \"*\",\r\n  \"MyCustomKey\": \"MyCustomKey Value coming from appsettings.test.json\"\r\n}";

        private readonly List<string> filePaths = new List<string>();

        [TestInitialize]
        public void Settings_InitTest()
        {
            filePaths.Add(CreateFile($"{APP_SETTINGS_NAME}.{APP_SETTINGS_EXT}",
                                     jsonConfig,
                                     Directory.GetCurrentDirectory()));
            filePaths.Add(CreateFile($"{APP_SETTINGS_NAME}.dev.{APP_SETTINGS_EXT}",
                                     jsonConfigDev,
                                     Directory.GetCurrentDirectory()));
            filePaths.Add(CreateFile($"{APP_SETTINGS_NAME}.test.{APP_SETTINGS_EXT}",
                                     jsonConfigTest,
                                     Directory.GetCurrentDirectory()));
        }

        [TestCleanup]
        public void Settings_CleanTest() => filePaths.ForEach(DeleteFile);

        private static void SetupJsonEnv(string env)
        {
            Environment.SetEnvironmentVariable(ASP_ENVIRONMENT, env);

            var jsonFiles = new List<string>
            {
                string.IsNullOrEmpty(env)
                    ? $"{APP_SETTINGS_NAME}.{APP_SETTINGS_EXT}"
                    : $"{APP_SETTINGS_NAME}.{env}.{APP_SETTINGS_EXT}",
            };

            Settings.LogLevelLife.JsonFiles = jsonFiles;
            Settings.LogLevelDefault.JsonFiles = jsonFiles;
        }

        [TestMethod]
        [DataRow("", "Warning", "Information")]
        [DataRow("dev", "Warning", "Verbose")]
        [DataRow("test", "Warning", "Warning")]
        public void Get(string env, string expectedLife, string expectedDefault)
        {
            SetupJsonEnv(env);

            Settings.LogLevelLife.Get().Should().Be(expectedLife);
            Settings.LogLevelDefault.Get().Should().Be(expectedDefault);
            var settings = new Settings();
            settings[Settings.LogLevelDefault.Key].Should().Be(expectedDefault);
            settings[Settings.LogLevelLife.Key].Should().Be(expectedLife);
        }

        [TestMethod]
        [DataRow("", "Warning", "Information")]
        [DataRow("dev", "Warning", "Verbose")]
        [DataRow("test", "Warning", "Warning")]
        public void TryGet_Good(string env, string expectedLife, string expectedDefault)
        {
            SetupJsonEnv(env);

            Settings.LogLevelLife.TryGet(out var a1).Should().BeTrue();
            Settings.LogLevelDefault.TryGet(out var a2).Should().BeTrue();
            a1.Should().Be(expectedLife);
            a2.Should().Be(expectedDefault);
        }
    }
}
