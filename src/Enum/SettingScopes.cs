using EnhancedEnum;
using EnhancedEnum.Attributes;

namespace AppConfigSettings.Enum
{
    [Flag]
    public sealed class SettingScopes : EnhancedEnum<int, SettingScopes>
    {
        [Value(0)]public static readonly SettingScopes Any = new SettingScopes();

        [Value(1)]public static readonly SettingScopes AppSettings = new SettingScopes();

        [Value(2)]public static readonly SettingScopes Json = new SettingScopes();

        [Value(4)]public static readonly SettingScopes Environment = new SettingScopes();

        // Operators allow conversion to this class from other values.
        public static implicit operator SettingScopes(string value) => Convert(value);
        public static implicit operator SettingScopes(int value) => Convert(value);
    }
}
