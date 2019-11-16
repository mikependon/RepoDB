using Npgsql;
using RepoDb.DbHelpers;
using RepoDb.PostgreSql.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb.PostgreSql
{
    /// <summary>
    /// A class used to initialize necessary objects that is connected to <see cref="NpgsqlConnection"/> object.
    /// </summary>
    public static class Bootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value indicating whether the initialization is completed.
        /// </summary>
        public static bool Initialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes all necessary settings for PostgreSql.
        /// </summary>
        public static void Initialize()
        {
            // Skip if already initialized
            if (Initialized == true)
            {
                return;
            }

            // Map the DbSetting
            DbSettingMapper.Add(typeof(NpgsqlConnection), new PostgreSqlDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add(typeof(NpgsqlConnection), new PostgreSqlDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add(typeof(NpgsqlConnection), new PostgreSqlStatementBuilder(), true);

            // Set the flag
            Initialized = true;
        }

        #endregion
    }
}
