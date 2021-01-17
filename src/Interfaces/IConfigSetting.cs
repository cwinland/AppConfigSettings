using System.Collections.Generic;
using System.Collections.Specialized;
using AppConfigSettings.Enum;
using Microsoft.Extensions.Configuration;

namespace AppConfigSettings.Interfaces
{
    public interface IConfigSetting
    {
        /// <summary>
        /// Configuration App Settings
        /// </summary>

        //NameValueCollection AppConfig { get; set; }
        void SetAppSettings(NameValueCollection appSettings);

        /// <summary>
        /// Overall Configurations
        /// </summary>
        /// <value>The configuration.</value>
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

        SettingScopes Scopes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [throw on exception].
        /// </summary>
        /// <value><c>true</c> if [throw on exception]; otherwise, <c>false</c>.</value>
        bool ThrowOnException { get; set; }
    }
}
