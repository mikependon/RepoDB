using Microsoft.Data.SqlClient;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="SqlConnection"/> object.
    /// </summary>
    public static partial class SqlServerGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for SQL Server.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSqlServer(this GlobalConfiguration globalConfiguration)
        {
            SqlServerBootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
