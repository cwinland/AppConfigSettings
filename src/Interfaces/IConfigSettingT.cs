using System;

namespace AppConfigSettings.Interfaces
{
    public interface IConfigSetting<T> : IConfigSetting
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>T.</returns>
        T Convert(string val);

        /// <summary>
        /// Gets the specified use backup setting.
        /// </summary>
        /// <param name="useBackupSetting">if set to <c>true</c> [use backup setting].</param>
        /// <returns>T.</returns>
        T Get(bool useBackupSetting = true);

        /// <summary>
        /// Gets the specified backup configuration setting.
        /// </summary>
        /// <param name="backupConfigSetting">The backup configuration setting.</param>
        /// <returns>T.</returns>
        T Get(ConfigSetting<T> backupConfigSetting);

        /// <summary>
        /// Tries the convert.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="newVal">The new value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool TryConvert(string val, out T newVal);

        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool TryGet(out T val);

        /// <summary>
        /// Gets or sets the backup configuration setting.
        /// </summary>
        /// <value>The backup configuration setting.</value>
        ConfigSetting<T> BackupConfigSetting { get; set; }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>The default value.</value>
        T DefaultValue { get; }

        /// <summary>
        /// Gets or sets the validation.
        /// </summary>
        /// <value>The validation.</value>
        Func<T, bool> Validation { get; set; }
    }
}
