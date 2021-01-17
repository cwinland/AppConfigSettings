using System.Reflection;
using AppConfigSettings.Interfaces;

namespace AppConfigSettings
{
    public class ConfigFieldInfo
    {
        public FieldInfo Info { get; }
        public IConfigSetting Value { get; }

        public ConfigFieldInfo(FieldInfo info)
        {
            Info = info;
            Value = info.GetValue(null) as IConfigSetting;
        }
    }
}
