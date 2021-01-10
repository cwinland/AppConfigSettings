using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using AppConfigSettings.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AppConfigSettings
{
    public class ConfigSetting<T> : IConfigSetting<T>
    {
        private const string APP_SETTINGS_NAME = "appsettings";
        private const string APP_SETTINGS_EXT = "json";
        private const string ASP_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public ConfigSetting(string key, T defaultValue)
        {
            Key = key;
            DefaultValue = defaultValue;
            Validation = _ => true;
            ThrowOnException = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="fallbackSetting">The fallback setting.</param>
        public ConfigSetting(string key, T defaultValue, ConfigSetting<T> fallbackSetting) : this(key, defaultValue) =>
            BackupConfigSetting = fallbackSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="validation">Validation for the setting.</param>
        /// <param name="throwOnException"></param>
        public ConfigSetting(
            string key, T defaultValue, Func<T, bool> validation, bool throwOnException = false) : this(key,
            defaultValue)
        {
            Validation = validation;
            ThrowOnException = throwOnException;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="validation">The validation.</param>
        /// <param name="throwOnException">if set to <c>true</c> [throw on exception].</param>
        /// <param name="fallbackConfigSetting">The fallback configuration setting.</param>
        public ConfigSetting(
            string key, T defaultValue, Func<T, bool> validation, bool throwOnException,
            ConfigSetting<T> fallbackConfigSetting) : this(key,
                                                           defaultValue,
                                                           validation,
                                                           throwOnException) =>
            BackupConfigSetting = fallbackConfigSetting;

        /// <inheritdoc />
        public List<string> JsonFiles { get; set; } = new List<string>
        {
            $"{APP_SETTINGS_NAME}.{APP_SETTINGS_EXT}",
            $"{APP_SETTINGS_NAME}.{Environment.GetEnvironmentVariable(ASP_ENVIRONMENT)}.{APP_SETTINGS_EXT}",
        };

        /// <inheritdoc />
        public bool IncludeEnvironment { get; set; } = true;

        /// <inheritdoc />
        public T DefaultValue { get; private set; }

        /// <inheritdoc />
        public Func<T, bool> Validation { get; set; }

        /// <inheritdoc />
        public ConfigSetting<T> BackupConfigSetting { get; set; }

        /// <inheritdoc />
        public NameValueCollection AppConfig { get; set; } = ConfigurationManager.AppSettings;

        /// <inheritdoc />
        public IConfigurationRoot Configuration => InitConfig(JsonFiles,
                                                              new List<NameValueCollection> { AppConfig, },
                                                              IncludeEnvironment,
                                                              false,
                                                              false);

        /// <inheritdoc />
        public string Key { get; private set; }

        /// <inheritdoc />
        public bool ThrowOnException { get; set; }

        /// <inheritdoc />
        public T Get(bool useBackupSetting = true) => Get(useBackupSetting ? BackupConfigSetting : null);

        /// <inheritdoc />
        public T Get(ConfigSetting<T> backupConfigSetting)
        {
            T setting;

            try
            {
                setting = TryGet(out var appSetting) &&
                          Validation(appSetting)
                    ? appSetting
                    : backupConfigSetting == null ||
                      !backupConfigSetting.TryGet(out var backupVal)
                        ? DefaultValue
                        : backupVal;
            }
            catch (Exception)
            {
                if (ThrowOnException)
                {
                    throw;
                }

                setting = DefaultValue;
            }

            return setting;
        }

        /// <inheritdoc />
        public bool TryGet(out T val)
        {
            var result = TryConvert(Configuration[Key], out var newVal) && Validation(newVal);

            val = result
                ? newVal
                : DefaultValue;

            return result;
        }

        /// <inheritdoc />
        public T Convert(string val)
        {
            TryConvert(val, out var result);

            return result;
        }

        /// <inheritdoc />
        public bool TryConvert(string val, out T newVal)
        {
            var valid = !string.IsNullOrWhiteSpace(val);

            try
            {
                newVal = valid
                    ? (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(val)
                    : DefaultValue;
            }
            catch
            {
                valid = false;
                newVal = DefaultValue;
            }

            return valid;
        }

        private static IConfigurationRoot InitConfig(
            List<string> jsonFiles, List<NameValueCollection> appSettingsCollections,
            bool includeEnvironmentVariables, bool addDefaultJson, bool addDefaultAppSettings)
        {
            var env = Environment.GetEnvironmentVariable(ASP_ENVIRONMENT);

            jsonFiles ??= new List<string>();
            appSettingsCollections ??= new List<NameValueCollection>();

            if (addDefaultAppSettings)
            {
                appSettingsCollections.Add(ConfigurationManager.AppSettings);
            }

            if (addDefaultJson)
            {
                jsonFiles.Add($"{APP_SETTINGS_NAME}.{APP_SETTINGS_EXT}");
                jsonFiles.Add($"{APP_SETTINGS_NAME}.{env}.{APP_SETTINGS_EXT}");
            }

            var inMemoryCollection = appSettingsCollections
                                     .Select(GetKeyPairSettings)
                                     .ToList();

            var builder = new ConfigurationBuilder();

            inMemoryCollection.ForEach(list => builder.AddInMemoryCollection(list));
            jsonFiles.ForEach(list => builder.AddJsonFile(list, true, true));

            if (includeEnvironmentVariables)
            {
                builder.AddEnvironmentVariables();
            }

            return builder.Build();
        }

        private static List<KeyValuePair<string, string>> GetKeyPairSettings(NameValueCollection appSettings) =>
            appSettings.AllKeys.Select(appKey =>
                                           new KeyValuePair<string, string>(appKey,
                                                                            appSettings[
                                                                                appKey]))
                       .ToList();
    }
}
