using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;

namespace AppConfigSettings.Interfaces
{
    public interface IConfigSetting
    {
        /// <summary>
        /// Configuration App Settings
        /// </summary>
        NameValueCollection AppConfig { get; set; }

        /// <summary>
        /// Overall Configurations
        /// </summary>
        /// <value>The configuration.</value>
        IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [include environment].
        /// </summary>
        /// <value><c>true</c> if [include environment]; otherwise, <c>false</c>.</value>
        bool IncludeEnvironment { get; set; }

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
        /// Gets or sets a value indicating whether [throw on exception].
        /// </summary>
        /// <value><c>true</c> if [throw on exception]; otherwise, <c>false</c>.</value>
        bool ThrowOnException { get; set; }
    }
}
