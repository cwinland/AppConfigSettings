# AppConfigSettings

Manage Validated and Strongly Typed Application Configuration Settings

## Feature Overview

1. Typed Configuration Settings.
2. Configuration Settings Multiple sources.
   1. App.Config.
   2. AppSettings.json.
   3. AppSettings.Environment.json (environment based).
   4. Custom json files.
   5. Environment variables.
3. Configuration Settings Validation.
   1. Type Validation.
   2. Custom Validation.
4. Fallback settings (if settings doesn't exist, check "this" setting).
5. Exception Handling (throwing errors on validation issues is optional).
6. Default Values.

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
        new ConfigSetting<string>("DefaultRunbookFolder", Directory.GetCurrentDirectory(), Directory.Exists);

    public static readonly ConfigSetting<int> MaxRetries = new ConfigSetting<int>("MaxRetries", 2, i => i > 0);

    public static readonly ConfigSetting<LoggingLevels> AppLoggingLevel =
        new ConfigSetting<LoggingLevels>("AppLoggingLevel", LoggingLevels.None);

    public static readonly ConfigSetting<LoggingLevels> LogLevel =
        new ConfigSetting<LoggingLevels>("Logging:LogLevel:Default", LoggingLevels.Information, AppLoggingLevel);

    public static readonly ConfigSetting<string> SystemRoot =
        new ConfigSetting<string>("SystemRoot", "None", null, false, null, new List<string>(), false);

    public static readonly ConfigSetting<string> SystemRoot2 =
        new ConfigSetting<string>("SystemRoot", "None");
}
```

| **Setting** | **Data Type** | **Validation** | **Default** | **Fallback Value** | **Other** |
| - | - | - | - | - | - |
| DefaultRunbookFolder | String | Directory existence | Current Directory | None |  |
| MaxRetries | Integer | MaxRetries > 0 | None | None |  |
| AppLoggingLevel | LoggingLevel (Enum) | None | None | None |  |
| LogLevel | LoggingLevel (Enum) | Default type checking to ensure Enum value is valid. No Custom Validation. | Information | AppLoggingLevel |  |
| SystemRoot | String | None | None | None | No AppSettings JSON / Environment Variables
| SystemRoot2 | String | None | None | None | Uses Default JSON file(s) / Environment Variables |

## MIT License

MIT License

Copyright (c) 2021 Christopher Winland

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
