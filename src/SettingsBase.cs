using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using AppConfigSettings.Interfaces;

namespace AppConfigSettings
{
    public abstract class SettingsBase<T> : Dictionary<string, object> where T : class, new()
    {
        private const string CONFIG_SETTING_KEY = "Key";
        private const string CONFIG_SETTING_VALUE = "Get";
        private const string CONFIG_SETTING_CONFIG = "AppConfig";

        protected SettingsBase() => ConfigSettings.ForEach(field =>
                                                           {
                                                               var fieldType = field?.GetType();

                                                               if (fieldType == null)
                                                               {
                                                                   return;
                                                               }

                                                               var key = fieldType
                                                                         .GetRuntimeProperty(CONFIG_SETTING_KEY)
                                                                         ?.GetValue(field)
                                                                         ?.ToString();

                                                               if (key == null)
                                                               {
                                                                   return;
                                                               }

                                                               var getMethod = fieldType
                                                                   .GetRuntimeMethod(CONFIG_SETTING_VALUE,
                                                                       new[] { typeof(bool), });

                                                               object[] paramArray = { true, };
                                                               var val = getMethod?.Invoke(field, paramArray);

                                                               Add(key, val);
                                                           });

        private static List<object> ConfigSettings => typeof(T)
                                                      .GetRuntimeFields()
                                                      .Where(fieldInfo =>
                                                                 typeof(IConfigSetting).IsAssignableFrom(
                                                                     fieldInfo.FieldType) &&
                                                                 fieldInfo.GetValue(null) != null)
                                                      .Select(fieldInfo => fieldInfo.GetValue(null))
                                                      .ToList();

        /// <summary>
        /// Sets the application settings to override the default.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <remarks>Default AppSettings is <see cref="ConfigurationManager.AppSettings"/></remarks>
        public static void SetAppSettings(NameValueCollection appSettings) => ConfigSettings.ForEach(
            field => field.GetType()
                          .GetRuntimeProperty(CONFIG_SETTING_CONFIG)
                          ?.SetValue(field, appSettings));
    }
}
