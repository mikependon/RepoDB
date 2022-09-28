using MySql.Data.MySqlClient;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="MySqlConnection"/> object.
    /// </summary>
    public static partial class MySqlGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for MySQL.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseMySql(this GlobalConfiguration globalConfiguration)
        {
            MySqlBootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
