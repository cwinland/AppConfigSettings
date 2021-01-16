using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppConfigSettingsTests
{
    [TestClass]
    public class AppConfigTests : TestBase
    {
        private readonly NameValueCollection appConfig = new NameValueCollection
        {
            { Settings.Author.Key, "Tom" },
            { Settings.Created.Key, GOOD_DATE },
            { Settings.DefaultStatus.Key, StatusEnum.Unknown.ToString() },
            { Settings.Path.Key, Directory.GetCurrentDirectory() },
            { Settings.Retries.Key, "3" },
        };

        private readonly NameValueCollection appConfigBad = new NameValueCollection
        {
            { Settings.Created.Key, BAD_DATE },
            { Settings.DefaultStatus.Key, StatusEnum.Closed.ToString() },
            { Settings.Path.Key, BAD_VAL },
            { Settings.Retries.Key, "0" },
        };

        [TestInitialize]
        public void Settings_InitTest() => Settings.SetAppSettings(appConfig);

        [TestCleanup]
        public void Settings_CleanTest() => Settings.SetAppSettings(ConfigurationManager.AppSettings);

        [TestMethod]
        public void Get()
        {
            Settings.Author.Get().Should().Be("Tom");
            Settings.Created.Get().Should().Be(DateTime.Parse(GOOD_DATE));
            Settings.DefaultStatus.Get().Should().Be(StatusEnum.Unknown);
            Settings.Path.Get().Should().Be(Directory.GetCurrentDirectory());
            Settings.Retries.Get().Should().Be(3);
            Settings.HomePath.Get(false).Should().Be(Environment.GetEnvironmentVariable("HomePath") ?? string.Empty);
            Settings2.Public.Get(false).Should().Be(Environment.GetEnvironmentVariable("Public") ?? string.Empty);
        }

        [TestMethod]
        public void TryGet_Good()
        {
            Settings.Author.TryGet(out var a1).Should().BeTrue();
            a1.Should().Be("Tom");
            Settings.Created.TryGet(out var a2).Should().BeTrue();
            a2.Should().Be(DateTime.Parse(GOOD_DATE));
            Settings.DefaultStatus.TryGet(out var a3).Should().BeTrue();
            a3.Should().Be(StatusEnum.Unknown);
            Settings.Path.TryGet(out var a4).Should().BeTrue();
            a4.Should().Be(Directory.GetCurrentDirectory());
            Settings.Retries.TryGet(out var a5).Should().BeTrue();
            a5.Should().Be(3);
        }

        [TestMethod]
        public void TryGet_Bad()
        {
            Settings.SetAppSettings(appConfigBad);
            Settings.Author.TryGet(out var a1).Should().BeFalse();
            a1.Should().Be(Settings.Author.DefaultValue);
            Settings.Created.TryGet(out var a2).Should().BeFalse();
            a2.Should().Be(Settings.Created.DefaultValue);
            Settings.DefaultStatus.TryGet(out var a3).Should().BeFalse();
            a3.Should().Be(Settings.DefaultStatus.DefaultValue);
            Settings.Path.TryGet(out var a4).Should().BeFalse();
            a4.Should().Be(Settings.Path.DefaultValue);
            Settings.Retries.TryGet(out var a5).Should().BeFalse();
            a5.Should().Be(Settings.Retries.DefaultValue);
        }

        [TestMethod]
        public void Convert_BadConversion()
        {
            Settings.Created.Convert(BAD_VAL).Should().Be(Settings.Created.DefaultValue);
            Settings.DefaultStatus.Convert(BAD_VAL).Should().Be(Settings.DefaultStatus.DefaultValue);
            Settings.Retries.Convert(BAD_VAL).Should().Be(Settings.Retries.DefaultValue);
        }

        [TestMethod]
        public void Convert_GoodConversion()
        {
            Settings.Created.Convert(GOOD_DATE).Should().Be(DateTime.Parse(GOOD_DATE));
            Settings.DefaultStatus.Convert(StatusEnum.Unknown.ToString()).Should().Be(StatusEnum.Unknown);
            Settings.Retries.Convert("44").Should().Be(44);
        }

        [TestMethod]
        public void TryConvert_Bad()
        {
            Settings.Created.TryConvert(BAD_VAL, out var a1).Should().BeFalse();
            a1.Should().Be(Settings.Created.DefaultValue);
            Settings.Retries.TryConvert(BAD_VAL, out var a2).Should().BeFalse();
            a2.Should().Be(Settings.Retries.DefaultValue);
            Settings.DefaultStatus.TryConvert(BAD_VAL, out var a3).Should().BeFalse();
            a3.Should().Be(Settings.DefaultStatus.DefaultValue);
        }

        [TestMethod]
        public void TryConvert_Good()
        {
            Settings.Created.TryConvert(GOOD_DATE, out var a1).Should().BeTrue();
            a1.Should().Be(DateTime.Parse(GOOD_DATE));
            Settings.Retries.TryConvert("1", out var a2).Should().BeTrue();
            a2.Should().Be(1);
            Settings.DefaultStatus.TryConvert("Open", out var a3).Should().BeTrue();
            a3.Should().Be(StatusEnum.Open);
        }
    }
}
