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
        private const string CONFIG_SETTING_VALUE = "Get";

        protected SettingsBase() => ConfigFields.ForEach(fieldInfo =>
                                                         {
                                                             var field = fieldInfo.GetValue(null) as IConfigSetting;
                                                             var fieldType = field?.GetType();

                                                             if (fieldType == null)
                                                             {
                                                                 return;
                                                             }

                                                             var key = ConfigFields
                                                                       .Where(x => x.FieldHandle ==
                                                                                  fieldInfo.FieldHandle)
                                                                       .Select(x => x.Name)
                                                                       .First();

                                                             if (key == null ||
                                                                 ContainsKey(key))
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

        /// <summary>
        /// Sets the application settings to override the default.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <remarks>Default AppSettings is <see cref="ConfigurationManager.AppSettings"/></remarks>
        public static void SetAppSettings(NameValueCollection appSettings) => ConfigFields
                                                                              .Select(x => x.GetValue(null) as
                                                                                  IConfigSetting)
                                                                              .ToList()
                                                                              .ForEach(
                                                                                  field => field.SetAppSettings(
                                                                                      appSettings));

        private static List<FieldInfo> ConfigFields => typeof(T).GetRuntimeFields()
                                                                .Where(fieldInfo =>
                                                                           typeof(IConfigSetting)
                                                                               .IsAssignableFrom(fieldInfo.FieldType) &&
                                                                           fieldInfo.GetValue(null) != null)
                                                                .ToList();
    }
}
