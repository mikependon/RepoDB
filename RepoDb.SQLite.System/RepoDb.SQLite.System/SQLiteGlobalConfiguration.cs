using System.Data.SQLite;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="SQLiteConnection"/> object.
    /// </summary>
    public static partial class SQLiteGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for SqLite.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseSQLite(this GlobalConfiguration globalConfiguration)
        {
            SQLiteBootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
