using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace AppConfigSettingsConsoleTest
{
    class Program
    {
        private static void Main(string[] args)
        {
            var configuration = InitConfig();

            var version = configuration["Version"];
            var version2 = configuration.GetValue<string>("Version");
            var apiKey = configuration["ApiKey"];
            var apiAddress = configuration["ApiAddress"];
            var users = configuration.GetSection("Auth:Users").GetChildren().Select(x => x.Value);
            var users2 = configuration.GetSection("Auth:Users").Get<List<string>>();
            var availableDays = configuration.GetSection("ServerAvailable:Days").GetChildren().Select(x => x.Value);
            var updateAtHours = configuration.GetSection("UpdatesOn:Hours").GetChildren().Select(x => x.Value);

            Console.ReadLine();
        }

        private static IConfigurationRoot InitConfig()
        {
            var configSettings = ConfigurationManager.AppSettings.AllKeys
                                                     .Select(appKey => new KeyValuePair<string, string>(
                                                                 appKey,
                                                                 ConfigurationManager.AppSettings[appKey]))
                                                     .ToList();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                          .AddJsonFile("appsettings.json", true, true)
                          .AddJsonFile($"appsettings.{env}.json", true, true)
                          .AddInMemoryCollection(configSettings)
                          .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
