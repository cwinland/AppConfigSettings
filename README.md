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

## Configuration Settings

Configuration settings are independent of each other. The settings may be used in a static class, an instance class, or as standalone declarations in a variable.

### Scope

The optional scope (defaults to Any) allows the setting to be configured to restrict the search scope to any combination of available configuration type. 

```c#
[Flags]
public enum SettingScopes
{
    Any = 0,
    AppSettings = 1,
    Json = 2,
    Environment = 4,
}
```

### Common Constructors

When initializing a configuration setting, the only **required** parameter is the **Key**.

#### Basic Constructor with Key, Default Value (optional), and Scope (optional)

- An optional default value will allow the setting to use an expected value when the key is not found or fails validation.
- An optional scope allows to set the setting to a default scope.

```c#
public ConfigSetting(string key, T defaultValue = default, SettingScopes scope = SettingScopes.Any);
```

#### Basic Constructor Plus Validation and Fallback Settings

- The fallbackSetting allows the setting to try another configuration setting before failing.
- Validation specifies a particular validation function for the value of the setting.
- ThrowOnException specifies whether to throw an error when validation fails. Otherwise, the default value is used.

```c#
public ConfigSetting(
    string key, T defaultValue, SettingScopes scope, Func<T, bool> validation, bool throwOnException,
    ConfigSetting<T> fallbackConfigSetting);
```

### Reading Settings

```c#
public T Get(bool useBackupSetting = true)
public T Get(ConfigSetting<T> backupConfigSetting)
public bool TryGet(out T val)
```

### Setting Conversion

```c#
public T Convert(string val)
public bool TryConvert(string val, out T newVal)
```

## Usage

### Get

Get allows retrieval of the setting, respecting the fallback value, but using the default value if the config value and back values fail.

#### Get Settings with backup and defaults

Access the static configuration settings from the Settings class.

```c#
var logLevel = Settings.LogLevel.Get();
var path = Settings.DefaultFolder.Get();
var maxRetries = Settings.MaxRetries.Get();
```

Create a configuration setting and use it immediately.

```c#
var val = new ConfigSetting<string>("TestSetting").Get();
```

Create a configuration setting variable and get the value.

```c#
var testSetting = new ConfigSetting<string>("TestSetting", "My Default");
testSetting.Get();
```

Create an instance of the Settings class and access the setting by the name of the variable.

```c#
var settings = new Settings();
var level = settings[nameof(Settings.LogLevel)].ToString();
// Or
var level2 = settings["LogLevel"].ToString();
```

#### Get Settings Without Fallback Setting

To ignore any set fallback settings, pass the boolean, **false**, to the Get method.

```c#
var LogLevel = Settings.LogLevel.Get(false);
```

#### Get Settings With Custom Fallback Setting

To override or use a different configuration setting, pass the configuration setting to the Get method. Either use an existing configuration setting or create a new configuration setting.

```c#
var Path = Settings.DefaultFolder.Get(Settings.SystemRoot);
var maxRetries = Settings.MaxRetries.Get(new ConfigSetting<int>("OtherRetry", 2));
```

## Data Types, Return Values, and Validation

All configuration settings are declared with data types. In the example code below, each setting specifies the data type of the configuration value.

The datatype is validated for proper conversion. The converted data type is passed to the custom validation routine, if supplied.

The return value with Get, TryGet, Convert, and TryConvert will return the data type that is specified.

```c#
new ConfigSetting<int>("IntKey", 1, SettingScopes.Any, number => number > 0);
new ConfigSetting<LogLevels>("LogKey", LogLevels.Information, SettingScopes.Any, level => level != LogLevels.None);
new ConfigSetting<string>("StringKey", string.Empty, SettingScopes.Any, stringSetting => stringSetting != "Foo");
new ConfigSetting<double?>("NullableDoubleKey", 1.5, SettingScopes.Any, number => number.HasValue && number > 0.5);
```

## Example Configuration Values

### Basic Setting

The most basic configuration setting will be the key itself.

```c#
var testSetting = new ConfigSetting<string>("TestSetting");
```

### Basic Setting with a Default Value and All Configuration Sources

This setting gets the value of **TestSetting** "key" from all available configuration sources. If the key is not found, it returns the string **None**.

```c#
var testSetting = new ConfigSetting<string>("TestSetting", "None");
```

### Scope Setting

This setting gets the value of **AllowedHosts** "key" from available _AppSettings_ and _Json_ configuration sources. This setting ignores environment variables. If the key is not found, it returns the string **None**.

```c#
var allowedHosts = new ConfigSetting<string>("AllowedHosts", "None", SettingScopes.AppSettings | SettingScopes.Json);
```

### Validation Setting

Both settings validates the value in different ways. Validation functions are typed as specified in the declaration.

```c#
var maxRetries = new ConfigSetting<int>("MaxRetries", 2, SettingScopes.Any, i => i > 0);
var folder = new ConfigSetting<string>("Folder", @"C:\Test", SettingScopes.Any, Directory.Exists);
```

The first setting validates that the configuration value is _greater (>) than 0._ If the key is _not found_ or the value _fails validation,__ it returns the integer **2**.
The second setting validates that the configuration value exists as a real folder. If the key is _not found_ or the value _fails validation,__ it returns the string **C:\Test**.

## Example Settings

```c#
public class Settings : SettingsBase<Settings>
{
    public static readonly ConfigSetting<LoggingLevels> AppLoggingLevel =
        new ConfigSetting<LoggingLevels>("AppLoggingLevel");

    public static readonly ConfigSetting<int> MaxRetries =
        new ConfigSetting<int>("MaxRetries", 2, SettingScopes.Any, i => i > 0);
}
```

## Release Notes

- Current
  - Remove AppConfig setting from ConfigSetting public interface.
  - Add SetAppSettings(NameValueCollection appSettings) to ConfigSetting public interface.
  - Add Missing ThrowOnException code.
  - Add Action ProcessSettingValue to process data based on found value.

- 1.21.1.1619
  - Convert project to .NET Standard 2.0 for greater compatibility.

- 1.21.1.1602
  - Allow declaring ConfigSetting without a default value - Uses the data type default.
  - Instance dictionary uses the name of the variable instead of the configuration settings Key.
  - Update documentation
  - Bug Fixes

- 1.21.1.1220
  - Add Scoping
  - Bug Fixes

- 1.21.1.1116
  - Original release

## MIT License

[MIT License]https://github.com/cwinland/AppConfigSettings/blob/master/LICENSE
Copyright (c) 2021 Christopher Winland