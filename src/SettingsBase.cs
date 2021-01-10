using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using AppConfigSettings.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AppConfigSettings
{
    public abstract class SettingsBase<T> : Dictionary<string, object> where T : class, new()
    {
        protected SettingsBase()
        {
            foreach (IConfigSetting field in typeof(T)
                                             .GetRuntimeFields()
                                             .Where(x => typeof(IConfigSetting).IsAssignableFrom(x.FieldType))
                                             .Select(fieldInfo => fieldInfo.GetValue(null)))
            {
                var fieldType = field?.GetType();

                if (fieldType == null)
                {
                    continue;
                }

                var key = fieldType
                          .GetRuntimeProperty("Key")
                          ?.GetValue(field)
                          ?.ToString();

                if (key == null)
                {
                    continue;
                }

                var getMethod = fieldType
                    .GetRuntimeMethod("Get", new[] { typeof(bool), });

                object[] paramArray = { true, };
                var val = getMethod?.Invoke(field, paramArray);

                Add(key, val);
            }
        }

        private static List<KeyValuePair<string, string>> GetAppSettingsList(NameValueCollection appSettings) =>
            appSettings.AllKeys.Select(appKey =>
                                           new KeyValuePair<string, string>(appKey,
                                                                            appSettings[
                                                                                appKey]))
                       .ToList();

        public static IConfigurationRoot Init(NameValueCollection appSettings) =>
            InitConfig(null, new List<NameValueCollection> { appSettings, }, true, true, true);

        private static IConfigurationRoot InitConfig(
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

        ///// <summary>
        ///// Sets the application settings to override the default.
        ///// </summary>
        ///// <param name="appSettings">The application settings.</param>
        ///// <remarks>Default AppSettings is <see cref="ConfigurationManager.AppSettings"/></remarks>
        //public void SetAppSettings(NameValueCollection appSettings)
        //{
        //    foreach (IConfigSetting field in typeof(T)
        //                                     .GetRuntimeFields()
        //                                     .Where(x => typeof(IConfigSetting).IsAssignableFrom(x.FieldType))
        //                                     .Select(fieldInfo => fieldInfo.GetValue(null)))
        //    {
        //        field?.GetType()
        //             .GetRuntimeProperty("AppConfig")
        //             ?.SetValue(field, appSettings);
        //    }
        //}

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
