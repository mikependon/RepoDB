using Microsoft.Data.SqlClient;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb.SqlServer
{
    /// <summary>
    /// A class used to initialize necessary objects that is connected to <see cref="SqlConnection"/> object.
    /// </summary>
    public static class SqlServerBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value indicating whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes all necessary settings for SqlServer.
        /// </summary>
        public static void Initialize()
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            // Map the DbSetting
            DbSettingMapper.Add(typeof(SqlConnection), new SqlServerDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add(typeof(SqlConnection), new SqlServerDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add(typeof(SqlConnection),
                new SqlServerStatementBuilder(DbSettingMapper.Get<SqlConnection>()), true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
