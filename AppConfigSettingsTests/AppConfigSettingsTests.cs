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
            { Settings.Created.Key, GoodDate },
            { Settings.DefaultStatus.Key, StatusEnum.Unknown.ToString() },
            { Settings.Path.Key, Directory.GetCurrentDirectory() },
            { Settings.Retries.Key, "3" },
        };

        private readonly NameValueCollection appConfigBad = new NameValueCollection
        {
            { Settings.Created.Key, BadDate },
            { Settings.DefaultStatus.Key, StatusEnum.Closed.ToString() },
            { Settings.Path.Key, BadVal },
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
            Settings.Created.Get().Should().Be(DateTime.Parse(GoodDate));
            Settings.DefaultStatus.Get().Should().Be(StatusEnum.Unknown);
            Settings.Path.Get().Should().Be(Directory.GetCurrentDirectory());
            Settings.Retries.Get().Should().Be(3);
            Settings.HomePath.Get().Should().Be(Environment.GetEnvironmentVariable("HomePath"));
            Settings2.Public.Get().Should().Be(Environment.GetEnvironmentVariable("Public"));
        }

        [TestMethod]
        public void TryGet_Good()
        {
            Settings.Author.TryGet(out var a1).Should().BeTrue();
            a1.Should().Be("Tom");
            Settings.Created.TryGet(out var a2).Should().BeTrue();
            a2.Should().Be(DateTime.Parse(GoodDate));
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
            Settings.Created.Convert(BadVal).Should().Be(Settings.Created.DefaultValue);
            Settings.DefaultStatus.Convert(BadVal).Should().Be(Settings.DefaultStatus.DefaultValue);
            Settings.Retries.Convert(BadVal).Should().Be(Settings.Retries.DefaultValue);
        }

        [TestMethod]
        public void Convert_GoodConversion()
        {
            Settings.Created.Convert(GoodDate).Should().Be(DateTime.Parse(GoodDate));
            Settings.DefaultStatus.Convert(StatusEnum.Unknown.ToString()).Should().Be(StatusEnum.Unknown);
            Settings.Retries.Convert("44").Should().Be(44);
        }

        [TestMethod]
        public void TryConvert_Bad()
        {
            Settings.Created.TryConvert(BadVal, out var a1).Should().BeFalse();
            a1.Should().Be(Settings.Created.DefaultValue);
            Settings.Retries.TryConvert(BadVal, out var a2).Should().BeFalse();
            a2.Should().Be(Settings.Retries.DefaultValue);
            Settings.DefaultStatus.TryConvert(BadVal, out var a3).Should().BeFalse();
            a3.Should().Be(Settings.DefaultStatus.DefaultValue);
        }

        [TestMethod]
        public void TryConvert_Good()
        {
            Settings.Created.TryConvert(GoodDate, out var a1).Should().BeTrue();
            a1.Should().Be(DateTime.Parse(GoodDate));
            Settings.Retries.TryConvert("1", out var a2).Should().BeTrue();
            a2.Should().Be(1);
            Settings.DefaultStatus.TryConvert("Open", out var a3).Should().BeTrue();
            a3.Should().Be(StatusEnum.Open);
        }
    }
}
