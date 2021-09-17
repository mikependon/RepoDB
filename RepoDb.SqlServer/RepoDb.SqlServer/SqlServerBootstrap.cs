using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to initialize the necessary settings for both the <see cref="Microsoft.Data.SqlClient.SqlConnection"/> and <see cref="System.Data.SqlClient.SqlConnection"/> objects.
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
            DbSettingMapper.Add<Microsoft.Data.SqlClient.SqlConnection>(dbSetting, true);
            DbSettingMapper.Add<System.Data.SqlClient.SqlConnection>(dbSetting, true);

            // Map the DbHelper
            var dbHelper = new SqlServerDbHelper();
            DbHelperMapper.Add<Microsoft.Data.SqlClient.SqlConnection>(dbHelper, true);
            DbHelperMapper.Add<System.Data.SqlClient.SqlConnection>(dbHelper, true);

            // Map the Statement Builder
            var statementBuilder = new SqlServerStatementBuilder(dbSetting);
            StatementBuilderMapper.Add<Microsoft.Data.SqlClient.SqlConnection>(statementBuilder, true);
            StatementBuilderMapper.Add<System.Data.SqlClient.SqlConnection>(statementBuilder, true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
