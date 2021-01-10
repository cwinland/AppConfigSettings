using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using AppConfigSettings.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AppConfigSettings
{
    public class ConfigSetting<T> : IConfigSetting
    {
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

        public ConfigSetting(
            string key, T defaultValue, Func<T, bool> validation, bool throwOnException,
            ConfigSetting<T> fallbackConfigSetting) : this(key,
                                                           defaultValue,
                                                           validation,
                                                           throwOnException) =>
            BackupConfigSetting = fallbackConfigSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="validation"></param>
        /// <param name="throwOnException"></param>
        /// <param name="appConfig"></param>
        public ConfigSetting(
            string key, T defaultValue, Func<T, bool> validation, bool throwOnException,
            NameValueCollection appConfig) : this(key,
                                                  defaultValue,
                                                  validation,
                                                  throwOnException) => AppConfig = appConfig;

        /// <inheritdoc />
        public NameValueCollection AppConfig { get; set; } = ConfigurationManager.AppSettings;

        public List<string> JsonFiles { get; set; } = new List<string>
        {
            "appsettings.json", $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
        };

        public bool IncludeEnvironment { get; set; } = true;

        /// <inheritdoc />
        public IConfigurationRoot Configuration => InitConfig(JsonFiles,
                                                              new List<NameValueCollection> { AppConfig, },
                                                              IncludeEnvironment,
                                                              false,
                                                              false);

        private IConfigurationRoot InitConfig(
            List<string> jsonFiles, List<NameValueCollection> appSettingsCollections,
            bool includeEnvironmentVariables, bool addDefaultJson, bool addDefaultAppSettings)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            jsonFiles ??= new List<string>();

            appSettingsCollections ??= new List<NameValueCollection>();

            if (addDefaultAppSettings)
            {
                appSettingsCollections.Add(ConfigurationManager.AppSettings);
            }

            if (addDefaultJson)
            {
                jsonFiles.Add("appsettings.json");
                jsonFiles.Add($"appsettings.{env}.json");
            }

            var inMemoryCollection = appSettingsCollections
                                     .Select(GetAppSettingsList)
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

        private static List<KeyValuePair<string, string>> GetAppSettingsList(NameValueCollection appSettings) =>
            appSettings.AllKeys.Select(appKey =>
                                           new KeyValuePair<string, string>(appKey,
                                                                            appSettings[
                                                                                appKey]))
                       .ToList();

        /// <inheritdoc />
        public string Key { get; private set; }

        /// <inheritdoc />
        public bool ThrowOnException { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public T DefaultValue { get; private set; }

        /// <summary>
        /// Gets or sets the optional validation routine for this setting.
        /// </summary>
        /// <value>The validation.</value>
        public Func<T, bool> Validation { get; set; }

        public ConfigSetting<T> BackupConfigSetting { get; set; }

        public T Get(bool useBackupSetting = true) => Get(useBackupSetting ? BackupConfigSetting : null);

        /// <summary>
        /// Gets the setting value.
        /// </summary>
        /// <returns>T.</returns>
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

        /// <summary>
        /// Tries to get the value for the specified settings key.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns><c>true</c> if value exists and is valid, <c>false</c> otherwise.</returns>
        public bool TryGet(out T val)
        {
            var result = TryConvert(Configuration[Key], out var newVal) && Validation(newVal);

            val = result
                ? newVal
                : DefaultValue;

            return result;
        }

        /// <summary>
        /// Converts the specified value to specified type or default value.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>T.</returns>
        public T Convert(string val)
        {
            TryConvert(val, out var result);

            return result;
        }

        /// <summary>
        /// Tries to convert the specified value to specified type or default value.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="newVal">The new value.</param>
        /// <returns><c>true</c> if value exists and is valid, <c>false</c> otherwise.</returns>
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

        /// <summary>
        /// Sets the application settings to override the default.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <remarks>Default AppSettings is <see cref="ConfigurationManager.AppSettings"/></remarks>
        public static void SetAppSettings(NameValueCollection appSettings)
        {
            foreach (IConfigSetting field in typeof(T)
                                             .GetRuntimeFields()
                                             .Where(x => typeof(IConfigSetting).IsAssignableFrom(x.FieldType))
                                             .Select(fieldInfo => fieldInfo.GetValue(null)))
            {
                field?.GetType()
                     .GetRuntimeProperty("AppConfig")
                     ?.SetValue(field, appSettings);
            }
        }

        //private void SetAppSettings(IConfigurationRoot configuration)
        //{
        //    foreach (IConfigSetting field in typeof(T)
        //                                     .GetRuntimeFields()
        //                                     .Where(x => typeof(IConfigSetting).IsAssignableFrom(x.FieldType))
        //                                     .Select(fieldInfo => fieldInfo.GetValue(null)))
        //    {
        //        field?.GetType()
        //             .GetRuntimeProperty("Configuration")
        //             ?.SetValue(field, configuration);
        //    }
        //}
    }
}
