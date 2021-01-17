using AppConfigSettings.Interfaces;

namespace AppConfigSettings
{
    public class SelectedSetting<T>
    {
        public string Key { get; }

        public T Value { get; }

        internal SelectedSetting(T value, IConfigSetting configSetting)
        {
            Value = value;
            Key = configSetting.Key;
        }
    }
}
