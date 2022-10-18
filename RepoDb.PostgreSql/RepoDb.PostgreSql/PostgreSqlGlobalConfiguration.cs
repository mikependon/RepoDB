using Npgsql;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="NpgsqlConnection"/> object.
    /// </summary>
    public static partial class PostgreSqlGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for PostgreSql.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UsePostgreSql(this GlobalConfiguration globalConfiguration)
        {
            PostgreSqlBootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
