using System;
using System.IO;

namespace TestSettingsConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "");

            Console.WriteLine($"Current Dir: {Directory.GetCurrentDirectory()}");

            Console.WriteLine("");
            Console.WriteLine("Default:");

            Console.WriteLine($"Log Level: {Settings.LogLevel.Get()}");
            Console.WriteLine($"App Log Level: {Settings.AppLoggingLevel.Get()}");
            Console.WriteLine($"DefaultRunbookFolder: {Settings.DefaultRunbookFolder.Get()}");
            Console.WriteLine($"MaxRetries: {Settings.MaxRetries.Get()}");
            Console.WriteLine($"AllowedHosts: {Settings.AllowedHosts.Get()}");
            Console.WriteLine($"TestSetting: {Settings.TestSetting.Get()}");
            Console.WriteLine($"SystemRoot: {Settings.SystemRoot.Get()}");
            Console.WriteLine($"SystemRoot2: {Settings.SystemRoot2.Get()}");

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            Console.WriteLine("");
            Console.WriteLine("Development:");

            Console.WriteLine($"Log Level: {Settings.LogLevel.Get()}");
            Console.WriteLine($"App Log Level: {Settings.AppLoggingLevel.Get()}");
            Console.WriteLine($"DefaultRunbookFolder: {Settings.DefaultRunbookFolder.Get()}");
            Console.WriteLine($"MaxRetries: {Settings.MaxRetries.Get()}");
            Console.WriteLine($"AllowedHosts: {Settings.AllowedHosts.Get()}");
            Console.WriteLine($"TestSetting: {Settings.TestSetting.Get()}");
            Console.WriteLine($"SystemRoot: {Settings.SystemRoot.Get()}");
            Console.WriteLine($"SystemRoot2: {Settings.SystemRoot2.Get()}");
            Console.ReadKey();
        }
    }
}
