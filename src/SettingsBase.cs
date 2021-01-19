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

        protected SettingsBase() => ConfigFields.ForEach(configFieldInfo => Add(configFieldInfo.Info.Name,
                                                             configFieldInfo.Value.GetType()
                                                                            .GetRuntimeMethod(CONFIG_SETTING_VALUE,
                                                                                new[] { typeof(bool), })
                                                                            ?.Invoke(configFieldInfo.Value,
                                                                                new object[] { true, })));

        /// <summary>
        /// Sets the application settings to override the default.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <remarks>Default AppSettings is <see cref="ConfigurationManager.AppSettings"/></remarks>
        public static void SetAppSettings(NameValueCollection appSettings) => ConfigFields
            .ForEach(configFieldInfo => configFieldInfo.Value.SetAppSettings(appSettings));

        private static List<ConfigFieldInfo> ConfigFields => typeof(T).GetRuntimeFields()
                                                                      .Where(fieldInfo =>
                                                                                 typeof(IConfigSetting)
                                                                                     .IsAssignableFrom(
                                                                                         fieldInfo.FieldType) &&
                                                                                 fieldInfo.GetValue(null) != null)
                                                                      .Select(x => new ConfigFieldInfo(x))
                                                                      .ToList();
    }
}
