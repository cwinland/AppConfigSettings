using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using AppConfigSettings.Interfaces;

namespace AppConfigSettings
{
    public abstract class SettingsBase<T> : Dictionary<string, object> where T : new()
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

        /// <summary>
        /// Sets the application settings to override the default.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <remarks>Default AppSettings is <see cref="ConfigurationManager.AppSettings"/></remarks>
        public void SetAppSettings(NameValueCollection appSettings)
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
    }
}
