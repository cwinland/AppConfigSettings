using System.Collections.Generic;
using System.Collections.Specialized;
using AppConfigSettings.Enum;
using Microsoft.Extensions.Configuration;

// ReSharper disable UnusedMemberInSuper.Global

namespace AppConfigSettings.Interfaces
{
    public interface IConfigSetting
    {
        /// <summary>
        /// Sets the application settings.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        void SetAppSettings(NameValueCollection appSettings);

        /// <summary>
        /// Overall Configurations
        /// </summary>
        /// <value>The configuration <see cref="IConfigurationRoot"/>.</value>
        IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Gets or sets the default directory containing the configuration files.
        /// </summary>
        /// <value>The default directory.</value>
        string DefaultDirectory { get; set; }

        /// <summary>
        /// Gets or sets the json files.
        /// </summary>
        /// <value>The json files.</value>
        List<string> JsonFiles { get; set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        string Key { get; }

        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>The scopes <see cref="SettingScopes"/>.</value>
        SettingScopes Scopes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [throw on exception].
        /// </summary>
        /// <value><c>true</c> if [throw on exception]; otherwise, <c>false</c>.</value>
        bool ThrowOnException { get; set; }

        /// <summary>
        /// Adds the default json.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        /// <remarks>Must be called again to update if the environment variable 'ASPNETCORE_ENVIRONMENT' changes.</remarks>
        List<string> AddDefaultJson();
    }
}
