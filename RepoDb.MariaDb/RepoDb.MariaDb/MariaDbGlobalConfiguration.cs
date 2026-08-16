using MySql.Data.MySqlClient;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="MySqlConnection"/> object.
    /// </summary>
    public static partial class MariaDbGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for MariaDb.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMariaDb(this GlobalConfiguration globalConfiguration)
        {
            MariaDbBootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
