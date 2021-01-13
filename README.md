# AppConfigSettings

Multiple Source Configuration Reader to manage validated and strongly typed application configuration settings from App.Config, AppSettings.json, and environment variables.

## Feature Overview

1. Typed Configuration Settings.
2. Configuration Settings Multiple sources.
   - App.Config.
   - AppSettings.json.
   - AppSettings.Environment.json (environment based).
   - Custom json files.
   - Environment variables.
3. Configuration Settings Validation.
   - Type Validation.
   - Custom Validation.
4. Fallback settings (if settings doesn't exist, check the 'fallback' setting).
5. Exception Handling (throwing errors on validation issues is optional).
6. Default Values.
7. Scope Restrictions - Setting focuses on specific source, if duplicate values are found.

## Available Method Calls

### Reading Settings

```
public T Get(bool useBackupSetting = true)
public T Get(ConfigSetting<T> backupConfigSetting)
public bool TryGet(out T val)
```

### Setting Conversion

```
public T Convert(string val)
public bool TryConvert(string val, out T newVal)
```

## Usage

### Get

Get allows retrieval of the setting, respecting the fallback value, but using the default value if the config value and back values fail.

#### Get Settings with backup and defaults

```
var LogLevel = Settings.LogLevel.Get();
var Path = Settings.DefaultRunbookFolder.Get();
var maxRetries = Settings.MaxRetries.Get();
```

#### Get Settings Without Fallback Setting

```
var LogLevel = Settings.LogLevel.Get(false);
```

#### Get Settings With Custom Fallback Setting

```
var Path = Settings.DefaultRunbookFolder.Get(Settings.SystemRoot);
var maxRetries = Settings.MaxRetries.Get(new ConfigSetting<int>("OtherRetry", 2));
```

## Example Settings

```
public class Settings : SettingsBase<Settings>
{
    public static readonly ConfigSetting<string> DefaultRunbookFolder =
        new ConfigSetting<string>("DefaultRunbookFolder",
                                    Directory.GetCurrentDirectory(),
                                    SettingScopes.Any,
                                    Directory.Exists);

    public static readonly ConfigSetting<int> MaxRetries =
        new ConfigSetting<int>("MaxRetries", 2, SettingScopes.Any, i => i > 0);

    public static readonly ConfigSetting<LoggingLevels> AppLoggingLevel =
        new ConfigSetting<LoggingLevels>("AppLoggingLevel", LoggingLevels.None, SettingScopes.AppSettings);

    public static readonly ConfigSetting<LoggingLevels> LogLevel =
        new ConfigSetting<LoggingLevels>("Logging:LogLevel:Default",
                                            LoggingLevels.Information,
                                            SettingScopes.Json,
                                            AppLoggingLevel);

    public static readonly ConfigSetting<string> AllowedHosts =
        new ConfigSetting<string>("AllowedHosts", "None", SettingScopes.AppSettings | SettingScopes.Json);

    public static readonly ConfigSetting<string> TestSetting = new ConfigSetting<string>("TestSetting", "None");

    public static readonly ConfigSetting<string> SystemRoot =
        new ConfigSetting<string>("SystemRoot",
                                    "None",
                                    SettingScopes.AppSettings,
                                    null,
                                    false,
                                    null,
                                    Environment.CurrentDirectory);

    public static readonly ConfigSetting<string> SystemRoot2 =
        new ConfigSetting<string>("SystemRoot", "None", SettingScopes.Environment);

```

| **Setting** | **Data Type** | **Scope** | **Validation** | **Default** | **Fallback Value** |
| - | - | - | - | - | - |
| DefaultRunbookFolder | String | Any | Directory existence | Current Directory | None | |
| MaxRetries | Integer | Any | MaxRetries > 0 | None | None | |
| AppLoggingLevel | LoggingLevel (Enum) | AppSettings | None | None | None | |
| LogLevel | LoggingLevel (Enum) | Json | Default type checking to ensure Enum value is valid. No Custom Validation. | Information | AppLoggingLevel | |
| AllowedHosts | string | AppSettings, Json | None | None | | |
| TestSetting | string | Any (Default) | None | None | None | |
| SystemRoot | String | AppSettings | None | None | None |
| SystemRoot2 | String | Environment | None | None | None |

## MIT License

MIT License

Copyright (c) 2021 Christopher Winland

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
