using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb.SqlServer
{
    /// <summary>
    /// A class used to initialize necessary objects that is connected to both <see cref="Microsoft.Data.SqlClient.SqlConnection"/> and <see cref="System.Data.SqlClient.SqlConnection"/> object.
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
            var dbSetting = new SqlServerDbSetting();
            DbSettingMapper.Add(typeof(Microsoft.Data.SqlClient.SqlConnection), dbSetting, true);
            DbSettingMapper.Add(typeof(System.Data.SqlClient.SqlConnection), dbSetting, true);

            // Map the DbHelper
            var dbHelper = new SqlServerDbHelper();
            DbHelperMapper.Add(typeof(Microsoft.Data.SqlClient.SqlConnection), dbHelper, true);
            DbHelperMapper.Add(typeof(System.Data.SqlClient.SqlConnection), dbHelper, true);

            // Map the Statement Builder
            var statementBuilder = new SqlServerStatementBuilder(dbSetting);
            StatementBuilderMapper.Add(typeof(Microsoft.Data.SqlClient.SqlConnection), statementBuilder, true);
            StatementBuilderMapper.Add(typeof(System.Data.SqlClient.SqlConnection), statementBuilder, true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
