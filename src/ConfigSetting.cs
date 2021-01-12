using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using AppConfigSettings.Enum;
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
        /// <param name="scope">Determines the scope for this setting.</param>
        public ConfigSetting(string key, T defaultValue, SettingScopes scope = SettingScopes.Any)
        {
            Key = key;
            DefaultValue = defaultValue;
            Validation = _ => true;
            ThrowOnException = false;
            Scopes = scope;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="scope">Determines the scope for this setting.</param>
        /// <param name="fallbackSetting">The fallback setting.</param>
        public ConfigSetting(string key, T defaultValue, SettingScopes scope, ConfigSetting<T> fallbackSetting) :
            this(key, defaultValue, scope) => BackupConfigSetting = fallbackSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="scope">Determines the scope for this setting.</param>
        /// <param name="validation">Validation for the setting.</param>
        /// <param name="throwOnException"></param>
        public ConfigSetting(
            string key, T defaultValue, SettingScopes scope, Func<T, bool> validation,
            bool throwOnException = false) : this(key,
                                                  defaultValue,
                                                  scope)
        {
            validation ??= _ => true;
            Validation = validation;
            ThrowOnException = throwOnException;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="scope">Determines the scope for this setting.</param>
        /// <param name="validation">The validation.</param>
        /// <param name="throwOnException">if set to <c>true</c> [throw on exception].</param>
        /// <param name="fallbackConfigSetting">The fallback configuration setting.</param>
        public ConfigSetting(
            string key, T defaultValue, SettingScopes scope, Func<T, bool> validation, bool throwOnException,
            ConfigSetting<T> fallbackConfigSetting) : this(key,
                                                           defaultValue,
                                                           scope,
                                                           validation,
                                                           throwOnException) =>
            BackupConfigSetting = fallbackConfigSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="scope">Determines the scope for this setting.</param>
        /// <param name="validation">The validation.</param>
        /// <param name="throwOnException">if set to <c>true</c> [throw on exception].</param>
        /// <param name="fallbackConfigSetting">The fallback configuration setting.</param>
        /// <param name="jsonFiles">The json files.</param>
        public ConfigSetting(
            string key, T defaultValue, SettingScopes scope, Func<T, bool> validation, bool throwOnException,
            ConfigSetting<T> fallbackConfigSetting, List<string> jsonFiles) : this(key,
            defaultValue,
            scope,
            validation,
            throwOnException,
            fallbackConfigSetting) => JsonFiles = jsonFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="scope">Determines the scope for this setting.</param>
        /// <param name="validation">The validation.</param>
        /// <param name="throwOnException">if set to <c>true</c> [throw on exception].</param>
        /// <param name="fallbackConfigSetting">The fallback configuration setting.</param>
        /// <param name="jsonFiles">The json files.</param>
        /// <param name="defaultDirectory">The default directory.</param>
        public ConfigSetting(
            string key, T defaultValue, SettingScopes scope, Func<T, bool> validation, bool throwOnException,
            ConfigSetting<T> fallbackConfigSetting, List<string> jsonFiles,
            string defaultDirectory) : this(key,
                                            defaultValue,
                                            scope,
                                            validation,
                                            throwOnException,
                                            fallbackConfigSetting,
                                            jsonFiles) => DefaultDirectory = defaultDirectory;

        /// <inheritdoc />
        public List<string> JsonFiles { get; set; }

        /// <inheritdoc />
        public string DefaultDirectory { get; set; } = Directory.GetCurrentDirectory();

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
                                                              HasScope(SettingScopes.AppSettings)
                                                                  ? new List<NameValueCollection> { AppConfig, }
                                                                  : new List<NameValueCollection>(),
                                                              HasScope(SettingScopes.Environment),
                                                              HasScope(SettingScopes.Json) &&
                                                              (JsonFiles == null ||
                                                               JsonFiles.Count == 0),
                                                              false,
                                                              DefaultDirectory);

        /// <inheritdoc />
        public string Key { get; private set; }

        /// <inheritdoc />
        public bool ThrowOnException { get; set; }

        public SettingScopes Scopes { get; set; }

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
            bool includeEnvironmentVariables, bool addDefaultJson, bool addDefaultAppSettings, string currentDirectory)
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

                if (!string.IsNullOrWhiteSpace(env))
                {
                    jsonFiles.Add($"{APP_SETTINGS_NAME}.{env}.{APP_SETTINGS_EXT}");
                }
            }

            var inMemoryCollection = appSettingsCollections
                                     .Select(GetKeyPairSettings)
                                     .ToList();

            return BuildConfig(currentDirectory, inMemoryCollection, jsonFiles, includeEnvironmentVariables);
        }

        /// <summary>
        /// Builds the configuration.
        /// </summary>
        /// <param name="currentDirectory">The current directory.</param>
        /// <param name="inMemoryCollection">The in memory collection.</param>
        /// <param name="jsonFiles">The json files.</param>
        /// <param name="includeEnvironmentVariables">if set to <c>true</c> [include environment variables].</param>
        /// <returns>IConfigurationRoot.</returns>
        /// <remarks>Order added to the builder matters. Last added is the tie breaker.</remarks>
        private static IConfigurationRoot BuildConfig(
            string currentDirectory, List<List<KeyValuePair<string, string>>> inMemoryCollection,
            List<string> jsonFiles, bool includeEnvironmentVariables)
        {
            var builder = new ConfigurationBuilder();
            builder.Sources.Clear();
            builder.SetBasePath(currentDirectory);

            if (includeEnvironmentVariables)
            {
                builder.AddEnvironmentVariables();
            }

            inMemoryCollection.ForEach(list => builder.AddInMemoryCollection(list));

            jsonFiles.ForEach(list => builder.AddJsonFile(list, true, true));

            return builder.Build();
        }

        private static List<KeyValuePair<string, string>> GetKeyPairSettings(NameValueCollection appSettings) =>
            appSettings.AllKeys.Select(appKey =>
                                           new KeyValuePair<string, string>(appKey,
                                                                            appSettings[
                                                                                appKey]))
                       .ToList();

        private bool HasScope(SettingScopes scope) => Scopes == SettingScopes.Any || Scopes.HasFlag(scope);
    }
}
