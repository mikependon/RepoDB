using Microsoft.Data.Sqlite;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="SqliteConnection"/> object.
    /// </summary>
    public static partial class SqliteGlobalConfiguration
    {
        /// <summary>
        /// Initializes all necessary settings for SqLite.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSqlite(this GlobalConfiguration globalConfiguration)
        {
            SqliteBootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
