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

        /// <inheritdoc />
        public ConfigSetting<T> BackupConfigSetting { get; set; }

        /// <inheritdoc />
        public string DefaultDirectory { get; set; } = Directory.GetCurrentDirectory();

        /// <inheritdoc />
        public T DefaultValue { get; private set; }

        /// <inheritdoc />
        public List<string> JsonFiles { get; set; }

        /// <inheritdoc />
        public string Key { get; private set; }

        /// <inheritdoc />
        public Func<SelectedSetting<T>, bool> ProcessSettingValue { get; set; }

        public SettingScopes Scopes { get; set; }

        /// <inheritdoc />
        public bool ThrowOnException { get; set; }

        /// <inheritdoc />
        public Func<T, bool> Validation { get; set; }

        private List<KeyValuePair<string, string>> AppConfig { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">Optional default value when key is not found or validation fails.</param>
        /// <param name="scope">Optional <see cref="SettingScopes"/> available to this <see cref="ConfigSetting{T}"/>.</param>
        public ConfigSetting(string key, T defaultValue = default, SettingScopes scope = SettingScopes.Any)
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
        /// <param name="defaultValue">Default value when key is not found or validation fails.</param>
        /// <param name="scope"><see cref="SettingScopes"/> available to this <see cref="ConfigSetting{T}"/>.</param>
        /// <param name="fallbackSetting">The fallback <see cref="ConfigSetting{T}"/>.</param>
        public ConfigSetting(
            string key,
            T defaultValue,
            SettingScopes scope,
            ConfigSetting<T> fallbackSetting) :
            this(key, defaultValue, scope) => BackupConfigSetting = fallbackSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">Default value when key is not found or validation fails.</param>
        /// <param name="scope"><see cref="SettingScopes"/> available to this <see cref="ConfigSetting{T}"/>.</param>
        /// <param name="validation">Validation for the <see cref="ConfigSetting{T}"/>.</param>
        /// <param name="throwOnException">if set to <c>true</c> throw an error when validation fails. Otherwise, the default value is used.</param>
        public ConfigSetting(
            string key,
            T defaultValue, SettingScopes scope,
            Func<T, bool> validation,
            bool throwOnException = false) : this(key,
                                                  defaultValue,
                                                  scope)
        {
            if (validation == null)
            {
                validation = _ => true;
            }

            Validation = validation;
            ThrowOnException = throwOnException;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSetting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">Default value when key is not found or validation fails.</param>
        /// <param name="scope"><see cref="SettingScopes"/> available to this <see cref="ConfigSetting{T}"/>.</param>
        /// <param name="validation">Validation for the <see cref="ConfigSetting{T}"/>.</param>
        /// <param name="throwOnException">if set to <c>true</c> throw an error when validation fails. Otherwise, the default value is used.</param>
        /// <param name="fallbackConfigSetting">The fallback <see cref="ConfigSetting{T}"/>.</param>
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
        /// <param name="defaultValue">Default value when key is not found or validation fails.</param>
        /// <param name="scope"><see cref="SettingScopes"/> available to this <see cref="ConfigSetting{T}"/>.</param>
        /// <param name="validation">Validation for the <see cref="ConfigSetting{T}"/>.</param>
        /// <param name="throwOnException">if set to <c>true</c> throw an error when validation fails. Otherwise, the default value is used.</param>
        /// <param name="fallbackConfigSetting">The fallback <see cref="ConfigSetting{T}"/>.</param>
        /// <param name="defaultDirectory">The default directory containing the configuration settings.</param>
        public ConfigSetting(
            string key, T defaultValue, SettingScopes scope, Func<T, bool> validation, bool throwOnException,
            ConfigSetting<T> fallbackConfigSetting,
            string defaultDirectory) : this(key,
                                            defaultValue,
                                            scope,
                                            validation,
                                            throwOnException,
                                            fallbackConfigSetting) => DefaultDirectory = defaultDirectory;

        /// <inheritdoc />
        public IConfigurationRoot Configuration => InitConfig(JsonFiles,
                                                              HasScope(SettingScopes.AppSettings)
                                                                  ? AppConfig ??
                                                                    GetKeyPairSettings(ConfigurationManager.AppSettings)
                                                                  : new List<KeyValuePair<string, string>>(),
                                                              HasScope(SettingScopes.Environment),
                                                              HasScope(SettingScopes.Json) &&
                                                              (JsonFiles == null ||
                                                               JsonFiles.Count == 0),
                                                              false,
                                                              DefaultDirectory);

        /// <inheritdoc />
        public T Get(bool useBackupSetting = true) => Get(useBackupSetting ? BackupConfigSetting : null);

        /// <inheritdoc />
        public T Get(ConfigSetting<T> backupConfigSetting) => TryGet(out var appSetting) &&
                                                              Validation(appSetting)
            ? SettingFound(appSetting, this, DefaultValue)
            : backupConfigSetting == null ||
              !backupConfigSetting.TryGet(out var backupVal)
                ? DefaultValue
                : SettingFound(backupVal, backupConfigSetting, DefaultValue);

        /// <inheritdoc />
        public bool TryGet(out T val)
        {
            var canConvert = TryConvert(Configuration[Key], out var newVal);
            var isValidated = canConvert && Validation(newVal);

            if (!isValidated && ThrowOnException)
            {
                if (canConvert)
                {
                    throw new InvalidDataException();
                }

                throw new InvalidCastException();
            }

            val = isValidated
                ? newVal
                : DefaultValue;

            return isValidated;
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

        /// <inheritdoc />
        public void SetAppSettings(NameValueCollection appSettings) => AppConfig =
            new List<KeyValuePair<string, string>>(1) { new KeyValuePair<string, string>(Key, appSettings[Key]), };

        private T SettingFound(T setting, IConfigSetting validSetting, T defaultValue) =>
            ProcessSettingValue?.Invoke(new SelectedSetting<T>(setting, validSetting)) ?? true
                ? setting
                : defaultValue;

        private static IConfigurationRoot InitConfig(
            List<string> jsonFiles, List<KeyValuePair<string, string>> appSettingsCollections,
            bool includeEnvironmentVariables, bool addDefaultJson, bool addDefaultAppSettings, string currentDirectory)
        {
            var env = Environment.GetEnvironmentVariable(ASP_ENVIRONMENT);

            if (jsonFiles == null)
            {
                jsonFiles = new List<string>();
            }

            if (appSettingsCollections == null)
            {
                appSettingsCollections = new List<KeyValuePair<string, string>>();
            }

            if (addDefaultAppSettings)
            {
                GetKeyPairSettings(ConfigurationManager.AppSettings)
                    .ForEach(x => appSettingsCollections.Add(new KeyValuePair<string, string>(x.Key, x.Value)));
            }

            if (addDefaultJson)
            {
                jsonFiles.Add($"{APP_SETTINGS_NAME}.{APP_SETTINGS_EXT}");

                if (!string.IsNullOrWhiteSpace(env))
                {
                    jsonFiles.Add($"{APP_SETTINGS_NAME}.{env}.{APP_SETTINGS_EXT}");
                }
            }

            return BuildConfig(currentDirectory, appSettingsCollections, jsonFiles, includeEnvironmentVariables);
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
            string currentDirectory, IEnumerable<KeyValuePair<string, string>> inMemoryCollection,
            List<string> jsonFiles, bool includeEnvironmentVariables)
        {
            var builder = new ConfigurationBuilder();
            builder.Sources.Clear();
            builder.SetBasePath(currentDirectory);

            if (includeEnvironmentVariables)
            {
                builder.AddEnvironmentVariables();
            }

            builder.AddInMemoryCollection(inMemoryCollection);

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
