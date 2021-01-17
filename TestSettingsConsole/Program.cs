using System;
using System.IO;
using AppConfigSettings;

namespace TestSettingsConsole
{
    internal class Program
    {
        private static void Main()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "");

            var logLevel = Settings.LogLevel.Get(false);
            var path = Settings.DefaultFolder.Get(Settings.SystemRoot);
            var maxRetries = Settings.MaxRetries.Get(new ConfigSetting<int>("OtherRetry", 2));
            var level = new Settings()[nameof(Settings.LogLevel)].ToString();

            Console.WriteLine($"Static: {logLevel}");
            Console.WriteLine($"Instance: {level}");
            Console.WriteLine(path);
            Console.WriteLine(maxRetries);

            Console.WriteLine($"Current Dir: {Directory.GetCurrentDirectory()}");

            Console.WriteLine("");
            Console.WriteLine("Default:");

            Console.WriteLine($"Log Level: {Settings.LogLevel.Get()}");
            Console.WriteLine($"App Log Level: {Settings.AppLoggingLevel.Get()}");
            Console.WriteLine($"DefaultFolder: {Settings.DefaultFolder.Get()}");
            Console.WriteLine($"AllowedHosts: {Settings.AllowedHosts.Get()}");
            Console.WriteLine($"TestSetting: {Settings.TestSetting.Get()}");
            Console.WriteLine($"SystemRoot: {Settings.SystemRoot.Get()}");
            Console.WriteLine($"SystemRoot2: {Settings.SystemRoot2.Get()}");

            Console.WriteLine($"MaxRetries: {Settings.MaxRetries.Get()}");

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            Settings.MaxRetries.AddDefaultJson();
            Console.WriteLine("");
            Console.WriteLine("Development:");

            Console.WriteLine($"MaxRetries: {Settings.MaxRetries.Get()}");

            Console.ReadKey();
        }
    }
}
