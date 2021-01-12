using System;

namespace AppConfigSettings.Enum
{
    [Flags]
    public enum SettingScopes
    {
        Any = 0,
        AppSettings = 1,
        Json = 2,
        Environment = 4,
    }
}
